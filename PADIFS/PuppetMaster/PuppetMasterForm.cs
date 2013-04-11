using PADICommonTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PuppetMaster
{
    public partial class PuppetMasterForm : Form
    {

        private string[] script;
        Hashtable metadataList = new Hashtable();
        Hashtable clientList = new Hashtable();
        Hashtable dataserverList = new Hashtable();
        private DebugDelegate debug;

        public PuppetMasterForm()
        {
            InitializeComponent();

        }

        private void PuppetMasterForm_Load(object sender, EventArgs e)
        {

        }

        private void loadbt_Click(object sender, EventArgs e)
        {
            string scriptPath = scriptPathTX.Text;

            if (scriptPath.Equals(""))
            {
                infoTX.Text = infoTX.Text + "You must enter a file name!" + "\r\n";
            }
            else
            {
                infoTX.Text = infoTX.Text + scriptPath + "\r\n";
                //C:\Users\Luís\Desktop\sample_script_checkpoint.txt
                System.IO.File.OpenRead(@scriptPath);
                script = System.IO.File.ReadAllLines(@scriptPath);
            }
        }

        private void runbt_Click(object sender, EventArgs e)
        {
            if (script == null)
            {
                infoTX.Text = infoTX.Text + "Erro na leitura do script!" + "\r\n";
            }
            else
            {
                foreach (string command in script)
                {
                    infoTX.Text = infoTX.Text + command;
                    executeCommand(command);
                }
            }
        }


        private void stepbt_Click(object sender, EventArgs e)
        {

        }

        private void executeCommand(string commandline)
        {
            string[] command = commandline.Split(' ', ',');

            switch (command[0])
            {

                case "RECOVER":
                    Recover(command);
                    break;
                case "UNFREEZE":
                    Unfreeze(command);
                    break;
                case "FAIL":
                    //Fail(command);
                    break;
                case "FREEZE":
                    //Freeze(command);
                    break;
                case "CREATE":
                    Create(command);
                    break;
                case "OPEN":
                    Open(command);
                    break;
                case "CLOSE":
                    //closeFile(command);
                    break;
                case "READ":
                    //read(command);
                    break;
                case "WRITE":
                    Write(command);
                    break;
                case "COPY":
                    //copy(command);
                    break;
                case "DUMP":
                    //dump(command);
                    break;
                case "EXESCRIPT":
                    //exescript(command);
                    break;
                default:
                    break;
            }
        }

        private void Write(string[] command)
        {
            if (clientList.Contains(command[1]))
            {
                // Commands client to create a file
                IClient cWrite = (IClient)Activator.GetObject(typeof(IClient)
                   , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                byte[] text = Encoding.ASCII.GetBytes(command[3]);
                //byte[] text = command[3].Split(' ' ).Select(s => Convert.ToByte(s, 16)).ToArray();
                cWrite.WRITE(command[1], command[2], text, debug); 
            }
            else
            {
                // comando que lança um processo clien... NESTE CASO NAO SEI SE LANCAMOS O PROCESSO CLIENTE
                string[] nclient = command[1].Split('-');
                System.Diagnostics.Process.Start(".\\Client\\bin\\Debug\\Client.exe", command[1] + " 808" + nclient[1]);
                infoTX.Text = infoTX.Text + "Start Client: " + command[1] + "with port: " + " 807" + nclient[1] + "\r\n";
                metadataList.Add(command[1], "807" + nclient[1]);
                IClient cWrite = (IClient)Activator.GetObject(typeof(IClient)
                   , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                byte[] text = Encoding.ASCII.GetBytes(command[3]);
                //byte[] text = command[3].Split(' ' ).Select(s => Convert.ToByte(s, 16)).ToArray();
                cWrite.WRITE(command[1], command[2], text, debug); 
            }
        }

        private void Unfreeze(string[] command)
        {
            if(dataserverList.Contains(command[1]))
            {
                // Unfreeze data server
                IDServer dsUnfreeze = (IDServer)Activator.GetObject(typeof(IDServer)
                   , "tcp://localhost:" + dataserverList[command[1]] + "/ClientRemote");
                dsUnfreeze.UNFREEZE(command[1], debug);
            }
            else
            {
                // comando que lança um processo dataserver
                string[] nserver = command[1].Split('-');
                System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + "\\DataServer\\bin\\Debug\\DataServer.exe", command[1].ToString() + " :807" + nserver[1].ToString());
                infoTX.Text = infoTX.Text + "Start data server: " + command[1] + "with port: " + "807" + nserver[1] + "\r\n";
                dataserverList.Add(command[1], "807" + nserver[1]);
            }
        }

        private void Recover(string[] command)
        {

            if(metadataList.Contains(command[1]))
            {
                IMDServer mdsrecover = (IMDServer)Activator.GetObject(typeof(IMDServer)
                    , "tcp://localhost:" + metadataList[command[1]] + "/MetaData_Server");
                mdsrecover.RECOVER(" ", new DebugDelegate(debug));
            }
            else
            {
                // comando que lança um processo metadata server
                string[] nserver = command[1].Split('-');
                infoTX.Text += Directory.GetCurrentDirectory().ToString() + "\r\n";
                System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + "\\Metadata_Server\\bin\\Debug\\Metadata_Server.exe", command[1] + " " + "808" + nserver[1]);
                infoTX.Text = infoTX.Text + "Start metadata server: " + command[1] + "with port: " + "808" + nserver[1] + "\r\n";
                metadataList.Add(command[1], "808" + nserver[1]);
            }

        }

        private void Create(string[] command)
        {
            if(clientList.Contains(command[1]))
            {
                // Commands client to create a file
                IClient cCreate = (IClient)Activator.GetObject(typeof(IClient)
                   , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cCreate.CREATE(command[1], command[2], Convert.ToInt32(command[3]), Convert.ToInt32(command[4]), Convert.ToInt32(command[5]), debug);
            }
            else
            {
                // comando que lança um processo client
                string[] nclient = command[1].Split('-');
                System.Diagnostics.Process.Start(".\\Client\\bin\\Debug\\Client.exe", command[1] + " 808" + nclient[1]);
                infoTX.Text = infoTX.Text + "Start Client: " + command[1] + "with port: " + " 807" + nclient[1] + "\r\n";
                metadataList.Add(command[1], "807" + nclient[1]);
                IClient cCreate = (IClient)Activator.GetObject(typeof(IClient)
                    , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cCreate.CREATE(command[1], command[2], Convert.ToInt32(command[3]), Convert.ToInt32(command[4]), Convert.ToInt32(command[5]), debug);
            }
        }

        private void Open(string[] command)
        {
            if(clientList.Contains(command[1]))
            {
                // Commands client to create a file
                IClient cOpen = (IClient)Activator.GetObject(typeof(IClient)
                   , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cOpen.OPEN(command[1], command[2], debug);
            }
            else
            {
                // comando que lança um processo client
                string[] nclient = command[1].Split('-');
                System.Diagnostics.Process.Start(".\\Client\\bin\\Debug\\Client.exe", command[1] + " 808" + nclient[1]);
                infoTX.Text = infoTX.Text + "Start Client: " + command[1] + "with port: " + " 807" + nclient[1] + "\r\n";
                metadataList.Add(command[1], "807" + nclient[1]);
                IClient cOpen = (IClient)Activator.GetObject(typeof(IClient)
                    , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cOpen.OPEN(command[1], command[2], debug);
            }
        }
    }

}
