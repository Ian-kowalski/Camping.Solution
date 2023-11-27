using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace camping.Database
{
    public class SshConnection
    {
        private SshClient ssh = new SshClient("145.44.233.138", "student", "r2Njj8#4");
        private ForwardedPortLocal port = new ForwardedPortLocal("127.0.0.1", 1433, "localhost", 1433);
        public SshConnection() {
            ssh.Connect();
            ssh.AddForwardedPort(port);
            port.Start();
        }
    }
}
