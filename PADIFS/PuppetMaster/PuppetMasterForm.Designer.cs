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
            this.loadbt = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.stepbt = new System.Windows.Forms.Button();
            this.runbt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // loadbt
            // 
            this.loadbt.Location = new System.Drawing.Point(12, 38);
            this.loadbt.Name = "loadbt";
            this.loadbt.Size = new System.Drawing.Size(75, 23);
            this.loadbt.TabIndex = 0;
            this.loadbt.Text = "Load Script";
            this.loadbt.UseVisualStyleBackColor = true;
            this.loadbt.Click += new System.EventHandler(this.loadbt_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(237, 20);
            this.textBox1.TabIndex = 1;
            // 
            // stepbt
            // 
            this.stepbt.Location = new System.Drawing.Point(174, 38);
            this.stepbt.Name = "stepbt";
            this.stepbt.Size = new System.Drawing.Size(75, 23);
            this.stepbt.TabIndex = 2;
            this.stepbt.Text = "Next Step";
            this.stepbt.UseVisualStyleBackColor = true;
            this.stepbt.Click += new System.EventHandler(this.stepbt_Click);
            // 
            // runbt
            // 
            this.runbt.Location = new System.Drawing.Point(93, 38);
            this.runbt.Name = "runbt";
            this.runbt.Size = new System.Drawing.Size(75, 23);
            this.runbt.TabIndex = 3;
            this.runbt.Text = "Run";
            this.runbt.UseVisualStyleBackColor = true;
            this.runbt.Click += new System.EventHandler(this.runbt_Click);
            // 
            // PuppetMasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 142);
            this.Controls.Add(this.runbt);
            this.Controls.Add(this.stepbt);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.loadbt);
            this.Name = "PuppetMasterForm";
            this.Text = "PuppetMaster";
            this.Load += new System.EventHandler(this.PuppetMasterForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button loadbt;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button stepbt;
        private System.Windows.Forms.Button runbt;
    }
}
