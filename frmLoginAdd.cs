using ClassLibrary_FQY;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BCM检测工装
{
    public partial class frmLoginAdd : Form
    {
        public frmLoginAdd()
        {
            InitializeComponent();
            //Form属性配置
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            //开启双缓冲
            this.DoubleBuffered = true;

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            //获取文本框输入信息
            string Username = guna2TextBox1.Text;
            string Password = guna2TextBox2.Text;
            //定义sql语句文本,向表中插入新数据
            string cmdText = "Insert into UserLogin (username, password) VALUES (@Username, @Password)";

            try
            {
                if (Username.Length > 0 && Password.Length > 0)
                {
                    if (PublicData.LoginUsername == "admin" && PublicData.LoginOnflag)
                    {
                        //给参数赋值
                        SQLiteParameter[] parameter = new SQLiteParameter[] {new SQLiteParameter("@Username", Username),
                                                            new SQLiteParameter("@Password", Password)};
                        //执行SQL语句
                        int result = SqLiteHelper.ExecuteNonQuery(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);
                        //返回执行结果
                        if (result > 0)
                        {
                            MessageBox.Show("新用户录入成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("新用户录入失败", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show("请以管理员身份登陆后再进行操作", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("用户名或密码不能为空", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception)
            {


            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
