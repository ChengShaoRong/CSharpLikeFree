//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TreasureBox
{
    /// <summary>
    /// Customize MessageBox2 in hot update script
    /// </summary>
	public class MessageBox2 : LikeBehaviour
    {
        [SerializeField]
        TMP_Text textTitle;
        [SerializeField]
        TMP_Text textContent;
        [SerializeField]
        TMP_Text textButtonOK;
        [SerializeField]
        Button btOK;
        [SerializeField]
        TMP_Text textButtonCancel;
        [SerializeField]
        Button btCancel;
        [SerializeField]
        KissTweenPosition tweenPosition;


        static GameObject curInstance = null;
        public static void Show(string strContent, string strTitle, string strButtonOK, UnityAction onClickOK,
            string strButtonCancel, UnityAction onClickCancel)
        //Free version not support default param
        //public static void Show(string strContent, string strTitle = "", string strButtonOK = "", UnityAction onClickOK = null,
        //    string strButtonCancel = "", UnityAction onClickCancel = null)
        {
            Debug.Log("MessageBox2.Show('" + strContent + "')");
            //Debug.Log($"MessageBox2.Show('{strContent}')");
            string prefabName = Global.GetPrefabFullName("MessageBox2");
            var req = ResourceManager.LoadAssetAsync<GameObject>(prefabName);
            req.OnCompleted +=
                (go) =>
                {
                    if (curInstance != null)//Make sure just ONE MessageBox2 in one request
                    {
                        Debug.LogError("Exist an old MessageBox2 instance");
                        return;
                    }
                    GameObject goMainRoot = GameObject.Find("MainRoot");
                    if (goMainRoot == null)
                    {
                        Debug.LogError("Not exist 'MainRoot'");
                        return;
                    }
                    go = GameObject.Instantiate(go, new Vector3(0f, 0f, 0f), Quaternion.identity);
                    if (go == null)
                    {
                        Debug.LogError("Instantiate '" + prefabName + "' error");
                        //Debug.LogError($"Instantiate '{prefabName}' error");
                        return;
                    }
                    curInstance = go;
                    go.transform.SetParent(goMainRoot.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;
                    MessageBox2 mb = HotUpdateBehaviour.GetComponentByType(go, typeof(MessageBox2)) as MessageBox2;
                    mb.textTitle.text = Global.GetString(strTitle);
                    mb.textContent.text = Global.GetString(strContent);
                    if (!string.IsNullOrEmpty(strButtonOK))
                        mb.textButtonOK.text = Global.GetString(strButtonOK);
                    if (onClickOK != null)
                        mb.btOK.onClick.AddListener(onClickOK);
                    if (!string.IsNullOrEmpty(strButtonCancel))
                        mb.textButtonCancel.text = Global.GetString(strButtonCancel);
                    if (onClickCancel != null)
                        mb.btCancel.onClick.AddListener(onClickCancel);
                    mb.btOK.onClick.AddListener(()=> { mb.StartClose(); });
                    mb.btCancel.onClick.AddListener(() => { mb.StartClose(); });
                    KissTweenAlpha alpha = go.GetComponent<KissTweenAlpha>();
                    alpha.onFinished = null;
                    alpha.PlayForward();
                    KissTweenPosition position = go.GetComponentInChildren<KissTweenPosition>();
                    position.PlayForward();
                };
            req.OnError += (error) =>
            {
                Debug.LogError("ResourceManager.LoadAssetAsync(\"" + prefabName + "\") occur error : " + error);
                //Debug.LogError($"ResourceManager.LoadAssetAsync(\"{prefabName}\") occur error : {error}");
            };
        }
        void StartClose()
        {
            KissTweenAlpha alpha = gameObject.GetComponent<KissTweenAlpha>();
            alpha.onFinished += (tween) =>
            {
                curInstance = null;
                GameObject.Destroy(gameObject);
            };
            alpha.PlayReverse();

            tweenPosition.PlayReverse();
        }
    }
}

