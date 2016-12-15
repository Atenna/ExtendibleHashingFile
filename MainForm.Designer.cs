namespace PoliceSystem
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageCar = new System.Windows.Forms.TabPage();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnDeleteCar = new System.Windows.Forms.Button();
            this.pgdCar = new System.Windows.Forms.PropertyGrid();
            this.label3 = new System.Windows.Forms.Label();
            this.tbxCarEcv = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxCarVin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageDriver = new System.Windows.Forms.TabPage();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.btnDeleteDriver = new System.Windows.Forms.Button();
            this.pgdDriver = new System.Windows.Forms.PropertyGrid();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxDriverId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPageCar.SuspendLayout();
            this.tabPageDriver.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageCar);
            this.tabControl1.Controls.Add(this.tabPageDriver);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(539, 370);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageCar
            // 
            this.tabPageCar.Controls.Add(this.button1);
            this.tabPageCar.Controls.Add(this.button4);
            this.tabPageCar.Controls.Add(this.button3);
            this.tabPageCar.Controls.Add(this.button2);
            this.tabPageCar.Controls.Add(this.btnDeleteCar);
            this.tabPageCar.Controls.Add(this.pgdCar);
            this.tabPageCar.Controls.Add(this.label3);
            this.tabPageCar.Controls.Add(this.tbxCarEcv);
            this.tabPageCar.Controls.Add(this.label2);
            this.tabPageCar.Controls.Add(this.tbxCarVin);
            this.tabPageCar.Controls.Add(this.label1);
            this.tabPageCar.Location = new System.Drawing.Point(4, 22);
            this.tabPageCar.Name = "tabPageCar";
            this.tabPageCar.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCar.Size = new System.Drawing.Size(531, 344);
            this.tabPageCar.TabIndex = 0;
            this.tabPageCar.Text = "Car";
            this.tabPageCar.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.AutoSize = true;
            this.button4.Location = new System.Drawing.Point(265, 70);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(104, 23);
            this.button4.TabIndex = 10;
            this.button4.Text = "Search by Ecv";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.OnSearchCarByEcvButtonClick);
            // 
            // button3
            // 
            this.button3.AutoSize = true;
            this.button3.Location = new System.Drawing.Point(265, 24);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(104, 23);
            this.button3.TabIndex = 9;
            this.button3.Text = "Search by Vin";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.OnSearchCarByVinButtonClick);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.AutoSize = true;
            this.button2.Location = new System.Drawing.Point(433, 315);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(92, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "Update or insert";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.OnUpdateOrInsertCarButtonClick);
            // 
            // btnDeleteCar
            // 
            this.btnDeleteCar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteCar.AutoSize = true;
            this.btnDeleteCar.Location = new System.Drawing.Point(395, 24);
            this.btnDeleteCar.Name = "btnDeleteCar";
            this.btnDeleteCar.Size = new System.Drawing.Size(110, 23);
            this.btnDeleteCar.TabIndex = 7;
            this.btnDeleteCar.Text = "Delete by Vin";
            this.btnDeleteCar.UseVisualStyleBackColor = true;
            this.btnDeleteCar.Click += new System.EventHandler(this.OnDeleteCarByVinButtonClick);
            // 
            // pgdCar
            // 
            this.pgdCar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgdCar.HelpVisible = false;
            this.pgdCar.Location = new System.Drawing.Point(19, 123);
            this.pgdCar.Name = "pgdCar";
            this.pgdCar.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgdCar.Size = new System.Drawing.Size(506, 177);
            this.pgdCar.TabIndex = 6;
            this.pgdCar.ToolbarVisible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Car information:";
            // 
            // tbxCarEcv
            // 
            this.tbxCarEcv.Location = new System.Drawing.Point(19, 72);
            this.tbxCarEcv.Name = "tbxCarEcv";
            this.tbxCarEcv.Size = new System.Drawing.Size(100, 20);
            this.tbxCarEcv.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Ecv code";
            // 
            // tbxCarVin
            // 
            this.tbxCarVin.Location = new System.Drawing.Point(19, 27);
            this.tbxCarVin.Name = "tbxCarVin";
            this.tbxCarVin.Size = new System.Drawing.Size(212, 20);
            this.tbxCarVin.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Vin code";
            // 
            // tabPageDriver
            // 
            this.tabPageDriver.Controls.Add(this.button5);
            this.tabPageDriver.Controls.Add(this.button6);
            this.tabPageDriver.Controls.Add(this.btnDeleteDriver);
            this.tabPageDriver.Controls.Add(this.pgdDriver);
            this.tabPageDriver.Controls.Add(this.label4);
            this.tabPageDriver.Controls.Add(this.tbxDriverId);
            this.tabPageDriver.Controls.Add(this.label5);
            this.tabPageDriver.Location = new System.Drawing.Point(4, 22);
            this.tabPageDriver.Name = "tabPageDriver";
            this.tabPageDriver.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageDriver.Size = new System.Drawing.Size(531, 344);
            this.tabPageDriver.TabIndex = 1;
            this.tabPageDriver.Text = "Driver";
            this.tabPageDriver.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.AutoSize = true;
            this.button5.Location = new System.Drawing.Point(258, 23);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(104, 23);
            this.button5.TabIndex = 16;
            this.button5.Text = "Search";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.OnSearchDriverButtonClick);
            // 
            // button6
            // 
            this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button6.AutoSize = true;
            this.button6.Location = new System.Drawing.Point(426, 314);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(92, 23);
            this.button6.TabIndex = 15;
            this.button6.Text = "Update or insert";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.OnUpdateOrInsertDriverButtonClick);
            // 
            // btnDeleteDriver
            // 
            this.btnDeleteDriver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteDriver.AutoSize = true;
            this.btnDeleteDriver.Location = new System.Drawing.Point(406, 23);
            this.btnDeleteDriver.Name = "btnDeleteDriver";
            this.btnDeleteDriver.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteDriver.TabIndex = 14;
            this.btnDeleteDriver.Text = "Delete";
            this.btnDeleteDriver.UseVisualStyleBackColor = true;
            this.btnDeleteDriver.Click += new System.EventHandler(this.OnDeleteDriverButtonClick);
            // 
            // pgdDriver
            // 
            this.pgdDriver.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgdDriver.HelpVisible = false;
            this.pgdDriver.Location = new System.Drawing.Point(12, 77);
            this.pgdDriver.Name = "pgdDriver";
            this.pgdDriver.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgdDriver.Size = new System.Drawing.Size(506, 222);
            this.pgdDriver.TabIndex = 13;
            this.pgdDriver.ToolbarVisible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Driver information:";
            // 
            // tbxDriverId
            // 
            this.tbxDriverId.Location = new System.Drawing.Point(12, 26);
            this.tbxDriverId.Name = "tbxDriverId";
            this.tbxDriverId.Size = new System.Drawing.Size(212, 20);
            this.tbxDriverId.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Driver Id";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.AutoSize = true;
            this.button1.Location = new System.Drawing.Point(395, 70);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(110, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Delete by Ecv";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnDeleteCarByEcvButtonClick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 394);
            this.Controls.Add(this.tabControl1);
            this.Name = "MainForm";
            this.Text = "Police system";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabPageCar.ResumeLayout(false);
            this.tabPageCar.PerformLayout();
            this.tabPageDriver.ResumeLayout(false);
            this.tabPageDriver.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageCar;
        private System.Windows.Forms.TextBox tbxCarEcv;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxCarVin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageDriver;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnDeleteCar;
        private System.Windows.Forms.PropertyGrid pgdCar;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button btnDeleteDriver;
        private System.Windows.Forms.PropertyGrid pgdDriver;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbxDriverId;
        private System.Windows.Forms.Button button1;
    }
}