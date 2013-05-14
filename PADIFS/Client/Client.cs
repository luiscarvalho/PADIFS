using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using PADICommonTypes;
using System.Data;

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
            Cliente c = new Cliente(args[0], args[1]);
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
        string clientname;
        string clientport;
        int fileRegister = 0;
        List<KeyValuePair<KeyValuePair<int, string>, object[]>> filesClient = new List<KeyValuePair<KeyValuePair<int, string>, object[]>>();
        List<KeyValuePair<KeyValuePair<int, string>, object[]>> localFilesClient = new List<KeyValuePair<KeyValuePair<int, string>, object[]>>();


        public Cliente(string clientname, string clientport)
        {
            this.clientname = clientname;
            this.clientport = clientport;
        }

        public object[] CREATE(string clientname, string filename, int nb_dataservers,
            int read_quorum, int write_quorum, string primaryPort)
        {
            object[] createResult = null;
            try
            {
                IMDServer mdscreate = (IMDServer)Activator.GetObject(typeof(IMDServer)
                , "tcp://localhost:" + primaryPort + "/MetaData_Server");

                createResult = mdscreate.CREATE(filename, nb_dataservers, read_quorum, write_quorum, clientport);
                KeyValuePair<int, string> fileR = new KeyValuePair<int, string>(fileRegister, filename);
                filesClient.Add(new KeyValuePair<KeyValuePair<int, string>, object[]>(new KeyValuePair<int, string>(fileRegister, filename), createResult));
                fileRegister++;

                System.Console.WriteLine("O ficheiro foi criado com sucesso." + "\r\n");
                System.Console.WriteLine("Ficheiro: " + createResult[0].ToString());
                System.Console.WriteLine("está contido em " + createResult[1].ToString() + " data servers");
                System.Console.WriteLine("Read Quorum: " + createResult[2].ToString() + " ");
                System.Console.WriteLine("Write Quorum: " + createResult[3].ToString() + "\r\n");

                foreach (KeyValuePair<string,byte[]> kp in (List<KeyValuePair<string,byte[]>>) createResult[5]){

                    System.Console.WriteLine(kp.Key.ToString() + " - " + ByteArrayToString(kp.Value) + "\r\n");
                }
                
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                System.Console.WriteLine("Falha do Servidor Metadata, o seu pedido não foi efectuado.");
                throw new RemotingException("Falha do Servidor Metadata com o porto: " + primaryPort);
            }
            catch (RemotingException ey)
            {
                System.Console.WriteLine(ey.Message + "\r\n");
                throw new RemotingException("Não foi possível satisfazer o pedido: Data Servers insuficientes");
            }
            return createResult;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public object[] OPEN(string clientname, string filename, string primaryPort)
        {
            System.Console.WriteLine("I want to open a file." + "\r\n");
            IMDServer mdsopen = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:" + primaryPort + "/MetaData_Server");
            object[] rowResult = mdsopen.OPEN(filename);

            foreach (KeyValuePair<KeyValuePair<int, string>, object[]> value in filesClient)
            {
                if (value.Key.Value.Equals(filename))
                {
                    localFilesClient.Add(new KeyValuePair<KeyValuePair<int, string>, object[]>(new KeyValuePair<int, string>(value.Key.Key, value.Key.Value),rowResult));
                    System.Console.WriteLine("O ficheiro foi criado com sucesso." + "\r\n");
                    System.Console.WriteLine("Ficheiro: " + rowResult[0].ToString());
                    System.Console.WriteLine("está contido em " + rowResult[1].ToString() + " data servers");
                    System.Console.WriteLine("Read Quorum: " + rowResult[2].ToString() + " ");
                    System.Console.WriteLine("Write Quorum: " + rowResult[3].ToString() + "\r\n");
                    foreach (KeyValuePair<string, byte[]> kp in (List<KeyValuePair<string, byte[]>>)rowResult[5])
                    {
                        System.Console.WriteLine(kp.Key.ToString() + " - " + ByteArrayToString(kp.Value) + "\r\n");
                    }
                    break;
                }

            }
            return rowResult;

        }

        public void CLOSE(string clientname, string filename, string primaryPort)
        {
            IMDServer mdsclose = (IMDServer)Activator.GetObject(typeof(IMDServer)
            , "tcp://localhost:" + primaryPort + "/MetaData_Server");
            mdsclose.CLOSE(filename);
        }

        public string READ(string clientname, string filename, string semantics)
        {
            string result = null;
            foreach (KeyValuePair<KeyValuePair<int, string>, object[]> value in filesClient)
            {

                if (value.Key.Key.Equals(filename))
                {
                    List<KeyValuePair<string, string>> servers = (List<KeyValuePair<string, string>>)value.Value[4];
                    foreach (KeyValuePair<string, string> server in servers)
                    {
                        IDServer dsread = (IDServer)Activator.GetObject(typeof(IDServer), "tcp://localhost:" + server.Value + "/Data_Server");
                        result = dsread.READ(filename, semantics);
                    }
                }
            }
            System.Console.WriteLine("File: " + filename + " Content: " + result);
            return result;
        }

        public void WRITE(string clientname, string filename, byte[] content)
        {
            foreach (KeyValuePair<KeyValuePair<int, string>, object[]> value in filesClient)
            {

                if (value.Key.Key.Equals(filename))
                {
                    List<KeyValuePair<string, string>> servers = (List<KeyValuePair<string, string>>)value.Value[4];
                    foreach (KeyValuePair<string, string> server in servers)
                    {
                        IDServer dswrite = (IDServer)Activator.GetObject(typeof(IDServer)
                        , "tcp://localhost:807" + server.Value + "/Data_Server");
                        dswrite.WRITE(filename, content);
                    }
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
            string readResult = READ(clientname, fileregister1, semantics);
            WRITE(clientname, fileregister2, Encoding.ASCII.GetBytes(readResult + salt));
        }

        public void DUMP()
        {
            System.Console.WriteLine("Client name: " + this.clientname + "\r\n");

            System.Console.WriteLine("File List opened by this client: \r\n");

            foreach (KeyValuePair<KeyValuePair<int, string>, object[]> file in filesClient)
            {
                System.Console.WriteLine("File: " + file.Key.Key + " Content: " + file.Key.Value + "\r\n");
            }
        }

        public void EXESCRIPT(string clientname, string script)
        {
            string[] scriptFile = System.IO.File.ReadAllLines(@script);

            foreach (string command in scriptFile)
            {
                executeCommand(command);
            }
        }

        private void executeCommand(string commandline)
        {
            string[] command = commandline.Split(' ', ',');

            switch (command[0])
            {

                case "OPEN":
                    OPEN(command[1], command[3],command[5]);
                    break;
                case "CLOSE":
                    CLOSE(command[1], command[3],command[5]);
                    break;
                case "READ":
                    READ(command[1], command[3], command[5]);
                    break;
                case "WRITE":
                    WRITE(command[1], command[3], Encoding.ASCII.GetBytes(command[5]));
                    break;
                case "DUMP":
                    DUMP();
                    break;
                case "DELETE":
                    DELETE(command[1], command[3]);
                    break;
                case "EXESCRIPT":
                    EXESCRIPT(command[1], command[3]);
                    break;
                default:
                    break;
            }
        }
    }
}
