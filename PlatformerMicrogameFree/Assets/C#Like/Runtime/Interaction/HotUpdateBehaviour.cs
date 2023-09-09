/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
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
		[Serializable]
        public
#if UNITY_EDITOR
			class
#else
			struct
#endif
			stStringValue<T>
		{
			public stStringValue(string key, T value)
            {
				this.key = key;
				this.value = value;
            }
			public string key;
			public T value;
        }
        public List<stStringValue<int>> integers = new List<stStringValue<int>>();
#if !UNITY_EDITOR
        Dictionary<string, int> mInts = new Dictionary<string, int>();
#endif
		/// <summary>
		/// get int value from this instance
		/// </summary>
        public int GetInt(string key, int iDefault = 0)
		{
#if UNITY_EDITOR
			foreach(var item in integers)
            {
				if (item.key == key)
					return item.value;
            }
#else
			int value;
			if (mInts.TryGetValue(key, out value))
				return value;
#endif
            return iDefault;
		}
        /// <summary>
        /// set int value to this instance
        /// </summary>
        public void SetInt(string key, int value)
        {
#if UNITY_EDITOR
			for(int i = integers.Count-1; i>=0; i--)
            {
                if (key == integers[i].key)
                {
					integers[i].value = value;
                    return;
                }
            }
			integers.Add(new stStringValue<int>(key, value));
#else
			mInts[key] = value;
#endif
        }
        public List<stStringValue<bool>> booleans = new List<stStringValue<bool>>();
#if !UNITY_EDITOR
        Dictionary<string, bool> mBooleans = new Dictionary<string, bool>();
#endif
        /// <summary>
        /// get bool value from this instance
        /// </summary>
        public bool GetBoolean(string key, bool bDefault = false)
        {
#if UNITY_EDITOR
            foreach (var item in booleans)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			bool value;
			if (mBooleans.TryGetValue(key, out value))
				return value;
#endif
			return bDefault;
        }
        /// <summary>
        /// get bool value to this instance
        /// </summary>
        public void SetBoolean(string key, bool value)
        {
#if UNITY_EDITOR
            for (int i = booleans.Count - 1; i >= 0; i--)
            {
                if (key == booleans[i].key)
                {
					booleans[i].value = value;
                    return;
                }
            }
			booleans.Add(new stStringValue<bool>(key, value));
#else
			mBooleans[key] = value;
#endif
        }
        public List<stStringValue<Sprite>> sprites = new List<stStringValue<Sprite>>();
#if !UNITY_EDITOR
        Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();
#endif
        /// <summary>
        /// get Sprite value from this instance
        /// </summary>
        public Sprite GetSprite(string key)
        {
#if UNITY_EDITOR
            foreach (var item in sprites)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			Sprite value;
			if (mSprites.TryGetValue(key, out value))
				return value;
#endif
            return null;
        }
        /// <summary>
        /// get Sprite value to this instance
        /// </summary>
        public void SetSprite(string key, Sprite value)
        {
#if UNITY_EDITOR
            for (int i = sprites.Count - 1; i >= 0; i--)
            {
                if (key == sprites[i].key)
                {
                    sprites[i].value = value;
                    return;
                }
            }
            sprites.Add(new stStringValue<Sprite>(key, value));
#else
			mSprites[key] = value;
#endif
        }
        public List<stStringValue<double>> doubles = new List<stStringValue<double>>();
#if !UNITY_EDITOR
        Dictionary<string, double> mDoubles = new Dictionary<string, double>();
#endif
        /// <summary>
        /// get double value from this instance
        /// </summary>
        public double GetDouble(string key, double lDefault = 0)
        {
#if UNITY_EDITOR
            foreach (var item in doubles)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			double value;
			if (mDoubles.TryGetValue(key, out value))
				return value;
#endif
			return lDefault;
        }
        /// <summary>
        /// set double value to this instance
        /// </summary>
        public void SetDouble(string key, double value)
        {
#if UNITY_EDITOR
            for (int i = doubles.Count - 1; i >= 0; i--)
            {
                if (key == doubles[i].key)
                {
					doubles[i].value = value;
                    return;
                }
            }
			doubles.Add(new stStringValue<double>(key, value));
#else
			mDoubles[key] = value;
#endif
		}
		public List<stStringValue<float>> floats = new List<stStringValue<float>>();
