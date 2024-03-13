/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using CSharpLike.Internal;
using System.Reflection;
using UnityEngine.EventSystems;

namespace CSharpLike
{
	[HelpURL("https://www.csharplike.com/HotUpdateBehaviour.html")]
	public class HotUpdateBehaviour : MonoBehaviour
    {
		[SerializeField]
		List<KissSerializableObject> m_KissSerializeFields = new List<KissSerializableObject>();
		Dictionary<string, KissSerializableObject> m_KissSerializeFieldsDic;
		[SerializeField]
		bool m_KissFieldEnd;
		public Dictionary<string, KissSerializableObject> KissSerializeFields
        {
			get
			{
				if (m_KissSerializeFieldsDic == null)
				{
					m_KissSerializeFieldsDic = new Dictionary<string, KissSerializableObject>();
					foreach (KissSerializableObject field in m_KissSerializeFields)
						m_KissSerializeFieldsDic[field.name] = field;
				}
				return m_KissSerializeFieldsDic;
			}
			set
            {
				m_KissSerializeFieldsDic = value;

			}
        }
#if UNITY_EDITOR
		public int SerializeFieldsCount
        {
			get
            {
				int count = 0;
				foreach(KissSerializableObject item in KissSerializeFields.Values)
                {
					if (item != null && !item.IsHideInInspector)
						count++;
				}
				return count;
            }
        }
#endif
		public KissSerializableObject GetKissSerializableObject(string fieldName)
        {
			if (KissSerializeFields.TryGetValue(fieldName, out KissSerializableObject value))
				return value;
			return null;
		}
		public void NewKissSerializableObject(KissSerializableObject obj)
		{
			m_KissSerializeFields.Add(obj);
			KissSerializeFields[obj.name] = obj;
		}
		public void RemoveUnusedKissSerializableObject(Dictionary<string, bool> useds)
		{
			bool bModify = false;
			foreach(var one in KissSerializeFields)
            {
				if (!useds.ContainsKey(one.Key))
				{
					m_KissSerializeFields.Remove(one.Value);
					bModify = true;
				}
			}
			if (bModify)
				m_KissSerializeFieldsDic = null;
		}

		public string bindHotUpdateClassFullName = "";
		
		public bool EnableClick
		{
			get
			{
				BoxCollider bc = GetComponent<BoxCollider>();
				if (bc != null)
					return bc.enabled;
				BoxCollider2D bc2 = GetComponent<BoxCollider2D>();
				if (bc2 != null)
					return bc2.enabled;
				return false;
			}
			set
			{
				BoxCollider bc = GetComponent<BoxCollider>();
				if (bc != null)
					bc.enabled = value;
				BoxCollider2D bc2 = GetComponent<BoxCollider2D>();
				if (bc2 != null)
					bc2.enabled = value;
			}
		}

		private Dictionary<string, object> mObjectDatas;
		/// <summary>
		/// Set object to this instance
		/// </summary>
		public void SetObject(string key, object obj)
		{
			if (mObjectDatas == null)
				mObjectDatas = new Dictionary<string, object>();
			if (mObjectDatas.ContainsKey(key))
				mObjectDatas[key] = obj;
			else
				mObjectDatas.Add(key, obj);
		}
		/// <summary>
		/// Get object from this instance
		/// </summary>
		public object GetObject(string key, object oDefault = null)
		{
			if (mObjectDatas == null)
				mObjectDatas = new Dictionary<string, object>();
			if (mObjectDatas.ContainsKey(key))
				return mObjectDatas[key];
			return oDefault;
		}

		static Dictionary<string,Dictionary<string, bool>> functionAlls = new Dictionary<string, Dictionary<string, bool>>();
		Dictionary<string, bool> functions;
		protected bool HasFunction(string strFunc)
        {
			if (helper == null)
			{
				helper = new Helper(bindHotUpdateClassFullName, this);
				InitFunctions();
			}
			return functions.ContainsKey(strFunc);
		}
		void InitFunctions()
        {
			if (!functionAlls.ContainsKey(bindHotUpdateClassFullName))
            {
				functions = new Dictionary<string, bool>();
				functionAlls.Add(bindHotUpdateClassFullName, functions);
				helper.CheckMemberCall(functions);
			}
			else
				functions = functionAlls[bindHotUpdateClassFullName];
        }
        /// <summary>
        /// Start coroutine without param
        /// </summary>
        [Obsolete("Not supported 'coroutine' in FREE version (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256", true)]
        public new Coroutine StartCoroutine(string methodName)
        {
			return StartCoroutine(methodName, null);
		}
        /// <summary>
        /// Mark it obsolete, don't call this function.
        /// </summary>
        [Obsolete("Not supported 'coroutine' in FREE version (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256", true)]
        public new Coroutine StartCoroutine(IEnumerator routine)
		{
			return base.StartCoroutine(routine);
        }
        static Dictionary<string, HotUpdateBehaviour> bridges = new Dictionary<string, HotUpdateBehaviour>();
        /// <summary>
        /// Get the bridge instance by the hot update class full name.
        /// You must call this after call function 'ResourceManager.LoadCode' success.
        /// </summary>
        /// <param name="bindHotUpdateClassFullName">You full class name in your hot update class, default value is "CSharpLike.HotUpdateClassBridge".
        /// You can customize SOME classes with different usages, so it make your code tidy.</param>
        /// <returns></returns>
        public static HotUpdateBehaviour GetHotUpdateBridge(string bindHotUpdateClassFullName = "")
		{
			if (string.IsNullOrEmpty(bindHotUpdateClassFullName))
				bindHotUpdateClassFullName = ResourceManager.DefaultBridge;
			if (HotUpdateManager.vm.GetTypeByKeywordQuiet(bindHotUpdateClassFullName) == null)
				return null;
			if (!bridges.TryGetValue(bindHotUpdateClassFullName, out HotUpdateBehaviour behaviour))
            {
                GameObject go = new GameObject(bindHotUpdateClassFullName);
                DontDestroyOnLoad(go);
                go.SetActive(false);
                behaviour = go.AddComponent<HotUpdateBehaviour>();
                behaviour.bindHotUpdateClassFullName = bindHotUpdateClassFullName;
                go.SetActive(true);
                bridges[bindHotUpdateClassFullName] = behaviour;
            }
            return behaviour;
        }
        public static void ClearHotUpdateBridge()
        {
            foreach (HotUpdateBehaviour one in bridges.Values)
            {
                try
                {
                    if (one != null && one.gameObject != null)
                        Destroy(one.gameObject);
                }
                catch { }
            }
            bridges.Clear();
        }

