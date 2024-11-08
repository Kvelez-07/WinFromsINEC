namespace WinFromsINEC
{
    partial class frmINEC
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
            this.lblProgress = new System.Windows.Forms.Label();
            this.prgAge = new System.Windows.Forms.ProgressBar();
            this.gridScholarship = new System.Windows.Forms.DataGridView();
            this.gridGenderAge = new System.Windows.Forms.DataGridView();
            this.gridAgeToGender = new System.Windows.Forms.DataGridView();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.lblScholarship = new System.Windows.Forms.Label();
            this.lblGenderAge = new System.Windows.Forms.Label();
            this.lblAgeToGender = new System.Windows.Forms.Label();
            this.txtThreadId = new System.Windows.Forms.TextBox();
            this.lblThread = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridScholarship)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridGenderAge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAgeToGender)).BeginInit();
            this.SuspendLayout();
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.BackColor = System.Drawing.Color.Transparent;
            this.lblProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.Location = new System.Drawing.Point(372, 298);
            this.lblProgress.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(67, 17);
            this.lblProgress.TabIndex = 23;
            this.lblProgress.Text = "Calcular";
            this.lblProgress.Visible = false;
            // 
            // prgAge
            // 
            this.prgAge.Location = new System.Drawing.Point(375, 329);
            this.prgAge.Margin = new System.Windows.Forms.Padding(2);
            this.prgAge.Name = "prgAge";
            this.prgAge.Size = new System.Drawing.Size(221, 36);
            this.prgAge.TabIndex = 22;
            this.prgAge.Visible = false;
            // 
            // gridScholarship
            // 
            this.gridScholarship.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridScholarship.Location = new System.Drawing.Point(726, 118);
            this.gridScholarship.Margin = new System.Windows.Forms.Padding(2);
            this.gridScholarship.Name = "gridScholarship";
            this.gridScholarship.RowHeadersWidth = 51;
            this.gridScholarship.RowTemplate.Height = 24;
            this.gridScholarship.Size = new System.Drawing.Size(355, 403);
            this.gridScholarship.TabIndex = 21;
            // 
            // gridGenderAge
            // 
            this.gridGenderAge.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridGenderAge.Location = new System.Drawing.Point(295, 118);
            this.gridGenderAge.Margin = new System.Windows.Forms.Padding(2);
            this.gridGenderAge.Name = "gridGenderAge";
            this.gridGenderAge.RowHeadersWidth = 51;
            this.gridGenderAge.RowTemplate.Height = 24;
            this.gridGenderAge.Size = new System.Drawing.Size(411, 403);
            this.gridGenderAge.TabIndex = 20;
            // 
            // gridAgeToGender
            // 
            this.gridAgeToGender.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAgeToGender.Location = new System.Drawing.Point(28, 118);
            this.gridAgeToGender.Margin = new System.Windows.Forms.Padding(2);
            this.gridAgeToGender.Name = "gridAgeToGender";
            this.gridAgeToGender.RowHeadersWidth = 51;
            this.gridAgeToGender.RowTemplate.Height = 24;
            this.gridAgeToGender.Size = new System.Drawing.Size(252, 403);
            this.gridAgeToGender.TabIndex = 19;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1021, 558);
            this.btnExit.Margin = new System.Windows.Forms.Padding(2);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(73, 32);
            this.btnExit.TabIndex = 18;
            this.btnExit.Text = "Salir";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Location = new System.Drawing.Point(893, 558);
            this.btnLoadFile.Margin = new System.Windows.Forms.Padding(2);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(123, 32);
            this.btnLoadFile.TabIndex = 17;
            this.btnLoadFile.Text = "Cargar Archivo";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // lblScholarship
            // 
            this.lblScholarship.AutoSize = true;
            this.lblScholarship.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScholarship.Location = new System.Drawing.Point(774, 87);
            this.lblScholarship.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblScholarship.Name = "lblScholarship";
            this.lblScholarship.Size = new System.Drawing.Size(265, 25);
            this.lblScholarship.TabIndex = 16;
            this.lblScholarship.Text = "Lista segun Escolaridad";
            // 
            // lblGenderAge
            // 
            this.lblGenderAge.AutoSize = true;
            this.lblGenderAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblGenderAge.Location = new System.Drawing.Point(323, 87);
            this.lblGenderAge.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblGenderAge.Name = "lblGenderAge";
            this.lblGenderAge.Size = new System.Drawing.Size(353, 25);
            this.lblGenderAge.TabIndex = 15;
            this.lblGenderAge.Text = "Lista segun Sexo y Grupo Etario";
            // 
            // lblAgeToGender
            // 
            this.lblAgeToGender.AutoSize = true;
            this.lblAgeToGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.lblAgeToGender.Location = new System.Drawing.Point(56, 87);
            this.lblAgeToGender.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblAgeToGender.Name = "lblAgeToGender";
            this.lblAgeToGender.Size = new System.Drawing.Size(197, 25);
            this.lblAgeToGender.TabIndex = 14;
            this.lblAgeToGender.Text = "Edad segun Sexo";
            // 
            // txtThreadId
            // 
            this.txtThreadId.Location = new System.Drawing.Point(137, 15);
            this.txtThreadId.Margin = new System.Windows.Forms.Padding(2);
            this.txtThreadId.Name = "txtThreadId";
            this.txtThreadId.Size = new System.Drawing.Size(76, 20);
            this.txtThreadId.TabIndex = 13;
            // 
            // lblThread
            // 
            this.lblThread.AutoSize = true;
            this.lblThread.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThread.Location = new System.Drawing.Point(13, 15);
            this.lblThread.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblThread.Name = "lblThread";
            this.lblThread.Size = new System.Drawing.Size(127, 17);
            this.lblThread.TabIndex = 12;
            this.lblThread.Text = "Ejecutando Hilo:";
            // 
            // frmINEC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1106, 606);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.prgAge);
            this.Controls.Add(this.gridScholarship);
            this.Controls.Add(this.gridGenderAge);
            this.Controls.Add(this.gridAgeToGender);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnLoadFile);
            this.Controls.Add(this.lblScholarship);
            this.Controls.Add(this.lblGenderAge);
            this.Controls.Add(this.lblAgeToGender);
            this.Controls.Add(this.txtThreadId);
            this.Controls.Add(this.lblThread);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmINEC";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.gridScholarship)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridGenderAge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridAgeToGender)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar prgAge;
        private System.Windows.Forms.DataGridView gridScholarship;
        private System.Windows.Forms.DataGridView gridGenderAge;
        private System.Windows.Forms.DataGridView gridAgeToGender;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.Label lblScholarship;
        private System.Windows.Forms.Label lblGenderAge;
        private System.Windows.Forms.Label lblAgeToGender;
        private System.Windows.Forms.TextBox txtThreadId;
        private System.Windows.Forms.Label lblThread;
    }
}

