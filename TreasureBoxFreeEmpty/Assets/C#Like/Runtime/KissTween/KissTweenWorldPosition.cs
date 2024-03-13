/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Tween the world position of the gameobject
    /// </summary>
    [AddComponentMenu("KissTween/Tween World Position")]
    public class KissTweenWorldPosition : KissTweenPosition
    {
        /// <summary>
        /// The current world position
        /// </summary>
        public override Vector3 Value
        {
            get => cachedTransform.position;
            set => cachedTransform.position = value;
        }
        /// <summary>
        /// Manually start a KissTweenWorldPosition operation, if not exist create a new one.
        /// </summary>
        public static new KissTweenWorldPosition Tween(GameObject go, float duration, Vector3 to)
        {
            KissTweenWorldPosition comp = Tween<KissTweenWorldPosition>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}