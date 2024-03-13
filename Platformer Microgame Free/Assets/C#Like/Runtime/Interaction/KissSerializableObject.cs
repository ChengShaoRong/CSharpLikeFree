/*
 *           C#Like
 * Copyright © 2024 RongRong
 */
using System;
using UnityEngine;
using System.Globalization;
using System.Reflection;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
#endif

namespace CSharpLike
{
    /// <summary>
    /// For build-in object, you should not modify this file.
    /// You can modify class KissSerializableObjec instead.
    /// Support serializable type :
    /// 1 Build-in type : byte/sbyte/short/ushort/int/uint/long/ulong/char/string/float/bool/double.
    /// 2 Class inherited from 'LikeBehaviour', e.g. your hot update class.
    /// 3 Class inherited from 'UnityEngine.Object', e.g. MonoBehaviour/GameObject/TextAsset/Material/Texture/AudioClip/,etc
    /// 4 Some common structs in UnityEngine, e.g. Vector2/Vector3/Vector4/Vector2Int/Vector3Int/Matrix4x4/Rect/RectInt/Bounds/BoundsInt/Color/Color32/Quaternion/LayerMask.
    /// 5 List&lt;AboveType1~4>, e.g. List&lt;int>/List&lt;GameObject>/List&lt;Vector3>/....
    /// Support PropertyAttribute:
    /// 1 [Range(1,10)]. e.g. for 'byte/sbyte/short/ushort/int/uint/long/ulong/float/double' and List&lt;byte/sbyte/short/ushort/int/uint/long/ulong/float/double>.
    /// 2 [HideInInspector].
    /// 3 [SerializeField].
    /// 4 [System.NonSerialized].
    /// 5 [Tooltip("XXXXX")].
    /// </summary>
    [Serializable]
    public class KissSerializableObject
    {
        public enum ObjectType
        {
            //Type 
            Value,
            UnityEngineObject,
            LikeBehaviourObject,
            //List<Type>
            Values,
            UnityEngineObjects,
            LikeBehaviourObjects,
            //Type[]
            Values2,
            UnityEngineObjects2,
            LikeBehaviourObjects2,
            //class
            Class,
            Classes
        }
        public string name;
        public string typeFullName;
        public UnityEngine.Object obj = null;
        public string value = "";
        public List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
        public List<string> values = new List<string>();
        public ObjectType objectType;
        //public KissSerializableObject _class;
        //public List<KissSerializableObject> _classes;
        public string TypeFullName => typeFullName.Replace("+",".");
        Type Type
        {
            get
            {
                if (string.IsNullOrEmpty(typeFullName))
                    return null;
                if (mCacheType.TryGetValue(typeFullName, out Type type))
                    return type;

                type = Type.GetType(typeFullName);
#if UNITY_EDITOR
                if (type == null)
                {
                    if (mAssemblys.Count == 0)
                        mAssemblys = GetAssemblys();
                    foreach (var item in mAssemblys)
                    {
                        type = Type.GetType(typeFullName + "," + item);
                        if (type != null)
                            break;
                    }
                    if (type == null)
                    {
                        string subTypeName = "";
                        if (typeFullName.StartsWith("System.Collections.Generic.List`1["))
                        {
                            subTypeName = typeFullName.Substring(34, typeFullName.Length - 35);
                        }
                        else if (typeFullName.EndsWith("[]"))
                        {
                            subTypeName = typeFullName.Substring(0, typeFullName.Length - 2);
                        }
                        if (subTypeName != "")
                        {
                            Type subType = Type.GetType(subTypeName);
                            if (subType == null)
                            {
                                foreach (var item in mAssemblys)
                                {
                                    subType = Type.GetType(subTypeName + "," + item);
                                    if (subType != null)
                                        break;
                                }
                                if (subType != null)
                                {
                                    if (typeFullName.EndsWith("[]"))
                                    {
                                        type = subType.MakeArrayType();
                                    }
                                    else
                                    {
                                        type = typeof(List<>).MakeGenericType(new Type[] { subType });
                                    }
                                }
                            }
                        }
                    }
                }
#endif
                if (type == null && Internal.CSL_VirtualMachine.Instance != null)
                {
                    string temp = TypeFullName;
                    if (temp.StartsWith("System.Collections.Generic.List`1["))
                    {
                        temp = temp.Replace("System.Collections.Generic.List`1[", "List<");
                        temp = temp.Substring(0, temp.Length-1)+">";
                    }
                    Internal.CSL_TypeBase btype = Internal.CSL_VirtualMachine.Instance.GetMyIType(temp);
                    if (btype != null)
                        type = btype.type;
                }
                mCacheType[typeFullName] = type;
                return type;
            }
        }
#if UNITY_EDITOR
        public List<string> GetAssemblys()
        {
            //don't remove this
            List<string> list = new List<string>();
            list.Add("mscorlib");//.Net API
            list.Add("Assembly-CSharp");//default user custom c# script assembly
            list.Add("UnityEngine");//unity engine core
            list.Add("UnityEngine.UI");//unity engine UI core

            //you may config here
            List<string> listIgnoreKeyWord = new List<string>();
            listIgnoreKeyWord.Add("editor");//we ignore the dll which name with the 'editor' keyword 

            string dataPath = Application.dataPath;
            //auto add other *.dll in '/Assets/' folder,exclude in the 'StreamingAssets' folder
            string[] files = Directory.GetFiles(dataPath, "*.dll", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (!file.Contains("/StreamingAssets"))//not in StreamingAssets folder
                {
                    string strFileName = file.Substring(Mathf.Max(file.LastIndexOf('/'), file.LastIndexOf('\\')) + 1);
                    strFileName = strFileName.Substring(0, strFileName.Length - 4);//remove '.dll'
                    if (!list.Contains(strFileName))
                    {
                        bool bIgnore = false;
                        foreach (string strIgnore in listIgnoreKeyWord)
                        {
                            if (strFileName.ToLower().Contains(strIgnore))
                            {
                                bIgnore = true;
                                break;
                            }
                        }
                        if (bIgnore)
                            continue;
                        list.Add(strFileName);
                    }
                }
            }
            //auto add other *.dll in '/Library/ScriptAssemblies/' folder
            files = Directory.GetFiles(dataPath.Substring(0, dataPath.Length - 6) + "/Library/ScriptAssemblies/", "*.dll", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string strFileName = file.Substring(Mathf.Max(file.LastIndexOf('/'), file.LastIndexOf('\\')) + 1);
                strFileName = strFileName.Substring(0, strFileName.Length - 4);
                if (!list.Contains(strFileName))
                {
                    bool bIgnore = false;
                    foreach (string strIgnore in listIgnoreKeyWord)
                    {
                        if (strFileName.ToLower().Contains(strIgnore))
                        {
                            bIgnore = true;
                            break;
                        }
                    }
                    if (bIgnore)
                        continue;
                    list.Add(strFileName);
                }
            }
            return list;
        }
#endif
        static List<string> mAssemblys = new List<string>();
        static Dictionary<string, Type> mCacheType = new Dictionary<string, Type>();
#if UNITY_EDITOR
        public bool IsHideInInspector { get; set; } = false;
        public RangeAttribute Range { get; set; } = null;
#endif

