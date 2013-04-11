using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PADICommonTypes
{
    public delegate void DebugDelegate(string mensagem);

    public interface IMDServer
    {
        void OPEN(string filename, DebugDelegate debug);
        void CLOSE(string filename, DebugDelegate debug);
        void CREATE(string filename, int nb_dataservers, int read_quorum, int write_quorum, DebugDelegate debug);
        void DELETE(string filename, DebugDelegate debug);
        void RECOVER(string mdserver, DebugDelegate debug);
        void FAIL(string mdserver, DebugDelegate debug);
    }

    public interface IDServer
    {
        void READ(string filename, string semantics, DebugDelegate debug);
        void WRITE(string filename, byte[] content, DebugDelegate debug);
        void FREEZE(string dserver, DebugDelegate debug);
        void UNFREEZE(string dserver, DebugDelegate debug);
        void FAIL(string dserver, DebugDelegate debug);
        void RECOVER(string dserver, DebugDelegate debug);
    }

    public interface IClient
    {
        void CREATE(string clientname, string filename, int nb_dataservers,
                int read_quorum, int write_quorum, DebugDelegate debug);
        void OPEN(string clientname, string filename, DebugDelegate debug);
        void CLOSE(string clientname, string filename, DebugDelegate debug);
        void READ(string clientname, string filename, string semantics, DebugDelegate debug);
        void WRITE(string clientname, string filename, byte[] content, DebugDelegate debug);
        void DELETE(string clientname, string filename, DebugDelegate debug);
    }
}
