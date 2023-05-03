using ClassLibrary_FQY;
using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace BCM检测工装
{
    public partial class frmOnline : Form
    {
        private string s = "";

        private int time1Cnt;

        public frmOnline()
        {
            InitializeComponent();
            Init_Control();
            Init_Guna_UI2();
        }

        private void Init_Control()
        {
            timer1.Interval = 100;
            timer2.Interval = 100;
            guna2PictureBox3.Visible = false;
            guna2PictureBox2.Visible = false;
            label2.Text = "未知扫码";
            label2.ForeColor = Color.Blue;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer2.IsSplitterFixed = true;
            splitContainer2.FixedPanel = FixedPanel.Panel1;
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


        private void frmOnline_Load(object sender, EventArgs e)
        {
            timer2.Start();
            GetConfig();
            ShowData();
            guna2TextBox1.SelectAll();
            guna2TextBox1.Focus();//让按钮获得焦点
        }

        private void frmOnline_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
        }


        /// <summary>
        /// 显示刷新产品信息数据
        /// </summary>
        private void ShowData()
        {
            //定义sql语句文本,查询当日数据显示在表格中
            string cmdText = "select 第一次上线扫码时间,第二次上线扫码时间,第三次上线扫码时间,操作人,胶水序号, 胶水ID,胶水状态,胶水回温次数, 胶水回温计时s,胶水常温计时s,产品编码1,产品批次号1,产品数量1,产品备注信息1,产品编码2,产品批次号2,产品数量2,产品备注信息2,产品编码3,产品批次号3,产品数量3,产品备注信息3,产品编码4,产品批次号4,产品数量4,产品备注信息4 from ProductInformation";
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
                    for (int i = 0; i < 3; i++)
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
                        PublicData.Productdatatime4 = reader[3].ToString();//获取第一次上线时间
                        PublicData.Productdatatime5 = reader[4].ToString();//获取第二次上线时间
                        PublicData.Productdatatime6 = reader[5].ToString();//获取第三次上线时间
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
        private void UpdateData(int i, int times)
        {
            if (times == 1)
            {
                //定义sql语句文本,更新表中二维码相同的数据
                string cmdText = "UPDATE ProductInformation SET 第一次上线扫码时间=@DateTime4,操作人=@Name,胶水序号=@Seqnum,胶水状态=@State,胶水回温次数=@Numbers,胶水回温计时s=@BackTimes,胶水常温计时s=@Times,产品编码1=@ProductModel1,产品批次号1=@Batch1,产品数量1=@ProductNumbers1,产品备注信息1=@ProductInformtion1 WHERE 胶水ID=@QrCode";
                //给参数赋值
                SQLiteParameter[] parameter = new SQLiteParameter[] {
                                                                        new SQLiteParameter("@DateTime4", DateTime.Now),
                                                                        new SQLiteParameter("@Name", PublicData.LoginUsername),
                                                                        new SQLiteParameter("@Seqnum", PublicData.GlueSeqNumberList[i]),
                                                                        new SQLiteParameter("@QrCode", PublicData.GlueIDList[i]),
                                                                        new SQLiteParameter("@State", PublicData.GlueStateList[i]),
                                                                        new SQLiteParameter("@Numbers", PublicData.BackTemperatureNumberList[i]),
                                                                        new SQLiteParameter("@BackTimes", PublicData.BackTemperatureTimingList[i]),
                                                                        new SQLiteParameter("@Times", PublicData.NormalTemperatureTimingList[i]),
                                                                        new SQLiteParameter("@ProductModel1", PublicData.ProductInformationStruct.Model),
                                                                        new SQLiteParameter("@Batch1", PublicData.ProductInformationStruct.Batch),
                                                                        new SQLiteParameter("@ProductNumbers1", PublicData.ProductInformationStruct.Numbers),
                                                                        new SQLiteParameter("@ProductInformtion1", PublicData.ProductInformationStruct.Informtion)
                                                                    };
                //执行SQL语句
                SqLiteHelper.ExecuteNonQuery(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);
            }
            else if (times == 2)
            {
                //定义sql语句文本,更新表中二维码相同的数据
                string cmdText = "UPDATE ProductInformation SET 第二次上线扫码时间=@DateTime5,操作人=@Name,胶水序号=@Seqnum,胶水状态=@State,胶水回温次数=@Numbers,胶水回温计时s=@BackTimes,胶水常温计时s=@Times,产品编码2=@ProductModel2,产品批次号2=@Batch2,产品数量2=@ProductNumbers2,产品备注信息2=@ProductInformtion2 WHERE 胶水ID=@QrCode";
                //给参数赋值
                SQLiteParameter[] parameter = new SQLiteParameter[] {
                                                                        new SQLiteParameter("@DateTime5", DateTime.Now),
                                                                        new SQLiteParameter("@Name", PublicData.LoginUsername),
                                                                        new SQLiteParameter("@Seqnum", PublicData.GlueSeqNumberList[i]),
                                                                        new SQLiteParameter("@QrCode", PublicData.GlueIDList[i]),
                                                                        new SQLiteParameter("@State", PublicData.GlueStateList[i]),
                                                                        new SQLiteParameter("@Numbers", PublicData.BackTemperatureNumberList[i]),
                                                                        new SQLiteParameter("@BackTimes", PublicData.BackTemperatureTimingList[i]),
                                                                        new SQLiteParameter("@Times", PublicData.NormalTemperatureTimingList[i]),
                                                                        new SQLiteParameter("@ProductModel2", PublicData.ProductInformationStruct.Model),
                                                                        new SQLiteParameter("@Batch2", PublicData.ProductInformationStruct.Batch),
                                                                        new SQLiteParameter("@ProductNumbers2", PublicData.ProductInformationStruct.Numbers),
                                                                        new SQLiteParameter("@ProductInformtion2", PublicData.ProductInformationStruct.Informtion)
                                                                    };
                //执行SQL语句
                SqLiteHelper.ExecuteNonQuery(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);
            }
            else if (times == 3)
            {
                //定义sql语句文本,更新表中二维码相同的数据
                string cmdText = "UPDATE ProductInformation SET 第三次上线扫码时间=@DateTime6,操作人=@Name,胶水序号=@Seqnum,胶水状态=@State,胶水回温次数=@Numbers,胶水回温计时s=@BackTimes,胶水常温计时s=@Times,产品编码3=@ProductModel3,产品批次号3=@Batch3,产品数量3=@ProductNumbers3,产品备注信息3=@ProductInformtion3 WHERE 胶水ID=@QrCode";
                //给参数赋值
                SQLiteParameter[] parameter = new SQLiteParameter[] {
                                                                        new SQLiteParameter("@DateTime6", DateTime.Now),
                                                                        new SQLiteParameter("@Name", PublicData.LoginUsername),
                                                                        new SQLiteParameter("@Seqnum", PublicData.GlueSeqNumberList[i]),
                                                                        new SQLiteParameter("@QrCode", PublicData.GlueIDList[i]),
                                                                        new SQLiteParameter("@State", PublicData.GlueStateList[i]),
                                                                        new SQLiteParameter("@Numbers", PublicData.BackTemperatureNumberList[i]),
                                                                        new SQLiteParameter("@BackTimes", PublicData.BackTemperatureTimingList[i]),
                                                                        new SQLiteParameter("@Times", PublicData.NormalTemperatureTimingList[i]),
                                                                        new SQLiteParameter("@ProductModel3", PublicData.ProductInformationStruct.Model),
                                                                        new SQLiteParameter("@Batch3", PublicData.ProductInformationStruct.Batch),
                                                                        new SQLiteParameter("@ProductNumbers3", PublicData.ProductInformationStruct.Numbers),
                                                                        new SQLiteParameter("@ProductInformtion3", PublicData.ProductInformationStruct.Informtion)
                                                                    };
                //执行SQL语句
                SqLiteHelper.ExecuteNonQuery(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);
            }
            else if (times == 4)
            {
                //定义sql语句文本,更新表中二维码相同的数据
                string cmdText = "UPDATE ProductInformation SET 产品编码4=@ProductModel4,产品批次号4=@Batch4,产品数量4=@ProductNumbers4,产品备注信息4=@ProductInformtion4 WHERE 胶水ID=@QrCode";
                //给参数赋值
                SQLiteParameter[] parameter = new SQLiteParameter[] {
                                                                        new SQLiteParameter("@QrCode", guna2TextBox1.Text),
                                                                        new SQLiteParameter("@ProductModel4", PublicData.ProductInformationStruct.Model),
                                                                        new SQLiteParameter("@Batch4", PublicData.ProductInformationStruct.Batch),
                                                                        new SQLiteParameter("@ProductNumbers4", PublicData.ProductInformationStruct.Numbers),
                                                                        new SQLiteParameter("@ProductInformtion4", PublicData.ProductInformationStruct.Informtion)
                                                                    };
                //执行SQL语句
                SqLiteHelper.ExecuteNonQuery(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);
            }
            else
            {
                //定义sql语句文本,更新表中二维码相同的数据
                string cmdText = "UPDATE ProductInformation SET 操作人=@Name,胶水序号=@Seqnum,胶水状态=@State,胶水回温次数=@Numbers,胶水回温计时s=@BackTimes,胶水常温计时s=@Times,产品编码=@ProductModel,产品批次号=@Batch,产品数量=@ProductNumbers,产品备注信息=@ProductInformtion WHERE 胶水ID=@QrCode";
                //给参数赋值
                SQLiteParameter[] parameter = new SQLiteParameter[] {
                                                                        new SQLiteParameter("@Name", PublicData.LoginUsername),
                                                                        new SQLiteParameter("@Seqnum", PublicData.GlueSeqNumberList[i]),
                                                                        new SQLiteParameter("@QrCode", PublicData.GlueIDList[i]),
                                                                        new SQLiteParameter("@State", PublicData.GlueStateList[i]),
                                                                        new SQLiteParameter("@Numbers", PublicData.BackTemperatureNumberList[i]),
                                                                        new SQLiteParameter("@BackTimes", PublicData.BackTemperatureTimingList[i]),
                                                                        new SQLiteParameter("@Times", PublicData.NormalTemperatureTimingList[i]),
                                                                        new SQLiteParameter("@ProductModel", PublicData.ProductInformationStruct.Model),
                                                                        new SQLiteParameter("@Batch", PublicData.ProductInformationStruct.Batch),
                                                                        new SQLiteParameter("@ProductNumbers", PublicData.ProductInformationStruct.Numbers),
                                                                        new SQLiteParameter("@ProductInformtion", PublicData.ProductInformationStruct.Informtion)
                                                                    };
                //执行SQL语句
                SqLiteHelper.ExecuteNonQuery(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);
            }
        }


        /// <summary>
        /// 检索数据库中是否存在未使用完且合格的胶水
        /// 若没有检索到符合条件的则可以使用新胶水；否则必须先使用完已使用的胶水后再允许使用新胶水
        /// </summary>
        /// <returns>允许使用新胶水返回true，否则返回false</returns>
        private int RetrievalData()
        {
            int DataCounts = 0;
            //定义sql语句文本,向表中插入新数据
            string cmdText = "SELECT * FROM ProductInformation WHERE 胶水序号=1";
            using (SQLiteConnection con = new SQLiteConnection(PublicData.ConnString))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(cmdText, con))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        DataCounts++;
                    }
                }
            }
            return DataCounts;
        }
        /// <summary>
        /// 产品信息录入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            try
            {
                PublicData.ProductInformationStruct.Model = guna2TextBox2.Text;
                PublicData.ProductInformationStruct.Batch = guna2TextBox3.Text;
                PublicData.ProductInformationStruct.Numbers = int.Parse(guna2TextBox4.Text);
                PublicData.ProductInformationStruct.Informtion = guna2TextBox5.Text;

                if (guna2TextBox2.Text.Length == PublicData.ProductModelLengthSet && guna2TextBox3.Text.Length == PublicData.ProductTimesLengthSet)
                {
                    PublicData.Writeflag = true;//录入标志

                    guna2TextBox1.SelectAll();
                    guna2TextBox1.Focus();//让按钮获得焦点
                    SetConfig();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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

                if (PublicData.Writeflag)
                {
                    PublicData.ProductCode = s;

                    if (QueryData(PublicData.ProductCode))//查询到数据库中已存在胶水信息
                    {
                        if (PublicData.ProductSeqNumber == 1)//上线必须是最先回温的胶水
                        {
                            PublicData.GlueIDList[0] = PublicData.ProductCode;
                            PublicData.GlueSeqNumberList[0] = PublicData.ProductSeqNumber;
                            PublicData.GlueStateList[0] = PublicData.ProductState;
                            PublicData.BackTemperatureNumberList[0] = PublicData.ProductTimes;
                            PublicData.BackTemperatureTimingList[0] = PublicData.ProductBackTemperatureTimes;
                            PublicData.NormalTemperatureTimingList[0] = PublicData.ProductNormalTemperatureTimes;


                            if (PublicData.GlueStateList[0] == 0)
                            {
                                //当胶水回温时间超过回温预设值
                                if (PublicData.BackTemperatureCompleteFlag1)
                                {
                                    ResultPictureShow(1);
                                    if (PublicData.BackTemperatureNumberList[0] == 1)
                                    {
                                        UpdateData(0, 1);
                                    }
                                    else if (PublicData.BackTemperatureNumberList[0] == 2)
                                    {
                                        if (PublicData.Productdatatime4 == "")
                                        {
                                            UpdateData(0, 1);
                                        }
                                        else
                                        {
                                            UpdateData(0, 2);
                                        }

                                    }
                                    else if (PublicData.BackTemperatureNumberList[0] == 3)
                                    {
                                        if (PublicData.Productdatatime4 == "")
                                        {
                                            UpdateData(0, 1);
                                        }
                                        else if (PublicData.Productdatatime5 == "")
                                        {
                                            UpdateData(0, 2);
                                        }
                                        else
                                        {
                                            UpdateData(0, 3);
                                        }

                                    }
                                    else
                                    {
                                        UpdateData(0, 0);
                                    }
                                    timer1.Start();
                                }
                                else
                                {
                                    ResultPictureShow(2);
                                }
                            }
                            else
                            {
                                ResultPictureShow(4);
                            }
                        }
                        else if (PublicData.ProductSeqNumber == 0)
                        {
                            ResultPictureShow(4);
                        }
                        else
                        {
                            if (RetrievalData() > 0)
                            {
                                ResultPictureShow(5);
                            }
                            else
                            {
                                PublicData.GlueIDList[1] = PublicData.ProductCode;
                                PublicData.GlueSeqNumberList[1] = PublicData.ProductSeqNumber;
                                PublicData.GlueStateList[1] = PublicData.ProductState;
                                PublicData.BackTemperatureNumberList[1] = PublicData.ProductTimes;
                                PublicData.BackTemperatureTimingList[1] = PublicData.ProductBackTemperatureTimes;
                                PublicData.NormalTemperatureTimingList[1] = PublicData.ProductNormalTemperatureTimes;

                                if (PublicData.GlueStateList[1] == 0)
                                {
                                    //当胶水回温时间超过回温预设值
                                    if (PublicData.BackTemperatureCompleteFlag2)
                                    {
                                        ResultPictureShow(1);
                                        if (PublicData.BackTemperatureNumberList[1] == 1)
                                        {
                                            UpdateData(1, 1);
                                        }
                                        else if (PublicData.BackTemperatureNumberList[1] == 2)
                                        {
                                            if (PublicData.Productdatatime4 == "")
                                            {
                                                UpdateData(1, 1);
                                            }
                                            else
                                            {
                                                UpdateData(1, 2);
                                            }

                                        }
                                        else if (PublicData.BackTemperatureNumberList[1] == 3)
                                        {
                                            if (PublicData.Productdatatime4 == "")
                                            {
                                                UpdateData(1, 1);
                                            }
                                            else if (PublicData.Productdatatime5 == "")
                                            {
                                                UpdateData(1, 2);
                                            }
                                            else
                                            {
                                                UpdateData(1, 3);
                                            }

                                        }
                                        else
                                        {
                                            UpdateData(1, 0);
                                        }
                                        timer1.Start();
                                    }
                                    else
                                    {
                                        ResultPictureShow(2);
                                    }
                                }
                                else
                                {
                                    ResultPictureShow(4);
                                }
                            }
                        }
                    }
                    else
                    {
                        ResultPictureShow(3);
                    }
                    ShowData();

                }
                else
                {
                    ResultPictureShow(0);
                }
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
                    label2.Text = "扫码成功";
                    label2.ForeColor = Color.LimeGreen;
                    break;
                case 2://扫码异常
                    guna2PictureBox1.Visible = false;
                    guna2PictureBox2.Visible = false;
                    guna2PictureBox3.Visible = true;
                    label2.Text = "胶水回温异常";
                    label2.ForeColor = Color.Red;
                    break;
                case 3://未知扫码
                    guna2PictureBox1.Visible = true;
                    guna2PictureBox2.Visible = false;
                    guna2PictureBox3.Visible = false;
                    label2.Text = "未查询到胶水信息";
                    label2.ForeColor = Color.Blue;
                    break;
                case 4://扫码异常
                    guna2PictureBox1.Visible = false;
                    guna2PictureBox2.Visible = false;
                    guna2PictureBox3.Visible = true;
                    label2.Text = "胶水已失效";
                    label2.ForeColor = Color.Red;
                    break;
                case 5://存在未使用完胶水
                    guna2PictureBox1.Visible = false;
                    guna2PictureBox2.Visible = false;
                    guna2PictureBox3.Visible = true;
                    label2.Text = "存在未使用完胶水";
                    label2.ForeColor = Color.Red;
                    break;
                default:
                    guna2PictureBox1.Visible = false;
                    guna2PictureBox2.Visible = false;
                    guna2PictureBox3.Visible = true;
                    label2.Text = "未录入产品信息";
                    label2.ForeColor = Color.Red;
                    break;
            }
        }



        /// <summary>
        /// 保存配置
        /// </summary>
        private void SetConfig()
        {
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("产品信息", "编码", guna2TextBox2.Text, PublicData.IniPath);
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("产品信息", "批次号", guna2TextBox3.Text, PublicData.IniPath);
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("产品信息", "数量", guna2TextBox4.Text, PublicData.IniPath);
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("产品信息", "信息", guna2TextBox5.Text, PublicData.IniPath);
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        private void GetConfig()
        {
            guna2TextBox2.Text = ClassLibrary_FQY.INIFilesHelper.IniReadValue("产品信息", "编码", PublicData.IniPath);
            guna2TextBox3.Text = ClassLibrary_FQY.INIFilesHelper.IniReadValue("产品信息", "批次号", PublicData.IniPath);
            guna2TextBox4.Text = ClassLibrary_FQY.INIFilesHelper.IniReadValue("产品信息", "数量", PublicData.IniPath);
            guna2TextBox5.Text = ClassLibrary_FQY.INIFilesHelper.IniReadValue("产品信息", "信息", PublicData.IniPath);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!PublicData.WorkStation1)
            {
                time1Cnt++;
                if (time1Cnt < 5)
                {
                    PublicData.sendWirteflag = 10;//继电器1关闭
                }
                else if (time1Cnt < 10)
                {
                    PublicData.sendWirteflag = 31;//继电器3打开
                }
                else
                {
                    time1Cnt = 0;
                    timer1.Stop();
                }
            }
            else if (!PublicData.WorkStation2)
            {
                time1Cnt++;
                if (time1Cnt < 5)
                {
                    PublicData.sendWirteflag = 20;//继电器2关闭
                }
                else if (time1Cnt < 10)
                {
                    PublicData.sendWirteflag = 31;//继电器3打开
                }
                else
                {
                    time1Cnt = 0;
                    timer1.Stop();
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (PublicData.Writeflag)
            {
                this.guna2Button1.Enabled = false;
            }
            else
            {
                this.guna2Button1.Enabled = true;
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            try
            {
                PublicData.ProductInformationStruct.Model = guna2TextBox2.Text;
                PublicData.ProductInformationStruct.Batch = guna2TextBox3.Text;
                PublicData.ProductInformationStruct.Numbers = int.Parse(guna2TextBox4.Text);
                PublicData.ProductInformationStruct.Informtion = guna2TextBox5.Text;

                SetConfig();

                UpdateData(0, 4);
                ShowData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
