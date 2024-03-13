/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Tween the scrollbar value
    /// </summary>
    [RequireComponent(typeof(Scrollbar))]
    [AddComponentMenu("KissTween/Tween Scrollbar")]
    public class KissTweenScrollbar : KissTweenBase
    {
        [Range(0f, 1f)]
        public float from = 0.5f;
        [Range(0f, 1f)]
        public float to = 1f;

        /// <summary>
        /// The current scrollbar value
        /// </summary>
        public float Value
        {
            get => cachedScrollbar.value;
            set => cachedScrollbar.value = value;
        }
        protected override void OnUpdate(float factor)
        {
            Value = from * (1f - factor) + to * factor;
        }
        /// <summary>
        /// Manually start a KissTweenScrollbar operation, if not exist create a new one.
        /// </summary>
        public static KissTweenScrollbar Tween(GameObject go, float duration, float to)
        {
            KissTweenScrollbar comp = Tween<KissTweenScrollbar>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}