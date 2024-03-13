/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Tween the alpha of Graphic component
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    [AddComponentMenu("KissTween/Tween Alpha")]
    public class KissTweenAlpha : KissTweenBase
    {
        [Range(0f, 1f)]
        public float from = 1f;
        [Range(0f, 1f)]
        public float to = 1f;
        /// <summary>
        /// The current alpha
        /// </summary>
        public virtual float Value
        {
            get => cachedGraphic.color.a;
            set
            {
                Color c = cachedGraphic.color;
                c.a = value;
                cachedGraphic.color = c;
            }
        }
        protected override void OnUpdate(float factor)
        {
            Value = from * (1f - factor) + to * factor;
        }
        /// <summary>
        /// Manually start a KissTweenAlpha operation, if not exist create a new one.
        /// </summary>
        public static KissTweenAlpha Tween(GameObject go, float duration, float to)
        {
            KissTweenAlpha comp = Tween<KissTweenAlpha>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}