#if !UNITY_EDITOR
        Dictionary<string, float> mFloats = new Dictionary<string, float>();
#endif
        /// <summary>
        /// get float value from this instance
        /// </summary>
        public float GetFloat(string key, float fDefault = 0f)
        {
#if UNITY_EDITOR
            foreach (var item in floats)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			float value;
			if (mFloats.TryGetValue(key, out value))
				return value;
#endif
			return fDefault;
        }
        /// <summary>
        /// set float value to this instance
        /// </summary>
        public void SetFloat(string key, float value)
        {
#if UNITY_EDITOR
            for (int i = floats.Count - 1; i >= 0; i--)
            {
                if (key == floats[i].key)
                {
					floats[i].value = value;
                    return;
                }
            }
			floats.Add(new stStringValue<float>(key, value));
#else
			mFloats[key] = value;
#endif
        }
        public List<stStringValue<string>> strings = new List<stStringValue<string>>();
#if !UNITY_EDITOR
        Dictionary<string, string> mStrings = new Dictionary<string, string>();
#endif
        /// <summary>
        /// set string value from this instance
        /// </summary>
        public string GetString(string key, string strDefault = "")
        {
#if UNITY_EDITOR
            foreach (var item in strings)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			string value;
			if (mStrings.TryGetValue(key, out value))
				return value;
#endif
			return strDefault;
        }
        /// <summary>
        /// set string value to this instance
        /// </summary>
        public void SetString(string key, string value)
        {
#if UNITY_EDITOR
            for (int i = strings.Count - 1; i >= 0; i--)
            {
                if (key == strings[i].key)
                {
					strings[i].value = value;
                    return;
                }
            }
			strings.Add(new stStringValue<string>(key, value));
#else
			mStrings[key] = value;
#endif
        }
        public List<stStringValue<Color>> colors = new List<stStringValue<Color>>();
#if !UNITY_EDITOR
        Dictionary<string, Color> mColors = new Dictionary<string, Color>();
#endif
        /// <summary>
        /// set Color value from this instance
        /// </summary>
        public Color GetColor(string key)
        {
#if UNITY_EDITOR
            foreach (var item in colors)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			Color value;
			if (mColors.TryGetValue(key, out value))
				return value;
#endif
            return Color.white;
        }
        /// <summary>
        /// set Color value to this instance
        /// </summary>
        public void SetColor(string key, Color value)
        {
#if UNITY_EDITOR
            for (int i = colors.Count - 1; i >= 0; i--)
            {
                if (key == colors[i].key)
                {
                    colors[i].value = value;
                    return;
                }
            }
            colors.Add(new stStringValue<Color>(key, value));
#else
			mColors[key] = value;
#endif
        }
        public List<stStringValue<Vector3>> vector3s = new List<stStringValue<Vector3>>();
#if !UNITY_EDITOR
        Dictionary<string, Vector3> mVector3s = new Dictionary<string, Vector3>();
#endif
        /// <summary>
        /// set Vector3 value from this instance
        /// </summary>
        public Vector3 GetVector3(string key)
        {
#if UNITY_EDITOR
            foreach (var item in vector3s)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			Vector3 value;
			if (mVector3s.TryGetValue(key, out value))
				return value;
#endif
            return Vector3.zero;
        }
        /// <summary>
        /// set Vector3 value to this instance
        /// </summary>
        public void SetVector3(string key, Vector3 value)
        {
#if UNITY_EDITOR
            for (int i = vector3s.Count - 1; i >= 0; i--)
            {
                if (key == vector3s[i].key)
                {
                    vector3s[i].value = value;
                    return;
                }
            }
            vector3s.Add(new stStringValue<Vector3>(key, value));
#else
			mVector3s[key] = value;
#endif
        }
        public List<stStringValue<GameObject>> gameObjects = new List<stStringValue<GameObject>>();
#if !UNITY_EDITOR
        Dictionary<string, GameObject> mGameObjects = new Dictionary<string, GameObject>();
