using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Tesseract;//https://github.com/tesseract-ocr/tessdata_best/tree/main

namespace OCR
{
    public partial class ReadLines : Form
    {
        string tessdata_folder = @"C:\something\tessdata";

        string tessdata_lang()
        {
                 if (cmb1.Text == "日本語") { return "jpn_best"; }
            else if (cmb1.Text == "English") { return "eng_best"; }
            return "eng_best";
        }
        PageSegMode SegMode() {
                 if (cmb2.Text == "SingleLine") { return PageSegMode.SingleLine; }
            else if (cmb2.Text == "Lines") { return PageSegMode.SingleLine; }
            else if (cmb2.Text == "Auto") { return PageSegMode.Auto; }
            return PageSegMode.Auto; ;
        }

        public ReadLines(string[] args)
        {
            InitializeComponent();

            if (args.Length>1) {
                     if (args[1] == "jpn") { cmb1.Text = "日本語"; }
                else if (args[1] == "eng") { cmb1.Text = "English"; }
            }

            try
            {
                System.Drawing.Image im = Clipboard.GetImage();
                if (im != null) { pic.Image = im; tryOCR(); }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        private void btn_copy_Click(object sender, EventArgs e)
        {
            text.Focus();
            text.SelectAll();
            text.Copy();
        }

        private void pic_MouseDown(object sender, MouseEventArgs e) { pic.Image = Clipboard.GetImage(); tryOCR(); }
        void tryOCR() {
            try
            {
                if (cmb2.Text == "Lines") { text.Text = OCR_行分割(pic.Image, new Bitmap(行間塗(new Bitmap(pic.Image),Color.Red)), tessdata_lang()); }
                else { text.Text = OCR(pic.Image, tessdata_lang(), SegMode()); }
                setrtf();
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }
        void setrtf()
        {
            string rtf = @"{\rtf1\ansi\deff0
\viewkind4
\pard\sl330\slmult0
" + text.Text.Replace("\r\n", @"\par ")
 .Replace("\n", @"\par ")
 .Replace("\r", "") +
@"\par}";
            text.Rtf = rtf;
        }
        string OCR(System.Drawing.Image bmp, string tessdata, PageSegMode pm)
        {
            using (Bitmap source = new Bitmap(bmp))
            using (var engine = new TesseractEngine(tessdata_folder, tessdata, EngineMode.Default))
            {
                    using (Bitmap bitmap = new Bitmap(source))
                    using (Pix img = PixConverter.ToPix(bitmap))
                    using (var page = engine.Process(img, pm))
                    { return  page.GetText(); }
            }
        }
        string OCR_行分割(System.Drawing.Image bmp, Bitmap spl, string tessdata)
        {
            using (Bitmap source = new Bitmap(bmp))
            using (var engine = new TesseractEngine(tessdata_folder, tessdata, EngineMode.Default))
            {
                //engine.SetVariable("tessedit_char_whitelist", "0123456789A");
                System.Drawing.Image[] rows = SplitImage(source, spl, Color.Red);
                string t;
                StringBuilder sb = new StringBuilder();
                foreach (System.Drawing.Image row in rows)
                {
                    using (row)
                    using (Bitmap bitmap = new Bitmap(row))
                    using (Pix img = PixConverter.ToPix(bitmap))
                    using (var page = engine.Process(img, PageSegMode.SingleLine))//PageSegMode.SingleLine PageSegMode.Auto
                    {
                        t= page.GetText().Trim();
                        if (string.IsNullOrWhiteSpace(t) == false) { sb.AppendLine(t); }
                    }
                }
                return sb.ToString();
            }
        }
        System.Drawing.Image[] SplitImage(Bitmap bmp, Bitmap spl, Color col)
        {
            List<System.Drawing.Image> images = new List<System.Drawing.Image>();
            int startY = 0;
            bool f; f = false;
            for (int y = 1; y < bmp.Height; y++)
            {
                Color currentColor = spl.GetPixel(0, y);
                if (currentColor.ToArgb() != col.ToArgb() && f == false)
                {
                    f = true;
                    startY = y - 1;
                    //Console.Write("start: " + y);
                }
                else if (currentColor.ToArgb() == col.ToArgb() && f == true)
                {
                    f = false;
                    int height = y - startY;
                    images.Add(
                        bmp.Clone(
                            new Rectangle(0, startY, bmp.Width, height),
                            bmp.PixelFormat
                        )
                    );
                    //Console.Write("- end: " + y + "\r\n");
                }
            }
            return images.ToArray();
        }
        Image 行間塗(Bitmap img, Color col)
        {
            int w = img.Width, h = img.Height;
            byte[] src = new byte[w * h * 4];
            byte[] dst = new byte[w * h * 4];
            bool[] linF = new bool[w];

            // データ吸出し 
            BitmapData bmpdat = img.LockBits(new Rectangle(Point.Empty, img.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            Marshal.Copy(bmpdat.Scan0, src, 0, src.Length);
            Marshal.Copy(bmpdat.Scan0, dst, 0, dst.Length);
            img.UnlockBits(bmpdat);

            // 調査
            for (int r = 0; r < h; r++)
            {
                linF[r] = true;
                for (int c = 0; c < w; c++)
                {
                    if (
                        src[(r * w + c) * 4 + 2] != src[(r * w + 0) * 4 + 2] ||
                        src[(r * w + c) * 4 + 1] != src[(r * w + 0) * 4 + 1] ||
                        src[(r * w + c) * 4 + 0] != src[(r * w + 0) * 4 + 0]
                        )
                    { linF[r] = false; break; }
                }
            }

            // データ変更
            for (int r = 0; r < h; r++)
            {
                for (int c = 0; c < w; c++)
                {
                    if (linF[r] == true)
                    {
                        //dst[(r * w + c) * 4 + 3] = col.A;//A
                        dst[(r * w + c) * 4 + 2] = col.R;//R
                        dst[(r * w + c) * 4 + 1] = col.G;//G
                        dst[(r * w + c) * 4 + 0] = col.B;//B
                    }
                }
            }

            // データ書き出し 
            Bitmap result = new Bitmap(w, h);
            bmpdat = result.LockBits(new Rectangle(Point.Empty, img.Size), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            Marshal.Copy(dst, 0, bmpdat.Scan0, dst.Length);
            result.UnlockBits(bmpdat);
            return result;
        }
    }
}
