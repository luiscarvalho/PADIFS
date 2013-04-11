using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADICommonTypes
{
    [Serializable]
    public delegate void DebugDelegate(string mensagem);
    
    public interface IMDServer
    {
        void OPEN(string filename);
        void CLOSE(string filename);
        void CREATE(string filename, int nb_dataservers, int read_quorum, int write_quorum);
        void DELETE(string filename);
        void RECOVER(string mdserver);
        void FAIL(string mdserver);
    }

   public interface IDServer
    {
        void READ(string filename, string semantics);
        void WRITE(string filename, byte[] content);
        void FREEZE(string dserver);
        void UNFREEZE(string dserver);
        void FAIL(string dserver);
        void RECOVER(string dserver);
    }

    public interface IClient 
    {
        void CREATE(string clientname, string filename,int nb_dataservers,
                int read_quorum, int write_quorum);
        void OPEN(string clientname, string filename);
        void CLOSE(string clientname, string filename);
        void READ(string clientname, string filename, string semantics);
        void WRITE(string clientname, string filename, byte[] content); 
        void DELETE(string clientname, string filename);
    }
}
