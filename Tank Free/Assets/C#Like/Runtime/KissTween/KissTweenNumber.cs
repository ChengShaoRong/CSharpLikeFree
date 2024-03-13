/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Tween the integer value of the Text component 
    /// </summary>
    [RequireComponent(typeof(Text))]
    [AddComponentMenu("KissTween/Tween Number")]
    public class KissTweenNumber : KissTweenBase
    {
        public int from = 0;
        public int to = 100;
        [Tooltip("The prefix of the number")]
        public string prefix = "+";
        [Tooltip("The suffix of the number")]
        public string suffix = "";
        /// <summary>
        /// The current interger number
        /// </summary>
        public int Value
        {
            get
            {
                string str = cachedText.text.Trim();
                if (prefix.Length > 0 && str.StartsWith(prefix))
                    str = str.Substring(prefix.Length);
                if (suffix.Length > 0 && str.EndsWith(suffix))
                    str = str.Substring(0, str.Length - prefix.Length);
                str = str.Trim();
                if (str.Length == 0)
                    return 0;
                try
                {
                    return Convert.ToInt32(str);
                }
                catch
                {
                    return 0;
                }
            }
            set => cachedText.text = prefix + value.ToString() + suffix;
        }
        protected override void OnUpdate(float factor)
        {
            Value = Mathf.CeilToInt(from * (1f - factor) + to * factor);
        }
        /// <summary>
        /// Manually start a KissTweenNumber operation, if not exist create a new one.
        /// </summary>
        public static KissTweenNumber Tween(GameObject go, float duration, int to)
        {
            KissTweenNumber comp = Tween<KissTweenNumber>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}