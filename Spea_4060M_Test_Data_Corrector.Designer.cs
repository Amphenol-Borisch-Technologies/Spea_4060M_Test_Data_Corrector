namespace Spea_4060M_Test_Data_Corrector {
    partial class Spea_4060M_Test_Data_Corrector {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.txtTestData = new System.Windows.Forms.RichTextBox();
            this.BtnDoNotCorrectData = new System.Windows.Forms.Button();
            this.BtnCorrectData = new System.Windows.Forms.Button();
            this.BtnOpenTestData = new System.Windows.Forms.Button();
            this.lblTestData = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtTestData
            // 
            this.txtTestData.Location = new System.Drawing.Point(54, 127);
            this.txtTestData.Name = "txtTestData";
            this.txtTestData.Size = new System.Drawing.Size(1613, 63);
            this.txtTestData.TabIndex = 3;
            this.txtTestData.TabStop = false;
            this.txtTestData.Text = "";
            // 
            // BtnDoNotCorrectData
            // 
            this.BtnDoNotCorrectData.BackColor = System.Drawing.Color.Red;
            this.BtnDoNotCorrectData.Enabled = false;
            this.BtnDoNotCorrectData.Location = new System.Drawing.Point(888, 227);
            this.BtnDoNotCorrectData.Name = "BtnDoNotCorrectData";
            this.BtnDoNotCorrectData.Size = new System.Drawing.Size(177, 50);
            this.BtnDoNotCorrectData.TabIndex = 2;
            this.BtnDoNotCorrectData.Text = "Don\'t Correct Data";
            this.BtnDoNotCorrectData.UseVisualStyleBackColor = false;
            this.BtnDoNotCorrectData.Click += new System.EventHandler(this.BtnDoNotCorrectData_Click);
            // 
            // BtnCorrectData
            // 
            this.BtnCorrectData.BackColor = System.Drawing.Color.ForestGreen;
            this.BtnCorrectData.Enabled = false;
            this.BtnCorrectData.ForeColor = System.Drawing.SystemColors.ControlText;
            this.BtnCorrectData.Location = new System.Drawing.Point(617, 227);
            this.BtnCorrectData.Name = "BtnCorrectData";
            this.BtnCorrectData.Size = new System.Drawing.Size(197, 50);
            this.BtnCorrectData.TabIndex = 1;
            this.BtnCorrectData.Text = "Correct Data";
            this.BtnCorrectData.UseVisualStyleBackColor = false;
            this.BtnCorrectData.Click += new System.EventHandler(this.BtnCorrectData_Click);
            // 
            // BtnOpenTestData
            // 
            this.BtnOpenTestData.Location = new System.Drawing.Point(54, 25);
            this.BtnOpenTestData.Name = "BtnOpenTestData";
            this.BtnOpenTestData.Size = new System.Drawing.Size(156, 50);
            this.BtnOpenTestData.TabIndex = 0;
            this.BtnOpenTestData.Text = "Open Test Data";
            this.BtnOpenTestData.UseVisualStyleBackColor = true;
            this.BtnOpenTestData.Click += new System.EventHandler(this.BtnOpenTestData_Click);
            // 
            // lblTestData
            // 
            this.lblTestData.AutoSize = true;
            this.lblTestData.Location = new System.Drawing.Point(51, 108);
            this.lblTestData.Name = "lblTestData";
            this.lblTestData.Size = new System.Drawing.Size(66, 16);
            this.lblTestData.TabIndex = 4;
            this.lblTestData.Text = "Test Data";
            // 
            // Spea_4060M_Test_Data_Corrector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1707, 314);
            this.Controls.Add(this.lblTestData);
            this.Controls.Add(this.BtnOpenTestData);
            this.Controls.Add(this.BtnCorrectData);
            this.Controls.Add(this.BtnDoNotCorrectData);
            this.Controls.Add(this.txtTestData);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.MaximizeBox = false;
            this.Name = "Spea_4060M_Test_Data_Corrector";
            this.Text = "Spea 4060M Test Data Corrector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtTestData;
        private System.Windows.Forms.Button BtnDoNotCorrectData;
        private System.Windows.Forms.Button BtnCorrectData;
        private System.Windows.Forms.Button BtnOpenTestData;
        private System.Windows.Forms.Label lblTestData;
    }
}

