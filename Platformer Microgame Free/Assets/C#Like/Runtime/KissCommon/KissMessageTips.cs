/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
	public class KissMessageTips : MonoBehaviour
    {
        public Text textContent;
        public static void Show(string strContent)
        {
            string prefabName = "KissMessageTips";
            GameObject goMainRoot = GameObject.Find("MainRoot");
            if (goMainRoot == null)
            {
                Debug.LogError("Not exist GameObject name 'MainRoot' for place KissMessageTips");
                return;
            }
            GameObject go = Resources.Load(prefabName) as GameObject;
            if (go == null)
            {
                Debug.LogError($"Not exist '{prefabName}.prefab'");
                return;
            }
            go = Instantiate(go, Vector3.zero, Quaternion.identity, goMainRoot.transform);
            if (go == null)
            {
                Debug.LogError($"Instantiate '{prefabName}.prefab' error");
                return;
            }
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            KissMessageTips mt = go.GetComponent<KissMessageTips>();
            if (mt == null)
            {
                Debug.LogError("Not exist 'KissMessageTips' component");
                Destroy(go);
                return;
            }
            mt.textContent.text = strContent;
        }
        void OnTweener(KissTweenBase tweener)
        {
            Destroy(gameObject);
        }
    }
}

