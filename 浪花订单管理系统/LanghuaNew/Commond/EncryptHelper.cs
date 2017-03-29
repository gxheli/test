using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Commond
{
    public class EncryptHelper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Encode(string data)
        {
            string KEY_64 = "VavicApp";
            string IV_64 = "VavicApp";
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Decode(string data)
        {
            string KEY_64 = "VavicApp";
            string IV_64 = "VavicApp";
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);
            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
        //static List<KeyValuePair<char, char>> MappingList;
        ////加密
        //public static string Encrypt(string str)
        //{
        //    MappingList = new List<KeyValuePair<char, char>>();
        //    for (char c = '0'; c <= '9'; c++)
        //        MappingList.Add(new KeyValuePair<char, char>(c, (char)(c - '0' + 'a')));
        //    for (char c = 'a'; c <= 'f'; c++)
        //        MappingList.Add(new KeyValuePair<char, char>(c, (char)(c - 'a' + 'u')));
        //    return Encoding.ASCII.GetBytes(str)
        //    .Select((b, i) => (b ^ ((byte)(0xa0 + i))).ToString("x2"))
        //    .Aggregate("", (s, c) => s + c)
        //    .ToCharArray().Select(c => MappingList.First(kv => kv.Key == c).Value)
        //    .Aggregate("", (s, c) => s + c);
        //}

        ////解密
        //public static string Decrypt(string str)
        //{
        //    string base16 = str.ToCharArray()
        //    .Select(c => MappingList.First(kv => kv.Value == c).Key)
        //    .Aggregate("", (s, c) => s + c);
        //    return Encoding.ASCII.GetString((new byte[base16.Length / 2])
        //    .Select((b, i) => (byte)(Convert.ToByte(base16.Substring(i * 2, 2), 16) ^ ((byte)(0xa0 + i)))).ToArray());
        //}
        private static void CreateRSAKey()
        {

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            RSAParameters keys = rsa.ExportParameters(true);
            String pkxml = "<root>\n<Modulus>" + ToHexString(keys.Modulus) + "</Modulus>";
            pkxml += "\n<Exponent>" + ToHexString(keys.Exponent) + "</Exponent>\n</root>";
            String psxml = "<root>\n<Modulus>" + ToHexString(keys.Modulus) + "</Modulus>";
            psxml += "\n<Exponent>" + ToHexString(keys.Exponent) + "</Exponent>";
            psxml += "\n<D>" + ToHexString(keys.D) + "</D>";
            psxml += "\n<DP>" + ToHexString(keys.DP) + "</DP>";
            psxml += "\n<P>" + ToHexString(keys.P) + "</P>";
            psxml += "\n<Q>" + ToHexString(keys.Q) + "</Q>";
            psxml += "\n<DQ>" + ToHexString(keys.DQ) + "</DQ>";
            psxml += "\n<InverseQ>" + ToHexString(keys.InverseQ) + "</InverseQ>\n</root>";

            SaveToFile("publickey.xml", pkxml);
            SaveToFile("privatekey.xml", psxml);

        }
        private static RSACryptoServiceProvider CreateRSADEEncryptProvider(String privateKeyFile)
        {
            RSAParameters parameters1;
            parameters1 = new RSAParameters();
            StreamReader reader1 = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/" + privateKeyFile));
            XmlDocument document1 = new XmlDocument();
            document1.LoadXml(reader1.ReadToEnd());
            XmlElement element1 = (XmlElement)document1.SelectSingleNode("root");
            parameters1.Modulus = ReadChild(element1, "Modulus");
            parameters1.Exponent = ReadChild(element1, "Exponent");
            parameters1.D = ReadChild(element1, "D");
            parameters1.DP = ReadChild(element1, "DP");
            parameters1.DQ = ReadChild(element1, "DQ");
            parameters1.P = ReadChild(element1, "P");
            parameters1.Q = ReadChild(element1, "Q");
            parameters1.InverseQ = ReadChild(element1, "InverseQ");
            CspParameters parameters2 = new CspParameters();
            parameters2.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider(parameters2);
            provider1.ImportParameters(parameters1);
            reader1.Close();
            return provider1;
        }
        private static RSACryptoServiceProvider CreateRSAEncryptProvider(String publicKeyFile)
        {
            RSAParameters parameters1;
            parameters1 = new RSAParameters();
            StreamReader reader1 = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/" + publicKeyFile));
            XmlDocument document1 = new XmlDocument();
            document1.LoadXml(reader1.ReadToEnd());
            XmlElement element1 = (XmlElement)document1.SelectSingleNode("root");
            parameters1.Modulus = ReadChild(element1, "Modulus");
            parameters1.Exponent = ReadChild(element1, "Exponent");
            CspParameters parameters2 = new CspParameters();
            parameters2.Flags = CspProviderFlags.UseMachineKeyStore;
            RSACryptoServiceProvider provider1 = new RSACryptoServiceProvider(parameters2);
            provider1.ImportParameters(parameters1);
            reader1.Close();
            return provider1;
        }

        private static byte[] ReadChild(XmlElement parent, string name)
        {
            XmlElement element1 = (XmlElement)parent.SelectSingleNode(name);
            return hexToBytes(element1.InnerText);
        }

        private static string ToHexString(byte[] bytes) // 0xae00cf => "AE00CF "  
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
        private static byte[] hexToBytes(String src)
        {
            int l = src.Length / 2;
            String str;
            byte[] ret = new byte[l];

            for (int i = 0; i < l; i++)
            {
                str = src.Substring(i * 2, 2);
                ret[i] = Convert.ToByte(str, 16);
            }
            return ret;
        }

        private static void SaveToFile(String filename, String data)
        {
            string savePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/");
            if (!Directory.Exists(savePath))//判断上传文件夹是否存在，若不存在，则创建
            {
                Directory.CreateDirectory(savePath);//创建文件夹
            }
            string Path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/" + filename);
            System.IO.StreamWriter sw = System.IO.File.CreateText(Path);
            sw.WriteLine(data);
            sw.Close();
        }

        public static string Encrypt(string str)
        {
            RSACryptoServiceProvider rsaencrype = CreateRSAEncryptProvider("publickey.xml");
            String text = str;
            byte[] data = new UnicodeEncoding().GetBytes(text);
            byte[] endata = rsaencrype.Encrypt(data, true);
            return ToHexString(endata);
        }

        public static string Decrypt(string hexstr)
        {
            RSACryptoServiceProvider rsadeencrypt = CreateRSADEEncryptProvider("privatekey.xml");
            byte[] miwen = hexToBytes(hexstr);
            byte[] dedata = rsadeencrypt.Decrypt(miwen, true);
            return System.Text.UnicodeEncoding.Unicode.GetString(dedata);
        }
    }
}
