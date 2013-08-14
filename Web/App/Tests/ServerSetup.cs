using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace EPSCoR.Tests
{
    /*public class ServerSetup
    {
        public static HttpSelfHostConfiguration GetConfiguration(string baseAddress)
        {
            var config = new HttpSelfHostConfiguration(baseAddress);

            var kernel = new StandardKernel();
            kernel.Bind<IResumeStore>().To<resumeStore>();

            config.Routes.MapHttpRoute(
                "DefualtApi", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });
            config.MessageHandlers.Add(new MethodOverrideHandler());
            config.DependencyResolver = new NinjectDepdencyResolver(kernel);

            return config;
        }
    }*/
}
