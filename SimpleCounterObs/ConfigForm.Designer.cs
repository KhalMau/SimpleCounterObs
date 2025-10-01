using System.Windows.Forms;

namespace SimpleCounterObs
{
    partial class ConfigForm
    {
        private System.ComponentModel.IContainer components = null;

        private TabControl tabs;
        private TabPage tabHotkeys;
        private TabPage tabStyle;

        private TextBox txtInc;
        private TextBox txtDec;
        private TextBox txtReset;
        private Label lblInc;
        private Label lblDec;
        private Label lblReset;

        private ComboBox cboFont;
        private NumericUpDown nudSize;
        private CheckBox chkMinimal;
        private CheckBox chkShowTitle;
        private Label lblFont;
        private Label lblSize;

        private Button btnOk;
        private Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabHotkeys = new System.Windows.Forms.TabPage();
            this.tabStyle = new System.Windows.Forms.TabPage();

            this.txtInc = new System.Windows.Forms.TextBox();
            this.txtDec = new System.Windows.Forms.TextBox();
            this.txtReset = new System.Windows.Forms.TextBox();
            this.lblInc = new System.Windows.Forms.Label();
            this.lblDec = new System.Windows.Forms.Label();
            this.lblReset = new System.Windows.Forms.Label();

            this.cboFont = new System.Windows.Forms.ComboBox();
            this.nudSize = new System.Windows.Forms.NumericUpDown();
            this.chkMinimal = new System.Windows.Forms.CheckBox();
            this.chkShowTitle = new System.Windows.Forms.CheckBox();
            this.lblFont = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();

            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.nudSize)).BeginInit();
            this.tabs.SuspendLayout();
            this.tabHotkeys.SuspendLayout();
            this.tabStyle.SuspendLayout();
            this.SuspendLayout();

            // tabs
            this.tabs.Controls.Add(this.tabHotkeys);
            this.tabs.Controls.Add(this.tabStyle);
            this.tabs.Location = new System.Drawing.Point(12, 12);
            this.tabs.Name = "tabs";
            this.tabs.Size = new System.Drawing.Size(420, 200);
            this.tabs.TabIndex = 0;

            // tabHotkeys
            this.tabHotkeys.Text = "Hotkeys";
            this.tabHotkeys.UseVisualStyleBackColor = true;
            this.tabHotkeys.Name = "tabHotkeys";

            // lblInc
            this.lblInc.AutoSize = true;
            this.lblInc.Location = new System.Drawing.Point(16, 18);
            this.lblInc.Name = "lblInc";
            this.lblInc.Size = new System.Drawing.Size(68, 15);
            this.lblInc.Text = "Increment:";

            // txtInc
            this.txtInc.Location = new System.Drawing.Point(120, 15);
            this.txtInc.Name = "txtInc";
            this.txtInc.Size = new System.Drawing.Size(250, 23);

            // lblDec
            this.lblDec.AutoSize = true;
            this.lblDec.Location = new System.Drawing.Point(16, 58);
            this.lblDec.Name = "lblDec";
            this.lblDec.Size = new System.Drawing.Size(72, 15);
            this.lblDec.Text = "Decrement:";

            // txtDec
            this.txtDec.Location = new System.Drawing.Point(120, 55);
            this.txtDec.Name = "txtDec";
            this.txtDec.Size = new System.Drawing.Size(250, 23);

            // lblReset
            this.lblReset.AutoSize = true;
            this.lblReset.Location = new System.Drawing.Point(16, 98);
            this.lblReset.Name = "lblReset";
            this.lblReset.Size = new System.Drawing.Size(38, 15);
            this.lblReset.Text = "Reset:";

            // txtReset
            this.txtReset.Location = new System.Drawing.Point(120, 95);
            this.txtReset.Name = "txtReset";
            this.txtReset.Size = new System.Drawing.Size(250, 23);

            // add hotkeys controls to tab
            this.tabHotkeys.Controls.Add(this.lblInc);
            this.tabHotkeys.Controls.Add(this.txtInc);
            this.tabHotkeys.Controls.Add(this.lblDec);
            this.tabHotkeys.Controls.Add(this.txtDec);
            this.tabHotkeys.Controls.Add(this.lblReset);
            this.tabHotkeys.Controls.Add(this.txtReset);

            // tabStyle
            this.tabStyle.Text = "Style";
            this.tabStyle.UseVisualStyleBackColor = true;
            this.tabStyle.Name = "tabStyle";

            // lblFont
            this.lblFont.AutoSize = true;
            this.lblFont.Location = new System.Drawing.Point(16, 18);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(71, 15);
            this.lblFont.Text = "Font family:";

            // cboFont
            this.cboFont.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFont.Location = new System.Drawing.Point(120, 15);
            this.cboFont.Name = "cboFont";
            this.cboFont.Size = new System.Drawing.Size(250, 23);

            // lblSize
            this.lblSize.AutoSize = true;
            this.lblSize.Location = new System.Drawing.Point(16, 58);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(56, 15);
            this.lblSize.Text = "Font size:";

            // nudSize
            this.nudSize.Location = new System.Drawing.Point(120, 55);
            this.nudSize.Minimum = 10;
            this.nudSize.Maximum = 300;
            this.nudSize.Value = 64;
            this.nudSize.Name = "nudSize";
            this.nudSize.Size = new System.Drawing.Size(90, 23);

            // chkMinimal
            this.chkMinimal.AutoSize = true;
            this.chkMinimal.Location = new System.Drawing.Point(16, 95);
            this.chkMinimal.Name = "chkMinimal";
            this.chkMinimal.Size = new System.Drawing.Size(191, 19);
            this.chkMinimal.Text = "Solo número (sin fondo/box)";

            // chkShowTitle
            this.chkShowTitle.AutoSize = true;
            this.chkShowTitle.Location = new System.Drawing.Point(16, 125);
            this.chkShowTitle.Name = "chkShowTitle";
            this.chkShowTitle.Size = new System.Drawing.Size(101, 19);
            this.chkShowTitle.Text = "Mostrar título";
            this.chkShowTitle.Checked = true;

            // add style controls to tab
            this.tabStyle.Controls.Add(this.lblFont);
            this.tabStyle.Controls.Add(this.cboFont);
            this.tabStyle.Controls.Add(this.lblSize);
            this.tabStyle.Controls.Add(this.nudSize);
            this.tabStyle.Controls.Add(this.chkMinimal);
            this.tabStyle.Controls.Add(this.chkShowTitle);

            // btnOk
            this.btnOk.Location = new System.Drawing.Point(246, 220);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.Text = "OK";
            // (NO lambdas aquí; el click se cablea en ConfigForm.cs)

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(336, 220);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.Text = "Cancelar";
            // (NO lambdas aquí; el click se cablea en ConfigForm.cs)

            // form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 255);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuración";

            this.tabs.ResumeLayout(false);
            this.tabHotkeys.ResumeLayout(false);
            this.tabHotkeys.PerformLayout();
            this.tabStyle.ResumeLayout(false);
            this.tabStyle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSize)).EndInit();
            this.ResumeLayout(false);
        }
    }
}

