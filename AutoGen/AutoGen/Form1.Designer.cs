namespace AutoGen
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxLog = new System.Windows.Forms.RichTextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.checkBoxFit = new System.Windows.Forms.CheckBox();
            this.checkBoxSel = new System.Windows.Forms.CheckBox();
            this.textBoxBrowse = new System.Windows.Forms.TextBox();
            this.checkBoxProbe = new System.Windows.Forms.CheckBox();
            this.buttonComp = new System.Windows.Forms.Button();
            this.buttonNet = new System.Windows.Forms.Button();
            this.buttonSelective = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxLog
            // 
            this.textBoxLog.Location = new System.Drawing.Point(12, 73);
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.Size = new System.Drawing.Size(360, 177);
            this.textBoxLog.TabIndex = 0;
            this.textBoxLog.Text = "";
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "(.pcb文件)|*.pcb|全部文件|*.*";
            this.openFileDialog.InitialDirectory = "C:\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Session Log :";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(307, 34);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(65, 21);
            this.buttonBrowse.TabIndex = 3;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // checkBoxFit
            // 
            this.checkBoxFit.AutoSize = true;
            this.checkBoxFit.Location = new System.Drawing.Point(160, 12);
            this.checkBoxFit.Name = "checkBoxFit";
            this.checkBoxFit.Size = new System.Drawing.Size(96, 16);
            this.checkBoxFit.TabIndex = 2;
            this.checkBoxFit.Text = "Fit Selected";
            this.checkBoxFit.UseVisualStyleBackColor = true;
            this.checkBoxFit.CheckedChanged += new System.EventHandler(this.checkBoxFit_CheckedChanged);
            // 
            // checkBoxSel
            // 
            this.checkBoxSel.AutoSize = true;
            this.checkBoxSel.Location = new System.Drawing.Point(270, 12);
            this.checkBoxSel.Name = "checkBoxSel";
            this.checkBoxSel.Size = new System.Drawing.Size(102, 16);
            this.checkBoxSel.TabIndex = 1;
            this.checkBoxSel.Text = "Move Selected";
            this.checkBoxSel.UseVisualStyleBackColor = true;
            this.checkBoxSel.CheckedChanged += new System.EventHandler(this.checkBoxSel_CheckedChanged);
            // 
            // textBoxBrowse
            // 
            this.textBoxBrowse.Location = new System.Drawing.Point(12, 34);
            this.textBoxBrowse.Name = "textBoxBrowse";
            this.textBoxBrowse.Size = new System.Drawing.Size(285, 21);
            this.textBoxBrowse.TabIndex = 4;
            // 
            // checkBoxProbe
            // 
            this.checkBoxProbe.AutoSize = true;
            this.checkBoxProbe.Location = new System.Drawing.Point(12, 12);
            this.checkBoxProbe.Name = "checkBoxProbe";
            this.checkBoxProbe.Size = new System.Drawing.Size(102, 16);
            this.checkBoxProbe.TabIndex = 2;
            this.checkBoxProbe.Text = "Cross Probing";
            this.checkBoxProbe.UseVisualStyleBackColor = true;
            this.checkBoxProbe.CheckedChanged += new System.EventHandler(this.checkBoxProbe_CheckedChanged);
            // 
            // buttonComp
            // 
            this.buttonComp.Location = new System.Drawing.Point(135, 227);
            this.buttonComp.Name = "buttonComp";
            this.buttonComp.Size = new System.Drawing.Size(75, 23);
            this.buttonComp.TabIndex = 5;
            this.buttonComp.Text = "Comps";
            this.buttonComp.UseVisualStyleBackColor = true;
            this.buttonComp.Click += new System.EventHandler(this.buttonComp_Click);
            // 
            // buttonNet
            // 
            this.buttonNet.Location = new System.Drawing.Point(216, 227);
            this.buttonNet.Name = "buttonNet";
            this.buttonNet.Size = new System.Drawing.Size(75, 23);
            this.buttonNet.TabIndex = 5;
            this.buttonNet.Text = "Nets";
            this.buttonNet.UseVisualStyleBackColor = true;
            this.buttonNet.Click += new System.EventHandler(this.buttonNet_Click);
            // 
            // buttonSelective
            // 
            this.buttonSelective.Location = new System.Drawing.Point(297, 227);
            this.buttonSelective.Name = "buttonSelective";
            this.buttonSelective.Size = new System.Drawing.Size(75, 23);
            this.buttonSelective.TabIndex = 5;
            this.buttonSelective.Text = "Selective";
            this.buttonSelective.UseVisualStyleBackColor = true;
            this.buttonSelective.Click += new System.EventHandler(this.buttonSelective_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 262);
            this.Controls.Add(this.buttonSelective);
            this.Controls.Add(this.buttonNet);
            this.Controls.Add(this.buttonComp);
            this.Controls.Add(this.textBoxBrowse);
            this.Controls.Add(this.buttonBrowse);
            this.Controls.Add(this.checkBoxProbe);
            this.Controls.Add(this.checkBoxFit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxSel);
            this.Controls.Add(this.textBoxLog);
            this.MaximumSize = new System.Drawing.Size(400, 300);
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "Form1";
            this.Text = "OrCAD <---> Xpedition";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox textBoxLog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.CheckBox checkBoxFit;
        private System.Windows.Forms.CheckBox checkBoxSel;
        private System.Windows.Forms.TextBox textBoxBrowse;
        private System.Windows.Forms.CheckBox checkBoxProbe;
        private System.Windows.Forms.Button buttonComp;
        private System.Windows.Forms.Button buttonNet;
        private System.Windows.Forms.Button buttonSelective;
    }
}

