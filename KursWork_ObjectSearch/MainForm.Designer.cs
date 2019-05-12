namespace KursWork_ObjectSearch
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FindPointButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.InputFull_PictureBox = new System.Windows.Forms.PictureBox();
            this.InputSample_pictureBox = new System.Windows.Forms.PictureBox();
            this.OutputPictureBoxMatches = new System.Windows.Forms.PictureBox();
            this.capturedImageBox = new Emgu.CV.UI.ImageBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InputFull_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputSample_pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPictureBoxMatches)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.capturedImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // FindPointButton
            // 
            this.FindPointButton.Location = new System.Drawing.Point(698, 606);
            this.FindPointButton.Name = "FindPointButton";
            this.FindPointButton.Size = new System.Drawing.Size(187, 51);
            this.FindPointButton.TabIndex = 28;
            this.FindPointButton.Text = "EmguCV";
            this.FindPointButton.UseVisualStyleBackColor = true;
            this.FindPointButton.Click += new System.EventHandler(this.FindPointButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.InputFull_PictureBox);
            this.groupBox1.Controls.Add(this.InputSample_pictureBox);
            this.groupBox1.Location = new System.Drawing.Point(12, 372);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(680, 285);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Входные данные";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(413, 260);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Полное изображение";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 260);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Эталонное изображение";
            // 
            // InputFull_PictureBox
            // 
            this.InputFull_PictureBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.InputFull_PictureBox.Location = new System.Drawing.Point(297, 19);
            this.InputFull_PictureBox.Name = "InputFull_PictureBox";
            this.InputFull_PictureBox.Size = new System.Drawing.Size(367, 238);
            this.InputFull_PictureBox.TabIndex = 0;
            this.InputFull_PictureBox.TabStop = false;
            // 
            // InputSample_pictureBox
            // 
            this.InputSample_pictureBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.InputSample_pictureBox.Location = new System.Drawing.Point(6, 19);
            this.InputSample_pictureBox.Name = "InputSample_pictureBox";
            this.InputSample_pictureBox.Size = new System.Drawing.Size(274, 238);
            this.InputSample_pictureBox.TabIndex = 0;
            this.InputSample_pictureBox.TabStop = false;
            // 
            // OutputPictureBoxMatches
            // 
            this.OutputPictureBoxMatches.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.OutputPictureBoxMatches.Location = new System.Drawing.Point(12, 12);
            this.OutputPictureBoxMatches.Name = "OutputPictureBoxMatches";
            this.OutputPictureBoxMatches.Size = new System.Drawing.Size(579, 354);
            this.OutputPictureBoxMatches.TabIndex = 31;
            this.OutputPictureBoxMatches.TabStop = false;
            // 
            // capturedImageBox
            // 
            this.capturedImageBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.capturedImageBox.Location = new System.Drawing.Point(607, 12);
            this.capturedImageBox.Name = "capturedImageBox";
            this.capturedImageBox.Size = new System.Drawing.Size(538, 354);
            this.capturedImageBox.TabIndex = 33;
            this.capturedImageBox.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(698, 549);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(187, 51);
            this.button1.TabIndex = 34;
            this.button1.Text = "MikhailCV";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 661);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.capturedImageBox);
            this.Controls.Add(this.FindPointButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.OutputPictureBoxMatches);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InputFull_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputSample_pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPictureBoxMatches)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.capturedImageBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button FindPointButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox InputFull_PictureBox;
        private System.Windows.Forms.PictureBox InputSample_pictureBox;
        private System.Windows.Forms.PictureBox OutputPictureBoxMatches;
        private Emgu.CV.UI.ImageBox capturedImageBox;
        private System.Windows.Forms.Button button1;
    }
}

