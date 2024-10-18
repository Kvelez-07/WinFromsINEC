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
            this.lblProgress.Location = new System.Drawing.Point(551, 370);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(79, 20);
            this.lblProgress.TabIndex = 23;
            this.lblProgress.Text = "Calcular";
            this.lblProgress.Visible = false;
            // 
            // prgAge
            // 
            this.prgAge.Location = new System.Drawing.Point(555, 409);
            this.prgAge.Name = "prgAge";
            this.prgAge.Size = new System.Drawing.Size(295, 44);
            this.prgAge.TabIndex = 22;
            this.prgAge.Visible = false;
            // 
            // gridScholarship
            // 
            this.gridScholarship.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridScholarship.Location = new System.Drawing.Point(1032, 149);
            this.gridScholarship.Name = "gridScholarship";
            this.gridScholarship.RowHeadersWidth = 51;
            this.gridScholarship.RowTemplate.Height = 24;
            this.gridScholarship.Size = new System.Drawing.Size(426, 496);
            this.gridScholarship.TabIndex = 21;
            // 
            // gridGenderAge
            // 
            this.gridGenderAge.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridGenderAge.Location = new System.Drawing.Point(448, 149);
            this.gridGenderAge.Name = "gridGenderAge";
            this.gridGenderAge.RowHeadersWidth = 51;
            this.gridGenderAge.RowTemplate.Height = 24;
            this.gridGenderAge.Size = new System.Drawing.Size(548, 496);
            this.gridGenderAge.TabIndex = 20;
            // 
            // gridAgeToGender
            // 
            this.gridAgeToGender.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAgeToGender.Location = new System.Drawing.Point(29, 149);
            this.gridAgeToGender.Name = "gridAgeToGender";
            this.gridAgeToGender.RowHeadersWidth = 51;
            this.gridAgeToGender.RowTemplate.Height = 24;
            this.gridAgeToGender.Size = new System.Drawing.Size(390, 496);
            this.gridAgeToGender.TabIndex = 19;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(1361, 687);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(97, 40);
            this.btnExit.TabIndex = 18;
            this.btnExit.Text = "Salir";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click_1);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Location = new System.Drawing.Point(1191, 687);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(164, 40);
            this.btnLoadFile.TabIndex = 17;
            this.btnLoadFile.Text = "Cargar Archivo";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click_1);
            // 
            // lblScholarship
            // 
            this.lblScholarship.AutoSize = true;
            this.lblScholarship.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblScholarship.Location = new System.Drawing.Point(1159, 111);
            this.lblScholarship.Name = "lblScholarship";
            this.lblScholarship.Size = new System.Drawing.Size(174, 16);
            this.lblScholarship.TabIndex = 16;
            this.lblScholarship.Text = "Lista segun Escolaridad";
            // 
            // lblGenderAge
            // 
            this.lblGenderAge.AutoSize = true;
            this.lblGenderAge.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGenderAge.Location = new System.Drawing.Point(601, 111);
            this.lblGenderAge.Name = "lblGenderAge";
            this.lblGenderAge.Size = new System.Drawing.Size(228, 16);
            this.lblGenderAge.TabIndex = 15;
            this.lblGenderAge.Text = "Lista segun Sexo y Grupo Etario";
            // 
            // lblAgeToGender
            // 
            this.lblAgeToGender.AutoSize = true;
            this.lblAgeToGender.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAgeToGender.Location = new System.Drawing.Point(131, 111);
            this.lblAgeToGender.Name = "lblAgeToGender";
            this.lblAgeToGender.Size = new System.Drawing.Size(129, 16);
            this.lblAgeToGender.TabIndex = 14;
            this.lblAgeToGender.Text = "Edad segun Sexo";
            // 
            // txtThreadId
            // 
            this.txtThreadId.Location = new System.Drawing.Point(183, 19);
            this.txtThreadId.Name = "txtThreadId";
            this.txtThreadId.Size = new System.Drawing.Size(100, 22);
            this.txtThreadId.TabIndex = 13;
            // 
            // lblThread
            // 
            this.lblThread.AutoSize = true;
            this.lblThread.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThread.Location = new System.Drawing.Point(17, 19);
            this.lblThread.Name = "lblThread";
            this.lblThread.Size = new System.Drawing.Size(148, 20);
            this.lblThread.TabIndex = 12;
            this.lblThread.Text = "Procesando Hilo";
            // 
            // frmINEC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1475, 746);
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

