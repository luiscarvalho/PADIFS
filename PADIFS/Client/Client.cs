using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using PADICommonTypes;

namespace Client
{
    class Client
    {
        static int nclient = 0;
        static string clientname = "c-";

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Cliente), "ClientRemote",
            WellKnownObjectMode.Singleton);
            clientname += nclient.ToString();
            System.Console.WriteLine("Client c-" + clientname + " on");
            nclient++;
            System.Console.ReadLine();
        }
    }

    public class Cliente : MarshalByRefObject, IClient
    {
        public void CREATE(string clientname, string filename, int nb_dataservers,
            int read_quorum, int write_quorum, DebugDelegate debug)
        {
            IMDServer mdscreate = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:8086/MetaData_Server");
            mdscreate.CREATE(filename, nb_dataservers, read_quorum, write_quorum,new DebugDelegate(debug));
        }
        public void OPEN(string clientname, string filename, DebugDelegate debug)
        {
            IMDServer mdsopen = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:8086/MetaData_Server");
            mdsopen.OPEN(filename,new DebugDelegate(debug));
        }
        public void CLOSE(string clientname, string filename, DebugDelegate debug)
        {
            IMDServer mdsclose = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:8086/MetaData_Server");
            mdsclose.CLOSE(filename, new DebugDelegate(debug));
        }
        public void READ(string clientname, string filename, string semantics, DebugDelegate debug)
        {
            IDServer dsread = (IDServer)Activator.GetObject(typeof(IDServer)
            , "tcp://localhost:8087/Data_Server");
            dsread.READ(filename, semantics, new DebugDelegate(debug));
        }
        public void WRITE(string clientname, string filename, byte[] content, DebugDelegate debug)
        {
            IDServer dswrite = (IDServer)Activator.GetObject(typeof(IDServer)
            , "tcp://localhost:8087/Data_Server");
            dswrite.WRITE(filename, content, new DebugDelegate(debug));
        }
        public void DELETE(string clientname, string filename, DebugDelegate debug)
        {
            IMDServer mdsdelete = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:8086/MetaData_Server");
            mdsdelete.DELETE(filename, new DebugDelegate(debug));
        }
    }
}
