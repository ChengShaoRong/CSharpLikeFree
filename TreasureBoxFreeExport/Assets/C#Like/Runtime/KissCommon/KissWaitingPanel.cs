/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CSharpLike
{
	public class KissWaitingPanel : MonoBehaviour
	{
        static KissWaitingPanel curInstance = null;
        IEnumerator CorTimeout(float fTimeout, UnityAction actionTimeout, bool realTime)
        {
            if (realTime)
                yield return new WaitForSecondsRealtime(fTimeout);
            else
                yield return new WaitForSeconds(fTimeout);
            actionTimeout.Invoke();
            Hide();
        }
        public static void Hide()
        {
            if (curInstance != null && curInstance.gameObject != null)
            {
                KissTweenAlpha kta = curInstance.gameObject.GetComponent<KissTweenAlpha>();
                kta.onFinished += (tween) => { GameObject.Destroy(kta.gameObject); };
                kta.PlayReverse();
            }
            curInstance = null;
        }

        public static void Show(float fTimeout = -1f, UnityAction actionTimeout = null, bool realTime = true)
        {
            if (curInstance == null)
            {
                string prefabName = "KissWaitingPanel";
                GameObject goMainRoot = GameObject.Find("MainRoot");
                if (goMainRoot == null)
                {
                    Debug.LogError("Not exist GameObject name 'MainRoot' for place KissWaitingPanel");
                    return;
                }
                GameObject go = Resources.Load(prefabName) as GameObject;
                if (go == null)
                {
                    Debug.LogError($"Not exist '{prefabName}.prefab'");
                    return;
                }
                go = Instantiate(go, new Vector3(0f, 0f, 0f), Quaternion.identity, goMainRoot.transform);
                if (go == null)
                {
                    Debug.LogError($"Instantiate '{prefabName}.prefab' error");
                    return;
                }
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                curInstance = go.GetComponent<KissWaitingPanel>();
                if (curInstance == null)
                {
                    Debug.LogError($"Not exist 'KissWaitingPanel' component in '{prefabName}.prefab'");
                    Destroy(go);
                    return;
                }
            }
            curInstance.StopAllCoroutines();
            if (fTimeout > 0f && actionTimeout != null)
                curInstance.StartCoroutine(curInstance.CorTimeout(fTimeout, actionTimeout, realTime));
        }
    }
}

