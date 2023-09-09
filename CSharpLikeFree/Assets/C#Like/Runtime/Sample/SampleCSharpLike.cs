/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Flow diagram : 
    /// 1. Initialize the hot update script system, goto step 4 directly if only ONE game.
    /// 2. Show the your dynamic games in this scene for player choose.
    /// 3. Player choose one of your games.
    /// 4. Initialize the AssetBundle system.
    /// 5. Initialize the hot update script.
    /// 6. Enter your default game scene.
    /// 
    /// e.g. We suppose that we have 3 complete different games, they are with different hot update code and AssetBundle.
    /// This scene has 3 buttons, one button for one game.
    /// Game A : the build-in demo of the C#Like.
    /// Game B : the 'Tanks! Tutorial' power by Unity, we turn it into hot update project by C#Like(C#LikeFree). 
    /// Game C : the 'Platformer Microgame' power by Unity, we turn it into hot update project by C#Like(C#LikeFree). 
    /// (More detail in https://www.csharplike.com/index.html?page=tutorials ).
    /// 
    /// You don't have to use this class, you can custom your first none hot update class.
    /// It just call the follow functions in your class :
    /// 1. Call 'HotUpdateManager.Init();' in your 'IEnumerator Start()', for initialize the hot update script system, after initialize success, the 'HotUpdateManager.Games' will contains your game infomation.
    /// 2. Call 'ResourceManager.Init(HotUpdateManager.Games[0]);', for initialize the AssetBundle system.
    /// 3. Call 'yield return ResourceManager.LoadCode();', for initialize the hot update script.
    /// 4. Call 'ResourceManager.LoadSceneAsync();', for enter your default game scene.
    /// </summary>
    public class SampleCSharpLike : MonoBehaviour
    {
        /// <summary>
        /// Flow diagram :
        /// 1. Initialize the hot update script system, goto step 4 directly if only ONE game.
        /// </summary>
        IEnumerator Start()
        {
            Tips = "Waiting for initialize 'HotUpdateManager.Init'";
            //Initialize the hot update script system
            /// 1. In UnityEditor, FORCE using 'StreamingAssets/AssetBundles/games[Platform].json' if the 'automatic compile' in C#Like Setting panel WAS CHECKED.
            /// 2. Using the value of 'url' you just input.
            /// 3. Using the value of 'Download Path' in C#Like Setting panel.
            /// 4. Using 'StreamingAssets/AssetBundles/games[Platform].json', that mean you don't have a server and don't need hot update?
            HotUpdateManager.Init();
            //Waiting for initialize success, your config JSON file may be need to download, it'll a very short period of time.
            while (HotUpdateManager.Games == null)
                yield return null;
            Tips = "'HotUpdateManager.Init' done";
            //Notify to show the dynamic games for player choose. we show it in OnGUI.
            state = State.ShowLobby;
            //Only one game, we enter the game directly
            if (HotUpdateManager.Games.Count == 1) 
                StartCoroutine(CoroutineLoadGame(HotUpdateManager.Games[0]));
        }
        enum State
        {
            /// <summary>
            /// Waiting for initialize HotUpdateManager
            /// </summary>
            WaitingInitialize,
            /// <summary>
            /// Show some games for player choose.
            /// </summary>
            ShowLobby,
            /// <summary>
            /// Loading the game that player chosen
            /// </summary>
            LoadingGame,
            /// <summary>
            /// Show the game that player chosen
            /// </summary>
            ShowGame,
        }
        State state = State.WaitingInitialize;
        /// <summary>
        /// Flow diagram :
        /// 2. Show the your dynamic games in this scene for player choose.
        /// </summary>
        void OnGUI()
        {
            switch(state)
            {
                case State.ShowLobby:
                    {
                        //Flow diagram : 2. Show the your dynamic games in this scene for player choose.
                        GUIStyle fontStyle = new GUIStyle(GUI.skin.button) { fontSize = 24 };
                        int i = 0;
                        foreach (JSONData json in HotUpdateManager.Games.Value as List<JSONData>)
                        {
                            //We show game information very simple here, you may make it more beautiful. e.g. with some icon fit your game.
                            //You can config custom JSON in 'C#Like Setting' panel and accept them here.
                            //e.g. Config 'Icon' as 'ABC', you'll get value 'ABC' by 'json["Icon"]'.
                            if (GUI.Button(new Rect(100, 200 + 150 * i, 400, 64), json["displayName"], fontStyle))
                            {
                                StartCoroutine(CoroutineLoadGame(json));//Flow diagram : 3. Player choose one of your games.
                            }
                            i++;
                        }
                    }
                    break;
                case State.LoadingGame:
                    {
                        Tips = $"Downloading '{ResourceManager.config["displayName"]}', state {ResourceManager.StateString}";
                        GUI.HorizontalScrollbar(new Rect(100, 200, 800, 30), ResourceManager.DownloadProgress, 1f, 0f, 1f);
                    }
                    break;
            }
            //Show a tips
            GUI.Label(new Rect(100, 50, 800, 150), Tips);
        }
        /// <summary>
        /// Flow diagram : 
        /// 4. Initialize the AssetBundle system.
        /// 5. Initialize the hot update script.
        /// 6. Enter your default game scene.
        /// </summary>
        IEnumerator CoroutineLoadGame(JSONData json)
        {
            Tips = $"CoroutineLoadGame : Initialize ResourceManager.Init({json["url"]})";

            //Flow diagram : 4. Initialize the AssetBundle system.
            ResourceManager.Init(json["url"]);//If you set 'autoLoadAssetBundle' value false, you must call 'PreLoadAssetBundleManually' by yourself.
            //4.1. Waitting for initialize success, just initialize not include download AssetBundles.
            while (ResourceManager.state < ResourceManager.State.ConfigDone)
            {
                if (ResourceManager.state == ResourceManager.State.LoadingConfigError)//May be download error
                {
                    Tips = $"Loading {json["displayName"]} fail with error, please check the log.";
                    state = State.ShowLobby;
                    yield break;
                }
                yield return null;
            }
            //Show download progress if you need to.
            state = State.LoadingGame;

            //Flow diagram : 5. Initialize the hot update script.
            /// Don't need to wait all AssetBundles download success, it'll wait by itself, so we can initialize the hot update script now.
            List<string> loadCodeErrors = new List<string>();
            yield return ResourceManager.LoadCode(loadCodeErrors);//The 'loadCodeErrors' for get the return value
            if (loadCodeErrors.Count > 0)
            {
                Tips = loadCodeErrors[0];
                state = State.ShowLobby;
                yield break;
            }

            //Flow diagram : 6. Enter your default game scene.
            /// MUST need to wait the the hot update script initialize success, but don't need to wait for all AssetBundles downloaded.
            /// The LoadSceneAsync function will wait for the dependencies AssetBundles by itself.
            /// May be the the default scene was success loaded, and some AssetBundles are STILL downloading.
            // (Recommend) Load scene style 1: (Asynchronous Loading scene)
            ResourceManager.LoadSceneAsync("", UnityEngine.SceneManagement.LoadSceneMode.Single, (string error) =>
            {
                if (string.IsNullOrEmpty(error))//Load success
                {
                    state = State.ShowGame;
                    Debug.Log($"Scene '{ResourceManager.DefaultSceneName}' loaded");
                }
                else//Load error
                {
                    Debug.Log($"Scene '{ResourceManager.DefaultSceneName}' load error : {error}");
                    state = State.ShowLobby;
                }
            });
            /*
            // Load scene style 2: (Synchronizing load scene)
            if (!ResourceManager.AssetExist(ResourceManager.DefaultSceneName))//Must check whether exist
            {
                Tips = $"Scene '{ResourceManager.DefaultSceneName}' not exist in AssetBundle";
                state = State.ShowLobby;
                yield break;
            }
            Tips = $"Waitting for loading AssetBundle that include scene '{ResourceManager.DefaultSceneName}'";
            while (!ResourceManager.AssetLoaded(ResourceManager.DefaultSceneName))//Must waitting for AssetBundle loaded
            {
                yield return null;
            }
            Tips = $"Waitting for loading scene '{ResourceManager.DefaultSceneName}'";
            ResourceManager.LoadScene("", UnityEngine.SceneManagement.LoadSceneMode.Single);
            state = State.ShowGame;
            */
        }

        string mTips = "";
        /// <summary>
        /// The tips string show in label content
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
}