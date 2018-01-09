using System;
using Autofac;

namespace AdvancedRegistrationSamples
{
    public interface ILogger
    {
        void Write(string message);
    }
    
    public class SmsLogger : ILogger
    {
        private string phoneNumber;

        public SmsLogger(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
        }

        public void Write(string message)
        {
            Console.WriteLine(string.Format("SMS to {0}: {1}", phoneNumber, message));
        }
    }
    
    internal class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            
            //named parameter
//            builder.RegisterType<SmsLogger>()
//                .As<ILogger>()
//                .WithParameter("phoneNumber", "12345");
            
            //typed parameter
//            builder.RegisterType<SmsLogger>()
//                .As<ILogger>()
//                .WithParameter(new TypedParameter(typeof(string), "123456"));
            
            //resolved parameter
//            builder.RegisterType<SmsLogger>()
//                .As<ILogger>()
//                .WithParameter(new ResolvedParameter(
//                    // predicate
//                    (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
//                    //value accessor
//                    (pi, ctx) => "1234567"));    

            builder.Register((ctx, p) => new SmsLogger(p.Named<string>("phoneNumber")))
                .As<ILogger>();

            var container = builder.Build();

            //var logger = container.Resolve<ILogger>();
            
            var randomNumber = new Random().Next();
            var logger = container.Resolve<ILogger>(new NamedParameter("phoneNumber", randomNumber.ToString()));
            
            logger.Write("message");
        }
    }
}