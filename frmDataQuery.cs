using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace BCM检测工装
{
    public partial class frmDataQuery : Form
    {
        public frmDataQuery()
        {
            InitializeComponent();
            Init_Control();
            Init_Guna_UI2();
        }

        #region 控件初始化
        private void Init_Control()
        {
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            guna2RadioButton1.Checked = true;

            guna2DateTimePicker1.Format = DateTimePickerFormat.Custom;
            guna2DateTimePicker1.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            guna2DateTimePicker2.Format = DateTimePickerFormat.Custom;
            guna2DateTimePicker2.CustomFormat = "yyyy-MM-dd HH:mm:ss";
        }
        private void Init_Guna_UI2()
        {
            //guna2DataGridView初始化
            guna2DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;//显示列标题的所有内容
            guna2DataGridView1.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;//列标题不换行
            guna2DataGridView1.AllowUserToAddRows = false;
            guna2DataGridView1.AllowUserToDeleteRows = false;
            guna2DataGridView1.ReadOnly = true;
            guna2DataGridView1.MultiSelect = false;
            guna2DataGridView1.ColumnHeadersHeight = 30;
            guna2DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            guna2DataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            guna2DataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            guna2DataGridView1.BackgroundColor = ColorTranslator.FromHtml("#D6E4FF");
        }

        #endregion

        #region 按钮操作
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2RadioButton1.Checked)
            {
                ShowData(0, guna2TextBox1.Text);
            }
            else if (guna2RadioButton2.Checked)
            {
                ShowData(1, guna2TextBox2.Text);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            ShowData(guna2DateTimePicker1.Text, guna2DateTimePicker2.Text);
        }
        /// <summary>
        /// 数据导出excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            DataTable dt = guna2DataGridView1.DataSource as DataTable;

            PublicFunction.ExportDataToExcel(dt, DateTime.Now.ToString("D"));
        }

        #endregion

        #region 窗体加载及关闭

        private void frmDataQuery_Load(object sender, EventArgs e)
        {
            guna2TextBox1.SelectAll();
            guna2TextBox1.Focus();//让按钮获得焦点
        }

        private void frmDataQuery_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
        }
        #endregion

        /// <summary>
        /// 显示刷新产品信息数据
        /// </summary>
        private void ShowData(int queryflag, string strInformation)
        {
            //    string cmdText = "select * from ProductInformation where 日期时间>=datetime('now','start of day','+0 day') and 日期时间<datetime('now','start of day','+1 day')";
            string cmdText;
            //定义sql语句文本,查询当日数据显示在表格中
            if (queryflag == 0)
            {
                cmdText = "select * from ProductInformation where 胶水ID=@QrCode";
            }
            else
            {
                cmdText = "select * from ProductInformation where 产品编码1=@QrCode OR 产品编码2=@QrCode OR 产品编码3=@QrCode OR 产品编码4=@QrCode";
            }

            using (SQLiteConnection con = new SQLiteConnection(PublicData.ConnString))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(cmdText, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    //给参数赋值
                    SQLiteParameter[] parameter = new SQLiteParameter[] { new SQLiteParameter("@QrCode", strInformation) };
                    cmd.Parameters.AddRange(parameter);
                    //建立SqlDataAdapter和DataSet对象
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds, "ProductInformation");
                    guna2DataGridView1.DataSource = ds.Tables[0];
                    //使显示的时间格式化
                    for (int i = 0; i < 9; i++)
                    {
                        guna2DataGridView1.Columns[i].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                    }
                    if (this.guna2DataGridView1.Rows.Count > 0)
                    {
                        guna2DataGridView1.FirstDisplayedScrollingRowIndex = this.guna2DataGridView1.Rows.Count - 1;//显示最后一行
                    }
                }
            }
        }

        /// <summary>
        /// 显示刷新产品信息数据
        /// </summary>
        private void ShowData(string date1, string date2)
        {
            string cmdText = "select * from ProductInformation where 第一次回温扫码时间>=datetime(@datetim1) and 第一次回温扫码时间<datetime(@datetim2)";

            using (SQLiteConnection con = new SQLiteConnection(PublicData.ConnString))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(cmdText, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    //给参数赋值
                    SQLiteParameter[] parameter = new SQLiteParameter[] {
                                                                                new SQLiteParameter("@datetim1", date1),
                                                                                new SQLiteParameter("@datetim2", date2)
                                                                         };
                    cmd.Parameters.AddRange(parameter);
                    //建立SqlDataAdapter和DataSet对象
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds, "ProductInformation");
                    guna2DataGridView1.DataSource = ds.Tables[0];
                    //使显示的时间格式化
                    for (int i = 0; i < 9; i++)
                    {
                        guna2DataGridView1.Columns[i].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                    }
                    if (this.guna2DataGridView1.Rows.Count > 0)
                    {
                        guna2DataGridView1.FirstDisplayedScrollingRowIndex = this.guna2DataGridView1.Rows.Count - 1;//显示最后一行
                    }
                }
            }
        }


    }
}
