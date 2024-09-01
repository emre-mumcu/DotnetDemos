using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace src.App_Lib.RedisStack
{
    // When you deploy your application, use TLS and follow the Redis security guidelines.
    public sealed class RedisConnectorTls
    {
        private RedisConnectorTls() { }

        // To convert user certificate and private key from the PEM format to pfx, use this command:
        // openssl pkcs12 -inkey redis_user_private.key -in redis_user.crt -export -out redis.pfx        
        private static bool ValidateServerCertificate(object sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            if (certificate == null)
            {
                return false;
            }

            var ca = new X509Certificate2("redis_ca.pem");
            bool verdict = (certificate.Issuer == ca.Subject);
            if (verdict)
            {
                return true;
            }
            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }

        private static readonly Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            ConfigurationOptions options = new ConfigurationOptions
            {
                EndPoints = { { "localhost", 6379 } },
                User = "default",  // use your Redis user. More info https://redis.io/docs/latest/operate/oss_and_stack/management/security/acl/
                Password = "secret", // use your Redis password
                Ssl = true,
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12
            };

            options.CertificateSelection += delegate
            {
                return new X509Certificate2("redis.pfx", "secret"); // use the password you specified for pfx file
            };

            options.CertificateValidation += ValidateServerCertificate;

            return ConnectionMultiplexer.Connect(options);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }
    }
}