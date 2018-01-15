using System;
using Autofac;
using Autofac.Features.OwnedInstances;

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
            private Owned<ConsoleLogger> ownedLogger;
            private readonly Func<SmsLogger> logger;

            public Reporting(Func<SmsLogger> logger, Owned<ConsoleLogger> ownedLogger)
            {
                if (logger == null)
                {
                    throw new ArgumentNullException(nameof(logger));
                }

                if (ownedLogger == null)
                {
                    throw new ArgumentNullException(nameof(ownedLogger));                    
                }

                this.logger = logger;
                this.ownedLogger = ownedLogger;
            }

            public void Report()
            {
                logger().Write("Hello one!");
                logger().Write("Hello two!");
            }

            public void ReportOnce()
            {
                ownedLogger.Value.Write("Report once");
                ownedLogger.Dispose();
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
                c.Resolve<Reporting>().ReportOnce();
                Console.WriteLine("Done");
            }
        }
    }
}