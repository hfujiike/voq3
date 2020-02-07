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
using System.Windows.Shapes;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;

namespace voqui3
{
    /// <summary>
    /// SubWindow2.xaml の相互作用ロジック
    /// </summary>
    public partial class SubWindow2 : Window
    {
        Encoding EncJIS = Encoding.GetEncoding("Shift-JIS");

        // 実行dirの読込
        static string s_EPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        static string s_EDir = System.IO.Path.GetDirectoryName(s_EPath);

        static string s_pfile_param1 = s_EDir + "\\VOQ3P1.txt";   // 設定ファイル(年度,ライセンスキー,最終連番)
        static string s_pfile_op2log = s_EDir + "\\VOQ3L1.txt";   // ログファイル
        static string s_pfile_mainpg = s_EDir + "\\VOQ3AA.exe";   // メインプログラム
        static string s_pfile_templa = s_EDir + "\\VOQ3T1.xlsx";  // エクセルテンプレート
        static string s_pfile_exdata = s_EDir + "\\VOQ3D0.xlsx";  // エクセル簿記データ        

        // ファイル取得用
        FileStream fst = null;
        FileStream fsd = null;

        // ブック格納用
       IWorkbook book_exdata = null;  // エクセルック簿記データ 

        public long L_shisan = 0;       // 資産合計
        public long L_fusai = 0;        // 負債合計
        public long L_shihon = 0;       // 資本合計
        public long L_uriage = 0;       // 売上合計
        public long L_keihi = 0;        // 経費合計
        public long L_rieki = 0;        // 利益合計
        public long L_junshi = 0;       // 純資合計
        public long s_mesform = 0;      // メッセージ番号

        public string s_hyoud1 = "";
        public string s_hyoud2 = "";

        ICellStyle cstyle_price = null;
        ICellStyle cstyle_wb = null;
        ICellStyle cstyle_toprice = null;
        ICellStyle cstyle_tostr = null;
        ICellStyle cstyle_hyou1 = null;
        ICellStyle cstyle_hyou2 = null;

        List<Kessanrecord> list_shisan = new List<Kessanrecord>();
        List<Kessanrecord> list_fusai  = new List<Kessanrecord>();
        List<Kessanrecord> list_junshi = new List<Kessanrecord>();
        List<Kessanrecord> list_shuueki  = new List<Kessanrecord>();
        List<Kessanrecord> list_hiyou  = new List<Kessanrecord>();

        #region // const' SubWindow2 //
        public SubWindow2()
        {
            InitializeComponent();
            
        }
        #endregion

        #region エクセルデータ作成スタート処理
        public bool f201_start()
        {
            //  エクセルの簿記用のテンプレートから結果格納用ブックを作成

            using (fst = File.OpenRead(s_pfile_templa))
            {
                tbox_jstaus.Text = "テンプレート用Excelファイルの接続";
                Task.Delay(300);

                book_exdata = new XSSFWorkbook(s_pfile_templa);                

                tbox_jstaus.Text = "テンプレート用Excelファイルからセルスタイル読込";
                ISheet is_cstyle = book_exdata.GetSheet("cstylesheet");
                IRow row_cstyle = is_cstyle.GetRow(1);
                ICell cw1 = row_cstyle.GetCell(1);
                cstyle_wb = cw1.CellStyle;
                ICell cw2 = row_cstyle.GetCell(2);
                cstyle_price = cw2.CellStyle;
                ICell cw3 = row_cstyle.GetCell(3);
                cstyle_toprice = cw3.CellStyle;
                ICell cw4 = row_cstyle.GetCell(4);
                cstyle_tostr = cw4.CellStyle;
                ICell cw5 = row_cstyle.GetCell(5);
                cstyle_hyou1 = cw5.CellStyle;
                ICell cw6 = row_cstyle.GetCell(6);
                cstyle_hyou2 = cw6.CellStyle;

                int sheet_del = book_exdata.GetSheetIndex("cstylesheet");
                book_exdata.RemoveSheetAt(sheet_del);
            }

            // パラメータファイルの読み
            using (StreamReader SrParam = new StreamReader(s_pfile_param1, EncJIS))
            {
                string s_rec = "";

                while (SrParam.Peek() >= 0)
                {
                    s_rec = SrParam.ReadLine() + "==";
                    string[] a_rec = s_rec.Split('=');
                    string s_item0 = a_rec[0].Trim();
                    string s_item1 = a_rec[1].Trim();
                    //
                    if (s_item0 == "HYUD1")
                    {
                        s_hyoud1 = s_item1;
                    }
                    else if (s_item0 == "HYUD2")
                    {
                        s_hyoud2 = s_item1;
                    }
                }
            }

            return true;
        }
        #endregion