        protected Helper helper = null;
#region Unity event
		void Start()
        {
			InitHelper();
			BindField();
			helper.MemberCall("Start");
		}
		void InitHelper()
        {
            if (helper == null)
            {
                helper = new Helper(bindHotUpdateClassFullName, this);
                InitFunctions();
			}
		}

        void Awake()
        {
            InitHelper();
			BindField();
			helper.MemberCall("Awake");

#if UNITY_EDITOR
			ReloadMaterialShaders(gameObject);
#endif
		}
		List<KissSerializableObject> m_KissSerializeFieldsCheck = null;
		void BindField()
        {
			if (m_KissSerializeFields.Count == 0)
				return;
			if (m_KissSerializeFieldsCheck == null)
            {
				BindField(m_KissSerializeFields);
			}
			else if (m_KissSerializeFieldsCheck.Count > 0)
			{
				BindField(m_KissSerializeFieldsCheck);
			}
        }
#if UNITY_EDITOR
		public void UpdateFieldInEditor(KissSerializableObject field)
		{
			if (!Application.isPlaying)
				return;
			object objInstance = ScriptInstance;
			SInstance sInstance = objInstance is SInstance ? objInstance as SInstance : null;
			LikeBehaviour likeBehaviour = objInstance is LikeBehaviour ? objInstance as LikeBehaviour : null;
			if (sInstance == null && likeBehaviour == null)
			{
				return;
			}
			Type typeLikeBehaviour = likeBehaviour?.GetType();
			switch (field.objectType)
			{
				case KissSerializableObject.ObjectType.LikeBehaviourObject:
					{
						object value = field.BehaviourValue?.ScriptInstance;
						if (value != null)
						{
							if (sInstance != null)
							{
								if (!sInstance.SetMemberValue(field.name, value))
								{
									Debug.LogWarning($"Not exist member '{field.name}' in GameObject '{name}'");
								}
							}
							else if (likeBehaviour != null)
							{
								FieldInfo fi = typeLikeBehaviour.GetField(field.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
								if (fi != null)
									fi.SetValue(likeBehaviour, value);
							}
						}
					}
					break;
				case KissSerializableObject.ObjectType.LikeBehaviourObjects:
					{
						List<object> list = field.Value as List<object>;
						if (sInstance != null)
						{
							List<SInstance> listInstance = new List<SInstance>();
							foreach (SInstance obj in list)
							{
								listInstance.Add(obj);
							}
							if (!sInstance.SetMemberValue(field.name, listInstance))
							{
								Debug.LogWarning($"Not exist member '{field.name}' in GameObject '{name}'");
							}
						}
						else if (likeBehaviour != null)
						{
							FieldInfo fi = typeLikeBehaviour.GetField(field.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
							if (fi != null)
							{
								IList listLikeBehaviour = Activator.CreateInstance(fi.FieldType) as IList;
								foreach (object obj in list)
								{
									listLikeBehaviour.Add(obj);
								}
								fi.SetValue(likeBehaviour, listLikeBehaviour);
							}
						}
					}
					break;
				case KissSerializableObject.ObjectType.LikeBehaviourObjects2:
					{
						object[] list = field.Value as object[];
						if (sInstance != null)
						{
							SInstance[] listInstance = new SInstance[list.Length];
							for (int i = 0; i < list.Length; i++)
							{
								SInstance obj = list[i] as SInstance;
								listInstance[i] = obj;
							}
							if (!sInstance.SetMemberValue(field.name, listInstance))
							{
								Debug.LogWarning($"Not exist member '{field.name}' in GameObject '{name}'");
							}
						}
						else if (likeBehaviour != null)
						{
							FieldInfo fi = typeLikeBehaviour.GetField(field.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
							if (fi != null)
							{
								Array listLikeBehaviour = Activator.CreateInstance(fi.FieldType, new object[] { list.Length }) as Array;
								for (int i = 0; i < list.Length; i++)
								{
									object obj = list[i];
									listLikeBehaviour.SetValue(obj, i);
								}
								fi.SetValue(likeBehaviour, listLikeBehaviour);
							}
						}
					}
					break;
				case KissSerializableObject.ObjectType.UnityEngineObject:
				case KissSerializableObject.ObjectType.Value:
				case KissSerializableObject.ObjectType.UnityEngineObjects:
				case KissSerializableObject.ObjectType.Values:
				case KissSerializableObject.ObjectType.UnityEngineObjects2:
				case KissSerializableObject.ObjectType.Values2:
					{
						if (sInstance != null)
						{
							if (!sInstance.SetMemberValue(field.name, field.Value))
							{
								Debug.LogWarning($"Not exist member '{field.name}' in GameObject '{name}'");
							}
						}
						else if (likeBehaviour != null)
						{
							FieldInfo fi = typeLikeBehaviour.GetField(field.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
							if (fi != null)
								fi.SetValue(likeBehaviour, field.Value);
						}
					}
					break;
			}
		}
#endif
		void BindField(List<KissSerializableObject> fields)
        {
			bool isFirst;
			if (m_KissSerializeFieldsCheck == null)
            {
				m_KissSerializeFieldsCheck = new List<KissSerializableObject>();
				isFirst = true;
			}
			else
			{
				isFirst = false;
			}
			if (fields.Count == 0)
				return;
			object objInstance = ScriptInstance;
			SInstance sInstance = objInstance is SInstance ? objInstance as SInstance : null;
			LikeBehaviour likeBehaviour = objInstance is LikeBehaviour ? objInstance as LikeBehaviour : null;
			if (sInstance == null && likeBehaviour == null)
            {
				Debug.LogError($"Not exist hot update instance at GameObject '{name}'");
				return;
            }
			Type typeLikeBehaviour = likeBehaviour?.GetType();
			foreach (KissSerializableObject field in fields)
            {
				switch(field.objectType)
                {
					case KissSerializableObject.ObjectType.LikeBehaviourObject:
						{
							object value = field.BehaviourValue?.ScriptInstance;
							if (value != null)
							{
								if (sInstance != null)
								{
									if (!sInstance.SetMemberValue(field.name, value))
									{
										Debug.LogWarning($"Not exist member '{field.name}' in GameObject '{name}'");
									}
								}
								else if (likeBehaviour != null)
								{
									FieldInfo fi = typeLikeBehaviour.GetField(field.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
									if (fi != null)
										fi.SetValue(likeBehaviour, value);
								}
							}
							else if (isFirst)
								m_KissSerializeFieldsCheck.Add(field);
						}
						break;
					case KissSerializableObject.ObjectType.LikeBehaviourObjects:
						{
							List<object> list = field.Value as List<object>;
							bool hasNull = false;
							if (sInstance != null)
							{
								List<SInstance> listInstance = new List<SInstance>();
								foreach (SInstance obj in list)
								{
									if (obj == null)
										hasNull = true;
									listInstance.Add(obj);
								}
								if (!sInstance.SetMemberValue(field.name, listInstance))
								{
									Debug.LogWarning($"Not exist member '{field.name}' in GameObject '{name}'");
								}
							}
							else if (likeBehaviour != null)
							{
                                FieldInfo fi = typeLikeBehaviour.GetField(field.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                                if (fi != null)
                                {
									IList listLikeBehaviour = Activator.CreateInstance(fi.FieldType) as IList;
                                    foreach (object obj in list)
                                    {
                                        if (obj == null)
                                            hasNull = true;
                                        listLikeBehaviour.Add(obj);
                                    }
                                    fi.SetValue(likeBehaviour, listLikeBehaviour);
                                }
							}
							if (isFirst && hasNull)
								m_KissSerializeFieldsCheck.Add(field);
						}
						break;
					case KissSerializableObject.ObjectType.LikeBehaviourObjects2:
						{
							object[] list = field.Value as object[];
							bool hasNull = false;
							if (sInstance != null)
							{
								SInstance[] listInstance = new SInstance[list.Length];
								for(int i=0; i< list.Length; i++)
								{
									SInstance obj = list[i] as SInstance;
									if (obj == null)
										hasNull = true;
									listInstance[i] = obj;
								}
								if (!sInstance.SetMemberValue(field.name, listInstance))
								{
									Debug.LogWarning($"Not exist member '{field.name}' in GameObject '{name}'");
								}
							}
							else if (likeBehaviour != null)
							{
								FieldInfo fi = typeLikeBehaviour.GetField(field.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
								if (fi != null)
								{
									Array listLikeBehaviour = Activator.CreateInstance(fi.FieldType, new object[] { list.Length }) as Array;
									for (int i = 0; i < list.Length; i++)
									{
										object obj = list[i];
										if (obj == null)
											hasNull = true;
										listLikeBehaviour.SetValue(obj, i);
									}
									fi.SetValue(likeBehaviour, listLikeBehaviour);
								}
							}
							if (isFirst && hasNull)
								m_KissSerializeFieldsCheck.Add(field);
						}
						break;
					case KissSerializableObject.ObjectType.UnityEngineObject:
					case KissSerializableObject.ObjectType.Value:
					case KissSerializableObject.ObjectType.UnityEngineObjects:
					case KissSerializableObject.ObjectType.Values:
					case KissSerializableObject.ObjectType.UnityEngineObjects2:
					case KissSerializableObject.ObjectType.Values2:
						{
							if (sInstance != null)
							{
								if (!sInstance.SetMemberValue(field.name, field.Value))
								{
									Debug.LogWarning($"Not exist member '{field.name}' in GameObject '{name}'");
								}
							}
							else if (likeBehaviour != null)
							{
								FieldInfo fi = typeLikeBehaviour.GetField(field.name);
								if (fi != null)
									fi.SetValue(likeBehaviour, field.Value);
							}
						}
						break;
				}
            }
			if (!isFirst)
				m_KissSerializeFieldsCheck = null;
		}
		/// <summary>
		/// Reload the material shader
		/// </summary>
		public static void ReloadMaterialShader(GameObject go)
        {
			if (go == null)
				return;
			UnityEngine.UI.Graphic graphic = go.GetComponentInChildren<UnityEngine.UI.Graphic>(true);
			if (graphic != null && graphic.materialForRendering != null)
				graphic.materialForRendering.shader = Shader.Find(graphic.materialForRendering.shader.name);
		}
		/// <summary>
		/// Reload the all material shaders
		/// </summary>
		public static void ReloadMaterialShaders(GameObject go)
		{
			if (go == null)
				return;
			UnityEngine.UI.Graphic[] graphics = go.GetComponentsInChildren<UnityEngine.UI.Graphic>(true);
			Dictionary<string, bool> caches = new Dictionary<string, bool>();
			foreach(UnityEngine.UI.Graphic graphic in graphics)
			{
				if (graphic != null && graphic.materialForRendering != null)
				{
					string name = graphic.materialForRendering.shader.name;
					if (!caches.ContainsKey(name))
					{
						caches[name] = true;
						graphic.materialForRendering.shader = Shader.Find(graphic.materialForRendering.shader.name);
					}
				}
			}
		}
		public void ReloadMaterial<T>() where T : UnityEngine.UI.Graphic
        {
            T graphic = GetComponentInChildren<T>(true);
			if (graphic != null && graphic.materialForRendering != null)
				graphic.materialForRendering.shader = Shader.Find(graphic.materialForRendering.shader.name);
		}
		void OnDestroy()
		{
			helper.MemberCall("OnDestroy");
		}
		void OnEnable()
		{
			helper.MemberCall("OnEnable");
		}
		void OnDisable()
		{
			helper.MemberCall("OnDisable");
		}
#endregion //Unity event

		public bool isHotUpdateScript()
        {
			return helper != null && helper.isHotUpdateScript();
        }
        /// <summary>
        /// receive event message
        /// </summary>
        /// <param name="param">first param as function name, the other as params of that function</param>
        public object OnRecivedEvent(List<object> param)
		{
			if (param == null)
				return null;
			int count = param.Count;
			if (count == 0)
				return null;
			else if (count == 1)
				return helper.MemberCall((string)param[0]);
			else
			{
				object[] objs = new object[count - 1];
				for (int i = 1; i < count; i++)
					objs[i - 1] = param[i];
				return helper.MemberCall((string)param[0], objs);
			}
		}


		/// <summary>
		/// call member method from a string,that string will split into string[] by '+' as params(the first param is method name)
		/// </summary>
		public object MemberCallEx(string strSplit)
		{
			string[] strs = strSplit.Split('+');
			if (strs.Length == 0)
				return null;
			else if (strs.Length == 1)
				return helper.MemberCall(strs[0]);
			else
            {
				object[] objs = new object[strs.Length-1];
				for (int i = 1; i < strs.Length; i++)
					objs[i - 1] = strs[i];
				return helper.MemberCall(strs[0], objs);
			}
        }
        /// <summary>
        /// call member method without 0 param
        /// </summary>
        public void MemberCallForEvent(string funName)
		{
			helper.MemberCall(funName);
		}
		#region Event for UGUI compenent 
		/// <summary>
		/// Call member method without 0~N param, you can call mothed with params in prefab or scene.
		/// If method with params, support param type string/int/uint/ulong/long/float/double/bool/char.
		/// e.g.
		/// string type : Func("your string") 
		/// char type : Func('A')
		/// int type : Func(123)
		/// uint type : Func(123u)
		/// ulong type : Func(123UL)
		/// long type : Func(123L)
		/// float type : Func(123.321f)
		/// double type : Func(123D)
		/// bool type : Func(true)
		/// multiple types : Func("str", false, 123, 321.4f)
		/// </summary>
		/// <param name="funNameWithParams">method name,if null or empty then default as 'OnClick'</param>
		public void OnClick(string funNameWithParams)
        {
            funNameWithParams = funNameWithParams.Trim();
            if (string.IsNullOrEmpty(funNameWithParams))
                helper.MemberCall("OnClick");
            else
            {
                if (SplitFuncNameWithParams(funNameWithParams, out string funcName, out object[] param))
                {
                    if (param == null || param.Length == 0)
                        helper.MemberCall(funcName);
                    else
                        helper.MemberCall(funcName, param);
                }
            }
		}
		/// <summary>
		/// This mothed for OnValueChanged event of Toggle componenet.
		/// </summary>
		/// <param name="value">The Value from Toggle componenet</param>
		public void OnToggleValueChanged(bool value)
		{
			helper.MemberCall("OnToggleValueChanged", value);
		}
		/// <summary>
		/// This mothed for OnValueChanged event of Scrollbar componenet.
		/// </summary>
		/// <param name="value">The Value from Scrollbar componenet</param>
		public void OnScrollbarValueChanged(float value)
		{
			helper.MemberCall("OnScrollbarValueChanged", value);
		}
		/// <summary>
		/// This mothed for OnValueChanged event of Slider componenet.
		/// </summary>
		/// <param name="value">The Value from Slider componenet</param>
		public void OnSliderValueChanged(float value)
		{
			helper.MemberCall("OnSliderValueChanged", value);
		}
		/// <summary>
		/// This mothed for OnValueChanged event of ScrollView componenet.
		/// </summary>
		/// <param name="value">The Value from ScrollView componenet</param>
		public void OnScrollViewValueChanged(Vector2 value)
		{
			helper.MemberCall("OnScrollViewValueChanged", value);
		}
		/// <summary>
		/// This mothed for OnValueChanged event of InputField componenet.
		/// </summary>
		/// <param name="value">The Value from InputField componenet</param>
		public void OnInputFieldValueChanged(string value)
		{
			helper.MemberCall("OnInputFieldValueChanged", value);
		}
		/// <summary>
		/// This mothed for OnEndEdit event of InputField componenet.
		/// </summary>
		/// <param name="value">The Value from InputField componenet</param>
		public void OnInputFieldEndEdit(string value)
		{
			helper.MemberCall("OnInputFieldEndEdit", value);
		}
		/// <summary>
		/// This mothed for OnSelect event of InputField componenet.
		/// </summary>
		/// <param name="value">The Value from InputField componenet</param>
		public void OnInputFieldSelect(string value)
		{
			helper.MemberCall("OnInputFieldSelect", value);
		}
		/// <summary>
		/// This mothed for Deselect event of InputField componenet.
		/// </summary>
		/// <param name="value">The Value from InputField componenet</param>
		public void OnInputFieldDeselect(string value)
		{
			helper.MemberCall("OnInputFieldDeselect", value);
		}
		/// <summary>
		/// This mothed for OnValueChanged event of Dropdown componenet.
		/// </summary>
		/// <param name="value">The Value from Dropdown componenet</param>
		public void OnDropdownValueChanged(int value)
		{
			helper.MemberCall("OnDropdownValueChanged", value);
		}
#endregion //Event for UGUI compenent 
		static bool SplitFuncNameWithParams(string funNameWithParams, out string funcName, out object[] param)
        {
            funcName = "";
            param = null;
            if (funcName == null)
                return false;
            funNameWithParams = funNameWithParams.Trim();
            if (funNameWithParams.Length == 0)
                return false;
            if (funNameWithParams.EndsWith(")"))
            {
                int index = funNameWithParams.IndexOf("(");
                if (index > 0)
                {
                    funcName = funNameWithParams.Substring(0, index);
                    string strParams = funNameWithParams.Substring(index + 1);
                    if (strParams.Length <= 1)
                    {
                        return true;
                    }
                    else
                    {
                        string[] strs = strParams.Substring(0, strParams.Length - 1).Trim().Split(',');
                        param = new object[strs.Length];
                        for (int i = strs.Length - 1; i >= 0; i--)
                        {
                            string str = strs[i].Trim();
                            if (str.StartsWith("\""))
                                param[i] = (str.Length > 2) ? str.Substring(1, str.Length - 2) : "";
                            else if (str.StartsWith("'"))
                                param[i] = Convert.ToChar((str.Length > 2) ? str.Substring(1, str.Length - 2) : "");
                            else
                            {
                                str = str.ToLower();
                                if (str.EndsWith("f"))
                                    param[i] = Convert.ToSingle(str, System.Globalization.CultureInfo.InvariantCulture);
                                else if (str.EndsWith("d"))
                                    param[i] = Convert.ToDouble(str, System.Globalization.CultureInfo.InvariantCulture);
                                else if (str.EndsWith("ul"))
                                    param[i] = Convert.ToUInt64(str);
                                else if (str.EndsWith("l"))
                                    param[i] = Convert.ToInt64(str);
                                else if (str.EndsWith("u"))
                                    param[i] = Convert.ToInt32(str);
                                else if (str == "true")
                                    param[i] = true;
                                else if (str == "false")
                                    param[i] = false;
                                else if (str.Contains("."))
                                    param[i] = Convert.ToSingle(str, System.Globalization.CultureInfo.InvariantCulture);
                                else
                                    param[i] = Convert.ToInt32(str);
                            }
                        }
                    }
                    return true;
                }
                Debug.LogError($"execute function '{funNameWithParams}' error");
            }
            else
            {
                funcName = funNameWithParams;
                return true;
            }
            return false;
        }
        public void OnTweener(KissTweenBase tweener, string funNameWithParams)
        {
            funNameWithParams = funNameWithParams.Trim();
            if (!string.IsNullOrEmpty(funNameWithParams))
            {
                if (SplitFuncNameWithParams(funNameWithParams, out string funcName, out object[] param))
                {
                    if (param == null || param.Length == 0)
                        helper.MemberCall(funcName, tweener);
                    else
                    {
                        object[] param2 = new object[param.Length + 1];
                        param2[0] = tweener;
                        for (int i = param.Length - 1; i >= 0; i--)
                        {
                            param2[i + 1] = param[i];
                        }
                        helper.MemberCall(funcName, param2);
                    }
                }
            }
        }
        /// <summary>
        /// call member method with no param
        /// </summary>
        public object MemberCall(string funName)
        {
            return helper.MemberCall(funName);
        }
        /// <summary>
        /// call member method with 1 param
        /// </summary>
        public object MemberCall(string funName, object v1)
        {
            return helper.MemberCall(funName, v1);
        }
        /// <summary>
        /// call member method with 2 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2)
        {
            return helper.MemberCall(funName, v1, v2);
        }
        /// <summary>
        /// call member method with 3 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3)
        {
            return helper.MemberCall(funName, v1, v2, v3);
        }
        /// <summary>
        /// call member method with 4 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4);
        }
        /// <summary>
        /// call member method with 5 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5);
        }
        /// <summary>
        /// call member method with 6 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6);
        }
        /// <summary>
        /// call member method with 7 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7);
        }
        /// <summary>
        /// call member method with 8 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8);
        }
        /// <summary>
        /// call member method with 9 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9);
        }
        /// <summary>
        /// call member method with 10 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10);
        }
        /// <summary>
        /// call member method with 11 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11);
        }
        /// <summary>
        /// call member method with 12 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11, object v12)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12);
        }
        /// <summary>
        /// call member method with 13 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11, object v12, object v13)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13);
        }
        /// <summary>
        /// call member method with 14 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11, object v12, object v13, object v14)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14);
        }
        /// <summary>
        /// call member method with 15 params
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11, object v12, object v13, object v14, object v15)
        {
            return helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14, v15);
		}
		/// <summary>
		/// HTTP post JSON request.
		/// It's for the FREE version quick use the HTTP post request because the FREE version not support coroutine.
		/// The full version can direct using UnityEngine.Networking.UnityWebRequest.
		/// </summary>
		/// <param name="url">the http url</param>
		/// <param name="postData">the JSON data post to server</param>
		/// <param name="callback">the http callback(string callbackText, string error)</param>
		/// <param name="timeout">the timeout of the web request, default is 10 seconds</param>
		public void HttpPost(string url, JSONData postData, Action<string, string> callback = null, int timeout = 10)
		{
			base.StartCoroutine(colHttpPost(url, postData, callback, timeout));
		}
		IEnumerator colHttpPost(string url, JSONData postData, Action<string, string> callback, int timeout)
		{
			using (UnityEngine.Networking.UnityWebRequest uwr = new UnityEngine.Networking.UnityWebRequest(url, "POST"))
			{
				uwr.timeout = timeout;
				byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postData);
				uwr.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(postBytes);
				uwr.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
				uwr.SetRequestHeader("Content-Type", "application/json");
				yield return uwr.SendWebRequest();
				if (callback != null)
					callback(uwr.downloadHandler.text, uwr.error);
			}
		}
		/// <summary>
		/// HTTP post request.
		/// It's for the FREE version quick use the HTTP post request because the FREE version not support coroutine.
		/// The full version can direct using UnityEngine.Networking.UnityWebRequest.
		/// </summary>
		/// <param name="url">the http url</param>
		/// <param name="postData">the data post to server</param>
		/// <param name="callback">the http callback(string callbackText, string error)</param>
		/// <param name="timeout">the timeout of the web request, default is 10 seconds</param>
		public void HttpPost(string url, string postData, Action<string, string> callback = null, int timeout = 10)
		{
			base.StartCoroutine(colHttpPost(url, postData, callback, timeout));
		}
		IEnumerator colHttpPost(string url, string postData, Action<string, string> callback, int timeout)
		{
			using (UnityEngine.Networking.UnityWebRequest uwr = new UnityEngine.Networking.UnityWebRequest(url, "POST"))
			{
				uwr.timeout = timeout;
				byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(postData);
				uwr.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(postBytes);
				uwr.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
				yield return uwr.SendWebRequest();
				if (callback != null)
					callback(uwr.downloadHandler.text, uwr.error);
			}
		}
		/// <summary>
		/// HTTP get request.
		/// It's for the FREE version quick use the HTTP get request because the FREE version not support coroutine.
		/// The full version can direct using UnityEngine.Networking.UnityWebRequest.
		/// </summary>
		/// <param name="url">the http url</param>
		/// <param name="callback">the http callback(string callbackText, string error)</param>
		/// <param name="timeout">the timeout of the web request, default is 10 seconds</param>
		public void HttpGet(string url, Action<string, string> callback = null, int timeout = 10)
		{
			base.StartCoroutine(colHttpGet(url, callback, timeout));
		}
		IEnumerator colHttpGet(string url, Action<string, string> callback, int timeout)
		{
			using (UnityEngine.Networking.UnityWebRequest uwr = UnityEngine.Networking.UnityWebRequest.Get(url))
			{
				uwr.timeout = timeout;
				yield return uwr.SendWebRequest();
				if (callback != null)
					callback(uwr.downloadHandler.text, uwr.error);
			}
		}
		/// <summary>
		/// HTTP download file request.
		/// It's for the FREE version quick use the HTTP download file request because the FREE version not support coroutine.
		/// The full version can direct using UnityEngine.Networking.UnityWebRequest.
		/// </summary>
		/// <param name="url">the http url of the file that you want to download.</param>
		/// <param name="savePath">Where the file you want to save, normally is 'Application.persistentDataPath+"/"+YourFileName'</param>
		/// <param name="callback">the http callback(string error)</param>
		/// <param name="timeout">the timeout of the web request, default is 5 minutes</param>
		public void HttpDownload(string url, string savePath, Action<string> callback = null, int timeout = 300)
		{
			base.StartCoroutine(colHttpDownloadRequest(url, savePath, callback, timeout));
		}
		IEnumerator colHttpDownloadRequest(string url, string savePath, Action<string> callback, int timeout)
		{
			using (UnityEngine.Networking.UnityWebRequest uwr = UnityEngine.Networking.UnityWebRequest.Get(url))
			{
				uwr.timeout = timeout;
				if (string.IsNullOrEmpty(savePath))
					savePath = Application.persistentDataPath;
				uwr.downloadHandler = new UnityEngine.Networking.DownloadHandlerFile(savePath);
				yield return uwr.SendWebRequest();
				if (callback != null)
					callback(uwr.error);
#if UNITY_IOS
				UnityEngine.iOS.Device.SetNoBackupFlag(savePath);
#endif
			}
		}

		/// <summary>
		/// delay call member method without param
		/// </summary>
		/// <param name="funName">method name</param>
		/// <param name="delay">delay time in seconds</param>
		public void MemberCallDelay(string funName, float delay = 0.0f)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName);
		}

		/// <summary>
		/// delay call member method with 1 param
		/// </summary>
		/// <param name="funName">method name</param>
		/// <param name="v1">param 1</param>
		/// <param name="delay">delay time in seconds</param>
		public void MemberCallDelay(string funName, object v1, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1);
		}

        /// <summary>
        /// delay call member method with 2 params
        /// </summary>
        /// <param name="funName">method name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="delay">delay time in seconds</param>
        public void MemberCallDelay(string funName, object v1, object v2, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, v2, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, v2, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, object v2, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1, v2);
		}

        /// <summary>
        /// delay call member method with 3 params
        /// </summary>
        /// <param name="funName">method name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="delay">delay time in seconds</param>
        public void MemberCallDelay(string funName, object v1, object v2, object v3, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, v2, v3, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, v2, v3, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, object v2, object v3, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1, v2, v3);
		}

        /// <summary>
        /// delay call member method with 4 params
        /// </summary>
        /// <param name="funName">method name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="delay">delay time in seconds</param>
        public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, v2, v3, v4, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, v2, v3, v4, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, object v2, object v3, object v4, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1, v2, v3, v4);
		}

        /// <summary>
        /// delay call member method with 5 params
        /// </summary>
        /// <param name="funName">method name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="delay">delay time in seconds</param>
        public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, v2, v3, v4, v5, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, v2, v3, v4, v5, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1, v2, v3, v4, v5);
		}

        /// <summary>
        /// delay call member method with 6 params
        /// </summary>
        /// <param name="funName">method name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="delay">delay time in seconds</param>
        public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, v2, v3, v4, v5, v6, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, v2, v3, v4, v5, v6, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1, v2, v3, v4, v5, v6);
		}

        /// <summary>
        /// delay call member method with 7 params
        /// </summary>
        /// <param name="funName">method name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="v7">param 7</param>
        /// <param name="delay">delay time in seconds</param>
        public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, v2, v3, v4, v5, v6, v7, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, v2, v3, v4, v5, v6, v7, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7);
		}

        /// <summary>
        /// delay call member method with 8 params
        /// </summary>
        /// <param name="funName">method name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="v7">param 7</param>
        /// <param name="v8">param 8</param>
        /// <param name="delay">delay time in seconds</param>
        public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, v2, v3, v4, v5, v6, v7, v8, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, v2, v3, v4, v5, v6, v7, v8, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8);
		}

        /// <summary>
        /// delay call member method with 9 params
        /// </summary>
        /// <param name="funName">method name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="v7">param 7</param>
        /// <param name="v8">param 8</param>
        /// <param name="v9">param 9</param>
        /// <param name="delay">delay time in seconds</param>
        public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9);
        }

        /// <summary>
        /// delay call member method with 10 params
        /// </summary>
        /// <param name="funName">method name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="v7">param 7</param>
        /// <param name="v8">param 8</param>
        /// <param name="v9">param 9</param>
        /// <param name="v10">param 10</param>
        /// <param name="delay">delay time in seconds</param>
        public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, float delay)
		{
			if (gameObject.activeInHierarchy) base.StartCoroutine(CorMemberCallDelay(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, delay));
			else HotUpdateManager.instance.MemberCallDelay(this, funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, delay);
		}
		protected IEnumerator CorMemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, float delay)
		{
			yield return new WaitForSeconds(delay);
			helper.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10);
		}
