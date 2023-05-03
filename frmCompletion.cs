using ClassLibrary_FQY;
using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace BCM检测工装
{
    public partial class frmCompletion : Form
    {
        private string s = "";
        private int time1Cnt;
        public frmCompletion()
        {
            InitializeComponent();
            Init_Control();
            Init_Guna_UI2();
        }


        private void Init_Control()
        {
            timer1.Interval = 100;
            guna2PictureBox3.Visible = false;
            guna2PictureBox2.Visible = false;
            label2.Text = "未知扫码";
            label2.ForeColor = Color.Blue;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
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

        private void frmCompletion_Load(object sender, EventArgs e)
        {
            ShowData();
            guna2TextBox1.SelectAll();
            guna2TextBox1.Focus();//让按钮获得焦点
        }

        private void frmCompletion_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
        }


        /// <summary>
        /// 显示刷新产品信息数据
        /// </summary>
        private void ShowData()
        {
            //定义sql语句文本,查询当日数据显示在表格中
            string cmdText = "select 用毕扫码时间,操作人,胶水序号, 胶水ID,胶水状态,胶水回温次数, 胶水回温计时s,胶水常温计时s from ProductInformation";
            using (SQLiteConnection con = new SQLiteConnection(PublicData.ConnString))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(cmdText, con))
                {

                    //建立SqlDataAdapter和DataSet对象
                    SQLiteDataAdapter sda = new SQLiteDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds, "ProductInformation");
                    guna2DataGridView1.DataSource = ds.Tables[0];
                    //使显示的时间格式化
                    for (int i = 0; i < 1; i++)
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
        /// 查询数据表中胶水信息
        /// </summary>
        /// <param name="strcode"></param>
        /// <returns></returns>
        private bool QueryData(string strcode)
        {
            bool result = false;
            //定义sql语句文本,向表中插入新数据
            string cmdText = "SELECT * FROM ProductInformation WHERE 胶水ID=@QrCode";

            using (SQLiteConnection con = new SQLiteConnection(PublicData.ConnString))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(cmdText, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    //给参数赋值
                    SQLiteParameter[] parameter = new SQLiteParameter[] { new SQLiteParameter("@QrCode", strcode) };
                    cmd.Parameters.AddRange(parameter);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PublicData.ProductSeqNumber = Convert.ToInt32(reader[10]);//获取胶水序号
                        PublicData.ProductState = Convert.ToInt32(reader[12]);//获取胶水状态
                        PublicData.ProductTimes = Convert.ToInt32(reader[13]);//获取胶水回温次数
                        PublicData.ProductBackTemperatureTimes = Convert.ToInt32(reader[14]);//获取胶水回温计时时间
                        PublicData.ProductNormalTemperatureTimes = Convert.ToInt32(reader[15]);//获取胶水常温计时时间

                        result = true;
                    }
                }
            }
            return result;
        }
        private void UpdateData(int i)
        {
            //定义sql语句文本,更新表中二维码相同的数据
            string cmdText = "UPDATE ProductInformation SET 用毕扫码时间=@DateTime7,操作人=@Name,胶水序号=@Seqnum,胶水状态=@State,胶水回温次数=@Numbers,胶水回温计时s=@BackTimes,胶水常温计时s=@Times WHERE 胶水ID=@QrCode";
            //给参数赋值
            SQLiteParameter[] parameter = new SQLiteParameter[] {
                                                                    new SQLiteParameter("@DateTime7", DateTime.Now),
                                                                    new SQLiteParameter("@Name", PublicData.LoginUsername),
                                                                    new SQLiteParameter("@Seqnum", PublicData.GlueSeqNumberList[i]),
                                                                    new SQLiteParameter("@QrCode", PublicData.GlueIDList[i]),
                                                                    new SQLiteParameter("@State", PublicData.GlueStateList[i]),
                                                                    new SQLiteParameter("@Numbers", PublicData.BackTemperatureNumberList[i]),
                                                                    new SQLiteParameter("@BackTimes", PublicData.BackTemperatureTimingList[i]),
                                                                    new SQLiteParameter("@Times", PublicData.NormalTemperatureTimingList[i])
                                                                };
            //执行SQL语句
            SqLiteHelper.ExecuteNonQuery(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            s = guna2TextBox1.Text;
        }

        private void guna2TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                guna2TextBox1.SelectAll();
                PublicData.ProductCode = s;

                if (QueryData(PublicData.ProductCode))//查询到数据库中已存在胶水信息
                {
                    if (PublicData.ProductSeqNumber == 1)
                    {
                        PublicData.GlueIDList[0] = PublicData.ProductCode;
                        PublicData.GlueSeqNumberList[0] = PublicData.ProductSeqNumber;
                        PublicData.GlueStateList[0] = PublicData.ProductState;
                        PublicData.BackTemperatureNumberList[0] = PublicData.ProductTimes;
                        PublicData.BackTemperatureTimingList[0] = PublicData.ProductBackTemperatureTimes;
                        PublicData.NormalTemperatureTimingList[0] = PublicData.ProductNormalTemperatureTimes;

                        if (PublicData.GlueStateList[0] != 1)
                        {
                            ResultPictureShow(1);
                            PublicData.ReturnContainerFlag1 = true;
                            PublicData.Writeflag = false;
                            PublicData.GlueSeqNumberList[0] = 0;//赋胶水序号
                            PublicData.GlueStateList[0] = 1;
                            UpdateData(0);
                            timer1.Start();
                        }
                        else
                        {
                            ResultPictureShow(2);
                            PublicData.ReturnContainerFlag1 = true;
                            PublicData.GlueSeqNumberList[0] = 0;//赋胶水序号
                            UpdateData(0);
                        }
                    }
                    else if (PublicData.ProductSeqNumber == 2)
                    {
                        PublicData.GlueIDList[1] = PublicData.ProductCode;
                        PublicData.GlueSeqNumberList[1] = PublicData.ProductSeqNumber;
                        PublicData.GlueStateList[1] = PublicData.ProductState;
                        PublicData.BackTemperatureNumberList[1] = PublicData.ProductTimes;
                        PublicData.BackTemperatureTimingList[1] = PublicData.ProductBackTemperatureTimes;
                        PublicData.NormalTemperatureTimingList[1] = PublicData.ProductNormalTemperatureTimes;

                        if (PublicData.GlueStateList[1] != 1)
                        {
                            ResultPictureShow(1);
                            PublicData.ReturnContainerFlag2 = true;
                            PublicData.Writeflag = false;
                            PublicData.GlueSeqNumberList[1] = 0;//赋胶水序号
                            PublicData.GlueStateList[1] = 1;
                            UpdateData(1);
                            timer1.Start();
                        }
                        else
                        {
                            ResultPictureShow(2);
                            PublicData.ReturnContainerFlag2 = true;
                            PublicData.GlueSeqNumberList[1] = 0;//赋胶水序号
                            UpdateData(1);
                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    ResultPictureShow(3);
                }
                ShowData();
            }
        }


        /// <summary>
        /// 显示结果刷新
        /// </summary>
        /// <param name="num"></param>
        private void ResultPictureShow(int num)
        {
            switch (num)
            {
                case 1://扫码正常
                    guna2PictureBox1.Visible = false;
                    guna2PictureBox2.Visible = true;
                    guna2PictureBox3.Visible = false;
                    label2.Text = "报废完成";
                    label2.ForeColor = Color.LimeGreen;
                    break;
                case 2://扫码异常
                    guna2PictureBox1.Visible = false;
                    guna2PictureBox2.Visible = false;
                    guna2PictureBox3.Visible = true;
                    label2.Text = "胶水已失效";
                    label2.ForeColor = Color.Red;
                    break;
                case 3://未知扫码
                    guna2PictureBox1.Visible = true;
                    guna2PictureBox2.Visible = false;
                    guna2PictureBox3.Visible = false;
                    label2.Text = "未知扫码";
                    label2.ForeColor = Color.Blue;
                    break;
                default:
                    guna2PictureBox1.Visible = false;
                    guna2PictureBox2.Visible = false;
                    guna2PictureBox3.Visible = true;
                    label2.Text = "存在未使用完的胶水";
                    label2.ForeColor = Color.Red;
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time1Cnt++;
            if (time1Cnt < 10)
            {
                PublicData.sendWirteflag = 30;//继电器3关闭
            }
            else
            {
                time1Cnt = 0;
                timer1.Stop();
            }
        }
    }
}
