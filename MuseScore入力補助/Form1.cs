using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace MuseScore入力補助
{
    public partial class Form1 : Form
    {
        public Form3()
        {
            InitializeComponent();
            this.TopMost = true;

            //txt.Text = Clipboard.GetText();//クリップボードの内容をテキストボックスに設定するようにしてもよい
            /*
//cmb.Text == "-"　における例
Hay-yō-shēv-ʿal-ḥūḡ-hā-ʾā-reṣ,
wə-yōshə-ḇê-hā-ka-ḥă-ḡā-ḇîm;
han-nō-ṭeh-khaḏ-ḏōq-shā-mayim,
way-yim-ta-ḥēm-kā-ʾō-hel-lā-shā-ḇeṯ.
             */

            txt.SelectionStart = 0;
            文字選択();

        }
        void 文字選択()
        {
            if (cmb.Text == "一文字") { 次の一文字(); }
            else if (cmb.Text == "space") { 次の区切り(' '); }
            else if (cmb.Text == "-") { 次の区切り('-'); }
            this.Text = txt.SelectedText.Trim();
        }

        private void txt_MouseUp(object sender, MouseEventArgs e)
        {
            this.Text = txt.SelectedText;//選択文字が次の歌詞入力の対象になる
            if (this.Text == "") { this.Text = "MuseScoreにおける歌詞入力補助ツール"; }
        }

        //フォーカスを持たないフォーム
        private const int WS_EX_NOACTIVATE = 0x8000000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                if (!base.DesignMode)
                {
                    p.ExStyle = p.ExStyle | (WS_EX_NOACTIVATE);
                }
                return p;
            }
        }

        void 次の一文字()
        {
            if (txt.SelectionLength == 1)
            {
                txt.SelectionStart++;
                this.Text = txt.SelectedText;
            }
        }
        void 次の区切り(char sp)
        {
            // 次の検索開始位置
            int s = txt.SelectionStart;

            if (txt.SelectionLength > 0)
                s += txt.SelectionLength + 1;

            // 改行を読み飛ばす
            while (s < txt.Text.Length &&
                   (txt.Text[s] == '\r' || txt.Text[s] == '\n'))
            { s++; }

            if (s >= txt.Text.Length) return;

            // 区切り文字を設定
            char[] separators = new[] { sp, '\r', '\n' }; ;

            // 次の区切りまでの長さを取得
            int e = txt.Text.IndexOfAny(separators, s);

            if (e < 0)
                e = txt.Text.Length;

            // 選択
            txt.SelectionStart = s;
            txt.SelectionLength = e - s;
        }

        private async void lbl_next_MouseDown(object sender, MouseEventArgs e)
        {
            this.lbl_next.BackColor = System.Drawing.Color.Yellow;
            string sendText = this.Text;
            if (!string.IsNullOrEmpty(sendText))
            {
                Clipboard.SetText(sendText);
                await Task.Delay(200);
                SendKeys.SendWait("^v");
            }
        }

        private void lbl_next_MouseUp(object sender, MouseEventArgs e)
        {
            lbl_next.BackColor = System.Drawing.SystemColors.Control;
            文字選択();
        }

        private void lbl_sp_MouseDown(object sender, MouseEventArgs e)
        {
            lbl_sp.BackColor = System.Drawing.Color.Yellow;
            SendKeys.Send(" ");//歌詞入力中にspaceで次の音符に移動
        }

        private void lbl_sp_MouseUp(object sender, MouseEventArgs e)
        {
            lbl_sp.BackColor = System.Drawing.SystemColors.Control;
        }

        private void lbl_ctrlL_MouseDown(object sender, MouseEventArgs e) { SendKeys.Send("^l"); }//音符を選択してCtrl+Lで歌詞入力

        private async void lbl反映_Click(object sender, EventArgs e)
        {
            txt.Text = Clipboard.GetText();
            await Task.Delay(100);
            txt.SelectionStart = 0;
            文字選択();
        }
    }
}
