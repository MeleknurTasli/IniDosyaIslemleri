using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace IniDosyaIslemleri
{
    public class IniDosyaIslemleri
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Kategori, string Ad, string Deger, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode/*, SetLastError = true*/)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        static extern long WritePrivateProfileString(string lpAppName, string KeyName, string Deger, string lpFileName);

        /// <summary>
        /// INI DOSYASI YOKSA OLUŞTURURARAK, AD VE KARŞISINA DEGERİNİ YAZAR, KAYDEDER. DEGERI SAYIYSA BİLE STRING'E ÇEVRİLMELİDİR.
        /// </summary>
        /// <param name="Ad"></param>
        /// <param name="Deger"></param>
        /// <param name="INIFileName"></param>
        public static void WriteINI(string Ad, string Deger, string INIFileName)
        {
            WritePrivateProfileString("", Ad, Deger, INIFileName);
        }

        static string alinan;

        /// <summary>
        /// INI Dosyasından veri okur. Parametredeki yazan veriyi dosyada bulur ve karşısındaki degeri string olarak getirir.
        /// </summary>
        /// <param name="Ad"></param>
        /// <returns></returns>
        public static string GetValueFromIniFile(string Ad, string fileName)
        {
            if (File.Exists(System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName))
            {
                StringBuilder sb = new StringBuilder(5000);
                GetPrivateProfileString("", Ad, "", sb, sb.Capacity, System.AppDomain.CurrentDomain.BaseDirectory + "\\" + fileName);
                alinan = sb.ToString();
                sb.Clear();
                return alinan;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Şifreleme yapar. String deger döndürür. Örnek kullanımı:
        /// text1 = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\Ayarlar.ini");
        /// text1 = Encrypt(key, text1);
        /// File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\Ayarlar.ini", text1);
        /// </summary>
        
        /// <param name="key"></param>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            string key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }


        /// <summary>
        /// Şifre çözme yapar. String deger döndürür. Örnek kullanımı:
        /// text1 = File.ReadAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\Ayarlar.ini");
        /// text1 = Decrypt(key, text1);
        /// File.WriteAllText(System.AppDomain.CurrentDomain.BaseDirectory + "\\Ayarlar.ini", text1);
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            string key = "b14ca5898a4e4133bbce2ea2315a1916";
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

    }
}
