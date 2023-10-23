namespace SpreadsheetGUI
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileButtonMenuStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadBackgroundImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setBackgroundOpacityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpButtonMenuStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.CellInformationStrip = new System.Windows.Forms.ToolStrip();
            this.CellNameTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.CellValueTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.CellContentTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.UsernameTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.IPTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.ConnectButton = new System.Windows.Forms.ToolStripButton();
            this.UndoButton = new System.Windows.Forms.ToolStripButton();
            this.RevertButton = new System.Windows.Forms.ToolStripButton();
            this.SpreadsheetPanel = new SS.SpreadsheetPanel();
            this.menuStrip1.SuspendLayout();
            this.CellInformationStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileButtonMenuStrip,
            this.toolStripMenuItem1,
            this.HelpButtonMenuStrip});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(800, 28);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "MenuStrip";
            // 
            // FileButtonMenuStrip
            // 
            this.FileButtonMenuStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem});
            this.FileButtonMenuStrip.Name = "FileButtonMenuStrip";
            this.FileButtonMenuStrip.Size = new System.Drawing.Size(46, 24);
            this.FileButtonMenuStrip.Text = "File";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadBackgroundImageToolStripMenuItem,
            this.setBackgroundOpacityToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(49, 24);
            this.toolStripMenuItem1.Text = "Edit";
            // 
            // loadBackgroundImageToolStripMenuItem
            // 
            this.loadBackgroundImageToolStripMenuItem.Name = "loadBackgroundImageToolStripMenuItem";
            this.loadBackgroundImageToolStripMenuItem.Size = new System.Drawing.Size(254, 26);
            this.loadBackgroundImageToolStripMenuItem.Text = "Load Background Image";
            this.loadBackgroundImageToolStripMenuItem.Click += new System.EventHandler(this.loadBackgroundImageToolStripMenuItem_Click);
            // 
            // setBackgroundOpacityToolStripMenuItem
            // 
            this.setBackgroundOpacityToolStripMenuItem.Name = "setBackgroundOpacityToolStripMenuItem";
            this.setBackgroundOpacityToolStripMenuItem.Size = new System.Drawing.Size(254, 26);
            this.setBackgroundOpacityToolStripMenuItem.Text = "Set Background Opacity";
            this.setBackgroundOpacityToolStripMenuItem.Click += new System.EventHandler(this.setBackgroundOpacityToolStripMenuItem_Click);
            // 
            // HelpButtonMenuStrip
            // 
            this.HelpButtonMenuStrip.Name = "HelpButtonMenuStrip";
            this.HelpButtonMenuStrip.Size = new System.Drawing.Size(55, 24);
            this.HelpButtonMenuStrip.Text = "Help";
            this.HelpButtonMenuStrip.Click += new System.EventHandler(this.HelpButtonMenuStrip_Click);
            // 
            // CellInformationStrip
            // 
            this.CellInformationStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.CellInformationStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CellNameTextBox,
            this.CellValueTextBox,
            this.CellContentTextBox,
            this.UsernameTextBox,
            this.IPTextBox,
            this.ConnectButton,
            this.UndoButton,
            this.RevertButton});
            this.CellInformationStrip.Location = new System.Drawing.Point(0, 28);
            this.CellInformationStrip.Name = "CellInformationStrip";
            this.CellInformationStrip.Size = new System.Drawing.Size(800, 27);
            this.CellInformationStrip.TabIndex = 2;
            this.CellInformationStrip.Text = "toolStrip1";
            // 
            // CellNameTextBox
            // 
            this.CellNameTextBox.Name = "CellNameTextBox";
            this.CellNameTextBox.ReadOnly = true;
            this.CellNameTextBox.Size = new System.Drawing.Size(100, 27);
            // 
            // CellValueTextBox
            // 
            this.CellValueTextBox.Name = "CellValueTextBox";
            this.CellValueTextBox.ReadOnly = true;
            this.CellValueTextBox.Size = new System.Drawing.Size(100, 27);
            // 
            // CellContentTextBox
            // 
            this.CellContentTextBox.Name = "CellContentTextBox";
            this.CellContentTextBox.Size = new System.Drawing.Size(100, 27);
            this.CellContentTextBox.Leave += new System.EventHandler(this.CellContentTextBox_Leave);
            this.CellContentTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CellContentTextBox_KeyDown);
            // 
            // UsernameTextBox
            // 
            this.UsernameTextBox.Name = "UsernameTextBox";
            this.UsernameTextBox.Size = new System.Drawing.Size(100, 27);
            this.UsernameTextBox.Text = "Username";
            // 
            // IPTextBox
            // 
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(100, 27);
            this.IPTextBox.Text = "IP";
            // 
            // ConnectButton
            // 
            this.ConnectButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ConnectButton.Image = ((System.Drawing.Image)(resources.GetObject("ConnectButton.Image")));
            this.ConnectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ConnectButton.Name = "ConnectButton";
            this.ConnectButton.Size = new System.Drawing.Size(67, 24);
            this.ConnectButton.Text = "Connect";
            this.ConnectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // UndoButton
            // 
            this.UndoButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.UndoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.UndoButton.Image = ((System.Drawing.Image)(resources.GetObject("UndoButton.Image")));
            this.UndoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.UndoButton.Name = "UndoButton";
            this.UndoButton.RightToLeftAutoMirrorImage = true;
            this.UndoButton.Size = new System.Drawing.Size(29, 24);
            this.UndoButton.Text = "UndoButton";
            this.UndoButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.UndoButton.Click += new System.EventHandler(this.UndoButton_Click);
            // 
            // RevertButton
            // 
            this.RevertButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.RevertButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RevertButton.Image = ((System.Drawing.Image)(resources.GetObject("RevertButton.Image")));
            this.RevertButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RevertButton.Name = "RevertButton";
            this.RevertButton.Size = new System.Drawing.Size(29, 24);
            this.RevertButton.Text = "RevertButton";
            this.RevertButton.Click += new System.EventHandler(this.RevertButton_Click);
            // 
            // SpreadsheetPanel
            // 
            this.SpreadsheetPanel.BackColor = System.Drawing.Color.Transparent;
            this.SpreadsheetPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SpreadsheetPanel.Location = new System.Drawing.Point(0, 55);
            this.SpreadsheetPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SpreadsheetPanel.Name = "SpreadsheetPanel";
            this.SpreadsheetPanel.Size = new System.Drawing.Size(800, 395);
            this.SpreadsheetPanel.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SpreadsheetPanel);
            this.Controls.Add(this.CellInformationStrip);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Spreadsheet";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.CellInformationStrip.ResumeLayout(false);
            this.CellInformationStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SS.SpreadsheetPanel SpreadsheetPanel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileButtonMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem HelpButtonMenuStrip;
        private System.Windows.Forms.ToolStrip CellInformationStrip;
        private System.Windows.Forms.ToolStripTextBox CellNameTextBox;
        private System.Windows.Forms.ToolStripTextBox CellValueTextBox;
        private System.Windows.Forms.ToolStripTextBox CellContentTextBox;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadBackgroundImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setBackgroundOpacityToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox UsernameTextBox;
        private System.Windows.Forms.ToolStripTextBox IPTextBox;
        private System.Windows.Forms.ToolStripButton ConnectButton;
        private System.Windows.Forms.ToolStripButton UndoButton;
        private System.Windows.Forms.ToolStripButton RevertButton;
    }
}

