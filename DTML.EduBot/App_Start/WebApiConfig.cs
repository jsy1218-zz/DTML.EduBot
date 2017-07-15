using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Autofac;
using DTML.EduBot.Qna;

namespace DTML.EduBot
{
    public static class WebApiConfig
    {
        public static IContainer Container { get; set; }

        public static void Register(HttpConfiguration config)
        {
            // Json settings
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            RegisterDependencyInjections();
        }

        private static void RegisterDependencyInjections()
        {
            var builder = new ContainerBuilder();

            var QnaService = MakeQnaServiceFromAttributes();
            builder.RegisterInstance(QnaService).As<IQnaService>();

            WebApiConfig.Container = builder.Build();
        }

        private static QnaService MakeQnaServiceFromAttributes()
        {
            var qnaModel = new QnaModelAttribute("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0/",
                "d24b3b5df8b541cabfab6d4b12646ca0", "34aeee3d-51f6-42d4-bcb3-b8e8c1c1b88e");

            return qnaModel == null ? null : new QnaService(qnaModel);
        }
    }
}
