using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions;

namespace CloudTools.ServiceDiscovery.AutoRegister.Consul.Tests.Consul
{
    public class ConsulProcessWrapper
    {
        private object process;

        public void Start()
        {
            var ph = new ProcessHelper();

           process =  ph.LaunchProcess(".\\Consul\\consul.exe","agent -dev", Directory.GetCurrentDirectory(), null,null , null);

            var client = new HttpClient();
            client.BaseAddress = new Uri("http://127.0.0.1:8500");

            bool notrunning = true;

            var count = 0;

                while (notrunning || count ==5)
                {
                    var result = client.GetAsync("v1/status/leader").Result;

                    notrunning = !result.IsSuccessStatusCode;
                    
                    Thread.Sleep(1000);

                    count++;
                }


            if (count == 5)
            {
                throw new ApplicationException();
            }

        }

        public void Stop()
        {
            var ph = new ProcessHelper();

            ph.TerminateProcess(process);
        }
    }
}
