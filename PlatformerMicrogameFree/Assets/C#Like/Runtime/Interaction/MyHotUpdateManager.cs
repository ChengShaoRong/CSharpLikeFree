/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using UnityEngine;
using System;
#if UNITY_EDITOR
using System.IO;
#endif

namespace CSharpLike
{
    [HelpURL("https://www.csharplike.com/MyHotUpdateManager.html")]
    public class MyHotUpdateManager : HotUpdateManager
	{
		public override Type GetTypeEx(string typeName)
		{
			return Type.GetType(typeName);
		}
		/// <summary>
		/// initialize the hot update framework by script if you don't want to initialize by a prefab
		/// </summary>
		public static void Init()
		{
			if (instance != null)
			{
				Destroy(instance.gameObject);
				instance = null;
			}
			instance = (new GameObject("MyHotUpdateManager")).AddComponent<MyHotUpdateManager>();
#if _CSHARP_LIKE_ && UNITY_EDITOR
			string strFileName = Application.dataPath + "/C#Like/output/code.bytes";
			if (File.Exists(strFileName))
			{
				Debug.Log($"Try load {strFileName} in editor, success = {Load(File.ReadAllBytes(strFileName))}");
			}
#endif
		}
	}
}

