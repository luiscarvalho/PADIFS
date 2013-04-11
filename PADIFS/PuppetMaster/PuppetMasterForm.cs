using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMaster
{
    public partial class PuppetMasterForm : Form
    {

        string[] script;
        Hashtable metadataList = new Hashtable();
        Hashtable clientList = new Hashtable();
        Hashtable dataserverList = new Hashtable();

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

            if (scriptPath == null)
            {
                infoTX.Text = infoTX.Text + "You must enter a file name!" + "\r\n";
            }
            else
            {
                infoTX.Text = scriptPath;
             //   string[] script = System.IO.File.ReadAllLines(scriptPath);
            }
        }

        private void runbt_Click(object sender, EventArgs e)
        {
            foreach (string command in script)
            {
                executeCommand(command);
            }
        }


        private void stepbt_Click(object sender, EventArgs e)
        {

        }

        private void executeCommand(string commandline)
        {
            string[] command = commandline.Split(' ');

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
                    //open(command);
                    break;
                case "CLOSE":
                    //closeFile(command);
                    break;
                case "READ":
                    //read(command);
                    break;
                case "WRITE":
                    //write(command);
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

        private void Unfreeze(string[] command)
        {
            if (dataserverList.Contains(command[1]))
            {
                // Unfreeze data server
            }
            else
            {
                // comando que lança um processo... ainda não sei bem como funciona 
                System.Diagnostics.Process.Start(".\\Data_Server\\bin\\Release\\Data_Server.exe", "parametros-por exemplo o endereço");
                infoTX.Text = infoTX.Text + "Start data server: " + command[1] + "with address: " + "xxxxx" + "\r\n";
                dataserverList.Add(command[1], "endereço");
            }
        }

        private void Recover(string[] command)
        {

            if (metadataList.Contains(command[1]))
            {
                // Recover metadata
            }
            else
            {
                // comando que lança um processo... ainda não sei bem como funciona 
                System.Diagnostics.Process.Start(".\\Metadata_Server\\bin\\Release\\Metadata_Server.exe", "parametros-por exemplo o endereço");
                infoTX.Text = infoTX.Text + "Start metadata server: " + command[1] + "with address: " + "xxxxx" + "\r\n";
                metadataList.Add(command[1], "endereço");
            }

        }

        private void Create(string[] command)
        {
            if (clientList.Contains(command[1]))
            {
                // Commands client to create a file
            }
            else
            {
                // comando que lança um processo... ainda não sei bem como funciona 
                System.Diagnostics.Process.Start(".\\Client\\bin\\Release\\Client.exe", "parametros-por exemplo o endereço");
                infoTX.Text = infoTX.Text + "Start Client: " + command[1] + "with address: " + "xxxxx" + "\r\n";
                metadataList.Add(command[1], "endereço");
            }
        }
    }
}
