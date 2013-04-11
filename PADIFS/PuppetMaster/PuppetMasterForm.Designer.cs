namespace PuppetMaster
{
    partial class PuppetMasterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.loadBT = new System.Windows.Forms.Button();
            this.scriptPathTX = new System.Windows.Forms.TextBox();
            this.stepBT = new System.Windows.Forms.Button();
            this.runBT = new System.Windows.Forms.Button();
            this.infoTX = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // loadBT
            // 
            this.loadBT.Location = new System.Drawing.Point(77, 47);
            this.loadBT.Name = "loadBT";
            this.loadBT.Size = new System.Drawing.Size(75, 23);
            this.loadBT.TabIndex = 0;
            this.loadBT.Text = "Load Script";
            this.loadBT.UseVisualStyleBackColor = true;
            this.loadBT.Click += new System.EventHandler(this.loadbt_Click);
            // 
            // scriptPathTX
            // 
            this.scriptPathTX.Location = new System.Drawing.Point(12, 12);
            this.scriptPathTX.Name = "scriptPathTX";
            this.scriptPathTX.Size = new System.Drawing.Size(525, 20);
            this.scriptPathTX.TabIndex = 1;
            // 
            // stepBT
            // 
            this.stepBT.Location = new System.Drawing.Point(383, 47);
            this.stepBT.Name = "stepBT";
            this.stepBT.Size = new System.Drawing.Size(75, 23);
            this.stepBT.TabIndex = 2;
            this.stepBT.Text = "Next Step";
            this.stepBT.UseVisualStyleBackColor = true;
            this.stepBT.Click += new System.EventHandler(this.stepbt_Click);
            // 
            // runBT
            // 
            this.runBT.Location = new System.Drawing.Point(233, 47);
            this.runBT.Name = "runBT";
            this.runBT.Size = new System.Drawing.Size(75, 23);
            this.runBT.TabIndex = 3;
            this.runBT.Text = "Run";
            this.runBT.UseVisualStyleBackColor = true;
            this.runBT.Click += new System.EventHandler(this.runbt_Click);
            // 
            // infoTX
            // 
            this.infoTX.Location = new System.Drawing.Point(25, 97);
            this.infoTX.Multiline = true;
            this.infoTX.Name = "infoTX";
            this.infoTX.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.infoTX.Size = new System.Drawing.Size(496, 399);
            this.infoTX.TabIndex = 4;
            // 
            // PuppetMasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 543);
            this.Controls.Add(this.infoTX);
            this.Controls.Add(this.runBT);
            this.Controls.Add(this.stepBT);
            this.Controls.Add(this.scriptPathTX);
            this.Controls.Add(this.loadBT);
            this.Name = "PuppetMasterForm";
            this.Text = "PuppetMaster";
            this.Load += new System.EventHandler(this.PuppetMasterForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadBT;
        private System.Windows.Forms.TextBox scriptPathTX;
        private System.Windows.Forms.Button stepBT;
        private System.Windows.Forms.Button runBT;
        private System.Windows.Forms.TextBox infoTX;
    }
}
