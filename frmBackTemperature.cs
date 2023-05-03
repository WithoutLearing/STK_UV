using ClassLibrary_FQY;
using System;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Windows.Forms;

namespace BCM检测工装
{
    public partial class frmBackTemperature : Form
    {
        private string s = "";
        private int time2Cnt1;
        private int time2Cnt2;
        private MillisecondTimer timer1s;
        private MillisecondTimer timer2s;

        public frmBackTemperature()
        {
            InitializeComponent();
            Init_Control();
            Init_Guna_UI2();
            Init_Timer();
        }

        #region 控件初始化
        private void Init_Control()
        {
            timer1.Interval = 1000;
            timer2.Interval = 100;
            timer3.Interval = 1000;
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

        private void Init_Timer()
        {
            timer1s = new MillisecondTimer();
            timer2s = new MillisecondTimer();
            timer1s.Interval = 1000;
            timer2s.Interval = 1000;
            timer1s.Tick += Timer1s_Tick;
            timer2s.Tick += Timer2s_Tick;
        }



        #endregion

        #region 精确定时器
        private void Timer1s_Tick(object sender, EventArgs e)
        {
            if (PublicData.GlueSeqNumberList.Count > 0)
            {
                if (PublicData.GlueSeqNumberList[0] != 0)
                {
                    PublicData.BackTemperatureTimingList[0]++;
                    PublicData.NormalTemperatureTimingList[0]++;
                }
                //回温计时到达回温时间设定后，计时值将不再变化
                if (PublicData.BackTemperatureTimingList[0] >= PublicData.BackTemperatureTimeSet)
                {
                    PublicData.BackTemperatureCompleteFlag1 = true;
                    PublicData.BackTemperatureFlag1 = false;
                    PublicData.BackTemperatureTimingList[0] = PublicData.BackTemperatureTimeSet;
                }
                UpdateData_Real(0);
                if (this.InvokeRequired)
                {
                    this.Invoke(new EventHandler(delegate
                    {

                        ShowData();
                    }));

                }
            }
        }

        private void Timer2s_Tick(object sender, EventArgs e)
        {
            if (PublicData.GlueSeqNumberList.Count > 1)
            {
                if (PublicData.GlueSeqNumberList[1] != 0)
                {
                    PublicData.BackTemperatureTimingList[1]++;
                    PublicData.NormalTemperatureTimingList[1]++;
                }
                //回温计时到达回温时间设定后，计时值将不再变化
                if (PublicData.BackTemperatureTimingList[1] >= PublicData.BackTemperatureTimeSet)
                {
                    PublicData.BackTemperatureCompleteFlag2 = true;
                    PublicData.BackTemperatureFlag2 = false;
                    PublicData.BackTemperatureTimingList[1] = PublicData.BackTemperatureTimeSet;
                }
                UpdateData_Real(1);
                if (this.InvokeRequired)
                {
                    this.Invoke(new EventHandler(delegate
                    {

                        ShowData();
                    }));

                }
            }
        }

        #endregion

        private void frmBackTemperature_Load(object sender, EventArgs e)
        {
            //获取未使用完的胶水数量
            int count = FirstTimeRetrievalData();
            if (count > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    UpdateData(i, 0);
                }
            }
            else if (count > 0)
            {
                PublicData.GlueSeqNumberList[count - 1] = 1;
                UpdateData(count - 1, 0);
            }

            ShowData();
            guna2TextBox1.SelectAll();
            guna2TextBox1.Focus();//让按钮获得焦点
            timer2.Start();

        }
        private void frmBackTemperature_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
        }

        #region 定时器
        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (PublicData.ReturnContainerFlag1)
            {
                timer1s.Stop();
                PublicData.ReturnContainerFlag1 = false;
                PublicData.BackTemperatureFlag1 = false;
                PublicData.NormalTemperatureFlag1 = false;
                PublicData.BackTemperatureCompleteFlag1 = false;
                PublicData.BackTemperatureSendCnt1 = 0;
            }

            if (PublicData.ReturnContainerFlag2)
            {
                timer2s.Stop();
                PublicData.ReturnContainerFlag2 = false;
                PublicData.BackTemperatureFlag2 = false;
                PublicData.NormalTemperatureFlag2 = false;
                PublicData.BackTemperatureCompleteFlag2 = false;
                PublicData.BackTemperatureSendCnt2 = 0;
            }

