using System;
using System.Windows.Forms;
using System.Drawing;

namespace フレーム型フォーム
{
    public partial class Frame_main : Form
    {
        public Frame_main(string[] args)
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.BackColor = Color.Red;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            posSize = this.Size;

            int[] a=new int[4]; bool ssa1=false; bool ssa2 = false;
            if (args.Length ==2 && int.TryParse(args[0],out a[0]) && int.TryParse(args[1], out a[1])) { setRectangle(a[0], a[1]); ssa1 = true; }
            if (args.Length ==4 && int.TryParse(args[0], out a[0]) && int.TryParse(args[1], out a[1]) && int.TryParse(args[2], out a[2]) && int.TryParse(args[3], out a[3]))
             { this.Location = new Point(a[0], a[1]); ssa2 = true; setRectangle(a[2], a[3]); ssa1 = true; }
            if (ssa1 == false) { setRectangle(this.Width, this.Height); }
            if (ssa2 == false) { this.Location = new Point(Cursor.Position.X - this.Width / 2, Cursor.Position.Y); }
            this.Opacity = 0.5;
            this.TopMost = true;

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            tip.SetToolTip(this, "ドラッグ/右クリックしてください");
            fm.Owner=this;
        }

        // メニュー画面
        Frame_menu fm = new Frame_menu();

        // 右クリックでメニュー画面が起動
        void Rclick() { fm.show(); }


        // 位置やサイズの変更 について
        void setRectangle(int w, int h)
        {
            w = Math.Max(w, grip * 2 + 1);
            h = Math.Max(h, grip * 2 + 1);
            Size r = new Size(w, h);

            if (posSize == r) { return; }
            posSize = r;

            using (System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath())
            {
                gp.AddRectangle(new Rectangle(0, 0, r.Width, r.Height));
                gp.AddRectangle(new Rectangle(grip, grip, r.Width - grip * 2, r.Height - grip * 2));
                RectangleF bounds = gp.GetBounds();
                this.SetBounds(0, 0, (int)(bounds.Width) * 1, (int)(bounds.Height) * 1, BoundsSpecified.Size);

                Region old = this.Region;
                this.Region = new Region(gp);//やや重い。別メソッドに分けた方が軽いのかと思って試したが効果のある方法は見つけられていない（再描画抑制工夫や描画タイミングに関するWindows仕様に配慮するのが妥当だと思います）
                old?.Dispose();
            }
        }
        /// <summary>
        /// 把持ふとさ
        /// </summary>
        int grip = 5;

        /// <summary>
        /// 旧Sizeを保持。無駄な再描画の抑制や初期のWM_WINDOWPOSCHANGING前に発生する予期せぬResize対応のため
        /// </summary>
        Size posSize;

        /// <summary>
        /// フォーム把持やサイズ変更のためのint mode。ResizeMode mode; にした方が読み易いかもしれないが整数値をあてた。
        /// </summary>
        int mode; 
        enum ResizeMode
        {
            //modeはenumにした方が読み易いだろうか
            None,
            Move,
            Bottom,
            Right,
            Left,
            RightBottom,
            RightTop,
            LeftTop,
            LeftBottom
        }

        int mx; int my; Point pos; Size poz;
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            if (posSize != this.Size) {
                this.Size=posSize; 
                setRectangle(posSize.Width, posSize.Height);
            }

            mode = 0;
            if (e.Button == MouseButtons.Right) { Rclick();return; }

            this.Capture = true;
            mx= Cursor.Position.X; my= Cursor.Position.Y; 
            pos= this.Location;
            poz = this.Size;
                 if (e.Y < grip)               { mode = 1; }
            else if (e.Y > this.Height - grip) { mode = 2; }
            else if (e.X > this.Width - grip)  { mode = 3; }
            else if (e.X < grip)               { mode = 4; }
                        
            if (e.X > this.Width - grip && e.Y > this.Height - grip) { mode = 5; }
            if (e.X > this.Width - grip && e.Y < grip)               { mode = 6; }
            if (e.X < grip && e.Y < grip)                            { mode = 7; }
            if (e.X < grip && e.Y > this.Height - grip)              { mode = 8; }

