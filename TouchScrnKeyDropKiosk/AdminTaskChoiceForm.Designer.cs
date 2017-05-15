namespace KeyCabinetKiosk
{
    partial class AdminTaskChoiceForm
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
            this.panelMessage = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonLocation = new System.Windows.Forms.Button();
            this.buttonTransactionEmail = new System.Windows.Forms.Button();
            this.buttonTransactionDisplay = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonRFIDInfo = new System.Windows.Forms.Button();
            this.buttonDataImport = new System.Windows.Forms.Button();
            this.panelMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMessage
            // 
            this.panelMessage.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMessage.Controls.Add(this.label1);
            this.panelMessage.Location = new System.Drawing.Point(20, 26);
            this.panelMessage.Name = "panelMessage";
            this.panelMessage.Size = new System.Drawing.Size(749, 221);
            this.panelMessage.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Black", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(102, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(570, 68);
            this.label1.TabIndex = 0;
            this.label1.Text = "Administrative tasks";
            // 
            // buttonLocation
            // 
            this.buttonLocation.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLocation.Location = new System.Drawing.Point(145, 271);
            this.buttonLocation.Name = "buttonLocation";
            this.buttonLocation.Size = new System.Drawing.Size(500, 65);
            this.buttonLocation.TabIndex = 1;
            this.buttonLocation.Text = "Cabinet Box Access";
            this.buttonLocation.UseVisualStyleBackColor = true;
            this.buttonLocation.Click += new System.EventHandler(this.buttonLocation_Click);
            // 
            // buttonTransactionEmail
            // 
            this.buttonTransactionEmail.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTransactionEmail.Location = new System.Drawing.Point(37, 352);
            this.buttonTransactionEmail.Name = "buttonTransactionEmail";
            this.buttonTransactionEmail.Size = new System.Drawing.Size(354, 65);
            this.buttonTransactionEmail.TabIndex = 2;
            this.buttonTransactionEmail.Text = "Email Transaction Report";
            this.buttonTransactionEmail.UseVisualStyleBackColor = true;
            this.buttonTransactionEmail.Click += new System.EventHandler(this.buttonTransactionEmail_Click);
            // 
            // buttonTransactionDisplay
            // 
            this.buttonTransactionDisplay.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonTransactionDisplay.Location = new System.Drawing.Point(37, 434);
            this.buttonTransactionDisplay.Name = "buttonTransactionDisplay";
            this.buttonTransactionDisplay.Size = new System.Drawing.Size(354, 65);
            this.buttonTransactionDisplay.TabIndex = 3;
            this.buttonTransactionDisplay.Text = "Display Transactions";
            this.buttonTransactionDisplay.UseVisualStyleBackColor = true;
            this.buttonTransactionDisplay.Click += new System.EventHandler(this.buttonTransactionDisplay_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExit.Location = new System.Drawing.Point(145, 514);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(500, 65);
            this.buttonExit.TabIndex = 4;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonRFIDInfo
            // 
            this.buttonRFIDInfo.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRFIDInfo.Location = new System.Drawing.Point(411, 434);
            this.buttonRFIDInfo.Name = "buttonRFIDInfo";
            this.buttonRFIDInfo.Size = new System.Drawing.Size(354, 65);
            this.buttonRFIDInfo.TabIndex = 5;
            this.buttonRFIDInfo.Text = "Display RFID Info";
            this.buttonRFIDInfo.UseVisualStyleBackColor = true;
            this.buttonRFIDInfo.Click += new System.EventHandler(this.buttonRFIDInfo_Click);
            // 
            // buttonDataImport
            // 
            this.buttonDataImport.Font = new System.Drawing.Font("Arial Black", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonDataImport.Location = new System.Drawing.Point(411, 352);
            this.buttonDataImport.Name = "buttonDataImport";
            this.buttonDataImport.Size = new System.Drawing.Size(354, 65);
            this.buttonDataImport.TabIndex = 6;
            this.buttonDataImport.Text = "USB Data Import";
            this.buttonDataImport.UseVisualStyleBackColor = true;
            this.buttonDataImport.Click += new System.EventHandler(this.buttonDataImport_Click);
            // 
            // AdminTaskChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(Program.WindowBackRed, Program.WindowBackGreen, Program.WindowBackBlue);
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.buttonDataImport);
            this.Controls.Add(this.buttonRFIDInfo);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonTransactionDisplay);
            this.Controls.Add(this.buttonTransactionEmail);
            this.Controls.Add(this.buttonLocation);
            this.Controls.Add(this.panelMessage);
            this.Name = "AdminTaskChoiceForm";
            this.Text = "AdminTaskChoiceForm";
            this.panelMessage.ResumeLayout(false);
            this.panelMessage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonLocation;
        private System.Windows.Forms.Button buttonTransactionEmail;
        private System.Windows.Forms.Button buttonTransactionDisplay;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonRFIDInfo;
        private System.Windows.Forms.Button buttonDataImport;
    }
}