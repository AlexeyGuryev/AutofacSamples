using System;
using System.Collections.Generic;
using Autofac;

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
            private IList<ILogger> loggers;

            public Reporting(IList<ILogger> loggers)
            {
                this.loggers = loggers;
            }

            public void Report()
            {
                foreach (var logger in loggers)
                {
                    logger.Write(string.Format("Hello, this is {0}!", logger.GetType().Name));
                }
            }
        }
        
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.Register(c => new SmsLogger("123")).As<ILogger>();
            builder.RegisterType<ConsoleLogger>().As<ILogger>();
            builder.RegisterType<Reporting>();

            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
                Console.WriteLine("Done");
            }
        }
    }
}