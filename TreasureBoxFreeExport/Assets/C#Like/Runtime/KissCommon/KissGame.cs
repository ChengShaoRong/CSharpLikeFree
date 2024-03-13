/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
	public class KissGame : MonoBehaviour
	{
        IEnumerator Start()
        {
            //Set target frame rate, you should config it according to your requirements
            Application.targetFrameRate = 30;
            //Initialize the hot update script system
            /// 1. In UnityEditor, FORCE using 'StreamingAssets/AssetBundles/games[Platform].json' if the 'automatic compile' in C#Like Setting panel WAS CHECKED.
            /// 2. Using the value of 'url' you just input.
            /// 3. Using the value of 'Download Path' in C#Like Setting panel.
            /// 4. Using 'StreamingAssets/AssetBundles/games[Platform].json', that mean you don't have a server and don't need hot update?
            HotUpdateManager.Init();
            //Waiting for initialize success, your config JSON file may be need to download, it'll a very short period of time.
            while (HotUpdateManager.Games == null)
                yield return null;

            //Show game button if you has multiple games, it's optional.
            {
                //Find the GameObject name "games"
                Transform transformGames = null;
                foreach (Transform child in transform)
                {
                    if (child.name == "games")
                    {
                        transformGames = child;
                        break;
                    }
                }
                if (transformGames != null)
                {
                    foreach (JSONData json in (HotUpdateManager.Games.Value as List<JSONData>))
                    {
                        KissGameOneIcon.NewInstance(transformGames, json, this);
                    }
                }
            }

            //Caching.ClearCache(); //For test clear cache

            //Only one game, we enter the game directly
            if (HotUpdateManager.Games.Count == 1)
                OnClickGame(HotUpdateManager.Games[0]);
        }

        bool mIsLoading = false;

        public void OnClickGame(JSONData json)
        {
            if (mIsLoading)
                return;
            mIsLoading = true;
            StartCoroutine(CoroutineLoadGame(json));
        }

        void OnClickClearCache()
        {
            Caching.ClearCache();
        }

        /// <summary>
        /// Flow diagram : 
        /// 4. Initialize the AssetBundle system.
        /// 5. Initialize the hot update script.
        /// 6. Enter your default game scene.
        /// </summary>
        IEnumerator CoroutineLoadGame(JSONData json)
        {
            Debug.Log($"CoroutineLoadGame : Initialize ResourceManager.Init({json["url"]})");

            //Flow diagram : 4. Initialize the AssetBundle system.
            ResourceManager.Init(json["url"]);//If you set 'autoLoadAssetBundle' value false, you must call 'PreLoadAssetBundleManually' by yourself.
            //4.1. Waiting for initialize success, just initialize not include download AssetBundles.
            while (ResourceManager.state < ResourceManager.State.ConfigDone)
            {
                if (ResourceManager.state == ResourceManager.State.LoadingConfigError)//May be download error
                {
                    Debug.LogError($"Loading {json["displayName"]} fail with error, please check the log.");
                    mIsLoading = false;
                    yield break;
                }
                yield return null;
            }
            //Show download progress if you need to.

            //Flow diagram : 5. Initialize the hot update script.
            /// Don't need to wait all AssetBundles download success, it'll wait by itself, so we can initialize the hot update script now.
            List<string> loadCodeErrors = new List<string>();
            yield return ResourceManager.LoadCode(loadCodeErrors);//The 'loadCodeErrors' for get the return value
            if (loadCodeErrors.Count > 0)
            {
                Debug.LogError(loadCodeErrors[0]);
                mIsLoading = false;
                yield break;
            }
            //Sample for you call the function in your hot update script from none hot update script.
            ///e.g. we want to call the function 'public int Add(int a, int b)' of class 'CSharpLike.HotUpdateClassBridge'
            Debug.Log("Test call hot update function from none hot update script: Add(1,2) = "
                + HotUpdateBehaviour.GetHotUpdateBridge()?.MemberCall("Add", 1, 2));

            //Flow diagram : 6. Enter your default game scene.
            /// MUST need to wait the the hot update script initialize success, but don't need to wait for all AssetBundles downloaded.
            /// The LoadSceneAsync function will wait for the dependencies AssetBundles by itself.
            /// May be the the default scene was success loaded, and some AssetBundles are STILL downloading.
            // (Recommend) Load scene style 1: (Asynchronous Loading scene)
            ResourceManager.LoadSceneAsync("", UnityEngine.SceneManagement.LoadSceneMode.Single, (string error) =>
            {
                if (string.IsNullOrEmpty(error))//Load success
                {
                    Debug.Log($"Scene '{ResourceManager.DefaultSceneName}' loaded");
                    HotUpdateBehaviour.GetHotUpdateBridge()?.MemberCall("OnLoadDefaultSceneDone");
                }
                else//Load error
                {
                    Debug.Log($"Scene '{ResourceManager.DefaultSceneName}' load error : {error}");
                }
            });
            mIsLoading = false;
            /*
            // Load scene style 2: (Synchronizing load scene)
            if (!ResourceManager.AssetExist(ResourceManager.DefaultSceneName))//Must check whether exist
            {
                Tips = $"Scene '{ResourceManager.DefaultSceneName}' not exist in AssetBundle";
                mIsLoading = false;
                yield break;
            }
            Tips = $"Waiting for loading AssetBundle that include scene '{ResourceManager.DefaultSceneName}'";
            while (!ResourceManager.AssetLoaded(ResourceManager.DefaultSceneName))//Must waiting for AssetBundle loaded
            {
                yield return null;
            }
            Tips = $"Waiting for loading scene '{ResourceManager.DefaultSceneName}'";
            ResourceManager.LoadScene("", UnityEngine.SceneManagement.LoadSceneMode.Single);
            HotUpdateBehaviour.GetHotUpdateBridge().MemberCall("OnLoadDefaultSceneDone");
            mIsLoading = false;
            */
        }
    }
}

