/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Tween the local scale of the gameobject
    /// </summary>
    [AddComponentMenu("KissTween/Tween Scale")]
    public class KissTweenScale : KissTweenBase
    {
        public Vector3 from = Vector3.one;
        public Vector3 to = Vector3.one;
        /// <summary>
        /// The current local scale
        /// </summary>
        public Vector3 Value
        {
            get => cachedTransform.localScale;
            set => cachedTransform.localScale = value;
        }
        protected override void OnUpdate(float factor)
        {
            Value = from * (1f - factor) + to * factor;
        }
        /// <summary>
        /// Manually start a KissTweenPosition operation, if not exist create a new one.
        /// </summary>
        public static KissTweenPosition Tween(GameObject go, float duration, Vector3 to)
        {
            KissTweenPosition comp = Tween<KissTweenPosition>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}