        SupportInfo supportInfo;


        public static Array ResizeArray(Array array, int newSize)
        {
            Array arrayNew = Activator.CreateInstance(array.GetType(), new object[] { newSize }) as Array;
            int oldCount = array.Length;
            if (newSize > 0 && oldCount > 0)
                Array.Copy(array, arrayNew, Mathf.Min(newSize, oldCount));
            return arrayNew;
        }

        static Dictionary<Type, Type> mSubTypes = new Dictionary<Type, Type>();
        public static Type GetSubType(Type type)
        {
            if (type == null)
                return null;
            if (mSubTypes.TryGetValue(type, out Type subType))
                return subType;
            if (type.IsArray)
                subType = type.Assembly.GetType(type.FullName.Substring(0, type.FullName.Length - 2));
            else
                subType = type.GenericTypeArguments[0];
            mSubTypes[type] = subType;
            return subType;
        }
        public KissSerializableObject(FieldInfo fieldInfo)
        {
            name = fieldInfo.Name;
            supportInfo = GetSupportInfo(fieldInfo.FieldType);
            typeFullName = fieldInfo.FieldType.FullName;
            if (typeFullName.StartsWith("System.Collections.Generic.List`1["))
            {
                ProcessFullName(ref typeFullName);
                Type type = fieldInfo.FieldType.GenericTypeArguments[0];
                if (type.IsSubclassOf(typeof(LikeBehaviour)))
                    objectType = ObjectType.LikeBehaviourObjects;
                else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                    objectType = ObjectType.UnityEngineObjects;
                else
                    objectType = ObjectType.Values;
            }
            else if (typeFullName.EndsWith("[]"))
            {
                //ProcessFullName(ref typeFullName);
                Type type = GetSubType(fieldInfo.FieldType);
                if (type.IsSubclassOf(typeof(LikeBehaviour)))
                    objectType = ObjectType.LikeBehaviourObjects2;
                else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                    objectType = ObjectType.UnityEngineObjects2;
                else
                    objectType = ObjectType.Values2;
            }
            else
            {
                if (fieldInfo.FieldType.IsSubclassOf(typeof(LikeBehaviour)))
                    objectType = ObjectType.LikeBehaviourObject;
                else if (fieldInfo.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
                    objectType = ObjectType.UnityEngineObject;
                else
                    objectType = ObjectType.Value;
            }
        }
        static Array ListToArray(System.Collections.IList list)
        {
            int count = list.Count;
            Array array = Activator.CreateInstance(list.GetType().GetGenericArguments()[0].MakeArrayType(), new object[] { count }) as Array;
            for (int i = 0; i < count; i++)
                array.SetValue(list[i], i);
            return array;
        }
        class SupportInfo
        {
            public MethodInfo miGet;
            public MethodInfo miSet;
        }
        static Dictionary<Type, SupportInfo> mSupportTypes = null;
        static SupportInfo GetSupportInfo(Type type)
        {
            if (mSupportTypes == null) 
                mSupportTypes = new Dictionary<Type, SupportInfo>();
            if (mSupportTypes.TryGetValue(type, out SupportInfo supportInfo))
                return supportInfo;
            Type typeKissSerializableObject = typeof(KissSerializableObject);
            supportInfo = new SupportInfo();
            if (type.FullName.StartsWith("System.Collections.Generic.List`1["))
            {
                Type type1 = type.GetGenericArguments()[0];
                if (!type1.IsSubclassOf(typeof(LikeBehaviour)) && !type1.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    string typeName = "List" + type1.FullName.Replace("System.", "").Replace("UnityEngine.", "").Replace(".", "").Replace("+", "");
                    supportInfo.miGet = typeKissSerializableObject.GetMethod($"Get{typeName}Value");
                    if (supportInfo.miGet != null)
                        supportInfo.miSet = typeKissSerializableObject.GetMethod($"Set{typeName}Value");
                    if (supportInfo.miSet == null)
                        supportInfo = null;
                }
            }
            else if (type.FullName.EndsWith("[]"))
            {
                Type type1 = GetSubType(type);
                if (!type1.IsSubclassOf(typeof(LikeBehaviour)) && !type1.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    string typeName = "List" + type1.FullName.Replace("System.", "").Replace("UnityEngine.", "").Replace(".", "").Replace("+", "");
                    supportInfo.miGet = typeKissSerializableObject.GetMethod($"Get{typeName}Value");
                    if (supportInfo.miGet != null)
                        supportInfo.miSet = typeKissSerializableObject.GetMethod($"Set{typeName}Value");
                    if (supportInfo.miSet == null)
                        supportInfo = null;
                }
            }
            else
            {
                if (!type.IsSubclassOf(typeof(LikeBehaviour)) && !type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    string typeName = type.FullName.Replace("System.", "").Replace("UnityEngine.", "").Replace(".", "").Replace("+", "");
                    supportInfo.miGet = typeKissSerializableObject.GetMethod($"Get{typeName}Value");
                    if (supportInfo.miGet != null)
                        supportInfo.miSet = typeKissSerializableObject.GetMethod($"Set{typeName}Value");
                    if (supportInfo.miSet == null)
                        supportInfo = null;
                }
            }
            mSupportTypes[type] = supportInfo;
            return supportInfo;
        }
        static void ProcessFullName(ref string str)
        {
            str = str.Substring(35);
            int i = str.IndexOf(',');
            if (i > 0)
                str = str.Substring(0, i);
            else if (str.EndsWith("]"))
                str = str.Substring(0, str.Length - 1);
            str = "System.Collections.Generic.List`1[" + str + "]";
        }
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        static void OnDidReloadScripts()
        {
            mSupportTypes = null;
        }
#endif
        public static bool IsSupportType(Type type)
        {
            return GetSupportInfo(type) != null;
        }
        public bool IsLikeBehaviour => objectType == ObjectType.LikeBehaviourObject;

        public object Value
        {
            get
            {
                switch(objectType)
                {
                    case ObjectType.LikeBehaviourObject:
                        {
                            HotUpdateBehaviour hotUpdateBehaviour = BehaviourValue;
                            object value = null;
                            if (hotUpdateBehaviour != null)
                            {
                                value = hotUpdateBehaviour.ScriptInstance;
                                if (value == null)
                                {
                                    hotUpdateBehaviour._ForceInit();
                                    value = hotUpdateBehaviour.ScriptInstance;
                                }
                            }
                            return value;
                        }
                    case ObjectType.LikeBehaviourObjects:
                        {
                            System.Collections.IList list = Activator.CreateInstance(Type) as System.Collections.IList;
                            foreach(UnityEngine.Object _obj in objs)
                            {
                                HotUpdateBehaviour hub = _obj as HotUpdateBehaviour;
                                object _obj_ = null;
                                if (hub != null && hub.gameObject != null)
                                {
                                    foreach (HotUpdateBehaviour behaviour in hub.gameObject.GetComponents<HotUpdateBehaviour>())
                                    {
                                        if (behaviour.bindHotUpdateClassFullName == typeFullName)
                                        {
                                            _obj_ = behaviour.ScriptInstance;
                                            if (_obj_ == null)
                                            {
                                                behaviour._ForceInit();
                                                _obj_ = behaviour.ScriptInstance;
                                            }
                                            break;
                                        }
                                    }
                                }
                                list.Add(_obj_);
                            }
                            return list;
                        }
                    case ObjectType.LikeBehaviourObjects2:
                        {
                            int count = objs.Count;
                            Array array = Activator.CreateInstance(Type, new object[] { count }) as Array;
                            for (int i = 0; i < count; i++)
                            {
                                UnityEngine.Object _obj = objs[i];
                                HotUpdateBehaviour hub = _obj as HotUpdateBehaviour;
                                object _obj_ = null;
                                if (hub != null && hub.gameObject != null)
                                {
                                    foreach (HotUpdateBehaviour behaviour in hub.gameObject.GetComponents<HotUpdateBehaviour>())
                                    {
                                        if (behaviour.bindHotUpdateClassFullName == typeFullName)
                                        {
                                            _obj_ = behaviour.ScriptInstance;
                                            if (_obj_ == null)
                                            {
                                                behaviour._ForceInit();
                                                _obj_ = behaviour.ScriptInstance;
                                            }
                                            break;
                                        }
                                    }
                                }
                                array.SetValue(_obj_, i);
                            }
                            return array;
                        }
                    case ObjectType.UnityEngineObject:
                        return obj;
                    case ObjectType.UnityEngineObjects:
                        {
                            System.Collections.IList list = Activator.CreateInstance(Type) as System.Collections.IList;
                            foreach (UnityEngine.Object _obj in objs)
                            {
                                list.Add(_obj);
                            }
                            return list;
                        }
                    case ObjectType.UnityEngineObjects2:
                        {
                            int count = objs.Count;
                            Array array = Activator.CreateInstance(Type, new object[] { count }) as Array;
                            for (int i = 0; i < count; i++)
                                array.SetValue(objs[i], i);
                            return array;
                        }
                    case ObjectType.Value:
                        if (supportInfo == null)
                            supportInfo = GetSupportInfo(Type);
                        if (supportInfo != null && supportInfo.miGet != null)
                            return supportInfo.miGet.Invoke(this, null);
                        Debug.LogError($"Not support serializable type '{typeFullName}'");
                        return Activator.CreateInstance(Type);
                    case ObjectType.Values:
                        if (supportInfo == null)
                            supportInfo = GetSupportInfo(Type);
                        if (supportInfo != null && supportInfo.miGet != null)
                            return supportInfo.miGet.Invoke(this, null);
                        Debug.LogError($"Not support serializable type '{typeFullName}'");
                        return Activator.CreateInstance(Type);
                    case ObjectType.Values2:
                        if (supportInfo == null)
                            supportInfo = GetSupportInfo(Type);
                        if (supportInfo != null && supportInfo.miGet != null)
                        {
                            return ListToArray(supportInfo.miGet.Invoke(this, null) as System.Collections.IList);
                        }
                        Debug.LogError($"Not support serializable type '{typeFullName}'");
                        return Activator.CreateInstance(Type);
                    default:
                        return null;
                }
            }
            set
            {
                switch(objectType)
                {
                    case ObjectType.LikeBehaviourObject:
                        {
                            LikeBehaviour likeBehaviour = value as LikeBehaviour;
                            if (likeBehaviour != null && likeBehaviour.behaviour != null)
                            {
                                obj = likeBehaviour.behaviour.gameObject;

                            }
                            else
                            {
                                Internal.SInstance instance = value as Internal.SInstance;
                                if (instance != null && instance.likeBehaviour != null)
                                {
                                    obj = instance.likeBehaviour.gameObject;
                                }
                                else
                                    obj = null;
                            }
                        }
                        break;
                    case ObjectType.LikeBehaviourObjects:
                    case ObjectType.LikeBehaviourObjects2:
                        {
                            if (objs != value)
                            {
                                objs.Clear();
                                System.Collections.IList list = value as System.Collections.IList;
                                if (list != null)
                                {
                                    foreach (object _obj in list)
                                    {
                                        HotUpdateBehaviour hub = _obj as HotUpdateBehaviour;
                                        UnityEngine.Object _obj_ = null;
                                        if (hub != null && hub.gameObject != null)
                                        {
                                            foreach (HotUpdateBehaviour behaviour in hub.gameObject.GetComponents<HotUpdateBehaviour>())
                                            {
                                                if (behaviour.bindHotUpdateClassFullName == typeFullName)
                                                {
                                                    _obj_ = behaviour.ScriptInstance as UnityEngine.Object;
                                                    break;
                                                }
                                            }
                                        }
                                        objs.Add(_obj_);
                                    }
                                }
                            }
                        }
                        break;
                    case ObjectType.UnityEngineObject:
                        obj = (UnityEngine.Object)value;
                        break;
                    case ObjectType.UnityEngineObjects:
                    case ObjectType.UnityEngineObjects2:
                        {
                            if (objs != value)
                            {
                                objs.Clear();
                                System.Collections.IList list = value as System.Collections.IList;
                                if (list != null)
                                {
                                    foreach (UnityEngine.Object _obj in list)
                                    {
                                        objs.Add(_obj);
                                    }
                                }
                            }
                        }
                        break;
                    case ObjectType.Value:
                        if (supportInfo == null)
                            supportInfo = GetSupportInfo(Type.GetType(typeFullName));
                        if (supportInfo != null && supportInfo.miSet != null)
                            supportInfo.miSet.Invoke(this, new object[] { value });
                        else
                            Debug.LogError($"Not support serializable type '{typeFullName}'");
                        break;
                    case ObjectType.Values:
                        if (supportInfo == null)
                            supportInfo = GetSupportInfo(Type.GetType(typeFullName));
                        if (value == null)
                            values.Clear();
                        else if (supportInfo != null && supportInfo.miSet != null)
                            supportInfo.miSet.Invoke(this, new object[] { value });
                        else
                            Debug.LogError($"Not support serializable type '{typeFullName}'");
                        break;
                    case ObjectType.Values2:
                        if (supportInfo == null)
                            supportInfo = GetSupportInfo(Type.GetType(typeFullName));
                        if (value == null)
                            values.Clear();
                        else if (supportInfo != null && supportInfo.miSet != null)
                        {
                            Type type = typeof(List<>).MakeGenericType(new Type[] { GetSubType(Type) });
                            supportInfo.miSet.Invoke(this, new object[] { Activator.CreateInstance(type, new object[] { value }) });
                        }
                        else
                            Debug.LogError($"Not support serializable type '{typeFullName}'");
                        break;
                    default:
                        break;
                }
            }
        }
        public HotUpdateBehaviour BehaviourValue
        {
            get
            {
                HotUpdateBehaviour hub = obj as HotUpdateBehaviour;
                if (hub != null && hub.gameObject != null)
                {
                    foreach (HotUpdateBehaviour behaviour in hub.gameObject.GetComponents<HotUpdateBehaviour>())
                    {
                        if (behaviour.bindHotUpdateClassFullName == typeFullName)
                            return behaviour;
                    }
                }
                return null;
            }
            set => obj = value != null ? value.gameObject : null;
        }
        public long GetInt64Value()
        {
            try { return Convert.ToInt64(value); } catch { }
            return default;
        }
        public void SetInt64Value(long newValue)
        {
            value = newValue.ToString();
        }
        public List<long> GetListInt64Value()
        {
            List<long> list = new List<long>();
            foreach (string str in values)
            {
                long v;
                try { v = Convert.ToInt64(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListInt64Value(List<long> newValue)
        {
            values.Clear();
            foreach (long i in newValue)
                values.Add(i.ToString());
        }
        public ulong GetUInt64Value()
        {
            try { return Convert.ToUInt64(value); } catch { }
            return default;
        }
        public void SetUInt64Value(ulong newValue)
        {
            value = newValue.ToString();
        }
        public List<ulong> GetListUInt64Value()
        {
            List<ulong> list = new List<ulong>();
            foreach (string str in values)
            {
                ulong v;
                try { v = Convert.ToUInt64(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListUInt64Value(List<ulong> newValue)
        {
            values.Clear();
            foreach (ulong i in newValue)
                values.Add(i.ToString());
        }
        public int GetInt32Value()
        {
            try { return Convert.ToInt32(value); } catch { }
            return default;
        }
        public void SetInt32Value(int newValue)
        {
            value = newValue.ToString();
        }
        public List<int> GetListInt32Value()
        {
            List<int> list = new List<int>();
            foreach (string str in values)
            {
                int v;
                try { v = Convert.ToInt32(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListInt32Value(List<int> newValue)
        {
            values.Clear();
            foreach (int i in newValue)
                values.Add(i.ToString());
        }
        public uint GetUInt32Value()
        {
            try { return Convert.ToUInt32(value); } catch { }
            return default;
        }
        public void SetUInt32Value(uint newValue)
        {
            value = newValue.ToString();
        }
        public List<uint> GetListUInt32Value()
        {
            List<uint> list = new List<uint>();
            foreach (string str in values)
            {
                uint v;
                try { v = Convert.ToUInt32(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListUInt32Value(List<uint> newValue)
        {
            values.Clear();
            foreach (uint i in newValue)
                values.Add(i.ToString());
        }
        public short GetInt16Value()
        {
            try { return Convert.ToInt16(value); } catch { }
            return default;
        }
        public void SetInt16Value(short newValue)
        {
            value = newValue.ToString();
        }
        public List<short> GetListInt16Value()
        {
            List<short> list = new List<short>();
            foreach (string str in values)
            {
                short v;
                try { v = Convert.ToInt16(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListInt16Value(List<short> newValue)
        {
            values.Clear();
            foreach (short i in newValue)
                values.Add(i.ToString());
        }
        public ushort GetUInt16Value()
        {
            try { return Convert.ToUInt16(value); } catch { }
            return default;
        }
        public void SetUInt16Value(ushort newValue)
        {
            value = newValue.ToString();
        }
        public List<ushort> GetListUInt16Value()
        {
            List<ushort> list = new List<ushort>();
            foreach (string str in values)
            {
                ushort v;
                try { v = Convert.ToUInt16(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListUInt16Value(List<ushort> newValue)
        {
            values.Clear();
            foreach (ushort i in newValue)
                values.Add(i.ToString());
        }
        public byte GetByteValue()
        {
            try { return Convert.ToByte(value); } catch { }
            return default;
        }
        public void SetByteValue(byte newValue)
        {
            value = newValue.ToString();
        }
        public List<byte> GetListByteValue()
        {
            List<byte> list = new List<byte>();
            foreach (string str in values)
            {
                byte v;
                try { v = Convert.ToByte(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListByteValue(List<byte> newValue)
        {
            values.Clear();
            foreach (byte i in newValue)
                values.Add(i.ToString());
        }
        public sbyte GetSByteValue()
        {
            try { return Convert.ToSByte(value); } catch { }
            return default;
        }
        public void SetSByteValue(sbyte newValue)
        {
            value = newValue.ToString();
        }
        public List<sbyte> GetListSByteValue()
        {
            List<sbyte> list = new List<sbyte>();
            foreach (string str in values)
            {
                sbyte v;
                try { v = Convert.ToSByte(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListSByteValue(List<sbyte> newValue)
        {
            values.Clear();
            foreach (sbyte i in newValue)
                values.Add(i.ToString());
        }
        public char GetCharValue()
        {
            try { return Convert.ToChar(value); } catch { }
            return default;
        }
        public void SetCharValue(char newValue)
        {
            value = newValue.ToString();
        }
        public List<char> GetListCharValue()
        {
            List<char> list = new List<char>();
            foreach (string str in values)
            {
                char v;
                try { v = Convert.ToChar(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListCharValue(List<char> newValue)
        {
            values.Clear();
            foreach (char i in newValue)
                values.Add(i.ToString());
        }
        public float GetSingleValue()
        {
            try { return Convert.ToSingle(value, CultureInfo.InvariantCulture); } catch { }
            return default;
        }
        public void SetSingleValue(float newValue)
        {
            value = newValue.ToString(CultureInfo.InvariantCulture);
        }
        public List<float> GetListSingleValue()
        {
            List<float> list = new List<float>();
            foreach (string str in values)
            {
                float v;
                try { v = Convert.ToSingle(str, CultureInfo.InvariantCulture); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListSingleValue(List<float> newValue)
        {
            values.Clear();
            foreach (float i in newValue)
                values.Add(i.ToString(CultureInfo.InvariantCulture));
        }
        public double GetDoubleValue()
        {
            try { return Convert.ToDouble(value, CultureInfo.InvariantCulture); } catch { }
            return default;
        }
        public void SetDoubleValue(double newValue)
        {
            value = newValue.ToString(CultureInfo.InvariantCulture);
        }
        public List<double> GetListDoubleValue()
        {
            List<double> list = new List<double>();
            foreach (string str in values)
            {
                double v;
                try { v = Convert.ToDouble(str, CultureInfo.InvariantCulture); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListDoubleValue(List<double> newValue)
        {
            values.Clear();
            foreach (double i in newValue)
                values.Add(i.ToString(CultureInfo.InvariantCulture));
        }
        public bool GetBooleanValue()
        {
            try { return Convert.ToBoolean(value); } catch { }
            return default;
        }
        public void SetBooleanValue(bool newValue)
        {
            value = newValue.ToString();
        }
        public List<bool> GetListBooleanValue()
        {
            List<bool> list = new List<bool>();
            foreach (string str in values)
            {
                bool v;
                try { v = Convert.ToBoolean(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListBooleanValue(List<bool> newValue)
        {
            values.Clear();
            foreach (bool i in newValue)
                values.Add(i.ToString());
        }
        public string GetStringValue()
        {
            return value;
        }
        public void SetStringValue(string newValue)
        {
            value = newValue == null ? "" : newValue;
        }
        public string GetCSharpLikeJSONDataValue()
        {
            try { return KissJson.ToJSONData(value); } catch { }
            return null;
        }
        public void SetCSharpLikeJSONDataValue(string newValue)
        {
            try
            {
                JSONData data = KissJson.ToJSONData(newValue);
                value = data.ToJson();
            }
            catch { }
        }
        public List<JSONData> GetListJSONDataValue()
        {
            List<JSONData> list = new List<JSONData>();
            foreach (string str in values)
            {
                JSONData v;
                try { v = KissJson.ToJSONData(str); } catch { v = JSONData.NewDictionary(); }
                list.Add(v);
            }
            return list;
        }
        public void SetListJSONDataValue(List<JSONData> newValue)
        {
            values.Clear();
            foreach (JSONData i in newValue)
                values.Add(i.ToJson());
        }
        public Vector2 GetVector2Value()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 2)
                {
                    return new Vector2(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[1], CultureInfo.InvariantCulture));
                }
            }
            catch { }
            return Vector2.zero;
        }
        public void SetVector2Value(Vector2 newValue)
        {
            value = $"{newValue.x.ToString(CultureInfo.InvariantCulture)},{newValue.y.ToString(CultureInfo.InvariantCulture)}";
        }
        public List<Vector2> GetListVector2Value()
        {
            List<Vector2> list = new List<Vector2>();
            foreach (string str in values)
            {
                Vector2 v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 2)
                    {
                        v = new Vector2(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[1], CultureInfo.InvariantCulture));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListVector2Value(List<Vector2> newValue)
        {
            values.Clear();
            foreach (Vector2 i in newValue)
                values.Add($"{i.x.ToString(CultureInfo.InvariantCulture)},{i.y.ToString(CultureInfo.InvariantCulture)}");
        }
        public Vector3 GetVector3Value()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 3)
                {
                    return new Vector3(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[2], CultureInfo.InvariantCulture));
                }
            }
            catch { }
            return Vector3.zero;
        }
        public void SetVector3Value(Vector3 newValue)
        {
            value = $"{newValue.x.ToString(CultureInfo.InvariantCulture)},{newValue.y.ToString(CultureInfo.InvariantCulture)},{newValue.z.ToString(CultureInfo.InvariantCulture)}";
        }
        public List<Vector3> GetListVector3Value()
        {
            List<Vector3> list = new List<Vector3>();
            foreach (string str in values)
            {
                Vector3 v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 3)
                    {
                        v = new Vector3(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[2], CultureInfo.InvariantCulture));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListVector3Value(List<Vector3> newValue)
        {
            values.Clear();
            foreach (Vector3 i in newValue)
                values.Add($"{i.x.ToString(CultureInfo.InvariantCulture)},{i.y.ToString(CultureInfo.InvariantCulture)},{i.z.ToString(CultureInfo.InvariantCulture)}");
        }
        public Vector4 GetVector4Value()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 4)
                {
                    return new Vector4(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[2], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[3], CultureInfo.InvariantCulture));
                }
            }
            catch { }
            return Vector4.zero;
        }
        public void SetVector4Value(Vector4 newValue)
        {
            value = $"{newValue.x.ToString(CultureInfo.InvariantCulture)},{newValue.y.ToString(CultureInfo.InvariantCulture)},{newValue.z.ToString(CultureInfo.InvariantCulture)},{newValue.w.ToString(CultureInfo.InvariantCulture)}";
        }
        public List<Vector4> GetListVector4Value()
        {
            List<Vector4> list = new List<Vector4>();
            foreach (string str in values)
            {
                Vector4 v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 4)
                    {
                        v = new Vector4(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[2], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[3], CultureInfo.InvariantCulture));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListVector4Value(List<Vector4> newValue)
        {
            values.Clear();
            foreach (Vector4 i in newValue)
                values.Add($"{i.x.ToString(CultureInfo.InvariantCulture)},{i.y.ToString(CultureInfo.InvariantCulture)},{i.z.ToString(CultureInfo.InvariantCulture)},{i.w.ToString(CultureInfo.InvariantCulture)}");
        }
        public Vector2Int GetVector2IntValue()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 2)
                {
                    return new Vector2Int(Convert.ToInt32(strs[0]),
                        Convert.ToInt32(strs[1]));
                }
            }
            catch { }
            return Vector2Int.zero;
        }
        public void SetVector2IntValue(Vector2Int newValue)
        {
            value = $"{newValue.x},{newValue.y}";
        }
        public List<Vector2Int> GetListVector2IntValue()
        {
            List<Vector2Int> list = new List<Vector2Int>();
            foreach (string str in values)
            {
                Vector2Int v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 2)
                    {
                        v = new Vector2Int(Convert.ToInt32(strs[0]),
                            Convert.ToInt32(strs[1]));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListVector2IntValue(List<Vector2Int> newValue)
        {
            values.Clear();
            foreach (Vector2Int i in newValue)
                values.Add($"{i.x},{i.y}");
        }
        public Vector3Int GetVector3IntValue()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 3)
                {
                    return new Vector3Int(Convert.ToInt32(strs[0]),
                        Convert.ToInt32(strs[1]),
                        Convert.ToInt32(strs[2]));
                }
            }
            catch { }
            return Vector3Int.zero;
        }
        public void SetVector3IntValue(Vector3Int newValue)
        {
            value = $"{newValue.x},{newValue.y},{newValue.z}";
        }
        public List<Vector3Int> GetListVector3IntValue()
        {
            List<Vector3Int> list = new List<Vector3Int>();
            foreach (string str in values)
            {
                Vector3Int v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 3)
                    {
                        v = new Vector3Int(Convert.ToInt32(strs[0]),
                            Convert.ToInt32(strs[1]),
                            Convert.ToInt32(strs[2]));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListVector3IntValue(List<Vector3Int> newValue)
        {
            values.Clear();
            foreach (Vector3Int i in newValue)
                values.Add($"{i.x},{i.y},{i.z}");
        }
        public Color GetColorValue()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 4)
                {
                    return new Color(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[2], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[3], CultureInfo.InvariantCulture));
                }
            }
            catch { }
            return Color.white;
        }
        public void SetColorValue(Color newValue)
        {
            value = $"{newValue.r.ToString(CultureInfo.InvariantCulture)},{newValue.g.ToString(CultureInfo.InvariantCulture)},{newValue.b.ToString(CultureInfo.InvariantCulture)},{newValue.a.ToString(CultureInfo.InvariantCulture)}";
        }
        public List<Color> GetListColorValue()
        {
            List<Color> list = new List<Color>();
            foreach (string str in values)
            {
                Color v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 4)
                    {
                        v = new Color(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[2], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[3], CultureInfo.InvariantCulture));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListColorValue(List<Color> newValue)
        {
            values.Clear();
            foreach (Color i in newValue)
                values.Add($"{i.r.ToString(CultureInfo.InvariantCulture)},{i.g.ToString(CultureInfo.InvariantCulture)},{i.b.ToString(CultureInfo.InvariantCulture)},{i.a.ToString(CultureInfo.InvariantCulture)}");
        }
        public Color32 GetColor32Value()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 4)
                {
                    return new Color32(Convert.ToByte(strs[0]),
                        Convert.ToByte(strs[1]),
                        Convert.ToByte(strs[2]),
                        Convert.ToByte(strs[3]));
                }
            }
            catch { }
            return new Color32(255, 255, 255, 255);
        }
        public void SetColor32Value(Color32 newValue)
        {
            value = $"{newValue.r},{newValue.g},{newValue.b},{newValue.a}";
        }
        public List<Color32> GetListColor32Value()
        {
            List<Color32> list = new List<Color32>();
            foreach (string str in values)
            {
                Color32 v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 4)
                    {
                        v = new Color32(Convert.ToByte(strs[0]),
                            Convert.ToByte(strs[1]),
                            Convert.ToByte(strs[2]),
                            Convert.ToByte(strs[3]));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListColor32Value(List<Color32> newValue)
        {
            values.Clear();
            foreach (Color32 i in newValue)
                values.Add($"{i.r},{i.g},{i.b},{i.a}");
        }
        public Matrix4x4 GetMatrix4x4Value()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 16)
                {
                    Matrix4x4 matrix4X4 = new Matrix4x4();
                    for (int i = 0; i < 16; i++)
                        matrix4X4[i] = Convert.ToSingle(strs[i], CultureInfo.InvariantCulture);
                    return matrix4X4;
                }
            }
            catch { }
            return Matrix4x4.zero;
        }
        public void SetMatrix4x4Value(Matrix4x4 newValue)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for(int i=0; i<16; i++)
                sb.Append(newValue[i].ToString(CultureInfo.InvariantCulture) + ",");
            sb.Remove(sb.Length-1, 1);
            value = sb.ToString();
        }
        public List<Matrix4x4> GetListMatrix4x4Value()
        {
            List<Matrix4x4> list = new List<Matrix4x4>();
            foreach (string str in values)
            {
                Matrix4x4 v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 16)
                    {
                        v = new Matrix4x4();
                        for (int i = 0; i < 16; i++)
                            v[i] = Convert.ToSingle(strs[i], CultureInfo.InvariantCulture);
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListMatrix4x4Value(List<Matrix4x4> newValue)
        {
            values.Clear();
            foreach (Matrix4x4 i in newValue)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int j = 0; j < 16; j++)
                    sb.Append(i[j].ToString(CultureInfo.InvariantCulture) + ",");
                sb.Remove(sb.Length - 1, 1);
                values.Add(sb.ToString());
            }
        }
        public Rect GetRectValue()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 4)
                {
                    return new Rect(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[2], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[3], CultureInfo.InvariantCulture));
                }
            }
            catch { }
            return Rect.zero;
        }
        public void SetRectValue(Rect newValue)
        {
            value = $"{newValue.x.ToString(CultureInfo.InvariantCulture)},{newValue.y.ToString(CultureInfo.InvariantCulture)},{newValue.width.ToString(CultureInfo.InvariantCulture)},{newValue.height.ToString(CultureInfo.InvariantCulture)}";
        }
        public List<Rect> GetListRectValue()
        {
            List<Rect> list = new List<Rect>();
            foreach (string str in values)
            {
                Rect v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 4)
                    {
                        v = new Rect(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[2], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[3], CultureInfo.InvariantCulture));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListRectValue(List<Rect> newValue)
        {
            values.Clear();
            foreach (Rect i in newValue)
                values.Add($"{i.x.ToString(CultureInfo.InvariantCulture)},{i.y.ToString(CultureInfo.InvariantCulture)},{i.width.ToString(CultureInfo.InvariantCulture)},{i.height.ToString(CultureInfo.InvariantCulture)}");
        }
        public RectInt GetRectIntValue()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 4)
                {
                    return new RectInt(Convert.ToByte(strs[0]),
                        Convert.ToByte(strs[1]),
                        Convert.ToByte(strs[2]),
                        Convert.ToByte(strs[3]));
                }
            }
            catch { }
            return default;
        }
        public void SetRectIntValue(RectInt newValue)
        {
            value = $"{newValue.x},{newValue.y},{newValue.width},{newValue.height}";
        }
        public List<RectInt> GetListRectIntValue()
        {
            List<RectInt> list = new List<RectInt>();
            foreach (string str in values)
            {
                RectInt v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 4)
                    {
                        v = new RectInt(Convert.ToInt32(strs[0]),
                            Convert.ToInt32(strs[1]),
                            Convert.ToInt32(strs[2]),
                            Convert.ToInt32(strs[3]));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListRectIntValue(List<RectInt> newValue)
        {
            values.Clear();
            foreach (RectInt i in newValue)
                values.Add($"{i.x},{i.y},{i.width},{i.height}");
        }
        public Bounds GetBoundsValue()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 6)
                {
                    return new Bounds(new Vector3(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[2], CultureInfo.InvariantCulture)),
                        new Vector3(Convert.ToSingle(strs[3], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[4], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[5], CultureInfo.InvariantCulture)));
                }
            }
            catch { }
            return default;
        }
        public void SetBoundsValue(Bounds newValue)
        {
            value = $"{newValue.center.x.ToString(CultureInfo.InvariantCulture)},{newValue.center.y.ToString(CultureInfo.InvariantCulture)},{newValue.center.z.ToString(CultureInfo.InvariantCulture)},{newValue.size.x.ToString(CultureInfo.InvariantCulture)},{newValue.size.y.ToString(CultureInfo.InvariantCulture)},{newValue.size.z.ToString(CultureInfo.InvariantCulture)}";
        }
        public List<Bounds> GetListBoundsValue()
        {
            List<Bounds> list = new List<Bounds>();
            foreach (string str in values)
            {
                Bounds v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 6)
                    {
                        v = new Bounds(new Vector3(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[2], CultureInfo.InvariantCulture)),
                            new Vector3(Convert.ToSingle(strs[3], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[4], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[5], CultureInfo.InvariantCulture)));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListBoundsValue(List<Bounds> newValue)
        {
            values.Clear();
            foreach (Bounds i in newValue)
                values.Add($"{i.center.x.ToString(CultureInfo.InvariantCulture)},{i.center.y.ToString(CultureInfo.InvariantCulture)},{i.center.z.ToString(CultureInfo.InvariantCulture)},{i.size.x.ToString(CultureInfo.InvariantCulture)},{i.size.y.ToString(CultureInfo.InvariantCulture)},{i.size.z.ToString(CultureInfo.InvariantCulture)}");
        }
        public BoundsInt GetBoundsIntValue()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 6)
                {
                    return new BoundsInt(new Vector3Int(Convert.ToInt32(strs[0]),
                        Convert.ToInt32(strs[1]),
                        Convert.ToInt32(strs[2])),
                        new Vector3Int(Convert.ToInt32(strs[3]),
                        Convert.ToInt32(strs[4]),
                        Convert.ToInt32(strs[5])));
                }
            }
            catch { }
            return default;
        }
        public void SetBoundsIntValue(BoundsInt newValue)
        {
            value = $"{newValue.center.x},{newValue.center.y},{newValue.center.z},{newValue.size.x},{newValue.size.y},{newValue.size.z}";
        }
        public List<BoundsInt> GetListBoundsIntValue()
        {
            List<BoundsInt> list = new List<BoundsInt>();
            foreach (string str in values)
            {
                BoundsInt v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 6)
                    {
                        v = new BoundsInt(new Vector3Int(Convert.ToInt32(strs[0]),
                            Convert.ToInt32(strs[1]),
                            Convert.ToInt32(strs[2])),
                            new Vector3Int(Convert.ToInt32(strs[3]),
                            Convert.ToInt32(strs[4]),
                            Convert.ToInt32(strs[5])));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListBoundsIntValue(List<BoundsInt> newValue)
        {
            values.Clear();
            foreach (BoundsInt i in newValue)
                values.Add($"{i.center.x},{i.center.y},{i.center.z},{i.size.x},{i.size.y},{i.size.z}");
        }
        public Quaternion GetQuaternionValue()
        {
            try
            {
                string[] strs = value.Split(',');
                if (strs.Length == 4)
                {
                    return new Quaternion(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[2], CultureInfo.InvariantCulture),
                        Convert.ToSingle(strs[3], CultureInfo.InvariantCulture));
                }
            }
            catch { }
            return Quaternion.identity;
        }
        public void SetQuaternionValue(Quaternion newValue)
        {
            value = $"{newValue.x.ToString(CultureInfo.InvariantCulture)},{newValue.y.ToString(CultureInfo.InvariantCulture)},{newValue.z.ToString(CultureInfo.InvariantCulture)},{newValue.w.ToString(CultureInfo.InvariantCulture)}";
        }
        public List<Quaternion> GetListQuaternionValue()
        {
            List<Quaternion> list = new List<Quaternion>();
            foreach (string str in values)
            {
                Quaternion v;
                try
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 4)
                    {
                        v = new Quaternion(Convert.ToSingle(strs[0], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[1], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[2], CultureInfo.InvariantCulture),
                            Convert.ToSingle(strs[3], CultureInfo.InvariantCulture));
                    }
                    else
                        v = default;
                }
                catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListQuaternionValue(List<Quaternion> newValue)
        {
            values.Clear();
            foreach (Quaternion i in newValue)
                values.Add($"{i.x.ToString(CultureInfo.InvariantCulture)},{i.y.ToString(CultureInfo.InvariantCulture)},{i.z.ToString(CultureInfo.InvariantCulture)},{i.w.ToString(CultureInfo.InvariantCulture)}");
        }
        public LayerMask GetLayerMaskValue()
        {
            try
            {
                return Convert.ToInt32(value);
            }
            catch { }
            return default;
        }
        public void SetLayerMaskValue(LayerMask newValue)
        {
            value = $"{newValue.value}";
        }
        public List<LayerMask> GetListLayerMaskValue()
        {
            List<LayerMask> list = new List<LayerMask>();
            foreach (string str in values)
            {
                LayerMask v;
                try { v = Convert.ToInt32(str); } catch { v = default; }
                list.Add(v);
            }
            return list;
        }
        public void SetListLayerMaskValue(List<LayerMask> newValue)
        {
            values.Clear();
            foreach (LayerMask i in newValue)
                values.Add(i.value.ToString());
        }
    }
}
