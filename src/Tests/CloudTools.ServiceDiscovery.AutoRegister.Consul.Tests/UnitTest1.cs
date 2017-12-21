using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CloudTools.ServiceDiscovery.AutoRegister.Consul.Tests.Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Xunit;

namespace CloudTools.ServiceDiscovery.AutoRegister.Consul.Tests
{
    public class UnitTest1:IDisposable
    {
        private ConsulProcessWrapper wrapper;

        public UnitTest1()
        {
            wrapper = new ConsulProcessWrapper();
            wrapper.Start();
        }


        [Fact]
        public void validate_serivce_is_properly_registered()
        {
            RegisterService();

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:8500");

            var result = client.GetAsync("v1/catalog/services").Result;

            var response = result.Content.ReadAsStringAsync().Result;

            dynamic obj = JsonConvert.DeserializeObject(response);

            string actual = obj["school-api"][0].ToString();

            Assert.Equal("Test", actual );


        }

        private static void RegisterService()
        {
            var webHostBuilder = new WebHostBuilder();

            var Features = new FeatureCollection();
            var _serverAddresses = new ServerAddressesFeature();
            Features.Set<IServerAddressesFeature>(_serverAddresses);

            _serverAddresses.Addresses.Add("http://localhost");

            var _server = new TestServer(webHostBuilder.UseStartup<UnitTest1>(), Features);

            var _client = _server.CreateClient();

            IServiceCollection services = new ServiceCollection();

            services.AddOptions();
            services.AddLogging();
            services.AddTransient<IServer, TestServer>(p => _server);

            services.AddTransient(p => Task.CompletedTask);

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var configuration = configurationBuilder.Build();

            services.UseConsul(configuration);

            var provider = services.BuildServiceProvider();

            var hostservice = provider.GetService<IHostedService>();

            Assert.IsType<ConsulHostedService>(hostservice);

            hostservice.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            wrapper.Stop();
        }


        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
        }
    }
}
