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
    [Serializable]
    class Client
    {
        static string clientname;

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(Convert.ToInt32(args[1]));
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Cliente), "ClientRemote",
            WellKnownObjectMode.Singleton);
            Cliente c = new Cliente(args[0]);
            RemotingServices.Marshal(c, "ClientRemote", typeof(Cliente));
            clientname += args[0];
            System.Console.WriteLine("Client " + clientname + " on" + Environment.NewLine);
            System.Console.ReadLine();
        }
    }

    public class Cliente : MarshalByRefObject, IClient
    {
        string cname;

        public Cliente(string clientname)
        {
            this.cname = clientname;
        }

        public void CREATE(string clientname, string filename, int nb_dataservers,
            int read_quorum, int write_quorum, DebugDelegate debug)
        {
            debug("Create : cheguei aqui!" + "\r\n");
            System.Console.WriteLine("Create : cheguei aqui!" + "\r\n");
            IMDServer mdscreate = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:8080/MetaData_Server");
            mdscreate.CREATE(filename, nb_dataservers, read_quorum, write_quorum,debug);
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

        public void COPY(string clientname, string fileregister1, string semantics, string fileregister2, string salt, DebugDelegate debug)
        {
            // Ainda não percebi bem o que são os file-register
        }

        public void DUMP()
        {
            // print values
        }

        public void EXESCRIPT(string clientname, string script)
        {
        }
    }
}
