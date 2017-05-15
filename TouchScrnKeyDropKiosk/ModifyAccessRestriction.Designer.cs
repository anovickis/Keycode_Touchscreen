namespace KeyCabinetKiosk
{
    partial class ModifyAccessRestriction
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
            this.labelheadline = new System.Windows.Forms.Label();
            this.datePickerStart = new System.Windows.Forms.DateTimePicker();
            this.radioButtonAlwaysOn = new System.Windows.Forms.RadioButton();
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonApplyChanges = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButtonLimitedUses = new System.Windows.Forms.RadioButton();
            this.radioButtonTimePeriod = new System.Windows.Forms.RadioButton();
            this.radioButtonLimitTimePeriod = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.datePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.timePickerStart = new System.Windows.Forms.DateTimePicker();
            this.timePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.buttonSetStartTime = new System.Windows.Forms.Button();
            this.buttonSetEndTime = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonLimitedUses = new System.Windows.Forms.Button();
            this.limitUsesUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.limitUsesUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // labelheadline
            // 
            this.labelheadline.AutoSize = true;
            this.labelheadline.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelheadline.Location = new System.Drawing.Point(87, 19);
            this.labelheadline.Name = "labelheadline";
            this.labelheadline.Size = new System.Drawing.Size(536, 31);
            this.labelheadline.TabIndex = 0;
            this.labelheadline.Text = "Set Restrictions for Box or User Number";
            // 
            // datePickerStart
            // 
            this.datePickerStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.datePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePickerStart.Location = new System.Drawing.Point(67, 231);
            this.datePickerStart.Name = "datePickerStart";
            this.datePickerStart.Size = new System.Drawing.Size(196, 38);
            this.datePickerStart.TabIndex = 2;
            // 
            // radioButtonAlwaysOn
            // 
            this.radioButtonAlwaysOn.AutoSize = true;
            this.radioButtonAlwaysOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonAlwaysOn.Location = new System.Drawing.Point(67, 139);
            this.radioButtonAlwaysOn.Name = "radioButtonAlwaysOn";
            this.radioButtonAlwaysOn.Size = new System.Drawing.Size(134, 29);
            this.radioButtonAlwaysOn.TabIndex = 3;
            this.radioButtonAlwaysOn.TabStop = true;
            this.radioButtonAlwaysOn.Text = "Always On";
            this.radioButtonAlwaysOn.UseVisualStyleBackColor = true;
            this.radioButtonAlwaysOn.CheckedChanged += new System.EventHandler(this.radioButtonAlwaysOn_CheckedChanged);
            // 
            // buttonExit
            // 
            this.buttonExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExit.Location = new System.Drawing.Point(111, 469);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(152, 89);
            this.buttonExit.TabIndex = 4;
            this.buttonExit.Text = "Cancel";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonApplyChanges
            // 
            this.buttonApplyChanges.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonApplyChanges.Location = new System.Drawing.Point(555, 469);
            this.buttonApplyChanges.Name = "buttonApplyChanges";
            this.buttonApplyChanges.Size = new System.Drawing.Size(152, 89);
            this.buttonApplyChanges.TabIndex = 5;
            this.buttonApplyChanges.Text = "Apply Changes";
            this.buttonApplyChanges.UseVisualStyleBackColor = true;
            this.buttonApplyChanges.Click += new System.EventHandler(this.buttonApplyChanges_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.panel1.Controls.Add(this.labelheadline);
            this.panel1.Location = new System.Drawing.Point(42, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(713, 71);
            this.panel1.TabIndex = 6;
            // 
            // radioButtonLimitedUses
            // 
            this.radioButtonLimitedUses.AutoSize = true;
            this.radioButtonLimitedUses.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonLimitedUses.Location = new System.Drawing.Point(207, 139);
            this.radioButtonLimitedUses.Name = "radioButtonLimitedUses";
            this.radioButtonLimitedUses.Size = new System.Drawing.Size(154, 29);
            this.radioButtonLimitedUses.TabIndex = 7;
            this.radioButtonLimitedUses.TabStop = true;
            this.radioButtonLimitedUses.Text = "Limited Uses";
            this.radioButtonLimitedUses.UseVisualStyleBackColor = true;
            this.radioButtonLimitedUses.CheckedChanged += new System.EventHandler(this.radioButtonLimitedUses_CheckedChanged);
            // 
            // radioButtonTimePeriod
            // 
            this.radioButtonTimePeriod.AutoSize = true;
            this.radioButtonTimePeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonTimePeriod.Location = new System.Drawing.Point(367, 139);
            this.radioButtonTimePeriod.Name = "radioButtonTimePeriod";
            this.radioButtonTimePeriod.Size = new System.Drawing.Size(146, 29);
            this.radioButtonTimePeriod.TabIndex = 8;
            this.radioButtonTimePeriod.TabStop = true;
            this.radioButtonTimePeriod.Text = "Time Period";
            this.radioButtonTimePeriod.UseVisualStyleBackColor = true;
            this.radioButtonTimePeriod.CheckedChanged += new System.EventHandler(this.radioButtonTimePeriod_CheckedChanged);
            // 
            // radioButtonLimitTimePeriod
            // 
            this.radioButtonLimitTimePeriod.AutoSize = true;
            this.radioButtonLimitTimePeriod.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButtonLimitTimePeriod.Location = new System.Drawing.Point(519, 139);
            this.radioButtonLimitTimePeriod.Name = "radioButtonLimitTimePeriod";
            this.radioButtonLimitTimePeriod.Size = new System.Drawing.Size(216, 29);
            this.radioButtonLimitTimePeriod.TabIndex = 9;
            this.radioButtonLimitTimePeriod.TabStop = true;
            this.radioButtonLimitTimePeriod.Text = "Limit + Time Period";
            this.radioButtonLimitTimePeriod.UseVisualStyleBackColor = true;
            this.radioButtonLimitTimePeriod.CheckedChanged += new System.EventHandler(this.radioButtonLimitTimePeriod_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(268, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(245, 25);
            this.label2.TabIndex = 10;
            this.label2.Text = "Access Restriction Type";
            // 
            // datePickerEnd
            // 
            this.datePickerEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.datePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datePickerEnd.Location = new System.Drawing.Point(317, 231);
            this.datePickerEnd.Name = "datePickerEnd";
            this.datePickerEnd.Size = new System.Drawing.Size(196, 38);
            this.datePickerEnd.TabIndex = 11;
            // 
            // timePickerStart
            // 
            this.timePickerStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timePickerStart.Location = new System.Drawing.Point(67, 305);
            this.timePickerStart.Name = "timePickerStart";
            this.timePickerStart.ShowUpDown = true;
            this.timePickerStart.Size = new System.Drawing.Size(196, 38);
            this.timePickerStart.TabIndex = 12;
            // 
            // timePickerEnd
            // 
            this.timePickerEnd.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timePickerEnd.Location = new System.Drawing.Point(317, 305);
            this.timePickerEnd.Name = "timePickerEnd";
            this.timePickerEnd.ShowUpDown = true;
            this.timePickerEnd.Size = new System.Drawing.Size(196, 38);
            this.timePickerEnd.TabIndex = 13;
            // 
            // buttonSetStartTime
            // 
            this.buttonSetStartTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSetStartTime.Location = new System.Drawing.Point(67, 375);
            this.buttonSetStartTime.Name = "buttonSetStartTime";
            this.buttonSetStartTime.Size = new System.Drawing.Size(196, 53);
            this.buttonSetStartTime.TabIndex = 14;
            this.buttonSetStartTime.Text = "Set Start Time";
            this.buttonSetStartTime.UseVisualStyleBackColor = true;
            this.buttonSetStartTime.Click += new System.EventHandler(this.buttonSetStartTime_Click);
            // 
            // buttonSetEndTime
            // 
            this.buttonSetEndTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 19F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonSetEndTime.Location = new System.Drawing.Point(317, 375);
            this.buttonSetEndTime.Name = "buttonSetEndTime";
            this.buttonSetEndTime.Size = new System.Drawing.Size(196, 53);
            this.buttonSetEndTime.TabIndex = 15;
            this.buttonSetEndTime.Text = "Set End Time";
            this.buttonSetEndTime.UseVisualStyleBackColor = true;
            this.buttonSetEndTime.Click += new System.EventHandler(this.buttonSetEndTime_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(67, 193);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 25);
            this.label3.TabIndex = 16;
            this.label3.Text = "Start Time";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(326, 193);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 25);
            this.label4.TabIndex = 17;
            this.label4.Text = "End Time";
            // 
            // buttonLimitedUses
            // 
            this.buttonLimitedUses.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonLimitedUses.Location = new System.Drawing.Point(555, 304);
            this.buttonLimitedUses.Name = "buttonLimitedUses";
            this.buttonLimitedUses.Size = new System.Drawing.Size(196, 53);
            this.buttonLimitedUses.TabIndex = 18;
            this.buttonLimitedUses.Text = "Set Limited Uses";
            this.buttonLimitedUses.UseVisualStyleBackColor = true;
            this.buttonLimitedUses.Click += new System.EventHandler(this.buttonLimitedUses_Click);
            // 
            // limitUsesUpDown
            // 
            this.limitUsesUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.limitUsesUpDown.Location = new System.Drawing.Point(555, 235);
            this.limitUsesUpDown.Name = "limitUsesUpDown";
            this.limitUsesUpDown.Size = new System.Drawing.Size(196, 38);
            this.limitUsesUpDown.TabIndex = 19;
            this.limitUsesUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(532, 193);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(241, 25);
            this.label5.TabIndex = 20;
            this.label5.Text = "Limited Number of Uses";
            // 
            // ModifyAccessRestriction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.limitUsesUpDown);
            this.Controls.Add(this.buttonLimitedUses);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonSetEndTime);
            this.Controls.Add(this.buttonSetStartTime);
            this.Controls.Add(this.timePickerEnd);
            this.Controls.Add(this.timePickerStart);
            this.Controls.Add(this.datePickerEnd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.radioButtonLimitTimePeriod);
            this.Controls.Add(this.radioButtonTimePeriod);
            this.Controls.Add(this.radioButtonLimitedUses);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonApplyChanges);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.radioButtonAlwaysOn);
            this.Controls.Add(this.datePickerStart);
            this.Name = "ModifyAccessRestriction";
            this.Text = "ModifyAccessRestriction";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.limitUsesUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelheadline;
        private System.Windows.Forms.DateTimePicker datePickerStart;
        private System.Windows.Forms.RadioButton radioButtonAlwaysOn;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonApplyChanges;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButtonLimitedUses;
        private System.Windows.Forms.RadioButton radioButtonTimePeriod;
        private System.Windows.Forms.RadioButton radioButtonLimitTimePeriod;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker datePickerEnd;
        private System.Windows.Forms.DateTimePicker timePickerStart;
        private System.Windows.Forms.DateTimePicker timePickerEnd;
        private System.Windows.Forms.Button buttonSetStartTime;
        private System.Windows.Forms.Button buttonSetEndTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonLimitedUses;
        private System.Windows.Forms.NumericUpDown limitUsesUpDown;
        private System.Windows.Forms.Label label5;
    }
}