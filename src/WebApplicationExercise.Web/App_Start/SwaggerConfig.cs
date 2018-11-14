using System.Web.Http;
using WebActivatorEx;
using Swashbuckle.Application;
using WebApplicationExercise.Web;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WebApplicationExercise.Web
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
