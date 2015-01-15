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
    public partial class Form1 : Form
    {
        const int TIMER_NUM = 6;
        
        WMPLib.WindowsMediaPlayer mediaPlayer = new WMPLib.WindowsMediaPlayer();

        // 音源設定フォーム
        FormSound f2 = new FormSound();
        // 音源設定フォームとの間で音源ファイル名とOn/Offフラグの受け渡し用変数
        private Dictionary<string, string> soundFileNames = new Dictionary<string, string>();
        private Dictionary<string, bool> soundEnables = new Dictionary<string, bool>();
        // 音源ファイル名とOn/Offフラグ
        // タイマーの割り込みハンドラ内で使用するため、これらの変数へアクセスするときのオーバーヘッドを極力小さくすること。
        // 外部設定ファイルから初期化する場合は直接これらの変数へ設定する。
        private string[] soundFileName = new string[TIMER_NUM];
        private bool[] soundEnable = new bool[TIMER_NUM];

        private enum TIMERS { MIN0 = 0, MIN1, MIN3, MIN5, MIN10, MIN30 };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
//            string filePath = "C:\\Users\\Yuki\\Documents\\Visual Studio 2013\\Projects\\NextLife\\NextLife\\IndexEvent.xml";
            string filePath = "IndexEvent.xml";

            indexEventDataSet.ReadXml(filePath);
            //            indexEventDataSet.ReadXmlSchema();    // XMLスキーマ拡張予定

            // 日付を取得しXMLデータセットから該当テーブルを設定
            dataGridView1.DataSource = indexEventDataSet;
            try
            {
                dataGridView1.DataMember = "index_" + System.DateTime.Today.ToString("yyMMdd");
            }
            catch (Exception)
            {
                dataGridView1.DataMember = "index_nothing";
            }

            // リンクラベルに日付を設定
            linkLabel1.Text = System.DateTime.Today.ToLongDateString() + "の指標";
            // WebブラウザのURLはブログページへ変更予定
            linkLabel1.Links.Add(0, 0, "www.yahoo.co.jp");
            
            // ツールチップのお試し実装。最終的にはブログから取得し、XMLにエントリを追加する。
            for (int counter = 0; counter < (dataGridView1.Rows.Count); counter++)
            {
                if (dataGridView1.Rows[counter].Cells["ie_description"].Value != null)
                {
                    if (dataGridView1.Rows[counter].Cells["ie_description"].Value.ToString().Length != 0)
                    {
                        dataGridView1[2, counter].ToolTipText = "アメリカの雇用統計の発表です。市場予想は+216千人です。行：" + counter.ToString();
                    }
                }
            }

            dataGridView1.CurrentCellDirtyStateChanged += new System.EventHandler(this.dataGridView1_CurrentCellDirtyStateChanged);
            dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);

            initializeSoundSettings();
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedItemIndex = comboBox1.SelectedIndex;

            if (selectedItemIndex == 1)
            {
                DialogResult result;

                result = f2.ShowDialog(this);
                if (result == DialogResult.OK)
                {
                    updateSoundSettings();
                    // 音源設定を外部ファイルへ保存
                    Properties.Settings.Default.Save();
                }
                else
                {
                    // 音源設定をリロード
//                    Properties.Settings.Default.Reload();
                }                
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {

            linkLabel1.Text = monthCalendar1.SelectionStart.ToLongDateString() + "の指標";
            try
            {
                dataGridView1.DataMember = "index_" + monthCalendar1.SelectionStart.ToString("yyMMdd");
            }
            catch (Exception)
            {
                dataGridView1.DataMember = "index_nothing";
            }
        }

        //CurrentCellDirtyStateChangedイベントハンドラ
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCellAddress.X == 0 &&
                dataGridView1.IsCurrentCellDirty)
            {
                //チェックボックスを変更した時点でコミットする
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        //CellValueChangedイベントハンドラ
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //列のインデックスを確認する
            if (e.ColumnIndex == 0)
            {
                if((string)dataGridView1[0, e.RowIndex].Value == "true")
                {
                    string selectedEventDayTime;

                    // イベントの時刻を取得
                    selectedEventDayTime = this.convertTimeString((string)dataGridView1[1, e.RowIndex].Value);
                    // イベントの時刻が正しく取得できない場合はチェックボックスをOFFに戻す(未実装)
                    if (selectedEventDayTime == "")
                    {
                        dataGridView1[0, e.RowIndex].Value = "false"; 
                        return;
                    }

                    //// 以下はデバッグ用実装
/*                    
                    // カレンダーから表示中の日付を取得
                    string selectedYearMonth = monthCalendar1.SelectionStart.ToString("yyyy/MM/");
                    // DataTimeオブジェクトへ変換
                    DateTime eventTime = DateTime.ParseExact(selectedYearMonth + selectedEventDayTime, "yyyy/MM/dd HH:mm:ss", null);
                    // 現在時刻との差分計算
                    System.TimeSpan diff1 = eventTime.Subtract(DateTime.Now);

//                    if (monthCalendar1.SelectionStart.Day == DateTime.Now.Day)
//                    {
                        MessageBox.Show(Math.Round(diff1.TotalMinutes) + "分後に発表です。");
//                    }
*/                    
                    //// ここまで
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            for (int counter = 0; counter < (dataGridView1.Rows.Count); counter++)
            {
                if ((string)dataGridView1[0, counter].Value == "true")
                {
                    string selectedEventDayTime;

                    // イベントの時刻を取得
                    selectedEventDayTime = this.convertTimeString((string)dataGridView1[1, counter].Value);
                    // イベントの時刻が正しく取得できない場合は何もしない
                    if (selectedEventDayTime == "") { continue; }
                    // カレンダーから表示中の日付を取得
                    string selectedYearMonth = monthCalendar1.SelectionStart.ToString( "yyyy/MM/" );
                    // DataTimeオブジェクトへ変換
                    DateTime eventTime = DateTime.ParseExact(selectedYearMonth + selectedEventDayTime, "yyyy/MM/dd HH:mm:ss", null);
                    // 現在時刻との差分計算
                    System.TimeSpan diff1 = eventTime.Subtract(DateTime.Now);

                    double min = Math.Round(diff1.TotalMinutes);
                    double sec = Math.Round(diff1.TotalSeconds);
                    // 差分がマイナスの場合は過去イベントのため無視
                    if (sec < 0) 
                    {
                        continue;
                    }
                    else if (soundEnable[(int)TIMERS.MIN0] && sec == 3) // 3秒を切ったら鳴らす(0分前)
                    {
                        mediaPlayer.URL = soundFileName[(int)TIMERS.MIN0];
                        mediaPlayer.controls.play();
//                        MessageBox.Show((string)dataGridView1[2, counter].Value + " 発表 " + min.ToString() + " 分前");
                    }
                    else if (soundEnable[(int)TIMERS.MIN1] && sec == 63) // 63秒を切ったら鳴らす(1分前)
                    {
                        mediaPlayer.URL = soundFileName[(int)TIMERS.MIN1];
                        mediaPlayer.controls.play();
//                        MessageBox.Show((string)dataGridView1[2, counter].Value + " 発表 " + min.ToString() + " 分前");
                    }
                    else if (soundEnable[(int)TIMERS.MIN3] && sec == 183) // 183秒を切ったら鳴らす(3分前)
                    {
                        mediaPlayer.URL = soundFileName[(int)TIMERS.MIN3];
                        mediaPlayer.controls.play();
//                        MessageBox.Show((string)dataGridView1[2, counter].Value + " 発表 " + min.ToString() + " 分前");
                    }
                    else if (soundEnable[(int)TIMERS.MIN5] && sec == 303) // 303秒を切ったら鳴らす(5分前)
                    {
                        mediaPlayer.URL = soundFileName[(int)TIMERS.MIN5];
                        mediaPlayer.controls.play();
//                        MessageBox.Show((string)dataGridView1[2, counter].Value + " 発表 " + min.ToString() + " 分前");
                    }
                    else if (soundEnable[(int)TIMERS.MIN10] && sec == 603) // 603秒を切ったら鳴らす(10分前)
                    {
                        mediaPlayer.URL = soundFileName[(int)TIMERS.MIN10];
                        mediaPlayer.controls.play();
//                        MessageBox.Show((string)dataGridView1[2, counter].Value + " 発表 " + min.ToString() + " 分前");
                    }
                    else if (soundEnable[(int)TIMERS.MIN30] && sec == 1803) // 1803秒を切ったら鳴らす(30分前)
                    {
                        mediaPlayer.URL = soundFileName[(int)TIMERS.MIN30];
                        mediaPlayer.controls.play();
//                        MessageBox.Show((string)dataGridView1[2, counter].Value + " 発表 " + min.ToString() + " 分前");
                    }
                }
            }
        }

        private string convertTimeString(string timeStr)
        {
            char[] delimiterChars = { ':' };
            string timeString = "";
            bool isInt;
            int day;
            int hour;
            int min;

            // カレンダーから表示中の日付を取得
            string selectedDay = monthCalendar1.SelectionStart.ToString("dd");
            isInt = Int32.TryParse(selectedDay, out day);
            if (!isInt) { return timeString; }

            if (0 <= timeStr.IndexOf(":"))
            {
                string[] timeWords = timeStr.Split(delimiterChars);

                isInt = Int32.TryParse(timeWords[0], out hour);
                if (isInt && (0 <= hour && hour <= 23))
                {
                    if (0 <= hour && hour <= 4)
                    {
                        day = day + 1;
                    }

                    isInt = Int32.TryParse(timeWords[1], out min);
                    if (isInt && (0 <= min && min <= 59))
                    {

                        timeString = String.Format("{0:D2} {1:D2}:{2:D2}:00", day, hour, min);
                    }
                } 
            }

            // 変換成功： HH:mm:00 フォーマットの文字列
            // 変換失敗： 空文字列
            return timeString;
        }

        private void initializeSoundSettings()
        {
            soundEnable[(int)TIMERS.MIN0] = System.Convert.ToBoolean(Properties.Settings.Default["MIN0_ENABLE"]);
            soundEnable[(int)TIMERS.MIN1] = System.Convert.ToBoolean(Properties.Settings.Default["MIN1_ENABLE"]);
            soundEnable[(int)TIMERS.MIN3] = System.Convert.ToBoolean(Properties.Settings.Default["MIN3_ENABLE"]);
            soundEnable[(int)TIMERS.MIN5] = System.Convert.ToBoolean(Properties.Settings.Default["MIN5_ENABLE"]);
            soundEnable[(int)TIMERS.MIN10] = System.Convert.ToBoolean(Properties.Settings.Default["MIN10_ENABLE"]);
            soundEnable[(int)TIMERS.MIN30] = System.Convert.ToBoolean(Properties.Settings.Default["MIN30_ENABLE"]);

            soundFileName[(int)TIMERS.MIN0] = System.Convert.ToString(Properties.Settings.Default["MIN0_FILE"]);
            soundFileName[(int)TIMERS.MIN1] = System.Convert.ToString(Properties.Settings.Default["MIN1_FILE"]);
            soundFileName[(int)TIMERS.MIN3] = System.Convert.ToString(Properties.Settings.Default["MIN3_FILE"]);
            soundFileName[(int)TIMERS.MIN5] = System.Convert.ToString(Properties.Settings.Default["MIN5_FILE"]);
            soundFileName[(int)TIMERS.MIN10] = System.Convert.ToString(Properties.Settings.Default["MIN10_FILE"]);
            soundFileName[(int)TIMERS.MIN30] = System.Convert.ToString(Properties.Settings.Default["MIN30_FILE"]);

            /*
            this.soundEnables.Add(FormSound.BEFORE_0_MIN, soundEnable[(int)TIMERS.MIN0]);
            this.soundEnables.Add(FormSound.BEFORE_1_MIN, soundEnable[(int)TIMERS.MIN1]);
            this.soundEnables.Add(FormSound.BEFORE_3_MIN, soundEnable[(int)TIMERS.MIN3]);
            this.soundEnables.Add(FormSound.BEFORE_5_MIN, soundEnable[(int)TIMERS.MIN5]);
            this.soundEnables.Add(FormSound.BEFORE_10_MIN, soundEnable[(int)TIMERS.MIN10]);
            this.soundEnables.Add(FormSound.BEFORE_30_MIN, soundEnable[(int)TIMERS.MIN30]);

            this.soundFileNames.Add(FormSound.BEFORE_0_MIN, soundFileName[(int)TIMERS.MIN0]);
            this.soundFileNames.Add(FormSound.BEFORE_1_MIN, soundFileName[(int)TIMERS.MIN1]);
            this.soundFileNames.Add(FormSound.BEFORE_3_MIN, soundFileName[(int)TIMERS.MIN3]);
            this.soundFileNames.Add(FormSound.BEFORE_5_MIN, soundFileName[(int)TIMERS.MIN5]);
            this.soundFileNames.Add(FormSound.BEFORE_10_MIN, soundFileName[(int)TIMERS.MIN10]);
            this.soundFileNames.Add(FormSound.BEFORE_30_MIN, soundFileName[(int)TIMERS.MIN30]);

            // 音源設定フォームのデータベースの初期化
            // 外部設定ファイルから初期化する場合に必要
            f2.setSoundFileNames(this.soundFileNames);
            f2.setSoundEnables(this.soundEnables);
            */
        }


        private void updateSoundSettings()
        {
            this.soundFileNames = f2.getSoundFileNames();
            this.soundEnables = f2.getSoundEnables();

            soundEnable[(int)TIMERS.MIN0] = this.soundEnables[FormSound.BEFORE_0_MIN];
            soundEnable[(int)TIMERS.MIN1] = this.soundEnables[FormSound.BEFORE_1_MIN];
            soundEnable[(int)TIMERS.MIN3] = this.soundEnables[FormSound.BEFORE_3_MIN];
            soundEnable[(int)TIMERS.MIN5] = this.soundEnables[FormSound.BEFORE_5_MIN];
            soundEnable[(int)TIMERS.MIN10] = this.soundEnables[FormSound.BEFORE_10_MIN];
            soundEnable[(int)TIMERS.MIN30] = this.soundEnables[FormSound.BEFORE_30_MIN];

            soundFileName[(int)TIMERS.MIN0] = this.soundFileNames[FormSound.BEFORE_0_MIN];
            soundFileName[(int)TIMERS.MIN1] = this.soundFileNames[FormSound.BEFORE_1_MIN];
            soundFileName[(int)TIMERS.MIN3] = this.soundFileNames[FormSound.BEFORE_3_MIN];
            soundFileName[(int)TIMERS.MIN5] = this.soundFileNames[FormSound.BEFORE_5_MIN];
            soundFileName[(int)TIMERS.MIN10] = this.soundFileNames[FormSound.BEFORE_10_MIN];
            soundFileName[(int)TIMERS.MIN30] = this.soundFileNames[FormSound.BEFORE_30_MIN];

        }
    }
}
