//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TreasureBox
{
    /// <summary>
    /// PlayerPrefs using a prefix string to distinguish it from your another hot update game.
    /// </summary>
	public class MyPlayerPrefs
    {
        //#region Config
        /// <summary>
        /// This Game save into PlayerPrefs with a prefix,
        /// to distinguish it from your another hot update game.
        /// Recommend your game short name with '_'.
        /// </summary>
        static string mPrefix = "TB_";
        //#endregion //Config
        public static int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(mPrefix + key, defaultValue);
        }
        public static string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(mPrefix + key, defaultValue);
        }
        public static float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(mPrefix + key, defaultValue);
        }
        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(mPrefix + key, value);
        }
        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(mPrefix + key, value);
        }
        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(mPrefix + key, value);
        }
        public static void Save()
        {
            PlayerPrefs.Save();
        }
        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(mPrefix + key);
        }
        public static void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(mPrefix + key);
        }
        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}