#region event of EventSystem
		public void OnBeginDrag(BaseEventData eventData)
        {
			helper.MemberCall("OnBeginDrag", eventData);
		}
		public void OnCancel(BaseEventData eventData)
		{
			helper.MemberCall("OnCancel", eventData);
		}
		public void OnDeselect(BaseEventData eventData)
		{
			helper.MemberCall("OnDeselect", eventData);
		}
		public void OnDrag(BaseEventData eventData)
		{
			helper.MemberCall("OnDrag", eventData);
		}
		public void OnDrop(BaseEventData eventData)
		{
			helper.MemberCall("OnDrop", eventData);
		}
		public void OnEndDrag(BaseEventData eventData)
		{
			helper.MemberCall("OnEndDrag", eventData);
		}
		public void OnInitializePotentialDrag(BaseEventData eventData)
		{
			helper.MemberCall("OnInitializePotentialDrag", eventData);
		}
		public void OnMove(BaseEventData eventData)
		{
			helper.MemberCall("OnMove", eventData);
		}
		public void OnPointerClick(BaseEventData eventData)
		{
			helper.MemberCall("OnPointerClick", eventData);
		}
		public void OnPointerDown(BaseEventData eventData)
		{
			helper.MemberCall("OnPointerDown", eventData);
		}
		public void OnPointerEnter(BaseEventData eventData)
		{
			helper.MemberCall("OnPointerEnter", eventData);
		}
		public void OnPointerExit(BaseEventData eventData)
		{
			helper.MemberCall("OnPointerExit", eventData);
		}
		public void OnPointerUp(BaseEventData eventData)
		{
			helper.MemberCall("OnPointerUp", eventData);
		}
		public void OnScroll(BaseEventData eventData)
		{
			helper.MemberCall("OnScroll", eventData);
		}
		public void OnSelect(BaseEventData eventData)
		{
			helper.MemberCall("OnSelect", eventData);
		}
		public void OnSubmit(BaseEventData eventData)
		{
			helper.MemberCall("OnSubmit", eventData);
		}
		public void OnUpdateSelected(BaseEventData eventData)
		{
			helper.MemberCall("OnUpdateSelected", eventData);
		}
