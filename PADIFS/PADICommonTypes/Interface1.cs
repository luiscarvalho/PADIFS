using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADICommonTypes
{

    public interface IMDServer
    {
        object[] OPEN(string filename);
        void CLOSE(string filename);
        object[] CREATE(string filename, int nb_dataservers, int read_quorum, int write_quorum, string clientport);
        void DELETE(string filename);
        void RECOVER(string mdserver);
        void FAIL(string mdserver);
        void loadMDServer(DataTable MDtable);
        void primaryMDServer(string mdserver_name, string mdserverport);
        void aliveMDServer(string mdserver_name, string mdserverport);
        void sendMDServer(DataTable MDtable, List<KeyValuePair<string, string>> DServerList);
        void addMDServerTable(string filename, int nDServers, int rquorum, int wquorum, List<KeyValuePair<string, string>> DServers, Hashtable dataServerLoad, List<KeyValuePair<string,byte[]>> localFList);
        //void registeDS(string dservername, string port);
        //DataTable copyMDServer();
        void DUMP();
        bool RegisteDServer(string dservername, string port);
    }

   public interface IDServer
    {
        byte[] CREATE(string filename);
        List<KeyValuePair<string, byte[]>> OPEN(string filename);
        string READ(string filename, string semantics);
        void WRITE(string filename, byte[] content);
        void FREEZE(string dserver);
        void UNFREEZE(string dserver);
        void FAIL(string dserver);
        void RECOVER(string dserver);
        void DUMP();
    }

    public interface IClient 
    {
        object[] CREATE(string clientname, string filename, int nb_dataservers,
                int read_quorum, int write_quorum, string primaryPort);
        object[] OPEN(string clientname, string filename, string primaryPort);
        void CLOSE(string clientname, string filename, string primaryPort);
        string READ(string clientname, string filename, string semantics);
        void WRITE(string clientname, string filename, byte[] content);
        void DELETE(string clientname, string filename);
        void COPY(string clientname, string fileregister1, string semantics, string fileregister2, string salt);
        void DUMP();
        void EXESCRIPT(string clientname, string script);
    }
}
