/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_VirtualMachine
        {
            public string version
            {
                get
                {
                    return "1.1.0 FREE";
                }
            }
            public static bool bPrintError = true;
            public CSL_VirtualMachine()
            {
                RegType(new CSL_Type_Int());
                RegType(new CSL_Type_UInt());
                RegType(new CSL_Type_Float());
                RegType(new CSL_Type_Double());
                RegType(new CSL_Type_String());
                RegType(new CSL_Type_Var());
                RegType(new CSL_Type_Bool());
                RegType(new CSL_Type_Lambda());
                RegType(new CSL_Type_Delegate());
                RegType(new CSL_Type_Byte());
                RegType(new CSL_Type_Char());
                RegType(new CSL_Type_UShort());
                RegType(new CSL_Type_Sbyte());
                RegType(new CSL_Type_Short());
                RegType(new CSL_Type_Long());
                RegType(new CSL_Type_ULong());
                RegType(new CSL_Type_Object());

                typess["null"] = new CSL_Type_NULL();
#if UNITY_EDITOR
                typesss[typess["null"]] = "null";
#endif
                mBuildInAssemblys.Add("mscorlib");
                mBuildInAssemblys.Add("Assembly-CSharp");
                mBuildInAssemblys.Add("UnityEngine");
                mBuildInAssemblys.Add("UnityEngine.UI");
            }
            public bool Load(byte[] buff)
            {
                if (buff.Length <= 192)
                {
                    UnityEngine.Debug.Log("HotUpdateManager.Load error: invalid buff length.");
                    return false;
                }
                byte[] key = new byte[32];
                Array.Copy(buff, 135, key, 0, key.Length);
                byte[] iv = new byte[16];
                Array.Copy(buff, 170, iv, 0, iv.Length);
                byte[] buffEx = new byte[buff.Length - 192];
                Array.Copy(buff, 192, buffEx, 0, buffEx.Length);
                CSL_StreamRead stream = new CSL_StreamRead(CSL_Utils.Decompress(CSL_Utils.Decrypt(key, iv, buffEx)));
                stream.pos = stream.buff.Length - 10;
                if (CSL_Utils.GetMD5(stream.buff, 0, stream.pos).Substring(10, 8) != stream.ReadString())
                {
                    UnityEngine.Debug.Log("HotUpdateManager.Load error: invalid file hash.");
                    return false;
                }
                stream.pos = 0;
                UnityEngine.Debug.Log("C#Like version=" + stream.ReadInt32());
                UnityEngine.Debug.Log("C#Like type=" + stream.ReadBoolean());
                Dictionary<string, string> types;
                stream.Read(out types);
                foreach (var item in types.Keys)
                    stream.types.Add(item);
                int count = stream.ReadInt32();
                List<CSL_Type_Class> classs = new List<CSL_Type_Class>();
                for (int i = 0; i < count; i++)
                {
                    CSL_Type_Class st = new CSL_Type_Class(stream.ReadString(), stream.ReadBoolean(), stream.ReadString(), stream.ReadString());
                    RegType(st);
                    classs.Add(st);
                }
                foreach (var item in types)
                {
                    if (!string.IsNullOrEmpty(item.Value))
                        GetMyIType(item.Key, item.Value);
                }
                foreach (var item in types)
                {
                    if (string.IsNullOrEmpty(item.Value))
                        GetMyIType(item.Key);
                }
                foreach (var st in classs)
                {
                    st.Read(stream);
                }
                return true;
            }

            Dictionary<CSL_Type, CSL_TypeBase> types = new Dictionary<CSL_Type, CSL_TypeBase>();
            public Dictionary<string, CSL_TypeBase> typess = new Dictionary<string, CSL_TypeBase>();
#if UNITY_EDITOR
            public Dictionary<CSL_TypeBase, string> typesss = new Dictionary<CSL_TypeBase, string>();
            public string GetKeywordByIType(CSL_TypeBase type)
            {
                string ret;
                if (!typesss.TryGetValue(type, out ret))
                {
                    //Debug.LogError("not exist type :"+type.keyword);
                    return type.Keyword;
                }
                return ret;
            }
            public string GetKeywordByType(CSL_Type type)
            {
                return GetKeywordByIType(GetType(type));
            }
            public delegate void OnRegType(string keyword);
            public event OnRegType onRegType;
#endif
            public static Type GetMyType(string typeName)
            {
                return Type.GetType(typeName);
            }
            public void RegType(CSL_TypeBase type, string keyword = "")
            {
                //if (types.ContainsKey(type.type))
                //    return;
                types[type.type] = type;
                string typeName = keyword;
                if (string.IsNullOrEmpty(typeName))
                    typeName = type.FullName;
                typeCaches[typeName] = type;

                if (string.IsNullOrEmpty(typeName))
                {
                }
                else
                {
                    typess[typeName] = type;
#if UNITY_EDITOR
                    typesss[type] = typeName;
                    if (onRegType != null)
                        onRegType(typeName);
#endif
                }
            }
            public CSL_Content CreateContent()
            {
                return new CSL_Content(this);
            }

            public CSL_TypeBase GetType(CSL_Type type)
            {
                if (type == null)
                    return typess["null"];

                CSL_TypeBase ret;
                if (!types.TryGetValue(type, out ret))
                {
                    ret = RegHelper_Type.MakeType(type, type.FullName);
                    RegType(ret);
                }
                return ret;
            }
            Dictionary<string, CSL_TypeBase> typeCaches = new Dictionary<string, CSL_TypeBase>();
            Assembly[] mAssemblys = AppDomain.CurrentDomain.GetAssemblies();
            List<string> mBuildInAssemblys = new List<string>();
            public CSL_TypeBase GetMyIType(string str, string ex = null)
            {
                CSL_TypeBase ret;
                if (typeCaches.TryGetValue(str, out ret))
                    return ret;
                Type t = null;
                if (ex != null)
                {
                    t = Type.GetType(ex);
                    if (t == null)
                    {
                        foreach (var item in mBuildInAssemblys)
                        {
                            t = Type.GetType(str + "," + item);
                            if (t != null)
                                break;
                        }
                    }
                }
                else
                {
                    if (str.EndsWith(">") || str.EndsWith("]") || str.EndsWith("?"))
                    {
                        ret = GetTypeByKeyword(str);
                        typeCaches[str] = ret;
                        return ret;
                    }
                    foreach (var item in mBuildInAssemblys)
                    {
                        t = Type.GetType(str + "," + item);
                        if (t != null)
                            break;
                    }
                }
                if (t == null)
                {
                    foreach (var item in mAssemblys)
                    {
                        t = Type.GetType(str + "," + item.GetName());
                        if (t != null)
                            break;
                    }
                }
                if (t != null)
                {
                    string keyword = t.FullName;
                    if (keyword[keyword.Length - 2] == '`')
                        keyword = keyword.Substring(0, keyword.Length - 2);
                    ret = RegHelper_Type.MakeType(t, keyword);
                    RegType(ret);
                    if (!typess.ContainsKey(keyword))
                        typess[keyword] = ret;
                }
                else
                {
                    ret = GetTypeByKeyword(str);
                }
                typeCaches[str] = ret;
                return ret;
            }
            int depthGetTypeByKeyword = 0;
            public CSL_TypeBase GetTypeByKeyword(string keyword)
            {
                try
                {
                    ++depthGetTypeByKeyword;
                    if (depthGetTypeByKeyword >= 10)//prevent dead loop
                    {
                        UnityEngine.Debug.LogError("GetTypeByKeyword(" + keyword + ") may be dead loop!");
                        return null;
                    }
                    if (string.IsNullOrEmpty(keyword))
                    {
                        return null;
                    }
                    CSL_TypeBase ret;
                    if (!typess.TryGetValue(keyword, out ret))
                    {
                        if (keyword[keyword.Length - 1] == '>')
                        {
                            int iis = keyword.IndexOf('<');
                            string func = keyword.Substring(0, iis);
                            List<string> _types = new List<string>();
                            int istart = iis + 1;
                            int inow = istart;
                            int dep = 0;
                            while (inow < keyword.Length)
                            {
                                if (keyword[inow] == '<')
                                {
                                    dep++;
                                }
                                if (keyword[inow] == '>')
                                {
                                    dep--;
                                    if (dep < 0)
                                    {
                                        _types.Add(keyword.Substring(istart, inow - istart));
                                        break;
                                    }
                                }

                                if (keyword[inow] == ',' && dep == 0)
                                {
                                    _types.Add(keyword.Substring(istart, inow - istart));
                                    istart = inow + 1;
                                    inow = istart;
                                    continue; ;
                                }

                                inow++;
                            }

                            if (!typess.ContainsKey(func))
                            {
                                GetMyIType(func + "`" + _types.Count);
                            }

                            if (typess.ContainsKey(func))
                            {
                                Type gentype = GetTypeByKeyword(func).type;
                                if (gentype.IsGenericTypeDefinition)
                                {
                                    Type[] types = new Type[_types.Count];
                                    for (int i = 0; i < types.Length; i++)
                                    {
                                        CSL_Type t = GetTypeByKeyword(_types[i]).type;
                                        Type rt = t;
                                        if (rt == null && t != null)
                                        {
                                            //rt = typeof(object);
                                            rt = typeof(SInstance);
                                        }
                                        types[i] = rt;
                                    }
                                    Type IType = gentype.MakeGenericType(types);
                                    RegType(RegHelper_Type.MakeType(IType, keyword), keyword);
                                    return GetTypeByKeyword(keyword);
                                }
                            }
                        }
                        else if (keyword[keyword.Length - 1] == ']')
                        {
                            int iis = keyword.IndexOf('[');
                            string func = keyword.Substring(0, iis);
                            string arr = keyword.Substring(iis);
                            int count = 0;
                            for (int i = 0; i < arr.Length; i++)
                            {
                                if (arr[i] == ',')
                                    count++;
                            }

                            Type t;
                            if (!typess.ContainsKey(func))
                            {
                                t = Type.GetType(func);
                                if (t != null)
                                {
                                    RegType(RegHelper_Type.MakeType(t, func), func);
                                }
                                else
                                {
                                    UnityEngine.Debug.LogError("not register:" + keyword + ", invalid " + func);
                                    return ret;
                                }
                            }
                            else
                            {
                                CSL_TypeBase iType = GetTypeByKeyword(func);
                                if (iType.type != null)
                                {
                                    t = iType.type;
                                    if (t == null)
                                    {
                                        t = typeof(SInstance);
                                    }
                                }
                                else
                                    t = null;
                            }
                            if (t == null)
                            {
                                UnityEngine.Debug.LogError("not register:" + keyword + ", type null");
                                return ret;
                            }
                            Type ta;
                            if (count == 0)
                                ta = t.MakeArrayType();
                            else
                                ta = t.MakeArrayType(count);
                            RegType(RegHelper_Type.MakeType(ta, keyword), keyword);
                            return GetTypeByKeyword(keyword);
                        }
                        else
                        {
                            Type t = Type.GetType(keyword);
                            if (t != null)
                            {
                                RegType(RegHelper_Type.MakeType(t, keyword), keyword);
                            }
                        }
                        ret = GetMyIType(keyword);
                        if (ret == null)
                            UnityEngine.Debug.LogError("not register:" + keyword);
                    }

                    return ret;
                }
                finally
                {
                    --depthGetTypeByKeyword;
                }
            }
            public CSL_TypeBase GetTypeByKeywordQuiet(string keyword)
            {
                CSL_TypeBase ret;
                if (typess.TryGetValue(keyword, out ret))
                    return ret;
                return null;
            }
        }
    }
}
