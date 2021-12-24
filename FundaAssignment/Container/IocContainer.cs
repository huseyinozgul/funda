using System;
using System.Linq;
using fundaconsole.Abstractions;
using fundaconsole.Logger;
using fundaconsole.Services;
using Newtonsoft.Json;
using UniCorn.IoC;

namespace fundaconsole.Container
{
    public static class IocContainer
    {
        public static UniIoC Container;
        public static ILogger Logger;


        static IocContainer()
        {
            Container = new UniIoC();
            Container.Register(ServiceCriteria.For<IFeedService>().ImplementedBy<FeedService>().OnIntercepting(OnIntercepting));

            Logger = new ConsoleLogger();
        }

        static void OnIntercepting(IInvocation invocation)
        {
            object request = invocation.MethodParameters.FirstOrDefault();

            try
            {
                invocation.Proceed();

                object response = invocation.ReturnValue;
                var logModel = new { MethodName = invocation.Method.Name, Request = request, Response = response };

                string log = JsonConvert.SerializeObject(logModel);
                Logger.Info(log);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw ex;
            }
        }
    }
}
