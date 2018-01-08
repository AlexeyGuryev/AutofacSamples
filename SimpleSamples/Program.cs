using System;
using System.Collections.Generic;
using Autofac;

namespace SimpleSamples
{
    public interface ILogger
    {
        void Write(string message);
    }
    
    public class ConsoleLogger : ILogger 
    {
        public void Write(string message)
        {
            Console.WriteLine(message);
        }
    }
    
    public class EmailLogger : ILogger 
    {
        public void Write(string message)
        {
            Console.WriteLine(string.Format("Email: {0}", message));
        }
    }
    
    public class Engine
    {
        private ILogger logger;
        private int id;

        public Engine(ILogger logger) : this(logger, new Random().Next())
        {          
        }

        public Engine(ILogger logger, int id)
        {
            this.id = id;
            this.logger = logger;
        }

        public void Ahead(int power)
        {
            logger.Write(string.Format("Engine [{0}] started with power: {1}", id, power));
        }
    }

    public class Car
    {
        private ILogger logger;
        private Engine engine;

        public Car(Engine engine)
        {
            this.engine = engine;
            this.logger = new ConsoleLogger();
        }

        public Car(Engine engine, ILogger logger)
        {
            this.logger = logger;
            this.engine = engine;
        }

        public void Go()
        {
            engine.Ahead(100);
            logger.Write("Car going...");
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            
            // Prevent registration from changing default
//            builder.RegisterType<EmailLogger>().As<ILogger>();
//            builder.RegisterType<ConsoleLogger>().As<ILogger>().PreserveExistingDefaults(); // with this we'll get the first variant

            // Custom instantiation
//            var logger = new EmailLogger();
//            builder.RegisterInstance(logger).As<ILogger>();

            builder.RegisterType<ConsoleLogger>().As<ILogger>();
            
            // Custom instantiation with parameter
//            builder.Register<Engine>(c => new Engine(c.Resolve<ILogger>(), 42));

            builder.RegisterType<Engine>();
            
            // Choosing a constructor 
//            builder.RegisterType<Car>().UsingConstructor(typeof(Engine));

            builder.RegisterType<Car>();

            // Generic
            builder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>));
            IContainer container = builder.Build();
            
            var car = container.Resolve<Car>();
            car.Go();
            
            // Check generic
            var list = container.Resolve<IList<int>>();
            Console.WriteLine(list.GetType());
        }
    }
}