/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;
using CSharpLike;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace UnityEditor.UI
{
    /// <summary>
    /// Editor class used to edit UI HotUpdateBehaviour.
    /// </summary>

    [CustomEditor(typeof(HotUpdateBehaviour), true)]
    [CanEditMultipleObjects]
    /// <summary>
    ///   Custom Editor for the HotUpdateBehaviour Component.
    /// </summary>
    public class HotUpdateBehaviourEditor : GraphicEditor
    {
        SerializedProperty m_bindHotUpdateClassFullName;
        SerializedProperty m_KissFieldEnd;

        GUIContent m_bindHotUpdateClassNameContent;
        GUIContent m_btListAdd;
        GUIContent m_btListSub;
        GUIContent m_btListElementAdd;
        GUIContent m_btListElementSub;

        class DefaultValue
        {
            public DefaultValue(Type type)
            {
                try
                {
                    instance = Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    Debug.LogError($"'{type.FullName}' CreateInstance error '{e.Message}', you should has an constructor without param");
                }
            }
            object instance;
            public Dictionary<FieldInfo, object> values = new Dictionary<FieldInfo, object>();
            public object GetDefaultValue(FieldInfo fieldInfo)
            {
                if (values.TryGetValue(fieldInfo, out object obj))
                    return obj;
                if (instance != null)
                    obj = fieldInfo.GetValue(instance);
                else
                    obj = fieldInfo.FieldType.IsValueType ? Activator.CreateInstance(fieldInfo.FieldType) : null;
                if (fieldInfo.FieldType == typeof(Color))
                {
                    Color color = (Color)obj;
                    if (color.r == 0 && color.g == 0 && color.b == 0 && color.a == 0)
                        obj = Color.white;
                }
                else if (fieldInfo.FieldType == typeof(Color32))
                {
                    Color32 color = (Color32)obj;
                    if (color.r == 0 && color.g == 0 && color.b == 0 && color.a == 0)
                        obj = (Color32)Color.white;
                }
                values[fieldInfo] = obj;
                return obj;
            }
        }
        static Dictionary<Type, DefaultValue> mCaches = new Dictionary<Type, DefaultValue>();
        static object GetDefaultValue(Type type, FieldInfo fieldInfo)
        {
            if (!mCaches.TryGetValue(type, out DefaultValue defaultValue))
            {
                defaultValue = new DefaultValue(type);
                mCaches[type] = defaultValue;
            }
            return defaultValue.GetDefaultValue(fieldInfo);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_bindHotUpdateClassFullName = serializedObject.FindProperty("bindHotUpdateClassFullName");
            m_KissFieldEnd = serializedObject.FindProperty("m_KissFieldEnd");
            errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            centerStyle = new GUIStyle();
            centerStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            centerStyle.alignment = TextAnchor.MiddleCenter;
            rightStyle = new GUIStyle();
            rightStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            rightStyle.alignment = TextAnchor.MiddleRight;
            m_bindHotUpdateClassNameContent = EditorGUIUtility.TrTextContent("Hot Update Class", "This component will bind to hot update class, your class must inherited from LikeBehaviour.");
            m_btListAdd = EditorGUIUtility.TrTextContent("+", "Add 1 array element to the tail");
            m_btListSub = EditorGUIUtility.TrTextContent("-", "Remove 1 array element at the tail");
            m_btListElementAdd = EditorGUIUtility.TrTextContent("+", "Insert 1 element AFTER this element");
            m_btListElementSub = EditorGUIUtility.TrTextContent("-", "Remove this element");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            isInited = false;
        }
        static Dictionary<Type, bool> mSupportTypeCaches = new Dictionary<Type, bool>();
        static bool IsSupportType(Type type)
        {
            if (!mSupportTypeCaches.TryGetValue(type, out bool isSupport))
            {
                isSupport = KissSerializableObject.IsSupportType(type);
                mSupportTypeCaches[type] = isSupport;
            }
            return isSupport;
        }
        Dictionary<string, GUIContent> mContentCaches = new Dictionary<string, GUIContent>();
        static string ConvertEasyName(string strName)
        {
            if (strName.Length > 2 && strName.StartsWith("m_"))
                strName = strName.Substring(2);
            char[] strs = strName.ToCharArray();
            int length = strs.Length;
            if (length == 0)
                return "";
            else if (length == 1)
                return strName.ToUpper();
            StringBuilder sb = new StringBuilder();
            bool preIsUpper = true;
            for (int i = 0; i < length; i++)
            {
                char c = strs[i];
                if (i == 0)
                    sb.Append(char.ToUpper(c));
                else if (c == '_')
                    sb.Append(" ");
                else
                {
                    bool b = char.IsUpper(c);
                    if (preIsUpper)
                    {
                        sb.Append(c);
                        preIsUpper = b;
                    }
                    else
                    {
                        preIsUpper = b;
                        if (preIsUpper)
                            sb.Append(" ");
                        sb.Append(c);
                    }
                }
            }
            return sb.ToString().Trim();
        }
        GUIContent GetTrTextContent(FieldInfo fi)
        {
            if (!mContentCaches.TryGetValue(fi.Name, out GUIContent content))
            {
                TooltipAttribute ta = fi.GetCustomAttribute<TooltipAttribute>();
                content = EditorGUIUtility.TrTextContent(ConvertEasyName(fi.Name), ta?.tooltip);
                mContentCaches[fi.Name] = content;
            }
            return content;
        }
        GUIStyle errorStyle;
        GUIStyle centerStyle;
        GUIStyle rightStyle;
        class MySerializedProperty
        {
            public string name;
            public SerializedProperty obj;
            public SerializedProperty value;
            public SerializedProperty objs;
            public List<SerializedProperty> _objs;
            public SerializedProperty values;
            public List<SerializedProperty> _values;
        }
        Dictionary<string, MySerializedProperty> mSerializedPropertyCaches = new Dictionary<string, MySerializedProperty>();
        MySerializedProperty GetSerializedProperty(string name)
        {
            if (mSerializedPropertyCaches.TryGetValue(name, out MySerializedProperty mySerializedProperty))
                return mySerializedProperty;
            return null;
        }
        static GUIContent[] allHotUpdateClassNames;
        static int[] allHotUpdateClassNameIndexs;
        static Dictionary<string, int> classNames = new Dictionary<string, int>();
        [Callbacks.DidReloadScripts]
        static void ResetInit()
        {
            Assembly assembly = typeof(HotUpdateBehaviour).Assembly;
            Type typeLikeBehaviour = typeof(LikeBehaviour);
            SortedDictionary<string, bool> sorts = new SortedDictionary<string, bool>();
            List<GUIContent> contents = new List<GUIContent>();
            List<int> indexs = new List<int>();
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeLikeBehaviour) && type != typeLikeBehaviour)
                    sorts[type.FullName] = true;
            }
            int i = 0;
            foreach (string fullName in sorts.Keys)
            {
                contents.Add(EditorGUIUtility.TrTextContent(fullName));
                indexs.Add(i);
                classNames[fullName] = i++;
            }
            allHotUpdateClassNames = contents.ToArray();
            allHotUpdateClassNameIndexs = indexs.ToArray();
        }
        public HotUpdateBehaviourEditor()
        {
            isInited = false;
        }
        bool isInited = false;
        List<FieldInfo> mFieldInfoCaches = new List<FieldInfo>();
        void Init(HotUpdateBehaviour hub, Type type)
        {
            if (isInited) 
                return;
            isInited = true;
            mFieldInfoCaches.Clear(); 
            mSerializedPropertyCaches.Clear();
            hub.KissSerializeFields = null;
            Dictionary<string, bool> useds = new Dictionary<string, bool>();
            foreach (FieldInfo fi in type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (!fi.IsStatic && IsSupportType(fi.FieldType))
                {
                    NonSerializedAttribute nsa = fi.GetCustomAttribute<NonSerializedAttribute>();
                    SerializeField sf = fi.GetCustomAttribute<SerializeField>();
                    if ((fi.IsPublic && nsa == null)
                        || (fi.IsPrivate && sf != null))
                    {
                        mFieldInfoCaches.Add(fi);
                        useds[fi.Name] = true;
                        KissSerializableObject obj = hub.GetKissSerializableObject(fi.Name);
                        if (obj == null)
                        {
                            obj = new KissSerializableObject(fi) { Value = GetDefaultValue(type, fi) };
                            hub.NewKissSerializableObject(obj);
                        }
                        obj.IsHideInInspector = fi.GetCustomAttribute<HideInInspector>() != null;
                        obj.Range = fi.GetCustomAttribute<RangeAttribute>();
                    }
                }
            }
            hub.RemoveUnusedKissSerializableObject(useds);
            serializedObject.Update();
            SerializedProperty serializedProperty;
            serializedProperty = serializedObject.FindProperty("m_KissSerializeFields");
            if (serializedProperty != null && serializedProperty.Next(true) && serializedProperty != m_KissFieldEnd)
            {
                serializedProperty.Next(true);
                serializedProperty.Next(false);
                MySerializedProperty temp = null;
                while (serializedProperty.Next(serializedProperty.name == "data") && serializedProperty != m_KissFieldEnd)
                {
                    switch (serializedProperty.name)
                    {
                        case "name":
                            temp = new MySerializedProperty() { name = serializedProperty.stringValue };
                            break;
                        case "obj":
                            temp.obj = serializedProperty.Copy();
                            break;
                        case "objs":
                            temp.objs = serializedProperty.Copy();
                            temp._objs = new List<SerializedProperty>();
                            for(int i=0;i<serializedProperty.arraySize; i++)
                                temp._objs.Add(serializedProperty.GetArrayElementAtIndex(i).Copy());
                            break;
                        case "value":
                            temp.value = serializedProperty.Copy();
                            break;
                        case "values":
                            temp.values = serializedProperty.Copy();
                            temp._values = new List<SerializedProperty>();
                            for (int i = 0; i < serializedProperty.arraySize; i++)
                                temp._values.Add(serializedProperty.GetArrayElementAtIndex(i).Copy());
                            mSerializedPropertyCaches.Add(temp.name, temp);
                            break;
                    }
                }
            }
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            Assembly assembly = typeof(HotUpdateBehaviour).Assembly;
            HotUpdateBehaviour hub = serializedObject.targetObject as HotUpdateBehaviour; 
            bool isValidClassName = false;
            if (hub != null)
            {
                int indexOld = classNames.ContainsKey(hub.bindHotUpdateClassFullName) ? classNames[hub.bindHotUpdateClassFullName] : -1;
                int indexNew = EditorGUILayout.IntPopup(m_bindHotUpdateClassNameContent, indexOld, allHotUpdateClassNames, allHotUpdateClassNameIndexs);
                if (indexOld != indexNew)
                {
                    hub.bindHotUpdateClassFullName = allHotUpdateClassNames[indexNew].text;
                    m_bindHotUpdateClassFullName.stringValue = hub.bindHotUpdateClassFullName;
                    isInited = false;
                }
            }
            if (hub != null && hub.bindHotUpdateClassFullName != null && hub.bindHotUpdateClassFullName.Length > 0)
            {
                Type type = assembly.GetType(hub.bindHotUpdateClassFullName);
                if (type != null && type.IsSubclassOf(typeof(LikeBehaviour)))
                {
                    isValidClassName = true;
                    Init(hub, type);
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField($"-- {hub.SerializeFieldsCount} Serialize fields of '{hub.bindHotUpdateClassFullName}' --", centerStyle);
                    foreach (FieldInfo fi in mFieldInfoCaches)
                    {
                        KissSerializableObject obj = hub.GetKissSerializableObject(fi.Name);
                        MySerializedProperty mySerializedProperty = GetSerializedProperty(fi.Name);
                        if (mySerializedProperty == null || obj == null || obj.IsHideInInspector)
                            continue;
                        switch(obj.objectType)
                        {
                            case KissSerializableObject.ObjectType.UnityEngineObject:
                                {
                                    UnityEngine.Object _objOld = obj.obj;
                                    UnityEngine.Object _objNew = EditorGUILayout.ObjectField(GetTrTextContent(fi), _objOld, fi.FieldType, true);
                                    if (_objOld != _objNew)
                                    {
                                        obj.obj = _objNew;
                                        mySerializedProperty.obj.objectReferenceValue = _objNew;
                                        hub.UpdateFieldInEditor(obj);
                                    }
                                }
                                break;
                            case KissSerializableObject.ObjectType.LikeBehaviourObject:
                                {
                                    UnityEngine.Object _objOld = obj.obj;
                                    UnityEngine.Object _objNew = EditorGUILayout.ObjectField(GetTrTextContent(fi), _objOld, typeof(HotUpdateBehaviour), true);
                                    if (_objOld != _objNew)
                                    {
                                        obj.obj = _objNew;
                                        mySerializedProperty.obj.objectReferenceValue = _objNew;
                                        hub.UpdateFieldInEditor(obj);
                                    }
                                }
                                break;
                            case KissSerializableObject.ObjectType.Value:
                                switch (fi.FieldType.FullName)
                                {
                                    case "System.String":
                                        {
                                            string valueOld = obj.GetStringValue();
                                            string valueNew = EditorGUILayout.TextField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetStringValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.Char":
                                        {
                                            char valueOld = obj.GetCharValue();
                                            char[] chars = EditorGUILayout.TextField(GetTrTextContent(fi), valueOld.ToString()).ToCharArray();
                                            if (chars.Length > 0)
                                            {
                                                char valueNew = chars[0];
                                                if (valueOld != valueNew)
                                                {
                                                    obj.SetCharValue(valueNew);
                                                    mySerializedProperty.value.stringValue = obj.value;
                                                    hub.UpdateFieldInEditor(obj);
                                                }
                                            }
                                        }
                                        break;
                                    case "System.Int64":
                                        {
                                            long valueOld = obj.GetInt64Value();
                                            long valueNew = (obj.Range != null) ? Convert.ToInt64(Slider(obj.Range, valueOld, GetTrTextContent(fi))) : EditorGUILayout.LongField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetInt64Value(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.UInt64":
                                        {
                                            long valueOld = (long)obj.GetUInt64Value();
                                            long valueNew = Convert.ToInt64((obj.Range != null) ? Slider(obj.Range, valueOld, GetTrTextContent(fi)) : EditorGUILayout.LongField(GetTrTextContent(fi), valueOld));
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetUInt64Value(Convert.ToUInt64(valueNew));
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.Int32":
                                        {
                                            int valueOld = obj.GetInt32Value();
                                            int valueNew = (obj.Range != null) ? Slider(obj.Range, valueOld, GetTrTextContent(fi)) : EditorGUILayout.IntField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetInt32Value(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.UInt32":
                                        {
                                            long valueOld = obj.GetUInt32Value();
                                            long valueNew = Convert.ToUInt32((obj.Range != null) ? Slider(obj.Range, valueOld, GetTrTextContent(fi)) : EditorGUILayout.LongField(GetTrTextContent(fi), valueOld));
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetUInt32Value(Convert.ToUInt32(valueNew));
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.Int16":
                                        {
                                            int valueOld = obj.GetInt16Value();
                                            int valueNew = Convert.ToInt16((obj.Range != null) ? Slider(obj.Range, valueOld, GetTrTextContent(fi)) : EditorGUILayout.IntField(GetTrTextContent(fi), valueOld));
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetInt16Value(Convert.ToInt16(valueNew));
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.UInt16":
                                        {
                                            int valueOld = obj.GetUInt16Value();
                                            int valueNew = Convert.ToUInt16((obj.Range != null) ? Slider(obj.Range, valueOld, GetTrTextContent(fi)) : EditorGUILayout.IntField(GetTrTextContent(fi), valueOld));
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetUInt16Value(Convert.ToUInt16(valueNew));
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.Byte":
                                        {
                                            int valueOld = obj.GetByteValue();
                                            int valueNew = Convert.ToByte((obj.Range != null) ? Slider(obj.Range, valueOld, GetTrTextContent(fi)) : EditorGUILayout.IntField(GetTrTextContent(fi), valueOld));
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetByteValue(Convert.ToByte(valueNew));
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.SByte":
                                        {
                                            int valueOld = obj.GetSByteValue();
                                            int valueNew = Convert.ToSByte((obj.Range != null) ? Slider(obj.Range, valueOld, GetTrTextContent(fi)) : EditorGUILayout.IntField(GetTrTextContent(fi), valueOld));
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetSByteValue(Convert.ToSByte(valueNew));
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.Single":
                                        {
                                            float valueOld = obj.GetSingleValue();
                                            float valueNew = obj.Range != null ? Slider(obj.Range, valueOld, GetTrTextContent(fi)) : EditorGUILayout.FloatField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetSingleValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.Double":
                                        {
                                            double valueOld = obj.GetDoubleValue();
                                            double valueNew = (obj.Range != null) ? Convert.ToByte(Slider(obj.Range, valueOld, GetTrTextContent(fi))) : EditorGUILayout.DoubleField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetDoubleValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "System.Boolean":
                                        {
                                            bool valueOld = obj.GetBooleanValue();
                                            bool valueNew = EditorGUILayout.Toggle(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetBooleanValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Vector2":
                                        {
                                            Vector2 valueOld = obj.GetVector2Value();
                                            Vector2 valueNew = EditorGUILayout.Vector2Field(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetVector2Value(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Vector3":
                                        {
                                            Vector3 valueOld = obj.GetVector3Value();
                                            Vector3 valueNew = EditorGUILayout.Vector3Field(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetVector3Value(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Vector4":
                                        {
                                            Vector4 valueOld = obj.GetVector4Value();
                                            Vector4 valueNew = EditorGUILayout.Vector4Field(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetVector4Value(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Color":
                                        {
                                            Color valueOld = obj.GetColorValue();
                                            Color valueNew = EditorGUILayout.ColorField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetColorValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Color32":
                                        {
                                            Color valueOld = obj.GetColor32Value();
                                            Color valueNew = EditorGUILayout.ColorField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetColor32Value(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "CSharpLike.JSONData":
                                        {
                                            string valueOld = obj.GetCSharpLikeJSONDataValue();
                                            if (valueOld == null)
                                                valueOld = "";
                                            string valueNew = EditorGUILayout.TextField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetCSharpLikeJSONDataValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Vector2Int":
                                        {
                                            Vector2Int valueOld = obj.GetVector2IntValue();
                                            Vector2Int valueNew = EditorGUILayout.Vector2IntField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetVector2IntValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Vector3Int":
                                        {
                                            Vector3Int valueOld = obj.GetVector3IntValue();
                                            Vector3Int valueNew = EditorGUILayout.Vector3IntField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetVector3IntValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Matrix4x4":
                                        {
                                            Matrix4x4 valueOld = obj.GetMatrix4x4Value();
                                            EditorGUILayout.LabelField(GetTrTextContent(fi));
                                            Vector4 columnOld0 = valueOld.GetColumn(0);
                                            Vector4 columnNew0 = EditorGUILayout.Vector4Field("    column0", columnOld0);
                                            Vector4 columnOld1 = valueOld.GetColumn(1);
                                            Vector4 columnNew1 = EditorGUILayout.Vector4Field("    column1", columnOld1);
                                            Vector4 columnOld2 = valueOld.GetColumn(2);
                                            Vector4 columnNew2 = EditorGUILayout.Vector4Field("    column2", columnOld2);
                                            Vector4 columnOld3 = valueOld.GetColumn(3);
                                            Vector4 columnNew3 = EditorGUILayout.Vector4Field("    column3", columnOld3);
                                            Matrix4x4 valueNew = new Matrix4x4(columnNew0, columnNew1, columnNew2, columnNew3);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetMatrix4x4Value(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Rect":
                                        {
                                            Rect valueOld = obj.GetRectValue();
                                            Rect valueNew = EditorGUILayout.RectField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetRectValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.RectInt":
                                        {
                                            RectInt valueOld = obj.GetRectIntValue();
                                            RectInt valueNew = EditorGUILayout.RectIntField(GetTrTextContent(fi), valueOld);
                                            if (valueOld.center != valueNew.center
                                                || valueOld.size != valueNew.size)
                                            {
                                                obj.SetRectIntValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.Bounds":
                                        {
                                            Bounds valueOld = obj.GetBoundsValue();
                                            Bounds valueNew = EditorGUILayout.BoundsField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetBoundsValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.BoundsInt":
                                        {
                                            BoundsInt valueOld = obj.GetBoundsIntValue();
                                            BoundsInt valueNew = EditorGUILayout.BoundsIntField(GetTrTextContent(fi), valueOld);
                                            if (valueOld.position != valueNew.position
                                                || valueOld.size != valueNew.size)
                                            {
                                                obj.SetBoundsIntValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    case "UnityEngine.LayerMask":
                                        {
                                            int valueOld = obj.GetLayerMaskValue();
                                            int valueNew = EditorGUILayout.LayerField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.SetLayerMaskValue(valueNew);
                                                mySerializedProperty.value.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            try
                                            {
                                                string valueOld = (string)obj.Value;
                                                if (valueOld == null)
                                                    valueOld = "";
                                                string valueNew = EditorGUILayout.TextField(GetTrTextContent(fi), valueOld);
                                                if (valueOld != valueNew)
                                                {
                                                    obj.Value = valueNew;
                                                    mySerializedProperty.obj.stringValue = obj.value;
                                                    hub.UpdateFieldInEditor(obj);
                                                }
                                            }
                                            catch
                                            {

                                            }
                                        }
                                        break;
                                }
                                break;
                            case KissSerializableObject.ObjectType.Values:
                            case KissSerializableObject.ObjectType.Values2:
                                {
                                    Type subType = KissSerializableObject.GetSubType(fi.FieldType);
                                    switch (subType.FullName)
                                    {
                                        case "System.String":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    string _valueNew = EditorGUILayout.TextField("   |- Element " + index, (string)_valueOld);
                                                    if (_valueNew != (string)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew;
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, "");
                                            }
                                            break;
                                        case "System.Char":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    char[] chars = EditorGUILayout.TextField("   |- Element " + index, _valueOld.ToString()).ToCharArray();
                                                    if (chars.Length > 0)
                                                    {
                                                        char _valueNew = chars[0];
                                                        if (_valueNew != (char)_valueOld)
                                                        {
                                                            SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                            valueOld[index] = _valueNew;
                                                            _sp.stringValue = _valueNew.ToString();
                                                            obj.Value = valueOld;
                                                            hub.UpdateFieldInEditor(obj);
                                                        }
                                                    }
                                                }, default(char));
                                            }
                                            break;
                                        case "System.Int64":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    long _valueNew = (obj.Range != null) ? Convert.ToInt64(Slider(obj.Range, _valueOld, "   |- Element " + index)) : EditorGUILayout.LongField("   |- Element " + index, (long)_valueOld);
                                                    if (_valueNew != (long)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString();
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(long));
                                            }
                                            break;
                                        case "System.UInt64":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    ulong _valueNew = Convert.ToUInt64((obj.Range != null) ? Slider(obj.Range, _valueOld, "   |- Element " + index) : EditorGUILayout.LongField("   |- Element " + index, Convert.ToInt64(_valueOld)));
                                                    if (_valueNew != (ulong)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString();
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(ulong));
                                            }
                                            break;
                                        case "System.Int32":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    int _valueNew = (obj.Range != null) ? Slider(obj.Range, (int)_valueOld, "   |- Element " + index) : EditorGUILayout.IntField("   |- Element " + index, (int)_valueOld);
                                                    if (_valueNew != (int)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString();
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(int));
                                            }
                                            break;
                                        case "System.UInt32":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    uint _valueNew = Convert.ToUInt32((obj.Range != null) ? Slider(obj.Range, _valueOld, "   |- Element " + index) : EditorGUILayout.LongField("   |- Element " + index, (uint)_valueOld));
                                                    if (_valueNew != (uint)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString();
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(uint));
                                            }
                                            break;
                                        case "System.Int16":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    short _valueNew = Convert.ToInt16((obj.Range != null) ? Slider(obj.Range, _valueOld, "   |- Element " + index) : EditorGUILayout.IntField("   |- Element " + index, Convert.ToInt32(_valueOld)));
                                                    if (_valueNew != (short)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString();
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(short));
                                            }
                                            break;
                                        case "System.UInt16":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    ushort _valueNew = Convert.ToUInt16((obj.Range != null) ? Slider(obj.Range, _valueOld, "   |- Element " + index) : EditorGUILayout.IntField("   |- Element " + index, Convert.ToInt32(_valueOld)));
                                                    if (_valueNew != (short)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString();
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(ushort));
                                            }
                                            break;
                                        case "System.Byte":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    byte _valueNew = Convert.ToByte((obj.Range != null) ? Slider(obj.Range, _valueOld, "   |- Element " + index) : EditorGUILayout.IntField("   |- Element " + index, Convert.ToInt32(_valueOld)));
                                                    if (_valueNew != (byte)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString();
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(byte));
                                            }
                                            break;
                                        case "System.SByte":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    sbyte _valueNew = Convert.ToSByte((obj.Range != null) ? Slider(obj.Range, _valueOld, "   |- Element " + index) : EditorGUILayout.IntField("   |- Element " + index, Convert.ToInt32(_valueOld)));
                                                    if (_valueNew != (sbyte)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString();
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(sbyte));
                                            }
                                            break;
                                        case "System.Single":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    float _valueNew = (obj.Range != null) ? Slider(obj.Range, _valueOld, "   |- Element " + index) : EditorGUILayout.FloatField("   |- Element " + index, (float)_valueOld);
                                                    if (_valueNew != (float)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString(CultureInfo.InvariantCulture);
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(float));
                                            }
                                            break;
                                        case "System.Double":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    double _valueNew = (obj.Range != null) ? Convert.ToDouble(Slider(obj.Range, _valueOld, "   |- Element " + index)) : EditorGUILayout.DoubleField("   |- Element " + index, (double)_valueOld);
                                                    if (_valueNew != (float)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString(CultureInfo.InvariantCulture);
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(double));
                                            }
                                            break;
                                        case "System.Boolean":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    bool _valueNew = EditorGUILayout.Toggle("   |- Element " + index, (bool)_valueOld);
                                                    if (_valueNew != (bool)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        _sp.stringValue = _valueNew.ToString();
                                                        obj.Value = valueOld;
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(bool));
                                            }
                                            break;
                                        case "UnityEngine.Vector2":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    Vector2 _valueNew = EditorGUILayout.Vector2Field("   |- Element " + index, (Vector2)_valueOld);
                                                    if (_valueNew != (Vector2)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(Vector2));
                                            }
                                            break;
                                        case "UnityEngine.Vector3":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    Vector3 _valueNew = EditorGUILayout.Vector3Field("   |- Element " + index, (Vector3)_valueOld);
                                                    if (_valueNew != (Vector3)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(Vector3));
                                            }
                                            break;
                                        case "UnityEngine.Vector4":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    Vector4 _valueNew = EditorGUILayout.Vector4Field("   |- Element " + index, (Vector4)_valueOld);
                                                    if (_valueNew != (Vector4)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(Vector4));
                                            }
                                            break;
                                        case "UnityEngine.Color":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    Color _valueNew = EditorGUILayout.ColorField("   |- Element " + index, (Color)_valueOld);
                                                    if (_valueNew != (Color)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, Color.white);
                                            }
                                            break;
                                        case "UnityEngine.Color32":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    Color _valueNew = EditorGUILayout.ColorField("   |- Element " + index, (Color)_valueOld);
                                                    if (_valueNew != (Color)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = (Color32)_valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, Color.white);
                                            }
                                            break;
                                        case "CSharpLike.JSONData":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    string _valueOld_ = _valueOld == null ? "{}" : ((JSONData)_valueOld).ToJson();
                                                    string _valueNew = EditorGUILayout.TextField("   |- Element " + index, _valueOld_);
                                                    if (_valueNew != _valueOld_)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        JSONData _valueNew_;
                                                        try { _valueNew_ = KissJson.ToJSONData(_valueNew); } catch { _valueNew_ = JSONData.NewDictionary(); }
                                                        valueOld[index] = _valueNew_;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, JSONData.NewDictionary());
                                            }
                                            break;
                                        case "UnityEngine.Vector2Int":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    Vector2Int _valueNew = EditorGUILayout.Vector2IntField("   |- Element " + index, (Vector2Int)_valueOld);
                                                    if (_valueNew != (Vector2Int)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(Vector2Int));
                                            }
                                            break;
                                        case "UnityEngine.Vector3Int":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    Vector3Int _valueNew = EditorGUILayout.Vector3IntField("   |- Element " + index, (Vector3Int)_valueOld);
                                                    if (_valueNew != (Vector3Int)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(Vector3Int));
                                            }
                                            break;
                                        case "UnityEngine.Matrix4x4":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    EditorGUILayout.LabelField("   |- Element " + index);
                                                    Matrix4x4 _valueOld_ = (Matrix4x4)_valueOld;
                                                    Vector4 columnNew0 = EditorGUILayout.Vector4Field("   |   |-- column0", _valueOld_.GetColumn(0));
                                                    Vector4 columnNew1 = EditorGUILayout.Vector4Field("   |   |-- column1", _valueOld_.GetColumn(1));
                                                    Vector4 columnNew2 = EditorGUILayout.Vector4Field("   |   |-- column2", _valueOld_.GetColumn(2));
                                                    Vector4 columnNew3 = EditorGUILayout.Vector4Field("   |   |-- column3", _valueOld_.GetColumn(3));
                                                    Matrix4x4 _valueNew_ = new Matrix4x4(columnNew0, columnNew1, columnNew2, columnNew3);
                                                    if (_valueOld_ != _valueNew_)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew_;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(Matrix4x4));
                                            }
                                            break;
                                        case "UnityEngine.Rect":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    Rect _valueNew = EditorGUILayout.RectField("   |- Element " + index, (Rect)_valueOld);
                                                    if (_valueNew != (Rect)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(Rect));
                                            }
                                            break;
                                        case "UnityEngine.RectInt":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    RectInt _valueOld_ = (RectInt)_valueOld;
                                                    RectInt _valueNew = EditorGUILayout.RectIntField("   |- Element " + index, _valueOld_);
                                                    if (_valueNew.x != _valueOld_.x || _valueNew.y != _valueOld_.y || _valueNew.width != _valueOld_.width || _valueNew.height != _valueOld_.height)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(RectInt));
                                            }
                                            break;
                                        case "UnityEngine.Bounds":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    Bounds _valueNew = EditorGUILayout.BoundsField("   |- Element " + index, (Bounds)_valueOld);
                                                    if (_valueNew != (Bounds)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(Bounds));
                                            }
                                            break;
                                        case "UnityEngine.BoundsInt":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    BoundsInt _valueOld_ = (BoundsInt)_valueOld;
                                                    BoundsInt _valueNew = EditorGUILayout.BoundsIntField("   |- Element " + index, _valueOld_);
                                                    if (_valueNew.position != _valueOld_.position || _valueNew.size != _valueOld_.size)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(BoundsInt));
                                            }
                                            break;
                                        case "UnityEngine.LayerMask":
                                            {
                                                ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                {
                                                    LayerMask _valueNew = EditorGUILayout.LayerField("   |- Element " + index, (LayerMask)_valueOld);
                                                    if (_valueNew != (LayerMask)_valueOld)
                                                    {
                                                        SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                        valueOld[index] = _valueNew;
                                                        obj.Value = valueOld;
                                                        _sp.stringValue = obj.values[index];
                                                        hub.UpdateFieldInEditor(obj);
                                                    }
                                                }, default(LayerMask));
                                            }
                                            break;
                                        default:
                                            {
                                                try
                                                {
                                                    ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                                    {
                                                        string _valueOld_ = (string)_valueOld;
                                                        if (_valueOld_ == null)
                                                            _valueOld_ = "";
                                                        string _valueNew = EditorGUILayout.TextField("   |- Element " + index, _valueOld_);
                                                        if (_valueNew != (string)_valueOld)
                                                        {
                                                            SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                                            valueOld[index] = _valueNew;
                                                            _sp.stringValue = _valueNew;
                                                            obj.Value = valueOld;
                                                            hub.UpdateFieldInEditor(obj);
                                                        }
                                                    }, "");
                                                }
                                                catch
                                                {

                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                            case KissSerializableObject.ObjectType.UnityEngineObjects:
                            case KissSerializableObject.ObjectType.UnityEngineObjects2:
                                {
                                    ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                    {
                                        UnityEngine.Object _valueNew = EditorGUILayout.ObjectField("   |- Element " + index, (UnityEngine.Object)_valueOld, KissSerializableObject.GetSubType(fi.FieldType), true);
                                        if (_valueNew != (UnityEngine.Object)_valueOld)
                                        {
                                            SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                            valueOld[index] = _valueNew;
                                            _sp.objectReferenceValue = obj.objs[index];
                                            obj.Value = valueOld;
                                            hub.UpdateFieldInEditor(obj);
                                        }
                                    }, null);
                                }
                                break;
                            case KissSerializableObject.ObjectType.LikeBehaviourObjects:
                            case KissSerializableObject.ObjectType.LikeBehaviourObjects2:
                                {
                                    ProcessList(hub, obj, mySerializedProperty, fi, (_valueOld, sp, index, valueOld) =>
                                    {
                                        UnityEngine.Object _valueNew = EditorGUILayout.ObjectField("   |- Element " + index, (UnityEngine.Object)_valueOld, typeof(HotUpdateBehaviour), true);
                                        if (_valueNew != (UnityEngine.Object)_valueOld)
                                        {
                                            SerializedProperty _sp = sp.GetArrayElementAtIndex(index);
                                            valueOld[index] = _valueNew;
                                            _sp.objectReferenceValue = obj.objs[index];
                                            obj.Value = valueOld;
                                            hub.UpdateFieldInEditor(obj);
                                        }
                                    }, null);
                                }
                                break;
                            default:
                                {
                                    object _obj = obj.Value;
                                    if (_obj is UnityEngine.Object)
                                    {
                                        UnityEngine.Object _objOld = _obj as UnityEngine.Object;
                                        UnityEngine.Object _objNew = EditorGUILayout.ObjectField(GetTrTextContent(fi), _objOld, fi.FieldType, true);
                                        if (_objOld != _objNew)
                                        {
                                            obj.obj = _objNew;
                                            mySerializedProperty.obj.objectReferenceValue = _objNew;
                                            hub.UpdateFieldInEditor(obj);
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            string valueOld = (string)obj.Value;
                                            if (valueOld == null)
                                                valueOld = "";
                                            string valueNew = EditorGUILayout.TextField(GetTrTextContent(fi), valueOld);
                                            if (valueOld != valueNew)
                                            {
                                                obj.Value = valueNew;
                                                mySerializedProperty.obj.stringValue = obj.value;
                                                hub.UpdateFieldInEditor(obj);
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                break;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            if (!isValidClassName)
            {
                isInited = false;
                EditorGUILayout.LabelField("-- Must select a hot update class that inherited from LikeBehaviour --", errorStyle);
            }
            EditorGUILayout.Separator();
            SerializedProperty serializedProperty = serializedObject.FindProperty("bindHotUpdateClassFullName");
            if (serializedProperty.Next(false))
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField($"-- Serialize fields of '{serializedObject.targetObject.GetType().FullName}' --", centerStyle);
                do
                {
                    EditorGUILayout.PropertyField(serializedProperty, EditorGUIUtility.TrTextContent(serializedProperty.name, serializedProperty.tooltip));
                } while (serializedProperty.Next(false));
                EditorGUILayout.EndVertical();
            }
            serializedObject.ApplyModifiedProperties();

        }
        int Slider(RangeAttribute range, int valueOld, GUIContent content)
        {
            return EditorGUILayout.IntSlider(content, valueOld, (int)range.min, (int)range.max);
        }
        int Slider(RangeAttribute range, int valueOld, string content)
        {
            return EditorGUILayout.IntSlider(content, valueOld, (int)range.min, (int)range.max);
        }
        float Slider(RangeAttribute range, object valueOld, GUIContent content)
        {
            return EditorGUILayout.Slider(content, Convert.ToSingle(valueOld), range.min, range.max);
        }
        float Slider(RangeAttribute range, object valueOld, string content)
        {
            return EditorGUILayout.Slider(content, Convert.ToSingle(valueOld), range.min, range.max);
        }
        void ProcessList(HotUpdateBehaviour hub, KissSerializableObject obj, MySerializedProperty mySerializedProperty, FieldInfo fi, Action<object, SerializedProperty, int, System.Collections.IList> action, object defaultValue)
        {
            EditorGUILayout.BeginHorizontal();
            System.Collections.IList valueOld = obj.Value as System.Collections.IList;
            Array array = valueOld as Array;
            int countOld = valueOld.Count;
            int countNew = EditorGUILayout.IntField(GetTrTextContent(fi), valueOld.Count);
            SerializedProperty serializedPropertys = null;
            switch(obj.objectType)
            {
                case KissSerializableObject.ObjectType.LikeBehaviourObjects:
                case KissSerializableObject.ObjectType.LikeBehaviourObjects2:
                case KissSerializableObject.ObjectType.UnityEngineObjects:
                case KissSerializableObject.ObjectType.UnityEngineObjects2:
                    serializedPropertys = mySerializedProperty.objs;
                    break;
                default:
                    serializedPropertys = mySerializedProperty.values;
                    break;
            }
            if (countOld != countNew)
            {
                if (array != null)
                {
                    if (countNew <= 0)
                    {
                        array = KissSerializableObject.ResizeArray(array, 0);
                        serializedPropertys.ClearArray();
                        obj.Value = array;
                    }
                    else if (countOld < countNew)//+
                    {
                        array = KissSerializableObject.ResizeArray(array, countNew);
                        object _objDefault = countOld > 0 ? array.GetValue(countOld-1) : defaultValue;
                        serializedPropertys.arraySize = countNew;
                        for (int i = countOld; i < countNew; i++)
                            array.SetValue(_objDefault, i);
                        obj.Value = array;
                        for (int i = countOld; i < countNew; i++)
                        {
                            switch (obj.objectType)
                            {
                                case KissSerializableObject.ObjectType.Values:
                                case KissSerializableObject.ObjectType.Values2:
                                    {
                                        SerializedProperty sp = serializedPropertys.GetArrayElementAtIndex(i);
                                        sp.stringValue = obj.values[i];
                                    }
                                    break;
                                case KissSerializableObject.ObjectType.UnityEngineObjects:
                                case KissSerializableObject.ObjectType.UnityEngineObjects2:
                                case KissSerializableObject.ObjectType.LikeBehaviourObjects:
                                case KissSerializableObject.ObjectType.LikeBehaviourObjects2:
                                    {
                                        SerializedProperty sp = serializedPropertys.GetArrayElementAtIndex(i);
                                        sp.objectReferenceValue = obj.objs[i];
                                    }
                                    break;
                            }
                        }
                    }
                    else//-
                    {
                        array = KissSerializableObject.ResizeArray(array, countNew);
                        serializedPropertys.arraySize = countNew;
                        obj.Value = array;
                    }
                }
                else
                {
                    if (countNew <= 0)
                    {
                        countNew = 0;
                        valueOld.Clear();
                        serializedPropertys.ClearArray();
                    }
                    else if (countOld < countNew)//+
                    {
                        int count = countNew - countOld;
                        while (count-- > 0)
                        {
                            valueOld.Add(defaultValue);
                        }
                    }
                    else//-
                    {
                        int count = countOld - countNew;
                        while (count-- > 0)
                            valueOld.RemoveAt(valueOld.Count - 1);
                    }
                    serializedPropertys.arraySize = countNew;
                    obj.Value = valueOld;
                }
                hub.UpdateFieldInEditor(obj);
            }
            if (GUILayout.Button(m_btListAdd, GUILayout.Width(18), GUILayout.Height(18)))
            {
                if (array != null)
                {
                    array = KissSerializableObject.ResizeArray(array, array.Length + 1);
                    int i = array.Length - 1;
                    array.SetValue(valueOld.Count > 0 ? valueOld[valueOld.Count - 1] : defaultValue, i);
                    serializedPropertys.arraySize = array.Length;
                    obj.Value = array;
                    switch(obj.objectType)
                    {
                        case KissSerializableObject.ObjectType.Values:
                        case KissSerializableObject.ObjectType.Values2:
                            {
                                SerializedProperty sp = serializedPropertys.GetArrayElementAtIndex(i);
                                sp.stringValue = obj.values[i];
                            }
                            break;
                        case KissSerializableObject.ObjectType.UnityEngineObjects:
                        case KissSerializableObject.ObjectType.UnityEngineObjects2:
                        case KissSerializableObject.ObjectType.LikeBehaviourObjects:
                        case KissSerializableObject.ObjectType.LikeBehaviourObjects2:
                            {
                                SerializedProperty sp = serializedPropertys.GetArrayElementAtIndex(i);
                                sp.objectReferenceValue = obj.objs[i];
                            }
                            break;
                    }
                }
                else
                {
                    valueOld.Add(valueOld.Count > 0 ? valueOld[valueOld.Count - 1] : defaultValue);
                    serializedPropertys.arraySize = valueOld.Count;
                    obj.Value = valueOld;
                }
                hub.UpdateFieldInEditor(obj);
            }
            if (GUILayout.Button(m_btListSub, GUILayout.Width(18), GUILayout.Height(18)))
            {
                if (valueOld.Count > 0)
                {
                    if (array != null)
                    {
                        array = KissSerializableObject.ResizeArray(array, array.Length - 1);
                        serializedPropertys.arraySize = array.Length;
                        obj.Value = array;
                    }
                    else
                    {
                        valueOld.RemoveAt(valueOld.Count - 1);
                        serializedPropertys.arraySize = valueOld.Count;
                        obj.Value = valueOld;
                    }
                    hub.UpdateFieldInEditor(obj);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();
            if (valueOld.Count > 0)
            {
                SerializedProperty sp = serializedPropertys.Copy();
                sp.Next(true);
                for (int i = 0; i < valueOld.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    object _valueOld = valueOld[i];
                    action.Invoke(_valueOld, sp, i, valueOld);
                    if (GUILayout.Button(m_btListElementAdd, GUILayout.Width(18), GUILayout.Height(18)))
                    {
                        if (array != null)
                        {
                            array = KissSerializableObject.ResizeArray(array, array.Length + 1);
                            for (int j = array.Length - 1; j > i + 1; j--)
                                array.SetValue(array.GetValue(j-1), j);
                            array.SetValue(valueOld[i], i + 1);
                            serializedPropertys.InsertArrayElementAtIndex(i);
                            serializedPropertys.arraySize = array.Length;
                            obj.Value = array;
                        }
                        else
                        {
                            valueOld.Insert(i + 1, valueOld[i]);
                            serializedPropertys.InsertArrayElementAtIndex(i);
                            serializedPropertys.arraySize = valueOld.Count;
                            obj.Value = valueOld;
                        }
                        hub.UpdateFieldInEditor(obj);
                    }
                    if (GUILayout.Button(m_btListElementSub, GUILayout.Width(18), GUILayout.Height(18)))
                    {
                        if (valueOld.Count > 0)
                        {
                            if (array != null)
                            {
                                for (int j = i; j < array.Length-1; j++)
                                    array.SetValue(array.GetValue(j + 1), j);
                                array = KissSerializableObject.ResizeArray(array, array.Length - 1);
                                serializedPropertys.DeleteArrayElementAtIndex(i);
                                serializedPropertys.arraySize = array.Length;
                                obj.Value = array;
                            }
                            else
                            {
                                valueOld.RemoveAt(i);
                                serializedPropertys.DeleteArrayElementAtIndex(i);
                                serializedPropertys.arraySize = valueOld.Count;
                                obj.Value = valueOld;
                            }
                            hub.UpdateFieldInEditor(obj);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space(5);
            }
            EditorGUILayout.EndVertical();
        }
    }
}
