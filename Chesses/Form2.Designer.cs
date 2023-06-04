namespace Chesses
{
    partial class Form2
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
            this.recording = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // recording
            // 
            this.recording.AutoSize = true;
            this.recording.Location = new System.Drawing.Point(836, 224);
            this.recording.Name = "recording";
            this.recording.Size = new System.Drawing.Size(0, 13);
            this.recording.TabIndex = 97;
            // 
            // Form2
            // 
            this.ClientSize = new System.Drawing.Size(604, 501);
            this.Controls.Add(this.recording);
            this.Name = "Form2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion
        private System.Windows.Forms.Label recording;
    }
}

