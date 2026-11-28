using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace 基礎
{ 
    public partial class BasicSample : Form
    {
        public BasicSample()
        {
            this.BackColor = Color.Red;

            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            WindowsFormの変形.文字(this, "変形フォーム", "メイリオ", 200);
            //WindowsFormの変形.任意の画像(this, "test.png", Color.FromArgb(255,255,255,255));//.exeと同じフォルダにtest.pngを置いてください

            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form_MouseUp);
            this.TopMost = true;
        }

        //ドラッグすると動きます
        Point pos;int mx; int my;  
        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            this.Capture = true;
            pos = this.Location;
            mx = Cursor.Position.X; my = Cursor.Position.Y;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Capture == false) { return; }
            this.Location = new Point(pos.X + Cursor.Position.X - mx, pos.Y + Cursor.Position.Y - my);
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            this.Capture = false;

            this.Close();//本件ではここで閉じるようにした
        }
    }

    // Designer
    partial class BasicSample
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            { components.Dispose(); }
            base.Dispose(disposing);
        }
    }
}

class WindowsFormの変形
{
    public static void 文字(Form fm, string st, string fontname, int fontsize)
    {
        using (GraphicsPath gp = new GraphicsPath())
        {
            using (StringFormat stringFormat = new StringFormat())
            using (FontFamily ff = new FontFamily(fontname))
            {
                stringFormat.FormatFlags = StringFormatFlags.NoWrap;
                stringFormat.Trimming = StringTrimming.None;
                gp.AddString(st, ff, 0, fontsize, new Point(0, 0), stringFormat);
            }
            Region newRegion = new Region(gp);
            Region oldRegion = fm.Region;
            fm.Region = newRegion;
            oldRegion?.Dispose();
            RectangleF bounds = gp.GetBounds();
            fm.SetBounds(fm.Left, fm.Top, (int)(bounds.Width) * 2, (int)(bounds.Height) * 2, BoundsSpecified.Size);//文字に対して十分に大きなサイズ
        }
    }

    // ほぼChatGPT様に書いていただいた
    public static void 任意の画像(Form fm, string 画像ファイルのpath, Color 透過色)
    {
        Bitmap bmp;

        // 画像を32bit ARGB形式に統一
        using (Bitmap source = new Bitmap(画像ファイルのpath))
        {
            bmp = new Bitmap(
                source.Width,
                source.Height,
                PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(bmp))
            { g.DrawImageUnscaled(source, 0, 0); }
        }

        bool 設定済み = false;

        try
        {
            bmp.MakeTransparent(透過色);

            using (GraphicsPath gp = new GraphicsPath())
            {
                bool f = false;

                int rowBytes = bmp.Width * 4;
                byte[] src = new byte[rowBytes];

                BitmapData bmpdat = bmp.LockBits(
                    new Rectangle(Point.Empty, bmp.Size),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb);

                try
                {
                    for (int r = 0; r < bmp.Height; r++)
                    {
                        // 各行をコピー
                        IntPtr row = IntPtr.Add(bmpdat.Scan0, r * bmpdat.Stride);
                        System.Runtime.InteropServices.Marshal.Copy( row, src, 0, rowBytes);
                        int start = -1;
                        for (int c = 0; c <= bmp.Width; c++)
                        {
                            bool solidF = c < bmp.Width && src[c * 4 + 3] != 0;
                            if (solidF && start == -1)
                            { start = c; }
                            else if (!solidF && start != -1)
                            {
                                gp.AddRectangle( new Rectangle(start, r, c - start, 1));
                                start = -1;
                                f = true;
                            }
                        }
                    }
                }
                finally
                {
                    bmp.UnlockBits(bmpdat);
                }

                if (!f) return;

                Region newRegion = new Region(gp);
                Region oldRegion = fm.Region;

                fm.Region = newRegion;
                oldRegion?.Dispose();

                fm.SetBounds(
                    fm.Left, fm.Top,
                    bmp.Width, bmp.Height,
                    BoundsSpecified.Size);

                fm.BackgroundImage = bmp;
                fm.BackgroundImageLayout = ImageLayout.None;

                設定済み = true;
            }
        }
        finally
        {
            if (!設定済み)
            {
                bmp.Dispose();
            }
        }
    }

}

