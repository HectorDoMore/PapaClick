namespace PapaClick.GUI;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem tsmFile;
    private System.Windows.Forms.ToolStripMenuItem tsmFileExit;
    private System.Windows.Forms.ToolStripMenuItem tsmHelp;
    private System.Windows.Forms.ToolStripMenuItem tsmHelpAbout;
    private System.Windows.Forms.TableLayoutPanel layout;
    private System.Windows.Forms.Label lblInterval;
    private System.Windows.Forms.NumericUpDown numInterval;
    private System.Windows.Forms.Label lblButton;
    private System.Windows.Forms.ComboBox cboButton;
    private System.Windows.Forms.Label lblClickType;
    private System.Windows.Forms.ComboBox cboClickType;
    private System.Windows.Forms.FlowLayoutPanel flowButtons;
    private System.Windows.Forms.Button btnStart;
    private System.Windows.Forms.Button btnStop;
    private System.Windows.Forms.StatusStrip statusStrip;
    private System.Windows.Forms.ToolStripStatusLabel lblStatus;

    private void InitializeComponent()
    {
        this.menuStrip = new System.Windows.Forms.MenuStrip();
        this.tsmFile = new System.Windows.Forms.ToolStripMenuItem();
        this.tsmFileExit = new System.Windows.Forms.ToolStripMenuItem();
        this.tsmHelp = new System.Windows.Forms.ToolStripMenuItem();
        this.tsmHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
        this.layout = new System.Windows.Forms.TableLayoutPanel();
        this.lblInterval = new System.Windows.Forms.Label();
        this.numInterval = new System.Windows.Forms.NumericUpDown();
        this.lblButton = new System.Windows.Forms.Label();
        this.cboButton = new System.Windows.Forms.ComboBox();
        this.lblClickType = new System.Windows.Forms.Label();
        this.cboClickType = new System.Windows.Forms.ComboBox();
        this.flowButtons = new System.Windows.Forms.FlowLayoutPanel();
        this.btnStart = new System.Windows.Forms.Button();
        this.btnStop = new System.Windows.Forms.Button();
        this.statusStrip = new System.Windows.Forms.StatusStrip();
        this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
        this.menuStrip.SuspendLayout();
        this.layout.SuspendLayout();
        this.flowButtons.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.numInterval).BeginInit();
        this.statusStrip.SuspendLayout();
        this.SuspendLayout();
        //
        // menuStrip
        //
        this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.tsmFile, this.tsmHelp });
        this.menuStrip.Location = new System.Drawing.Point(0, 0);
        this.menuStrip.Name = "menuStrip";
        this.menuStrip.Size = new System.Drawing.Size(480, 25);
        this.menuStrip.TabIndex = 0;
        //
        // tsmFile
        //
        this.tsmFile.Name = "tsmFile";
        this.tsmFile.Size = new System.Drawing.Size(44, 21);
        this.tsmFile.Text = "文件";
        this.tsmFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.tsmFileExit });
        //
        // tsmFileExit
        //
        this.tsmFileExit.Name = "tsmFileExit";
        this.tsmFileExit.Size = new System.Drawing.Size(124, 22);
        this.tsmFileExit.Text = "退出";
        this.tsmFileExit.Click += new System.EventHandler(this.tsmFileExit_Click);
        //
        // tsmHelp
        //
        this.tsmHelp.Name = "tsmHelp";
        this.tsmHelp.Size = new System.Drawing.Size(44, 21);
        this.tsmHelp.Text = "帮助";
        this.tsmHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.tsmHelpAbout });
        //
        // tsmHelpAbout
        //
        this.tsmHelpAbout.Name = "tsmHelpAbout";
        this.tsmHelpAbout.Size = new System.Drawing.Size(100, 22);
        this.tsmHelpAbout.Text = "关于";
        this.tsmHelpAbout.Click += new System.EventHandler(this.tsmHelpAbout_Click);
        //
        // layout
        //
        this.layout.ColumnCount = 2;
        this.layout.RowCount = 4;
        this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90));
        this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100));
        for (int i = 0; i < 4; i++)
        {
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38));
        }
        this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
        this.layout.Padding = new System.Windows.Forms.Padding(14, 10, 14, 6);
        this.layout.Name = "layout";
        this.layout.TabIndex = 1;
        //
        // lblInterval
        //
        this.lblInterval.Name = "lblInterval";
        this.lblInterval.Text = "点击间隔";
        this.lblInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblInterval.Dock = System.Windows.Forms.DockStyle.Fill;
        this.layout.Controls.Add(this.lblInterval, 0, 0);
        //
        // numInterval
        //
        this.numInterval.Name = "numInterval";
        this.numInterval.Minimum = 10;
        this.numInterval.Maximum = 60000;
        this.numInterval.Increment = 10;
        this.numInterval.Value = 100;
        this.numInterval.Dock = System.Windows.Forms.DockStyle.Fill;
        this.numInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
        this.layout.Controls.Add(this.numInterval, 1, 0);
        //
        // lblButton
        //
        this.lblButton.Name = "lblButton";
        this.lblButton.Text = "鼠标按键";
        this.lblButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblButton.Dock = System.Windows.Forms.DockStyle.Fill;
        this.layout.Controls.Add(this.lblButton, 0, 1);
        //
        // cboButton
        //
        this.cboButton.Name = "cboButton";
        this.cboButton.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cboButton.Items.AddRange(new object[] { "左键", "右键" });
        this.cboButton.SelectedIndex = 0;
        this.cboButton.Dock = System.Windows.Forms.DockStyle.Fill;
        this.cboButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.layout.Controls.Add(this.cboButton, 1, 1);
        //
        // lblClickType
        //
        this.lblClickType.Name = "lblClickType";
        this.lblClickType.Text = "点击方式";
        this.lblClickType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        this.lblClickType.Dock = System.Windows.Forms.DockStyle.Fill;
        this.layout.Controls.Add(this.lblClickType, 0, 2);
        //
        // cboClickType
        //
        this.cboClickType.Name = "cboClickType";
        this.cboClickType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cboClickType.Items.AddRange(new object[] { "单击", "双击" });
        this.cboClickType.SelectedIndex = 0;
        this.cboClickType.Dock = System.Windows.Forms.DockStyle.Fill;
        this.cboClickType.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.layout.Controls.Add(this.cboClickType, 1, 2);
        //
        // flowButtons
        //
        this.flowButtons.Name = "flowButtons";
        this.flowButtons.Dock = System.Windows.Forms.DockStyle.Fill;
        this.layout.SetColumnSpan(this.flowButtons, 2);
        this.flowButtons.Controls.Add(this.btnStart);
        this.flowButtons.Controls.Add(this.btnStop);
        this.layout.Controls.Add(this.flowButtons, 0, 3);
        //
        // btnStart
        //
        this.btnStart.Name = "btnStart";
        this.btnStart.Text = "开始 (F5)";
        this.btnStart.Size = new System.Drawing.Size(90, 28);
        this.btnStart.UseVisualStyleBackColor = true;
        this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
        //
        // btnStop
        //
        this.btnStop.Name = "btnStop";
        this.btnStop.Text = "停止 (F6)";
        this.btnStop.Size = new System.Drawing.Size(90, 28);
        this.btnStop.Enabled = false;
        this.btnStop.UseVisualStyleBackColor = true;
        this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
        //
        // statusStrip
        //
        this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblStatus });
        this.statusStrip.Location = new System.Drawing.Point(0, 275);
        this.statusStrip.Name = "statusStrip";
        this.statusStrip.Size = new System.Drawing.Size(480, 22);
        //
        // lblStatus
        //
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Text = "就绪";
        //
        // MainForm
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(480, 297);
        this.Controls.Add(this.layout);
        this.Controls.Add(this.statusStrip);
        this.Controls.Add(this.menuStrip);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.Name = "MainForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "PapaClick";
        this.menuStrip.ResumeLayout(false);
        this.menuStrip.PerformLayout();
        this.layout.ResumeLayout(false);
        this.flowButtons.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.numInterval).EndInit();
        this.statusStrip.ResumeLayout(false);
        this.statusStrip.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }
}