            //当回温时间到达时，发送命令（3次）使继电器1打开
            if (PublicData.BackTemperatureCompleteFlag1 && PublicData.BackTemperatureSendCnt1 < 3)
            {
                PublicData.BackTemperatureSendCnt1++;
                //判断哪个坑里胶水回温好了
                if (PublicData.WorkStation1)
                {
                    PublicData.sendWirteflag = 11;//继电器1打开
                }
                else if (PublicData.WorkStation2)
                {
                    PublicData.sendWirteflag = 21;//继电器2打开
                }
            }

            if (PublicData.BackTemperatureCompleteFlag2 && PublicData.BackTemperatureSendCnt2 < 3)
            {
                PublicData.BackTemperatureSendCnt2++;
                //判断哪个坑里胶水回温好了
                if (PublicData.WorkStation2)
                {
                    PublicData.sendWirteflag = 21;//继电器2打开
                }
                else if (PublicData.WorkStation1)
                {
                    PublicData.sendWirteflag = 11;//继电器1打开
                }
            }

            if (PublicData.NormalTemperatureTimingList.Count > 0)
            {
                //当胶水常温计时到达设定值后，胶水状态更新为报废且阻挡器回弹
                if (PublicData.NormalTemperatureTimingList[0] >= PublicData.NormalTemperatureLimitTimeSet)
                {
                    if (time2Cnt1 < 5)
                    {
                        time2Cnt1++;
                        PublicData.sendWirteflag = 30;//继电器3关闭

                        //   PublicData.GlueSeqNumberList[0] = 0;//赋胶水序号
                        PublicData.GlueStateList[0] = 1;//赋胶水状态
                        UpdateData(0, 0);
                        timer1s.Stop();
                        PublicData.Writeflag = false;
                        PublicData.ReturnContainerFlag1 = true;
                    }
                }
                else
                {
                    time2Cnt1 = 0;
                }


            }

            if (PublicData.NormalTemperatureTimingList.Count > 1)
            {
                if (PublicData.NormalTemperatureTimingList[1] >= PublicData.NormalTemperatureLimitTimeSet)
                {
                    if (time2Cnt2 < 5)
                    {
                        time2Cnt2++;
                        PublicData.sendWirteflag = 30;//继电器3关闭

                        //   PublicData.GlueSeqNumberList[1] = 0;//赋胶水序号
                        PublicData.GlueStateList[1] = 1;//赋胶水状态
                        UpdateData(1, 0);
                        timer2s.Stop();
                        PublicData.Writeflag = false;
                        PublicData.ReturnContainerFlag2 = true;
                    }
                }
                else
                {
                    time2Cnt2 = 0;
                }
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {

        }
        #endregion

        #region 获取扫码枪扫码信息
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            s = guna2TextBox1.Text;
        }