#endif
        /// <summary>
        /// get GameObject instance which bind to this prefab
        /// </summary>
        public GameObject GetGameObject(string key)
        {
#if UNITY_EDITOR
            foreach (var item in gameObjects)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			GameObject value;
			if (mGameObjects.TryGetValue(key, out value))
				return value;
#endif
			return null;
        }
        /// <summary>
        /// set GameObject instance to this prefab
        /// </summary>
        public void SetGameObject(string key, GameObject value)
        {
#if UNITY_EDITOR
            for (int i = gameObjects.Count - 1; i >= 0; i--)
            {
                if (key == gameObjects[i].key)
                {
					gameObjects[i].value = value;
                    return;
                }
            }
			gameObjects.Add(new stStringValue<GameObject>(key, value));
#else
			mGameObjects[key] = value;
#endif
		}
		public List<stStringValue<TextAsset>> textAssets = new List<stStringValue<TextAsset>>();
#if !UNITY_EDITOR
        Dictionary<string, TextAsset> mTextAssets = new Dictionary<string, TextAsset>();
#endif
        /// <summary>
        /// get Text asset which bind to this prefab
        /// </summary>
        public TextAsset GetTextAsset(string key)
        {
#if UNITY_EDITOR
            foreach (var item in textAssets)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			TextAsset value;
			if (mTextAssets.TryGetValue(key, out value))
				return value;
#endif
			return null;
        }
		/// <summary>
		/// set Text asset to this prefab
		/// </summary>
		public void SetTextAsset(string key, TextAsset value)
        {
#if UNITY_EDITOR
            for (int i = textAssets.Count - 1; i >= 0; i--)
            {
                if (key == textAssets[i].key)
                {
					textAssets[i].value = value;
                    return;
                }
            }
			textAssets.Add(new stStringValue<TextAsset>(key, value));
#else
			mTextAssets[key] = value;
#endif
        }
        public List<stStringValue<Material>> materials = new List<stStringValue<Material>>();
#if !UNITY_EDITOR
        Dictionary<string, Material> mMaterials = new Dictionary<string, Material>();
#endif
		/// <summary>
		/// get Material which bind to this prefab
		/// </summary>
		public Material GetMaterial(string key)
        {
#if UNITY_EDITOR
            foreach (var item in materials)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			Material value;
			if (mMaterials.TryGetValue(key, out value))
				return value;
#endif
			return null;
        }
		/// <summary>
		/// set Material to this prefab
		/// </summary>
		public void SetMaterial(string key, Material value)
        {
#if UNITY_EDITOR
            for (int i = materials.Count - 1; i >= 0; i--)
            {
                if (key == materials[i].key)
                {
					materials[i].value = value;
                    return;
                }
            }
			materials.Add(new stStringValue<Material>(key, value));
#else
			mMaterials[key] = value;
#endif
        }
        public List<stStringValue<Texture>> textures = new List<stStringValue<Texture>>();
#if !UNITY_EDITOR
        Dictionary<string, Texture> mTextures = new Dictionary<string, Texture>();
#endif
		/// <summary>
		/// get Texture which bind to this prefab
		/// </summary>
		public Texture GetTexture(string key)
        {
#if UNITY_EDITOR
            foreach (var item in textures)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			Texture value;
			if (mTextures.TryGetValue(key, out value))
				return value;
#endif
			return null;
        }
		/// <summary>
		/// set Texture to this prefab
		/// </summary>
		public void SetTexture(string key, Texture value)
        {
#if UNITY_EDITOR
            for (int i = textures.Count - 1; i >= 0; i--)
            {
                if (key == textures[i].key)
                {
					textures[i].value = value;
                    return;
                }
            }
			textures.Add(new stStringValue<Texture>(key, value));
#else
			mTextures[key] = value;
#endif
        }
        public List<stStringValue<AudioClip>> audioClips = new List<stStringValue<AudioClip>>();
#if !UNITY_EDITOR
        Dictionary<string, AudioClip> mAudioClips = new Dictionary<string, AudioClip>();
