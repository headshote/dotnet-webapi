using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;
using AutoMapper;
using Unity;
using Unity.Lifetime;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Core.Logging;
using WebApplicationExercise.Core.Managers;
using WebApplicationExercise.Core.Models;
using WebApplicationExercise.Infrastructure.Data;
using WebApplicationExercise.Web.DTO;
using WebApplicationExercise.Web.Resolver;

namespace WebApplicationExercise.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var container = new UnityContainer();

            var providers = config.Services.GetFilterProviders().ToList();
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));
            var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);
            config.Services.Remove(typeof(IFilterProvider), defaultprovider);

            container.RegisterType<ICustomerManager, CustomerManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ILogger, Logger>(new HierarchicalLifetimeManager());
            container.RegisterType<MainDataContext, MainDataContext>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Order, OrderDTO>();
                cfg.CreateMap<Product, ProductDTO>();
                    //.ForMember(dest => dest.Id, opts => opts.Ignore())
                    //.ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name));
            });

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
