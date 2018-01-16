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
            private Owned<ConsoleLogger> ownedLogger;
            private readonly Func<SmsLogger> logger;
            private readonly Func<string, SmsLogger> loggerWithParam;

            public Reporting(Func<SmsLogger> logger, Owned<ConsoleLogger> ownedLogger, Func<string, SmsLogger> loggerWithParam)
            {
                if (logger == null) throw new ArgumentNullException(nameof(logger));
                if (ownedLogger == null) throw new ArgumentNullException(nameof(ownedLogger));
                if (loggerWithParam == null) throw new ArgumentNullException(nameof(loggerWithParam));

                this.logger = logger;
                this.ownedLogger = ownedLogger;
                this.loggerWithParam = loggerWithParam;
            }

            public void Report()
            {
                logger().Write("Hello one!");
                logger().Write("Hello two!");
                loggerWithParam("123").Write("Hello one with param!");
                loggerWithParam("123").Write("Hello two with param!");
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