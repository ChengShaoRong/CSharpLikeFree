/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    /// <summary>
    /// Make RectTransform size same with Canvas in parent.
    /// That using to scale the background image.
    /// </summary>
    public class KissSizeScaler : MonoBehaviour
	{
		public bool runOnlyOnce = true;

        #region internal impl
        RectTransform mTransParent;
        RectTransform mTrans;
        IEnumerator Start()
        {
            mTrans = transform as RectTransform;
            int tryCount = 20;
            while(--tryCount > 0)
            {
                Canvas canvas = gameObject.GetComponentInParent<Canvas>();
                if (canvas != null)
                {
                    mTransParent = canvas.GetComponent<RectTransform>();
                    SetPosition();
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
            Debug.LogError($"Not exist Canvas component in parent in GameObject '{name}'");
            enabled = false;
        }
        void LateUpdate()
        {
#if !UNITY_EDITOR
            if (!runOnlyOnce)
#endif
            {
                SetPosition();
            }
        }
        void SetPosition()
        {
#if UNITY_EDITOR
            if (mTrans == null || mTransParent == null)
                return;
#endif
            if (mTrans.sizeDelta != mTransParent.sizeDelta)
                mTrans.sizeDelta = mTransParent.sizeDelta;
        }
#if UNITY_EDITOR
        void OnValidate()
        {
            mTrans = transform as RectTransform;
            Canvas canvas = gameObject.GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                mTransParent = canvas.GetComponent<RectTransform>();
                SetPosition();
            }
            else
                Debug.LogError($"Not exist Canvas component in parent in GameObject '{name}'");
        }
#endif
        #endregion //internal impl
    }
}

