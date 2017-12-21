using System.Collections.Generic;

namespace CloudTools.ServiceDiscovery.AutoRegister.Consul
{
    public class ConsulConfig
    {
        public string HealthCheckPath { get; set; } = "HealthCheck";
        public string Address { get; set; }
        public string ServiceName { get; set; }
        public string ServiceID { get; set; }

        public List<string> Tags { get; set; }
    }
}
