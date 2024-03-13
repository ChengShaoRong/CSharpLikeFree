/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CSharpLikeEditor
{
    public class CSL_Config
    {
        string file;
        bool modify;
        public CSL_Config(string strFile)
        {
            file = strFile;
            modify = false;
            dictionary = new Dictionary<string, string>();
            if (File.Exists(file))
            {
                string[] strs = File.ReadAllLines(file, Encoding.UTF8);
                foreach (var str in strs)
                {
                    if (!string.IsNullOrEmpty(str))
                    {
                        int i = str.IndexOf(':');
                        if (i>0)
                            dictionary.Add(str.Substring(0, i), str.Substring(i+1));
                    }
                }
            }
        }
        public void Clear()
        {
            if (dictionary.Count > 0)
            {
                dictionary.Clear();
                modify = true;
            }
        }
        Dictionary<string, string> dictionary;
        public string GetString(string key, string strDefault = "")
        {
            string value;
            if (dictionary.TryGetValue(key, out value))
                return value;
            return strDefault;
        }
        public void SetString(string key, string value)
        {
            string valueOld;
            if (dictionary.TryGetValue(key, out valueOld))
            {
                if (valueOld != value)
                {
                    dictionary[key] = value;
                    modify = true;
                }
            }
            else
            {
                dictionary.Add(key, value);
                modify = true;
            }
        }
        public int GetInt(string key, int iDefault = 0)
        {
            return Convert.ToInt32(GetString(key, iDefault.ToString()));
        }
        public void SetInt(string key, int value)
        {
            SetString(key, value.ToString());
        }
        public long GetLong(string key, long iDefault = 0)
        {
            return Convert.ToInt64(GetString(key, iDefault.ToString()));
        }
        public void SetLong(string key, long value)
        {
            SetString(key, value.ToString());
        }
        public double GetDouble(string key, double iDefault = 0)
        {
            return Convert.ToDouble(GetString(key, iDefault.ToString()), CSharpLike.MyCustomConfig.cultureInfoForConvertSingleAndDouble);
        }
        public void SetDouble(string key, double value)
        {
            SetString(key, value.ToString());
        }
        public bool GetBoolean(string key, bool bDefault = false)
        {
            return Convert.ToBoolean(GetString(key, bDefault.ToString()));
        }
        public void SetBoolean(string key, bool value)
        {
            SetString(key, value.ToString());
        }
        public bool GetFloat(string key, float fDefault = 0.0f)
        {
            return Convert.ToBoolean(GetString(key, fDefault.ToString("F6")));
        }
        public void SetFloat(string key, float value)
        {
            SetString(key, value.ToString("F6"));
        }
        public void Save()
        {
            if (modify)
            {
                StringBuilder str = new StringBuilder();
                foreach(var one in dictionary)
                {
                    str.AppendFormat("{0}:{1}\n", one.Key, one.Value);
                }
                File.WriteAllText(file, str.ToString());
            }
        }
    }
}
