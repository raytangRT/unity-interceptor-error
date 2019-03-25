using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unity4
{
    public class Program
    {
        public class LogCallHandler : ICallHandler
        {
            public int Order { get; set; }

            public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
            {
                Console.WriteLine("Method Invoked...");

                var result = getNext()(input, getNext);

                Console.WriteLine("Method Completed...");

                return result;
            }
        }

        public class LogAttribute : HandlerAttribute
        {
            public override ICallHandler CreateHandler(IUnityContainer container)
            {
                return container.Resolve<LogCallHandler>();
            }
        }

        public interface IService
        {
            void Execute();
        }

        [Log]
        public class DefaultService : IService
        {
            public void Execute()
            {
                Console.WriteLine("\t Unity4 Service Executed...");
            }
        }

        public static void Main(string[] args)
        {
            var container = new UnityContainer();

            /// Register Services
            var injectionMembers = new List<InjectionMember>
            {
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>()
            };

            container.RegisterType<IService, DefaultService>(injectionMembers.ToArray());

            /// Register Supplements
            container.RegisterType<ICallHandler, LogCallHandler>(nameof(LogCallHandler));

            container.AddNewExtension<Interception>();

            /// Execute the service
            var service = container.Resolve<IService>();

            service.Execute();

            var key = Console.ReadKey();
        }
    }
}
