/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.IO.Compression;

namespace CSharpLike
{
    public class CSL_Utils
    {
        #region MD5 Utils
        public static string GetMD5(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            return GetMD5(Encoding.UTF8.GetBytes(str));
        }
        static MD5 md5 = new MD5CryptoServiceProvider();
        public static string GetMD5(byte[] data, int offset = 0, int count = 0)
        {
            if (count <= 0)
                count = data.Length;
            byte[] targetData = md5.ComputeHash(data, offset, count);
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < targetData.Length; i++)
                strBuilder.AppendFormat("{0:x2}", targetData[i]);
            return strBuilder.ToString();
        }
        public static string GetMD5(FileStream stream)
        {
            byte[] targetData = md5.ComputeHash(stream);
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < targetData.Length; i++)
                strBuilder.AppendFormat("{0:x2}", targetData[i]);
            return strBuilder.ToString();
        }
        public static string GetMD5ByFileName(string str)
        {
            using (FileStream fs = new FileStream(str, FileMode.Open, FileAccess.Read))
            {
                return GetMD5(fs);
            }
        }
        #endregion
        #region Compress/Decompress final binary file
        public static byte[] Compress(byte[] buff)
        {
            ////No compress,direct return.
            //return buff;

            //Compress with zip
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(buff, 0, buff.Length);
                    zipStream.Close();
                    return outStream.ToArray();
                }
            }
        }
        public static byte[] Decompress(byte[] buff)
        {
            ////No decompress,direct return.
            //return buff;

            //Decompress with zip
            using (MemoryStream inputStream = new MemoryStream(buff))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }
            }
        }
        #endregion
        #region Strongly recommended customize Encrypt/Decrypt final binary file, that call by the C#Like Editor
#if UNITY_EDITOR //This function just call by the C#Like Editor
        /// <summary>
        /// You should customize this Encrypt function to prevent other crack your hot update script code.
        /// If you not customize this function, everyone can easy to open your binary file and reverse to C# code.
        /// Default is Encrypt by AES.
        /// </summary>
        /// <param name="key">key is byte[32]</param>
        /// <param name="iv">iv is byte[16]</param>
        /// <param name="buff">the data to be encrypt</param>
        public static byte[] Encrypt(byte[] key, byte[] iv, byte[] buff)
        {
            ////Encrypt by AES,
            //Your special encrypt will make your hot update script binary file hard to be cracked.
            //You can encrypt the 'key' or the 'iv' or the 'buff', or change to other encryption algorithm.
            //Make sure your encryption algorithm is unique in the world
            // sample modify key (swap 2 bit)
            byte temp = key[key.Length / 3];
            key[key.Length / 3] = key[key.Length / 5];
            key[key.Length / 5] = temp;
            // sample modify iv (Reverse one bit)
            iv[iv.Length / 2] = (byte)~iv[iv.Length / 2];
            // AES Encrypt (You may change to other encryption algorithm)
            buff = (new RijndaelManaged()).CreateEncryptor(key, iv).TransformFinalBlock(buff, 0, buff.Length);
            // sample modify buff (1 bit self subtraction)
            buff[buff.Length / 13]--;
            return buff;
        }
#endif
        /// <summary>
        /// Decrypt data (Contrary to Encrypt).
        /// Default is Decrypt by AES.
        /// </summary>
        /// <param name="key">key is byte[32]</param>
        /// <param name="iv">iv is byte[16]</param>
        /// <param name="buff">the data to be decrypt</param>
        public static byte[] Decrypt(byte[] key, byte[] iv, byte[] buff)
        {
            ////Decrypt by AES
            // sample modify key (swap 2 bit)
            byte temp = key[key.Length / 3];
            key[key.Length / 3] = key[key.Length / 5];
            key[key.Length / 5] = temp;
            // sample modify iv (Reverse one bit)
            iv[iv.Length / 2] = (byte)~iv[iv.Length / 2];
            // sample modify buff (1 bit self add)
            buff[buff.Length / 13]++;
            // AES Decrypt (You may change to other decryption algorithm)
            return (new RijndaelManaged()).CreateDecryptor(key, iv).TransformFinalBlock(buff, 0, buff.Length);
        }
        #endregion
        /// <summary>
        /// Check send mask for FREE version due to FREE version not support bit operation.
        /// </summary>
        public static bool CheckSendMask(ulong sendmask, ulong mask)
        {
            return (sendmask & mask) > 0;
        }
    }
}
