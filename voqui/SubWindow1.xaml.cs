using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace voqui3
{
    /// <summary>
    /// SubWindow1.xaml の相互作用ロジック
    /// </summary>
    public partial class SubWindow1 : Window
    {
        // 実行dirの読込        
        public static string s_EPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static string s_EDir = System.IO.Path.GetDirectoryName(s_EPath);

        public static string s_pfile_param1 = s_EDir + "\\VOQ3P1.txt";   // 設定ファイル(年度,ライセンスキー,最終連番)
        public static string s_pfile_log    = s_EDir + "\\VOQ3L1.txt";   // ログファイル
        public static string s_pfile_mainp  = s_EDir + "\\VOQ3AA.exe";   // メインプログラム

        readonly Encoding EncJIS = Encoding.GetEncoding("Shift-JIS");

        // プリグラムのヴァージョン
        static System.Diagnostics.FileVersionInfo oFVI =
            System.Diagnostics.FileVersionInfo.GetVersionInfo(
            System.Reflection.Assembly.GetExecutingAssembly().Location);
        static readonly string s_pv = oFVI.FileVersion;

        public int i_nendo = 0;
        public int i_endjno = 1000;
        public string s_hyoud1 = "";
        public string s_hyoud2 = "";

        public string s_log = "";
        public string s_check = "";

        // ------------------------------------------------------------------------------------

        public SubWindow1()
        {
            InitializeComponent();
        }

        // ------------------------------------------------------------------------------------

        //簡略化したﾒｯｾｰｼﾞﾎﾞｯｸｽ Error
        private void BoxError(string m1)
        {
            string w_mes = m1 + "\r\n修整しやり直しください。";
            MessageBox.Show(w_mes,
                "voqui3 Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        //簡略化したﾒｯｾｰｼﾞﾎﾞｯｸｽ Check
        private void BoxCheck(string m1)
        {    
            string w_mes = m1 + "\r\n問題があればやり直しください。";
            MessageBox.Show(w_mes,
                "voqui3 Check",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        //簡略化したﾒｯｾｰｼﾞﾎﾞｯｸｽ OK CANCEL
        private bool BoxDoch(string mes)
        {
            string w_mes;
            bool b_OK = true;

            w_mes = mes + "\r\n";
            w_mes += "この確認に問題なければ「OK」。\r\n";
            w_mes += "以外の場合「ｷｬﾝｾﾙ」しまず。";

            MessageBoxResult DR = MessageBox.Show(w_mes,
                "voqui3 Question",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);
            if (DR == MessageBoxResult.Cancel)
            {
                b_OK = false;
            }
            return b_OK;
        }

        // ----------------------------------------------------------------------------------------------

        // ﾊﾟﾗﾒｰﾀ1の読込
        public bool F01_get_param1_file()
        {
            string s_rec;

            try
            {
                // 画面表示式設定

                s_log += "\r\n f01 01 s get_param_file";
                TbKekka.Text = "";
                TbVersion.Text = s_pv;

                // 基本のパラメータ情報の読み込みとリスト作成

                using (StreamReader SrParam = new StreamReader(s_pfile_param1, EncJIS))
                {
                    //
                    while (SrParam.Peek() >= 0)
                    {
                        s_rec = SrParam.ReadLine() + "==";
                        s_check = s_rec;
                        string[] a_rec = s_rec.Split('=');
                        string s_item0 = a_rec[0].Trim();
                        string s_item1 = a_rec[1].Trim();
                        //
                        if (s_item0 == "NENDO")
                        {
                            i_nendo = int.Parse(s_item1);
                            s_log += "\r\n f01 11 NENDO " + s_item1;
                            TbNendo.Text = s_item1;
                        }
                        else if (s_item0 == "HYUD1")
                        {
                            s_hyoud1 = s_item1;
                            s_log += "\r\n f01 13 HYOUD1 " + s_item1;
                            tbHyou1.Text = s_item1;
                        }
                        else if (s_item0 == "HYUD2")
                        {
                            s_hyoud2 = s_item1;
                            s_log += "\r\n f01 14 HYUD2 " + s_item1;
                            tbHyou2.Text = s_item1;
                        }
                        else if (s_item0 == "ENDJNO")
                        {
                            i_endjno = int.Parse(s_item1);
                            s_log += "\r\n f01 15 ENDJNO " + s_item1;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                 BoxError("f01 99 e get_param_file \r\n" + ex.Message + s_check);
                s_log += "\r\n f01 99 error " + s_check;
                return false;
            }

        }

        // ﾊﾟﾗﾒｰﾀのファイル保存
        public bool F02_put_param1_file()
        {            
            string s_job = "";
            try
            {
                s_job = "f02 03 表題 ";
                // 表題格納
                s_hyoud1 = tbHyou1.Text;
                s_hyoud2 = tbHyou2.Text;

                // ファイル格納
                s_job = "f02 11 ファイル格納 ";
                s_log += "\r\n f02 01 s put_param1_file ";
                using (StreamWriter SwParam = new StreamWriter(s_pfile_param1, false, EncJIS))
                {
                    string s_rec = "";

                    // 年度格納
                    s_rec = "NENDO=" + i_nendo.ToString();
                    SwParam.WriteLine(s_rec);

                    // 表題格納
                    s_rec = "HYUD1=" + s_hyoud1;
                    SwParam.WriteLine(s_rec);
                    s_rec = "HYUD2=" + s_hyoud2;
                    SwParam.WriteLine(s_rec);

                    // 最終処理項目番号の格納
                    s_rec = "ENDJNO=" + i_endjno.ToString();
                    SwParam.WriteLine(s_rec);
                    SwParam.Close();
                }
                string s_dt = DateTime.Now.ToString();
                TbKekka.Text = "完了 " + s_dt;

                return true;
            }
            catch (Exception ex)
            {
                //
                BoxError(s_job + "\r\n" + ex.Message);
                s_log += "\r\n f02 98 " + ex.Message;
                s_log += "\r\n f02 99 error ";
                return false;
            }
        }

        // ログのファイルアウト
        public bool F09_put_log_file()
        {
            try
            {
                using (StreamWriter SwParam = new StreamWriter(s_pfile_log, true, EncJIS))
                {
                    SwParam.Write(s_log);
                    SwParam.Close();
                    s_log = "";
                }
                return true;
            }
            catch (Exception ex)
            {
                BoxError("f09 99 e log out \r\n" + ex.Message);
                return false;
            }
        }

        // -------------------------------------------------------------------------------------------------

        // 設定の登録
        private void ButtonSet_Click(object sender, RoutedEventArgs e)
        {
            bool b_value =true;
            bool b_r = int.TryParse(TbNendo.Text, out i_nendo);
            if (!b_r)
            {
                BoxError("年度チェック\r\n  数値エラー");
                return;
            }            
            if (i_nendo < 2010 || i_nendo > 2040)
            {
                BoxError("年度チェック\r\n  範囲エラー");
                return;
            }

            if (CBoxNew.IsChecked == true)
            {
                if (BoxDoch("年頭での初期化の設定をします。よろしいですか？"))
                {                    
                    s_log += "\r\n ButtonSet_Click 年度新規 ";

                    i_endjno = 1000;
                    s_hyoud2 = i_nendo.ToString() + "(1/1-12/31)";
                    tbHyou2.Text = s_hyoud2;

                    b_value = F02_put_param1_file();
                }
                else
                {
                    s_log += "\r\n ButtonSet_Click 年度新規をキャンセル ";
                }
            } else
            {
                s_log += "\r\n ButtonSet_Click 年度新規 ";
                b_value = F02_put_param1_file();
            }
            if (!b_value)
            {
                s_log += "\r\n ButtonSet_Click error ";
            }
            F09_put_log_file();
        }

        // ウィンドロード
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool b_value;

            b_value = F01_get_param1_file();

            F09_put_log_file();

            if (!b_value) return;

            if (i_nendo == 0)
            {
                CBoxNew.IsChecked = true;
            }

        }

        // Closed
        private void Window_Closed(object sender, EventArgs e)
        {
            //
        }



        // 「戻る」ボタン
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(s_pfile_mainp);

            this.Close();
        }
    }
}