        #region 仕分けの作成 f231
        public bool f231_shiwake()
        {
            tbox_jstaus.Text = "仕訳処理 開始";
            Task.Delay(300);

            ISheet is_journal = book_exdata.GetSheet("journal");

            IRow row_journal_h1 = is_journal.GetRow(1);
            ICell cell_jh1b = row_journal_h1.CreateCell(0);
            cell_jh1b.CellStyle = cstyle_hyou1;
            cell_jh1b.SetCellValue(s_hyoud1);
            IRow row_journal_h2 = is_journal.GetRow(2);
            ICell cell_jh2b = row_journal_h2.CreateCell(0);
            cell_jh2b.CellStyle = cstyle_hyou2;
            cell_jh2b.SetCellValue(s_hyoud2);

            string s_pf_data = s_EDir + @"\VOQ3D1.csv";
            using (StreamReader sr_data = new StreamReader(s_pf_data, EncJIS))
            {                
                int i_linej = 5;

                string s_rec = "";
                while ( (s_rec = sr_data.ReadLine() ) != null)
                {
                    string[] a_item = s_rec.Split(',');
                    is_journal.CreateRow(i_linej);
                    IRow row_journal = is_journal.GetRow(i_linej);

                    ICell cell_1 = row_journal.CreateCell(1);    // 仕訳日と連番
                    cell_1.SetCellValue(a_item[0]);
                    ICell cell_2 = row_journal.CreateCell(2);    // 科目コード
                    cell_2.SetCellValue(a_item[1]);
                    ICell cell_3 = row_journal.CreateCell(3);    // 科目名
                    cell_3.SetCellValue(a_item[2]);
                    ICell cell_4 = row_journal.CreateCell(4);    // 科目コード
                    cell_4.SetCellValue(a_item[3]);
                    ICell cell_5 = row_journal.CreateCell(5);    // 科目名
                    cell_5.SetCellValue(a_item[4]);
                    ICell cell_6 = row_journal.CreateCell(6);    // 金額
                    cell_6.CellStyle = cstyle_price;
                    long L_kin = 0;
                    long.TryParse(a_item[5], out L_kin);
                    cell_6.SetCellValue(L_kin);
                    ICell cell_7 = row_journal.CreateCell(7);    // 借貸適用
                    cell_7.SetCellValue(a_item[6]);

                    i_linej++;
                }

                tbox_jstaus.Text = "仕訳処理 修了";

            }

            return true;
        }
        #endregion

