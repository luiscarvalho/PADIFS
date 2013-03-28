using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace MetaData_Server
{
    class Metadata_Server
    {
        static int nserver = 0;

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MDServer), "MetaData_Server",
            WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Metadata Server m - " + nserver +" on");
            nserver++;
            System.Console.ReadLine();
        }
    }

    public class MDServer : MarshalByRefObject, IMDServer

    {

    }
}