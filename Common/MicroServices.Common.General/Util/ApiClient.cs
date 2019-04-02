﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace MicroServices.Common.General.Util
{
    /// <summary>
    /// Simple API Client class
    /// </summary>
    public static class ApiClient
    {
        private static readonly string DefaultContentType = "text/json";

        /// <summary>
        /// Load the specified url.
        /// </summary>
        /// <returns>The load.</returns>
        /// <param name="uri">URL.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T Load<T>(string uri)
        {
            T result = default(T);
            var webRequest = CreateRequest(uri, HttpMethod.Get);

            var response = webRequest.GetResponse();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var stringResult = streamReader.ReadToEnd();
                result = JsonConvert.DeserializeObject<T>(stringResult);
            }
            return result;
        }

        public static IEquatable<T> LoadMany<T>(string uri)
        {
            return Load<IEquatable<T>>(uri);
        }

        private static WebRequest CreateRequest(string uri, HttpMethod method, string contentType = "text/json")
        {
            var webRequest = WebRequest.Create(uri);
            webRequest.ContentType = contentType ?? DefaultContentType;
            webRequest.Method = method.Method;

            return webRequest;
        }
    }
}