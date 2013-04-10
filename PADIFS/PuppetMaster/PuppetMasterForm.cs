using System;
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
            { }
            else
            {
                string[] script = System.IO.File.ReadAllLines(scriptPath);
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
                    recover(command);
                    break;
                case "UNFREEZE":
                    unfreeze(command);
                    break;
                case "FAIL":
                    fail(command);
                    break;
                case "FREEZE":
                    freeze(command);
                    break;
                case "CREATE":
                    create(command);
                    break;
                case "OPEN":
                    open(command);
                    break;
                case "CLOSE":
                    closeFile(command);
                    break;
                case "READ":
                    read(command);
                    break;
                case "WRITE":
                    write(command);
                    break;
                case "COPY":
                    copy(command);
                    break;
                case "DUMP":
                    dump(command);
                    break;
                case "EXESCRIPT":
                    exescript(command);
                    break;
                default:
                    break;
            }
        }

        private void unfreeze(string[] command)
        {
            throw new NotImplementedException();
        }

        private void recover(string[] command)
        {
            throw new NotImplementedException();
        }
    }
}
