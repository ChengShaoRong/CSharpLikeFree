/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Base class for hot update script,just look like MonoBehaviour.
    /// You should inherit this class if you want to interactive data with unity.
    /// Don't try override the attribute or method in Subclass
    /// </summary>
    public class LikeBehaviour
    {
        [KissJsonDontSerialize]
        public HotUpdateBehaviour behaviour { get; private set; }
        [KissJsonDontSerialize]
        public GameObject gameObject { get; private set; }
        [KissJsonDontSerialize]
        public Transform transform { get; private set; }
        [KissJsonDontSerialize]
        public bool enabled { get { return behaviour != null && behaviour.enabled; } set { if (behaviour) behaviour.enabled = value; } }
        [KissJsonDontSerialize]
        public bool isActiveAndEnabled { get { return behaviour != null && behaviour.isActiveAndEnabled; } }
        [Obsolete("Not supported 'coroutine' in FREE version (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256", true)]
        public Coroutine StartCoroutine(string methodName, params object[] vars)
        {
            return null;
        }
        [Obsolete("Not supported 'coroutine' in FREE version (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256", true)]
        public void StopCoroutine(string methodName)
        {
        }
        [Obsolete("Not supported 'coroutine' in FREE version (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256", true)]
        public void StopCoroutine(Coroutine routine)
        {
        }
        [Obsolete("Not supported 'coroutine' in FREE version (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256", true)]
        public void StopAllCoroutines()
        {
        }
        /// <summary>
        /// call member method with no param, You may direct call 'funName();' instead of 'MemberCall(funName);'.
        /// </summary>
        public object MemberCall(string funName)
        {
            return behaviour.MemberCall(funName);
        }
        /// <summary>
        /// call member method with 1 param, You may direct call 'funName(v1);' instead of 'MemberCall(funName, v1);'.
        /// </summary>
        public object MemberCall(string funName, object v1)
        {
            return behaviour.MemberCall(funName, v1);
        }
        /// <summary>
        /// call member method with 2 params, You may direct call 'funName(v1, v2);' instead of 'MemberCall(funName, v1, v2);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2)
        {
            return behaviour.MemberCall(funName, v1, v2);
        }
        /// <summary>
        /// call member method with 3 params, You may direct call 'funName(v1, v2, v3);' instead of 'MemberCall(funName, v1, v2, v3);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3)
        {
            return behaviour.MemberCall(funName, v1, v2, v3);
        }
        /// <summary>
        /// call member method with 4 params, You may direct call 'funName(v1, v2, v3, v4);' instead of 'MemberCall(funName, v1, v2, v3, v4);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4);
        }
        /// <summary>
        /// call member method with 5 params, You may direct call 'funName(v1, v2, v3, v4, v5);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5);
        }
        /// <summary>
        /// call member method with 6 params, You may direct call 'funName(v1, v2, v3, v4, v5, v6);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6);
        }
        /// <summary>
        /// call member method with 7 params, You may direct call 'funName(v1, v2, v3, v4, v5, v6, v7);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6, v7);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7);
        }
        /// <summary>
        /// call member method with 8 params, You may direct call 'funName(v1, v2, v3, v4, v5, v6, v7, v8);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8);
        }
        /// <summary>
        /// call member method with 9 params, You may direct call 'funName(v1, v2, v3, v4, v5, v6, v7, v8, v9);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9);
        }
        /// <summary>
        /// call member method with 10 params, You may direct call 'funName(v1, v2, v3, v4, v5, v6, v7, v8, v9, v10);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10);
        }
        /// <summary>
        /// call member method with 11 params, You may direct call 'funName(v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11);
        }
        /// <summary>
        /// call member method with 12 params, You may direct call 'funName(v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11, object v12)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12);
        }
        /// <summary>
        /// call member method with 13 params, You may direct call 'funName(v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11, object v12, object v13)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13);
        }
        /// <summary>
        /// call member method with 14 params, You may direct call 'funName(v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11, object v12, object v13, object v14)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14);
        }
        /// <summary>
        /// call member method with 15 params, You may direct call 'funName(v1, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14, v15);' instead of 'MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14, v15);'.
        /// </summary>
        public object MemberCall(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, object v11, object v12, object v13, object v14, object v15)
        {
            return behaviour.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12, v13, v14, v15);
        }
        public void MemberCallDelay(string funName, float delay = 0.0f)
        {
			behaviour.MemberCallDelay(funName, delay);
		}
		public void MemberCallDelay(string funName, object v1, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, delay);
        }
		public void MemberCallDelay(string funName, object v1, object v2, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, v2, delay);
        }
		public void MemberCallDelay(string funName, object v1, object v2, object v3, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, v2, v3, delay);
        }
		public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, v2, v3, v4, delay);
        }
		public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, v2, v3, v4, v5, delay);
        }
		public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, v2, v3, v4, v5, v6, delay);
        }
		public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, v2, v3, v4, v5, v6, v7, delay);
        }
		public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, v2, v3, v4, v5, v6, v7, v8, delay);
        }
		public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, delay);
        }
		public void MemberCallDelay(string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, float delay)
        {
            behaviour.MemberCallDelay(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, delay);
        }
        /// <summary>
        /// for private call only,don't call it.
        /// </summary>
        internal void ____Init(HotUpdateBehaviour _behaviour)
        {
            behaviour = _behaviour;
            gameObject = _behaviour.gameObject;
            transform = _behaviour.transform;
        }
        public sealed override string ToString()
        {
            return behaviour.ToString();
        }
        public static implicit operator bool(LikeBehaviour likeBehaviour)
        {
            return likeBehaviour != null;
        }
    }
}