using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Transactions;
using System.Threading.Tasks;
using PADICommonTypes;
using System.IO;
using System.Threading;

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
            ds.Register(args[1], args[2]);
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
        private List<KeyValuePair<string, KeyValuePair<byte[],int>>> fileList = new List<KeyValuePair<string, KeyValuePair<byte[],int>>>();
        private List<KeyValuePair<string, byte[]>> localFileList = new List<KeyValuePair<string, byte[]>>();
        static ReaderWriterLock rwl = new ReaderWriterLock();
        static int readerTimeouts = 0;
        static int writerTimeouts = 0;

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

        public byte[] CREATE(string filename)
        {
            fileList.Add(new KeyValuePair<string, KeyValuePair<byte[],int>>(filename,
                new KeyValuePair<byte[],int>(System.Text.Encoding.ASCII.GetBytes(""),0)));
            System.Console.WriteLine("File " + filename + " created." + "\r\n");
            localFileList.Add(new KeyValuePair<string, byte[]>(filename, System.Text.Encoding.ASCII.GetBytes(filename)));
            return System.Text.Encoding.ASCII.GetBytes(filename);

        }

        public List<KeyValuePair<string, byte[]>> OPEN(string filename)
        {
            List<KeyValuePair<string, byte[]>> openResult = new List<KeyValuePair<string, byte[]>>();
            foreach (KeyValuePair<string, byte[]> file in localFileList)
            {
                if (file.Key.Equals(filename))
                {
                    openResult.Add(new KeyValuePair<string, byte[]>(dserver_name, file.Value));
                }
            }
            return openResult;
        }

        public void Register(string DserverPort, string MDServerPort)
        {
            IMDServer mdserverRegister = (IMDServer)Activator.GetObject(typeof(IMDServer)
                , "tcp://localhost:" + MDServerPort + "/MetaData_Server");
            if (mdserverRegister.RegisteDServer(this.dserver_name, DserverPort))
            {
                this.freezeServer = 0;
                System.Console.WriteLine("Data Server " + this.dserver_name + " registered");
            }
        }

        public string READ(string filename, string semantics)
        {
            string result = null;
            try
            {
                rwl.AcquireReaderLock(10);
                try
                {
                    if (freezeServer == 0)
                    {
                        foreach (KeyValuePair<string, KeyValuePair<byte[],int>> file in fileList)
                        {
                            if (file.Key.Equals(filename+".txt"))
                            {
                                for (int i = 0; i <= file.Value.Value; i++)
                                {
                                    // procurar ultima versao
                                    if (semantics.Equals("default"))
                                    {
                                        i = file.Value.Value;
                                        System.Console.WriteLine("Versao do ficheiro n: " + i);
                                    }

                                    if (file.Value.Value.Equals(i))
                                    {
                                        System.Console.WriteLine("RESULT: " + System.Text.Encoding.Default.GetString(file.Value.Key));
                                        result = System.Text.Encoding.UTF8.GetString(file.Value.Key);
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
                    return result;
                }
                finally
                {
                    // Ensure that the lock is released.
                    rwl.ReleaseReaderLock();
                }
            }
            catch (ApplicationException)
            {
                // The reader lock request timed out.
                Interlocked.Increment(ref readerTimeouts);
            }
            return result;
        }

        public void WRITE(string filename, byte[] content)
        {
            try
            {
                rwl.AcquireWriterLock(10);
                try
                {
                    KeyValuePair<string, KeyValuePair<byte[],int>> result = new KeyValuePair<string, KeyValuePair<byte[],int>>();
                    if (freezeServer == 0)
                    {
                        foreach (KeyValuePair<string, KeyValuePair<byte[],int>> file in fileList)
                        {         
                            if (file.Key.Equals(filename+".txt"))
                            {
                                result = new KeyValuePair<string, KeyValuePair<byte[],int>>(file.Key,
                        new KeyValuePair<byte[],int>(content,(file.Value.Value) + 1));
                            }
                        }
                        fileList.Add(result);
                    }
                    else
                    {
                        dsrequests.Add("WRITE/" + filename + "/" + content.ToString());
                    }
                }
                finally
                {
                    // Ensure that the lock is released.
                    rwl.ReleaseWriterLock();
                }
            }
            catch (ApplicationException)
            {
                // The writer lock request timed out.
                Interlocked.Increment(ref writerTimeouts);
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
            this.freezeServer = 0;
            if (failServer == 0)
            {
               foreach (string request in dsrequests.ToArray())
                {
                    string[] req = request.Split('/');

                    if (req[0].Equals("READ"))
                    {
                        System.Console.WriteLine(req[1] + " " + req[2]);
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

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public void DUMP()
        {
            System.Console.WriteLine("DataServer: " + this.dserver_name + "\r\n");
            System.Console.WriteLine("Path: " + this.serverpath + "\r\n");

            System.Console.WriteLine("File List: \r\n");

            foreach (KeyValuePair<string, KeyValuePair<byte[],int>> file in this.fileList)
            {
                System.Console.WriteLine("File: " + file.Key + " Version: " + file.Value.Value + " Content: "
                    + System.Text.Encoding.ASCII.GetString(file.Value.Key) + "\r\n");
            }

            System.Console.WriteLine("Local File List: \r\n");

            foreach (KeyValuePair<string, byte[]> localfile in this.localFileList)
            {
                System.Console.WriteLine("File: " + localfile.Key + " MetaData: " + System.Text.Encoding.UTF8.GetString(localfile.Value));
            }
        }
    }
}
