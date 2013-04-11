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
        private List<string> dsrequests = new List<string>();
        private int numServer = 0;
        private int freezeServer = 0;
        private int failServer = 0;
        private string serverpath;
        private string filename;
        private byte[] filecontent;
        private int fileversion = 0;
        private DebugDelegate debug;

        public DServer(DebugDelegate debug)
        {
            dserver_name += numServer;
            freezeServer = 0;
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
            if (failServer == 0)
            {
                if (freezeServer == 0)
                {
                    foreach (string fname in Directory.GetFiles(serverpath))
                    {
                        if (fname.Equals(filename))
                        {
                            System.Console.WriteLine("Encontrei ficheiro!");
                            File.OpenRead(serverpath += filename);
                            File.ReadAllLines(serverpath += filename);
                        }
                    }
                }
                else
                {
                    dsrequests.Add("READ/" + filename + "/" + semantics);
                }
                debug("File" + filename + "opened for read purposes");
            }
            else
            {
                debug("Server" + dserver_name + "has failed");
            }
        }

        public void WRITE(string filename, byte[] content, DebugDelegate debug)
        {
            if (failServer == 0)
            {
                if (freezeServer == 1)
                {
                    foreach (string fname in Directory.GetFiles(serverpath))
                    {
                        if (fname.Equals(filename))
                        {
                            File.AppendAllText(serverpath += filename, content.ToString());
                        }
                    }
                    File.WriteAllText(serverpath += filename, content.ToString());
                }
                else
                {
                    dsrequests.Add("WRITE/" + filename + "/" + content.ToString());
                }
                debug("File" + filename + "opened for write purposes");
            }
            else
            {
                debug("Server" + dserver_name + "has failed");
            }
        }

        public void FREEZE()
        {
            if (failServer == 0)
            {
                freezeServer = 1;
                debug("Requests holded.");
            }
            else
            {
                debug("Server" + dserver_name + "has failed");
            }
        }

        public void UNFREEZE()
        {
            if (failServer == 0)
            {
                foreach (string request in dsrequests)
                {
                    string[] req = request.Split('/');

                    if (req[0].Equals("READ"))
                    {
                        READ(req[1], req[2], new DebugDelegate(debug));
                    }
                    else if (req[0].Equals("WRITE"))
                    {
                        WRITE(req[1], System.Text.Encoding.UTF8.GetBytes(req[2]), new DebugDelegate(debug));
                    }
                }
                debug("Dealt with Requests.");
            }
            else
            {
                debug("Server" + dserver_name + "has failed");
            }
        }

        public void FAIL()
        {
            failServer = 1;
            debug("Server " + dserver_name + "has failed.");
        }

        public void RECOVER()
        {
            failServer = 0;
            debug("Server " + dserver_name + "is back online.");
        }
    }
}
