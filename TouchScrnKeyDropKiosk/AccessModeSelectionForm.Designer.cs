namespace KeyCabinetKiosk
{
    partial class AccessModeSelectionForm
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
            this.labelPanelMessage = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.labelAccessType = new System.Windows.Forms.Label();
            this.buttonSimple = new System.Windows.Forms.Button();
            this.buttonCard = new System.Windows.Forms.Button();
            this.buttonBoth = new System.Windows.Forms.Button();
            this.buttonExit = new System.Windows.Forms.Button();
            this.panelMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelMessage
            // 
            this.panelMessage.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMessage.Controls.Add(this.labelPanelMessage);
            this.panelMessage.Location = new System.Drawing.Point(20, 20);
            this.panelMessage.Name = "panelMessage";
            this.panelMessage.Size = new System.Drawing.Size(735, 177);
            this.panelMessage.TabIndex = 0;
            // 
            // labelPanelMessage
            // 
            this.labelPanelMessage.AutoSize = true;
            this.labelPanelMessage.Font = new System.Drawing.Font("Arial Black", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPanelMessage.Location = new System.Drawing.Point(115, 55);
            this.labelPanelMessage.Name = "labelPanelMessage";
            this.labelPanelMessage.Size = new System.Drawing.Size(515, 56);
            this.labelPanelMessage.TabIndex = 0;
            this.labelPanelMessage.Text = "Select Access Method";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(153, 215);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(264, 38);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Current Access :";
            // 
            // labelAccessType
            // 
            this.labelAccessType.AutoSize = true;
            this.labelAccessType.Font = new System.Drawing.Font("Arial Black", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAccessType.Location = new System.Drawing.Point(423, 215);
            this.labelAccessType.Name = "labelAccessType";
            this.labelAccessType.Size = new System.Drawing.Size(210, 38);
            this.labelAccessType.TabIndex = 2;
            this.labelAccessType.Text = "Access Code";
            // 
            // buttonSimple
            // 
            this.buttonSimple.Font = new System.Drawing.Font("Arial Black", 23F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSimple.Location = new System.Drawing.Point(92, 269);
            this.buttonSimple.Name = "buttonSimple";
            this.buttonSimple.Size = new System.Drawing.Size(603, 70);
            this.buttonSimple.TabIndex = 3;
            this.buttonSimple.Text = "Access Code Only";
            this.buttonSimple.UseVisualStyleBackColor = true;
            this.buttonSimple.Click += new System.EventHandler(this.buttonSimple_Click);
            // 
            // buttonCard
            // 
            this.buttonCard.Font = new System.Drawing.Font("Arial Black", 23F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCard.Location = new System.Drawing.Point(92, 358);
            this.buttonCard.Name = "buttonCard";
            this.buttonCard.Size = new System.Drawing.Size(603, 66);
            this.buttonCard.TabIndex = 4;
            this.buttonCard.Text = "Card Only";
            this.buttonCard.UseVisualStyleBackColor = true;
            this.buttonCard.Click += new System.EventHandler(this.buttonCard_Click);
            // 
            // buttonBoth
            // 
            this.buttonBoth.Font = new System.Drawing.Font("Arial Black", 23F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonBoth.Location = new System.Drawing.Point(92, 440);
            this.buttonBoth.Name = "buttonBoth";
            this.buttonBoth.Size = new System.Drawing.Size(603, 73);
            this.buttonBoth.TabIndex = 5;
            this.buttonBoth.Text = "Access Code And Card";
            this.buttonBoth.UseVisualStyleBackColor = true;
            this.buttonBoth.Click += new System.EventHandler(this.buttonBoth_Click);
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(655, 519);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(122, 69);
            this.buttonExit.TabIndex = 6;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // AccessModeSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(Program.WindowBackRed, Program.WindowBackGreen, Program.WindowBackBlue);
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.buttonBoth);
            this.Controls.Add(this.buttonCard);
            this.Controls.Add(this.buttonSimple);
            this.Controls.Add(this.labelAccessType);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.panelMessage);
            this.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.Name = "AccessModeSelectionForm";
            this.Text = "AccessModeSelectionForm";
            this.panelMessage.ResumeLayout(false);
            this.panelMessage.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelMessage;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelAccessType;
        private System.Windows.Forms.Button buttonSimple;
        private System.Windows.Forms.Button buttonCard;
        private System.Windows.Forms.Button buttonBoth;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Label labelPanelMessage;
    }
}