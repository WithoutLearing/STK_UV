using System;
using System.Windows.Forms;

namespace BCM检测工装
{
    public partial class frmParamConfig : Form
    {
        public frmParamConfig()
        {
            InitializeComponent();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            PublicData.WorkLimitTimesSet = (int)guna2NumericUpDown1.Value;
            PublicData.BackTemperatureTimeSet = (int)((float)guna2NumericUpDown2.Value * 3600);
            PublicData.NormalTemperatureLimitTimeSet = (int)guna2NumericUpDown3.Value * 3600;
            PublicData.GlueIDLengthSet = (int)guna2NumericUpDown4.Value;
            PublicData.ProductModelLengthSet = (int)guna2NumericUpDown5.Value;
            PublicData.ProductTimesLengthSet = (int)guna2NumericUpDown6.Value;

            SetConfig();
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SetConfig()
        {
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("胶水信息", "工作次数", PublicData.WorkLimitTimesSet.ToString(), PublicData.IniPath);
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("胶水信息", "回温时长设定值", PublicData.BackTemperatureTimeSet.ToString(), PublicData.IniPath);
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("胶水信息", "常温累计时长上限", PublicData.NormalTemperatureLimitTimeSet.ToString(), PublicData.IniPath);
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("胶水信息", "胶水信息长度", PublicData.GlueIDLengthSet.ToString(), PublicData.IniPath);
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("胶水信息", "产品编码长度", PublicData.ProductModelLengthSet.ToString(), PublicData.IniPath);
            ClassLibrary_FQY.INIFilesHelper.IniWriteValue("胶水信息", "产品批次号长度", PublicData.ProductTimesLengthSet.ToString(), PublicData.IniPath);
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        private void GetConfig()
        {
            guna2NumericUpDown1.Value = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "工作次数", PublicData.IniPath));
            guna2NumericUpDown2.Value = (decimal)(float.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "回温时长设定值", PublicData.IniPath)) / 3600.0f);
            guna2NumericUpDown3.Value = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "常温累计时长上限", PublicData.IniPath)) / 3600;
            guna2NumericUpDown4.Value = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "胶水信息长度", PublicData.IniPath));
            guna2NumericUpDown5.Value = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "产品编码长度", PublicData.IniPath));
            guna2NumericUpDown6.Value = int.Parse(ClassLibrary_FQY.INIFilesHelper.IniReadValue("胶水信息", "产品批次号长度", PublicData.IniPath));

        }

        private void frmParamConfig_Load(object sender, EventArgs e)
        {
            GetConfig();
        }
    }
}