            tip.ShowAlways=true;
            tip.Show(tipString(), this, tipPosition());
        }
        
        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            cursorType(e);
            if (this.Capture == false) { return; }

            if (mode == 1) { this.Location = new Point(pos.X + Cursor.Position.X - mx, pos.Y + Cursor.Position.Y - my); }
            if (mode == 2) { int h = poz.Height + Cursor.Position.Y - my; setRectangle(this.Width, h); }
            if (mode == 3) { int w = poz.Width + Cursor.Position.X - mx;  setRectangle(w, this.Height); }
            if (mode == 4) { 
                this.Location = new Point(pos.X + Cursor.Position.X - mx, pos.Y); 
                int w = poz.Width - Cursor.Position.X + mx; setRectangle(w, this.Height); 
            }
            if (mode == 5) {
                int w = poz.Width + Cursor.Position.X - mx;
                int h = poz.Height + Cursor.Position.Y - my; setRectangle(w, h); 
            }
            if (mode == 6)
            {
                this.Location = new Point(pos.X, pos.Y + Cursor.Position.Y - my);
                int w = poz.Width + Cursor.Position.X - mx; 
                int h = poz.Height - Cursor.Position.Y + my; setRectangle(w, h);
            }
            if (mode == 7)
            {
                this.Location = new Point(pos.X + Cursor.Position.X - mx, pos.Y + Cursor.Position.Y - my);
                int w = poz.Width - Cursor.Position.X + mx;
                int h = poz.Height - Cursor.Position.Y + my; setRectangle(w, h);
            }
            if (mode == 8)
            {
                this.Location = new Point(pos.X + Cursor.Position.X - mx, pos.Y);
                int w = poz.Width - Cursor.Position.X + mx;
                int h = poz.Height + Cursor.Position.Y - my; setRectangle(w, h);
            }
            tip.Show(tipString(), this, tipPosition());
        }
        
        private void Form_MouseUp(object sender, MouseEventArgs e) { 
            this.Capture = false; tip.ShowAlways = false; 
            tip.SetToolTip(this, "ドラッグ/右クリックしてください");
        }


        // Cursorやtipの表示 について
        Point tipPosition() { return this.PointToClient(Cursor.Position); }
        string tipString() { return (this.Location.ToString() + ", " + this.Size.ToString()).Replace("{","").Replace("}", ""); }
        void cursorType(MouseEventArgs e) {
            if (e.Y < grip && this.Cursor != System.Windows.Forms.Cursors.Hand) { this.Cursor = System.Windows.Forms.Cursors.Hand; }
            else if (e.Y > this.Height - grip && this.Cursor != System.Windows.Forms.Cursors.SizeNS) { this.Cursor = System.Windows.Forms.Cursors.SizeNS; }
            else if (e.X > this.Width - grip && this.Cursor != System.Windows.Forms.Cursors.SizeWE) { this.Cursor = System.Windows.Forms.Cursors.SizeWE;  }
            else if (e.X < grip && this.Cursor != System.Windows.Forms.Cursors.SizeWE) { this.Cursor = System.Windows.Forms.Cursors.SizeWE; }

            if (e.X > this.Width - grip && e.Y > this.Height - grip && this.Cursor != System.Windows.Forms.Cursors.SizeNWSE) { this.Cursor = System.Windows.Forms.Cursors.SizeNWSE;  }
            if (e.X > this.Width - grip && e.Y < grip && this.Cursor != System.Windows.Forms.Cursors.SizeNESW) { this.Cursor = System.Windows.Forms.Cursors.SizeNESW; }
            if (e.X < grip && e.Y < grip && this.Cursor != System.Windows.Forms.Cursors.SizeNWSE) { this.Cursor = System.Windows.Forms.Cursors.SizeNWSE; }
            if (e.X < grip && e.Y > this.Height - grip && this.Cursor != System.Windows.Forms.Cursors.SizeNESW) { this.Cursor = System.Windows.Forms.Cursors.SizeNESW; }
        }
        private void Frame_main_MouseLeave(object sender, System.EventArgs e) { tip.Hide(this); }


        // 外フォームから閉じるためのコード
        bool isclosing = false;
        public void closeForm() {
            if (isclosing == true) { return; }
            isclosing = true;
            this.Close();
        }


        // あると便利な機能群。外フォーム(本件の場合はFrame_menu fm)から呼ぶことを想定しています
        public void camera() {
            this.Opacity = 0;

            Bitmap bmp = new Bitmap(this.Size.Width, this.Size.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            { g.CopyFromScreen(this.Location.X, this.Location.Y, 0, 0, bmp.Size); }
            Clipboard.SetImage(bmp);

            bmp.Dispose();
            this.Opacity = 0.5;
        }

        public void clipRect() { Clipboard.SetText(this.Location.X.ToString() +","+ this.Location.Y.ToString() + "," + this.Size.Width.ToString() + "," + this.Size.Height.ToString() ); }

        public void setPosition(int x, int y, int w, int h) {
            this.Location=new Point(x,y);
            this.Size = new Size(w,h);
            setRectangle(w, h);
        }

    }

  // Designer
  partial class Frame_main
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

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.tip = new System.Windows.Forms.ToolTip(this.components);
        this.SuspendLayout();
        // 
        // Frame_main
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Name = "Frame_main";
        this.Text = "Form1";
        this.MouseLeave += new System.EventHandler(this.Frame_main_MouseLeave);
        this.ResumeLayout(false);
    }

    private System.Windows.Forms.ToolTip tip;
  }

  // 子フォーム（例）
    public partial class Frame_menu : Form
    {
        public Frame_menu()
        {
            InitializeComponent();
            set_events();

            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            rectBox.Select();
            tip.SetToolTip(btn_camera, "範囲のスクリーンショット画像をクリップボードにコピーします");
            tip.SetToolTip(btn_Rect, "範囲のRectangle情報（座標とサイズ）をクリップボードにコピーします");
            tip.SetToolTip(this, "Rectangle情報をペーストすると設定されます");
        }
        public void show() {
            this.TopMost = true;
            this.Location = new Point(Cursor.Position.X - this.Width / 2, Cursor.Position.Y - 69);
            this.Show();
        }

        // Events
        private void set_events() {
            this.btn_camera.Click += new System.EventHandler(this.btn_camera_Click);
            this.btn_Rect.Click += new System.EventHandler(this.btn_Rect_Click);
            this.rectBox.TextChanged += new System.EventHandler(this.rectBox_TextChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.menu_FormClosing);
            this.MouseEnter += new System.EventHandler(this.Frame_menu_MouseEnter);
        }
        private void menu_FormClosing(object sender, FormClosingEventArgs e) { ((Frame_main)(this.Owner)).closeForm(); }

        private void btn_camera_Click(object sender, EventArgs e) {
            this.Opacity = 0;
            System.Threading.Thread.Sleep(100);
            ((Frame_main)(this.Owner)).camera();
            this.Opacity = 1;
        }

        private void btn_Rect_Click(object sender, EventArgs e) { ((Frame_main)(this.Owner)).clipRect();  }

        private void Frame_menu_MouseEnter(object sender, EventArgs e) { rectBox.Select(); }
        private void rectBox_TextChanged(object sender, EventArgs e)
        {
            string[] v = rectBox.Text.Split(new[] { "," }, StringSplitOptions.None);
            if (v.Length != 4) { return; }
            int[] w = new int[v.Length];
            for (int i = 0; i < v.Length; i++) {
                if (int.TryParse(v[i], out w[i]) == false) { return; }
            }
            ((Frame_main)(this.Owner)).setPosition(w[0], w[1], w[2], w[3]);
            rectBox.Text = "";
        }
    }

