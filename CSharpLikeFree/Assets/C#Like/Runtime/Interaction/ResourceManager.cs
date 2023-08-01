/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 * 
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

namespace CSharpLike
{

    /*
Config JSON format :
{
    "version": "1.0",
    "url": "http://your-assetBundle-file-download-path/AssetBundles/StandaloneWindows/",
    "files": [
        {
            "fileName": "AssetBundleFile1.ab",
            "hash128": "76eaff59eafeac0e1b21c61fc5660077",
            "size": 123,
            "assets": [
                "Assets/C#Like/Resources/Sample/CSV/Item.csv",
                "Assets/C#Like/Resources/Sample/image/Money0.png",
                "Assets/C#Like/Resources/Sample/AircraftBattle/Enemy1.prefab"
            ],
            "dependencies":[],
        },
        {
            "fileName": "AssetBundleFile2.ab",
            "hash128": "76eaff59eafeac0e1b21c61fc5660071",
            "size": 321,
            "assets": [
                "Assets/_Complete-Game_HotUpdate.unity"
            ],
            "dependencies":[
                "AssetBundleFile1.ab"
            ],
        }
    ]
}
     */
    /// <summary>
    /// Resource manager for loading asset from AssetBundle.
    /// Design base on rule of 'Keep It simple, Stupid'.
    /// We make it as simple as possible.
    /// If you want more control with AssetBundle, recommand using 'Addressables'.
    /// </summary>
    public class ResourceManager : MonoBehaviour
	{
        public class MyAssetBundleRequest<T> : CustomYieldInstruction where T : UnityEngine.Object
        {
            /// <summary>
            /// The final loaded asset from AssetBundle
            /// </summary>
			public T asset;
            /// <summary>
            /// Error infomation when loading asset from AssetBundle, default is null
            /// </summary>
            public string error;