        #region 勘定元帳コピー f232
        public bool f232_kanjou()
        {
            // 勘定元帳コピー
            tbox_jstaus.Text = "勘定元帳コピー 開始";
            Task.Delay(300);

            ISheet is_accounting = book_exdata.GetSheet("accounting");
            
            // 屋号（会社名）と年度と期間からまでのセット
            IRow row_a1 = is_accounting.CreateRow(1);
            ICell cell_a1 = row_a1.CreateCell(1);
            cell_a1.SetCellValue(s_hyoud1);
            cell_a1.CellStyle = cstyle_hyou1;
            IRow row_a2 = is_accounting.CreateRow(2);
            ICell cell_a2 = row_a2.CreateCell(1);
            cell_a2.SetCellValue(s_hyoud2);
            cell_a2.CellStyle = cstyle_hyou2;

            // 勘定元帳明細
            string s_pf_kdata = s_EDir + @"\VOQ3D2.csv";
            using (StreamReader sr_kdata = new StreamReader(s_pf_kdata, EncJIS))
            {
                string s_rec = "";
                string s_kamoku = "";
                int i_linek = 0;                
                i_linek = 4;
                s_rec = "";
                bool b_himokutop = false;
                bool b_kanjoutop = true;
                while ((s_rec = sr_kdata.ReadLine()) != null)
                {
                    string[] a_item = s_rec.Split(',');
                    if (s_kamoku != a_item[0])
                    {
                        // 費目の終わり（費目がブレーク）
                        s_kamoku = a_item[0];
                        if (b_kanjoutop)
                        {
                            b_kanjoutop = false;
                        }
                        else
                        {                            
                            i_linek++;
                            IRow row_a3 = is_accounting.CreateRow(i_linek);
                            // 区切りのダブル線をいれる
                            i_linek++;
                            IRow row_a4 = is_accounting.CreateRow(i_linek);
                            for (int ix = 1; ix < 11; ix++)
                            {
                                ICell cell_a4x = row_a4.CreateCell(ix);
                                cell_a4x.CellStyle = cstyle_wb;
                            }
                        }
                        
                        // 費目の1件目の設定
                        b_himokutop = true;
                    }
                    else
                    {
                        // 同じ費目の2行目以降の設定
                        b_himokutop = false;
                        
                    }
                    i_linek++;
                    long L_kin = 0;
                    IRow row_a5 = is_accounting.CreateRow(i_linek);
                    ICell cell_a5b = row_a5.CreateCell(1);   // 科目コード1
                    if (b_himokutop) cell_a5b.SetCellValue(a_item[0]);
                    ICell cell_a5c = row_a5.CreateCell(2);   // 科目名1
                    if (b_himokutop) cell_a5c.SetCellValue(a_item[1]);
                    ICell cell_a5d = row_a5.CreateCell(3);   // 仕訳日と連番
                    cell_a5d.SetCellValue(a_item[2]);
                    ICell cell_a5e = row_a5.CreateCell(4);   // 科目コード2
                    cell_a5e.SetCellValue(a_item[3]);
                    ICell cell_a5f = row_a5.CreateCell(5);   // 科目名2
                    cell_a5f.SetCellValue(a_item[4]);
                    ICell cell_a5g = row_a5.CreateCell(6);   // 適用
                    cell_a5g.SetCellValue(a_item[5]);
                    ICell cell_a5h = row_a5.CreateCell(7);   // 借方金額
                    cell_a5h.CellStyle = cstyle_price;
                    long.TryParse(a_item[6], out L_kin);
                    cell_a5h.SetCellValue(L_kin);
                    ICell cell_a5i = row_a5.CreateCell(8);   // 貸方金額
                    cell_a5i.CellStyle = cstyle_price;
                    long.TryParse(a_item[7], out L_kin);
                    cell_a5i.SetCellValue(L_kin);
                    ICell cell_a5j = row_a5.CreateCell(9);   // 借貸
                    cell_a5j.SetCellValue(a_item[8]);
                    ICell cell_a5k = row_a5.CreateCell(10);   // 残高
                    cell_a5k.CellStyle = cstyle_price;
                    long.TryParse(a_item[9], out L_kin);
                    cell_a5k.SetCellValue(L_kin);
                }
            }
            return true;
        }
        #endregion


