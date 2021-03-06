﻿using PADICommonTypes;
using Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.Remoting.Proxies;
using System.Diagnostics;

namespace PuppetMaster
{
    public partial class PuppetMasterForm : Form
    {

        private string[] script;
        Hashtable metadataList = new Hashtable();
        Hashtable clientList = new Hashtable();
        Hashtable dataserverList = new Hashtable();
        Hashtable processList = new Hashtable();
        List<KeyValuePair<int, object[]>> filesRegister = new List<KeyValuePair<int, object[]>>();
        int fileR = 0;
        string primaryMDserver;
        private List<KeyValuePair<int, string>> localStringRegister = new List<KeyValuePair<int, string>>();


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
                    //infoTX.Text = infoTX.Text + command;
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
                    Fail(command);
                    break;
                case "FREEZE":
                    Freeze(command);
                    break;
                case "CREATE":
                    Create(command);
                    break;
                case "OPEN":
                    Open(command);
                    break;
                case "CLOSE":
                    Close(command);
                    break;
                case "READ":
                    Read(command);
                    break;
                case "WRITE":
                    Write(command);
                    break;
                case "COPY":
                    Copy(command);
                    break;
                case "DUMP":
                    Dump(command);
                    break;
                case "DELETE":
                    Delete(command);
                    break;
                case "EXESCRIPT":
                    ExeScript(command);
                    break;
                default:
                    break;
            }

        }

        private void Fail(string[] command)
        {
            if (metadataList.Contains(command[1]))
            {
                IMDServer mdsFail = (IMDServer)Activator.GetObject(typeof(IMDServer)
                   , "tcp://localhost:" + metadataList[command[1]] + "/MetaData_Server");
                mdsFail.FAIL(command[1]);
                metadataList.Remove(command[1]);
            }
            else if (dataserverList.Contains(command[1]))
            {
                IDServer dsFail = (IDServer)Activator.GetObject(typeof(IDServer)
                    , "tcp://localhost:" + dataserverList[command[1]] + "/Data_Server");
                dsFail.FAIL(command[1]);
            }
            else
            {
                infoTX.Text = infoTX.Text + "The process " + command[1] + "does not exist!";
            }
        }

        private void ExeScript(string[] command)
        {
            if (clientList.Contains(command[1]))
            {
                IClient cExeScript = (IClient)Activator.GetObject(typeof(IClient)
                    , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cExeScript.EXESCRIPT(command[1], command[2]);
            }
            else
            {
                infoTX.Text = infoTX.Text + "Client " + command[1] + "does not exist!";
            }
        }

        private void Delete(string[] command)
        {
            if (clientList.Contains(command[1]))
            {
                IClient cDelete = (IClient)Activator.GetObject(typeof(IClient)
                    , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cDelete.DELETE(command[1], command[2]);
            }
            else
            {
                infoTX.Text = infoTX.Text + "Client " + command[1] + "does not exist!";
            }
        }

        private void Close(string[] command)
        {
            if (clientList.Contains(command[1]))
            {
                IClient cClose = (IClient)Activator.GetObject(typeof(IClient)
                    , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cClose.CLOSE(command[1], command[2], metadataList[this.primaryMDserver].ToString());
            }
            else
            {
                infoTX.Text = infoTX.Text + "Client " + command[1] + "does not exist!";
            }
        }

        private void Dump(string[] command)
        {
            if (clientList.Contains(command[1]))
            {
                IClient cDump = (IClient)Activator.GetObject(typeof(IClient)
                    , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cDump.DUMP();
            }
            else if (metadataList.Contains(command[1]))
            {
                IMDServer mdsDump = (IMDServer)Activator.GetObject(typeof(IMDServer)
                   , "tcp://localhost:" + metadataList[command[1]] + "/MetaData_Server");
                mdsDump.DUMP();
            }
            else if (dataserverList.Contains(command[1]))
            {
                IDServer dsDump = (IDServer)Activator.GetObject(typeof(IDServer)
                    , "tcp://localhost:" + dataserverList[command[1]] + "/Data_Server");
                dsDump.DUMP();
            }
            else
            {
                infoTX.Text = infoTX.Text + "The process " + command[1] + "does not exist!";
            }
        }

        private void Copy(string[] command)
        {
            if (clientList.Contains(command[1]))
            {
                IClient cCopy = (IClient)Activator.GetObject(typeof(IClient)
                    , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cCopy.COPY(command[1], command[3], command[5], command[7], command[8]);
            }
            else
            {
                infoTX.Text = infoTX.Text + "Client " + command[1] + "does not exist!";
            }
        }

        private void Freeze(string[] command)
        {
            if (dataserverList.Contains(command[1]))
            {
                // Freeze data server
                IDServer dsFreeze = (IDServer)Activator.GetObject(typeof(IDServer)
                   , "tcp://localhost:" + dataserverList[command[1]] + "/Data_Server");
                dsFreeze.FREEZE(command[1]);
            }
            else
            {
                infoTX.Text = infoTX.Text + "Data Server " + command[1] + "does not exist!";
            }
        }

        private void Read(string[] command)
        {
            string result = null;
            if (clientList.Contains(command[1]))
            {
                // Commands client to read a file
                IClient cRead = (IClient)Activator.GetObject(typeof(IClient)
                   , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                // O metodo READ tem de devolver o conteudo do ficheiro e guarda-lo numa string[] com o nome commmand[4]
                result = cRead.READ(command[1], command[3], command[5]);
            }
            else
            {
                // comando que lança um processo clien... NESTE CASO NAO SEI SE LANCAMOS O PROCESSO CLIENTE
                string[] nclient = command[1].Split('-');
                System.Diagnostics.Process.Start(".\\Client\\bin\\Debug\\Client.exe", command[1] + " 806" + nclient[1]);
                infoTX.Text = infoTX.Text + "Start Client: " + command[1] + "with port: " + " 806" + nclient[1] + "\r\n";
                clientList.Add(command[1], "806" + nclient[1]);
                IClient cRead = (IClient)Activator.GetObject(typeof(IClient)
                   , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                // O metodo READ tem de devolver o conteudo do ficheiro e guarda-lo numa string[] com o nome commmand[4]
                result = cRead.READ(command[1], command[3], command[5]);
            }
            localStringRegister.Add(new KeyValuePair<int, string>(int.Parse(command[7]), result));
            infoTX.Text = infoTX.Text + "Localfile: " + command[3] + " Result: " + result;
        }

        private void Write(string[] command)
        {
            if (clientList.Contains(command[1]))
            {
                // Commands client to write a file
                IClient cWrite = (IClient)Activator.GetObject(typeof(IClient)
                   , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                string textResult = "";
                for (int i = 5; i < command.Length; i++)
                {
                    textResult += " " + command[i];
                }
                infoTX.Text = infoTX.Text + "Text to Write" + textResult + "\r\n";
                //byte[] text = command[3].Split(' ' ).Select(s => Convert.ToByte(s, 16)).ToArray();
                cWrite.WRITE(command[1], command[3], Encoding.ASCII.GetBytes(textResult));

            }
            else
            {
                // comando que lança um processo clien... NESTE CASO NAO SEI SE LANCAMOS O PROCESSO CLIENTE
                string[] nclient = command[1].Split('-');
                System.Diagnostics.Process.Start(".\\Client\\bin\\Debug\\Client.exe", command[1] + " 806" + nclient[1]);
                infoTX.Text = infoTX.Text + "Start Client: " + command[1] + " with port: " + " 806" + nclient[1] + "\r\n";
                clientList.Add(command[1], "806" + nclient[1]);
                IClient cWrite = (IClient)Activator.GetObject(typeof(IClient)
                   , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                string textResult = "";
                for (int i = 5; i < command.Length; i++)
                {
                    textResult += " " + command[i];
                }
                infoTX.Text = infoTX.Text + "Text to Write" + textResult + "\r\n";
                //byte[] text = command[3].Split(' ' ).Select(s => Convert.ToByte(s, 16)).ToArray();
                cWrite.WRITE(command[1], command[3], Encoding.ASCII.GetBytes(textResult));
            }
        }

        private void Unfreeze(string[] command)
        {
            if (dataserverList.Contains(command[1]))
            {
                // Unfreeze data server
                IDServer dsUnfreeze = (IDServer)Activator.GetObject(typeof(IDServer)
                   , "tcp://localhost:" + dataserverList[command[1]] + "/Data_Server");
                dsUnfreeze.UNFREEZE(command[1]);
            }
            else
            {
                // comando que lança um processo dataserver
                string[] nserver = command[1].Split('-');
                System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + "\\DataServer\\bin\\Debug\\DataServer.exe",
                    command[1].ToString() + " 807" + nserver[1].ToString() + " " + metadataList[this.primaryMDserver].ToString());
                infoTX.Text = infoTX.Text + "Start data server: " + command[1] + " with port: " + "807" + nserver[1] + "\r\n";
                dataserverList.Add(command[1], "807" + nserver[1]);
            }
        }

        private void Recover(string[] command)
        {
            // comando que lança um processo metadata server
            string[] nserver = command[1].Split('-');

            if (metadataList.Contains(command[1]))
            {
                infoTX.Text += "O metadata server: " + command[1] + "já se encontra online" + "\r\n";
                return;
            }

            infoTX.Text += Directory.GetCurrentDirectory().ToString() + "\r\n";
            Process mdRecover = System.Diagnostics.Process.Start(Directory.GetCurrentDirectory() + "\\Metadata_Server\\bin\\Debug\\Metadata_Server.exe", command[1] + " " + " 808" + nserver[1]);
            infoTX.Text = infoTX.Text + "Start metadata server: " + command[1] + " with port: " + "808" + nserver[1] + "\r\n";
            metadataList.Add(command[1], "808" + nserver[1]);
            processList.Add("808" + nserver[1], mdRecover.ProcessName);

            if (metadataList.Count > 1)
            {
                IMDServer mdsPrimaryMDServer = (IMDServer)Activator.GetObject(typeof(IMDServer)
                                       , "tcp://localhost:" + metadataList[command[1]] + "/MetaData_Server");
                mdsPrimaryMDServer.primaryMDServer(primaryMDserver, metadataList[this.primaryMDserver].ToString());
            }
            else
            {
                primaryMDserver = command[1];
            }
        }

        private void Create(string[] command)
        {
            object[] dataRow = null;
            if (clientList.Contains(command[1]))
            {
                // Commands client to create a file
                try
                {
                    IClient cCreate = (IClient)Activator.GetObject(typeof(IClient)
                       , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                    dataRow = cCreate.CREATE(command[1], command[3], Convert.ToInt32(command[5]), Convert.ToInt32(command[7]), Convert.ToInt32(command[9]), metadataList[this.primaryMDserver].ToString());
                    filesRegister.Add(new KeyValuePair<int, object[]>(fileR, dataRow));
                    showInfo(fileR, dataRow);
                    fileR++;
                }
                catch (RemotingException ex)
                {
                    infoTX.Text = infoTX.Text + ex.Message + "\r\n";
                    // O primário falhou temos de escolher outro como primário
                    metadataList.Remove(primaryMDserver);
                    if (metadataList.Count > 0)
                    {
                        foreach (string mdserver in metadataList.Keys)
                        {
                            primaryMDserver = mdserver;
                            break;
                        }
                    }
                }
            }
            else
            {
                // comando que lança um processo client
                string[] nclient = command[1].Split('-');
                System.Diagnostics.Process.Start(".\\Client\\bin\\Debug\\Client.exe", command[1] + " 806" + nclient[1]);
                infoTX.Text = infoTX.Text + "Start Client: " + command[1] + " with port: " + " 806" + nclient[1] + "\r\n";
                clientList.Add(command[1], "806" + nclient[1]);
                try
                {
                    IClient cCreate = (IClient)Activator.GetObject(typeof(IClient)
                        , "tcp://localhost:" + clientList[command[1]].ToString() + "/ClientRemote");
                    Thread.Sleep(1000);
                    dataRow = cCreate.CREATE(command[1], command[3], Convert.ToInt32(command[5]), Convert.ToInt32(command[7]), Convert.ToInt32(command[9]), metadataList[this.primaryMDserver].ToString());
                    filesRegister.Add(new KeyValuePair<int, object[]>(fileR, dataRow));
                    showInfo(fileR, dataRow);
                    fileR++;
                }
                catch (RemotingException ex)
                {
                    infoTX.Text = infoTX.Text + ex.Message + "\r\n";
                    // O primário falhou temos de escolher outro como primário
                    metadataList.Remove(primaryMDserver);
                    if (metadataList.Count > 0)
                    {
                        foreach (string mdserver in metadataList.Keys)
                        {
                            primaryMDserver = mdserver;
                            break;
                        }
                    }
                }
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        private void showInfo(int fileR, object[] dataRow)
        {
            infoTX.Text = infoTX.Text + "File-Register " + fileR + " info:\r\n";
            infoTX.Text = infoTX.Text + "Filename - " + dataRow[0] + "\r\n";
            infoTX.Text = infoTX.Text + "Número de Data Servers - " + dataRow[1] + "\r\n";
            infoTX.Text = infoTX.Text + "Read Quorum - " + dataRow[2] + "\r\n";
            infoTX.Text = infoTX.Text + "Write Quorum - " + dataRow[3] + "\r\n";
            infoTX.Text = infoTX.Text + "Data Servers:\r\n";
            foreach (KeyValuePair<string, byte[]> dserver in (List<KeyValuePair<string, byte[]>>) dataRow[5])
            {
                infoTX.Text = infoTX.Text + dserver.Key + " - " + ByteArrayToString(dserver.Value) + "\r\n";
            }
        }

        private void Open(string[] command)
        {
            if (clientList.Contains(command[1]))
            {
                // Commands client to create a file
                IClient cOpen = (IClient)Activator.GetObject(typeof(IClient)
                   , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cOpen.OPEN(command[1], command[3], metadataList[this.primaryMDserver].ToString());
            }
            else
            {
                // comando que lança um processo client
                string[] nclient = command[1].Split('-');
                System.Diagnostics.Process.Start(".\\Client\\bin\\Debug\\Client.exe", command[1] + " 806" + nclient[1]);
                infoTX.Text = infoTX.Text + "Start Client: " + command[1] + "with port: " + " 806" + nclient[1] + "\r\n";
                clientList.Add(command[1], "806" + nclient[1]);
                IClient cOpen = (IClient)Activator.GetObject(typeof(IClient)
                    , "tcp://localhost:" + clientList[command[1]] + "/ClientRemote");
                cOpen.OPEN(command[1], command[3], metadataList[this.primaryMDserver].ToString());
            }
        }
    }

}
