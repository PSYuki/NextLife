using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NextLife
{
    public partial class FormSound : Form
    {
        private Dictionary<string, string> soundFileName = new Dictionary<string, string>();
        private Dictionary<string, bool> soundEnable = new Dictionary<string, bool>();

        WMPLib.WindowsMediaPlayer mediaPlayer = new WMPLib.WindowsMediaPlayer();

        // 連想配列用のキーワード。ハッシュ計算用に使用。
        // 文字列はアプリケーション構成ファイル（App.config）におけるKeyとしても使用
        public const string BEFORE_0_MIN = "0_MIN";
        public const string BEFORE_1_MIN = "1_MIN";
        public const string BEFORE_3_MIN = "3_MIN";
        public const string BEFORE_5_MIN = "5_MIN";
        public const string BEFORE_10_MIN = "10_MIN";
        public const string BEFORE_30_MIN = "30_MIN";


        public FormSound()
        {
            InitializeComponent();

            // 音源ファイルとEnableフラグの連想配列を作成しておく
            // FormSoundのインスタンス作成時に一度作成すればよいためコンストラクタからcallする
            InitializeHashTable();
        }

        private void InitializeHashTable()
        {
            soundFileName.Add(BEFORE_0_MIN, "");
            soundFileName.Add(BEFORE_1_MIN, "");
            soundFileName.Add(BEFORE_3_MIN, "");
            soundFileName.Add(BEFORE_5_MIN, "");
            soundFileName.Add(BEFORE_10_MIN, "");
            soundFileName.Add(BEFORE_30_MIN, "");
            soundEnable.Add(BEFORE_0_MIN, false);
            soundEnable.Add(BEFORE_1_MIN, false);
            soundEnable.Add(BEFORE_3_MIN, false);
            soundEnable.Add(BEFORE_5_MIN, false);
            soundEnable.Add(BEFORE_10_MIN, false);
            soundEnable.Add(BEFORE_30_MIN, false);
        }

        private void FormSound_Load(object sender, EventArgs e)
        {
            // アプリケーション構成ファイルからコンポーネントへロードされた設定値を連想配列へ代入
            UpdateHashTable();
        }

        public Dictionary<string, string> getSoundFileNames()
        {
            return this.soundFileName;
        }

        public void setSoundFileNames(Dictionary<string, string> fileNames)
        {
            this.soundFileName = fileNames;
        }

        public Dictionary<string, bool> getSoundEnables()
        {
            return this.soundEnable;
        }

        public void setSoundEnables(Dictionary<string, bool> enables)
        {
            this.soundEnable = enables;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = getSoundFileName();
        }

        private string getSoundFileName()
        {
            string selectedFileName = "";
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                selectedFileName = openFileDialog.FileName;
            }

            return selectedFileName;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = getSoundFileName();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = getSoundFileName();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox4.Text = getSoundFileName();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox5.Text = getSoundFileName();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox6.Text = getSoundFileName();
        }

        private void playSound(CheckBox cb, TextBox tb)
        {
            string fileName = tb.Text;

            if (!System.IO.File.Exists(fileName))
            {
                cb.Checked = false;
                return;
            }

            /*
            if (cb.Checked)
            {
                mediaPlayer.URL = fileName;
                mediaPlayer.controls.play();
            }
            else
            {
                mediaPlayer.controls.stop();
            }
            */ 
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            playSound(checkBox1, textBox1);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            playSound(checkBox2, textBox2);
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            playSound(checkBox3, textBox3);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            playSound(checkBox4, textBox4);
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            playSound(checkBox5, textBox5);
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            playSound(checkBox6, textBox6);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // コンポーネントの値を取得
            UpdateHashTable();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
//            this.Close();
            this.Hide();
            textBox1.Text = soundFileName[BEFORE_0_MIN];
            textBox2.Text = soundFileName[BEFORE_1_MIN];
            textBox3.Text = soundFileName[BEFORE_3_MIN];
            textBox4.Text = soundFileName[BEFORE_5_MIN];
            textBox5.Text = soundFileName[BEFORE_10_MIN];
            textBox6.Text = soundFileName[BEFORE_30_MIN];
            checkBox1.Checked = soundEnable[BEFORE_0_MIN];
            checkBox2.Checked = soundEnable[BEFORE_1_MIN];
            checkBox3.Checked = soundEnable[BEFORE_3_MIN];
            checkBox4.Checked = soundEnable[BEFORE_5_MIN];
            checkBox5.Checked = soundEnable[BEFORE_10_MIN];
            checkBox6.Checked = soundEnable[BEFORE_30_MIN];

        }

        private void UpdateHashTable()
        {
            // コンポーネントに設定された値を取得
            soundFileName[BEFORE_0_MIN] = textBox1.Text;
            soundFileName[BEFORE_1_MIN] = textBox2.Text;
            soundFileName[BEFORE_3_MIN] = textBox3.Text;
            soundFileName[BEFORE_5_MIN] = textBox4.Text;
            soundFileName[BEFORE_10_MIN] = textBox5.Text;
            soundFileName[BEFORE_30_MIN] = textBox6.Text;
            soundEnable[BEFORE_0_MIN] = checkBox1.Checked;
            soundEnable[BEFORE_1_MIN] = checkBox2.Checked;
            soundEnable[BEFORE_3_MIN] = checkBox3.Checked;
            soundEnable[BEFORE_5_MIN] = checkBox4.Checked;
            soundEnable[BEFORE_10_MIN] = checkBox5.Checked;
            soundEnable[BEFORE_30_MIN] = checkBox6.Checked;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            trySound(checkBox1, textBox1);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            trySound(checkBox2, textBox2);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            trySound(checkBox3, textBox3);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            trySound(checkBox4, textBox4);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            trySound(checkBox5, textBox5);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            trySound(checkBox6, textBox6);
        }

        private void trySound(CheckBox cb, TextBox tb)
        {
            string fileName = tb.Text;

            if (System.IO.File.Exists(fileName))
            {
                mediaPlayer.URL = fileName;
                mediaPlayer.controls.play();
            }
        }
    }
}
