using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace voqui3
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // 実行パス ---------------------------
        static readonly string s_EPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static readonly string s_EDir = System.IO.Path.GetDirectoryName(s_EPath);

        // ファイル関係 ---------------------------
        public string s_pfile_param1 = s_EDir + "\\VOQ3P1.txt";
        public string s_pfile_param2 = s_EDir + "\\VOQ3P2.txt";
        public string s_pfile_log    = s_EDir + "\\VOQ3L1.txt";
        public string s_pfile_jounal = s_EDir + "\\VOQ3D1.csv";
        public string s_pfile_Ledger = s_EDir + "\\VOQ3D2.csv";
        public string s_pfile_Shisan = s_EDir + "\\VOQ3D3.csv";
        public string s_pfile_output = s_EDir + "\\VOQ3D0.xlsx";

        Encoding EncJIS = Encoding.GetEncoding("Shift-JIS");

        // その他変数  param -------------------------------
        public int i_nendo = 0;
        public int i_endjno = 0;
        public string s_hyoud1 = "";
        public string s_hyoud2 = "";

        // その他変数 --------------------------------------
        public int i_sdrcode = 0;
        public int i_scrcode = 0;
        public string s_sdrname = "";
        public string s_scrname = "";
        public int i_jamount = 0;
        public string s_jamount = "";
        public string s_jxplanation = "";

        // LIST ----------------------------
        // 作業リスト　コンボボックスへバインド
        public List<JounalJS> ListJounalJS = new List<JounalJS>();
        // 選択combobox用のリスト　借方と貸方
        public List<SelectDr> ListSelectDr = new List<SelectDr>();
        public List<SelectCr> ListSelectCr = new List<SelectCr>();
        // 科目リスト　参照のみ
        public List<BSubject> ListBSubject = new List<BSubject>();
        // 仕訳データ
        public List<JounalData> ListJounalData = new List<JounalData>();
        // 元帳データのリスト
        public List<GLedger> ListGLeger = new List<GLedger>();
        // 試算表データのリスト
        public List<ShisanHyou> ListShisanhyou = new List<ShisanHyou>();

        // ログ用コンスタント ----------------
        public string s_log = "\r\n\r\n";
        public string s_check = "";
        public string s_w1 = "";
        public string s_w2 = "";
        public string s_kanma = ",";

        // ----------------------------------------------------------------------

        // 初回処理
        public bool F00_saisho()
        {
            try
            {

                // ログファイル初期化
                DateTime d_now = DateTime.Now;
                using (StreamWriter SwParam = new StreamWriter(s_pfile_log, false, EncJIS))
                {
                    SwParam.Write("----- " + d_now.ToString() + " -------");
                    SwParam.Close();
                    s_log = "";
                }

                // 処理日の表示
                DateTime d_ope = DateTime.Now;
                TbOpeDate.Text = d_ope.ToShortDateString();

                // 表示のクリア
                TbZenDelBi.Text = "";
                TbZenDelNo.Text = "";
                TbZenJAmount.Text = "";
                TbZenJDate.Text = "";
                TbZenJExp.Text = "";

                // 初期の非活性
                ButtonSORT.IsEnabled = false;

                // テスト用
                ButtonTest.Visibility = System.Windows.Visibility.Hidden;


                return true;

            }
            catch (Exception ex)
            {
                BoxError("f00 99 e saisho \r\n" + ex.Message);
                s_log += "\r\n f00 99 error ";
                return false;
            }
        }

        // ﾊﾟﾗﾒｰﾀ1の読込
        public bool F01_get_param1_file()
        {
            string s_rec = "";

            try
            {
                // 基本のパラメータ情報の読み込みとリスト作成
                s_log += "\r\n f01 01 s get_param_file1";
                using (StreamReader SrParam = new StreamReader(s_pfile_param1, EncJIS))
                {
                    while (SrParam.Peek() >= 0)
                    {
                        s_rec = SrParam.ReadLine() + "==";
                        s_check = s_rec;
                        string[] a_rec = s_rec.Split('=');
                        string s_item0 = a_rec[0].Trim();
                        string s_item1 = a_rec[1].Trim();

                        if (s_item0 == "NENDO")
                        {
                            bool b_r = int.TryParse(s_item1, out i_nendo);
                            s_log += "\r\n f01 11 NENDO " + s_item1 + b_r.ToString();
                        }
                        else if (s_item0 == "HYUD1")
                        {
                            s_hyoud1 = s_item1;
                            s_log += "\r\n f01 14 HYUD1 " + s_item1;
                        }
                        else if (s_item0 == "HYUD2")
                        {
                            s_hyoud2 = s_item1;
                            s_log += "\r\n f01 15 HYUD2 " + s_item1;
                        }
                        else if (s_item0 == "ENDJNO")
                        {
                            bool b_r =  int.TryParse(s_item1, out i_endjno);
                            s_log += "\r\n f01 16 ENDJNO " + s_item1 + b_r.ToString();
                        }                        
                    }

                    SrParam.Close();
                }

                return true;
            }
            catch (Exception ex)
            {
                //
                BoxError("f01 99 e get_param_file \r\n" + ex.Message + s_check);
                s_log += "\r\n f01 99 error " + s_check;
                return false;
            }

        }

        // ﾊﾟﾗﾒｰﾀ1のファイル保存
        public bool F02_put_param1_file()
        {
            string s_rec = "";

            try
            {
                // ファイル格納
                s_log += "\r\n f02 01 s put_param1_file ";
                using (StreamWriter SwParam = new StreamWriter(s_pfile_param1, false, EncJIS))
                {
                    // 年度
                    s_rec = "NENDO=" + i_nendo.ToString();
                    SwParam.WriteLine(s_rec);

                    // 最終連番
                    if (i_endjno > 999)
                    {
                        s_rec = "ENDJNO=" + i_endjno.ToString();
                        SwParam.WriteLine(s_rec);
                    }
                    {
                        s_rec = "HYUD1=" + s_hyoud1;
                        SwParam.WriteLine(s_rec);
                    }
                    {
                        s_rec = "HYUD2=" + s_hyoud2;
                        SwParam.WriteLine(s_rec);
                    }
                    SwParam.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                //
                BoxError("f02 99 e put_param1_file \r\n" + ex.Message);
                s_log += "\r\n f02 98 " + ex.Message;
                s_log += "\r\n f02 99 error ";
                return false;
            }
        }

        // ﾊﾟﾗﾒｰﾀ２の読込
        public bool F03_get_param2_file()
        {
            string s_rec;

            ListJounalJS.Clear();
            ListBSubject.Clear();

            try
            {
                //
                s_log += "\r\n f03 01 s get_param2_file";
                using (StreamReader SrParam = new StreamReader(s_pfile_param2, EncJIS))
                {
                    //
                    while (SrParam.Peek() >= 0)
                    {
                        //
                        s_rec = SrParam.ReadLine() + "==";
                        string[] a_rec = s_rec.Split('=');
                        string s_item0 = a_rec[0].Trim();
                        string s_item1 = a_rec[1].Trim();
                        //
                        string[] a_item = s_item1.Split(',');
                        //
                        if (s_item0 == "JOUNALJS")
                        {
                            // 仕訳作業表取り込み
                            string s_c0 = a_item[0];
                            string s_c1 = a_item[1];
                            string s_c2 = a_item[2];
                            string s_c3 = a_item[3];
                            string s_c4 = a_item[4];
                            JounalJS JO = new JounalJS
                            {
                                JOpeName = s_c0,
                                DrGroup = s_c1,
                                CrGroup = s_c2,
                                DrGMes = s_c3,
                                CrGMes = s_c4
                            };
                            ListJounalJS.Add(JO);
                            s_log += s_c0;
                        }
                        else if (s_item0 == "BSUBJECT")
                        {
                            // 科目表取り込み
                            string s_c0 = a_item[0];
                            string s_c1 = a_item[1];
                            int i_c0 = int.Parse(s_c0);
                            BSubject BS = new BSubject
                            {
                                SCode = i_c0,
                                Sname = s_c1
                            };
                            ListBSubject.Add(BS);
                            s_log += s_c1;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                //
                BoxError("f03 01 s get_param2_file \r\n" + ex.Message + s_check);
                s_log += "\r\n f03 99 error " + s_check;
                return false;
            }

        }

        // 作業選択  コンボボックス作業の選択
        public bool F04_seljob()
        {
            string s_pos = "";
            try
            {
                s_pos = "f04 01 s sagyou_handan";
                s_log += "\r\n" + s_pos;

                // コンボへ処理選択を設定
                this.Combo_job.ItemsSource = ListJounalJS;
                this.Combo_job.Items.Refresh();
                // コンボかりをクリア
                ListSelectDr.Clear();
                this.CBox_kari.ItemsSource = ListSelectDr;
                this.CBox_kari.Items.Refresh();
                this.TbDr.Text = "";
                // コンボかしをクリア
                ListSelectCr.Clear();
                this.CBox_kashi.ItemsSource = ListSelectCr;
                this.CBox_kashi.Items.Refresh();
                this.TbCr.Text = "";

                return true;
            }
            catch (Exception ex)
            {
                BoxError(s_pos + "\r\n" + ex.Message);
                return false;
            }
        }

        // 作業判断　通常用　コンボボックス作業の設定
        public bool F05_sagyou_handan(int Sel)
        {
            string s_pos = "";
            try
            {
                s_pos = "F05 01 s sagyou_handan ";
                s_log += "\r\n" + s_pos;

                int i_gs = 0;
                int i_ge = 0;
                JounalJS JO = new JounalJS();
                JO = ListJounalJS[Sel];
                TbDr.Text = JO.DrGMes;
                TbCr.Text = JO.CrGMes;

                s_pos = "F05 02 s karikata combo";
                s_log += "\r\n" + s_pos;

                string s_nen = JO.JOpeName.Substring(0, 2);
                if (s_nen == "年頭")
                {
                    TbJDate.Text = "1/1";
                }
                else if (s_nen == "通常")
                {
                    TbJDate.Text = DateTime.Now.ToString("MM/dd");
                } else if (s_nen == "年末")
                {
                    TbJDate.Text = "12/31";
                }

                string s_dg = JO.DrGroup;                
                if ( s_dg.Length == 2 )
                {
                    i_gs = int.Parse(s_dg + "0");
                    i_ge = int.Parse(s_dg + "9");
                }
                else
                {
                    i_gs = int.Parse(s_dg.Substring(0, 2) + "0");
                    i_ge = int.Parse(s_dg.Substring(2, 2) + "9");
                }                

                ListSelectDr.Clear();
                foreach (BSubject BS in ListBSubject)
                {
                    if (i_gs < BS.SCode && BS.SCode <= i_ge )
                    {
                        SelectDr SD = new SelectDr
                        {
                            SDrCode = BS.SCode,
                            SDrName = BS.Sname
                        };
                        ListSelectDr.Add(SD);
                    }
                }

                this.CBox_kari.ItemsSource = ListSelectDr;
                this.CBox_kari.Items.Refresh();

                s_pos = "F05 03 s kashikata combo";
                s_log += "\r\n" + s_pos;

                string s_cg = JO.CrGroup;
                if (s_cg.Length == 2)
                {
                    i_gs = int.Parse(s_cg + "0");
                    i_ge = int.Parse(s_cg + "9");
                }
                else
                {
                    i_gs = int.Parse(s_cg.Substring(0, 2) + "0");
                    i_ge = int.Parse(s_cg.Substring(2, 2) + "9");
                }
               
                ListSelectCr.Clear();
                foreach (BSubject BS in ListBSubject)
                {
                    if (i_gs < BS.SCode && BS.SCode <= i_ge)
                    {
                        SelectCr SC = new SelectCr
                        {
                            SCrCode = BS.SCode,
                            SCrName = BS.Sname
                        };
                        ListSelectCr.Add(SC);                    
                    }
                }

                this.CBox_kashi.ItemsSource = ListSelectCr;
                this.CBox_kashi.Items.Refresh();

                return true;
            }
            catch (Exception ex)
            {
                BoxError(s_pos + "\r\n" + ex.Message);
                return false;
            }
        }

        // コンボから借方情報セット
        public bool F06_karikata_sel()
        {
            int i_index = CBox_kari.SelectedIndex;
            
            if (i_index >= 0)
            {
                i_sdrcode = ListSelectDr[i_index].SDrCode;
                s_sdrname = ListSelectDr[i_index].SDrName;
                return true;
            }
            else
            {
                i_sdrcode = 0;
                s_sdrname = "";
                return false;
            }
        }

        // コンボから貸方情報セット
        public bool F07_kashikata_sel()
        {
            int i_index = CBox_kashi.SelectedIndex;
            if (i_index >= 0)
            {
                i_scrcode = ListSelectCr[i_index].SCrCode;
                s_scrname = ListSelectCr[i_index].SCrName;
                return true;
            }
            else
            {
                i_sdrcode = 0;
                s_sdrname = "";
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

        // 仕訳データの読込
        public bool F11_get_JounalData()
        {
            string s_rec = "";
            int i_amount = 0;

            ListJounalData.Clear();

            try
            {
                s_log += "\r\n f11 12 JounalData bind";
                if (i_endjno > 1000)
                {
                    // 仕分けがある場合ListJounalDataへadd
                    using (StreamReader SrJounal = new StreamReader(s_pfile_jounal, EncJIS))
                    {
                        while (SrJounal.Peek() >= 0)
                        {
                            s_rec = SrJounal.ReadLine() + ",,";
                            s_check = s_rec;
                            string[] a_item = s_rec.Split(',');
                            // 仕訳データ取り込み
                            string s_c0 = a_item[0].Trim();
                            string s_c1 = a_item[1].Trim();
                            string s_c2 = a_item[2].Trim();
                            string s_c3 = a_item[3].Trim();
                            string s_c4 = a_item[4].Trim();
                            string s_c5 = a_item[5].Trim();
                            string s_c6 = a_item[6].Trim();
                            i_amount = int.Parse(s_c5);

                            JounalData JD = new JounalData();
                            JD.JDateNo = s_c0;
                            JD.DrCode = s_c1;
                            JD.DrName = s_c2;
                            JD.CrCode = s_c3;
                            JD.CrName = s_c4;
                            JD.JAmount = i_amount.ToString("#,0");
                            JD.JExplanation = s_c6;
                            ListJounalData.Add(JD);
                        }
                    }
                }

                LV_shiwake.ItemsSource = ListJounalData;
                
                return true;
            }
            catch (Exception ex)
            {
                BoxError("f11 99 e get_JounalData \r\n" + ex.Message);
                s_log += "\r\n f11 99 error " + s_check;
                return false;
            }

        }

        // 仕訳データのSORT
        public bool F12_sort_jounal_data()
        {
            try
            {
                s_log += "\r\n f13 01 s put_JounalData";

                TbMes.Text = DateTime.Now.ToString() + " SORT start (JounalData)";

                // ソート仕訳データ
                ListJounalData.Sort(
                    delegate (JounalData jd1, JounalData jd2)
                    {
                        return string.Compare(jd1.JDateNo, jd2.JDateNo);
                    }
                );
                this.LV_shiwake.Items.Refresh();


                // 画面の仕分け表のカーソルを下にする。
                if (LV_shiwake.Items.Count > 0)
                {
                    var KonoDGC = VisualTreeHelper.GetChild(LV_shiwake, 0) as Decorator;
                    if (KonoDGC != null)
                    {
                        var KonoScroll = KonoDGC.Child as ScrollViewer;
                        if (KonoScroll != null) KonoScroll.ScrollToEnd();
                    }
                }
                LV_shiwake.Items.Refresh();

                TbMes.Text = DateTime.Now.ToString() + " SORT end (JounalData)";

                return true;
            }
            catch (Exception ex)
            {
                //
                BoxError("f12 99 e sort_JounalData \r\n" + ex.Message);
                s_log += "\r\n f11 99 error ";
                return false;
            }
           
        }

        // 仕訳データのファイルアウト
        public bool F13_put_jounal_file()
        {
            string s_rec = "";
            int i_amount;            

            try
            {
                s_log += "\r\n f13 01 s put_JounalData";
                using (StreamWriter SwJounal = new StreamWriter(s_pfile_jounal, false, EncJIS))
                {

                    foreach (JounalData JD in ListJounalData)
                    {
                        s_w1 = JD.JAmount;                        
                        s_w2 = s_w1.Replace(s_kanma, "");
                        i_amount = int.Parse(s_w2);

                        s_rec = "";
                        s_rec += JD.JDateNo + ",";
                        s_rec += JD.DrCode + ",";
                        s_rec += JD.DrName + ",";
                        s_rec += JD.CrCode + ",";
                        s_rec += JD.CrName + ",";
                        s_rec += i_amount.ToString() + ",";
                        s_rec += JD.JExplanation + ",";
                        SwJounal.WriteLine(s_rec);
                    }
                    SwJounal.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                
                BoxError("f12 99 error \r\n" + ex.Message);
                
                s_log += "\r\n f12 98 " + ex.Message;
                s_log += "\r\n f12 98 error";
                return false;
            }
        }

        // データの追加
        public bool F21_data_add()
        {
            string s_jdateno = "";
            int i_nextjno = 0;
            string s_kanma = ",";

            try
            {
                s_log += "\r\n f21 01 sdata_add ";
                // 仕訳日がDateTimeに変換できるか確かめる
                string s_jdate = TbJDate.Text.Trim();
                if (DateTime.TryParse(s_jdate, out DateTime d_jdate))
                {
                    i_nextjno = i_endjno + 1;
                    s_jdateno = d_jdate.ToString("MM/dd") + " " + i_nextjno.ToString();
                }
                else
                {
                    BoxError("21 11 error \r\n" + "日付を認識できません。");
                    s_log += "\r\n f21 11 日付を認識できません。";
                    s_log += "\r\n f21 11 error";
                    return false;
                }
                // 借方のインプットチェック
                if (i_sdrcode == 0 || s_sdrname == "" )
                {
                    BoxError("21 12 error \r\n" + "借方の選択異常です。");
                    s_log += "\r\n f21 12 借方の選択異常です。";
                    s_log += "\r\n f21 12 error";
                    return false;
                }
                // 借方貸方のインプットチェック
                if (i_scrcode == 0 || s_scrname == "")
                {
                    BoxError("21 13 error \r\n" + "貸方の選択異常です。");
                    s_log += "\r\n f21 13 貸方の選択異常です。";
                    s_log += "\r\n f21 13 error";
                    return false;
                }
                // 金額のインプットチェック                
                string s_workamount = TbJAmount.Text.Trim();
                s_workamount = s_workamount.Replace(s_kanma, "");
                s_log += "\r\n f21 14.1 s_work amount=" + s_workamount;
                if (int.TryParse(s_workamount, out i_jamount))
                {
                    s_jamount = i_jamount.ToString("#,0");
                }
                else
                {
                    BoxError("21 14 error \r\n" + "金額の認識できません。");
                    s_log += "\r\n f21 14 金額の認識できません。";
                    s_log += "\r\n f21 14 error";
                    return false;
                }
                // 項目適用のインプットチェック
                string s_workxp = TbJExp.Text;
                s_workxp = s_workxp.Replace(s_kanma, " ");
                s_jxplanation = s_workxp.Trim();
                if (s_jxplanation == "")
                {
                    BoxError("21 15 error \r\n" + "項目適用の認識できません。");
                    s_log += "\r\n f21 15 項目適用の認識できません。";
                    s_log += "\r\n f21 15 error";
                    return false;
                }

                // 追加処理
                JounalData JD = new JounalData();
                JD.JDateNo = s_jdateno;
                JD.DrCode = i_sdrcode.ToString();
                JD.DrName = s_sdrname;
                JD.CrCode = i_scrcode.ToString();
                JD.CrName = s_scrname;
                JD.JAmount = s_jamount;
                JD.JExplanation = s_jxplanation;
                ListJounalData.Add(JD);

                TbZenJDate.Text = TbJDate.Text;
                TbZenJAmount.Text = TbJAmount.Text;
                TbZenJExp.Text = TbJExp.Text;
                TbJDate.Text = "";
                TbJAmount.Text = "";
                TbJExp.Text = "";

                i_endjno = i_nextjno;

                // データグリッドのリフレッシュ　追加に対応して
                LV_shiwake.Items.Refresh();

                DateTime d_Add = DateTime.Now;
                TbMes.Text = d_Add.ToString() + " 追加 (" + s_jdateno + ")";

                // スクロールバーを下に持ってくる
                if (LV_shiwake.Items.Count > 0)
                {
                    if (VisualTreeHelper.GetChild(LV_shiwake, 0) is Decorator KonoDGC)
                    {
                        if (KonoDGC.Child is ScrollViewer KonoScroll) KonoScroll.ScrollToEnd();
                    }
                }
                LV_shiwake.Items.Refresh();

                return true;
            }
            catch (Exception ex)
            {
                //
                BoxError("f21 99 error \r\n" + ex.Message);
                s_log += "\r\n f21 98 " + ex.Message;
                s_log += "\r\n f21 98 error";
                return false;
            }
        }

        // delete 仕訳データの削除
        public bool F22_siwake_delete()
        {
            string s_jdateno = "";


            try
            {
                // 削除日がDateTimeに変換できるか確かめる                
                string s_deldate = TbDelBi.Text.Trim();
                s_log += "\r\n f22 11 TbDelBi=" + s_deldate;
                if (DateTime.TryParse(s_deldate, out DateTime d_jdate))
                {
                    // 月日連番の項目の前部分作成
                    s_jdateno = d_jdate.ToString("MM/dd") + " ";
                }
                else
                {
                    BoxError("22 11 error \r\n" + "日付を認識できません。");
                    s_log += "\r\n f22 11 日付を認識できません。";
                    s_log += "\r\n f22 11 error";
                    return false;
                }

                // 削除の連番のインプットチェック                
                string s_renban = TbDelNo.Text.Trim();
                s_log += "\r\n f22 12 TbDelNo=" + s_renban;
                if (int.TryParse(s_renban, out int i_renban))
                {
                    // 月日連番の項目の後部分作成
                    s_jdateno += i_renban.ToString();
                }
                else
                {
                    BoxError("22 14 error \r\n" + "削除の連番が認識できません。");
                    s_log += "\r\n f22 12 削除の連番が認識できません。";
                    s_log += "\r\n f22 12 error";
                    return false;
                }

                // 対象行の削除
                s_log += "\r\n f22 13 siwake_delete s ";
                bool b_delete = false;
                foreach (JounalData oJ in ListJounalData)
                {
                    if (oJ.JDateNo == s_jdateno)
                    {
                        ListJounalData.Remove(oJ);
                        b_delete = true;
                        break;
                    }
                }
                TbZenDelBi.Text = TbDelBi.Text;
                TbZenDelNo.Text = TbDelNo.Text;
                TbDelBi.Text = "";
                TbDelNo.Text = "";

                // データグリッドのリフレッシュ　削除に対応して
                LV_shiwake.Items.Refresh();

                if (b_delete)
                {
                    s_log += "\r\n f22 14 siwake_delete c ";
                    DateTime d_Add = DateTime.Now;
                    TbMes.Text = d_Add.ToString() + " 削除 (" + s_jdateno + ")";

                    // スクロールバーを下に持ってくる
                    if (LV_shiwake.Items.Count > 0)
                    {
                        var KonoDGC = VisualTreeHelper.GetChild(LV_shiwake, 0) as Decorator;
                        if (KonoDGC != null)
                        {
                            var KonoScroll = KonoDGC.Child as ScrollViewer;
                            if (KonoScroll != null) KonoScroll.ScrollToEnd();
                        }
                    }
                    LV_shiwake.Items.Refresh();
                }
                else
                {
                    BoxError("f22 14 error \r\n" + "削除されませんでした。");
                    s_log += "\r\n f22 14 " + "削除されませんでした。";
                    s_log += "\r\n f22 14 error";
                }

                return true;
            }
            catch (Exception ex)
            {
                BoxError("f22 99 error \r\n" + ex.Message);
                s_log += "\r\n f22 98 " + ex.Message;
                s_log += "\r\n f22 98 error";
                return false;
            }
        }

        // 元帳データ作成とソート
        public bool F31_motochou()
        {
            try
            {
                s_log += "\r\n f31 01 Ledger Create ";
                TbMes.Text = DateTime.Now.ToString() + " Ledger Create start ";

                // 元帳初期化と仕訳データ張り込み
                ListGLeger.Clear();

                foreach(JounalData JD in ListJounalData)
                {
                    s_w1 = JD.JAmount;
                    s_w2 = s_w1.Replace(s_kanma, "");
                    int i_amount = int.Parse(s_w2);

                    GLedger LD = new GLedger();
                    GLedger LC = new GLedger();
                    LD.GLSKey = JD.DrCode + JD.JDateNo;
                    LC.GLSKey = JD.CrCode + JD.JDateNo;
                    int i_drcode = int.Parse(JD.DrCode);
                    int i_crcode = int.Parse(JD.CrCode);
                    LD.LSCode = i_drcode;
                    LC.LSCode = i_crcode;
                    LD.LSName = JD.DrName;
                    LC.LSName = JD.CrName;
                    LD.LDateNo = JD.JDateNo;
                    LC.LDateNo = JD.JDateNo;
                    LD.PSCode = int.Parse(JD.CrCode);
                    LC.PSCode = int.Parse(JD.DrCode);
                    LD.PSName = JD.CrName;
                    LC.PSName = JD.DrName;
                    LD.LExplanation = JD.JExplanation;
                    LC.LExplanation = JD.JExplanation;
                    LD.DrAmount = int.Parse(s_w2);
                    LD.CrAmount = 0;
                    LC.DrAmount = 0;
                    LC.CrAmount = int.Parse(s_w2);

                    // // ---------------------------------------------
                    s_check = "元帳初期化と仕訳データ張り込み 2";
                    LD.SKubun = "C";
                    LC.SKubun = "C";
                    if (LD.LSCode < 200 || LD.LSCode > 500) LD.SKubun = "D";
                    if (LC.LSCode < 200 || LC.LSCode > 500) LC.SKubun = "D";
                    // // ---------------------------------------------
                    s_check = "元帳初期化と仕訳データ張り込み 3";

                    if (LD.SKubun == "D") LD.LTotal = LD.DrAmount - LD.CrAmount;
                    if (LD.SKubun == "C") LD.LTotal = LD.CrAmount - LD.DrAmount;
                    if (LC.SKubun == "D") LC.LTotal = LC.DrAmount - LC.CrAmount;
                    if (LC.SKubun == "C") LC.LTotal = LC.CrAmount - LC.DrAmount;

                    ListGLeger.Add(LD);
                    ListGLeger.Add(LC);
                }
                
                s_log += "\r\n f31 01 Ledger SORT start ";
                TbMes.Text = DateTime.Now.ToString() + " Ledger SORT start ";

                // ソート元帳
                ListGLeger.Sort(
                    delegate (GLedger L1, GLedger L2)
                    {
                        return string.Compare(L1.GLSKey, L2.GLSKey);
                    }
                );

                s_log += "\r\n f31 01 Ledger SORT end ";
                TbMes.Text = DateTime.Now.ToString() + " Ledger SORT end ";

                return true;

            }
            catch (Exception ex)
            {
                BoxError("f31 99 error \r\n" + ex.Message + "\r\n" + s_check);
                s_log += "\r\n f31 98 " + ex.Message;
                s_log += "\r\n f31 98 error";
                return false;
            }  
        }

        // 元帳集計と試算表作成
        public bool F32_shisan()
        {
            try
            {
                s_log += "\r\n f32 01 Motochou shuukei start ";
                TbMes.Text = DateTime.Now.ToString() + " Motochou shuukei start ";

                ListShisanhyou.Clear();

                int i_L1SCode = 0;
                int i_L1DrZan = 0;
                int i_L1DrSum = 0;
                int i_L1CrSum = 0;
                int i_L1CrZan = 0;
                int i_L2DrZan = 0;
                int i_L2DrSum = 0;                
                int i_L2CrSum = 0;
                int i_L2CrZan = 0;
                int i_L3DrZan = 0;
                int i_L3DrSum = 0;
                int i_L3CrSum = 0;
                int i_L3CrZan = 0;
                string s_SSname = "";
                string s_skubun = "";
                double d_k1 = 0;
                double d_k2 = 0;

                int i_ledgercount = ListGLeger.Count;
                for (int i = 0; i < i_ledgercount; i++)
                {
                    // 科目ブレーク処理
                    int i_keycode = ListGLeger[i].LSCode;
                    if (i_keycode != i_L1SCode)
                    {
                        // 初回のskipチェック
                        if(i_L1SCode > 0)
                        {
                            // 科目ブレーク試算表書き込み　科目別
                            // // ---------------------------------------------
                            s_check = "科目ブレーク試算表書き込み　科目別";
                            ShisanHyou Sh1 = new ShisanHyou();
                            Sh1.SSCode = i_L1SCode;
                            Sh1.SSName = s_SSname;
                            Sh1.DrSum = i_L1DrSum;
                            Sh1.CrSum = i_L1CrSum;
                            Sh1.DrZan = i_L1DrZan;
                            Sh1.CrZan = i_L1CrZan;
                            ListShisanhyou.Add(Sh1);
                            // 中分類２番目以降の処理　合計
                            // // ---------------------------------------------
                            s_check = "中分類２番目以降の処理　合計";
                            i_L2DrSum += i_L1DrSum;
                            i_L2CrSum += i_L1CrSum;
                            i_L2DrZan += i_L1DrZan;
                            i_L2CrZan += i_L1CrZan;
                            // 最終合計の処理　合計
                            i_L3DrSum += i_L1DrSum;
                            i_L3CrSum += i_L1CrSum;
                            i_L3DrZan += i_L1DrZan;
                            i_L3CrZan += i_L1CrZan;

                            // 中分類ブレーク処理
                            // // ---------------------------------------------
                            s_check = "中分類ブレーク処理";
                            d_k1 = i_keycode / 100;
                            d_k2 = i_L1SCode / 100;
                            if (Math.Floor(d_k1) != Math.Floor(d_k2))
                            {
                                // ブレーク中分類行追加書き込み
                                ShisanHyou Sh2 = new ShisanHyou();
                                // // ---------------------------------------------
                                s_check = "ブレーク中分類行追加書き込み";
                                Sh2.SSCode = (int)Math.Floor(d_k2) * 100;
                                Sh2.SSName = "";
                                Sh2.DrSum = i_L2DrSum;
                                Sh2.CrSum = i_L2CrSum;
                                Sh2.DrZan = i_L2DrZan;
                                Sh2.CrZan = i_L2CrZan;
                                ListShisanhyou.Add(Sh2);
                                // 中分類 先頭処理初期化
                                i_L2DrSum = 0;
                                i_L2CrSum = 0;
                                i_L2DrZan = 0;
                                i_L2CrZan = 0;
                            }
                        }

                        // 科目先頭処理
                        // // ---------------------------------------------
                                s_check = " 科目先頭処理";
                        i_L1SCode = ListGLeger[i].LSCode;
                        s_SSname = ListGLeger[i].LSName;
                        i_L1DrSum = ListGLeger[i].DrAmount;
                        i_L1CrSum = ListGLeger[i].CrAmount;
                        s_skubun = ListGLeger[i].SKubun;
                        i_L1DrZan = 0;
                        i_L1CrZan = 0;
                        if (s_skubun == "D") i_L1DrZan = ListGLeger[i].LTotal;
                        if (s_skubun == "C") i_L1CrZan = ListGLeger[i].LTotal;
                    }
                    else
                    {
                        // 同一科目２番目以降の処理　合計
                        // // ---------------------------------------------
                        s_check = " 同一科目２番目以降の処理　合計";
                        i_L1DrSum += ListGLeger[i].DrAmount;
                        i_L1CrSum += ListGLeger[i].CrAmount;
                        if (s_skubun == "D") i_L1DrZan += ListGLeger[i].LTotal;
                        if (s_skubun == "C") i_L1CrZan += ListGLeger[i].LTotal;
                        // 同一科目２番目以降の処理　LIST更新
                        ListGLeger[i].LTotal = i_L1DrZan + i_L1CrZan;
                    }
                }
                // 最終の科目ブレーク試算表書き込み　科目別
                // // ---------------------------------------------
                s_check = " 最終の科目ブレーク試算表書き込み　科目別";
                ShisanHyou Sh3 = new ShisanHyou();
                Sh3.SSCode = i_L1SCode;
                Sh3.SSName = s_SSname;
                Sh3.DrSum = i_L1DrSum;
                Sh3.CrSum = i_L1CrSum;
                Sh3.DrZan = i_L1DrZan;
                Sh3.CrZan = i_L1CrZan;
                ListShisanhyou.Add(Sh3);
                // 最終の中分類２番目以降の処理　合計
                i_L2DrSum += i_L1DrSum;
                i_L2CrSum += i_L1CrSum;
                i_L2DrZan += i_L1DrZan;
                i_L2CrZan += i_L1CrZan;
                // 最終合計の処理　合計
                i_L3DrSum += i_L1DrSum;
                i_L3CrSum += i_L1CrSum;
                i_L3DrZan += i_L1DrZan;
                i_L3CrZan += i_L1CrZan;
                // 最終のブレーク中分類行追加書き込み
                ShisanHyou Sh4 = new ShisanHyou();
                d_k2 = i_L1SCode / 100;
                Sh4.SSCode = (int)Math.Floor(d_k2) * 100;
                Sh4.SSName = "";
                Sh4.DrSum = i_L2DrSum;
                Sh4.CrSum = i_L2CrSum;
                Sh4.DrZan = i_L2DrZan;
                Sh4.CrZan = i_L2CrZan;
                ListShisanhyou.Add(Sh4);
                // 最終のtotal行追加書き込み
                ShisanHyou Sh5 = new ShisanHyou();
                Sh5.SSCode = 0;
                Sh5.SSName = "";
                Sh5.DrSum = i_L3DrSum;
                Sh5.CrSum = i_L3CrSum;
                Sh5.DrZan = i_L3DrZan;
                Sh5.CrZan = i_L3CrZan;
                ListShisanhyou.Add(Sh5);


                s_log += "\r\n f32 81 Shisanhyou end ";
                TbMes.Text = DateTime.Now.ToString() + " Shisanhyou end ";

                return true;
            }
            catch (Exception ex)
            {
                BoxError("f32 99 error \r\n" + ex.Message + "\r\n" + s_check);
                s_log += "\r\n f32 98 " + ex.Message;
                s_log += "\r\n f32 98 error";
                return false;
            }
        }

        // 元帳データアウト
        public bool F34_put_motochou()
        {
            string s_rec = "";
            string s_kanma = ",";

            try
            {
                s_log += "\r\n f34 01 s put_LedgerData";
                using (StreamWriter SwLedger = new StreamWriter(s_pfile_Ledger, false, EncJIS))
                {
                    foreach( GLedger GL in ListGLeger)
                    {
                        s_rec = "";
                        s_rec += GL.LSCode.ToString() + s_kanma;
                        s_rec += GL.LSName + s_kanma;
                        s_rec += GL.LDateNo + s_kanma;
                        s_rec += GL.PSCode.ToString() + s_kanma;
                        s_rec += GL.PSName + s_kanma;
                        s_rec += GL.LExplanation + s_kanma;
                        s_rec += GL.DrAmount.ToString() + s_kanma;
                        s_rec += GL.CrAmount.ToString() + s_kanma;
                        s_rec += GL.SKubun + s_kanma;
                        s_rec += GL.LTotal.ToString() + s_kanma;
                        SwLedger.WriteLine(s_rec);
                    }
                    SwLedger.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                BoxError("f34 99 error \r\n" + ex.Message);
                s_log += "\r\n f34 98 " + ex.Message;
                s_log += "\r\n f34 98 error";
                return false;
            }
        }

        // 試算表データアウト
        public bool F35_put_shisan()
        {
            string s_rec = "";
            string s_kanma = ",";

            try
            {
                s_log += "\r\n f35 01 s put_shisan Data";
                using (StreamWriter SwShisan = new StreamWriter(s_pfile_Shisan, false, EncJIS))
                {
                    foreach(ShisanHyou Sh in ListShisanhyou)
                    {
                        s_rec = "";
                        s_rec += Sh.SSCode.ToString() + s_kanma;
                        s_rec += Sh.DrZan.ToString() + s_kanma;
                        s_rec += Sh.DrSum.ToString() + s_kanma;
                        s_rec += Sh.SSName + s_kanma;
                        s_rec += Sh.CrSum.ToString() + s_kanma;
                        s_rec += Sh.CrZan.ToString() + s_kanma;
                        SwShisan.WriteLine(s_rec);
                    }
                    SwShisan.Close(); 
                }
                return true;
            }
            catch (Exception ex)
            {
                BoxError("f35 99 error \r\n" + ex.Message);
                s_log += "\r\n f35 98 " + ex.Message;
                s_log += "\r\n f35 98 error";
                return false;
            }
        }

        // MainWindow
        public MainWindow()
        {
            InitializeComponent();
        }

// ﾒｯｾｰｼﾞﾎﾞｯｸｽ

        //簡略化したﾒｯｾｰｼﾞﾎﾞｯｸｽError
        private void BoxError(string m1)
        {
            string w_mes = m1 + "\r\n修整しやり直しください。";
            MessageBox.Show(w_mes,
                "voqui3 Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        //簡略化したﾒｯｾｰｼﾞﾎﾞｯｸｽExclamation
        private void BoxMes(string m1)
        {
            string w_mes = m1 + "\r\n確認します。";
                MessageBox.Show(w_mes,
                    "voqui3 Exclamation",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
        }
        //簡略化したﾒｯｾｰｼﾞﾎﾞｯｸｽ OK CANCEL
        private bool BoxDoch(string mes)
        {
            string w_mes;
            bool b_OK = true;

            w_mes = mes + "\r\n";
            w_mes += "この確認に問題なければ「OK」。\r\n";
            w_mes += "以外の場合「ｷｬﾝｾﾙ」し、まず見直しをしてください。";

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

// ボタンと イベント

        // ボタン「DATA追加」
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            bool b_value = true;

            ButtonAdd.IsEnabled = false;

            b_value = F21_data_add();

            ButtonAdd.IsEnabled = true;
            ButtonOut.IsEnabled = true;
            ButtonSORT.IsEnabled = true;

            F09_put_log_file();
            if (!b_value) return;

        }
        // ボタン「DATA削除」
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            bool b_value;

            ButtonDel.IsEnabled = false;

            b_value = F22_siwake_delete();

            ButtonDel.IsEnabled = true;
            ButtonOut.IsEnabled = true;
            ButtonSORT.IsEnabled = true;

            if (!b_value) s_log += "\r\n ButtonDel error exit";
            F09_put_log_file();

        }

        // ボタン「設定」
        private void ButtonSettei_Click(object sender, RoutedEventArgs e)
        {
            F13_put_jounal_file();

            var SWin1 = new SubWindow1();
            SWin1.Show();

            this.Close();

        }

        // ボタン「出力」エクセルファイルを作成
        private void ButtonOut_Click(object sender, RoutedEventArgs e)
        {
            bool b_value;

            ButtonOut.IsEnabled = false;

            b_value = F13_put_jounal_file();

            if (b_value) b_value = F31_motochou();

            if (b_value) b_value = F32_shisan();

            if (b_value) b_value = F34_put_motochou();

            if (b_value) b_value = F35_put_shisan();            

            if (b_value)
            {
                var SWin2 = new SubWindow2
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                SWin2.Show();
            }

            if (!b_value) s_log += "\r\n ButtonOut error exit";
            F09_put_log_file();
        }

        // ウインドウ　ロードイベント
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool b_value;

            b_value = F00_saisho();

            if (b_value) b_value = F01_get_param1_file();

            if (b_value) b_value = F03_get_param2_file();

            if (b_value) b_value = F04_seljob();

            if (b_value) b_value = F11_get_JounalData();

            if (b_value) s_log += "\r\n" + " ..";
            F09_put_log_file();

            if (i_nendo == 0)
            {
                var SWin1 = new SubWindow1();
                SWin1.Show();
                this.Close();
            }

        }


        // 仕訳作業種類　通常　COMBOBOX 選択変更イベント
        private void Combo_job_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool b_value = true;
            int i_si = Combo_job.SelectedIndex;

            if (i_si >= 0) b_value = F05_sagyou_handan(i_si);

            F09_put_log_file();

        }

        // テスト 8/27
        private void ButtonTest_Click(object sender, RoutedEventArgs e)
        {
            this.ButtonTest.Content = "--";
        }

        // 「SORT」ボタン
        private void ButtonSORT_Click(object sender, RoutedEventArgs e)
        {
            bool b_value;

            ButtonSORT.IsEnabled = false;

            b_value = F12_sort_jounal_data();

            if (!b_value) s_log += "\r\n ButtonSORT error exit";
            F09_put_log_file();

        }

        // 画面の「X」ボタン
        private void Window_Closed(object sender, EventArgs e)
        {
        }

        // 「保存終了」ボタン
        private void ButtonEnd_Click(object sender, RoutedEventArgs e)
        {
            bool b_value;

            b_value = F02_put_param1_file();

            if (b_value) b_value = F13_put_jounal_file();

            if (!b_value)
            {
                s_log += "\r\n ButtonEnd_Click error";
            }
            F09_put_log_file();

            this.Close();

        }

        // ボタン「Exls」エクセルファイルを開く
        private void ButtonExls_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(s_pfile_output))
            {
                _ = System.Diagnostics.Process.Start(s_pfile_output);
            }
        }

        // 仕訳追加　借方　COMBOBOX 選択変更イベント
        private void CBox_kari_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool b_value;

            b_value = F06_karikata_sel();

            if (!b_value)
            {
                s_log += "\r\n CBox_karierror";
            }
            F09_put_log_file();
        }

        // 仕訳追加　貸方　COMBOBOX 選択変更イベント
        private void CBox_kashi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bool b_value;

            b_value = F07_kashikata_sel();

            if (!b_value)
            {
                s_log += "\r\n CBox_kashi error";
            }
            F09_put_log_file();
        }
    }


    #region その他クラス

    // 仕訳作業
    public class JounalJS
    {
        public string JOpeName { get; set; }      // 仕訳作業名
        public string DrGroup { get; set; }       // 借方グループ
        public string CrGroup { get; set; }       // 貸方グループ
        public string DrGMes { get; set; }        // 借方グループ説明 Debt
        public string CrGMes { get; set; }        // 貸方グループ説明 Credit
    }

    // 選択借方
    public class SelectDr
    {
        public int SDrCode { get; set; }          // 選択借方コード
        public string SDrName { get; set; }       // 選択借方
    }

    // 選択貸方
    public class SelectCr
    {
        public int SCrCode { get; set; }          // 選択貸方コード
        public string SCrName { get; set; }       // 選択貸方
    }

    // 科目表
    public class BSubject
    {
        public int SCode { get; set; }            // 科目コード
        public string Sname { get; set; }         // 科目名
    }

    // 仕訳データ
    public class JounalData
    {
        public string JDateNo { get; set; }       // 日連番
        public string DrCode { get; set; }        // 借方コード
        public string DrName { get; set; }        // 借方
        public string CrCode { get; set; }        // 貸方コード
        public string CrName { get; set; }        // 貸方
        public string JAmount { get; set; }       // 金額
        public string JExplanation { get; set; }  // 適用
    }

    // 元帳データ
    public class GLedger
    {
        public string GLSKey { get; set; }        // 元帳キー(元帳科目コード+日連番)
        public int LSCode { get; set; }           // 元帳科目コード
        public string LSName { get; set; }        // 元帳科目
        public string LDateNo { get; set; }       // 日連番
        public int PSCode { get; set; }           // 相手科目コード
        public string PSName { get; set; }        // 相手科目
        public string LExplanation { get; set; }  // 適用
        public int DrAmount { get; set; }         // 借方金額
        public int CrAmount { get; set; }         // 貸方金額
        public string SKubun { get; set; }        // 区分 D=借 or C=貸
        public int LTotal { get; set; }           // 残高
    }

    // 試算表データ
    public class ShisanHyou
    {
        public int SSCode { get; set; }           // 科目コード
        public int DrZan { get; set; }            // 借方残高
        public int DrSum { get; set; }            // 借方合計
        public string SSName { get; set; }        // 科目
        public int CrSum { get; set; }            // 貸方合計
        public int CrZan { get; set; }            // 貸方残高
    }
    #endregion
    // ----------------------------------------------------------------------- voqui3
    // ----------------------------------------------------------------------- 2017/8/4 start
    // ----------------------------------------------------------------------- 2017/8/28 change
    // ----------------------------------------------------------------------- 2020/2/16 change 3.3
}