        #region 試算表の作成 f233
        public bool f233_shisanhyou()
        {
            tbox_jstaus.Text = "試算表の作成 開始";
            Task.Delay(300);

            ISheet is_trial = book_exdata.GetSheet("trial");

            // 屋号（会社名）と年度と期間からまでのセット
            IRow row_t01 = is_trial.CreateRow(1);
            ICell cell_t01b = row_t01.CreateCell(1);
            cell_t01b.CellStyle = cstyle_hyou1;
            cell_t01b.SetCellValue(s_hyoud1);
            IRow row_t02 = is_trial.CreateRow(2);
            ICell cell_t02b = row_t02.CreateCell(1);
            cell_t02b.CellStyle = cstyle_hyou2;
            cell_t02b.SetCellValue(s_hyoud2);

            // 試算集計データの初期処理（決算で参照）
            Kessanrecord rec_sshi = new Kessanrecord();
            rec_sshi.GName = "(資産）";
            rec_sshi.IName = "";
            rec_sshi.IData = 0;
            list_shisan.Add(rec_sshi);
            Kessanrecord rec_sfusai = new Kessanrecord();
            rec_sfusai.GName = "(負債）";
            rec_sfusai.IName = "";
            rec_sfusai.IData = 0;
            list_fusai.Add(rec_sfusai);
            Kessanrecord rec_sjshi = new Kessanrecord();
            rec_sjshi.GName = "(純資産）";
            rec_sjshi.IName = "";
            rec_sjshi.IData = 0;
            list_junshi.Add(rec_sjshi);
            Kessanrecord rec_srieki = new Kessanrecord();
            rec_srieki.GName = "(収益）";
            rec_srieki.IName = "";
            rec_srieki.IData = 0;
            list_shuueki.Add(rec_srieki);
            Kessanrecord rec_shiyou = new Kessanrecord();
            rec_shiyou.GName = "(費用）";
            rec_shiyou.IName = "";
            rec_shiyou.IData = 0;
            list_hiyou.Add(rec_shiyou);

            // 試算表
            string s_pf_tdata = s_EDir + @"\VOQ3D3.csv";
            using (StreamReader sr_tdata = new StreamReader(s_pf_tdata, EncJIS))
            {
                int i_linet = 4;
                int i_kamoku_a = 0;
                int i_kamoku_b = 0;
                bool b_gyouake = false;
                bool b_uesen = false;

                string s_rec = "";
                while ((s_rec = sr_tdata.ReadLine()) != null)
                {
                    string[] a_item = s_rec.Split(',');

                    i_kamoku_a = int.Parse(a_item[0]);     // 科目コード
                    long L_LZan = int.Parse(a_item[1]);    // 借方残高
                    long L_LTot = int.Parse(a_item[2]);    // 借方合計
                    string s_kname = a_item[3];            // 科目名
                    long L_RTot = int.Parse(a_item[4]);    // 貸方合計
                    long L_RZan = int.Parse(a_item[5]);    // 貸方残高

                    b_gyouake = false;
                    b_uesen = false;
                    if (i_kamoku_a < i_kamoku_b)                        
                    {
                        // 以下サブトータルの場合

                        // トータルへ集計
                        if (i_kamoku_a == 100) L_shisan = L_LZan;    // 資産
                        if (i_kamoku_a == 200) L_fusai = L_RZan;     // 負債
                        if (i_kamoku_a == 300) L_shihon = L_RZan;    // 資本
                        if (i_kamoku_a == 400) L_uriage = L_RZan;    // 売上
                        if (i_kamoku_a == 500) L_keihi =  L_LZan;    // 費用

                        // 設定
                        b_gyouake = true; // 行あけ
                        b_uesen = true;   // 上線
                    }
                    // 編集処理
                    i_linet++;
                    IRow row_t1 = is_trial.CreateRow(i_linet);

                    ICell cell_t1 = row_t1.CreateCell(1);      // 借方残高
                    cell_t1.CellStyle = cstyle_price;
                    if (b_uesen) cell_t1.CellStyle = cstyle_toprice;
                    cell_t1.SetCellValue(L_LZan);
                    ICell cell_t2 = row_t1.CreateCell(2);      // 借方合計
                    cell_t2.CellStyle = cstyle_price;
                    if (b_uesen) cell_t2.CellStyle = cstyle_toprice;
                    cell_t2.SetCellValue(L_LTot);
                    ICell cell_t3 = row_t1.CreateCell(3);      // 科目コード
                    cell_t3.CellStyle = cstyle_price;
                    if (b_uesen) cell_t3.CellStyle = cstyle_toprice;
                    if (i_kamoku_a > 0) cell_t3.SetCellValue(i_kamoku_a);
                    ICell cell_t4 = row_t1.CreateCell(4);      // 科目名
                    if (b_uesen) cell_t4.CellStyle = cstyle_tostr;
                    cell_t4.SetCellValue(s_kname);
                    ICell cell_t5 = row_t1.CreateCell(5);      // 借方残高
                    cell_t5.CellStyle = cstyle_price;
                    if (b_uesen) cell_t5.CellStyle = cstyle_toprice;
                    cell_t5.SetCellValue(L_RTot);
                    ICell cell_t6 = row_t1.CreateCell(6);      // 借方合計
                    cell_t6.CellStyle = cstyle_price;
                    if (b_uesen) cell_t6.CellStyle = cstyle_toprice;
                    cell_t6.SetCellValue(L_RZan);

                    // 試算集計データの明細格納（決算で参照）
                    if (i_kamoku_a > 100 && i_kamoku_a < 200)
                    {
                        Kessanrecord rec_shi = new Kessanrecord();
                        rec_shi.GName = "";
                        rec_shi.IName = s_kname;
                        rec_shi.IData = L_LZan;
                        list_shisan.Add(rec_shi);
                    }
                    if (i_kamoku_a > 200 && i_kamoku_a < 300)
                    {
                        Kessanrecord rec_fusai = new Kessanrecord();
                        rec_fusai.IName = s_kname;
                        rec_fusai.IData = L_RZan;
                        list_fusai.Add(rec_fusai);
                    }
                    if (i_kamoku_a > 300 && i_kamoku_a < 400)
                    {
                        Kessanrecord rec_jshi = new Kessanrecord();
                        rec_jshi.IName = s_kname;
                        rec_jshi.IData = L_RZan;
                        list_junshi.Add(rec_jshi);
                    }
                    if (i_kamoku_a > 400 && i_kamoku_a < 500)
                    {
                        Kessanrecord rec_rieki = new Kessanrecord();
                        rec_rieki.IName = s_kname;
                        rec_rieki.IData = L_RZan;
                        list_shuueki.Add(rec_rieki);
                    }
                    if (i_kamoku_a > 500 && i_kamoku_a < 600)
                    {
                        Kessanrecord rec_hiyou = new Kessanrecord();
                        rec_hiyou.IName = s_kname;
                        rec_hiyou.IData = L_LZan;
                        list_hiyou.Add(rec_hiyou);
                    }

                    // 行明け処理
                    if (b_gyouake)
                    {
                        i_linet++;
                        IRow row_t2 = is_trial.CreateRow(i_linet);
                    }

                    // キー入れ替え
                    i_kamoku_b = i_kamoku_a;
                }

                // 試算集計データの最終処理（決算で参照）
                L_rieki = L_uriage - L_keihi;   // 利益
                L_junshi = L_shihon + L_rieki;  // 純資     

                //t BoxMes("f233 L_rieki=" + L_rieki.ToString() + "\r\nL_junshi=" + L_junshi.ToString());      //t ------
                //t long L_ww = -999;  //t --------------
                //t BoxMes("f233 L_ww=" + L_ww.ToString());      //t -----


                Kessanrecord rec_efusai = new Kessanrecord();
                rec_efusai.GName = "";
                rec_efusai.IName = "TOTAL";
                rec_efusai.IData = L_fusai;
                list_fusai.Add(rec_efusai);

                Kessanrecord rec_spase = new Kessanrecord();
                rec_spase.GName = "";
                rec_spase.IName = "";
                rec_spase.IData = 0;
                list_fusai.Add(rec_spase);

                Kessanrecord rec_ejshi1 = new Kessanrecord();
                rec_ejshi1.GName = "";
                rec_ejshi1.IName = "特別控除前利益";
                rec_ejshi1.IData = L_rieki;
                list_junshi.Add(rec_ejshi1);

                Kessanrecord rec_ejshi2 = new Kessanrecord();
                rec_ejshi2.GName = "";
                rec_ejshi2.IName = "TOTAL";
                rec_ejshi2.IData = L_junshi;
                list_junshi.Add(rec_ejshi2);

                Kessanrecord rec_keihitot = new Kessanrecord();
                rec_keihitot.GName = "";
                rec_keihitot.IName = "TOTAL";
                rec_keihitot.IData = L_keihi;
                list_hiyou.Add(rec_keihitot);

                Kessanrecord rec_spase2 = new Kessanrecord();
                rec_spase2.GName = "";
                rec_spase2.IName = "";
                rec_spase2.IData = 0;
                list_hiyou.Add(rec_spase2);

                Kessanrecord rec_rieki1 = new Kessanrecord();
                rec_rieki1.GName = "(利益）";
                rec_rieki1.IName = "";
                rec_rieki1.IData = 0;
                list_hiyou.Add(rec_rieki1);

                Kessanrecord rec_rieki2 = new Kessanrecord();
                rec_rieki2.IName = "特別控除前利益";
                rec_rieki2.IData = L_rieki;
                list_hiyou.Add(rec_rieki2);

            }

            return true;
        }
        #endregion


