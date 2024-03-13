/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Tween local postion of the GameObject with specified local postions.
    /// You can move your GameObject with specified paths.
    /// </summary>
    [AddComponentMenu("KissTween/Tween Paths")]
    public class KissTweenPaths : KissTweenBase
    {
        [Tooltip("The local positions of the path, the first one for start position and the last one for end position. You MUST set 2+ positions.")]
        public List<Vector3> paths;
        /// <summary>
        /// The current local position
        /// </summary>
        public Vector3 Value
        {
            get
            {
                return cachedTransform.localPosition;
            }
            set
            {
                cachedTransform.localPosition = value;
            }
        }
        /// <summary>
        /// Manually start a KissTweenPaths operation, if not exist create a new one.
        /// </summary>
        public static KissTweenPaths Tween(GameObject go, float duration, List<Vector3> paths)
        {
            KissTweenPaths comp = Tween<KissTweenPaths>(go, duration);
            comp.paths = paths;
            if (paths.Count <= 1)
            {
                Debug.LogError("TweenPaths paths must cantains 2+ values, may be you should using component KissTweenPosition");
                comp.enabled = false;
                return comp;
            }
            return comp;
        }
        #region private impl
        protected override void OnUpdate(float factor)
        {
            int i = 0;
            float fStart = 0f;
            foreach (float _factor in factors)
            {
                if (factor <= _factor)
                {
                    factor = (factor - fStart) / (_factor - fStart);
                    Value = paths[i] * (1f - factor) + paths[i + 1] * factor;
                    return;
                }
                fStart = _factor;
                i++;
            }
        }
        List<float> factors;
        protected override void OnStart()
        {
            List<float> distances = new List<float>();
            int count = paths.Count;
            if (count <= 1)
            {
                Debug.LogError("KissTweenPaths paths must cantains 2+ values, may be you should using component KissTweenPosition");
                enabled = false;
                return;
            }
            float distanceTotal = 0f;
            for (int i = 0; i < count - 1; ++i)
            {
                distanceTotal += Vector3.Distance(paths[i], paths[i + 1]);
                distances.Add(distanceTotal);
            }
            factors = new List<float>();
            foreach (float distance in distances)
            {
                factors.Add(distance / distanceTotal);
            }
            factors[count - 2] = 1f;
        }
        #endregion
    }

}