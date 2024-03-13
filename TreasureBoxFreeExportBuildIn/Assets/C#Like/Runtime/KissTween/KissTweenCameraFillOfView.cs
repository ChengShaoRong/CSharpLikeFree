/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Tween the camera's field of view
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("KissTween/Tween Camera Fill Of View")]
    public class KissTweenCameraFillOfView : KissTweenBase
    {
        public float from = 40f;
        public float to = 40f;

        /// <summary>
        /// The current camera's field of view
        /// </summary>
        public float Value
        {
            get => cachedCamera.fieldOfView;
            set => cachedCamera.fieldOfView = value;
        }
        protected override void OnUpdate(float factor)
        {
            Value = from * (1f - factor) + to * factor;
        }
        /// <summary>
        /// Manually start a KissTweenCameraFillOfView operation, if not exist create a new one.
        /// </summary>
        public static KissTweenCameraFillOfView Tween(GameObject go, float duration, float to)
        {
            KissTweenCameraFillOfView comp = Tween<KissTweenCameraFillOfView>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}