// 子フォームのDesigner
    partial class Frame_menu
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

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btn_camera = new System.Windows.Forms.Button();
            this.btn_Rect = new System.Windows.Forms.Button();
            this.rectBox = new System.Windows.Forms.TextBox();
            this.tip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // btn_camera
            // 
            this.btn_camera.Location = new System.Drawing.Point(12, 12);
            this.btn_camera.Name = "btn_camera";
            this.btn_camera.Size = new System.Drawing.Size(75, 23);
            this.btn_camera.TabIndex = 1;
            this.btn_camera.Text = "camera";
            this.btn_camera.UseVisualStyleBackColor = true;
            // 
            // btn_Rect
            // 
            this.btn_Rect.Location = new System.Drawing.Point(11, 41);
            this.btn_Rect.Name = "btn_Rect";
            this.btn_Rect.Size = new System.Drawing.Size(75, 23);
            this.btn_Rect.TabIndex = 2;
            this.btn_Rect.Text = "rectangle";
            this.btn_Rect.UseVisualStyleBackColor = true;
            // 
            // rectBox
            // 
            this.rectBox.Location = new System.Drawing.Point(103, 43);
            this.rectBox.Name = "rectBox";
            this.rectBox.Size = new System.Drawing.Size(128, 19);
            this.rectBox.TabIndex = 3;
            // 
            // Frame_menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(97, 75);
            this.Controls.Add(this.rectBox);
            this.Controls.Add(this.btn_Rect);
            this.Controls.Add(this.btn_camera);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "Frame_menu";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btn_camera;
        private System.Windows.Forms.Button btn_Rect;
        private System.Windows.Forms.TextBox rectBox;
        private System.Windows.Forms.ToolTip tip;
    }
}
