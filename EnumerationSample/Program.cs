using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Features.Indexed;

namespace EnumerationSample
{
    internal class Program
    {
        public interface ILogger : IDisposable
        {
            void Write(string message);
        }
    
        public class ConsoleLogger : ILogger 
        {
            public void Write(string message)
            {
                Console.WriteLine(message);
            }

            public void Dispose()
            {
                Console.WriteLine("Dispose console logger...");
            }
        }
    
        public class SmsLogger : ILogger
        {
            private readonly Guid instanseId = Guid.NewGuid();
            private readonly string number;

            public SmsLogger()
            {
                this.number = new Random().Next().ToString();
            }
            
            public SmsLogger(string number)
            {
                this.number = number;                
            }

            public void Write(string message)
            {
                Console.WriteLine(string.Format("[{2}] SMS to: {0}, with text: {1}", number, message, instanseId));
            }

            public void Dispose()
            {
                Console.WriteLine("Dispose sms logger...");
            }
        }

        public class Reporting
        {
            // Enumeration
//            private IList<ILogger> loggers;
//
//            public Reporting(IList<ILogger> loggers)
//            {
//                this.loggers = loggers;                
//            }
//            public void Report()
//            {
//                foreach (var logger in loggers)
//                {
//                    logger.Write(string.Format("Hello, this is {0}!", logger.GetType().Name));
//                }
//            }            
            
            // Dictionary
            private IIndex<string, ILogger> loggersDictionary;
            
            public Reporting(IIndex<string, ILogger> loggersDictionary)
            {
                this.loggersDictionary = loggersDictionary;
            }

            public void Report()
            {
                loggersDictionary["sms"].Write("Hello from dictionary of loggers!");
            }
        }
        
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            
            // Enumeration
//            builder.Register(c => new SmsLogger("123")).As<ILogger>();
//            builder.RegisterType<ConsoleLogger>().As<ILogger>();
            
            // Dictionary
            builder.RegisterType<ConsoleLogger>().Keyed<ILogger>("cmd");
            builder.Register(c => new SmsLogger("123")).Keyed<ILogger>("sms");
            builder.RegisterType<Reporting>();

            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
                Console.WriteLine("Done");
            }
        }
    }
}