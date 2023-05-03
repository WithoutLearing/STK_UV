using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using static BCM检测工装.PublicFunction;

namespace BCM检测工装
{
    public partial class frmMain : Form
    {
        #region 实例化类

        //实例化各窗体
        private frmLogin frmlogin = new frmLogin();
        private frmLoginAdd frmloginadd;
        private frmBackTemperature frmbacktemp;
        private frmOnline frmonline;
        private frmCompletion frmcompletion;
        private frmRecycling frmrecycling;
        private frmParamConfig frmparamconfig;
        private frmDataQuery frmdataquery;
        private SerialPort serialPort = new SerialPort();

        #endregion


        private string[] portName = new string[] { };
        int sendflag = 0;
        List<byte> RecBuffer = new List<byte>();



        public frmMain()
        {
            InitializeComponent();
            Init_ControlProperty();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init_ControlMethod();
            Init_SerialPort();
            GetConfig();
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerInit.Stop();
            timer1.Stop();
            serialPort.Close();
            serialPort.Dispose();
        }

        private void Init_ControlProperty()
        {
            //设置从属关系,方便在登陆界面修改主界面的控件属性
            frmlogin.Owner = this;
            //Form属性配置
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;

            this.panel1.BackColor = ColorTranslator.FromHtml("#3D5183");
            this.panel2.BackColor = ColorTranslator.FromHtml("#3D5183");
            this.panel3.BackColor = ColorTranslator.FromHtml("#3D5183");
            //gun2Button
            guna2Button1.FillColor = ColorTranslator.FromHtml("#3D5183");
            guna2Button2.FillColor = ColorTranslator.FromHtml("#3D5183");
            guna2Button3.FillColor = ColorTranslator.FromHtml("#3D5183");
            guna2Button4.FillColor = ColorTranslator.FromHtml("#3D5183");
            guna2Button5.FillColor = ColorTranslator.FromHtml("#3D5183");
            guna2Button6.FillColor = ColorTranslator.FromHtml("#3D5183");
            guna2Button7.FillColor = ColorTranslator.FromHtml("#3D5183");
            guna2Button8.FillColor = ColorTranslator.FromHtml("#3D5183");
            guna2Button1.ForeColor = ColorTranslator.FromHtml("#F6FAFD");
            guna2Button2.ForeColor = ColorTranslator.FromHtml("#F6FAFD");
            guna2Button3.ForeColor = ColorTranslator.FromHtml("#F6FAFD");
            guna2Button4.ForeColor = ColorTranslator.FromHtml("#F6FAFD");
            guna2Button5.ForeColor = ColorTranslator.FromHtml("#F6FAFD");
            guna2Button6.ForeColor = ColorTranslator.FromHtml("#F6FAFD");
            guna2Button7.ForeColor = ColorTranslator.FromHtml("#F6FAFD");
            guna2Button8.ForeColor = ColorTranslator.FromHtml("#F6FAFD");
            guna2Button1.HoverState.BorderColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button1.HoverState.FillColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button1.HoverState.ForeColor = ColorTranslator.FromHtml("#25366C");
            guna2Button2.HoverState.BorderColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button2.HoverState.FillColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button2.HoverState.ForeColor = ColorTranslator.FromHtml("#25366C");
            guna2Button3.HoverState.BorderColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button3.HoverState.FillColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button3.HoverState.ForeColor = ColorTranslator.FromHtml("#25366C");
            guna2Button4.HoverState.BorderColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button4.HoverState.FillColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button4.HoverState.ForeColor = ColorTranslator.FromHtml("#25366C");
            guna2Button6.HoverState.BorderColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button6.HoverState.FillColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button6.HoverState.ForeColor = ColorTranslator.FromHtml("#25366C");
            guna2Button7.HoverState.BorderColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button7.HoverState.FillColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button7.HoverState.ForeColor = ColorTranslator.FromHtml("#25366C");
            guna2Button8.HoverState.BorderColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button8.HoverState.FillColor = ColorTranslator.FromHtml("#E1EBF6");
            guna2Button8.HoverState.ForeColor = ColorTranslator.FromHtml("#25366C");

            this.guna2PictureBox3.Visible = false;

            this.timerInit.Interval = 1000;
            timer1.Interval = 100;
        }

