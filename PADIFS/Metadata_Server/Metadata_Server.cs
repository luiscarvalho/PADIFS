using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using PADICommonTypes;
using System.IO;

namespace MetaData_Server
{
    [Serializable]
    class Metadata_Server
    {
        //static string servername;
        //static MDServer mdserver;

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(Convert.ToInt32(args[1]));
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MDServer), "MetaData_Server",
            WellKnownObjectMode.Singleton);
            MDServer mds = new MDServer(args[0]);
            RemotingServices.Marshal(mds, "MetaData_Server", typeof(MDServer));

            //servername = args[0];
            System.Console.WriteLine("MetaData Server " + args[0] + " start with port " + args[1]);
            System.Console.ReadLine();
        }
    }

    public class MDServer : MarshalByRefObject, IMDServer
    {
        private DataTable mdTable;
        private string mdserver_name = "m-";
        //private string filename;
        //private int numServer = 0;
        private int failServer = 0;
        //private int nb_dataservers;
        //private int read_quorum;
        //private int write_quorum;
        private List<KeyValuePair<string, string>> dataServerList;
        private List<KeyValuePair<string, string>> dataServers;

        public MDServer(string mdsname)
        {
            mdTable = new DataTable();
            dataServerList = new List<KeyValuePair<string, string>>();
            dataServers = new List<KeyValuePair<string, string>>();
            mdTable.Columns.Add("Filename", typeof(string));
            mdTable.Columns.Add("NB_DataServers", typeof(int));
            mdTable.Columns.Add("Read_Quorum", typeof(int));
            mdTable.Columns.Add("Write_Quorum", typeof(int));
            mdTable.Columns.Add("Data Servers", typeof(List<KeyValuePair<string, string>>));
            this.mdserver_name = mdsname;
            mdTable.TableName.Insert(0, mdserver_name);


            //numServer++;
            //debug("Metadata server " + mdserver_name + " created.");
        }

        public bool RegisteDServer(string dservername, string port)
        {
            System.Console.WriteLine("Metadata server " + mdserver_name + ": "+ dservername +" registered.");
            dataServerList.Add(new KeyValuePair<string, string>(dservername, port));
            return true;
        }
        public void CREATE(string fname, int dservers, int rquorum, int wquorum)
        {
            System.Console.WriteLine("Create : cheguei aqui!" + "\r\n");
            int nservers = dataServerList.Count();
            int i = 0;
            foreach (KeyValuePair<string,string> dserver in dataServerList)
            {
                if (i == nservers || i == dservers)
                {
                    break;
                }

                if(!dataServers.Contains(new KeyValuePair<string, string>(dserver.Key, dserver.Value))){
                    dataServers.Add(new KeyValuePair<string, string>(dserver.Key, dserver.Value));
                }
                System.Console.WriteLine("File added in: " + dserver.Key);
                string ds = dserver.Key;
                string serverpath = Directory.GetCurrentDirectory();
                serverpath += Path.Combine(ds);
                IDServer dsCreate = (IDServer)Activator.GetObject(typeof(IDServer)
                  , "tcp://localhost:" + dserver.Value + "/Data_Server");
                dsCreate.CREATE(serverpath + "\\" + fname + ".txt");

//                if (Directory.Exists(serverpath))
//                {
//                    File.CreateText(serverpath + "\\" + fname + ".txt");
//                }
//                i++;
            }
            mdTable.Rows.Add(fname, dservers, rquorum, wquorum, dataServers);
            System.Console.WriteLine("File " + fname + " created.");
        }

        public void DELETE(string fname)
        {
            foreach (DataRow dr in mdTable.Rows)
            {
                if (dr["Filename"].ToString() == fname)
                    dr.Delete();
            }
        }

        public List<KeyValuePair<string, string>> OPEN(string fname)
        {
            List<KeyValuePair<string, string>> openResult = new List<KeyValuePair<string,string>>();
            List<KeyValuePair<string, string>> openResultDS = new List<KeyValuePair<string,string>>();
            foreach (DataRow dr in mdTable.Rows)
            {
                if (dr["Filename"].ToString() == fname)
                {
                    List<KeyValuePair<string, string>> ldserver = (List<KeyValuePair<string,string>>) dr["Data Servers"];

                    foreach (KeyValuePair<string, string> dserver in ldserver){
                        openResult.Add(new KeyValuePair<string,string>(dr["Filename"].ToString() +".txt",dserver.Key));
                        string ds = dserver.Key;
                        string serverpath = Directory.GetCurrentDirectory();
                        serverpath += Path.Combine(ds);
                        
                        IDServer dsCreate = (IDServer)Activator.GetObject(typeof(IDServer)
                          , "tcp://localhost:" + dserver.Value + "/Data_Server");
                        openResultDS = dsCreate.OPEN(serverpath + "\\" + fname + ".txt");
                        foreach (KeyValuePair<string,string> value in openResultDS)
                        {
                            openResult.Add(new KeyValuePair<string,string>(value.Key, value.Value));
                        }
                    }
                }
            }
            return openResult;
        }

        public void CLOSE(string fname)
        {
            foreach (DataRow dr in mdTable.Rows)
            {
                if (dr["Filename"].ToString() == fname)
                {
                    dr.CancelEdit();
                    dr.EndEdit();
                }
            }

        }

        public void FAIL(string mdserver)
        {
            mdTable.EndInit();
            mdTable.EndLoadData();
            failServer = 1;
        }

        public void RECOVER(string mdserver)
        {
            if (failServer == 0)
            {
               //new MDServer(new DebugDelegate(debug));
            }
            else
            {
                this.mdTable = mdTable.Clone();
                failServer = 0;
            }
        }

        public void DUMP()
        {
            // print values
        }
    }
}