//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureBox
{
    /// <summary>
    /// Customize MessageTipsTop in hot update script
    /// </summary>
	public class MessageTipsTop : LikeBehaviour
    {
        [SerializeField]
        TMP_Text textContent;

        public static void Show(string strContent)
        {
            Debug.Log("MessageTipsTop.Show('" + strContent + "')");
            //Debug.Log($"MessageTipsTop.Show('{strContent}')");
            string prefabName = Global.GetPrefabFullName("MessageTipsTop");
            var req = ResourceManager.LoadAssetAsync<GameObject>(prefabName);
            req.OnCompleted +=
                (GameObject go) =>
                {
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
                    go.transform.SetParent(goMainRoot.transform);
                    go.transform.localScale = Vector3.one;
                    go.transform.localPosition = Vector3.zero;
                    MessageTipsTop mt = HotUpdateBehaviour.GetComponentByType(go, typeof(MessageTipsTop)) as MessageTipsTop;
                    mt.textContent.text = Global.GetString(strContent);
                };
            req.OnError += (string error) =>
            {
                Debug.LogError("ResourceManager.LoadAssetAsync(\"" + prefabName + "\") occur error : " + error);
                //Debug.LogError($"ResourceManager.LoadAssetAsync(\"{prefabName}\") occur error : {error}");
            };
        }
        void OnTweener(KissTweenBase tweener)
        {
            GameObject.Destroy(gameObject);
        }
    }
}

