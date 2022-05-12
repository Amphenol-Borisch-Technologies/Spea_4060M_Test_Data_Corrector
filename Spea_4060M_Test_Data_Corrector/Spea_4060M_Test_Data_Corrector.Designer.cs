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
            this.btnDoNotCorrectData = new System.Windows.Forms.Button();
            this.btnCorrectData = new System.Windows.Forms.Button();
            this.btnOpenTestData = new System.Windows.Forms.Button();
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
            // btnDoNotCorrectData
            // 
            this.btnDoNotCorrectData.BackColor = System.Drawing.Color.Red;
            this.btnDoNotCorrectData.Enabled = false;
            this.btnDoNotCorrectData.Location = new System.Drawing.Point(888, 227);
            this.btnDoNotCorrectData.Name = "btnDoNotCorrectData";
            this.btnDoNotCorrectData.Size = new System.Drawing.Size(177, 50);
            this.btnDoNotCorrectData.TabIndex = 2;
            this.btnDoNotCorrectData.Text = @"Don't Correct Data";
            this.btnDoNotCorrectData.UseVisualStyleBackColor = false;
            this.btnDoNotCorrectData.Click += new System.EventHandler(this.btnDoNotCorrectData_Click);
            // 
            // btnCorrectData
            // 
            this.btnCorrectData.BackColor = System.Drawing.Color.ForestGreen;
            this.btnCorrectData.Enabled = false;
            this.btnCorrectData.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCorrectData.Location = new System.Drawing.Point(617, 227);
            this.btnCorrectData.Name = "btnCorrectData";
            this.btnCorrectData.Size = new System.Drawing.Size(197, 50);
            this.btnCorrectData.TabIndex = 1;
            this.btnCorrectData.Text = "Correct Data";
            this.btnCorrectData.UseVisualStyleBackColor = false;
            this.btnCorrectData.Click += new System.EventHandler(this.btnCorrectData_Click);
            // 
            // btnOpenTestData
            // 
            this.btnOpenTestData.Location = new System.Drawing.Point(54, 25);
            this.btnOpenTestData.Name = "btnOpenTestData";
            this.btnOpenTestData.Size = new System.Drawing.Size(156, 50);
            this.btnOpenTestData.TabIndex = 0;
            this.btnOpenTestData.Text = "Open Test Data";
            this.btnOpenTestData.UseVisualStyleBackColor = true;
            this.btnOpenTestData.Click += new System.EventHandler(this.btnOpenTestData_Click);
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
            this.Controls.Add(this.btnOpenTestData);
            this.Controls.Add(this.btnCorrectData);
            this.Controls.Add(this.btnDoNotCorrectData);
            this.Controls.Add(this.txtTestData);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.MaximizeBox = false;
            this.Name = "Spea_4060M_Test_Data_Corrector";
            this.Text = "Spea 4060M Test Data Corrector";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtTestData;
        private System.Windows.Forms.Button btnDoNotCorrectData;
        private System.Windows.Forms.Button btnCorrectData;
        private System.Windows.Forms.Button btnOpenTestData;
        private System.Windows.Forms.Label lblTestData;
    }
}