        #region 貸借対象表と損益計算書の作成 f234
        public bool f234_kessan()
        {
            // 貸借対象表と損益計算書の作成
            tbox_jstaus.Text = "貸借対象表と損益計算書の作成 開始";
            Task.Delay(300);

            ISheet is_balance = book_exdata.GetSheet("balance");

            // 屋号（会社名）と年度と期間からまでのセット
            IRow row_b01 = is_balance.CreateRow(1);
            ICell cell_b01b = row_b01.CreateCell(1);
            cell_b01b.CellStyle = cstyle_hyou1;
            cell_b01b.SetCellValue(s_hyoud1);
            IRow row_b02 = is_balance.CreateRow(2);
            ICell cell_b02b = row_b02.CreateCell(1);
            cell_b02b.CellStyle = cstyle_hyou2;
            cell_b02b.SetCellValue(s_hyoud2);

            int i_lineb = 3;

            bool b_hida = false;
            bool b_migi = false;

            // 貸借対照表のヘッダー
            i_lineb++;
            IRow row_b1 = is_balance.CreateRow(i_lineb);
            ICell cell_bh1 = row_b1.CreateCell(1);
            cell_bh1.SetCellValue("貸借対照表");
            i_lineb++;
            IRow row_b2 = is_balance.CreateRow(i_lineb);
            for (int ix = 1; ix < 8; ix++)
            {
                ICell cell_bh2 = row_b2.CreateCell(ix);
                cell_bh2.CellStyle = cstyle_wb;
            }

            // 貸借対照表の内容準備
            b_hida = true;
            b_migi = true;
            int i_pos_shisan = 0;
            int i_len_shisan = list_shisan.Count();
            int i_pos_fusai = 0;
            int i_len_fusai = list_fusai.Count();
            int i_pos_junshi = 0;
            int i_len_junshi = list_junshi.Count();
            bool b_fus = true;
            bool b_jun = false;
            while (b_hida || b_migi)
            {
                i_lineb++;
                IRow row_b3 = is_balance.CreateRow(i_lineb);

                // 貸借対照表の左側
                if (b_hida)
                {
                    ICell cell_b3h1 = row_b3.CreateCell(1);
                    ICell cell_b3h2 = row_b3.CreateCell(2);
                    ICell cell_b3h3 = row_b3.CreateCell(3);
                    if (i_pos_shisan < i_len_shisan)
                    {
                        cell_b3h1.SetCellValue(list_shisan[i_pos_shisan].GName);
                        cell_b3h2.SetCellValue(list_shisan[i_pos_shisan].IName);
                        cell_b3h3.CellStyle = cstyle_price;
                        if (list_shisan[i_pos_shisan].IName != "") cell_b3h3.SetCellValue(list_shisan[i_pos_shisan].IData);
                        i_pos_shisan++;
                    }
                    else
                    {
                        b_hida = false;
                    }
                }

                // 貸借対照表の右側
                if (b_migi)
                {
                    ICell cell_b3h5 = row_b3.CreateCell(5);
                    ICell cell_b3h6 = row_b3.CreateCell(6);
                    ICell cell_b3h7 = row_b3.CreateCell(7);
                    if (b_fus)
                    {
                        if (i_pos_fusai < i_len_fusai)
                        {
                            cell_b3h5.SetCellValue(list_fusai[i_pos_fusai].GName);
                            cell_b3h6.SetCellValue(list_fusai[i_pos_fusai].IName);
                            cell_b3h7.CellStyle = cstyle_price;
                            if (list_fusai[i_pos_fusai].IName == "TOTAL")
                            {
                                cell_b3h6.SetCellValue("");
                                cell_b3h7.CellStyle = cstyle_toprice;
                            }
                            if (list_fusai[i_pos_fusai].IName != "") cell_b3h7.SetCellValue(list_fusai[i_pos_fusai].IData);
                            i_pos_fusai++;
                        }
                        else
                        {
                            b_fus = false;
                            b_jun = true;
                        }
                    }
                    if (b_jun)

                    {
                        if (i_pos_junshi < i_len_junshi)
                        {
                            cell_b3h5.SetCellValue(list_junshi[i_pos_junshi].GName);
                            cell_b3h6.SetCellValue(list_junshi[i_pos_junshi].IName);
                            cell_b3h7.CellStyle = cstyle_price;
                            if (list_junshi[i_pos_junshi].IName == "TOTAL")
                            {
                                cell_b3h6.SetCellValue("");
                                cell_b3h7.CellStyle = cstyle_toprice;
                            }
                            if (list_junshi[i_pos_junshi].IName != "") cell_b3h7.SetCellValue(list_junshi[i_pos_junshi].IData);
                            i_pos_junshi++;
                        }
                        else
                        {
                            b_migi = false;
                            b_jun = false;
                        }
                    }
                }
            }

            // 貸借対照表のフッター
            i_lineb++;
            IRow row_b4 = is_balance.CreateRow(i_lineb);
            for (int ix = 1; ix < 8; ix++)
            {
                ICell cell_bf0 = row_b4.CreateCell(ix);
                cell_bf0.CellStyle = cstyle_tostr;
            }
            ICell cell_b4h3 = row_b4.CreateCell(3);
            ICell cell_b4h7 = row_b4.CreateCell(7);
            cell_b4h3.CellStyle = cstyle_toprice;
            cell_b4h7.CellStyle = cstyle_toprice;
            cell_b4h3.SetCellValue(L_shisan);
            cell_b4h7.SetCellValue(L_shisan);

            // 損益計算書のヘッダー  

            i_lineb++;
            i_lineb++;
            IRow row_s1 = is_balance.CreateRow(i_lineb);
            ICell cell_sh1 = row_s1.CreateCell(1);
            cell_sh1.SetCellValue("損益計算書");
            i_lineb++;
            IRow row_s2 = is_balance.CreateRow(i_lineb);
            for (int ix = 1; ix < 8; ix++)
            {
                ICell cell_sh2 = row_s2.CreateCell(ix);
                cell_sh2.CellStyle = cstyle_wb;
            }

            // 損益計算書の内容準備
            b_hida = true;
            b_migi = true;
            int i_pos_hiyou = 0;
            int i_len_hiyou = list_hiyou.Count();
            int i_pos_shuue = 0;
            int i_len_shuue = list_shuueki.Count();
            while (b_hida || b_migi)
            {
                i_lineb++;
                IRow row_s3 = is_balance.CreateRow(i_lineb);

                // 損益計算書の左側
                if (b_hida)
                {
                    ICell cell_s3h1 = row_s3.CreateCell(1);
                    ICell cell_s3h2 = row_s3.CreateCell(2);
                    ICell cell_s3h3 = row_s3.CreateCell(3);
                    if (i_pos_hiyou < i_len_hiyou)
                    {
                        cell_s3h1.SetCellValue(list_hiyou[i_pos_hiyou].GName);
                        cell_s3h2.SetCellValue(list_hiyou[i_pos_hiyou].IName);
                        cell_s3h3.CellStyle = cstyle_price;
                        if (list_hiyou[i_pos_hiyou].IName == "TOTAL")
                        {
                            cell_s3h2.SetCellValue("");
                            cell_s3h3.CellStyle = cstyle_toprice;
                        }                        
                        if (list_hiyou[i_pos_hiyou].IName != "") cell_s3h3.SetCellValue(list_hiyou[i_pos_hiyou].IData);
                        i_pos_hiyou++;
                    }
                    else
                    {
                        b_hida = false;
                    }                
                }

                // 損益計算書の右側
                if (b_hida)
                {
                    ICell cell_s3h5 = row_s3.CreateCell(5);
                    ICell cell_s3h6 = row_s3.CreateCell(6);
                    ICell cell_s3h7 = row_s3.CreateCell(7);
                    if (i_pos_shuue < i_len_shuue)
                    {
                        cell_s3h5.SetCellValue(list_shuueki[i_pos_shuue].GName);
                        cell_s3h6.SetCellValue(list_shuueki[i_pos_shuue].IName);
                        cell_s3h7.CellStyle = cstyle_price;
                        if (list_shuueki[i_pos_shuue].IName != "") cell_s3h7.SetCellValue(list_shuueki[i_pos_shuue].IData);
                        i_pos_shuue++;
                    }
                    else
                    {
                        b_migi = false;
                    }
                }
            }

            // 損益計算書のフッター
            i_lineb++;
            IRow row_s4 = is_balance.CreateRow(i_lineb);
            for (int ix = 1; ix < 8; ix++)
            {
                ICell cell_sf0 = row_s4.CreateCell(ix);
                cell_sf0.CellStyle = cstyle_tostr;
            }
            ICell cell_s4h3 = row_s4.CreateCell(3);
            ICell cell_s4h7 = row_s4.CreateCell(7);
            cell_s4h3.CellStyle = cstyle_toprice;
            cell_s4h7.CellStyle = cstyle_toprice;
            cell_s4h3.SetCellValue(L_uriage);
            cell_s4h7.SetCellValue(L_uriage);

            return true;
        }
        #endregion

