using System;
using Autofac;

namespace ImplicitRelationshipSamples
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
                Console.WriteLine(string.Format("SMS to: {0}, with text: {1}", number, message));
            }

            public void Dispose()
            {
                Console.WriteLine("Dispose sms logger...");
            }
        }

        public class Reporting
        {
            private readonly Func<SmsLogger> logger;

            public Reporting(Func<SmsLogger> logger)
            {
                if (logger == null)
                {
                    throw new ArgumentNullException(nameof(logger));
                }
                
                this.logger = logger;
            }

            public void Report()
            {
                logger().Write("Hello one!");
                logger().Write("Hello two!");
            }
        }
        
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<SmsLogger>();
            builder.RegisterType<ConsoleLogger>();
            builder.RegisterType<Reporting>();

            using (var c = builder.Build())
            {
                c.Resolve<Reporting>().Report();
                c.Resolve<Reporting>().Report();
                Console.WriteLine("Done");
            }
        }
    }
}