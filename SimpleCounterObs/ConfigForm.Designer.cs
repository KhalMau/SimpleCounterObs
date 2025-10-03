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
            tabs = new TabControl();
            tabHotkeys = new TabPage();
            lblInc = new Label();
            txtInc = new TextBox();
            lblDec = new Label();
            txtDec = new TextBox();
            lblReset = new Label();
            txtReset = new TextBox();
            tabStyle = new TabPage();
            lblFont = new Label();
            cboFont = new ComboBox();
            lblSize = new Label();
            nudSize = new NumericUpDown();
            chkMinimal = new CheckBox();
            chkShowTitle = new CheckBox();
            btnOk = new Button();
            btnCancel = new Button();
            tabs.SuspendLayout();
            tabHotkeys.SuspendLayout();
            tabStyle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudSize).BeginInit();
            SuspendLayout();
            // 
            // tabs
            // 
            tabs.Controls.Add(tabHotkeys);
            tabs.Controls.Add(tabStyle);
            tabs.Location = new Point(12, 12);
            tabs.Name = "tabs";
            tabs.SelectedIndex = 0;
            tabs.Size = new Size(420, 200);
            tabs.TabIndex = 0;
            // 
            // tabHotkeys
            // 
            tabHotkeys.Controls.Add(lblInc);
            tabHotkeys.Controls.Add(txtInc);
            tabHotkeys.Controls.Add(lblDec);
            tabHotkeys.Controls.Add(txtDec);
            tabHotkeys.Controls.Add(lblReset);
            tabHotkeys.Controls.Add(txtReset);
            tabHotkeys.Location = new Point(4, 24);
            tabHotkeys.Name = "tabHotkeys";
            tabHotkeys.Size = new Size(412, 172);
            tabHotkeys.TabIndex = 0;
            tabHotkeys.Text = "Hotkeys";
            tabHotkeys.UseVisualStyleBackColor = true;
            // 
            // lblInc
            // 
            lblInc.AutoSize = true;
            lblInc.Location = new Point(16, 18);
            lblInc.Name = "lblInc";
            lblInc.Size = new Size(64, 15);
            lblInc.TabIndex = 0;
            lblInc.Text = "Increment:";
            // 
            // txtInc
            // 
            txtInc.Location = new Point(120, 15);
            txtInc.Name = "txtInc";
            txtInc.ReadOnly = true;
            txtInc.Size = new Size(250, 23);
            txtInc.TabIndex = 1;
            // 
            // lblDec
            // 
            lblDec.AutoSize = true;
            lblDec.Location = new Point(16, 58);
            lblDec.Name = "lblDec";
            lblDec.Size = new Size(68, 15);
            lblDec.TabIndex = 2;
            lblDec.Text = "Decrement:";
            // 
            // txtDec
            // 
            txtDec.Location = new Point(120, 55);
            txtDec.Name = "txtDec";
            txtDec.ReadOnly = true;
            txtDec.Size = new Size(250, 23);
            txtDec.TabIndex = 3;
            // 
            // lblReset
            // 
            lblReset.AutoSize = true;
            lblReset.Location = new Point(16, 98);
            lblReset.Name = "lblReset";
            lblReset.Size = new Size(38, 15);
            lblReset.TabIndex = 4;
            lblReset.Text = "Reset:";
            // 
            // txtReset
            // 
            txtReset.Location = new Point(120, 95);
            txtReset.Name = "txtReset";
            txtReset.ReadOnly = true;
            txtReset.Size = new Size(250, 23);
            txtReset.TabIndex = 5;
            // 
            // tabStyle
            // 
            tabStyle.Controls.Add(lblFont);
            tabStyle.Controls.Add(cboFont);
            tabStyle.Controls.Add(lblSize);
            tabStyle.Controls.Add(nudSize);
            tabStyle.Controls.Add(chkMinimal);
            tabStyle.Controls.Add(chkShowTitle);
            tabStyle.Location = new Point(4, 24);
            tabStyle.Name = "tabStyle";
            tabStyle.Size = new Size(412, 172);
            tabStyle.TabIndex = 1;
            tabStyle.Text = "Style";
            tabStyle.UseVisualStyleBackColor = true;
            // 
            // lblFont
            // 
            lblFont.AutoSize = true;
            lblFont.Location = new Point(16, 18);
            lblFont.Name = "lblFont";
            lblFont.Size = new Size(70, 15);
            lblFont.TabIndex = 0;
            lblFont.Text = "Font family:";
            // 
            // cboFont
            // 
            cboFont.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFont.Location = new Point(120, 15);
            cboFont.Name = "cboFont";
            cboFont.Size = new Size(250, 23);
            cboFont.TabIndex = 1;
            // 
            // lblSize
            // 
            lblSize.AutoSize = true;
            lblSize.Location = new Point(16, 58);
            lblSize.Name = "lblSize";
            lblSize.Size = new Size(56, 15);
            lblSize.TabIndex = 2;
            lblSize.Text = "Font size:";
            // 
            // nudSize
            // 
            nudSize.Location = new Point(120, 55);
            nudSize.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudSize.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            nudSize.Name = "nudSize";
            nudSize.Size = new Size(90, 23);
            nudSize.TabIndex = 3;
            nudSize.Value = new decimal(new int[] { 64, 0, 0, 0 });
            // 
            // chkMinimal
            // 
            chkMinimal.AutoSize = true;
            chkMinimal.Checked = true;
            chkMinimal.CheckState = CheckState.Checked;
            chkMinimal.Location = new Point(16, 95);
            chkMinimal.Name = "chkMinimal";
            chkMinimal.Size = new Size(131, 19);
            chkMinimal.TabIndex = 4;
            chkMinimal.Text = "No background box";
            // 
            // chkShowTitle
            // 
            chkShowTitle.AutoSize = true;
            chkShowTitle.Checked = true;
            chkShowTitle.CheckState = CheckState.Checked;
            chkShowTitle.Location = new Point(16, 125);
            chkShowTitle.Name = "chkShowTitle";
            chkShowTitle.Size = new Size(81, 19);
            chkShowTitle.TabIndex = 5;
            chkShowTitle.Text = "Show Title";
            // 
            // btnOk
            // 
            btnOk.Location = new Point(246, 220);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 1;
            btnOk.Text = "OK";
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(336, 220);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            // 
            // ConfigForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(444, 255);
            Controls.Add(tabs);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ConfigForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            tabs.ResumeLayout(false);
            tabHotkeys.ResumeLayout(false);
            tabHotkeys.PerformLayout();
            tabStyle.ResumeLayout(false);
            tabStyle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudSize).EndInit();
            ResumeLayout(false);
        }
    }
}

