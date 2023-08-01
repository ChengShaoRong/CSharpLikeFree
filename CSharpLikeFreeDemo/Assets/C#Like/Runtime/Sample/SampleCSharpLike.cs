/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace CSharpLike
{
    /// <summary>
    /// Flow diagram : 
    /// 1. Download the dynamic games config JSON from your server.
    /// 2. Show the your dynamic games in this scene for player choose.
    /// 3. Player choose one of your games.
    /// 4. Download that game AssetBundle if it was not downloaded.
    /// 5. Initialize the binary hot update code from the AssetBundle.
    /// 6. Load scene or prefab from the AssetBundle, that config in the JSON.
    /// 7. Enjoy yourself.
    /// 
    /// e.g. We suppose that we have two complete different game, they are with different hot update code and AssetBundle.
    /// This scene has two button, one button for enter game A, other button for enter game B.
    /// Game A : the sample of the C#Like.
    /// Game B : the 'Tanks! Tutorial' power by Unity, we turn it into hot update project by C#Like(C#LikeFree). 
    /// ('Tanks! Tutorial' original source in https://assetstore.unity.com/packages/essentials/tutorial-projects/tanks-tutorial-46209 ).
    /// </summary>
    public class SampleCSharpLike : MonoBehaviour
    {
        /// <summary>
        /// The dynamic config JSON
        /// </summary>
        public static JSONData games;
        /// <summary>
        /// Flow diagram :
        /// 1. Download the dynamic games config JSON from your server.
        /// </summary>
        IEnumerator Start()
        {
            //Initialize the hot update script system
            MyHotUpdateManager.Init();

            //Request config JSON from your web server.
#if UNITY_EDITOR
            string url = Path.Combine(Application.streamingAssetsPath, "AssetBundles/games" + HotUpdateManager.GetPlatformString() + ".json");
            //string url = "http://127.0.0.1/CSharpLikeFreeDemo/AssetBundles/games" + HotUpdateManager.GetPlatformString() + ".json";
#else
            //You MUST modify here, change to your real gateway URI
            //Default value '"https://www.csharplike.com/CSharpLikeFreeDemo/AssetBundles/games"+HotUpdateManager.GetPlatformString()+".json"' is the demo URI
            string url = "https://www.csharplike.com/CSharpLikeFreeDemo/AssetBundles/games"+HotUpdateManager.GetPlatformString()+".json";
#endif
            do
            {
                Tips = $"Requesting {url}";
                using (UnityWebRequest uwr = UnityWebRequest.Get(url))
                {
                    yield return uwr.SendWebRequest();
                    if (string.IsNullOrEmpty(uwr.error))
                    {
                        games = KissJson.ToJSONData(ResourceManager.GetUTF8String(uwr.downloadHandler.data));
                        break;
                    }
                    else
                    {
                        Tips = $"Requesting {url} with error {uwr.error}";
                        yield return new WaitForSeconds(5f);
                    }
                }
            } while (true);
            //Notify to show the dynamic games for player choose. we show it in OnGUI.
            showButton = true;
        }
        bool showButton = false;
        /// <summary>
        /// Flow diagram :
        /// 2. Show the your dynamic games in this scene for player choose.
        /// </summary>
        void OnGUI()
        {
            //Show a tips
            GUI.Label(new Rect(100, 50, 800, 150), Tips);

            //Show the dynamic games for player choose.
            if (showButton)
            {
                GUIStyle fontStyle = new GUIStyle(GUI.skin.button) { fontSize = 24 };
                int i = 0;
                foreach(JSONData json in games.Value as List<JSONData>)
                {
                    //We show game infomation very simple here, you may make it more beautiful. e.g. with some icon fit your game.
                    if (GUI.Button(new Rect(100, 200 + 150 * i, 400, 64), json["name"], fontStyle))
                    {
                        StartCoroutine(CoroutineLoadGame(json));//Flow diagram : 3. Player choose one of your games.
                        showButton = false;//Hide the buttons when click one of them
                    }
                    i++;
                }
            }
        }
        /// <summary>
        /// Flow diagram : 
        /// 4. Download that game AssetBundle if it was not downloaded.
        /// 5. Initialize the binary hot update code from the AssetBundle.
        /// 6. Load scene or prefab from the AssetBundle, that config in the JSON.
        /// 7. Enjoy yourself.
        /// </summary>
        IEnumerator CoroutineLoadGame(JSONData json)
        {
            Tips = $"CoroutineLoadGame : Initialize ResourceManager.Init({json["url"]})";


            //Initialize AssetBundle
            ResourceManager.Init(json["url"]);//If you set 'autoLoadAssetBundle' value false, you must call 'PreLoadAssetBundleManually' by yourself.
            //Waitting for AssetBundle loaded
            while (ResourceManager.state < ResourceManager.State.ConfigDone)
            {
                if (ResourceManager.state == ResourceManager.State.LoadingConfigError)
                {
                    Tips = $"Loading {json["name"]} fail with error, please check the log.";
                    showButton = true;
                    yield break;
                }
                yield return null;
            }
            //Load hot update script binary file from AssetBundle
            var codeFile = ResourceManager.LoadAssetAsync<TextAsset>(json["codeFile"]);
            yield return codeFile;
            if (!string.IsNullOrEmpty(codeFile.error))
            {
                Tips = $"Loading {json["codeFile"]} from AssetBundle fail with error {codeFile.error}.";
                showButton = true;
                yield break;
            }
            //Initialize hot update script binary file
            var loadAsync = HotUpdateManager.LoadAsync(codeFile.asset.bytes);
            yield return loadAsync;
            if (!loadAsync.success)
            {
                Tips = $"Initialize {json["codeFile"]} fail with error.";
                showButton = true;
                yield break;
            }
            if (json.ContainsKey("loadFirstScene") && json["loadFirstScene"] != "")// Load a new scene for chosen game (recommend)
            {
                string loadFirstScene = json["loadFirstScene"];
                // (Recommend) Load scene style 1: (Asynchronous Loading scene)
                ResourceManager.LoadSceneAsync(loadFirstScene, UnityEngine.SceneManagement.LoadSceneMode.Single, (string error) =>
                {
                    if (string.IsNullOrEmpty(error))//Load success
                        Debug.Log($"Scene '{loadFirstScene}' loaded");
                    else//Load error
                        Debug.Log($"Scene '{loadFirstScene}' load error : {error}");
                });
                // Load scene style 2: (Synchronizing load scene)
                /*
                if (!ResourceManager.AssetExist(loadFirstScene))//Must check whether exist
                {
                    Tips = $"Scene '{loadFirstScene}' not exist in AssetBundle";
                    showButton = true;
                    yield break;
                }
                Tips = $"Waitting for loading AssetBundle that include scene '{loadFirstScene}'";
                while (!ResourceManager.AssetLoaded(loadFirstScene))//Must waitting for AssetBundle loaded
                {
                    yield return null;
                }
                Tips = $"Waitting for loading scene '{loadFirstScene}'";
                ResourceManager.LoadScene(loadFirstScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
                */
            }
            else if (json.ContainsKey("loadFirstPrefab") && json["loadFirstPrefab"] != "")// Load a new prefab for chosen game
            {
                HotUpdateManager.Show("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab",
                    (HotUpdateBehaviour behaviour) =>
                    {
                        Debug.Log("Load SampleCSharpLikeHotUpdate");
                    },
                    transform.parent);
                Destroy(gameObject);//Destroy self
            }
        }

        string mTips = "";
        /// <summary>
        /// the tips string show in label content
        /// </summary>
        string Tips
        {
            get
            {
                return mTips;
            }
            set
            {
                mTips = value;
                Debug.Log(mTips);
            }
        }
    }
    public class TestClassBase
    {
        public TestClassBase(string str)
        {
            member = str;
            Debug.Log("TestClassBase(" + str + ")");
        }
        public int iA = 10;
        public string strA = "strA";
        public void Func(int i)
        {
            Debug.Log("Func:"+i);
        }
        public static void FuncStatic(int i)
        {
            Debug.Log("Func:" + i);
        }
        public string member
        {
            get;set;
        }
    }
}