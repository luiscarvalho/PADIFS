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
    [Serializable]
    class DataServer
    {
        //static int dserver;
        //static string servername;

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(Convert.ToInt32(args[1]));
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(DServer), "Data_Server",
            WellKnownObjectMode.Singleton);

            DServer ds = new DServer(args[0]);
            RemotingServices.Marshal(ds, "Data_Server", typeof(DServer));

            //servername = dserver.ToString();
            //dserver++;
            System.Console.WriteLine("Data Server " + args[0] + " start with port " + args[1]);
            ds.Register(args[1]);
            System.Console.ReadLine();
        }
    }

    public class DServer : MarshalByRefObject, IDServer
    {
        private string dserver_name;
        private string port;
        private List<string> dsrequests = new List<string>();
        private int numServer = 0;
        private int freezeServer;
        private int failServer = 0;
        private string serverpath;
        private string filename;
        private byte[] filecontent;
        private int fileversion = 0;

        public DServer(string dservername)
        {
            this.dserver_name = dservername;
            this.freezeServer = 1;
            this.serverpath = Directory.GetCurrentDirectory();
            this.serverpath += Path.Combine(this.dserver_name);
            if (!Directory.Exists(serverpath))
            {
                Directory.CreateDirectory(serverpath);
            }
            numServer++;
            //debug("Data server" + dserver_name + "created.");
        }

        public void Register(string port)
        {
            IMDServer mdserverRegister = (IMDServer)Activator.GetObject(typeof(IMDServer)
                , "tcp://localhost:8080/MetaData_Server");
            if (mdserverRegister.RegisteDServer(this.dserver_name, port))
            {
                this.freezeServer = 0;
                System.Console.WriteLine("Data Server " + this.dserver_name + " registered");
            }
        }

        public void READ(string filename, string semantics)
        {
            if (failServer == 0)
            {
                if (freezeServer == 0)
                {
                    foreach (string fname in Directory.GetFiles(serverpath))
                    {
                        if (fname.Equals(serverpath + "\\" + filename))
                        {
                            System.Console.WriteLine("Encontrei ficheiro!");
                            System.Diagnostics.Process.Start(serverpath + "\\" + filename);
                            //File.ReadAllLines(serverpath + "\\" + filename);
                        }
                    }
                }
                else
                {
                    dsrequests.Add("READ/" + filename + "/" + semantics);
                }
                //debug("File" + filename + "opened for read purposes");
            }
            else
            {
                //debug("Server" + dserver_name + "has failed");
            }
        }

        public void WRITE(string filename, byte[] content)
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
               // debug("File" + filename + "opened for write purposes");
            }
            else
            {
              //  debug("Server" + dserver_name + "has failed");
            }
        }

        public void FREEZE(string dserver)
        {
            if (failServer == 0)
            {
                freezeServer = 1;
               // debug("Requests holded.");
            }
            else
            {
              //  debug("Server" + dserver_name + "has failed");
            }
        }

        public void UNFREEZE(string dserver)
        {
            if (failServer == 0)
            {
                foreach (string request in dsrequests)
                {
                    string[] req = request.Split('/');

                    if (req[0].Equals("READ"))
                    {
                        READ(req[1], req[2]);
                    }
                    else if (req[0].Equals("WRITE"))
                    {
                        WRITE(req[1], System.Text.Encoding.UTF8.GetBytes(req[2]));
                    }
                }
               // debug("Dealt with Requests.");
            }
            else
            {
              //  debug("Server" + dserver_name + "has failed");
            }
        }

        public void FAIL(string dserver)
        {
            failServer = 1;
          //  debug("Server " + dserver_name + "has failed.");
        }

        public void RECOVER(string dserver)
        {
            failServer = 0;
            //debug("Server " + dserver_name + "is back online.");
        }

        public void DUMP()
        {
            // print values
        }
    }
}
