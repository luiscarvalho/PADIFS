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

namespace MetaData_Server
{
    [Serializable]
    class Metadata_Server
    {
        static string servername;
        static MDServer mdserver;

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(Convert.ToInt32(args[1]));
            ChannelServices.RegisterChannel(channel, true);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MDServer), "MetaData_Server",
            WellKnownObjectMode.Singleton);
            servername = args[0];
            DebugDelegate debug = new DebugDelegate(Debug);
            mdserver = new MDServer(debug);
            System.Console.WriteLine("Metadata Server " + servername + " on");
            System.Console.ReadLine();
        }

        static void Debug(string mensagem)
        {
            Console.WriteLine(mensagem);
        }
    }

    public class MDServer : MarshalByRefObject, IMDServer
    {
        private DataTable mdTable;
        private string mdserver_name = "m-";
        private string filename;
        private int numServer = 0;
        private int failServer = 0;
        private int nb_dataservers;
        private int read_quorum;
        private int write_quorum;
        private DebugDelegate debug;
        private List<KeyValuePair<string, string>> dataServers = new List<KeyValuePair<string, string>>();

        public MDServer(DebugDelegate debug)
        {
            mdTable = new DataTable();
            mdTable.Columns.Add("Filename", typeof(string));
            mdTable.Columns.Add("NB_DataServers", typeof(int));
            mdTable.Columns.Add("Read_Quorum", typeof(int));
            mdTable.Columns.Add("Write_Quorum", typeof(int));
            mdTable.Columns.Add("Data Servers", typeof(List<KeyValuePair<string, string>>));
            mdserver_name += numServer;
            mdTable.TableName.Insert(0, mdserver_name);
            numServer++;
            debug("Metadata server " + mdserver_name + " created.");
        }

        public void CREATE(string fname, int dservers, int rquorum, int wquorum, DebugDelegate debug)
        {
            dataServers.Add(new KeyValuePair<string, string>("d-0", "0"));
            mdTable.Rows.Add(fname, dservers, rquorum, wquorum, dataServers);
            debug("File" + fname + "created.");
        }

        public void DELETE(string fname, DebugDelegate debug)
        {
            foreach (DataRow dr in mdTable.Rows)
            {
                if (dr["Filename"].ToString() == fname)
                    dr.Delete();
            }

            debug("File" + fname + "deleted.");
        }

        public void OPEN(string fname, DebugDelegate debug)
        {
            foreach (DataRow dr in mdTable.Rows)
            {
                if (dr["Filename"].ToString() == fname)
                {
                    dr.BeginEdit();
                    Console.WriteLine(dr.ToString());
                    dr.AcceptChanges();
                }
            }
            debug("File" + fname + "opened.");
        }

        public void CLOSE(string fname, DebugDelegate debug)
        {
            foreach (DataRow dr in mdTable.Rows)
            {
                if (dr["Filename"].ToString() == fname)
                {
                    dr.CancelEdit();
                    dr.EndEdit();
                }
                debug("File" + fname + "closed.");
            }

        }

        public void FAIL(string mdserver, DebugDelegate debug)
        {
            mdTable.EndInit();
            mdTable.EndLoadData();
            failServer = 1;
            debug("MetaData_Server" + mdserver_name + "has failed");
        }

        public void RECOVER(string mdserver, DebugDelegate debug)
        {
            if (failServer == 0)
            {
               new MDServer(new DebugDelegate(debug));
               debug("MetaData_Server" + mdserver_name + "on");
            }
            else
            {
                this.mdTable = mdTable.Clone();
                failServer = 0;
                debug("MetaData_Server" + mdserver_name + "is back on");
            }
        }

        public void DUMP()
        {
            // print values
        }
    }
}