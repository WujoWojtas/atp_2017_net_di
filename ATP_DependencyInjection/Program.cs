using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATP_DependencyInjection
{
    interface IWorker
    {
        void DoWork(string request);
    }

    interface ILogger
    {
        void Log(string request);
    }

    class Logger : ILogger
    {
        public void Log(string request)
        {
            Console.WriteLine(request);
        }
    }

    class Worker : IWorker
    {
        public void DoWork(string request)
        {
            Console.WriteLine("Some work...");
        }
    }

    class Company
    {
        IWorker worker;
        ILogger logger;

        public Company(IWorker worker, ILogger logger)
        {
            this.worker = worker;
            this.logger = logger;
        }

        public void HandleRequestsFromClients(string request)
        {
            worker.DoWork(request);
            logger.Log(request);
        }
    }

    static class SimpleServiceLocator
    {
        static Dictionary<Type, object> container = new Dictionary<Type, object>()
        {
            {typeof(ILogger), new Logger()},
            {typeof(IWorker), new Worker()}
        };

        public static T GetService<T>() where T : class
        {
            object instance;
            if (!container.TryGetValue(typeof(T), out instance))
            {
                throw new ArgumentException("Service not found");
            }
            return instance as T;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var companyBadWay = new Company(new Worker(),
                                            new Logger());
            //---------------------------------------------------

            var companyServiceLocatorWay = new Company(SimpleServiceLocator.GetService<IWorker>(),
                                                       SimpleServiceLocator.GetService<ILogger>());
            //---------------------------------------------------

            InitializeSimpleInjector();
            var companyServiceAutoDI = simpleInjectorContainer.GetInstance<Company>();
            //---------------------------------------------------

            companyBadWay.HandleRequestsFromClients("Objects created manually");

            Console.WriteLine("##############");
            companyServiceLocatorWay.HandleRequestsFromClients("Objects created with ServiceLocator");

            Console.WriteLine("##############");
            companyServiceAutoDI.HandleRequestsFromClients("Objects created with SimpleInjector (best way!)");

        }

        static Container simpleInjectorContainer;

        static void InitializeSimpleInjector()
        {
            simpleInjectorContainer = new Container();

            simpleInjectorContainer.Register<IWorker, Worker>();
            simpleInjectorContainer.Register<ILogger, Logger>();
            simpleInjectorContainer.Register<Company, Company>();

            simpleInjectorContainer.Verify();
        }
    }
}
