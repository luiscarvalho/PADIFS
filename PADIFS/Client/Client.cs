using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using PADICommonTypes;

namespace Client
{
    [Serializable]
    class Client
    {
        static string clientname;
        
        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(Convert.ToInt32(args[1]));
            ChannelServices.RegisterChannel(channel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(Cliente), "ClientRemote",
            WellKnownObjectMode.Singleton);
            Cliente c = new Cliente(args[0]);
            RemotingServices.Marshal(c, "ClientRemote", typeof(Cliente));
            clientname += args[0];
            System.Console.WriteLine("Client " + args[0] + " start with port " + args[1]);
            System.Console.ReadLine();
        }

        static void Debug(string mensagem)
        {
            Console.WriteLine(mensagem);
        }
    }
    public class Cliente : MarshalByRefObject, IClient
    {
        string cname;
        List<KeyValuePair<string, string>> filesClient = new List<KeyValuePair<string,string>>();

        public Cliente(string clientname)
        {
            this.cname = clientname;
        }

        public void CREATE(string clientname, string filename, int nb_dataservers,
            int read_quorum, int write_quorum)
        {
            System.Console.WriteLine("Create : cheguei aqui!" + "\r\n");
            IMDServer mdscreate = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:8080/MetaData_Server");
           mdscreate.CREATE(filename, nb_dataservers, read_quorum, write_quorum);
        }

        public void OPEN(string clientname, string filename)
        {
            System.Console.WriteLine("I want to open a file." + "\r\n");
            IMDServer mdsopen = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:8080/MetaData_Server");
            List<KeyValuePair<string, string>> filesClientTemp = mdsopen.OPEN(filename);
            foreach (KeyValuePair<string, string> value in filesClientTemp)
            {
                filesClient.Add(new KeyValuePair<string,string>(value.Key, value.Value));
                System.Console.WriteLine(value.Key + " " + value.Value);
            }
            

        }

        public void CLOSE(string clientname, string filename)
        {
            IMDServer mdsclose = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:8080/MetaData_Server");
            mdsclose.CLOSE(filename);
        }

        public string READ(string clientname, string filename, string semantics)
        {
            string result = null;
            System.Console.WriteLine("Read it!"+"\r\n");
            foreach (KeyValuePair<string, string> value in filesClient)
            {
                System.Console.WriteLine("Key: " + value.Key + " " + "Value: " + value.Value);
                if (value.Value.Equals(filename))
                {
                    string[] nserver = value.Key.Split('-');
                    System.Console.WriteLine("nserver: " + nserver[1]);
                    IDServer dsread = (IDServer)Activator.GetObject(typeof(IDServer), "tcp://localhost:807" + nserver[1] + "/Data_Server");
                    result = dsread.READ(filename, semantics);
                }
            }
            System.Console.WriteLine("File: " + filename + " Content: " + result); 
            return result;
        }

        public void WRITE(string clientname, string filename, byte[] content)
        {
            System.Console.WriteLine("Write this!"+"\r\n");
            foreach (KeyValuePair<string, string> value in filesClient)
            {
                System.Console.WriteLine("Key: " + value.Key + " " + "Value: " + value.Value);
                if (value.Value.Equals(filename))
                {
                    string[] nserver = value.Key.Split('-');
                    System.Console.WriteLine("nserver: " + nserver[1]);
                    IDServer dswrite = (IDServer)Activator.GetObject(typeof(IDServer)
                    , "tcp://localhost:807" + nserver[1] + "/Data_Server");
                    dswrite.WRITE(filename, content);
                }
            }
        }

        public void DELETE(string clientname, string filename)
        {
            IMDServer mdsdelete = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:8086/MetaData_Server");
            mdsdelete.DELETE(filename);
        }

        public void COPY(string clientname, string fileregister1, string semantics, string fileregister2, string salt)
        {
            // Ainda não percebi bem o que são os file-register
        }

        public void DUMP()
        {
            // print values
        }

        public void EXESCRIPT(string clientname, string script)
        {
        }
    }
}
