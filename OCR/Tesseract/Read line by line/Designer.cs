namespace OCR
{
    partial class ReadLines
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
            this.pic = new System.Windows.Forms.PictureBox();
            this.btn_copy = new System.Windows.Forms.Button();
            this.text = new System.Windows.Forms.RichTextBox();
            this.cmb1 = new System.Windows.Forms.ComboBox();
            this.cmb2 = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pic)).BeginInit();
            this.SuspendLayout();
            // 
            // pic
            // 
            this.pic.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pic.BackColor = System.Drawing.Color.White;
            this.pic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pic.Location = new System.Drawing.Point(12, 38);
            this.pic.Name = "pic";
            this.pic.Size = new System.Drawing.Size(452, 400);
            this.pic.TabIndex = 0;
            this.pic.TabStop = false;
            this.pic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pic_MouseDown);
            // 
            // btn_copy
            // 
            this.btn_copy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_copy.Location = new System.Drawing.Point(722, 4);
            this.btn_copy.Name = "btn_copy";
            this.btn_copy.Size = new System.Drawing.Size(75, 23);
            this.btn_copy.TabIndex = 2;
            this.btn_copy.Text = "copy";
            this.btn_copy.UseVisualStyleBackColor = true;
            this.btn_copy.Click += new System.EventHandler(this.btn_copy_Click);
            // 
            // text
            // 
            this.text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.text.Location = new System.Drawing.Point(470, 38);
            this.text.Name = "text";
            this.text.Size = new System.Drawing.Size(327, 400);
            this.text.TabIndex = 3;
            this.text.Text = "";
            this.text.WordWrap = false;
            // 
            // cmb1
            // 
            this.cmb1.FormattingEnabled = true;
            this.cmb1.Items.AddRange(new object[] {
            "日本語",
            "English"});
            this.cmb1.Location = new System.Drawing.Point(12, 12);
            this.cmb1.Name = "cmb1";
            this.cmb1.Size = new System.Drawing.Size(178, 20);
            this.cmb1.TabIndex = 5;
            this.cmb1.Text = "English";
            // 
            // cmb2
            // 
            this.cmb2.FormattingEnabled = true;
            this.cmb2.Items.AddRange(new object[] {
            "SingleLine",
            "Lines",
            "Auto"});
            this.cmb2.Location = new System.Drawing.Point(196, 12);
            this.cmb2.Name = "cmb2";
            this.cmb2.Size = new System.Drawing.Size(178, 20);
            this.cmb2.TabIndex = 6;
            this.cmb2.Text = "Lines";
            // 
            // FreeClip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cmb2);
            this.Controls.Add(this.cmb1);
            this.Controls.Add(this.text);
            this.Controls.Add(this.btn_copy);
            this.Controls.Add(this.pic);
            this.Name = "FreeClip";
            this.Text = "Clip OCR";
            ((System.ComponentModel.ISupportInitialize)(this.pic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pic;
        private System.Windows.Forms.Button btn_copy;
        private System.Windows.Forms.RichTextBox text;
        private System.Windows.Forms.ComboBox cmb1;
        private System.Windows.Forms.ComboBox cmb2;
    }
}