        /// <summary>
        /// 控件方法初始化
        /// </summary>
        private void Init_ControlMethod()
        {
            timerInit.Start();
            timer1.Start();
        }

        private void Init_SerialPort()
        {
            //先判断电脑上是否存在串口再进行初始化
            if (GetPortName() > 0)
            {
                serialPort.PortName = GetSerialPortName();
                serialPort.BaudRate = 9600;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;
                serialPort.DtrEnable = true;
                serialPort.RtsEnable = false;

                serialPort.Open();
                serialPort.DataReceived += SerialPort_DataReceived;
            }
            else
            {
                MessageBox.Show("未发现串口，请检查电脑串口通信线", "通信错误");
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int length = serialPort.BytesToRead;
            byte[] data = new byte[length];
            serialPort.Read(data, 0, length);
            //1、缓存数据
            RecBuffer.AddRange(data);
            //2、解析数据
            while (RecBuffer.Count >= 2)
            {
                if (RecBuffer[0] == 0xEE && RecBuffer[1] == 0xFF)//开关状态   校验帧头
                {
                    if (RecBuffer.Count < 9)
                    {
                        break;
                    }
                    //校验位
                    if (RecBuffer[7] == ((RecBuffer[2] + RecBuffer[3] + RecBuffer[4] + RecBuffer[5] + RecBuffer[6]) & 0xFF) && RecBuffer[8] == ((RecBuffer[7] + RecBuffer[7]) & 0xFF))
                    {
                        //将完整数据复制到目标数组中
                        RecBuffer.CopyTo(0, PublicData.SwitchState, 0, 9);
                        //清除数据缓存
                        RecBuffer.Clear();
                    }

                }
                else if (RecBuffer[0] == 0xAA && RecBuffer[1] == 0xBB)//继电器状态   校验帧头
                {
                    if (RecBuffer.Count < 8)
                    {
                        break;
                    }
                    //校验位
                    if (RecBuffer[7] == ((RecBuffer[2] + RecBuffer[3] + RecBuffer[4] + RecBuffer[5] + RecBuffer[6]) & 0xFF))
                    {
                        //将完整数据复制到目标数组中
                        RecBuffer.CopyTo(0, PublicData.RelayState, 0, 8);
                        //清除数据缓存
                        RecBuffer.Clear();
                    }
                }
                else
                {
                    //清除数据缓存
                    RecBuffer.Clear();
                }
            }
        }

        #region 窗体最小化、关闭 
        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("关闭软件后,自动线体将无法继续运行，请确认是否关闭", "注意", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                PublicData.sendWirteflag = 10;//继电器1关闭
                Thread.Sleep(400);
                PublicData.sendWirteflag = 20;//继电器2关闭
                Thread.Sleep(400);
                PublicData.sendWirteflag = 30;//继电器3关闭
                Thread.Sleep(400);
                this.Close();
            }

        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void guna2PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            guna2PictureBox1.BackColor = ColorTranslator.FromHtml("#E1EBF6");
        }

        private void guna2PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            guna2PictureBox1.BackColor = Color.Transparent;
        }

        private void guna2PictureBox2_MouseMove(object sender, MouseEventArgs e)
        {
            guna2PictureBox2.BackColor = ColorTranslator.FromHtml("#E1EBF6");
        }

        private void guna2PictureBox2_MouseLeave(object sender, EventArgs e)
        {
            guna2PictureBox2.BackColor = Color.Transparent;
        }

        #endregion