#endregion //event of EventSystem
		/// <summary>
		/// get self instance of hot update script
		/// </summary>
		public CSL_Content.Value ScriptValue
		{
			get
			{
				if (helper != null)
				{
					CSL_Content.Value v = new CSL_Content.Value();
					if (helper.mType != null)
					{
						v.type = helper.mType;
						v.value = helper.mObject;
					}
					else
					{
						v.type = helper.content.CallType;
						v.value = helper.content.CallThis;
					}
					return v;
				}
				else
				{
					return CSL_Content.Value.Void;
				}
			}
		}
		public object ScriptInstance
		{
			get
			{
				return ScriptValue.value;
			}
		}
		public object GetComponentEx(string bindHotUpdateClassFullName)
		{
			foreach (HotUpdateBehaviour one in gameObject.GetComponents<HotUpdateBehaviour>())
			{
				if (one.bindHotUpdateClassFullName.Replace("+", ".") == bindHotUpdateClassFullName)
					return one.ScriptInstance;
			}
			return null;
		}
		public object[] GetComponentsEx(string bindHotUpdateClassFullName)
		{
			List<object> results = new List<object>();
			foreach (HotUpdateBehaviour one in gameObject.GetComponents<HotUpdateBehaviour>())
			{
				if (one.bindHotUpdateClassFullName.Replace("+", ".") == bindHotUpdateClassFullName)
					results.Add(one.ScriptInstance);
			}
			return results.ToArray();
		}
		public object GetComponentInChildrenEx(string bindHotUpdateClassFullName)
		{
			foreach (HotUpdateBehaviour one in gameObject.GetComponentsInChildren<HotUpdateBehaviour>())
			{
				if (one.bindHotUpdateClassFullName.Replace("+", ".") == bindHotUpdateClassFullName)
					return one.ScriptInstance;
			}
			return null;
		}
		public object[] GetComponentsInChildrenEx(string bindHotUpdateClassFullName)
		{
			List<object> results = new List<object>();
			foreach (HotUpdateBehaviour one in gameObject.GetComponentsInChildren<HotUpdateBehaviour>())
			{
				if (one.bindHotUpdateClassFullName.Replace("+", ".") == bindHotUpdateClassFullName)
					results.Add(one.ScriptInstance);
			}
			return results.ToArray();
		}
		public object GetComponentInParentEx(string bindHotUpdateClassFullName)
		{
			foreach (HotUpdateBehaviour one in gameObject.GetComponentsInParent<HotUpdateBehaviour>())
			{
				if (one.bindHotUpdateClassFullName.Replace("+", ".") == bindHotUpdateClassFullName)
					return one.ScriptInstance;
			}
			return null;
		}
		public object[] GetComponentsInParentEx(string bindHotUpdateClassFullName)
		{
			List<object> results = new List<object>();
			foreach (HotUpdateBehaviour one in gameObject.GetComponentsInParent<HotUpdateBehaviour>())
			{
				if (one.bindHotUpdateClassFullName.Replace("+", ".") == bindHotUpdateClassFullName)
					results.Add(one.ScriptInstance);
			}
			return results.ToArray();
		}
		internal static object GetComponentByFullName(GameObject go, string name)
		{
			if (go != null)
			{
				HotUpdateBehaviour hub = go.GetComponent<HotUpdateBehaviour>();
				if (hub != null)
					return hub.GetComponentEx(name);
			}
			return null;
		}
		public static object GetComponentByType(GameObject go, Type type)
        {
			if (type == null)
				return null;
			return GetComponentByFullName(go, type.FullName);
		}
		public static object GetComponentByType(GameObject go, SType type)
		{
			if (type == null)
				return null;
			return GetComponentByFullName(go, type.FullName);
		}
		static object GetComponentByFullName(Component component, string name)
		{
			if (component != null)
			{
				HotUpdateBehaviour hub = component.GetComponent<HotUpdateBehaviour>();
				if (hub != null)
					return hub.GetComponentEx(name);
			}
			return null;
		}
		public static object GetComponentByType(Component component, Type type)
		{
			if (type == null)
				return null;
			return GetComponentByFullName(component, type.FullName);
		}
		public static object GetComponentByType(Component component, SType type)
		{
			if (type == null)
				return null;
			return GetComponentByFullName(component, type.FullName);
		}
		static object[] FindObjectsByFullName(string fullName)
		{
			List<object> objs = new List<object>();
			foreach (HotUpdateBehaviour one in FindObjectsOfType<HotUpdateBehaviour>())
			{
				if (one != null && one.bindHotUpdateClassFullName.Replace("+",".") == fullName)
					objs.Add(one.ScriptInstance);
			}
			return objs.ToArray();
		}
		/// <summary>
		/// Find all objects by Type that inherited from LikeBehaviour.
		/// You should call this in Start instead of Awake, due to the HotUpdateBehaviour may be was not initialized yet.
		/// </summary>
		/// <param name="type">The Type that inherited from LikeBehaviour</param>
		public static object[] FindObjectsOfByType(SType type)
        {
			return FindObjectsByFullName(type.FullName);
		}
		/// <summary>
		/// Find all objects by Type that inherited from LikeBehaviour.
		/// You should call this in Start instead of Awake, due to the HotUpdateBehaviour may be was not initialized yet.
		/// </summary>
		/// <param name="type">The Type that inherited from LikeBehaviour</param>
		public static object[] FindObjectsOfByType(Type type)
		{
			return FindObjectsByFullName(type.FullName);
		}
		/// <summary>
		/// get self content of this hot update script
		/// </summary>
		public CSL_Content content
        {
			get
            {
				return helper != null ? helper.content : null;
            }
        }
