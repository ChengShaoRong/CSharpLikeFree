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
    }
}

