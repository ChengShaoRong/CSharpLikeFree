/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using CSharpLike.Internal;
using System;
using System.Globalization;
using UnityEngine;

namespace CSharpLike
{

    [HelpURL("https://www.csharplike.com/MyCustomConfig.html")]
    public class MyCustomConfig
    {
        /// <summary>
        /// Culture info for Convert.ToSingle() and Convert.ToDouble().
        /// Because value(float/double) with the default separator ("."), but some country using (",").
        /// </summary>
        public static CultureInfo cultureInfoForConvertSingleAndDouble
        {
            get
            {
                return CultureInfo.InvariantCulture;
            }
        }
        static void RegisterType(Type type, string keyword)
        {
            HotUpdateManager.vm.RegType(RegHelper_Type.MakeType(type, keyword));
        }
        public static void RegisterEnv()
        {
        }
    }
}

