//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike;
using UnityEngine;
using UnityEngine.Events;

namespace TreasureBox
{
    /// <summary>
    /// Customize WaitingPanel in hot update script.
    /// </summary>
	public class WaitingPanel : LikeBehaviour
    {
        float mTimeout = -1f;
        bool mRealTime = false;
        UnityAction mActionTimeout;
        void OnUpdate()
        {
            if (mTimeout > 0f)
            {
                float deltaTime = mRealTime ? Time.unscaledDeltaTime : Time.deltaTime;
                mTimeout -= deltaTime;
                if (mTimeout < 0)
                {
                    mActionTimeout.Invoke();
                    Hide();
                }
            }
        }
        //Not support Coroutine in FREE version, using 'OnUpdate' instead
        //IEnumerator CorTimeout(float fTimeout, UnityAction actionTimeout, bool realTime)
        //{
        //    if (realTime)
        //        yield return new WaitForSecondsRealtime(fTimeout);
        //    else
        //        yield return new WaitForSeconds(fTimeout);
        //    actionTimeout.Invoke();
        //    Hide();
        //}

        static WaitingPanel curInstance = null;
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
        public static void Show(float fTimeout, UnityAction actionTimeout, bool realTime)
        //Free version not support default param
        //public static void Show(float fTimeout = -1f, UnityAction actionTimeout = null, bool realTime = true)
        {
            string prefabName = Global.GetPrefabFullName("WaitingPanel");
            var req = ResourceManager.LoadAssetAsync<GameObject>(prefabName);
            req.OnCompleted +=
                (go) =>
                {
                    GameObject goMainRoot = GameObject.Find("MainRoot");
                    if (goMainRoot == null)
                    {
                        Debug.LogError("Not exist 'MainRoot'");
                        return;
                    }
                    if (curInstance != null && curInstance.gameObject != null)
                    {
                        Debug.LogWarning("Exist WaitingPanel, no need show again");
                        return;
                    }
                    go = GameObject.Instantiate(go, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    if (go == null)
                    {
                        Debug.LogError("Instantiate '" + prefabName + "' error");
                        //Debug.LogError($"Instantiate '{prefabName}' error");
                        return;
                    }
                    curInstance = HotUpdateBehaviour.GetComponentByType(go, typeof(WaitingPanel)) as WaitingPanel;

                    go.transform.SetParent(goMainRoot.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;
                    curInstance.mTimeout = fTimeout;
                    curInstance.mActionTimeout = actionTimeout;
                    curInstance.mRealTime = realTime;
                    //Not support Coroutine in FREE version, using 'OnUpdate' instead
                    //curInstance.StopAllCoroutines();
                    //if (fTimeout > 0f && actionTimeout != null)
                    //    curInstance.StartCoroutine("CorTimeout", fTimeout, actionTimeout, realTime);
                };
            req.OnError += (error) =>
            {
                Debug.LogError("ResourceManager.LoadAssetAsync(\"" + prefabName + "\") occur error : " + error);
                //Debug.LogError($"ResourceManager.LoadAssetAsync(\"{prefabName}\") occur error : {error}");
            };
        }
    }
}

