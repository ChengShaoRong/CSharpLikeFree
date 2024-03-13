/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Tween the color of Graphic component
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("KissTween/Tween Color")]
    public class KissTweenColor : KissTweenBase
    {
        public Color from = Color.white;
        public Color to = Color.white;
        /// <summary>
        /// The current color of graphic component
        /// </summary>
        public Color Value
        {
            get => cachedGraphic.color;
            set => cachedGraphic.color = value;
        }
        protected override void OnUpdate(float factor)
        {
            Value = Color.Lerp(from, to, factor);
        }
        /// <summary>
        /// Manually start a KissTweenColor operation, if not exist create a new one.
        /// </summary>
        public static KissTweenColor Tween(GameObject go, float duration, Color to)
        {
            KissTweenColor comp = Tween<KissTweenColor>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}