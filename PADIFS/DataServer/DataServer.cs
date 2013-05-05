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
        private List<string> dsrequests = new List<string>();
        private int numServer = 0;
        private int freezeServer;
        private int failServer = 0;
        private string serverpath;
        private int numFile = 0;
        private List<KeyValuePair<string,KeyValuePair<int,byte[]>>> fileList = new List<KeyValuePair<string,KeyValuePair<int,byte[]>>>();
        private List<KeyValuePair<int, string>> localFileList = new List<KeyValuePair<int, string>>();

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

        public void CREATE(string filename)
        {
            fileList.Add(new KeyValuePair<string, KeyValuePair<int, byte[]>>(filename,
                new KeyValuePair<int, byte[]>(0, System.Text.Encoding.ASCII.GetBytes(""))));
            System.Console.WriteLine("File " + filename + " created." + "\r\n");
            localFileList.Add(new KeyValuePair<int,string>(numFile,filename));
            numFile++;
        }

        public List<KeyValuePair<string, string>> OPEN(string filename)
        {
            List<KeyValuePair<string, string>> openResult = new List<KeyValuePair<string, string>>();
            foreach (KeyValuePair<int,string> file in localFileList)
            {
                if (file.Value.Equals(filename))
                {
                    openResult.Add(new KeyValuePair<string, string>(dserver_name, file.Key.ToString()));
                }
            }
            return openResult;
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

        public string READ(string filename, string semantics)
        {
            char[] fileName = new char[1];
            string result = null;

            if (failServer == 0)
            {
                if (freezeServer == 0)
                {
                    foreach (KeyValuePair<int, string> localfile in localFileList)
                    {
                        if (int.Parse(filename).Equals(localfile.Key))
                        {
                            fileName = new char[localfile.Value.Length];
                            localfile.Value.CopyTo(0,fileName,0,localfile.Value.Length);
                        }
                    }
    
                    foreach (KeyValuePair<string, KeyValuePair<int, byte[]>> file in fileList)
                    {
                        if (file.Key.Equals(new string(fileName)))
                        {
                            for (int i = 0; i <= file.Value.Key; i++)
                            {
                                // procurar ultima versao
                                if (semantics.Equals("default"))
                                {
                                    i = file.Value.Key;
                                    System.Console.WriteLine("Versao do ficheiro n: " + i);
                                }

                                if (file.Value.Key.Equals(i))
                                {
                                    System.Console.WriteLine("RESULT: " + System.Text.Encoding.Default.GetString(file.Value.Value));
                                    result = System.Text.Encoding.UTF8.GetString(file.Value.Value);
                                }
                            }
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
            return result;
        }

        public void WRITE(string filename, byte[] content)
        {
            string fileName = null;
            KeyValuePair<string, KeyValuePair<int, byte[]>> result = new KeyValuePair<string, KeyValuePair<int, byte[]>>();

            if (failServer == 0)
            {
                if (freezeServer == 0)
                {
                    foreach (KeyValuePair<int, string> localfile in localFileList)
                    {
                        if(int.Parse(filename).Equals(localfile.Key))
                        {
                            fileName = localfile.Value;
                            break;
                        }
                    }

                    foreach (KeyValuePair<string, KeyValuePair<int, byte[]>> file in fileList)
                    {
                        if (file.Key.Equals(fileName))
                        {
                            result = new KeyValuePair<string, KeyValuePair<int, byte[]>>(fileName,
                    new KeyValuePair<int, byte[]>((file.Value.Key) + 1, content));
                        }
                    }
                    fileList.Add(result);
                }
                else
                {
                    dsrequests.Add("WRITE/" + filename + "/" + content.ToString());
                }
               // debug("File" + filename + "opened for write purposes");
            }
            else
            {
              System.Console.WriteLine("Server" + dserver_name + "has failed");
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
            System.Console.WriteLine("DataServer: " + this.dserver_name + "\r\n");
            System.Console.WriteLine("Path: " + this.serverpath + "\r\n");

            System.Console.WriteLine("File List: \r\n");

            foreach (KeyValuePair<string, KeyValuePair<int, byte[]>> file in this.fileList)
            {
                System.Console.WriteLine("File: " + file.Key + " Version: " + file.Value.Key + " Content: " 
                    + System.Text.Encoding.UTF8.GetString(file.Value.Value) + "\r\n");
            }

            System.Console.WriteLine("Local File List: \r\n");

            foreach (KeyValuePair<int, string> localfile in this.localFileList) 
            {
                System.Console.WriteLine("File Register: " + localfile.Key + " Corresponding file: " + localfile.Value);
            }
        }
    }
}
