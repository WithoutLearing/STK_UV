using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Windows.Forms;

namespace BCM检测工装
{
    public class PublicFunction
    {

        /// <summary>
        /// 获取指定字符在字符串中的位置
        /// </summary>
        /// <param name="strSource">目标字符串</param>
        /// <param name="strTarget">指定字符</param>
        /// <returns>自动字符的索引位置</returns>
        public static List<int> SearchCharLocation(string strSource, char strTarget)
        {
            return strSource.Select((item, index) => new { item, index }).Where(t => t.item == strTarget).Select(t => t.index).ToList();
        }

        /// <summary>
        /// 将整数转换为二进制字符串
        /// 高字节在前，低字节在后
        /// </summary>
        /// <param name="Value">要转换的整数数组</param>
        /// <returns></returns>
        public static string ByteToBinaryString(byte[] Value)
        {
            string result = "";
            for (int i = Value.Length; i > 0; i--)
            {
                result = result + Convert.ToString(Value[i - 1], 2).PadLeft(8, '0') + " ";
            }

            return result.Trim();
        }

        /// <summary>
        /// 替换字符串指定位置的字符
        /// </summary>
        /// <param name="strSource">要替换的字符串</param>
        /// <param name="Index">要替换的索引位置</param>
        /// <param name="chrValue">目标字符</param>
        /// <returns></returns>
        public static string ReplaceSting(string strSource, List<int> Index, string chrValue)
        {
            string strResult = strSource;
            for (int i = 0; i < Index.Count; i++)
            {
                strResult = strResult.Remove(Index[i], 1).Insert(Index[i], chrValue);
            }
            return strResult.Trim();
        }

        /// <summary>
        /// 将二进制字符串转化为整数数组
        /// </summary>
        /// <param name="value">要转换的二进制字符串</param>
        /// <returns>整数数组</returns>
        public static byte[] BinaryStringToByte(string value)
        {
            byte[] ValueByte;
            string strValue = value.Replace(" ", "");//获取去除掉空格的字符串
            int lenth = strValue.Length;//获取去除掉空格的字符串的长度
            //判断长度能被8整除且不为0
            if (lenth % 8 == 0 && lenth != 0)
            {
                string[] strTemp = new string[lenth / 8];
                ValueByte = new byte[lenth / 8];
                for (int i = 0; i < lenth / 8; i++)
                {
                    strTemp[i] = strValue.Substring(lenth - 8 * (i + 1), 8).Trim();
                    ValueByte[i] = Convert.ToByte(strTemp[i], 2);
                }
                return ValueByte;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 将数据导出excel格式
        /// </summary>
        /// <param name="dt">数据源数据表</param>
        /// <param name="filename"></param>
        public static void ExportDataToExcel(DataTable dt, string filename)
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Title = filename;//设置文件标题
            saveFileDialog.Filter = "Mircosoft Office Excel 工作簿(*.xls)|*.xls";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = filename;
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                //获得文件路径
                string localFilePath = saveFileDialog.FileName.ToString();

                //初始化数据
                int TotalCount = 0;
                int RowsRead = 0;
                int Percent = 0;

                TotalCount = dt.Rows.Count;

                //数据流
                Stream stream = saveFileDialog.OpenFile();
                StreamWriter sw = new StreamWriter(stream, Encoding.GetEncoding("gb2312"));
                string strHeader = "";

                try
                {
                    //写入标题
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        if (i > 0)
                        {
                            strHeader += "\t";
                        }
                        strHeader += dt.Columns[i].ColumnName.ToString();
                    }
                    sw.WriteLine(strHeader);

                    //写入数据
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        RowsRead++;
                        Percent = (int)(100 * RowsRead / TotalCount);
                        Application.DoEvents();

                        string strData = "";
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (j > 0)
                            {
                                strData += "\t";
                            }
                            strData += dt.Rows[i][j].ToString();
                        }
                        sw.WriteLine(strData);
                    }

                    //关闭数据流
                    sw.Close();
                    stream.Close();

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    //关闭数据流
                    sw.Close();
                    stream.Close();
                }


                //成功提示
                if (MessageBox.Show("导出成功，是否立即打开？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(localFilePath);
                }
            }












        }


        /// <summary>
        /// 使用windows api获取系统设备信息
        /// </summary>
        /// <param name="hardType">设备类型</param>
        /// <param name="propKey">设备的属性</param>
        /// <returns></returns>
        public static string[] GetHardwareInfo(HardwareEnum hardType, string propKey)
        {
            List<string> strs = new List<string>();

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + hardType))
            {
                var hardInfos = searcher.Get();
                foreach (var hardInfo in hardInfos)
                {
                    if (hardInfo.Properties[propKey].Value != null)
                    {
                        strs.Add(hardInfo.Properties[propKey].Value.ToString());
                    }
                }
            }

            return strs.ToArray();
        }
        /// <summary>
        ///  枚举win32 api
        /// </summary>
        public enum HardwareEnum
        {
            Win32_SerialPort,// 串口
            Win32_SerialPortConfiguration, // 串口配置
            Win32_PnPEntity//all device
        }


    }
}
