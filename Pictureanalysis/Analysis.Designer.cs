namespace Pictureanalysis
{
    partial class Analysis
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.resullt_box = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图像识别ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.生成训练样本ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.旋转训练集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.识别ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.批量识别ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.计算识别率ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.图像处理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.灰度化ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.二值化ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.去躁ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.倾斜纠正ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.细化ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(28, 112);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(301, 153);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "预览";
            // 
            // resullt_box
            // 
            this.resullt_box.Location = new System.Drawing.Point(624, 112);
            this.resullt_box.Multiline = true;
            this.resullt_box.Name = "resullt_box";
            this.resullt_box.Size = new System.Drawing.Size(176, 56);
            this.resullt_box.TabIndex = 17;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.图像识别ToolStripMenuItem,
            this.识别ToolStripMenuItem,
            this.图像处理ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(832, 28);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开ToolStripMenuItem,
            this.保存ToolStripMenuItem});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.打开ToolStripMenuItem.Text = "打开";
            this.打开ToolStripMenuItem.Click += new System.EventHandler(this.打开ToolStripMenuItem_Click);
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.保存ToolStripMenuItem.Text = "保存";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem_Click);
            // 
            // 图像识别ToolStripMenuItem
            // 
            this.图像识别ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.生成训练样本ToolStripMenuItem,
            this.旋转训练集ToolStripMenuItem});
            this.图像识别ToolStripMenuItem.Name = "图像识别ToolStripMenuItem";
            this.图像识别ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.图像识别ToolStripMenuItem.Text = "训练样本";
            // 
            // 生成训练样本ToolStripMenuItem
            // 
            this.生成训练样本ToolStripMenuItem.Enabled = false;
            this.生成训练样本ToolStripMenuItem.Name = "生成训练样本ToolStripMenuItem";
            this.生成训练样本ToolStripMenuItem.Size = new System.Drawing.Size(187, 26);
            this.生成训练样本ToolStripMenuItem.Text = "1.生成训练样本";
            this.生成训练样本ToolStripMenuItem.Click += new System.EventHandler(this.生成训练样本ToolStripMenuItem_Click);
            // 
            // 旋转训练集ToolStripMenuItem
            // 
            this.旋转训练集ToolStripMenuItem.Name = "旋转训练集ToolStripMenuItem";
            this.旋转训练集ToolStripMenuItem.Size = new System.Drawing.Size(187, 26);
            this.旋转训练集ToolStripMenuItem.Text = "2.旋转训练集";
            this.旋转训练集ToolStripMenuItem.Click += new System.EventHandler(this.旋转训练集ToolStripMenuItem_Click);
            // 
            // 识别ToolStripMenuItem
            // 
            this.识别ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.批量识别ToolStripMenuItem,
            this.计算识别率ToolStripMenuItem});
            this.识别ToolStripMenuItem.Name = "识别ToolStripMenuItem";
            this.识别ToolStripMenuItem.Size = new System.Drawing.Size(51, 24);
            this.识别ToolStripMenuItem.Text = "识别";
            // 
            // 批量识别ToolStripMenuItem
            // 
            this.批量识别ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.批量识别ToolStripMenuItem.Name = "批量识别ToolStripMenuItem";
            this.批量识别ToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.批量识别ToolStripMenuItem.Text = "批量识别图片";
            this.批量识别ToolStripMenuItem.Click += new System.EventHandler(this.批量识别ToolStripMenuItem_Click);
            // 
            // 计算识别率ToolStripMenuItem
            // 
            this.计算识别率ToolStripMenuItem.Name = "计算识别率ToolStripMenuItem";
            this.计算识别率ToolStripMenuItem.Size = new System.Drawing.Size(174, 26);
            this.计算识别率ToolStripMenuItem.Text = "计算识别率";
            this.计算识别率ToolStripMenuItem.Click += new System.EventHandler(this.计算识别率ToolStripMenuItem_Click);
            // 
            // 图像处理ToolStripMenuItem
            // 
            this.图像处理ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.灰度化ToolStripMenuItem,
            this.二值化ToolStripMenuItem,
            this.去躁ToolStripMenuItem,
            this.倾斜纠正ToolStripMenuItem,
            this.细化ToolStripMenuItem});
            this.图像处理ToolStripMenuItem.Name = "图像处理ToolStripMenuItem";
            this.图像处理ToolStripMenuItem.Size = new System.Drawing.Size(81, 24);
            this.图像处理ToolStripMenuItem.Text = "图像处理";
            // 
            // 灰度化ToolStripMenuItem
            // 
            this.灰度化ToolStripMenuItem.Name = "灰度化ToolStripMenuItem";
            this.灰度化ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.灰度化ToolStripMenuItem.Text = "灰度化";
            this.灰度化ToolStripMenuItem.Click += new System.EventHandler(this.灰度化ToolStripMenuItem_Click);
            // 
            // 二值化ToolStripMenuItem
            // 
            this.二值化ToolStripMenuItem.Name = "二值化ToolStripMenuItem";
            this.二值化ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.二值化ToolStripMenuItem.Text = "二值化";
            this.二值化ToolStripMenuItem.Click += new System.EventHandler(this.二值化ToolStripMenuItem_Click);
            // 
            // 去躁ToolStripMenuItem
            // 
            this.去躁ToolStripMenuItem.Name = "去躁ToolStripMenuItem";
            this.去躁ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.去躁ToolStripMenuItem.Text = "去躁";
            this.去躁ToolStripMenuItem.Click += new System.EventHandler(this.去躁ToolStripMenuItem_Click);
            // 
            // 倾斜纠正ToolStripMenuItem
            // 
            this.倾斜纠正ToolStripMenuItem.Name = "倾斜纠正ToolStripMenuItem";
            this.倾斜纠正ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.倾斜纠正ToolStripMenuItem.Text = "倾斜纠正";
            this.倾斜纠正ToolStripMenuItem.Click += new System.EventHandler(this.倾斜纠正ToolStripMenuItem_Click);
            // 
            // 细化ToolStripMenuItem
            // 
            this.细化ToolStripMenuItem.Name = "细化ToolStripMenuItem";
            this.细化ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.细化ToolStripMenuItem.Text = "细化";
            this.细化ToolStripMenuItem.Click += new System.EventHandler(this.细化ToolStripMenuItem_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(621, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 19;
            this.label3.Text = "结果";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(624, 199);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(128, 23);
            this.button3.TabIndex = 31;
            this.button3.Text = "单个字符识别";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // Analysis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 515);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.resullt_box);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Analysis";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Analysis_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox resullt_box;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 图像识别ToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 识别ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 批量识别ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 生成训练样本ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 计算识别率ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 旋转训练集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 图像处理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 灰度化ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 二值化ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 去躁ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 倾斜纠正ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 细化ToolStripMenuItem;
        private System.Windows.Forms.Button button3;
    }
}

