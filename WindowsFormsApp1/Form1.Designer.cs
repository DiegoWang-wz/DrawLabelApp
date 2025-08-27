using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.printDocument_test = new System.Drawing.Printing.PrintDocument();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.toolbarPanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.WithDrawBtn = new System.Windows.Forms.Button();
            this.widthLabel = new System.Windows.Forms.Label();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.heightLabel = new System.Windows.Forms.Label();
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.setSizeButton = new System.Windows.Forms.Button();
            this.ExportBtn = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.containerPanel = new System.Windows.Forms.Panel();
            this.drawingPanel = new System.Windows.Forms.Panel();
            this.rightSplitContainer = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.detailGroupBox = new System.Windows.Forms.GroupBox();
            this.PrintDevice = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.PrintBtn = new System.Windows.Forms.Button();
            this.mainLayout.SuspendLayout();
            this.toolbarPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.containerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rightSplitContainer)).BeginInit();
            this.rightSplitContainer.Panel1.SuspendLayout();
            this.rightSplitContainer.Panel2.SuspendLayout();
            this.rightSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.bottomPanel.SuspendLayout();
            this.detailGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.toolbarPanel, 0, 0);
            this.mainLayout.Controls.Add(this.splitContainer1, 0, 1);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.RowCount = 2;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Size = new System.Drawing.Size(800, 600);
            this.mainLayout.TabIndex = 0;
            // 
            // toolbarPanel
            // 
            this.toolbarPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.toolbarPanel.Controls.Add(this.label2);
            this.toolbarPanel.Controls.Add(this.label1);
            this.toolbarPanel.Controls.Add(this.numericUpDown1);
            this.toolbarPanel.Controls.Add(this.WithDrawBtn);
            this.toolbarPanel.Controls.Add(this.widthLabel);
            this.toolbarPanel.Controls.Add(this.widthTextBox);
            this.toolbarPanel.Controls.Add(this.heightLabel);
            this.toolbarPanel.Controls.Add(this.heightTextBox);
            this.toolbarPanel.Controls.Add(this.setSizeButton);
            this.toolbarPanel.Controls.Add(this.ExportBtn);
            this.toolbarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolbarPanel.Location = new System.Drawing.Point(3, 3);
            this.toolbarPanel.Name = "toolbarPanel";
            this.toolbarPanel.Size = new System.Drawing.Size(794, 54);
            this.toolbarPanel.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 10F);
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(693, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 28);
            this.label2.TabIndex = 9;
            this.label2.Text = "软件里的单位\r\n都是毫米mm制!";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 11F);
            this.label1.Location = new System.Drawing.Point(547, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "线宽：";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("宋体", 11F);
            this.numericUpDown1.Increment = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.numericUpDown1.Location = new System.Drawing.Point(608, 18);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(67, 24);
            this.numericUpDown1.TabIndex = 7;
            this.numericUpDown1.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // WithDrawBtn
            // 
            this.WithDrawBtn.Font = new System.Drawing.Font("宋体", 11F);
            this.WithDrawBtn.Location = new System.Drawing.Point(436, 14);
            this.WithDrawBtn.Name = "WithDrawBtn";
            this.WithDrawBtn.Size = new System.Drawing.Size(90, 30);
            this.WithDrawBtn.TabIndex = 5;
            this.WithDrawBtn.Text = "撤销";
            this.WithDrawBtn.UseVisualStyleBackColor = true;
            this.WithDrawBtn.Click += new System.EventHandler(this.WithDrawBtn_Click);
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Font = new System.Drawing.Font("宋体", 11F);
            this.widthLabel.Location = new System.Drawing.Point(9, 21);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Size = new System.Drawing.Size(45, 15);
            this.widthLabel.TabIndex = 0;
            this.widthLabel.Text = "宽度:";
            // 
            // widthTextBox
            // 
            this.widthTextBox.Font = new System.Drawing.Font("宋体", 11F);
            this.widthTextBox.Location = new System.Drawing.Point(60, 17);
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.Size = new System.Drawing.Size(60, 24);
            this.widthTextBox.TabIndex = 1;
            this.widthTextBox.Text = "100";
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Font = new System.Drawing.Font("宋体", 11F);
            this.heightLabel.Location = new System.Drawing.Point(129, 21);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Size = new System.Drawing.Size(45, 15);
            this.heightLabel.TabIndex = 2;
            this.heightLabel.Text = "高度:";
            // 
            // heightTextBox
            // 
            this.heightTextBox.Font = new System.Drawing.Font("宋体", 11F);
            this.heightTextBox.Location = new System.Drawing.Point(180, 17);
            this.heightTextBox.Name = "heightTextBox";
            this.heightTextBox.Size = new System.Drawing.Size(60, 24);
            this.heightTextBox.TabIndex = 3;
            this.heightTextBox.Text = "80";
            // 
            // setSizeButton
            // 
            this.setSizeButton.Font = new System.Drawing.Font("宋体", 11F);
            this.setSizeButton.Location = new System.Drawing.Point(246, 14);
            this.setSizeButton.Name = "setSizeButton";
            this.setSizeButton.Size = new System.Drawing.Size(90, 30);
            this.setSizeButton.TabIndex = 4;
            this.setSizeButton.Text = "设置大小";
            this.setSizeButton.UseVisualStyleBackColor = true;
            this.setSizeButton.Click += new System.EventHandler(this.SetSizeButton_Click);
            // 
            // ExportBtn
            // 
            this.ExportBtn.Font = new System.Drawing.Font("宋体", 11F);
            this.ExportBtn.Location = new System.Drawing.Point(340, 13);
            this.ExportBtn.Name = "ExportBtn";
            this.ExportBtn.Size = new System.Drawing.Size(90, 30);
            this.ExportBtn.TabIndex = 6;
            this.ExportBtn.Text = "导出";
            this.ExportBtn.UseVisualStyleBackColor = true;
            this.ExportBtn.Click += new System.EventHandler(this.ExportBtn_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 63);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.containerPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.rightSplitContainer);
            this.splitContainer1.Size = new System.Drawing.Size(794, 534);
            this.splitContainer1.SplitterDistance = 529;
            this.splitContainer1.TabIndex = 1;
            // 
            // containerPanel
            // 
            this.containerPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.containerPanel.Controls.Add(this.drawingPanel);
            this.containerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerPanel.Location = new System.Drawing.Point(0, 0);
            this.containerPanel.Name = "containerPanel";
            this.containerPanel.Size = new System.Drawing.Size(529, 534);
            this.containerPanel.TabIndex = 0;
            // 
            // drawingPanel
            // 
            this.drawingPanel.BackColor = System.Drawing.Color.White;
            this.drawingPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.drawingPanel.Location = new System.Drawing.Point(64, 117);
            this.drawingPanel.Name = "drawingPanel";
            this.drawingPanel.Size = new System.Drawing.Size(400, 300);
            this.drawingPanel.TabIndex = 0;
            // 
            // rightSplitContainer
            // 
            this.rightSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.rightSplitContainer.Name = "rightSplitContainer";
            this.rightSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // rightSplitContainer.Panel1
            // 
            this.rightSplitContainer.Panel1.Controls.Add(this.dataGridView1);
            // 
            // rightSplitContainer.Panel2
            // 
            this.rightSplitContainer.Panel2.Controls.Add(this.bottomPanel);
            this.rightSplitContainer.Size = new System.Drawing.Size(261, 534);
            this.rightSplitContainer.SplitterDistance = 379;
            this.rightSplitContainer.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(261, 379);
            this.dataGridView1.TabIndex = 0;
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.Color.LightBlue;
            this.bottomPanel.Controls.Add(this.detailGroupBox);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomPanel.Location = new System.Drawing.Point(0, 0);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Padding = new System.Windows.Forms.Padding(5);
            this.bottomPanel.Size = new System.Drawing.Size(261, 151);
            this.bottomPanel.TabIndex = 0;
            // 
            // detailGroupBox
            // 
            this.detailGroupBox.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.detailGroupBox.Controls.Add(this.PrintDevice);
            this.detailGroupBox.Controls.Add(this.comboBox1);
            this.detailGroupBox.Controls.Add(this.PrintBtn);
            this.detailGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailGroupBox.Location = new System.Drawing.Point(5, 5);
            this.detailGroupBox.Name = "detailGroupBox";
            this.detailGroupBox.Padding = new System.Windows.Forms.Padding(5);
            this.detailGroupBox.Size = new System.Drawing.Size(251, 141);
            this.detailGroupBox.TabIndex = 0;
            this.detailGroupBox.TabStop = false;
            // 
            // PrintDevice
            // 
            this.PrintDevice.AutoSize = true;
            this.PrintDevice.Font = new System.Drawing.Font("宋体", 12F);
            this.PrintDevice.Location = new System.Drawing.Point(9, 25);
            this.PrintDevice.Name = "PrintDevice";
            this.PrintDevice.Size = new System.Drawing.Size(55, 16);
            this.PrintDevice.TabIndex = 3;
            this.PrintDevice.Text = "打印机";
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("宋体", 12F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(70, 22);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(162, 24);
            this.comboBox1.TabIndex = 2;
            // 
            // PrintBtn
            // 
            this.PrintBtn.Location = new System.Drawing.Point(12, 96);
            this.PrintBtn.Name = "PrintBtn";
            this.PrintBtn.Size = new System.Drawing.Size(108, 37);
            this.PrintBtn.TabIndex = 1;
            this.PrintBtn.Text = "打印";
            this.PrintBtn.UseVisualStyleBackColor = true;
            this.PrintBtn.Click += new System.EventHandler(this.PrintBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.mainLayout);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "标签绘制APP v1.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.mainLayout.ResumeLayout(false);
            this.toolbarPanel.ResumeLayout(false);
            this.toolbarPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.containerPanel.ResumeLayout(false);
            this.rightSplitContainer.Panel1.ResumeLayout(false);
            this.rightSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rightSplitContainer)).EndInit();
            this.rightSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.bottomPanel.ResumeLayout(false);
            this.detailGroupBox.ResumeLayout(false);
            this.detailGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Drawing.Printing.PrintDocument printDocument_test;
        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Panel toolbarPanel;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.TextBox widthTextBox;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.TextBox heightTextBox;
        private System.Windows.Forms.Button setSizeButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel containerPanel;
        private System.Windows.Forms.Panel drawingPanel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button WithDrawBtn;
        private System.Windows.Forms.Button ExportBtn;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        // 右侧上下分割容器及底部控件
        private System.Windows.Forms.SplitContainer rightSplitContainer;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.GroupBox detailGroupBox;
        private ComboBox comboBox1;
        private Button PrintBtn;
        private Label PrintDevice;
        private Label label1;
        private Label label2;
    }
}
