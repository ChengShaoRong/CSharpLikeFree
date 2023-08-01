/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    public class SampleHowToUseModifier
    {
        #region Modifier ref
        public static Vector3 currentVelocity = Vector3.zero;
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, float smoothTime)
        {
            return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime);
        }
        public static float currentVelocityFloat = 0f;
        public static float SmoothDamp(float current, float target, float smoothTime)
        {
            return Mathf.SmoothDamp(current, target, ref currentVelocityFloat, smoothTime);
        }
        #endregion
        #region Modifier out
        public static int value = 0;
        public static bool TryGetValue(Dictionary<string, int> dics, string key)
        {
            return dics.TryGetValue(key, out value);
        }
        #endregion
        #region Modifier in
        public static void TestModifierIn(string str, int iValue)
        {
            ModifierIn(str,in iValue);
        }
        public static void ModifierIn(string str, in int iValue)
        {
            Debug.Log("ModifierIn:"+str + iValue);
        }
        #endregion
        #region Modifier params
        public static void TestModifierParams(string str)
        {
            ModifierParams(str);
        }
        public static void TestModifierParams(string str, object v1)
        {
            ModifierParams(str, v1);
        }
        public static void TestModifierParams(string str, object v1, object v2)
        {
            ModifierParams(str, v1, v2);
        }
        public static void TestModifierParams(string str, object v1, object v2, object v3)
        {
            ModifierParams(str, v1, v2, v3);
        }
        public static void ModifierParams(string str, params object[] values)
        {
            string strTemp = "ModifierParams:" + str;
            foreach (var value in values)
                strTemp += "," + value;
            Debug.Log(strTemp);
        }
        #endregion
    }
    public class TestJsonNotDataSub2
    {
        public int id;
        public string name;
        public List<string> info;
        public Dictionary<string, int> maps;
    }
    public class TestJsonNotData2
    {
        public string str;
        public int i;
        public List<int> k;
        public Dictionary<string, TestJsonNotDataSub2> datas;
        TestJsonNotDataSub2 data;
    }

    public enum TestNotHotUpdateEnum
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }
    public class TestClassWithEnum
    {
        public enum TestEnumInClass
        {
            Abc,
            Defg,
            Xyz
        }
    }
}