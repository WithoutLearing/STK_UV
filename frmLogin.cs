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
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();

            //Form属性配置
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            //开启双缓冲
            this.DoubleBuffered = true;
        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void guna2PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            guna2PictureBox1.BackColor = Color.FromArgb(255, 192, 203);
        }

        private void guna2PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            guna2PictureBox1.BackColor = Color.Transparent;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            //获取文本框输入信息
            string Username = guna2TextBox1.Text;
            string Password = guna2TextBox2.Text;

            //定义sql语句文本,查询表中的用户名和对应的密码
            string cmdText = "select * from UserLogin where username = @Username and password = @Password";

            //用户名和密码不允许为空
            if (Username.Length > 0 && Password.Length > 0)
            {
                //给参数赋值
                SQLiteParameter[] parameter = new SQLiteParameter[] {new SQLiteParameter("@Username", Username),
                                                            new SQLiteParameter("@Password", Password)};
                SQLiteDataReader reader = SqLiteHelper.ExecuteReader(PublicData.ConnString, cmdText, System.Data.CommandType.Text, parameter);
                if (reader.Read())
                {
                    PublicData.LoginOnflag = true;
                    PublicData.LoginUsername = Username;
                    (this.Owner as frmMain).guna2PictureBox3.Visible = true;
                    (this.Owner as frmMain).labelusername.Text = guna2TextBox1.Text;
                    this.Hide();
                }
                else
                {
                    PublicData.LoginOnflag = false;
                    MessageBox.Show("用户名或密码错误，请重新输入", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                reader.Close();
            }
            else
            {
                MessageBox.Show("用户名或密码错误，请重新输入", "注意", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
           // guna2TextBox1.Focus();
        }

        private void frmLogin_Activated(object sender, EventArgs e)
        {
            guna2TextBox1.Focus();
        }
    }
}