#endif
		/// <summary>
		/// get AudioClip which bind to this prefab
		/// </summary>
		public AudioClip GetAudioClip(string key)
        {
#if UNITY_EDITOR
            foreach (var item in audioClips)
            {
                if (item.key == key)
                    return item.value;
            }
#else
			AudioClip value;
			if (mAudioClips.TryGetValue(key, out value))
				return value;
#endif
			return null;
        }
		/// <summary>
		/// set AudioClip to this prefab
		/// </summary>
		public void SetAudioClip(string key, AudioClip value)
        {
#if UNITY_EDITOR
            for (int i = audioClips.Count - 1; i >= 0; i--)
            {
                if (key == audioClips[i].key)
                {
					audioClips[i].value = value;
                    return;
                }
            }
			audioClips.Add(new stStringValue<AudioClip>(key, value));
#else
			mAudioClips[key] = value;
#endif
		}

		[Tooltip("This component will bind to hot update class,and you must input the full name of that class with the namespace if it have.")]
		public string bindHotUpdateClassFullName;
		
		public bool enableClick
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

		protected Helper helper = null;
#region Unity event
		void Start()
        {
			InitHelper();
			helper.MemberCall("Start");
        }
		void InitHelper()
        {
            if (helper == null)
            {
                helper = new Helper(bindHotUpdateClassFullName, this);
                InitFunctions();
#if !UNITY_EDITOR
				if (integers.Count > 0)
                {
					foreach (var item in integers)
						mInts[item.key] = item.value;
                }
                if (booleans.Count > 0)
                {
                    foreach (var item in booleans)
						mBooleans[item.key] = item.value;
                }
                if (strings.Count > 0)
                {
                    foreach (var item in strings)
						mStrings[item.key] = item.value;
                }
                if (colors.Count > 0)
                {
                    foreach (var item in colors)
						mColors[item.key] = item.value;
                }
                if (vector3s.Count > 0)
                {
                    foreach (var item in vector3s)
						mVector3s[item.key] = item.value;
                }
                if (floats.Count > 0)
                {
                    foreach (var item in floats)
						mFloats[item.key] = item.value;
                }
                if (doubles.Count > 0)
                {
                    foreach (var item in doubles)
						mDoubles[item.key] = item.value;
                }
                if (textAssets.Count > 0)
                {
                    foreach (var item in textAssets)
						mTextAssets[item.key] = item.value;
                }
                if (gameObjects.Count > 0)
                {
                    foreach (var item in gameObjects)
						mGameObjects[item.key] = item.value;
                }
                if (materials.Count > 0)
                {
                    foreach (var item in materials)
						mMaterials[item.key] = item.value;
                }
                if (textures.Count > 0)
                {
                    foreach (var item in textures)
						mTextures[item.key] = item.value;
                }
                if (audioClips.Count > 0)
                {
                    foreach (var item in audioClips)
						mAudioClips[item.key] = item.value;
                }
                if (sprites.Count > 0)
                {
                    foreach (var item in sprites)
						mSprites[item.key] = item.value;
                }
#endif
            }
        }

        void Awake()
        {
            InitHelper();
            helper.MemberCall("Awake");
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
        /// <param name="funName">method name,if null or empty then default as 'OnClick'</param>
        public void OnClick(string funName)
        {
            funName = funName.Trim();
            if (string.IsNullOrEmpty(funName))
                helper.MemberCall("OnClick");
            else if (funName.EndsWith(")"))
            {
                int index = funName.IndexOf("(");
                if (index > 0)
                {
                    string strFuncName = funName.Substring(0, index);
                    string strParams = funName.Substring(index + 1);
                    if (strParams.Length <= 1)
                        helper.MemberCall(strFuncName);
                    else
                    {
                        string[] strs = strParams.Substring(0, strParams.Length - 1).Trim().Split(',');
                        object[] objs = new object[strs.Length];
                        for (int i = strs.Length - 1; i >= 0; i--)
                        {
                            string str = strs[i].Trim();
                            if (str.StartsWith("\""))
                                objs[i] = (str.Length > 2) ? str.Substring(1, str.Length - 2) : "";
                            else if (str.StartsWith("'"))
                                objs[i] = Convert.ToChar((str.Length > 2) ? str.Substring(1, str.Length - 2) : "");
                            else
                            {
                                str = str.ToLower();
                                if (str.EndsWith("f"))
                                    objs[i] = Convert.ToSingle(str, System.Globalization.CultureInfo.InvariantCulture);
                                else if (str.EndsWith("d"))
                                    objs[i] = Convert.ToDouble(str, System.Globalization.CultureInfo.InvariantCulture);
                                else if (str.EndsWith("ul"))
                                    objs[i] = Convert.ToUInt64(str);
                                else if (str.EndsWith("l"))
                                    objs[i] = Convert.ToInt64(str);
                                else if (str.EndsWith("u"))
                                    objs[i] = Convert.ToInt32(str);
                                else if (str == "true")
                                    objs[i] = true;
                                else if (str == "false")
                                    objs[i] = false;
                                else if (str.Contains("."))
                                    objs[i] = Convert.ToSingle(str, System.Globalization.CultureInfo.InvariantCulture);
                                else
                                    objs[i] = Convert.ToInt32(str);
                            }
                        }
                        helper.MemberCall(strFuncName, objs);
                    }
                }
                else
                    Debug.LogError($"{gameObject.name} execute function '{funName}' error");
            }
            else
                helper.MemberCall(funName);
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
        /// HTTP get request.
        /// It's for the FREE version quick use the HTTP get request because the FREE version not support coroutine.
        /// The full version can direct using UnityEngine.Networking.UnityWebRequest.
        /// </summary>
        /// <param name="url">the http url</param>
        /// <param name="callback">the http callback(string callbackText, string error)</param>
        /// <param name="timeout">the timeout of the web request, default is 10 seconds</param>
        public void HttpGet(string url, Action<string, string> callback = null, int timeout = 10)
        {
            base.StartCoroutine(colHttpRequest(url, null, callback, timeout));
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
            base.StartCoroutine(colHttpRequest(url, postData, callback, timeout));
        }
        IEnumerator colHttpRequest(string url, string postData, Action<string, string> callback, int timeout)
        {
            using (UnityEngine.Networking.UnityWebRequest uwr = (string.IsNullOrEmpty(postData) ? UnityEngine.Networking.UnityWebRequest.Get(url) : UnityEngine.Networking.UnityWebRequest.Post(url, postData)))
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
                if (one.bindHotUpdateClassFullName == bindHotUpdateClassFullName)
                    return one.ScriptInstance;
            }
            return null;
        }
        public object[] GetComponentsEx(string bindHotUpdateClassFullName)
        {
            List<object> results = new List<object>();
            foreach (HotUpdateBehaviour one in gameObject.GetComponents<HotUpdateBehaviour>())
            {
                if (one.bindHotUpdateClassFullName == bindHotUpdateClassFullName)
                    results.Add(one.ScriptInstance);
            }
            return results.ToArray();
        }
        public object GetComponentInChildrenEx(string bindHotUpdateClassFullName)
        {
            foreach (HotUpdateBehaviour one in gameObject.GetComponentsInChildren<HotUpdateBehaviour>())
            {
                if (one.bindHotUpdateClassFullName == bindHotUpdateClassFullName)
                    return one.ScriptInstance;
            }
            return null;
        }
        public object[] GetComponentsInChildrenEx(string bindHotUpdateClassFullName)
        {
            List<object> results = new List<object>();
            foreach (HotUpdateBehaviour one in gameObject.GetComponentsInChildren<HotUpdateBehaviour>())
            {
                if (one.bindHotUpdateClassFullName == bindHotUpdateClassFullName)
                    results.Add(one.ScriptInstance);
            }
            return results.ToArray();
        }
        public object GetComponentInParentEx(string bindHotUpdateClassFullName)
        {
            foreach (HotUpdateBehaviour one in gameObject.GetComponentsInParent<HotUpdateBehaviour>())
            {
                if (one.bindHotUpdateClassFullName == bindHotUpdateClassFullName)
                    return one.ScriptInstance;
            }
            return null;
        }
        public object[] GetComponentsInParentEx(string bindHotUpdateClassFullName)
        {
            List<object> results = new List<object>();
            foreach (HotUpdateBehaviour one in gameObject.GetComponentsInParent<HotUpdateBehaviour>())
            {
                if (one.bindHotUpdateClassFullName == bindHotUpdateClassFullName)
                    results.Add(one.ScriptInstance);
            }
            return results.ToArray();
        }
        static object GetComponentByFullName(GameObject go, string name)
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
                if (one != null && one.bindHotUpdateClassFullName == fullName)
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
                type = HotUpdateManager.instance.GetScriptType(bindHotUpdateClassName, out content);
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

