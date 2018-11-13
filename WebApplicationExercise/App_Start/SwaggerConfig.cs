using System.Web.Http;
using WebActivatorEx;
using WebApplicationExercise;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WebApplicationExercise
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "WebApplicationExercise");
                        c.IncludeXmlComments(string.Format(@"{0}\bin\WebApplicationExercise.XML", System.AppDomain.CurrentDomain.BaseDirectory));

                        c.PrettyPrint();
                    })
                .EnableSwaggerUi(c =>
                    {
                        c.DocumentTitle("Orders Swagger UI");
                    });
        }
    }
}
