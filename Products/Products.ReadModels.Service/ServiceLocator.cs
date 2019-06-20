using System;
using MicroServices.Common.MessageBus;
using Products.ReadModels.Service.Views;

namespace Products.ReadModels.Service
{
    public static class ServiceLocator
    {
        public static IMessageBus Bus { get; set; }
        public static ProductView ProductView { get; set; }
    }
}
