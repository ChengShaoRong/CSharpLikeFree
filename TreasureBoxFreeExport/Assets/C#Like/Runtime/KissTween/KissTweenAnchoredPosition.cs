/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Tween the AnchoredPosition of the gameobject with RectTransform component
    /// </summary>
    [AddComponentMenu("KissTween/Tween AnchoredPosition")]
    [RequireComponent(typeof(RectTransform))]
    public class KissTweenAnchoredPosition : KissTweenBase
    {
        public Vector2 from = Vector2.zero;
        public Vector2 to = Vector2.zero;

        RectTransform mRectTrans;
        /// <summary>
        /// Cached RectTransform
        /// </summary>
        protected RectTransform cachedRectTransform { get { if (mRectTrans == null) mRectTrans = transform as RectTransform; return mRectTrans; } }
        /// <summary>
        /// The current local position
        /// </summary>
        public virtual Vector2 Value
        {
            get => cachedRectTransform.anchoredPosition;
            set => cachedRectTransform.anchoredPosition = value;
        }
        protected override void OnUpdate(float factor)
        {
            Value = from * (1f - factor) + to * factor;
        }
        /// <summary>
        /// Manually start a KissTweenAnchoredPosition operation, if not exist create a new one.
        /// </summary>
        public static KissTweenAnchoredPosition Tween(GameObject go, float duration, Vector2 to)
        {
            KissTweenAnchoredPosition comp = Tween<KissTweenAnchoredPosition>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}