        #region サブルーティン
        // メッセージ
        private void BoxMes(string m1)

        {
            string w_mes = m1 + "\r\n確認します。";
            MessageBox.Show(w_mes,
                "voqui3 Exclamation",
                MessageBoxButton.OK,
                MessageBoxImage.Exclamation);
        }
        #endregion


        #region エクセルブックをセーブ f291
        public bool f291_end()
        {
            //  簿記用のエクセルブックをセーブ
            try
            {
                if (File.Exists(s_pfile_exdata))
                {
                    File.Delete(s_pfile_exdata);
                }

                using (FileStream fsd = new FileStream(s_pfile_exdata, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    book_exdata.Write(fsd);
                    fsd.Close();
                }

                    return true;
            }
            catch (Exception ex)
            {
                BoxMes("test" + ex.Message);
                return false;
            }
        }
        #endregion

        #region イベント
        // イベント
        private void loadSubW2(object sender, RoutedEventArgs e)
        {
        }
        #endregion

        private void voqui_CRendered(object sender, EventArgs e)
        {
            bool ans = false;

            ans = f201_start();
            ans = f231_shiwake();
            ans = f232_kanjou();
            ans = f233_shisanhyou();
            ans = f234_kessan();
            ans = f291_end();

            this.Close();
        }
    }


    #region 内部用クラス

    public class Kessanrecord
    {
        public string GName { get; set; }
        public string IName { get; set; }
        public long IData { get; set; }
    }

    #endregion
}
