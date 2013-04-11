using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using PADICommonTypes;
using System.IO;

namespace DataServer
{
    class DataServer
    {
        static int dserver = 0;
        static string servername = "d - ";

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8087);
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(DServer), "Data_Server",
            WellKnownObjectMode.Singleton);
            servername += dserver.ToString();
            System.Console.WriteLine("Data Server m - " + dserver + " on");
            dserver++;
            System.Console.ReadLine();
        }

        static void Debug(string mensagem)
        {
            Console.WriteLine(mensagem);
        }
    }

    public class DServer : MarshalByRefObject, IDServer
    {
        private string dserver_name = "d - ";
        private int numServer = 0;
        private string serverpath;
        private string filename;
        private byte[] filecontent;
        private int fileversion = 0;
        private DebugDelegate debug;

        public DServer(DebugDelegate debug)
        {
            dserver_name += numServer;
            serverpath = Directory.GetCurrentDirectory();
            serverpath += Path.Combine(dserver_name);
            if (!Directory.Exists(serverpath))
            {
                Directory.CreateDirectory(serverpath);
            }
            numServer++;
            debug("Data server" + dserver_name + "created.");
        }

        public void READ(string filename, string semantics, DebugDelegate debug)
        { 
            foreach (string fname in Directory.GetFiles(serverpath))
            {
                if(fname.Equals(filename)){
                    System.Console.WriteLine("Encontrei ficheiro!");
                    File.OpenRead(serverpath += filename);
                    File.ReadAllLines(serverpath += filename);
                }
            }
            debug("File" + filename + "opened for read purposes");
        }

        public void WRITE(string filename, byte[] content, DebugDelegate debug)
        {
            foreach (string fname in Directory.GetFiles(serverpath))
            {
                if (fname.Equals(filename))
                {
                    File.AppendAllText(serverpath += filename, content.ToString());
                }
            }
            File.WriteAllText(serverpath += filename, content.ToString());
            debug("File" + filename + "opened for write purposes");
        }

        public void FREEZE()
        {

        }

        public void UNFREEZE()
        {

        }

    }
}
