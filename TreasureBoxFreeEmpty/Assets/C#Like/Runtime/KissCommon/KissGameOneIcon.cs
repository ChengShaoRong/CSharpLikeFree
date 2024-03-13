/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CSharpLike
{
	public class KissGameOneIcon : MonoBehaviour
	{
        public RawImage gameIcon;
        public Text gameName;
        public Text gameVersion;
        public Text gameSize;
        JSONData json;
		KissGame kissGame;
        void Init(JSONData json, KissGame kissGame)
        {
            this.json = json;
            this.kissGame = kissGame;
            gameName.text = json["displayName"];
            gameVersion.text = "v " + json["version"];
            gameSize.text = SizeToString(json["sumSize"]);
            //ResourceManager wasn't initialized yet. or else we can download texture like this : ResourceManager.LoadTexture(json["Icon"], gameIcon);
            StartCoroutine(CorLoadIcon());
        }
        IEnumerator CorLoadIcon()
        {
            using UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(json["Icon"]);
            yield return uwr.SendWebRequest();
            if (string.IsNullOrEmpty(uwr.error))
            {
                gameIcon.texture = DownloadHandlerTexture.GetContent(uwr);
                gameIcon.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError($"Download {json["Icon"]} occur error '{uwr.error}'");
                yield return new WaitForSecondsRealtime(10f);
                StartCoroutine(CorLoadIcon());//Try again
            }
        }
        /// <summary>
        /// File size convert to string
        /// </summary>
        string SizeToString(ulong uSize)
        {
            if (uSize < 1024)
                return $"{uSize}B";//e.g. 123B
            else if (uSize < 1024 * 1024)
                return $"{(uSize / 1024f):F02}KB";//e.g. 1.23KB
            else
                return $"{(uSize / (1024f * 1024f)):F02}MB";//e.g. 3.45MB
        }
        public static KissGameOneIcon NewInstance(Transform transformParent, JSONData json, KissGame kissGame)
        {
            string prefabName = "KissGameOneIcon";
            GameObject go = Resources.Load(prefabName) as GameObject;
            if (go == null)
            {
                Debug.LogError($"Not exist '{prefabName}.prefab'");
                return null;
            }
            go = Instantiate(go, new Vector3(0f, 0f, 0f), Quaternion.identity, transformParent);
            if (go == null)
            {
                Debug.LogError($"Instantiate '{prefabName}.prefab' error");
                return null;
            }
            KissGameOneIcon kgoi = go.GetComponent<KissGameOneIcon>();
            if (kgoi == null)
            {
                Debug.LogError($"Not exist 'KissGameOneIcon' component in '{prefabName}.prefab'");
                Destroy(go);
                return null;
            }
            kgoi.Init(json, kissGame);
            return kgoi;
		}
		public void OnClick()
        {
            kissGame.OnClickGame(json);
		}
	}
}

