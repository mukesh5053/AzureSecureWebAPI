using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IO;

namespace DaemonClient
{
    public class AuthConfig
    {
        public string Instance { get; set; }

        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public string Authority { get { return String.Format(CultureInfo.InvariantCulture, Instance, TenantId); } }

        public string ClientSecret { get; set; }

        public string BaseAddress { get; set; }

        public string ResourceId { get; set; }


        public static AuthConfig ReadFromJsonFile(string path)
        {

            IConfiguration Configuration;

            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile(path);

            Configuration = builder.Build();
            return Configuration.Get<AuthConfig>();
        }
    }
}