        #region 定时器
        private void timerInit_Tick(object sender, EventArgs e)
        {
            label3.Text = DateTime.Now.ToLocalTime().ToString();

            PublicData.WorkStation1 = (PublicData.SwitchState[5] & 0x01) == 0x01 ? true : false;//实时检测工位1是否有胶水
            PublicData.WorkStation2 = (PublicData.SwitchState[5] >> 1 & 0x01) == 0x01 ? true : false;//实时检测工位2是否有胶水
            PublicData.GlueWorkState = (PublicData.SwitchState[5] >> 2 & 0x01) == 0x01 ? true : false;//胶水是否用光
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //与控制器以轮询方式进行通信
            switch (sendflag)
            {
                case 0:
                    sendflag = 1;
                    SendData_Read(0);
                    break;
                case 1:
                    sendflag = 2;
                    SendData_Read(1);
                    break;
                case 2:
                    sendflag = 0;
                    SendData_Write(PublicData.sendWirteflag);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 界面选择
        /// <summary>
        /// 回温扫码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            // FrmClose(1);

            if (PublicData.LoginOnflag)
            {
                //保证窗体如果已经打开则不再重复打开
                if (frmbacktemp == null)
                {
                    frmbacktemp = new frmBackTemperature();
                    frmbacktemp.TopLevel = false;
                    frmbacktemp.FormBorderStyle = FormBorderStyle.None;
                    frmbacktemp.Dock = DockStyle.Fill;
                    this.panel3.Controls.Clear();
                    this.panel3.Controls.Add(frmbacktemp);

                    frmbacktemp.Show();
                    frmbacktemp.BringToFront();
                }
                else
                {
                    if (frmbacktemp.IsDisposed)
                    {
                        frmbacktemp = new frmBackTemperature();
                        frmbacktemp.TopLevel = false;
                        frmbacktemp.FormBorderStyle = FormBorderStyle.None;
                        frmbacktemp.Dock = DockStyle.Fill;
                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmbacktemp);

                        frmbacktemp.Show();
                    }
                    else
                    {
                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmbacktemp);
                        frmbacktemp.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请登录后再试", "注意", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 上线扫码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (PublicData.LoginOnflag)
            {
                //保证窗体如果已经打开则不再重复打开
                if (frmonline == null)
                {
                    frmonline = new frmOnline();
                    frmonline.TopLevel = false;
                    frmonline.FormBorderStyle = FormBorderStyle.None;
                    frmonline.Dock = DockStyle.Fill;

                    this.panel3.Controls.Clear();
                    this.panel3.Controls.Add(frmonline);

                    frmonline.Show();
                    frmonline.BringToFront();
                }
                else
                {
                    if (frmonline.IsDisposed)
                    {
                        frmonline = new frmOnline();
                        frmonline.TopLevel = false;
                        frmonline.FormBorderStyle = FormBorderStyle.None;
                        frmonline.Dock = DockStyle.Fill;

                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmonline);

                        frmonline.Show();
                    }
                    else
                    {
                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmonline);
                        frmonline.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请登录后再试", "注意", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 用毕扫码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            if (PublicData.LoginOnflag)
            {
                //保证窗体如果已经打开则不再重复打开
                if (frmcompletion == null)
                {
                    frmcompletion = new frmCompletion();
                    frmcompletion.TopLevel = false;
                    frmcompletion.FormBorderStyle = FormBorderStyle.None;
                    frmcompletion.Dock = DockStyle.Fill;

                    this.panel3.Controls.Clear();
                    this.panel3.Controls.Add(frmcompletion);

                    frmcompletion.Show();
                    frmcompletion.BringToFront();
                }
                else
                {
                    if (frmcompletion.IsDisposed)
                    {
                        frmcompletion = new frmCompletion();
                        frmcompletion.TopLevel = false;
                        frmcompletion.FormBorderStyle = FormBorderStyle.None;
                        frmcompletion.Dock = DockStyle.Fill;

                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmcompletion);

                        frmcompletion.Show();
                    }
                    else
                    {
                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmcompletion);
                        frmcompletion.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请登录后再试", "注意", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// 回箱扫码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (PublicData.LoginOnflag)
            {
                //保证窗体如果已经打开则不再重复打开
                if (frmrecycling == null)
                {
                    frmrecycling = new frmRecycling();
                    frmrecycling.TopLevel = false;
                    frmrecycling.FormBorderStyle = FormBorderStyle.None;
                    frmrecycling.Dock = DockStyle.Fill;

                    this.panel3.Controls.Clear();
                    this.panel3.Controls.Add(frmrecycling);

                    frmrecycling.Show();
                    frmrecycling.BringToFront();
                }
                else
                {
                    if (frmrecycling.IsDisposed)
                    {
                        frmrecycling = new frmRecycling();
                        frmrecycling.TopLevel = false;
                        frmrecycling.FormBorderStyle = FormBorderStyle.None;
                        frmrecycling.Dock = DockStyle.Fill;

                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmrecycling);

                        frmrecycling.Show();
                    }
                    else
                    {
                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmrecycling);
                        frmrecycling.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请登录后再试", "注意", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// 主页按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Click(object sender, EventArgs e)
        {

            //   FrmClose(0);
            panel3.Controls.Clear();


        }
        /// <summary>
        /// 报表查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button6_Click(object sender, EventArgs e)
        {
            if (PublicData.LoginOnflag)
            {
                //保证窗体如果已经打开则不再重复打开
                if (frmdataquery == null)
                {
                    frmdataquery = new frmDataQuery();
                    frmdataquery.TopLevel = false;
                    frmdataquery.FormBorderStyle = FormBorderStyle.None;
                    frmdataquery.Dock = DockStyle.Fill;

                    this.panel3.Controls.Clear();
                    this.panel3.Controls.Add(frmdataquery);

                    frmdataquery.Show();
                    frmdataquery.BringToFront();
                }
                else
                {
                    if (frmdataquery.IsDisposed)
                    {
                        frmdataquery = new frmDataQuery();
                        frmdataquery.TopLevel = false;
                        frmdataquery.FormBorderStyle = FormBorderStyle.None;
                        frmdataquery.Dock = DockStyle.Fill;

                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmdataquery);

                        frmdataquery.Show();
                    }
                    else
                    {
                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmdataquery);
                        frmdataquery.Show();
                    }
                }
            }
            else
            {
                MessageBox.Show("请登录后再试", "注意", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        /// <summary>
        /// 参数配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (PublicData.LoginOnflag)
            {
                if (PublicData.LoginUsername == "admin")
                {
                    //保证窗体如果已经打开则不再重复打开
                    if (frmparamconfig == null)
                    {
                        frmparamconfig = new frmParamConfig();
                        frmparamconfig.TopLevel = false;
                        frmparamconfig.FormBorderStyle = FormBorderStyle.None;
                        frmparamconfig.Dock = DockStyle.Fill;

                        this.panel3.Controls.Clear();
                        this.panel3.Controls.Add(frmparamconfig);

                        frmparamconfig.Show();
                        frmparamconfig.BringToFront();
                    }
                    else
                    {
                        if (frmparamconfig.IsDisposed)
                        {
                            frmparamconfig = new frmParamConfig();
                            frmparamconfig.TopLevel = false;
                            frmparamconfig.FormBorderStyle = FormBorderStyle.None;
                            frmparamconfig.Dock = DockStyle.Fill;

                            this.panel3.Controls.Clear();
                            this.panel3.Controls.Add(frmparamconfig);

                            frmparamconfig.Show();
                        }
                        else
                        {
                            this.panel3.Controls.Clear();
                            this.panel3.Controls.Add(frmparamconfig);
                            frmparamconfig.Show();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("请使用管理员身份登录后再试", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            else
            {
                MessageBox.Show("请登录后再试", "注意", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            frmlogin.Show();
        }
        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {
            frmlogin.Show();
        }

        /// <summary>
        /// 用户管理，用户添加界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void guna2Button8_Click(object sender, EventArgs e)
        {
            if (PublicData.LoginOnflag)
            {
                if (PublicData.LoginUsername == "admin")
                {
                    this.panel3.Controls.Clear();

                    frmloginadd = new frmLoginAdd();
                    frmloginadd.Show();
                }
                else
                {
                    MessageBox.Show("请使用管理员身份登录后再试", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("请登录后再试", "注意", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }
        #endregion


        /// <summary>
        /// 读取配置
        /// </summary>
        private void GetConfig()
        {
            PublicData.BackTemperatureTimeSet = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "回温时长设定值", PublicData.IniPath));
            PublicData.WorkLimitTimesSet = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "工作次数", PublicData.IniPath));
            PublicData.NormalTemperatureLimitTimeSet = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "常温累计时长上限", PublicData.IniPath));
            PublicData.GlueIDLengthSet = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "胶水信息长度", PublicData.IniPath));
            PublicData.ProductModelLengthSet = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "产品编码长度", PublicData.IniPath));
            PublicData.ProductTimesLengthSet = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "产品批次号长度", PublicData.IniPath));

        }
        /// <summary>
        /// 获取电脑上的串口
        /// </summary>
        /// <returns>返回串口数量</returns>
        private int GetPortName()
        {
            portName = SerialPort.GetPortNames();

            return portName.Length;
        }
        /// <summary>
        /// 仅获取电脑上的USB转串口名称
        /// </summary>
        /// <returns>返回串口名称</returns>
        private string GetSerialPortName()
        {
            string name = "";
            string[] portInfo = GetHardwareInfo(HardwareEnum.Win32_PnPEntity, "Name");//获取电脑上所有设备的名称
            //检索出USB转串口的COM号
            foreach (string s in portInfo)
            {
                if (s.Length > 10 && s.Contains("USB Serial Port"))
                {
                    string[] nameList = s.Split(new char[2] { '(', ')' });
                    name = nameList[1];
                    break;
                }

            }

            return name;
        }

        /// <summary>
        /// 配置继电器状态
        /// </summary>
        /// <param name="sendflag"></param>
        private void SendData_Write(int sendflag)
        {
            byte[] data = new byte[10];

            if (serialPort.IsOpen)
            {
                switch (sendflag)
                {
                    case 11:
                        data = new byte[] { 0xCC, 0xDD, 0xA1, 0x01, 0x00, 0x01, 0x00, 0x01, 0xA4, 0x48 };
                        serialPort.Write(data, 0, data.Length);
                        break;
                    case 10:
                        data = new byte[] { 0xCC, 0xDD, 0xA1, 0x01, 0x00, 0x00, 0x00, 0x01, 0xA3, 0x46 };
                        serialPort.Write(data, 0, data.Length);
                        break;
                    case 21:
                        data = new byte[] { 0xCC, 0xDD, 0xA1, 0x01, 0x00, 0x02, 0x00, 0x02, 0xA6, 0x4C };
                        serialPort.Write(data, 0, data.Length);
                        break;
                    case 20:
                        data = new byte[] { 0xCC, 0xDD, 0xA1, 0x01, 0x00, 0x00, 0x00, 0x02, 0xA4, 0x48 };
                        serialPort.Write(data, 0, data.Length);
                        break;
                    case 31:
                        data = new byte[] { 0xCC, 0xDD, 0xA1, 0x01, 0x00, 0x04, 0x00, 0x04, 0xAA, 0x54 };
                        serialPort.Write(data, 0, data.Length);
                        break;
                    case 30:
                        data = new byte[] { 0xCC, 0xDD, 0xA1, 0x01, 0x00, 0x00, 0x00, 0x04, 0xA6, 0x4C };
                        serialPort.Write(data, 0, data.Length);
                        break;
                    case 41:
                        data = new byte[] { 0xCC, 0xDD, 0xA1, 0x01, 0x00, 0x08, 0x00, 0x08, 0xB2, 0x64 };
                        serialPort.Write(data, 0, data.Length);
                        break;
                    case 40:
                        data = new byte[] { 0xCC, 0xDD, 0xA1, 0x01, 0x00, 0x00, 0x00, 0x08, 0xAA, 0x54 };
                        serialPort.Write(data, 0, data.Length);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 读取控制器状态
        /// </summary>
        /// <param name="Readflag">0：读取继电器输出状态
        /// 1：读取开关输入状态</param>
        private void SendData_Read(int Readflag)
        {
            byte[] data = new byte[9];

            if (serialPort.IsOpen)
            {
                if (Readflag == 0)//读取继电器状态
                {
                    data = new byte[] { 0xCC, 0xDD, 0xB0, 0x01, 0x00, 0x00, 0x0D, 0xBE, 0x7C };
                    serialPort.Write(data, 0, data.Length);
                }
                else//读取开关量状态
                {
                    data = new byte[] { 0xCC, 0xDD, 0xC0, 0x01, 0x00, 0x00, 0x0D, 0xCE, 0x9C };
                    serialPort.Write(data, 0, data.Length);
                }
            }
        }


    }
}










