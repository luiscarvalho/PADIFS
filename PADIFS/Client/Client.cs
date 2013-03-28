using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Client), "ClientRemote",
            WellKnownObjectMode.Singleton);
            //Server s = (Server)Activator.GetObject(typeof(Server)
             //   , "tcp://localhost:" + 8086 + "/ClienteRemote")        
        }

        public class Client : MarshalByRefObject //, IClient
        {


            public void create(string nome, string msg)
            {
            }

            public void open(string nome, string msg)
            {
            }

            public void close(string nome, string msg)
            {
            }

            public void delete(string nome, string msg)
            {
            }
    }
}