#region Private implementation
		protected class Helper
		{
			internal Type mType;
			internal object mObject;

			CSL_TypeBase type;
			SInstance inst;
			internal CSL_Content content;
			HotUpdateBehaviour self;
			public Helper(string bindHotUpdateClassName, HotUpdateBehaviour behaviour)
			{
				if (string.IsNullOrEmpty(bindHotUpdateClassName))
					throw new Exception("Are you forget input the full name of the hot update class in prefab?" + behaviour.name);
				self = behaviour;
#if !_CSHARP_LIKE_ && UNITY_EDITOR
				mType = Type.GetType(bindHotUpdateClassName);
				if (mType == null)
				{
					Debug.LogError("not exist class " + bindHotUpdateClassName);
					return;
				}
				mObject = mType.Assembly.CreateInstance(bindHotUpdateClassName);
				if (mObject == null)
				{
					Debug.LogError("not valid class " + bindHotUpdateClassName);
					return;
				}
				if (mObject is LikeBehaviour)
					((LikeBehaviour)mObject).____Init(behaviour);
#else
#if UNITY_EDITOR
				HotUpdateManager.InitForEditor();
#endif
				type = HotUpdateManager.instance.GetScriptType(bindHotUpdateClassName.Replace("+","."), out content);
				if (type == null)
				{
					mType = HotUpdateManager.instance.GetTypeEx(bindHotUpdateClassName);
					if (mType == null)
					{
						Debug.LogError("not exist class " + bindHotUpdateClassName);
						return;
					}
					mObject = mType.Assembly.CreateInstance(bindHotUpdateClassName);
					if (mObject == null)
					{
						Debug.LogError("not valid class " + bindHotUpdateClassName);
						return;
					}
					if (mObject is LikeBehaviour)
						((LikeBehaviour)mObject).____Init(behaviour);
                    return;
				}

				inst = type.function.New(content, null).value as SInstance;
				content.CallType = inst.type;
				content.CallThis = inst;
				inst.content = content;

				SInstance parent = inst;
				while (parent != null && parent.parent != null)
					parent = parent.parent;
				if (parent.parentType == typeof(LikeBehaviour))
					((LikeBehaviour)parent.parentObj).____Init(behaviour);
#endif
			}
			public bool isHotUpdateScript()
            {
				return mType == null;
			}

			public void CheckMemberCall(Dictionary<string, bool> funtions)
			{
				if (mType == null)
				{
					foreach(var item in ((SType)type.function).functions)
					{
						funtions[item.Key] = true;
					}
				}
			}

			public object MemberCall(string funName, params object[] objects)
			{
				try
				{
					if (mType != null)
					{
						MethodInfo mi = mType.GetMethod(funName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
						if (mi != null)
                        {
							mType.InvokeMember(funName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, mObject, objects);
                        }
					}
					else
					{
						if (self.HasFunction(funName))
                        {
                            List<CSL_Content.Value> list;
                            if (objects != null && objects.Length > 0)
                            {
                                list = new List<CSL_Content.Value>();
                                foreach (object obj in objects)
                                    list.Add(new CSL_Content.Value(typeof(object), obj));
                            }
                            else
                                list = null;
                            CSL_Content.Value v = type.function.MemberCall(content, inst, funName, list);
                            if (v != null)
                                return v.value;
                        }
					}
				}
				catch(Exception e)
				{
                    Debug.LogError($"{funName} : {e}");
                }
				return null;
			}
        }
		#endregion //Private implementation
	}
}

