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
using System.Threading;
using System.ComponentModel;
using System.Collections;


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
            MDServer mds = new MDServer(args[0], args[1]);
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
        private string mdserver_port;
        //private string filename;
        //private int numServer = 0;
        private int failServer = 0;
        //private int nb_dataservers;
        //private int read_quorum;
        //private int write_quorum;
        private List<KeyValuePair<string, string>> dataServerList;
        private List<KeyValuePair<string, string>> metaDataServersList;
        Hashtable dataServerLoad = new Hashtable();
        private string primaryMDS_name;
        private string primaryMDS_port;
        static ReaderWriterLock rwl = new ReaderWriterLock();
        static int readerTimeouts = 0;
        private Object thisLock = new Object();

        public MDServer(string mdsname, string port)
        {
            mdTable = new DataTable();
            dataServerList = new List<KeyValuePair<string, string>>();
            metaDataServersList = new List<KeyValuePair<string, string>>();
            dataServerLoad = new Hashtable();
            dataServerLoad.Add(0, new List<string>() { });
            mdTable.Columns.Add("Filename", typeof(string));
            mdTable.Columns.Add("NB_DataServers", typeof(int));
            mdTable.Columns.Add("Read_Quorum", typeof(int));
            mdTable.Columns.Add("Write_Quorum", typeof(int));
            mdTable.Columns.Add("Data Servers", typeof(List<KeyValuePair<string, string>>));
            mdTable.Columns.Add("Local Files", typeof(List<KeyValuePair<string, string>>));
            this.mdserver_name = mdsname;
            this.mdserver_port = port;
            mdTable.TableName.Insert(0, mdserver_name);
        }

        /**
        public DataTable copyMDServer()
        {
            DataTable copyMD = new DataTable();
            BackgroundWorker bwCopy = new BackgroundWorker();

            bwCopy.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

                copyMD = this.mdTable;

            });
            bwCopy.RunWorkerAsync();
            return copyMD;
        }
        **/
        public void loadMDServer(DataTable MDtable)
        {
            BackgroundWorker bwLoad = new BackgroundWorker();

            bwLoad.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;
                if (!MDtable.Equals(null))
                {
                    this.mdTable = MDtable;
                }
            });
            bwLoad.RunWorkerAsync();
        }

        public void primaryMDServer(string mds_name, string mds_port)
        {
            BackgroundWorker bwprimary = new BackgroundWorker();

            bwprimary.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;
                this.primaryMDS_name = mds_name;
                this.primaryMDS_port = mds_port;
                IMDServer mdsAlive = (IMDServer)Activator.GetObject(typeof(IMDServer)
                                       , "tcp://localhost:" + primaryMDS_port + "/MetaData_Server");
                mdsAlive.aliveMDServer(this.mdserver_name, this.mdserver_port);
            });
            bwprimary.RunWorkerAsync();
        }

        public void aliveMDServer(string mds_name, string mds_port)
        {
            BackgroundWorker bwAlive = new BackgroundWorker();

            bwAlive.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;
                metaDataServersList.Add(new KeyValuePair<string, string>(mds_name, mds_port));
                System.Console.WriteLine("MetaData Server " + mds_name + " registered in port " + mds_port);
                IMDServer mdsSendTable = (IMDServer)Activator.GetObject(typeof(IMDServer)
                                       , "tcp://localhost:" + primaryMDS_port + "/MetaData_Server");
                mdsSendTable.sendMDServer(mdTable, dataServerList);
            });
            bwAlive.RunWorkerAsync();
        }

        public void sendMDServer(DataTable mdtable, List<KeyValuePair<string, string>> dserverList)
        {
            BackgroundWorker bwSend = new BackgroundWorker();

            bwSend.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;
                this.mdTable = mdtable;
                this.metaDataServersList = dserverList;
                System.Console.WriteLine("Data received from the primary MDServer");
            });
            bwSend.RunWorkerAsync();
        }

        public bool RegisteDServer(string dservername, string port)
        {
            BackgroundWorker bwRegister = new BackgroundWorker();

            bwRegister.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                lock (thisLock)
                {
                    List<string> listAux;
                    System.Console.WriteLine("Metadata server " + mdserver_name + ": " + dservername + " registered.");
                    dataServerList.Add(new KeyValuePair<string, string>(dservername, port));
                    listAux = (List<string>)dataServerLoad[0];
                    listAux.Add(dservername);
                    dataServerLoad.Add(0, listAux);

                    if (metaDataServersList.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> mdserver in this.metaDataServersList)
                        {
                            try
                            {
                                IMDServer mdsRegisteDS = (IMDServer)Activator.GetObject(typeof(IMDServer)
                                                      , "tcp://localhost:" + mdserver.Value + "/MetaData_Server");
                                mdsRegisteDS.RegisteDServer(dservername, port);
                            }
                            catch (System.Net.Sockets.SocketException ex)
                            {
                                metaDataServersList.Remove(mdserver);
                            }
                        }
                    }
                }
            });
            bwRegister.RunWorkerAsync();
            return true;
        }

        public DataRow CREATE(string fname, int dservers, int rquorum, int wquorum, string clientport)
        {
            DataRow createResult = null;
            List<KeyValuePair<string, string>> dataServers = new List<KeyValuePair<string,string>>();
            List<KeyValuePair<string, byte[]>> localFileList = new List<KeyValuePair<string,byte[]>>();
            //BackgroundWorker bwCreate = new BackgroundWorker();
            //bwCreate.DoWork += new DoWorkEventHandler(
            //delegate(object o, DoWorkEventArgs args)
            //{
                lock (thisLock)
                {
                    Hashtable dataServerLoadAux = new Hashtable();
                    dataServerLoadAux = dataServerLoad;
                    List<string> dserverListAux1;
                    List<string> dserverListAux2;
                    int nservers = dataServerList.Count();
                    int nserverdone = 0;
                    int i = 0;
                    int done = 0;
                    if (nservers >= dservers)
                    {
                        while (nserverdone < dservers)
                        {
                            dserverListAux1 = (List<string>)dataServerLoadAux[i];

                            foreach (string dServer in dserverListAux1)
                            {
                                if (nserverdone >= dservers) break;
                                foreach (KeyValuePair<string, string> dser in dataServerList)
                                {
                                    if (dser.Key == dServer)
                                    {
                                        try
                                        {
                                            string ds = dser.Key;
                                            IDServer dsCreate = (IDServer)Activator.GetObject(typeof(IDServer)
                                              , "tcp://localhost:" + dser.Value + "/Data_Server");
                                           byte[] createR = dsCreate.CREATE(fname + ".txt");
                                            done = 1;
                                            nserverdone += done;
                                            dataServers.Add(new KeyValuePair<string, string>(dser.Key, dser.Value));
                                            localFileList.Add(new KeyValuePair<string, byte[]>(dser.Key, createR));
                                            System.Console.WriteLine("File added in: " + dser.Key);
                                            if (nserverdone >= dservers) break;
                                        }
                                        catch (System.Net.Sockets.SocketException ex)
                                        {
                                            done = 0;
                                        }
                                    }
                                    break;
                                }
                                if (done == 1)
                                {
                                    dserverListAux1.Remove(dServer);
                                    dataServerLoadAux.Add(i, dserverListAux1);
                                    if (dataServerLoadAux.Count > i)
                                    {
                                        dserverListAux2 = (List<string>)dataServerLoadAux[i + 1];
                                        dserverListAux2.Add(dServer);
                                        dataServerLoadAux.Add(i, dserverListAux2);
                                    }
                                    else
                                    {
                                        dataServerLoadAux.Add(i + 1, new List<string>() { dServer });
                                    }
                                }
                            }
                            i++;
                        }

                        createResult = mdTable.Rows.Add(fname, dservers, rquorum, wquorum, dataServers,localFileList);
                        dataServerLoad = dataServerLoadAux;
                        System.Console.WriteLine("File " + fname + " created.");

                        if (metaDataServersList.Count > 0)
                        {
                            foreach (KeyValuePair<string, string> mdserver in this.metaDataServersList)
                            {
                                try
                                {
                                    IMDServer mdsAddTable = (IMDServer)Activator.GetObject(typeof(IMDServer)
                                                          , "tcp://localhost:" + mdserver.Value + "/MetaData_Server");
                                    mdsAddTable.addMDServerTable(fname, dservers, rquorum, wquorum, dataServers, dataServerLoad, localFileList);
                                }
                                catch (System.Net.Sockets.SocketException ex)
                                {
                                    metaDataServersList.Remove(mdserver);
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new RemotingException("Não foi possível satisfazer o pedido: Data Servers insuficientes");
                    }
                }
           // });
           // bwCreate.RunWorkerAsync();
            return createResult;
        }

        public void addMDServerTable(string filename, int nDServers, int rquorum, int wquorum, List<KeyValuePair<string, string>> DServers, Hashtable DServerLoad, List<KeyValuePair<string, byte[]>> localFList)
        {
            BackgroundWorker bwUpdate = new BackgroundWorker();

            bwUpdate.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                mdTable.Rows.Add(filename, nDServers, rquorum, wquorum, DServers, localFList);
                dataServerLoad = DServerLoad;
            });
            bwUpdate.RunWorkerAsync();
        }

        public void DELETE(string fname)
        {
            BackgroundWorker bwDelete = new BackgroundWorker();

            bwDelete.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                lock (thisLock)
                {
                    List<KeyValuePair<string, string>> serversList;
                    int i;
                    List<string> serverListAux1;
                    List<string> serverListAux2;
                    foreach (DataRow dr in mdTable.Rows)
                    {
                        if (dr["Filename"].ToString() == fname)
                        {
                            serversList = (List<KeyValuePair<string, string>>)dr["Data Servers"];

                            foreach (KeyValuePair<string, string> server in serversList)
                            {
                                i = 1;
                                while (dataServerLoad.Count > i)
                                {
                                    serverListAux1 = (List<string>)dataServerLoad[i];
                                    serverListAux2 = (List<string>)dataServerLoad[i - 1];
                                    foreach (string serv in serverListAux1)
                                    {
                                        if (server.Key == serv)
                                        {
                                            serverListAux1.Remove(serv);
                                            serverListAux2.Add(serv);
                                            dataServerLoad.Add(i, serverListAux1);
                                            dataServerLoad.Add(i - 1, serverListAux2);
                                            break;
                                        }
                                    }
                                }
                            }
                            dr.Delete();
                            break;
                        }
                    }

                    foreach (KeyValuePair<string, string> mdserver in this.metaDataServersList)
                    {
                        IMDServer mdsDelTable = (IMDServer)Activator.GetObject(typeof(IMDServer)
                                              , "tcp://localhost:" + mdserver.Value + "/MetaData_Server");
                        mdsDelTable.DELETE(fname);
                    }
                }
            });
            bwDelete.RunWorkerAsync();
        }

        public DataRow OPEN(string fname)
        {
            DataRow openResult = null;

            BackgroundWorker bwOpen = new BackgroundWorker();

            bwOpen.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                try
                {
                    rwl.AcquireReaderLock(20);
                    try
                    {
                        foreach (DataRow dr in mdTable.Rows)
                        {
                            if (dr["Filename"].ToString() == fname)
                            {
                                openResult = dr;
                            }
                        }
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
            });
            bwOpen.RunWorkerAsync();
            return openResult;
        }

        public void CLOSE(string fname)
        {
            BackgroundWorker bwClose = new BackgroundWorker();

            bwClose.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                foreach (DataRow dr in mdTable.Rows)
                {
                    if (dr["Filename"].ToString() == fname)
                    {
                        dr.CancelEdit();
                        dr.EndEdit();
                    }
                }
            });
            bwClose.RunWorkerAsync();
        }

        public void FAIL(string mdserver)
        {
            BackgroundWorker bwFail = new BackgroundWorker();

            bwFail.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

                mdTable.EndInit();
                mdTable.EndLoadData();
                failServer = 1;
            });
            bwFail.RunWorkerAsync();

        }

        public void RECOVER(string mdserver)
        {
            BackgroundWorker bwRecover = new BackgroundWorker();

            bwRecover.DoWork += new DoWorkEventHandler(
            delegate(object o, DoWorkEventArgs args)
            {
                BackgroundWorker b = o as BackgroundWorker;

                if (failServer == 0)
                {
                    //new MDServer(new DebugDelegate(debug));
                }
                else
                {
                    this.mdTable = mdTable.Clone();
                    failServer = 0;
                }
            });
            bwRecover.RunWorkerAsync();
        }

        public void DUMP()
        {
            System.Console.WriteLine("MetaDataServer: " + this.mdserver_name + "\r\n");

            System.Console.WriteLine("DataServer List: \r\n");

            foreach (KeyValuePair<string, string> file in this.dataServerList)
            {
                System.Console.WriteLine("DataServer: " + file.Key + " Port: " + file.Value + "\r\n");
            }

            System.Console.WriteLine("Local File List: \r\n");

            foreach (DataRow mdRow in this.mdTable.Rows)
            {
                System.Console.WriteLine("---Row" + "\r\n");
                foreach (var item in mdRow.ItemArray)
                {
                    System.Console.WriteLine(item);
                }
            }
        }
    }
}