        private void guna2TextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (s.Length == PublicData.GlueIDLengthSet)
                {
                    guna2TextBox1.SelectAll();

                    PublicData.ProductCode = s;

                    if (QueryData(PublicData.ProductCode))//查询到数据库中已存在胶水信息
                    {
                        if (PublicData.ProductSeqNumber == 1)//最先回温的胶水
                        {
                            PublicData.GlueIDList[0] = PublicData.ProductCode;
                            PublicData.GlueSeqNumberList[0] = PublicData.ProductSeqNumber;
                            PublicData.GlueStateList[0] = PublicData.ProductState;
                            PublicData.BackTemperatureNumberList[0] = PublicData.ProductTimes;
                            PublicData.BackTemperatureTimingList[0] = PublicData.ProductBackTemperatureTimes;
                            PublicData.NormalTemperatureTimingList[0] = PublicData.ProductNormalTemperatureTimes;


                            //当胶水回温次数到达3次或常温时间超过（60-2）h
                            if (PublicData.ProductTimes >= PublicData.WorkLimitTimesSet || PublicData.ProductNormalTemperatureTimes >= (PublicData.NormalTemperatureLimitTimeSet - PublicData.BackTemperatureTimeSet))
                            {
                                if (PublicData.GlueStateList[0] == 0)
                                {
                                    PublicData.GlueSeqNumberList[0] = 0;//赋胶水序号
                                    PublicData.GlueStateList[0] = 1;//赋胶水状态
                                    UpdateData(0, PublicData.BackTemperatureNumberList[0]);
                                }

                            }

                            if (PublicData.GlueStateList[0] == 0)
                            {
                                if (PublicData.BackTemperatureNumberList[0] == 1)
                                {
                                    if (PublicData.Productdatatime8 != "")
                                    {
                                        ResultPictureShow(1);
                                        PublicData.BackTemperatureNumberList[0]++;
                                        PublicData.BackTemperatureTimingList[0] = 0;
                                        UpdateData(0, PublicData.BackTemperatureNumberList[0]);
                                        timer1s.Start();
                                        PublicData.BackTemperatureFlag1 = true;
                                        PublicData.NormalTemperatureFlag1 = true;
                                    }
                                }
                                else if (PublicData.BackTemperatureNumberList[0] == 2)
                                {
                                    if (PublicData.Productdatatime9 != "")
                                    {
                                        ResultPictureShow(1);
                                        PublicData.BackTemperatureNumberList[0]++;
                                        PublicData.BackTemperatureTimingList[0] = 0;
                                        UpdateData(0, PublicData.BackTemperatureNumberList[0]);
                                        timer1s.Start();
                                        PublicData.BackTemperatureFlag1 = true;
                                        PublicData.NormalTemperatureFlag1 = true;
                                    }
                                }
                            }
                            else
                            {
                                ResultPictureShow(2);
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

                            //当胶水回温次数到达3次或常温时间超过（60-2）h
                            if (PublicData.ProductTimes >= PublicData.WorkLimitTimesSet || PublicData.ProductNormalTemperatureTimes >= (PublicData.NormalTemperatureLimitTimeSet - PublicData.BackTemperatureTimeSet))
                            {
                                if (PublicData.GlueStateList[1] == 0)
                                {
                                    PublicData.GlueSeqNumberList[1] = 0;//赋胶水序号
                                    PublicData.GlueStateList[1] = 1;//赋胶水状态
                                    UpdateData(1, PublicData.BackTemperatureNumberList[1]);
                                }

                            }

                            if (PublicData.GlueStateList[1] == 0)
                            {
                                if (PublicData.BackTemperatureNumberList[1] == 1)
                                {
                                    if (PublicData.Productdatatime8 != "")
                                    {
                                        ResultPictureShow(1);
                                        PublicData.BackTemperatureNumberList[1]++;
                                        PublicData.BackTemperatureTimingList[1] = 0;
                                        UpdateData(1, PublicData.BackTemperatureNumberList[1]);
                                        timer2s.Start();
                                        PublicData.BackTemperatureFlag2 = true;
                                        PublicData.NormalTemperatureFlag2 = true;
                                    }
                                }
                                else if (PublicData.BackTemperatureNumberList[1] == 2)
                                {
                                    if (PublicData.Productdatatime9 != "")
                                    {
                                        ResultPictureShow(1);
                                        PublicData.BackTemperatureNumberList[1]++;
                                        PublicData.BackTemperatureTimingList[1] = 0;
                                        UpdateData(1, PublicData.BackTemperatureNumberList[1]);
                                        timer2s.Start();
                                        PublicData.BackTemperatureFlag2 = true;
                                        PublicData.NormalTemperatureFlag2 = true;
                                    }
                                }
                            }
                            else
                            {
                                ResultPictureShow(2);
                            }
                        }
                        else
                        {
                            ResultPictureShow(2);//扫码异常
                        }
                    }
                    else//使用新胶水时
                    {
                        int count = RetrievalData();
                        if (count < 1)//首先检索数据库中是否未使用完的胶水
                        {
                            ResultPictureShow(1);

                            PublicData.GlueSeqNumberList.Clear();
                            PublicData.GlueIDList.Clear();
                            PublicData.GlueStateList.Clear();
                            PublicData.BackTemperatureNumberList.Clear();
                            PublicData.BackTemperatureTimingList.Clear();
                            PublicData.NormalTemperatureTimingList.Clear();

                            PublicData.GlueSeqNumberList.Add(0);
                            PublicData.GlueIDList.Add(PublicData.ProductCode);
                            PublicData.BackTemperatureTimingList.Add(0);
                            PublicData.NormalTemperatureTimingList.Add(0);
                            PublicData.BackTemperatureNumberList.Add(0);
                            PublicData.GlueStateList.Add(0);

                            PublicData.BackTemperatureNumberList[PublicData.GlueIDList.Count - 1]++;
                            PublicData.GlueSeqNumberList[PublicData.GlueIDList.Count - 1] = PublicData.GlueIDList.Count;

                            InsertData(PublicData.GlueIDList.Count - 1);

                            timer1s.Start();
                            PublicData.BackTemperatureFlag1 = true;
                            PublicData.NormalTemperatureFlag1 = true;
                        }
                        else if (count < 2)
                        {
                            ResultPictureShow(1);

                            PublicData.GlueSeqNumberList.Clear();
                            PublicData.GlueIDList.Clear();
                            PublicData.GlueStateList.Clear();
                            PublicData.BackTemperatureNumberList.Clear();
                            PublicData.BackTemperatureTimingList.Clear();
                            PublicData.NormalTemperatureTimingList.Clear();

                            PublicData.GlueSeqNumberList.Add(1);//将之前的胶水序号置1
                            PublicData.GlueIDList.Add(PublicData.ProductID);
                            PublicData.GlueStateList.Add(PublicData.ProductState);
                            PublicData.BackTemperatureNumberList.Add(PublicData.ProductTimes);
                            PublicData.BackTemperatureTimingList.Add(PublicData.ProductBackTemperatureTimes);
                            PublicData.NormalTemperatureTimingList.Add(PublicData.ProductNormalTemperatureTimes);
                            UpdateData(0, 0);
                            if (PublicData.NormalTemperatureFlag2)
                            {
                                timer1s.Start();
                            }


                            PublicData.GlueSeqNumberList.Add(0);
                            PublicData.GlueIDList.Add(PublicData.ProductCode);
                            PublicData.GlueStateList.Add(0);
                            PublicData.BackTemperatureNumberList.Add(0);
                            PublicData.BackTemperatureTimingList.Add(0);
                            PublicData.NormalTemperatureTimingList.Add(0);

                            PublicData.BackTemperatureNumberList[PublicData.GlueIDList.Count - 1]++;
                            PublicData.GlueSeqNumberList[PublicData.GlueIDList.Count - 1] = PublicData.GlueIDList.Count;
                            InsertData(PublicData.GlueIDList.Count - 1);

                            timer2s.Start();
                            PublicData.BackTemperatureFlag2 = true;
                            PublicData.NormalTemperatureFlag2 = true;
                        }
                        else
                        {
                            ResultPictureShow(0);
                        }
                    }

                    ShowData();
                }
            }
        }

        #endregion

        #region 数据库操作
        /// <summary>
        /// 显示刷新产品信息数据
        /// </summary>
        private void ShowData()
        {
            //定义sql语句文本
            // string cmdText = "select * from ProductInformation";
            string cmdText = "select 第一次回温扫码时间,第二次回温扫码时间,第三次回温扫码时间,操作人,胶水序号, 胶水ID,胶水状态,胶水回温次数, 胶水回温计时s,胶水常温计时s from ProductInformation";
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
                        PublicData.Productdatatime8 = reader[7].ToString();//获取胶水第一次回箱扫码时间
                        PublicData.Productdatatime9 = reader[8].ToString();//获取胶水第二次回箱扫码时间
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
        /// <summary>
        /// 检索数据库中是否存在未使用完且合格的胶水
        /// 若没有检索到符合条件的则可以使用新胶水；否则必须先使用完已使用的胶水后再允许使用新胶水
        /// </summary>
        /// <returns>允许使用新胶水返回true，否则返回false</returns>
        private int RetrievalData()
        {
            int DataCounts = 0;
            //定义sql语句文本,向表中插入新数据
            string cmdText = "SELECT * FROM ProductInformation WHERE 胶水状态=0 AND 胶水回温次数<=@WorkTimes AND 胶水常温计时s<@NormalTemperatureLimitTime";
            using (SQLiteConnection con = new SQLiteConnection(PublicData.ConnString))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(cmdText, con))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    //给参数赋值
                    SQLiteParameter[] parameter = new SQLiteParameter[] {new SQLiteParameter("@WorkTimes", PublicData.WorkLimitTimesSet),
                                                                    new SQLiteParameter("@NormalTemperatureLimitTime", PublicData.NormalTemperatureLimitTimeSet)
                                                                };
                    cmd.Parameters.AddRange(parameter);
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PublicData.ProductSeqNumber = Convert.ToInt32(reader[10]);//获取胶水序号
                        PublicData.ProductID = reader[11].ToString();//获取胶水ID
                        PublicData.ProductState = Convert.ToInt32(reader[12]);//获取胶水状态
                        PublicData.ProductTimes = Convert.ToInt32(reader[13]);//获取胶水回温次数
                        PublicData.ProductBackTemperatureTimes = Convert.ToInt32(reader[14]);//获取胶水回温计时时间
                        PublicData.ProductNormalTemperatureTimes = Convert.ToInt32(reader[15]);//获取胶水常温计时时间

                        DataCounts++;
                    }
                }
            }
            return DataCounts;
        }
        private void InsertData(int i)
        {
            //定义sql语句文本,向表中插入新数据
            string cmdText = "Insert into ProductInformation VALUES (@DateTime1,@DateTime2,@DateTime3,@DateTime4,@DateTime5,@DateTime6,@DateTime7,@DateTime8,@DateTime9,@Name,@Seqnum,@QrCode,@State,@Numbers,@BackTimes,@Times,@ProductModel1,@Batch1,@ProductNumbers1,@ProductInformtion1,@ProductModel2,@Batch2,@ProductNumbers2,@ProductInformtion2,@ProductModel3,@Batch3,@ProductNumbers3,@ProductInformtion3,@ProductModel4,@Batch4,@ProductNumbers4,@ProductInformtion4)";
            //给参数赋值
            SQLiteParameter[] parameter = new SQLiteParameter[] {new SQLiteParameter("@DateTime1", DateTime.Now),
                                                                    new SQLiteParameter("@DateTime2", null),
                                                                    new SQLiteParameter("@DateTime3", null),
                                                                    new SQLiteParameter("@DateTime4", null),
                                                                    new SQLiteParameter("@DateTime5", null),
                                                                    new SQLiteParameter("@DateTime6", null),
                                                                    new SQLiteParameter("@DateTime7", null),
                                                                    new SQLiteParameter("@DateTime8", null),
                                                                    new SQLiteParameter("@DateTime9", null),
                                                                    new SQLiteParameter("@Name", PublicData.LoginUsername),
                                                                    new SQLiteParameter("@Seqnum", PublicData.GlueSeqNumberList[i]),
                                                                    new SQLiteParameter("@QrCode", PublicData.GlueIDList[i]),
                                                                    new SQLiteParameter("@State", PublicData.GlueStateList[i]),
                                                                    new SQLiteParameter("@Numbers", PublicData.BackTemperatureNumberList[i]),
                                                                    new SQLiteParameter("@BackTimes", PublicData.BackTemperatureTimingList[i]),
                                                                    new SQLiteParameter("@Times", PublicData.NormalTemperatureTimingList[i]),
                                                                    new SQLiteParameter("@ProductModel1", null),
                                                                    new SQLiteParameter("@Batch1", null),
                                                                    new SQLiteParameter("@ProductNumbers1", null),
                                                                    new SQLiteParameter("@ProductInformtion1", null),
                                                                    new SQLiteParameter("@ProductModel2", null),
                                                                    new SQLiteParameter("@Batch2", null),
                                                                    new SQLiteParameter("@ProductNumbers2", null),
                                                                    new SQLiteParameter("@ProductInformtion2", null),
                                                                    new SQLiteParameter("@ProductModel3", null),
                                                                    new SQLiteParameter("@Batch3", null),
                                                                    new SQLiteParameter("@ProductNumbers3", null),
                                                                    new SQLiteParameter("@ProductInformtion3", null),
                                                                    new SQLiteParameter("@ProductModel4", null),
                                                                    new SQLiteParameter("@Batch4", null),
                                                                    new SQLiteParameter("@ProductNumbers4", null),
                                                                    new SQLiteParameter("@ProductInformtion4",null)
                                                                };
            //执行SQL语句
            SqLiteHelper.ExecuteNonQuery(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);

        }
        private void UpdateData(int i, int times)
        {
            if (times == 2)
            {
                //定义sql语句文本,更新表中二维码相同的数据
                string cmdText = "UPDATE ProductInformation SET 第二次回温扫码时间=@DateTime2,操作人=@Name,胶水序号=@Seqnum,胶水状态=@State,胶水回温次数=@Numbers,胶水回温计时s=@BackTimes,胶水常温计时s=@Times WHERE 胶水ID=@QrCode";
                //给参数赋值
                SQLiteParameter[] parameter = new SQLiteParameter[] {
                                                                    new SQLiteParameter("@DateTime2", DateTime.Now),
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
            else if (times == 3)
            {
                //定义sql语句文本,更新表中二维码相同的数据
                string cmdText = "UPDATE ProductInformation SET 第三次回温扫码时间=@DateTime3,操作人=@Name,胶水序号=@Seqnum,胶水状态=@State,胶水回温次数=@Numbers,胶水回温计时s=@BackTimes,胶水常温计时s=@Times WHERE 胶水ID=@QrCode";
                //给参数赋值
                SQLiteParameter[] parameter = new SQLiteParameter[] {
                                                                    new SQLiteParameter("@DateTime3", DateTime.Now),
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
            else
            {
                //定义sql语句文本,更新表中二维码相同的数据
                string cmdText = "UPDATE ProductInformation SET 操作人=@Name,胶水序号=@Seqnum,胶水状态=@State,胶水回温次数=@Numbers,胶水回温计时s=@BackTimes,胶水常温计时s=@Times WHERE 胶水ID=@QrCode";
                //给参数赋值
                SQLiteParameter[] parameter = new SQLiteParameter[] {
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
        }
        /// <summary>
        /// 首次运行检索数据库中是否存在未使用完的胶水
        /// </summary>
        /// <returns>返回未使用的胶水数量</returns>
        private int FirstTimeRetrievalData()
        {
            int DataCounts = 0;
            //定义sql语句文本,向表中插入新数据
            string cmdText = "SELECT * FROM ProductInformation WHERE 胶水序号!=0";
            using (SQLiteConnection con = new SQLiteConnection(PublicData.ConnString))
            {
                con.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(cmdText, con))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        PublicData.GlueSeqNumberList.Add(Convert.ToInt32(reader[10]));//获取胶水序号
                        PublicData.GlueIDList.Add(reader[11].ToString());//获取胶水ID
                        PublicData.GlueStateList.Add(Convert.ToInt32(reader[12]));//获取胶水状态
                        PublicData.BackTemperatureNumberList.Add(Convert.ToInt32(reader[13]));//获取胶水回温次数
                        PublicData.BackTemperatureTimingList.Add(Convert.ToInt32(reader[14]));//获取胶水回温计时时间
                        PublicData.NormalTemperatureTimingList.Add(Convert.ToInt32(reader[15]));//获取胶水常温计时时间

                        DataCounts++;
                    }
                }
            }
            return DataCounts;
        }
        private void UpdateData_Real(int i)
        {
            //定义sql语句文本,更新表中二维码相同的数据
            string cmdText = "UPDATE ProductInformation SET 胶水回温计时s=@BackTimes,胶水常温计时s=@Times WHERE 胶水ID=@QrCode";
            //给参数赋值
            SQLiteParameter[] parameter = new SQLiteParameter[] {   new SQLiteParameter("@QrCode", PublicData.GlueIDList[i]),
                                                                    new SQLiteParameter("@BackTimes", PublicData.BackTemperatureTimingList[i]),
                                                                    new SQLiteParameter("@Times", PublicData.NormalTemperatureTimingList[i])
                                                                };
            //执行SQL语句
            SqLiteHelper.ExecuteNonQuery(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);
        }
        #endregion

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
                    label2.Text = "可使用";
                    label2.ForeColor = Color.LimeGreen;
                    break;
                case 2://扫码异常
                    guna2PictureBox1.Visible = false;
                    guna2PictureBox2.Visible = false;
                    guna2PictureBox3.Visible = true;
                    label2.Text = "不可使用";
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


    }
}
