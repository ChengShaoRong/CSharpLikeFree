/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CSharpLike
{
	public class KissMessageBox2 : MonoBehaviour
	{
        public Text textTitle;
        public Text textContent;
        public Text textButtonOK;
        public Button btOK;
        public Text textButtonCancel;
        public Button btCancel;
        public static void Show(string strContent, string strTitle = "", string strButtonOK = "", UnityAction onClickOK = null,
            string strButtonCancel = "", UnityAction onClickCancel = null)
        {
            string prefabName = "KissMessageBox2";
            GameObject goMainRoot = GameObject.Find("MainRoot");
            if (goMainRoot == null)
            {
                Debug.LogError("Not exist GameObject name 'MainRoot' for place KissMessageBox2");
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
            KissMessageBox2 mb = go.GetComponent<KissMessageBox2>();
            if (mb == null)
            {
                Debug.LogError($"Not exist 'KissMessageBox' component in '{prefabName}.prefab'");
                Destroy(go);
                return;
            }
            mb.textTitle.text = strTitle;
            mb.textContent.text = strContent;
            if (!string.IsNullOrEmpty(strButtonOK))
                mb.textButtonOK.text = strButtonOK;
            if (onClickOK != null)
                mb.btOK.onClick.AddListener(onClickOK);
            if (!string.IsNullOrEmpty(strButtonCancel))
                mb.textButtonCancel.text = strButtonCancel;
            if (onClickCancel != null)
                mb.btCancel.onClick.AddListener(onClickCancel);
            mb.btOK.onClick.AddListener(mb.CloseSelf);
            mb.btCancel.onClick.AddListener(mb.CloseSelf);
        }
        void CloseSelf()
        {
            KissTweenAlpha alpha = gameObject.GetComponent<KissTweenAlpha>();
            alpha.onFinished += (tween) =>
            {
                Destroy(gameObject);
            };
            alpha.PlayReverse();

            KissTweenPosition position = gameObject.GetComponentInChildren<KissTweenPosition>(true);
            position.PlayReverse();
        }
    }
}

