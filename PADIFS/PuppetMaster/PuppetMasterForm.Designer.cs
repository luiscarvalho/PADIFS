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
            this.SuspendLayout();
            // 
            // loadbt
            // 
            this.loadBT.Location = new System.Drawing.Point(12, 38);
            this.loadBT.Name = "loadbt";
            this.loadBT.Size = new System.Drawing.Size(75, 23);
            this.loadBT.TabIndex = 0;
            this.loadBT.Text = "Load Script";
            this.loadBT.UseVisualStyleBackColor = true;
            this.loadBT.Click += new System.EventHandler(this.loadbt_Click);
            // 
            // textBox1
            // 
            this.scriptPathTX.Location = new System.Drawing.Point(12, 12);
            this.scriptPathTX.Name = "textBox1";
            this.scriptPathTX.Size = new System.Drawing.Size(237, 20);
            this.scriptPathTX.TabIndex = 1;
            // 
            // stepbt
            // 
            this.stepBT.Location = new System.Drawing.Point(174, 38);
            this.stepBT.Name = "stepbt";
            this.stepBT.Size = new System.Drawing.Size(75, 23);
            this.stepBT.TabIndex = 2;
            this.stepBT.Text = "Next Step";
            this.stepBT.UseVisualStyleBackColor = true;
            this.stepBT.Click += new System.EventHandler(this.stepbt_Click);
            // 
            // runbt
            // 
            this.runBT.Location = new System.Drawing.Point(93, 38);
            this.runBT.Name = "runbt";
            this.runBT.Size = new System.Drawing.Size(75, 23);
            this.runBT.TabIndex = 3;
            this.runBT.Text = "Run";
            this.runBT.UseVisualStyleBackColor = true;
            this.runBT.Click += new System.EventHandler(this.runbt_Click);
            // 
            // PuppetMasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 142);
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
    }
}
