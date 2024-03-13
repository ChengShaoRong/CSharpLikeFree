/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Tween the local rotation of the gameobject
    /// </summary>
    [AddComponentMenu("KissTween/Tween Rotation")]
    public class KissTweenRotation : KissTweenBase
    {
        public Vector3 from = Vector3.zero;
        public Vector3 to = Vector3.zero;
        /// <summary>
        /// The current local rotation
        /// </summary>
        public Quaternion Value
        {
            get => cachedTransform.localRotation;
            set => cachedTransform.localRotation = value;
        }
        protected override void OnUpdate(float factor)
        {
            cachedTransform.localRotation = Quaternion.Slerp(Quaternion.Euler(from), Quaternion.Euler(to), factor);
        }
        /// <summary>
        /// Manually start a KissTweenRotation operation, if not exist create a new one.
        /// </summary>
        public static KissTweenRotation Tween(GameObject go, float duration, Quaternion to)
        {
            KissTweenRotation comp = Tween<KissTweenRotation>(go, duration);
            comp.from = comp.Value.eulerAngles;
            comp.to = to.eulerAngles;
            return comp;
        }
        /// <summary>
        /// Manually start a KissTweenRotation operation, if not exist create a new one.
        /// </summary>
        public static KissTweenRotation Tween(GameObject go, float duration, Vector3 to)
        {
            KissTweenRotation comp = Tween<KissTweenRotation>(go, duration);
            comp.from = comp.Value.eulerAngles;
            comp.to = to;
            return comp;
        }
    }

}