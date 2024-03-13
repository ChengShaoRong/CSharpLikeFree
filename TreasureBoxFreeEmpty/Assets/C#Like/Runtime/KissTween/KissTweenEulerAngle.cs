/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Tween the local euler angle of the gameobject
    /// </summary>
    [AddComponentMenu("KissTween/Tween Euler Angle")]
    public class KissTweenEulerAngle : KissTweenBase
    {
        public Vector3 from = Vector3.zero;
        public Vector3 to = Vector3.zero;
        /// <summary>
        /// The current local euler angle
        /// </summary>
        public Vector3 Value
        {
            get => cachedTransform.localEulerAngles;
            set => cachedTransform.localEulerAngles = value;
        }
        protected override void OnUpdate(float factor)
        {
            Value = from * (1f - factor) + to * factor;
        }
        /// <summary>
        /// Manually start a KissTweenPosition operation, if not exist create a new one.
        /// </summary>
        static public KissTweenPosition Tween(GameObject go, float duration, Vector3 to)
        {
            KissTweenPosition comp = Tween<KissTweenPosition>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}