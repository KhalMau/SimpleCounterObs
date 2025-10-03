

namespace SimpleCounterObs
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Button btnInc;
        private System.Windows.Forms.Button btnDec;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblOverlay;
        private System.Windows.Forms.TextBox txtOverlay;
        private System.Windows.Forms.Button btnCopy;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// 

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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtTitle = new TextBox();
            lblTitle = new Label();
            lblValue = new Label();
            btnInc = new Button();
            btnDec = new Button();
            btnReset = new Button();
            lblOverlay = new Label();
            txtOverlay = new TextBox();
            btnCopy = new Button();
            button1 = new Button();
            SuspendLayout();
            // 
            // txtTitle
            // 
            txtTitle.Location = new Point(103, 45);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(287, 23);
            txtTitle.TabIndex = 0;
            txtTitle.TextChanged += txtTitle_TextChanged;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(25, 53);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(33, 15);
            lblTitle.TabIndex = 1;
            lblTitle.Text = "Title:";
            // 
            // lblValue
            // 
            lblValue.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblValue.Location = new Point(209, 98);
            lblValue.Name = "lblValue";
            lblValue.Size = new Size(75, 57);
            lblValue.TabIndex = 2;
            lblValue.Text = "0";
            lblValue.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnInc
            // 
            btnInc.Location = new Point(107, 95);
            btnInc.Name = "btnInc";
            btnInc.Size = new Size(82, 27);
            btnInc.TabIndex = 3;
            btnInc.Text = "+1";
            btnInc.UseVisualStyleBackColor = true;
            btnInc.Click += btnInc_Click;
            // 
            // btnDec
            // 
            btnDec.Location = new Point(107, 128);
            btnDec.Name = "btnDec";
            btnDec.Size = new Size(82, 27);
            btnDec.TabIndex = 4;
            btnDec.Text = "−1";
            btnDec.UseVisualStyleBackColor = true;
            btnDec.Click += btnDec_Click;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(209, 170);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(75, 27);
            btnReset.TabIndex = 5;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += btnReset_Click;
            // 
            // lblOverlay
            // 
            lblOverlay.AutoSize = true;
            lblOverlay.Location = new Point(25, 16);
            lblOverlay.Name = "lblOverlay";
            lblOverlay.Size = new Size(72, 15);
            lblOverlay.TabIndex = 6;
            lblOverlay.Text = "URL overlay:";
            
            // 
            // txtOverlay
            // 
            txtOverlay.Location = new Point(396, 12);
            txtOverlay.Name = "txtOverlay";
            txtOverlay.ReadOnly = true;
            txtOverlay.Size = new Size(10, 23);
            txtOverlay.TabIndex = 7;
            txtOverlay.Visible = false;
            
            // 
            // btnCopy
            // 
            btnCopy.Location = new Point(103, 12);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(287, 23);
            btnCopy.TabIndex = 8;
            btnCopy.Text = "Copy";
            btnCopy.UseVisualStyleBackColor = true;
            btnCopy.Click += btnCopy_Click;
            // 
            // button1
            // 
            button1.Location = new Point(107, 170);
            button1.Name = "button1";
            button1.Size = new Size(82, 27);
            button1.TabIndex = 9;
            button1.Text = "Settings";
            button1.UseVisualStyleBackColor = true;
            button1.Click += btnCfg_Click;
            // 
            // Form1
            // 
            AcceptButton = btnInc;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(418, 209);
            Controls.Add(button1);
            Controls.Add(btnCopy);
            Controls.Add(txtOverlay);
            Controls.Add(lblOverlay);
            Controls.Add(btnReset);
            Controls.Add(btnDec);
            Controls.Add(btnInc);
            Controls.Add(lblValue);
            Controls.Add(lblTitle);
            Controls.Add(txtTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Simple Counter";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }





        #endregion

        private Button button1;
    }
}
