namespace KeyCabinetKiosk
{
    partial class DisplayAllInfoForm
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
            this.buttonExit = new System.Windows.Forms.Button();
            this.buttonPageDown = new System.Windows.Forms.Button();
            this.buttonPageUp = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonScan = new System.Windows.Forms.Button();
            this.numericUpDownScanLength = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonClearTable = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanLength)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonExit
            // 
            this.buttonExit.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonExit.Location = new System.Drawing.Point(568, 487);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(176, 78);
            this.buttonExit.TabIndex = 0;
            this.buttonExit.Text = "Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // buttonPageDown
            // 
            this.buttonPageDown.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPageDown.Location = new System.Drawing.Point(222, 487);
            this.buttonPageDown.Name = "buttonPageDown";
            this.buttonPageDown.Size = new System.Drawing.Size(140, 78);
            this.buttonPageDown.TabIndex = 2;
            this.buttonPageDown.Text = "Pg Dn";
            this.buttonPageDown.UseVisualStyleBackColor = true;
            this.buttonPageDown.Click += new System.EventHandler(this.buttonPageDown_Click);
            // 
            // buttonPageUp
            // 
            this.buttonPageUp.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPageUp.Location = new System.Drawing.Point(55, 487);
            this.buttonPageUp.Name = "buttonPageUp";
            this.buttonPageUp.Size = new System.Drawing.Size(140, 78);
            this.buttonPageUp.TabIndex = 3;
            this.buttonPageUp.Text = "Pg Up";
            this.buttonPageUp.UseVisualStyleBackColor = true;
            this.buttonPageUp.Click += new System.EventHandler(this.buttonPageUp_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(176, 30);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(467, 435);
            this.dataGridView1.TabIndex = 4;
            // 
            // buttonScan
            // 
            this.buttonScan.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonScan.Location = new System.Drawing.Point(390, 487);
            this.buttonScan.Name = "buttonScan";
            this.buttonScan.Size = new System.Drawing.Size(155, 78);
            this.buttonScan.TabIndex = 5;
            this.buttonScan.Text = "RFID Scan";
            this.buttonScan.UseVisualStyleBackColor = true;
            this.buttonScan.Visible = false;
            this.buttonScan.Click += new System.EventHandler(this.buttonScan_Click);
            // 
            // numericUpDownScanLength
            // 
            this.numericUpDownScanLength.Font = new System.Drawing.Font("Arial", 25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDownScanLength.Location = new System.Drawing.Point(50, 227);
            this.numericUpDownScanLength.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.numericUpDownScanLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownScanLength.Name = "numericUpDownScanLength";
            this.numericUpDownScanLength.Size = new System.Drawing.Size(65, 46);
            this.numericUpDownScanLength.TabIndex = 7;
            this.numericUpDownScanLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownScanLength.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownScanLength.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(167, 96);
            this.label1.TabIndex = 6;
            this.label1.Text = "Number Of \r\nSeconds\r\nTo Scan";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Visible = false;
            // 
            // buttonClearTable
            // 
            this.buttonClearTable.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonClearTable.Location = new System.Drawing.Point(12, 319);
            this.buttonClearTable.Name = "buttonClearTable";
            this.buttonClearTable.Size = new System.Drawing.Size(140, 78);
            this.buttonClearTable.TabIndex = 8;
            this.buttonClearTable.Text = "Clear Scan Table";
            this.buttonClearTable.UseVisualStyleBackColor = true;
            this.buttonClearTable.Visible = false;
            this.buttonClearTable.Click += new System.EventHandler(this.buttonClearTable_Click);
            // 
            // DisplayAllInfoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(Program.WindowBackRed, Program.WindowBackGreen, Program.WindowBackBlue);
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.buttonClearTable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownScanLength);
            this.Controls.Add(this.buttonScan);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonPageUp);
            this.Controls.Add(this.buttonPageDown);
            this.Controls.Add(this.buttonExit);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "DisplayAllInfoForm";
            this.Text = "DisplayAllInfoForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownScanLength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.Button buttonPageDown;
        private System.Windows.Forms.Button buttonPageUp;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonScan;
        private System.Windows.Forms.NumericUpDown numericUpDownScanLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonClearTable;
    }
}