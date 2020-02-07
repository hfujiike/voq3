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
        static string s_EPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string s_EDir = System.IO.Path.GetDirectoryName(s_EPath);
        
        static string s_pfile_param1 = s_EDir + "\\VOQ3P1.txt";   // 設定ファイル(年度,ライセンスキー,最終連番)
        static string s_pfile_log    = s_EDir + "\\VOQ3L1.txt";   // ログファイル
        static string s_pfile_mainp  = s_EDir + "\\VOQ3AA.exe";   // メインプログラム

        Encoding EncJIS = Encoding.GetEncoding("Shift-JIS");

        // プリグラムのヴァージョン
        static System.Diagnostics.FileVersionInfo oFVI =
            System.Diagnostics.FileVersionInfo.GetVersionInfo(
            System.Reflection.Assembly.GetExecutingAssembly().Location);
        static string s_pv = oFVI.FileVersion;

        static int i_nendo = 0;
        static string s_nendonew = "N";
        static int i_endjno = 1000;
        static string s_hyoud1 = "";
        static string s_hyoud2 = "";

        static string s_log = "";
        static string s_check = "";

        // ------------------------------------------------------------------------------------

        public SubWindow1()
        {
            InitializeComponent();
        }

        // ------------------------------------------------------------------------------------

        private void BoxError(string m1)
        {
            //簡略化したﾒｯｾｰｼﾞﾎﾞｯｸｽ Error

            string w_mes = m1 + "\r\n修整しやり直しください。";
            MessageBox.Show(w_mes,
                "voqui3 Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        private void BoxCheck(string m1)
        {
            //簡略化したﾒｯｾｰｼﾞﾎﾞｯｸｽ Check

            string w_mes = m1 + "\r\n問題があればやり直しください。";
            MessageBox.Show(w_mes,
                "voqui3 Check",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        // ----------------------------------------------------------------------------------------------

        public bool f01_get_param1_file()
        {
            // ﾊﾟﾗﾒｰﾀ1の読込

            string s_rec = "";

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


        public bool f02_put_param1_file()
        {
            // ﾊﾟﾗﾒｰﾀのファイル保存

            string s_rec = "";
            string s_job = "";

            try
            {
                s_job = "f02 01 年度チェック ";
                i_nendo = int.Parse(TbNendo.Text);
                if (i_nendo < 2010 && i_nendo > 2040)
                {
                    BoxError(s_job + "\r\n  範囲エラー");
                    return false;
                }

                s_job = "f02 03 表題 ";
                // 表題格納
                s_hyoud1 = tbHyou1.Text;
                s_hyoud2 = tbHyou2.Text;

                // ファイル格納
                s_job = "f02 11 ファイル格納 ";
                s_log += "\r\n f02 01 s put_param1_file ";
                using (StreamWriter SwParam = new StreamWriter(s_pfile_param1, false, EncJIS))
                {
                    // 年度格納
                    if (i_nendo > 2010)
                    {
                        s_rec = "NENDO=" + i_nendo.ToString();
                        SwParam.WriteLine(s_rec);
                    }
                    if (s_nendonew == "Y")
                    {
                        s_rec = "NENDONEW=" + s_nendonew;
                        SwParam.WriteLine(s_rec);
                        BoxCheck("年頭での初期化の設定もしています。よろしいですか？");
                    }

                    // 表題格納
                    s_rec = "HYUD1=" + s_hyoud1;
                    SwParam.WriteLine(s_rec);
                    s_rec = "HYUD2=" + s_hyoud2;
                    SwParam.WriteLine(s_rec);

                    // 最終処理項目番号の格納
                    if (i_endjno > 1000)
                    {
                        s_rec = "ENDJNO=" + i_endjno.ToString();
                        SwParam.WriteLine(s_rec);
                    }
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


        public bool f09_put_log_file()
        {
            // ログのファイルアウト

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


        private void ButtonSet_Click(object sender, RoutedEventArgs e)
        {
            // 設定の登録
            bool b_value = true;

            b_value = f02_put_param1_file();
            
            if (!b_value) return;

            f09_put_log_file();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ウィンドロード
            bool b_value = true;

            b_value = f01_get_param1_file();
            f09_put_log_file();
            if (!b_value) return;

            f09_put_log_file();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

            //
        }

        private void CBoxNew_Checked(object sender, RoutedEventArgs e)
        {
            // 年頭初期化
            s_nendonew = "Y";            
        }

        private void CBoxNew_Unchecked(object sender, RoutedEventArgs e)
        {
            // 年頭初期化
            s_nendonew = "N";
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            // 「戻る」ボタン

            System.Diagnostics.Process p = System.Diagnostics.Process.Start(s_pfile_mainp);

            this.Close();
        }
    }
}
