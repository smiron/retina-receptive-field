namespace RetinaReceptiveFieldFilter
{
    partial class Prezentation
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
            this.components = new System.ComponentModel.Container();
            this.drawArea = new System.Windows.Forms.PictureBox();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.cameraFpsLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.drawArea)).BeginInit();
            this.SuspendLayout();
            // 
            // drawArea
            // 
            this.drawArea.Location = new System.Drawing.Point(0, 0);
            this.drawArea.Name = "drawArea";
            this.drawArea.Size = new System.Drawing.Size(652, 501);
            this.drawArea.TabIndex = 0;
            this.drawArea.TabStop = false;
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.TimerTick);
            // 
            // cameraFpsLabel
            // 
            this.cameraFpsLabel.AutoSize = true;
            this.cameraFpsLabel.Location = new System.Drawing.Point(13, 487);
            this.cameraFpsLabel.Name = "cameraFpsLabel";
            this.cameraFpsLabel.Size = new System.Drawing.Size(0, 13);
            this.cameraFpsLabel.TabIndex = 1;
            // 
            // Prezentation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 513);
            this.Controls.Add(this.cameraFpsLabel);
            this.Controls.Add(this.drawArea);
            this.Name = "Prezentation";
            this.Text = "Retina Receptive Field ";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PrezentationFormClosed);
            this.Load += new System.EventHandler(this.PrezentationLoad);
            ((System.ComponentModel.ISupportInitialize)(this.drawArea)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox drawArea;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label cameraFpsLabel;
    }
}

