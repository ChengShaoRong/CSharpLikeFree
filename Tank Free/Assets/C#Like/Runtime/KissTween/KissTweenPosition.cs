/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Tween the local position of the gameobject
    /// </summary>
    [AddComponentMenu("KissTween/Tween Position")]
    public class KissTweenPosition : KissTweenBase
    {
        public Vector3 from = Vector3.zero;
        public Vector3 to = Vector3.zero;
        /// <summary>
        /// The current local position
        /// </summary>
        public virtual Vector3 Value
        {
            get => cachedTransform.localPosition;
            set => cachedTransform.localPosition = value;
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