namespace KeyCabinetKiosk
{
    partial class MainForm
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
            this.buttonDevExit = new System.Windows.Forms.Button();
            this.buttonStart = new System.Windows.Forms.Button();
            this.panelHiddenExit = new System.Windows.Forms.Panel();
            this.panelMessage = new System.Windows.Forms.Panel();
            this.labelMessageTitle = new System.Windows.Forms.Label();
            this.panelFullScreenTouch = new System.Windows.Forms.Panel();
            this.panelMessage.SuspendLayout();
            this.panelFullScreenTouch.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonDevExit
            // 
            this.buttonDevExit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonDevExit.Location = new System.Drawing.Point(562, 127);
            this.buttonDevExit.Name = "buttonDevExit";
            this.buttonDevExit.Size = new System.Drawing.Size(75, 23);
            this.buttonDevExit.TabIndex = 0;
            this.buttonDevExit.Text = "Dev Exit";
            this.buttonDevExit.UseVisualStyleBackColor = true;
            this.buttonDevExit.Click += new System.EventHandler(this.buttonDevExit_Click);
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("Arial Black", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStart.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStart.Location = new System.Drawing.Point(106, 20);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(424, 112);
            this.buttonStart.TabIndex = 1;
            this.buttonStart.Text = "Press screen to start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // panelHiddenExit
            // 
            this.panelHiddenExit.Location = new System.Drawing.Point(0, 2);
            this.panelHiddenExit.Name = "panelHiddenExit";
            this.panelHiddenExit.Size = new System.Drawing.Size(79, 60);
            this.panelHiddenExit.TabIndex = 2;
            this.panelHiddenExit.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelHiddenExit_MouseClick);
            // 
            // panelMessage
            // 
            this.panelMessage.BackColor = System.Drawing.SystemColors.Control;
            this.panelMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMessage.Controls.Add(this.labelMessageTitle);
            this.panelMessage.Location = new System.Drawing.Point(37, 25);
            this.panelMessage.Name = "panelMessage";
            this.panelMessage.Size = new System.Drawing.Size(720, 351);
            this.panelMessage.TabIndex = 3;
            this.panelMessage.Click += new System.EventHandler(this.panelMessage_Click);
            // 
            // labelMessageTitle
            // 
            this.labelMessageTitle.AutoSize = true;
            this.labelMessageTitle.Font = new System.Drawing.Font("Arial Black", 38F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMessageTitle.ForeColor = System.Drawing.SystemColors.ControlText;
            this.labelMessageTitle.Location = new System.Drawing.Point(97, 68);
            this.labelMessageTitle.Name = "labelMessageTitle";
            this.labelMessageTitle.Size = new System.Drawing.Size(514, 144);
            this.labelMessageTitle.TabIndex = 1;
            this.labelMessageTitle.Text = "SafePak \r\nKey Management";
            this.labelMessageTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panelFullScreenTouch
            // 
            this.panelFullScreenTouch.Controls.Add(this.buttonStart);
            this.panelFullScreenTouch.Controls.Add(this.buttonDevExit);
            this.panelFullScreenTouch.Location = new System.Drawing.Point(79, 412);
            this.panelFullScreenTouch.Name = "panelFullScreenTouch";
            this.panelFullScreenTouch.Size = new System.Drawing.Size(640, 150);
            this.panelFullScreenTouch.TabIndex = 4;
            this.panelFullScreenTouch.Click += new System.EventHandler(this.panelFullScreenTouch_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(Program.WindowBackRed, Program.WindowBackGreen, Program.WindowBackBlue);
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.panelFullScreenTouch);
            this.Controls.Add(this.panelMessage);
            this.Controls.Add(this.panelHiddenExit);
            this.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.panelMessage.ResumeLayout(false);
            this.panelMessage.PerformLayout();
            this.panelFullScreenTouch.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonDevExit;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Panel panelHiddenExit;
        private System.Windows.Forms.Panel panelMessage;
        private System.Windows.Forms.Panel panelFullScreenTouch;
        private System.Windows.Forms.Label labelMessageTitle;
    }
}