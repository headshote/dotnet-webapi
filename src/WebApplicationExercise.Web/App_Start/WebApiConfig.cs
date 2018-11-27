using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Filters;
using System.Web.Http.Routing;
using AutoMapper;
using Microsoft.Web.Http;
using Microsoft.Web.Http.Routing;
using Microsoft.Web.Http.Versioning;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using WebApplicationExercise.Core.Interfaces;
using WebApplicationExercise.Core.Managers;
using WebApplicationExercise.Core.Models;
using WebApplicationExercise.Core.Services;
using WebApplicationExercise.Infrastructure.Data;
using WebApplicationExercise.Infrastructure.Errors;
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
            log4net.Config.XmlConfigurator.Configure();

            var container = new UnityContainer();
            config.DependencyResolver = new UnityResolver(container);
            var providers = config.Services.GetFilterProviders().ToList();
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));
            var defaultprovider = providers.Single(i => i is ActionDescriptorFilterProvider);
            config.Services.Remove(typeof(IFilterProvider), defaultprovider);
            container.RegisterType<ICustomerManager, CustomerManager>(new HierarchicalLifetimeManager());
            container.RegisterType<ILogger, Logger>(new HierarchicalLifetimeManager());
            container.RegisterType<IOrdersRepository, OrdersRepository>(new HierarchicalLifetimeManager());
            container.RegisterType<MainDataContext, MainDataContext>(new HierarchicalLifetimeManager());
            container.RegisterType<IErrorManager, ErrorManager>();
            container.RegisterType<IExchangeRateProvider, ExchangeRateProvider>(new InjectionConstructor(
                ConfigurationManager.AppSettings["exRateServiceAddress"]));

            config.MessageHandlers.Add(new ErrorHandler(config.DependencyResolver.GetService(typeof(IErrorManager)) as IErrorManager));

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
                    .ForMember(dest => dest.Price, opts => opts.MapFrom(src => src.PriceUSD))
                    .ReverseMap()
                    .ForMember(dest => dest.Order, opts => opts.Ignore());
            });

            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler(config.DependencyResolver.GetService(typeof(IErrorManager)) as IErrorManager));

            // Web API routes
            var constraintResolver = new DefaultInlineConstraintResolver()
            {
                ConstraintMap =
                {
                    ["apiVersion"] = typeof( ApiVersionRouteConstraint )
                }
            };
            config.MapHttpAttributeRoutes(constraintResolver);

            config.AddApiVersioning(o =>
            {
                o.ApiVersionReader = new UrlSegmentApiVersionReader();
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
                o.ErrorResponses = new VersioningErrorResponseProvider(config.DependencyResolver.GetService(typeof(IErrorManager)) as IErrorManager);
            });

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v{version:apiVersion}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
