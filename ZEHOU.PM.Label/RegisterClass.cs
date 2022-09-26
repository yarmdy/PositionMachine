using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZEHOU.PM.Config;
using ZEHOU.PM.Command;
using ZEHOU.PM.DB.dbLabelInfo;
using System.IO;
using System.Text.RegularExpressions;
using System.Management;

namespace ZEHOU.PM.Label
{
    internal class RegisterClass
    {
        //  注册码类
        public class RegisterKey
        {
            // 注册码,由厂家生成,32个字符
            public string strKey;

            // 当前日期和当天使用次数
            public int intCurrentYear;
            public int intCurrentMonth;
            public int intCurrentDay;
            public int intCount;

            // 开始日期
            public int intFromYear;
            public int intFromMonth;
            public int intFromDay;

            // 结束日期
            public int intToYear;
            public int intToMonth;
            public int intToDay;
        }

        // 软件注册类
        public class SoftRegister
        {
            public static bool IsDate(string strIn)
            {
                return Regex.IsMatch(strIn, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
            }
            // 注册
            public static bool Register(string strUserID, string strCompanyName, string strDeviceID, string strRegisterKey)
            {
                string strDateFrom = "";        // 开始日期
                string strDateTo = "";          // 结束日期
                string strDateSys = "";         // 系统日期
                string strKey1 = "";
                string strKey2 = "";
                RegisterKey registerkey = null;
                string strDataBaseMD5Code = ""; // 数据库MD5码

                // 校验用户标识和注册码都必须是32个字符
                if (strUserID.Length != 32)
                {
                    return false;
                }
                if (strRegisterKey.Length != 32)
                {
                    return false;
                }
                // 校验公司名称不能为空
                if (strCompanyName == "")
                {
                    return false;
                }

                // 分析日期
                strDateFrom = strRegisterKey.Substring(8, 8);
                strDateFrom = SoftRegister.GetDateFromDateKey(strDateFrom);
                strDateTo = strRegisterKey.Substring(16, 8);
                strDateTo = SoftRegister.GetDateFromDateKey(strDateTo);

                // 校验日期是否合法
                if (IsDate(strDateFrom.Substring(0, 4) + "-" + strDateFrom.Substring(4, 2) + "-" + strDateFrom.Substring(6, 2)) == false)
                {
                    return false;
                }
                if (IsDate(strDateTo.Substring(0, 4) + "-" + strDateTo.Substring(4, 2) + "-" + strDateTo.Substring(6, 2)) == false)
                {
                    return false;
                }

                // 获取系统日期
                strDateSys = DateTime.Now.ToString("yyyyMMdd");

                // 校验系统日期必须与开始日期相同
                if (strDateFrom != strDateSys)
                {
                    return false;
                }

                // 校验注册码是否相同
                strKey1 = SoftRegister.CreateRegisterKey(strUserID, strCompanyName, strDeviceID, strDateFrom, strDateTo);
                strKey2 = strRegisterKey;
                if (strKey1 != strKey2)
                {
                    return false;
                }

                // 注册信息
                registerkey = new RegisterKey();
                registerkey.strKey = strRegisterKey;
                registerkey.intCurrentYear = Convert.ToInt32(strDateSys.Substring(0, 4));
                registerkey.intCurrentMonth = Convert.ToInt32(strDateSys.Substring(4, 2));
                registerkey.intCurrentDay = Convert.ToInt32(strDateSys.Substring(6, 2));
                registerkey.intCount = 1;

                registerkey.intFromYear = Convert.ToInt32(strDateFrom.Substring(0, 4));
                registerkey.intFromMonth = Convert.ToInt32(strDateFrom.Substring(4, 2));
                registerkey.intFromDay = Convert.ToInt32(strDateFrom.Substring(6, 2));

                registerkey.intToYear = Convert.ToInt32(strDateTo.Substring(0, 4));
                registerkey.intToMonth = Convert.ToInt32(strDateTo.Substring(4, 2));
                registerkey.intToDay = Convert.ToInt32(strDateTo.Substring(6, 2));

                // 将注册信息保存到文件
                if (SoftRegister.SaveRegisterKeyToFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\yt.ico", registerkey) == false)
                {
                    return false;
                }

                // 将公司名称保存到数据库
                if (SoftRegister.SaveCompanyNameToDataBase(strCompanyName) == false)
                {
                    return false;
                }

                // 将注册信息MD5码保存到数据库
                strDataBaseMD5Code = SoftRegister.CreateDataBaseMD5Code(registerkey);
                if (SoftRegister.SaveMD5CodeToDataBase(strDeviceID, strDataBaseMD5Code) == false)
                {
                    return false;
                }

                return true;
            }

            // 判断是否是注册用户
            public static bool IsRegister()
            {
                RegisterKey registerkey = null;
                string strDataBaseMD5Code1 = "";            // 数据库MD5码
                string strDataBaseMD5Code2 = "";            // 数据库MD5码
                string strUserID = "";
                string strCompanyName = "";
                string strDeviceID = Configs.Settings["DeviceID"];
                string strRegisterKey1 = "";
                string strRegisterKey2 = "";
                int intYear = 0;
                int intMonth = 0;
                int intDay = 0;
                DateTime datNow = DateTime.Now;
                int intCompare = 0;
                string strDataBaseMD5Code = "";
                string strDateFrom = "";
                string strDateTo = "";

                // 判断是否是付费用户
                if (SoftRegister.IsPayUser())
                {
                    return true;
                }

                // 读取注册信息
                registerkey = SoftRegister.ReadRegisterKeyFromFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\yt.ico");
                if (registerkey == null)
                {
                    return false;
                }

                // 判断数据是否已篡改
                strDataBaseMD5Code1 = SoftRegister.CreateDataBaseMD5Code(registerkey);
                strDataBaseMD5Code2 = SoftRegister.ReadMD5CodeFromDataBase(strDeviceID);
                if (strDataBaseMD5Code2 == null)
                {
                    return false;
                }
                if (strDataBaseMD5Code2 != strDataBaseMD5Code1)
                {
                    return false;
                }

                // 从数据库获取公司名称
                strCompanyName = SoftRegister.ReadCompanyNameFromDataBase();
                if (strCompanyName == "")
                {
                    return false;
                }

                // 判断注册码是否相同
                strUserID = SoftRegister.CreateUserID();
                strDateFrom = registerkey.intFromYear.ToString("D4") + registerkey.intFromMonth.ToString("D2") + registerkey.intFromDay.ToString("D2");
                strDateTo = registerkey.intToYear.ToString("D4") + registerkey.intToMonth.ToString("D2") + registerkey.intToDay.ToString("D2");
                strRegisterKey1 = SoftRegister.CreateRegisterKey(strUserID, strCompanyName, strDeviceID, strDateFrom, strDateTo);
                strRegisterKey2 = registerkey.strKey;
                if (strRegisterKey1 != strRegisterKey2)
                {
                    return false;
                }

                // 获取系统日期
                intYear = datNow.Year;
                intMonth = datNow.Month;
                intDay = datNow.Day;

                // 校验系统日期是否小于注册信息中的当前日期
                intCompare = SoftRegister.CompareDate(intYear, intMonth, intDay, registerkey.intCurrentYear, registerkey.intCurrentMonth, registerkey.intCurrentDay);
                if (intCompare == 0)
                {
                    if (registerkey.intCount > 100)
                    {
                        return false;
                    }
                    registerkey.intCount++;
                }
                else if (intCompare == 1)
                {
                    registerkey.intCount = 1;
                    registerkey.intCurrentYear = intYear;
                    registerkey.intCurrentMonth = intMonth;
                    registerkey.intCurrentDay = intDay;
                }
                else
                {
                    return false;
                }

                // 将注册信息保存到文件
                if (SoftRegister.SaveRegisterKeyToFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\yt.ico", registerkey) == false)
                {
                    return false;
                }

                // 将注册信息MD5码保存到数据库
                strDataBaseMD5Code = SoftRegister.CreateDataBaseMD5Code(registerkey);
                if (SoftRegister.SaveMD5CodeToDataBase(strDeviceID, strDataBaseMD5Code) == false)
                {
                    return false;
                }

                // 校验系统日期是与注册信息中的结束日期
                intCompare = SoftRegister.CompareDate(intYear, intMonth, intDay, registerkey.intToYear, registerkey.intToMonth, registerkey.intToDay);
                if (intCompare == 1)
                {
                    return false;
                }

                return true;
            }

            // 比较两个日期,1前面日期大 0日期相等 -1前面日期小
            public static int CompareDate(int intYearFrom, int intMonthFrom, int intDayFrom, int intYearTo, int intMonthTo, int intDayTo)
            {
                if (intYearFrom == intYearTo)
                {
                    if (intMonthFrom == intMonthTo)
                    {
                        if (intDayFrom == intDayTo)
                        {
                            return 0;
                        }
                        else
                        {
                            if (intDayFrom > intDayTo)
                            {
                                return 1;
                            }
                            else
                            {
                                return -1;
                            }
                        }
                    }
                    else
                    {
                        if (intMonthFrom > intMonthTo)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
                else
                {
                    if (intYearFrom > intYearTo)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }

            // 将RegisterKey的MD5码存储到数据库
            public static bool SaveMD5CodeToDataBase(string strDeviceID, string strMD5Code)
            {
                dbLabelInfoEntities db = new dbLabelInfoEntities();

                var model = db.DRs.FirstOrDefault(m => m.ID == strDeviceID);

                if (model == null)
                {
                    return false;
                }

                try
                {
                    model.Password = strMD5Code;
                    db.SaveChanges();
                }
                catch
                {
                    return false;
                }

                return true;
            }

            // 将公司名称保存到数据库
            public static bool SaveCompanyNameToDataBase(string strCompanyName)
            {
                dbLabelInfoEntities db = new dbLabelInfoEntities();
                var model = db.SystemParameters.FirstOrDefault(m => m.ID == "01");
                if (model == null)
                {
                    return false;
                }

                try
                {
                    model.CompanyName = strCompanyName;
                    db.SaveChanges();
                }
                catch
                {
                    return false;
                }

                return true;
            }

            // 从数据库读取数据库名称
            public static string ReadCompanyNameFromDataBase()
            {
                dbLabelInfoEntities db = new dbLabelInfoEntities();
                var model = db.SystemParameters.FirstOrDefault(m => m.ID == "01");
                if (model == null)
                {
                    return "";
                }

                return model.CompanyName;
            }

            // 从数据库读取MD5码
            // 异常返回null
            public static string ReadMD5CodeFromDataBase(string strDeviceID)
            {
                dbLabelInfoEntities db = new dbLabelInfoEntities();

                var model = db.DRs.FirstOrDefault(m => m.ID == strDeviceID);

                if (model == null)
                {
                    return null;
                }

                return model.Password;
            }

            // 保存注册码
            public static bool SaveRegisterKeyToFile(string strFile, RegisterKey key)
            {
                if (File.Exists(strFile) == false)
                {
                    return false;
                }

                try
                {
                    FileStream fs = new FileStream(strFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryWriter bw = new BinaryWriter(fs);

                    // 定位
                    // 766为yt.ico文件的初始大小
                    bw.Seek(766, SeekOrigin.Begin);


                    // 注册码,由厂家生成,32个字符
                    bw.Write((String)key.strKey);

                    // 当前日期和当天使用次数
                    bw.Write((Int32)key.intCurrentYear);
                    bw.Write((Int32)key.intCurrentMonth);
                    bw.Write((Int32)key.intCurrentDay);
                    bw.Write((Int32)key.intCount);

                    // 开始日期
                    bw.Write((Int32)key.intFromYear);
                    bw.Write((Int32)key.intFromMonth);
                    bw.Write((Int32)key.intFromDay);

                    // 结束日期
                    bw.Write((Int32)key.intToYear);
                    bw.Write((Int32)key.intToMonth);
                    bw.Write((Int32)key.intToDay);

                    bw.Close();
                    fs.Close();

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static RegisterKey ReadRegisterKeyFromFile(string strFile)
            {
                RegisterKey key = new RegisterKey();

                if (File.Exists(strFile) == false)
                {
                    return null;
                }
                try
                {
                    FileStream fs = new FileStream(strFile, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);

                    // 定位
                    // 766为yt.ico文件的初始大小
                    br.BaseStream.Seek(766, SeekOrigin.Begin);

                    // 注册码,由厂家生成,32个字符
                    key.strKey = br.ReadString();

                    // 当前日期和当天使用次数
                    key.intCurrentYear = br.ReadInt32();
                    key.intCurrentMonth = br.ReadInt32();
                    key.intCurrentDay = br.ReadInt32();
                    key.intCount = br.ReadInt32();

                    // 开始日期
                    key.intFromYear = br.ReadInt32();
                    key.intFromMonth = br.ReadInt32();
                    key.intFromDay = br.ReadInt32();

                    // 结束日期
                    key.intToYear = br.ReadInt32();
                    key.intToMonth = br.ReadInt32();
                    key.intToDay = br.ReadInt32();

                    br.Close();
                    fs.Close();

                    return key;
                }
                catch
                {
                    return null;
                }
            }

            // 获取硬盘序列号
            // 异常返回空字符串
            public static string GetDiskID()
            {
                string strDiskID = "";
                try
                {
                    ManagementClass mc = new ManagementClass("Win32_PhysicalMedia");
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        strDiskID = mo.Properties["SerialNumber"].Value.ToString().Trim();
                        break;
                    }
                }
                catch
                {
                    strDiskID = "";
                }

                return strDiskID;
            }

            // 生成用户标识,32位字符
            public static string CreateUserID()
            {
                string strUserID = "";
                string strDiskID = "";
                string strTemp = "";
                string strKey = "";

                // 获取硬盘物理序列号并加前缀
                strDiskID = SoftRegister.GetDiskID();
                strTemp = "yt#" + strDiskID;

                // md5加密
                strKey = strKey = "5315723044189026"; // 秘钥,16位字符
                strUserID = Crypt.Md5(strTemp, strKey);

                return strUserID;
            }

            // 生成日期密文
            public static string CreateDateKey(string strDate)
            {
                string strDateKey = "";
                string strKey = "";          // 秘钥
                int intCount = 0;
                int i = 0;
                string strT1 = "";
                string strT2 = "";

                strT1 = strDate;
                intCount = strT1.Length;
                strKey = "8735649102";       // 秘钥
                strDateKey = "";
                for (i = 0; i < intCount; i++)
                {
                    strT2 = strT1.Substring(i, 1);
                    strDateKey += strKey.Substring(Convert.ToInt32(strT2), 1);
                }

                return strDateKey;
            }

            // 获取日期明文,异常时返回空字符串
            public static string GetDateFromDateKey(string strDateKey)
            {
                string strDate = "";
                string strKey = "";         // 秘钥
                int intCount = 0;
                int i = 0;
                int j = 0;
                string strT1 = "";
                string strT2 = "";

                strT1 = strDateKey;
                intCount = strT1.Length;
                strKey = "8735649102";      // 秘钥对应 0123456789

                for (i = 0; i < intCount; i++)
                {
                    strT2 = strT1.Substring(i, 1);

                    for (j = 0; j < 10; j++)
                    {
                        if (strT2 == strKey.Substring(j, 1))
                        {
                            break;
                        }
                    }

                    if (j >= 10)
                    {
                        return "";
                    }

                    strDate += j.ToString();
                }

                return strDate;
            }

            // 生成注册码(=32位MD5编码左8位+开始日期[年月日]+截止日期[年月日]+32位MD5编码右8位)
            public static string CreateRegisterKey(string strUserID, string strCompanyName, string strDeviceID, string strFromDate, string strToDate)
            {
                string strRegisterKey = "";
                string strRegisterKey1 = "";
                string strKey = "";
                string strRegisterKey2 = "";
                string strRegisterKey3 = "";

                strKey = "4658476950832190";    // 秘钥,16位字符
                strRegisterKey1 = Crypt.Md5(strUserID + strCompanyName + strDeviceID, strKey);  // 对用户标识和公司名称和设备ID进行二次加密
                strRegisterKey2 = SoftRegister.CreateDateKey(strFromDate);
                strRegisterKey3 = SoftRegister.CreateDateKey(strToDate);

                strRegisterKey = strRegisterKey1.Substring(0, 8) + strRegisterKey2 + strRegisterKey3 + strRegisterKey1.Substring(24, 8);

                return strRegisterKey;
            }

            // 生成数据库MD5码
            public static string CreateDataBaseMD5Code(RegisterKey key)
            {
                string strTemp = "";
                string strKey = "";
                string strDataBaseKey = "";

                strTemp = key.strKey;
                strTemp += key.intCurrentYear.ToString();
                strTemp += key.intCurrentMonth.ToString();
                strTemp += key.intCurrentDay.ToString();
                strTemp += key.intCount.ToString();
                strTemp += key.intFromYear.ToString();
                strTemp += key.intFromMonth.ToString();
                strTemp += key.intFromDay.ToString();
                strTemp += key.intToYear.ToString();
                strTemp += key.intToMonth.ToString();
                strTemp += key.intToDay.ToString();

                strKey = "7136784254695230";    // 秘钥,16位字符
                strDataBaseKey = Crypt.Md5(strTemp, strKey);  // 对用户注册信息进行第三次加密

                return strDataBaseKey;
            }

            // 判断是否是付费用户,
            public static bool IsPayUser()
            {
                // 付费用户列表
                ArrayList list = new ArrayList();
                list.Add("01281060871734320352256736337363");     // 杨子光笔记本,T420,01号设备
                list.Add("65391196763253405584492404313433");     // 杨子光笔记本,T460,01号设备

                list.Add("04021209253442423249540435621217");     // 益通公司测试机
                list.Add("18713111815675203741340286950161");     // 益通公司测试机
                list.Add("31034828548415408097195211620325");     // 益通公司测试机
                list.Add("53156422684123903375100989276239");     // 益通公司测试机
                list.Add("04307041612464123221212328089386");     // 益通公司测试机
                list.Add("84834818945312212715062115843724");     // 益通公司测试机

                // 获取用户标识
                string strUserID = SoftRegister.CreateUserID();

                // 校验用户标识是否已经在付费用户列表中
                bool b = list.Contains(strUserID);

                return b;
            }

        }
    }
}
