using System;
using EasyNetQ;
using MicroServices.Common.General;
using MicroServices.Common.General.Util;
using MicroServices.Common.MessageBus;
using MicroServices.Common.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Products.Common.Models;
using Products.ReadModels.Service.Views;
using StackExchange.Redis;
using Aggregate = MicroServices.Common.General.Aggregate;


namespace Products.ReadModels.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigureHandlers();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc( routes =>
            {
                routes.MapRoute("DefaultApi", "api/{controller}/{id}");
            });
        }

        private void ConfigureHandlers()
        {
            var redis = ConnectionMultiplexer.Connect("localhost");
            var productView = new ProductView(new RedisRepository<ProductModel>(redis.GetDatabase()));

            var eventMapping = new EventHandlerDiscovery().Scan(productView).Handlers;

            var subscriptionName = "Product_readmodel";
            var topicFilter = "Product.Common.Events";

            var bus = RabbitHutch.CreateBus("host=localhost");

            bus.Subscribe<PublishedMessage>(subscriptionName, m =>
            {
                Aggregate handler;
                var messageType = Type.GetType(m.MessageTypeName);
                var handlerFound = eventMapping.TryGetValue(messageType, out handler);
                if (handlerFound)
                {
                    var @event = JsonConvert.DeserializeObject(m.SerialisedMessage, messageType);
                    handler.AsDynamic().ApplyEvent(@event, ((Event)@event).Version);
                }
            }, q => q.WithTopic(topicFilter));

            ServiceLocator.Bus = new RabbitMqBus(bus);
            ServiceLocator.ProductView = productView;
        }
    }
}
