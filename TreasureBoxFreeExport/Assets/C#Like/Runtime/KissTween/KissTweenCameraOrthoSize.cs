/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Tween the camera's orthographic size.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("KissTween/Tween Camera Ortho Size")]
    public class KissTweenCameraOrthoSize : KissTweenBase
    {
        public float from = 1f;
        public float to = 1f;

        /// <summary>
        /// The current camera's orthographic size
        /// </summary>
        public float Value
        {
            get => cachedCamera.orthographicSize;
            set => cachedCamera.orthographicSize = value;
        }
        protected override void OnUpdate(float factor)
        {
            Value = from * (1f - factor) + to * factor;
        }
        /// <summary>
        /// Manually start a KissTweenCameraOrthoSize operation, if not exist create a new one.
        /// </summary>
        public static KissTweenCameraOrthoSize Tween(GameObject go, float duration, float to)
        {
            KissTweenCameraOrthoSize comp = Tween<KissTweenCameraOrthoSize>(go, duration);
            comp.from = comp.Value;
            comp.to = to;
            return comp;
        }
    }

}