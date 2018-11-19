using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Filters;
using AutoMapper;
using Microsoft.Web.Http;
using Unity;
using Unity.Lifetime;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Core.Managers;
using WebApplicationExercise.Core.Models;
using WebApplicationExercise.Infrastructure.Data;
using WebApplicationExercise.Infrastructure.Logging;
using WebApplicationExercise.Web.DTO;
using WebApplicationExercise.Web.Resolver;

namespace WebApplicationExercise.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
            });

            log4net.Config.XmlConfigurator.Configure();

            var container = new UnityContainer();
            var providers = config.Services.GetFilterProviders().ToList();
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));
            var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);
            config.Services.Remove(typeof(IFilterProvider), defaultprovider);
            container.RegisterType<ICustomerManager, CustomerManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ILogger, Logger>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrdersRepository, OrdersRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<MainDataContext, MainDataContext>(new HierarchicalLifetimeManager());
            config.DependencyResolver = new UnityResolver(container);

            Mapper.Initialize(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AllowNullDestinationValues = true;
                cfg.CreateMap<Order, OrderDTO>()
                    .ForMember(dest => dest.Products, opt => opt.AllowNull())
                    .ForMember(dest => dest.CreatedDate, opts => opts.MapFrom(src => new DateTimeOffset(src.CreatedDate, TimeSpan.Zero)))
                    .ReverseMap()
                    .ForMember(dest => dest.Products, opt => opt.AllowNull())
                    .ForMember(dest => dest.CreatedDate, opts => opts.MapFrom(src => src.CreatedDate.UtcDateTime));
                cfg.CreateMap<Product, ProductDTO>()
                    .ReverseMap()
                    .ForMember(dest => dest.Order, opts => opts.Ignore());
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