            public MyAssetBundleRequest(MyAssetBundle mab, string nameWithSuffix)
            {
                asset = null;
                error = "";
                IsDone = false;
                instance.StartCoroutine(instance.CoroutineLoad(mab, nameWithSuffix, this));
			}
            public MyAssetBundleRequest()
            {
                asset = null;
                error = "Not exist this asset in AssetBundle";
                IsDone = true;
            }
            bool mIsDone = false;
            public bool IsDone
            {
                get { return mIsDone; }
                set
                {
                    mIsDone = value;
                    if (mIsDone)
                    {
                        if (string.IsNullOrEmpty(error))
                        {
                            OnCompleted?.Invoke(asset);
                        }
                        else
                        {
                            OnError?.Invoke(error);
                        }
                    }
                }
            }
            public override bool keepWaiting => !IsDone;
            /// <summary>
            /// On Completed action when the asset loaded from AssetBundle with NO error.
            /// </summary>
            public event Action<T> OnCompleted;
            /// <summary>
            /// On Error acction when loading asset from AssetBundle with error.
            /// </summary>
            public event Action<string> OnError;
        }
        /// <summary>
        /// The AssetBundle loading state
        /// </summary>
        public enum State
        {
            None,
            /// <summary>
            /// Loaing Config
            /// </summary>
            LoadingConfig,
            /// <summary>
            /// Loaing Config Error
            /// </summary>
            LoadingConfigError,
            /// <summary>
            /// Analyzing Config 
            /// </summary>
            LoadingConfigDone,
            /// <summary>
            /// Config Done
            /// </summary>
            ConfigDone,
            /// <summary>
            /// Loading AssetBundle
            /// </summary>
            LoadingAssetBundle,
            /// <summary>
            /// All AssetBundle was loaded
            /// </summary>
            AllAssetBundleLoaded
        }
        static string ReplaceStreamingAssets(string path)
        {
            if (path.StartsWith("StreamingAssets"))
            {
                return Path.Combine(Application.streamingAssetsPath, path.Substring(16));
            }
            return path;
        }
        /// <summary>
        /// Initialize ResourceManager
        /// </summary>
        /// <param name="configJsonUrl">Config JOSN URL, if start with 'StreamingAssets' will replace 'Application.streamingAssetsPath' automatically.</param>
        /// <param name="autoLoadAssetBundle">Whether download or load all AssetBundle automatically, default value is true.If value is false, you can call 'PreLoadAssetBundleManually' to preload some AssetBundle of them.</param>
        /// <param name="unloadOldLoadedObjects">Whether unload all loaded objects if was loaded AssetBundle before.</param>
        public static void Init(string configJsonUrl, bool autoLoadAssetBundle = true, bool unloadOldLoadedObjects = true)
        {
            UnLoadAll(unloadOldLoadedObjects);
            if (instance != null)
                GameObject.Destroy(instance.gameObject);
            instance = new GameObject("ResourceManager").AddComponent<ResourceManager>();
            GameObject.DontDestroyOnLoad(instance);
            instance.StopAllCoroutines();
            instance.StartCoroutine(instance.CoroutineLoad(ReplaceStreamingAssets(configJsonUrl), autoLoadAssetBundle));
        }
        /// <summary>
        /// Clear all old AssetBundle. Mean clear up the ResourceManager.
        /// It'll be call in initialize ResourceManager.
        /// </summary>
        /// <param name="unloadAllLoadedObjects">Whether unload all loaded objects if was loaded AssetBundle before.</param>
        public static void UnLoadAll(bool unloadAllLoadedObjects)
        {
            if (instance != null)
            {
                Debug.Log($"Start ResourceManager.UnLoadAll({unloadAllLoadedObjects})");
                foreach (MyAssetBundle mab in instance.assetBundles.Values)
                    mab.Release(unloadAllLoadedObjects);
                instance.assetBundles.Clear();
                Debug.Log($"Finish ResourceManager.UnLoadAll({unloadAllLoadedObjects})");
            }
        }
        /// <summary>
        /// PreLoad AssetBundle manually, if you was called function 'Init' with parameter 'autoLoadAssetBundle' as false. 
        /// </summary>
        /// <param name="assetBundleFileName">AssetBundle file name, if value empty mean preload all AssetBundle</param>
        public static void PreLoadAssetBundleManually(string assetBundleFileName = "")
        {
            assetBundleFileName = assetBundleFileName.ToLower();
            if (string.IsNullOrEmpty(assetBundleFileName))
            {
                instance.StartCoroutine(instance.CoroutineLoadAll());
            }
            else
            {
                if (instance.assetBundles.TryGetValue(assetBundleFileName, out MyAssetBundle mab))
                {
                    if (mab.state == MyAssetBundle.State.None || mab.state == MyAssetBundle.State.Error)
                        instance.StartCoroutine(instance.CoroutineLoad(mab));
                    else
                        Debug.Log($"ResourceManager.LoadAsset({assetBundleFileName}) state = {mab.state}");
                }
                else
                    Debug.Log($"ResourceManager.LoadAsset({assetBundleFileName}) not exist that name");
            }
        }
        /// <summary>
        /// The state of the resource manager.
        /// </summary>
        public static State state = State.None;
        /// <summary>
        /// The size of AssetBundle.
        /// Take effect in 'state >= State.ConfigDone'.
        /// </summary>
        public static ulong TotalSize
        {
            get
            {
                ulong size = 0ul;
                if (state >= State.ConfigDone)
                {
                    foreach (MyAssetBundle one in instance.assetBundles.Values)
                    {
                        size += one.size;
                    }
                }
                return size;
            }
        }
        /// <summary>
        /// The size of AssetBundle the need to be download.
        /// Take effect in 'state >= State.ConfigDone'.
        /// </summary>
        public static ulong DownloadTotalSize
        {
            get
            {
                ulong size = 0ul;
                if (state >= State.ConfigDone)
                {
                    foreach (MyAssetBundle one in instance.assetBundles.Values)
                    {
                        if (!one.wasDownloaded)
                            size += one.size;
                    }
                }
                return size;
            }
        }
        /// <summary>
        /// The size of AssetBundle the downloaded in this downloading.
        /// Take effect in 'state >= State.ConfigDone'.
        /// </summary>
        public static ulong DownloadedSize
        {
            get
            {
                ulong size = 0ul;
                if (state >= State.ConfigDone)
                {
                    foreach (MyAssetBundle one in instance.assetBundles.Values)
                    {
                        if (!one.wasDownloaded)
                        {
                            switch (one.state)
                            {
                                case MyAssetBundle.State.Loading:
                                    if (one.uwr != null)
                                    {
                                        size += one.uwr.downloadedBytes;
                                    }
                                    break;
                                case MyAssetBundle.State.Loaded:
                                    size += one.size;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                return size;
            }
        }
        /// <summary>
        /// The download progress in this downloading.  Value between 0f and 1f.
        /// Take effect in 'state >= State.ConfigDone'.
        /// </summary>
        public static float DownloadProgress
        {
            get
            {
                return DownloadTotalSize > 0ul ? DownloadedSize / (float)DownloadTotalSize : 1f;
            }
        }
        /// <summary>
        /// Synchronizing load Asset from AssetBundle by full name with subffix.
        /// You must make sure that the AssetBundle was loaded.
        /// You can using synchronizing load asset after 'ResourceManager.state == State.AllAssetBundleLoaded'.
        /// Or else, You should call 'AssetExist' and 'AssetLoaded' before call this due to it may be return 'null'.
        /// </summary>
        /// <typeparam name="T">The asset type</typeparam>
        /// <param name="nameWithSuffix">Full name with subffix and start with 'Asset/', Case-insensitive</param>
        /// <returns>The final asset load from AssetBundle, that may be null if that asset not exist or not loaded.</returns>
        public static T LoadAsset<T>(string nameWithSuffix) where T : UnityEngine.Object
        {
            nameWithSuffix = nameWithSuffix.ToLower();
            foreach (MyAssetBundle mab in instance.assetBundles.Values)
            {
                if (mab.assets.ContainsKey(nameWithSuffix))
                {
                    T ret = mab.LoadAsset<T>(nameWithSuffix);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Synchronizing instantiate object after synchronizing load Asset from AssetBundle by full name with subffix.
        /// You must make sure that the AssetBundle was loaded.
        /// You can using synchronizing load asset after 'ResourceManager.state == State.AllAssetBundleLoaded'.
        /// Or else, You should call 'AssetExist' and 'AssetLoaded' before call this due to it may be return 'null'.
        /// </summary>
        /// <typeparam name="T">The asset type</typeparam>
        /// <param name="nameWithSuffix">Full name with subffix and start with 'Asset/', Case-insensitive</param>
        /// <returns>The final instantiate object load from AssetBundle, that may be null if that asset not exist or not loaded.</returns>
        public static T Instantiate<T>(string nameWithSuffix) where T : UnityEngine.Object
        {
            T original = LoadAsset<T>(nameWithSuffix);
            if (original == null)
                return null;
            return Instantiate(original);
        }
        /// <summary>
        /// Synchronizing instantiate object after synchronizing load Asset from AssetBundle by full name with subffix.
        /// You must make sure that the AssetBundle was loaded.
        /// You can using synchronizing load asset after 'ResourceManager.state == State.AllAssetBundleLoaded'.
        /// Or else, You should call 'AssetExist' and 'AssetLoaded' before call this due to it may be return 'null'.
        /// </summary>
        /// <typeparam name="T">The asset type</typeparam>
        /// <param name="nameWithSuffix">Full name with subffix and start with 'Asset/', Case-insensitive</param>
        /// <param name="parent">parent</param>
        /// <returns>The final instantiate object load from AssetBundle, that may be null if that asset not exist or not loaded.</returns>
        public static T Instantiate<T>(string nameWithSuffix, Transform parent) where T : UnityEngine.Object
        {
            T original = LoadAsset<T>(nameWithSuffix);
            if (original == null)
                return null;
            return Instantiate(original, parent);
        }
        /// <summary>
        /// Synchronizing instantiate object after synchronizing load Asset from AssetBundle by full name with subffix.
        /// You must make sure that the AssetBundle was loaded.
        /// You can using synchronizing load asset after 'ResourceManager.state == State.AllAssetBundleLoaded'.
        /// Or else, You should call 'AssetExist' and 'AssetLoaded' before call this due to it may be return 'null'.
        /// </summary>
        /// <typeparam name="T">The asset type</typeparam>
        /// <param name="nameWithSuffix">Full name with subffix and start with 'Asset/', Case-insensitive</param>
        /// <param name="position">position</param>
        /// <param name="rotation">rotation</param>
        /// <returns>The final instantiate object load from AssetBundle, that may be null if that asset not exist or not loaded.</returns>
        public static T Instantiate<T>(string nameWithSuffix, Vector3 position, Quaternion rotation) where T : UnityEngine.Object
        {
            T original = LoadAsset<T>(nameWithSuffix);
            if (original == null)
                return null;
            return Instantiate(original, position, rotation);
        }
        /// <summary>
        /// Synchronizing instantiate object after synchronizing load Asset from AssetBundle by full name with subffix.
        /// You must make sure that the AssetBundle was loaded.
        /// You can using synchronizing load asset after 'ResourceManager.state == State.AllAssetBundleLoaded'.
        /// Or else, You should call 'AssetExist' and 'AssetLoaded' before call this due to it may be return 'null'.
        /// </summary>
        /// <typeparam name="T">The asset type</typeparam>
        /// <param name="nameWithSuffix">Full name with subffix and start with 'Asset/', Case-insensitive</param>
        /// <param name="position">position</param>
        /// <param name="rotation">rotation</param>
        /// <param name="parent">parent</param>
        /// <returns>The final instantiate object load from AssetBundle, that may be null if that asset not exist or not loaded.</returns>
        public static T Instantiate<T>(string nameWithSuffix, Vector3 position, Quaternion rotation, Transform parent) where T : UnityEngine.Object
        {
            T original = LoadAsset<T>(nameWithSuffix);
            if (original == null)
                return null;
            return Instantiate(original, position, rotation, parent);
        }
        /// <summary>
        /// Asynchronous Loading asset from AssetBundle.
        /// Recommend using this way load asset from AssetBundle.
        /// 1. You can using Coroutine. e.g. 
        /// '
        ///     var mabr = ResourceManager.LoadAssetAsync&lt;GameObject&gt;("Assets/xxxxx.prefab");
        ///     yield return mabr;
        ///     var go = mabr.asset;
        /// '
        /// 2. You can using OnCompleted event. e.g.
        /// '
        ///     ResourceManager.LoadAssetAsync&lt;GameObject&gt;("Assets/xxxxx.prefab").OnCompleted += (GameObject go) =>
        ///     {
        ///         //do something with result 'go'.
        ///     }
        /// '
        /// </summary>
        /// <typeparam name="T">The asset type</typeparam>
        /// <param name="nameWithSuffix">Full name with subffix and start with 'Asset/', Case-insensitive</param>
        /// <returns>The asynchronous request</returns>
        public static MyAssetBundleRequest<T> LoadAssetAsync<T>(string nameWithSuffix) where T : UnityEngine.Object
        {
            nameWithSuffix = nameWithSuffix.ToLower();
            foreach (MyAssetBundle mab in instance.assetBundles.Values)
            {
                if (mab.assets.ContainsKey(nameWithSuffix))
                {
                    return new MyAssetBundleRequest<T>(mab, nameWithSuffix);
                }
            }
            return new MyAssetBundleRequest<T>();
        }
        /// <summary>
        /// Synchronizing load scene.
        /// Just trim scene name and then call the 'SceneManager.LoadScene'.
        /// </summary>
        /// <param name="sceneName">Full name with subffix and start with 'Asset/' and end with '.unity', Case-sensitive</param>
        /// <param name="mode">If LoadSceneMode.Single then all current Scenes will be unloaded before loading.</param>
        public static bool LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (!AssetExist(sceneName))
                return false;
            if (!AssetLoaded(sceneName))
                return false;
            int i = sceneName.LastIndexOf("/");
            if (i > 0)
                sceneName = sceneName.Substring(i+1);
            if (sceneName.EndsWith(".unity"))
                sceneName = sceneName.Substring(0, sceneName.Length - 6);
            SceneManager.LoadScene(sceneName, mode);
            return true;
        }
        /// <summary>
        /// Asynchronous Loading scene.
        /// Just trim scene name and then call the 'SceneManager.LoadSceneAsync'.
        /// </summary>
        /// <param name="sceneName">Full name with subffix and start with 'Asset/' and end with '.unity', Case-sensitive</param>
        /// <param name="mode">If LoadSceneMode.Single then all current Scenes will be unloaded before loading.</param>
        /// <param name="completed">The completed event, default null mean you don't care it.</param>
        public static void LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single, Action<string> completed = null)
        {
            if (!AssetExist(sceneName))
                completed?.Invoke("Not exist sceneName :" + sceneName);
            instance.StartCoroutine(instance.CoroutineLoadScene(sceneName, mode, completed));
        }
        IEnumerator CoroutineLoadScene(string sceneName, LoadSceneMode mode, Action<string> completed)
        {
            while (!AssetLoaded(sceneName))
                yield return null;
            int i = sceneName.LastIndexOf("/");
            if (i > 0)
                sceneName = sceneName.Substring(i + 1);
            if (sceneName.EndsWith(".unity"))
                sceneName = sceneName.Substring(0, sceneName.Length - 6);
            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName, mode);
            if (ao == null)
            {
                completed?.Invoke("error");
                yield break;
            }
            yield return ao;
            completed?.Invoke("");
        }
        /// <summary>
        /// Get AudioClip from web, due to something error read AudioClip from AssetBundle in WebGL
        /// </summary>
        /// <param name="uri">The URI of the audio clip to download.</param>
        /// <param name="audioType">The type of audio encoding for the downloaded audio clip. See AudioType.</param>
        /// <param name="completed">The completed event for final result</param>
        public static void LoadAudioClipAsync(string uri, AudioType audioType, Action<AudioClip> completed)
        {
            if (instance == null)
            {
                instance = new GameObject("ResourceManager").AddComponent<ResourceManager>();
                GameObject.DontDestroyOnLoad(instance);
            }
            instance.StartCoroutine(instance.CoroutineLoadAudioClip(uri, audioType, completed));
        }
        IEnumerator CoroutineLoadAudioClip(string uri, AudioType audioType, Action<AudioClip> completed)
        {
            using UnityWebRequest uwrm = UnityWebRequestMultimedia.GetAudioClip(uri, audioType);
            yield return uwrm.SendWebRequest();
            completed?.Invoke(uwrm.result == UnityWebRequest.Result.Success ? DownloadHandlerAudioClip.GetContent(uwrm) : null);
        }
        /// <summary>
        /// Check the asset whether exist in AssetBundle.
        /// </summary>
        /// <param name="nameWithSuffix">Full name with subffix and start with 'Asset/', Case-insensitive</param>
        /// <returns>Whether that asset exist in AssetBundle</returns>
        public static bool AssetExist(string nameWithSuffix)
        {
            nameWithSuffix = nameWithSuffix.ToLower();
            foreach (MyAssetBundle mab in instance.assetBundles.Values)
            {
                if (mab.assets.ContainsKey(nameWithSuffix))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Check that AssetBundle whether was loaded.
        /// </summary>
        /// <param name="nameWithSuffix">Full name with subffix and start with 'Asset/', Case-insensitive</param>
        /// <returns>Whether that AssetBundle was loaded</returns>
        public static bool AssetLoaded(string nameWithSuffix)
        {
            nameWithSuffix = nameWithSuffix.ToLower();
            foreach (MyAssetBundle mab in instance.assetBundles.Values)
            {
                if (mab.assets.ContainsKey(nameWithSuffix))
                {
                    return mab.IsLoaded();
                }
            }
            return false;
        }
        /// <summary>
        /// Get UTF8 string by binary buffer that may be include UTF8 BOM.
        /// </summary>
        /// <param name="buffer">source buffer, that may be with UTF8 BOM</param>
        public static string GetUTF8String(byte[] buffer)
        {
            if (buffer == null)
                return null;

            if (buffer.Length <= 3)
            {
                return Encoding.UTF8.GetString(buffer);
            }

            byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };

            if (buffer[0] == bomBuffer[0]
                && buffer[1] == bomBuffer[1]
                && buffer[2] == bomBuffer[2])
            {
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
            }

            return Encoding.UTF8.GetString(buffer);
        }
        #region Private Impl, you won't care this
        static string RemoveSuffix(string str)
        {
            int i = str.LastIndexOf('.');
            if (i > 0)
                return str.Substring(0, i);
            return str;
        }
        public class MyAssetBundle
        {
            public MyAssetBundle(JSONData jsonData, string url)
            {
                hash128 = Hash128.Parse(jsonData["hash128"]);
                size = jsonData["size"];
                fileName = jsonData["fileName"];
                uri = url + fileName;
                wasDownloaded = PlayerPrefs.GetInt(fileName + hash128) > 0;
                foreach (JSONData one in jsonData["assets"].Value as List<JSONData>)
                    assets.Add(one.ToString().ToLower(), true);
                foreach (JSONData one in jsonData["dependencies"].Value as List<JSONData>)
                    _dependencies.Add(one.ToString().ToLower());
                Debug.Log($"fileName={fileName},uri={url},size={size},hash128={hash128},wasDownloaded={wasDownloaded}");
            }
            public List<string> _dependencies = new List<string>();
            public void CheckDependencies()
            {
                if (_dependencies.Count > 0)
                {
                    foreach(string one in _dependencies)
                    {
                        if (instance.assetBundles.TryGetValue(one, out MyAssetBundle mab))
                            dependencies.Add(mab);
                    }
                    _dependencies = null;
                }
            }
            public void RecheckWasDownloaded()
            {
                wasDownloaded = PlayerPrefs.GetInt(fileName + hash128) > 0;
            }
            public enum State
            {
                None,
                Loading,
                Loaded,
                Error
            }
            public State state = State.None;
            public AssetBundle ab = null;
            public string fileName;
            public string uri;
            public Hash128 hash128;
            public ulong size;
            public bool wasDownloaded;
            public UnityWebRequest uwr = null;
            public Dictionary<string, bool> assets = new Dictionary<string, bool>();
            public List<MyAssetBundle> dependencies = new List<MyAssetBundle>();
            public void Release(bool unloadAllLoadedObjects)
            {
                if (ab != null)
                {
                    Debug.Log($"Release {unloadAllLoadedObjects}, state={state}, ab={ab}");
                    ab.Unload(unloadAllLoadedObjects);
                }
            }
            public T LoadAsset<T>(string nameWithSuffix) where T : UnityEngine.Object
            {
                if (ab != null)
                {
                    return ab.LoadAsset<T>(nameWithSuffix);
                }
                return null;
            }
            public bool IsLoaded(MyAssetBundle mab = null)
            {
                if (state == State.None || state == State.Error)
                {
                    instance.StartCoroutine(instance.CoroutineLoad(this));
                    return false;
                }
                if (state == State.Loading)
                    return false;
                foreach(MyAssetBundle one in dependencies)
                {
                    if (mab != one && !one.IsLoaded(this))
                        return false;
                }
                return true;
            }
        }
        Dictionary<string, MyAssetBundle> assetBundles = new Dictionary<string, MyAssetBundle>();
        static ResourceManager instance;
        IEnumerator CoroutineLoad<T>(MyAssetBundle mab, string nameWithSuffix, MyAssetBundleRequest<T> mabr) where T : UnityEngine.Object
        {
            if (mab.state == MyAssetBundle.State.None || mab.state == MyAssetBundle.State.Error)
            {
                yield return StartCoroutine(CoroutineLoad(mab));
            }
            while (!mab.IsLoaded())
                yield return null;
            if (mab.ab != null)
            {
                if (!nameWithSuffix.EndsWith(".unity"))
                {
                    AssetBundleRequest ret = mab.ab.LoadAssetAsync<T>(nameWithSuffix);
                    yield return ret;
                    mabr.asset = ret.asset as T;
                }
                else
                {
                    yield return null;
                }
            }
            else
                mabr.error = "AssetBundle instance null";
            mabr.IsDone = true;
        }
        int loadingCount = 0;
        IEnumerator CoroutineLoad(MyAssetBundle mab)
        {
            if (mab.state == MyAssetBundle.State.Loading
                || mab.state == MyAssetBundle.State.Loaded)
                yield break;
            mab.state = MyAssetBundle.State.Loading;
            loadingCount++;
            foreach (MyAssetBundle one in mab.dependencies)
            {
                if (one.state == MyAssetBundle.State.None || one.state == MyAssetBundle.State.Error)
                    StartCoroutine(CoroutineLoad(one));
            }
            using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(mab.uri, mab.hash128))
            {
                mab.uwr = uwr;
                //(uwr.downloadHandler as DownloadHandlerAssetBundle).autoLoadAssetBundle = true;
                yield return uwr.SendWebRequest();
                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Download {mab.uri} fail : {uwr.error}");
                    mab.state = MyAssetBundle.State.Error;
                }
                else
                {
                    //AssetBundleCreateRequest abcr = AssetBundle.LoadFromMemoryAsync(uwr.downloadHandler.data);
                    //Debug.Log($"{mab.fileName} : abcr.allowSceneActivation 1 = {abcr.allowSceneActivation}");
                    //yield return abcr;
                    //Debug.Log($"{mab.fileName} : abcr.allowSceneActivation 2 = {abcr.allowSceneActivation}");
                    //mab.ab = abcr.assetBundle;
                    mab.ab = (uwr.downloadHandler as DownloadHandlerAssetBundle).assetBundle;//DownloadHandlerAssetBundle.GetContent(uwr);
                    DontDestroyOnLoad(mab.ab);
                    mab.state = MyAssetBundle.State.Loaded;
                    Debug.Log($"Download or Load '{mab.uri}' success");
                    PlayerPrefs.SetInt(mab.fileName + mab.hash128, 1);
                    PlayerPrefs.Save();
                }
                mab.uwr = null;
            }
            loadingCount--;
        }
        IEnumerator CoroutineLoadAll()
        {
            state = State.LoadingAssetBundle;
            Queue<MyAssetBundle> temps = new Queue<MyAssetBundle>();
            string codeAB = "code.ab";
            if (assetBundles.ContainsKey(codeAB))
                temps.Enqueue(assetBundles[codeAB]);
            foreach (var one in assetBundles)
            {
                if (one.Key != codeAB)
                    temps.Enqueue(one.Value);
            }
            do
            {
                while (temps.Count > 0 && loadingCount < 5)
                {
                    MyAssetBundle one = temps.Dequeue();
                    StartCoroutine(CoroutineLoad(one));
                }
                yield return new WaitForSeconds(0.1f);
            } while (temps.Count > 0 || loadingCount > 0);
            state = State.AllAssetBundleLoaded;
        }
        IEnumerator CoroutineLoad(string configJsonUrl, bool autoLoadAssetBundle)
        {
            state = State.LoadingConfig;
            Debug.Log($"ResourceManager.CoroutineLoad(\"{configJsonUrl}\",{autoLoadAssetBundle})");
            JSONData jsonData;
            //Load config
            using (UnityWebRequest uwr = UnityWebRequest.Get(configJsonUrl))
            {
                yield return uwr.SendWebRequest();
                if (string.IsNullOrEmpty(uwr.error))
                    jsonData = KissJson.ToJSONData(uwr.downloadHandler.text);
                else
                {
                    Debug.LogError($"ResourceManager.CoroutineLoad : load from : '{configJsonUrl}' with error : '{uwr.error}'");
                    state = State.LoadingConfigError;
                    yield break;
                }
            }
            state = State.LoadingConfigDone;
            //Analyzing config
            string url = ReplaceStreamingAssets(jsonData["url"]);
            foreach(JSONData one in jsonData["files"].Value as List<JSONData>)
            {
                assetBundles.Add(one["fileName"], new MyAssetBundle(one, url));
            }
            foreach (MyAssetBundle one in assetBundles.Values)
                one.CheckDependencies();
            state = State.ConfigDone;
            //Load all AssetBundle
            if (autoLoadAssetBundle)
                yield return StartCoroutine(CoroutineLoadAll());
        }
        void OnDestroy()
        {
            UnLoadAll(true);
        }
        #endregion
    }
}

