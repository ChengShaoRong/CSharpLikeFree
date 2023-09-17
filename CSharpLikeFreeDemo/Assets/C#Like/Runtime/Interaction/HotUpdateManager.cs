/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using CSharpLike.Internal;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace CSharpLike
{
    [HelpURL("https://www.csharplike.com/HotUpdateManager.html")]
    public class HotUpdateManager : MonoBehaviour
	{
		public static HotUpdateManager instance = null;
        /// <summary>
        /// current virtual machine
        /// </summary>
		public static CSL_VirtualMachine vm;
        /// <summary>
        /// Load hot update script 
        /// </summary>
        public static bool Load(byte[] buff)
        {
            vm = new CSL_VirtualMachine();
            RegisterBuildIn();
            return vm.Load(buff);
		}

        public class WaitForLoad : CustomYieldInstruction
		{
			public bool success = false;
			public WaitForLoad(byte[] buff)
            {
#if !UNITY_WEBGL
				Task.Run(() =>
				{
#endif
					try
					{
						success = Load(buff);
					}
					catch (Exception e)
					{
						Debug.LogError("HotUpdateManager.LoadAsync error:" + e);
					}
					finally
					{
						isDone = true;
					}
#if !UNITY_WEBGL
				});
#endif
			}
			bool isDone = false;
            public override bool keepWaiting => !isDone;
        }

		public static WaitForLoad LoadAsync(byte[] buff)
        {
			return new WaitForLoad(buff);
        }

        void Start()
		{
			instance = this;
            ClearAllHotUpdatePrefabs();
			DontDestroyOnLoad(gameObject);

            if (UnityEngine.Random.Range(1, 2) == 0)//condition alway be false 
                ForAOT();//Won't reach here!Don't remove it because it's for not stripping in IL2CPP            
        }
        public static void ClearAllHotUpdatePrefabs()
        {
            foreach (HotUpdateBehaviour one in allHotUpdatePrefabs.Values)
            {
                try
                {
                    if (one != null && one.gameObject != null)
                        Destroy(one.gameObject);
                }
                catch
                { }
            }
            allHotUpdatePrefabs.Clear();
        }
        void Update()
        {
            Simulation.Tick();
        }

        public CSL_TypeBase GetScriptType(string scriptTypeName, out CSL_Content content)
		{
			CSL_TypeBase t = vm.GetTypeByKeywordQuiet(scriptTypeName);
			if (t != null)
			{
				content = vm.CreateContent();
				return t;
			}
			do
            {
                int i = scriptTypeName.LastIndexOf(".");
				if (i > 0)
				{
					scriptTypeName = scriptTypeName.Substring(i + 1);
					t = vm.GetTypeByKeywordQuiet(scriptTypeName);
					if (t != null)
					{
						content = vm.CreateContent();
						return t;
					}
				}
				else
					break;
            } while (true);
			Debug.LogError("GetScriptType:not found:" + scriptTypeName);
			content = null;
			return null;
        }
        public CSL_TypeBase GetScriptType(string scriptTypeName)
        {
            CSL_TypeBase t = vm.GetTypeByKeywordQuiet(scriptTypeName);
            if (t != null)
            {
                return t;
            }
            do
            {
                int i = scriptTypeName.LastIndexOf(".");
                if (i > 0)
                {
                    scriptTypeName = scriptTypeName.Substring(i + 1);
                    t = vm.GetTypeByKeywordQuiet(scriptTypeName);
                    if (t != null)
                    {
                        return t;
                    }
                }
                else
                    break;
            } while (true);
            Debug.LogError("GetScriptType:not found:" + scriptTypeName);
            return null;
        }

		/// <summary>
		/// Register type for hot update script
		/// </summary>
		public static void RegisterType(Type type, string keyword)
		{
			vm.RegType(RegHelper_Type.MakeType(type, keyword));
        }
        public static void RegisterTypeEx(Type type)
        {
			RegHelper_Type iType = RegHelper_Type.MakeType(type, type.FullName);
			vm.RegType(iType);
			if (type.FullName[type.FullName.Length - 2] == '`' && !vm.typess.ContainsKey(type.FullName.Substring(0, type.FullName.Length - 2)))
				vm.typess[type.FullName.Substring(0, type.FullName.Length - 2)] = iType;
        }

		/// <summary>
		/// Register the build-in type.
		/// We are have support automatic regist,so don't need to register here.
		/// You can IGNORE this function.
		/// </summary>
		public static void RegisterBuildIn()
		{
            RegisterTypeEx(typeof(KeyValuePair<,>));
            RegisterTypeEx(typeof(List<>));
            RegisterTypeEx(typeof(Queue<>));
            RegisterTypeEx(typeof(Stack<>));
            RegisterTypeEx(typeof(Dictionary<,>));
            RegisterTypeEx(typeof(SortedDictionary<,>));
            RegisterTypeEx(typeof(IList<>));
            RegisterTypeEx(typeof(IDictionary<,>));
            RegisterTypeEx(typeof(Nullable<>));
            RegisterTypeEx(typeof(Task<>));
            RegisterTypeEx(typeof(ResourceManager.MyAssetBundleRequest<>));
            Simulation.Clear(true);
        }

		public virtual Type GetTypeEx(string typeName)
		{
			return GetTypeBase(typeName);
		}

		public Type GetTypeBase(string typeName)
		{
			return Type.GetType(typeName);
		}
		/// <summary>
		/// all instances load by HotUpdateBehaviour.Show()
		/// </summary>
		private static Dictionary<string, HotUpdateBehaviour> allHotUpdatePrefabs = new Dictionary<string, HotUpdateBehaviour>();

		/// <summary>
		/// Get hot update instance by name
		/// </summary>
		/// <returns>The hot update instance.</returns>
		/// <param name=""Name">file name</param>
		public static HotUpdateBehaviour GetHotUpdate(string fileName)
		{
			HotUpdateBehaviour hub = null;
			if (allHotUpdatePrefabs.ContainsKey(fileName))
			{
				try
				{
					hub = allHotUpdatePrefabs[fileName];
					if (hub != null && hub.gameObject != null)
						return hub;
				}
				catch
				{
					allHotUpdatePrefabs.Remove(fileName);
					hub = null;
				}
			}
			return hub;
		}
        /// <summary>
        /// Show a prefab with HotUpdateBehaviour component
        /// Recommend use addressables manager it.
        /// </summary>
        /// <param name="fileName">the file name of the path</param>
        /// <param name="callback">callback for you process after load finished</param>
        /// <param name="parent">the parent node</param>
        public static void Show(string fileName, UnityAction<object> callback = null,Transform parent =null)
		{
			Show(fileName, callback, parent, Vector3.zero);
        }
		/// <summary>
		/// Show a prefab with HotUpdateBehaviour component.
		/// Recommend use addressables manager it.
		/// </summary>
		/// <param name="fileName">the file name of the prefab</param>
		/// <param name="callback">callback for you process after load finished</param>
		/// <param name="parent">the parent node</param>
		/// <param name="pos">local position</param>
		public static void Show(string fileName, UnityAction<object> callback, Transform parent, Vector3 pos)
		{
			HotUpdateBehaviour hub = GetHotUpdate(fileName);
			if (hub == null)
            {
                var mab = ResourceManager.LoadAssetAsync<GameObject>(fileName);
                mab.OnCompleted += asset =>
                {
                    if (asset != null)
                    {
                        asset = Instantiate(asset, pos, Quaternion.identity, parent);
                        if (asset != null)
                        {
                            hub = asset.GetComponent<HotUpdateBehaviour>();
                            if (hub != null)
                            {
                                hub.transform.localPosition = pos;
                                allHotUpdatePrefabs.Add(fileName, hub);
                                callback?.Invoke(hub.ScriptInstance);
                            }
                            else
                            {
                                Debug.LogError("GameObject " + asset + " not exist HotUpdateBehaviour Component");
                            }
                        }
                    }
                };
                mab.OnError += (error) =>
                {
                    Debug.LogError("Load '" + fileName + "' error : " + error);
                };
            }
            else
            {
                hub.transform.localPosition = pos;
                hub.gameObject.SetActive(true);
                callback?.Invoke(hub.ScriptInstance);
            }
		}
		/// <summary>
		/// Hide the hot update instance
		/// </summary>
		/// <param name="fileName">the file name of the prefab</param>
		/// <param name="bDelete">delete it or keep it for cache(just hide it and show it when call function Show())</param>
		public static void Hide(string fileName, bool bDelete = false)
		{
			HotUpdateBehaviour hub = GetHotUpdate(fileName);
			if (hub != null)
			{
				if (bDelete)
				{
					allHotUpdatePrefabs.Remove(fileName);
					Destroy(hub.gameObject);
				}
				else
				{
					hub.gameObject.SetActive(false);
				}
			}
		}

		/// <summary>
		/// Check whether the hot update instance is active
		/// </summary>
		/// <param name="fileName">the file name of the prefab</param>
		public static bool IsActive(string fileName)
		{
			HotUpdateBehaviour hub = GetHotUpdate(fileName);
			if (hub != null)
				return hub.gameObject.activeSelf;
			return false;
        }

		/// <summary>
		/// Check whether the hot update instance is not active
		/// </summary>
		/// <param name="fileName">the file name of the prefab</param>
		public static bool IsNotActive(string fileName)
		{
			return !IsActive(fileName);
        }

		/// <summary>
		/// Check whether the hot update instance is exist
		/// </summary>
		/// <param name="fileName">the file name of the prefab</param>
		public static bool IsExist(string fileName)
		{
			return (GetHotUpdate(fileName) != null);
        }

		/// <summary>
		/// Check whether the hot update instance is not exist
		/// </summary>
		/// <param name="fileName">the file name of the prefab</param>
		public static bool IsNotExist(string fileName)
		{
			return (GetHotUpdate(fileName) == null);
		}
		/// <summary>
		/// load asset and callback by UnityAction.
		/// Recommend use addressables manager it.
		/// </summary>
		/// <typeparam name="T">UnityEngine.Object</typeparam>
		/// <param name="fileName">the file name of the prefab</param>
		/// <param name="callback">callback action</param>
		public static void Load<T>(string fileName,UnityAction<T> callback) where T : UnityEngine.Object
        {
            var mab = ResourceManager.LoadAssetAsync<GameObject>(fileName);
            mab.OnCompleted += asset =>
            {
                if (asset != null)
                {
                    callback(asset as T);
                }
                else
                    callback(null);
            };
            mab.OnError += (error) =>
            {
                Debug.LogError("Load '" + fileName + "' error : " + error);
            };
        }
        /// <summary>
        /// New a hot update instance with HotUpdateBehaviour component.
        /// The difference with Show() is NewInstance() you should manager yourself.
        /// Recommend use Addressables manager it.
        /// </summary>
        /// <param name="fileName">the file name of the prefab</param>
        /// <param name="callback">callback for you process after load finished</param>
        /// <param name="parent">the parent node</param>
        public static void NewInstance(string fileName, UnityAction<object> callback, Transform parent)
		{
			NewInstance(fileName, callback, parent, Vector3.zero);
		}
#region intance pool
        static Dictionary<string, Queue<HotUpdateBehaviour>> pools = new Dictionary<string, Queue<HotUpdateBehaviour>>();
		public static HotUpdateBehaviour GetFromPool(string prefabName)
        {
            if (pools.ContainsKey(prefabName))
            {
                Queue<HotUpdateBehaviour> list = pools[prefabName];
                while (list.Count > 0)
                {
                    HotUpdateBehaviour hub = list.Dequeue();
					if (hub != null)//maybe was destroy
                    {
                        if (!hub.gameObject.activeSelf)
                            hub.gameObject.SetActive(true);
                        return hub;
                    }
                }
            }
            return null;
        }
        public static void PushToPool(HotUpdateBehaviour hub)
        {
			if (hub == null)
				return;
			string prefabName = hub.GetString("__prefabName__");
			if (string.IsNullOrEmpty(prefabName))
				return;

            Queue<HotUpdateBehaviour> list = null;
            if (pools.ContainsKey(prefabName))
                list = pools[prefabName];
            else
            {
                list = new Queue<HotUpdateBehaviour>();
                pools.Add(prefabName, list);
            }
            if (hub.gameObject.activeSelf)
                hub.gameObject.SetActive(false);
            list.Enqueue(hub);
        }
        public static void ClearPool()
        {
            foreach (var pool in pools)
            {
                foreach (var hub in pool.Value)
                {
                    if (hub != null)
                    {
                        try
                        {
                            Destroy(hub.gameObject);//maybe crack here if contain OnDestroy()
                        }
                        catch
                        { }
                    }
                }
                pool.Value.Clear();
            }
            pools.Clear();
        }


        public static void ClearPool(string prefabName)
        {
			if (pools.ContainsKey(prefabName))
            {
                Queue<HotUpdateBehaviour> list = pools[prefabName];
                foreach (var hub in list)
                {
                    if (hub != null)
                    {
                        try
                        {
                            Destroy(hub.gameObject);//maybe crack here if contain OnDestroy()
						}
                        catch
                        { }
                    }
                }
                list.Clear();
                pools.Remove(prefabName);
            }
        }
#endregion //IntancePool
        /// <summary>
        /// New a GameObject instance.
        /// Recommend use Addressables manager it.
        /// </summary>
        /// <param name="fileName">the file name of the prefab</param>
        /// <param name="callback">callback for you process after load finished</param>
        /// <param name="parent">the parent node</param>
        /// <param name="pos">local position</param>
        public static void NewInstanceGameObject(string prefabName, UnityAction<GameObject> callback, Transform parent, Vector3 pos)
        {
            //load from Resource folder
            var mab = ResourceManager.LoadAssetAsync<GameObject>(prefabName);
            mab.OnCompleted += asset =>
            {
                if (asset != null)
                {
                    asset = Instantiate(asset, pos, Quaternion.identity, parent);
                    if (asset != null)
                    {
                        asset.transform.localPosition = pos;
                        callback?.Invoke(asset);
                    }
                }
            };
            mab.OnError += (error) =>
            {
                Debug.LogError("Load '" + prefabName + "' error : " + error);
            };
        }
        /// <summary>
        /// New a hot update instance with HotUpdateBehaviour component.
        /// The difference with Show() is NewInstance() you should manager yourself.
        /// Recommend use Addressables manager it.
        /// </summary>
        /// <param name="fileName">the file name of the prefab</param>
        /// <param name="callback">callback for you process after load finished</param>
        /// <param name="parent">the parent node</param>
        /// <param name="pos">local position</param>
        public static void NewInstance(string prefabName, UnityAction<object> callback, Transform parent, Vector3 pos, bool bPool = true)
		{
			HotUpdateBehaviour hub;
			if (bPool)
            {
				hub = GetFromPool(prefabName);
				if (hub != null)
                {
					hub.transform.localPosition = pos;
                    callback?.Invoke(hub.ScriptInstance);
                    return;
                }
			}
            //load from Resource folder
            var mab = ResourceManager.LoadAssetAsync<GameObject>(prefabName);
            mab.OnCompleted += asset =>
            {
                if (asset != null)
                {
                    asset = Instantiate(asset, pos, Quaternion.identity, parent);
                    if (asset != null)
                    {
                        hub = asset.GetComponent<HotUpdateBehaviour>();
                        if (hub != null)
                        {
                            if (bPool)
                                hub.SetString("__prefabName__", prefabName);
                            hub.transform.localPosition = pos;
                            callback?.Invoke(hub.ScriptInstance);
                        }
                        else
                        {
                            Debug.LogError("GameObject " + asset + " not exist HotUpdateBehaviour Component");
                        }
                    }
                }
            };
            mab.OnError += (error) =>
            {
                Debug.LogError("Load '" + prefabName + "' error : " + error);
            };
        }
		/// <summary>
		/// Easy way for delay call function of the hot update script with no param.
		/// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
		/// </summary>
		/// <param name="hub">target instance of HotUpdateBehaviour</param>
		/// <param name="funName">function name</param>
		/// <param name="delay">delay time by seconds</param>
		public void MemberCallDelay(HotUpdateBehaviour hub, string funName, float delay)
		{
			StartCoroutine(CorMemberCallDelay(hub, funName, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName);
		}

        /// <summary>
        /// Easy way for delay call function of the hot update script with 1 param.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1);
		}

        /// <summary>
        /// Easy way for delay call function of the hot update script with 2 params.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, v2, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1, v2);
        }

        /// <summary>
        /// Easy way for delay call function of the hot update script with 3 params.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, v2, v3, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1, v2, v3);
        }
        /// <summary>
        /// Easy way for delay call function of the hot update script with 4 params.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, v2, v3, v4, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1, v2, v3, v4);
        }
        /// <summary>
        /// Easy way for delay call function of the hot update script with 5 params.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, v2, v3, v4, v5, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1, v2, v3, v4, v5);
        }
        /// <summary>
        /// Easy way for delay call function of the hot update script with 6 params.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, v2, v3, v4, v5, v6, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1, v2, v3, v4, v5, v6);
        }
        /// <summary>
        /// Easy way for delay call function of the hot update script with 7 params.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="v7">param 7</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, v2, v3, v4, v5, v6, v7, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7);
        }
        /// <summary>
        /// Easy way for delay call function of the hot update script with 8 params.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="v7">param 7</param>
        /// <param name="v8">param 8</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, v2, v3, v4, v5, v6, v7, v8, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8);
        }
        /// <summary>
        /// Easy way for delay call function of the hot update script with 9 params.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="v7">param 7</param>
        /// <param name="v8">param 8</param>
        /// <param name="v9">param 9</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9);
        }
        /// <summary>
        /// Easy way for delay call function of the hot update script with 10 params.
        /// The target instance of HotUpdateBehaviour may be is not active,you can't use its own MemberCallDelay().
        /// </summary>
        /// <param name="hub">target instance of HotUpdateBehaviour</param>
        /// <param name="funName">function name</param>
        /// <param name="v1">param 1</param>
        /// <param name="v2">param 2</param>
        /// <param name="v3">param 3</param>
        /// <param name="v4">param 4</param>
        /// <param name="v5">param 5</param>
        /// <param name="v6">param 6</param>
        /// <param name="v7">param 7</param>
        /// <param name="v8">param 8</param>
        /// <param name="v9">param 9</param>
        /// <param name="v10">param 10</param>
        /// <param name="delay">delay time by seconds</param>
        public void MemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, float delay)
		{
			instance.StartCoroutine(CorMemberCallDelay(hub, funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, delay));
		}
		private IEnumerator CorMemberCallDelay(HotUpdateBehaviour hub, string funName, object v1, object v2, object v3, object v4, object v5, object v6, object v7, object v8, object v9, object v10, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (hub != null && hub.gameObject.activeInHierarchy)
				hub.MemberCall(funName, v1, v2, v3, v4, v5, v6, v7, v8, v9, v10);
		}
		static object threadLock = new object();
		static Queue<JSONData> threadObjs = new Queue<JSONData>();
		float deltaTime = 0f;
		void LateUpdate()
        {
            if (Time.realtimeSinceStartup - deltaTime > 0.1f)//we call this for 10 FPS only. you may modify the value depend on your business.
			{
				deltaTime = Time.realtimeSinceStartup;

				JSONData obj = null;
                lock (threadLock)
                {
					if (threadObjs.Count > 0)
                    {
						obj = threadObjs.Dequeue();
						if (threadObjs.Count > 0)//don't wait too much time if still have some work to do. We process one object in one frame
							deltaTime = 0f;
					}
                }
				if (obj != null)
                {
					HotUpdateBehaviour behaviour = obj.GetObjectExtern("__behaviour__") as HotUpdateBehaviour;
					try
                    {
                        if (behaviour != null && behaviour.gameObject != null)//the HotUpdateBehaviour may be destroyed while processing in thread!
						{
							ParameterizedThreadStart funcThreadDone = obj.GetObjectExtern("__funcThreadDone__") as ParameterizedThreadStart;
							if (funcThreadDone != null)
								funcThreadDone(obj);
						}
                    }
					catch(Exception err)
                    {
						Debug.LogError("error:"+ err);
                    }
				}
            }
        }
		public static void OnThreadDone(JSONData obj)
        {
            lock (threadLock)
            {
				threadObjs.Enqueue(obj);
			}
        }

		/// <summary>
		/// New a thread without param
		/// </summary>
		public static Thread NewThread(ThreadStart threadStart)
        {
			return new Thread(threadStart);
        }
        /// <summary>
        /// New a thread without a param
        /// </summary>
        public static Thread NewThreadWithParam(ParameterizedThreadStart parameterizedThreadStart)
        {
            return new Thread(parameterizedThreadStart);
        }
        /// <summary>
        /// Create a thread without param, and start it
        /// </summary>
        /// <param name="funcThread">the function will run in thread</param>
        public static Thread CreateThread(ThreadStart funcThread)
        {
            Thread thread = new Thread(funcThread);
            thread.Start();
            return thread;
        }
		/// <summary>
		/// Create a thread with param, and start it
		/// </summary>
		/// <param name="funcThread">the function will run in thread</param>
		/// <param name="jsonData">the json data object as param</param>
		/// <param name="behaviour">the HotUpdateBehaviour instance</param>
		/// <param name="funcThreadDone">the function will call back to the HotUpdateBehaviour instance</param>
		public static Thread CreateThread(ParameterizedThreadStart funcThread, JSONData jsonData, HotUpdateBehaviour behaviour = null, ParameterizedThreadStart funcThreadDone = null)
        {
			Thread thread = new Thread(funcThread);
			jsonData.SetObjectExtern("__behaviour__", behaviour);
            jsonData.SetObjectExtern("__funcThreadDone__", funcThreadDone);
			thread.Start(jsonData);
			return thread;
        }
		/// <summary>
		/// Add event trigger to target
		/// </summary>
		/// <param name="go">target GameObject instance</param>
		/// <param name="eventID">event trigger type</param>
		/// <param name="call">event function</param>
        public static void AddEventTrigger(GameObject go, EventTriggerType eventID, UnityAction<BaseEventData> call)
        {
            if (go == null)
            {
                Debug.LogError("AddEventTrigger:GameObject null");
                return;
            }
            EventTrigger trigger = go.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = go.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = eventID;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(call);
			trigger.triggers.Add(entry);
        }
        /// <summary>
        /// Remove event trigger from target
        /// </summary>
        /// <param name="go">target GameObject instance</param>
        /// <param name="eventID">event trigger type</param>
        public static void RemoveEventTrigger(GameObject go, EventTriggerType eventID)
        {
			if (go != null)
            {
                EventTrigger trigger = go.GetComponent<EventTrigger>();
				if (trigger != null && trigger.triggers != null && trigger.triggers.Count > 0)
                {
					foreach(var one in trigger.triggers)
                    {
						if (one != null && one.eventID == eventID)
                        {
							trigger.triggers.Remove(one);
							return;
						}
                    }
                }
            }
        }
        /// <summary>
        /// Remove all events trigger from target
        /// </summary>
        /// <param name="go">target GameObject instance</param>
        public static void RemoveAllEventTrigger(GameObject go)
        {
            if (go != null)
            {
                EventTrigger trigger = go.GetComponent<EventTrigger>();
                if (trigger != null && trigger.triggers != null && trigger.triggers.Count > 0)
                {
					trigger.triggers.Clear();
				}
            }
        }
		static Dictionary<string, Dictionary<string, CSL_Content.Value>> enumsCache = new Dictionary<string, Dictionary<string, CSL_Content.Value>>();
		static Dictionary<string, CSL_Content.Value> GetEnumCache(string strEnumName)
        {
            Dictionary<string, CSL_Content.Value> enums;
            if (!enumsCache.TryGetValue(strEnumName, out enums))
            {
                CSL_TypeBase t = instance.GetScriptType(strEnumName);
                if (t != null)
                {
                    SType st = t.type;
                    if (st != null)
                    {
                        enums = ((SType)t.type).staticMemberInstance;
						if (enums == null)
							enums = ((SType)t.type).GetStaticMemberInstance();
                    }
                }
				enumsCache[strEnumName] = enums;
			}
			return enums;
		}
        static string ConvertEnumString(string strEnumName, object value)
        {
			if (value == null)
				return "null";
			Dictionary<string, CSL_Content.Value> enums = GetEnumCache(strEnumName);
			if (enums != null)
            {
                foreach (var one in enums)
                {
                    if (value.Equals(one.Value.value))
                        return one.Key;
                }
            }
            return value.ToString();
        }
		/// <summary>
		/// Convert enum value to string type
		/// </summary>
		/// <param name="type">Type of the enum, you must use typeof(XXXX)</param>
		/// <param name="value">the enum instance</param>
        public static string ConvertEnumString(Type type, object value)
        {
            if (value == null)
                return "null";
			return value.ToString();
        }
        public static string ConvertEnumString(SType type, object value)
        {
            return ConvertEnumString(type.FullName, value);
		}
		/// <summary>
		/// Convert enum value to number type
		/// </summary>
		/// <param name="type">Type of the enum, you must use typeof(XXXX)</param>
		/// <param name="value">the enum instance</param>
		/// <returns></returns>
		public static object ConvertEnumNumber(Type type, string value)
		{
			if (type == null)
				return 0;
			return Enum.Parse(type, value);
		}
		public static object ConvertEnumNumber(SType type, string value)
		{
			if (type == null)
				return 0;
			Dictionary<string, CSL_Content.Value> enums = GetEnumCache(type.FullName);
			if (enums != null)
			{
				if (enums.ContainsKey(value))
					return enums[value].value;
			}
			return 0;
		}
		/// <summary>
		/// register for the internal Ahead-of-time.
		/// never call this.
		/// </summary>
		public void ForAOT()
        {
#pragma warning disable CS0168
            HotUpdateBehaviour[] v1;
			Dictionary<int, SInstance> v2;
            Dictionary<string, SInstance> v3;
            SortedDictionary<int, SInstance> v4;
			SortedDictionary<string, SInstance> v5;
			SInstance[] v6;
            SInstance[,] v7;
            new List<HotUpdateBehaviour>();
            new Queue<byte>();
            new Queue<sbyte>();
            new Queue<short>();
            new Queue<ushort>();
            new Queue<int>();
            new Queue<uint>();
            new Queue<long>();
            new Queue<ulong>();
            new Queue<double>();
            new Queue<float>();
            new Queue<bool>();
            new Queue<string>();
            new Stack<char>();
            new Stack<byte>();
            new Stack<sbyte>();
            new Stack<short>();
            new Stack<ushort>();
            new Stack<int>();
            new Stack<uint>();
            new Stack<long>();
            new Stack<ulong>();
            new Stack<double>();
            new Stack<float>();
            new Stack<bool>();
            new Stack<string>();
            new Stack<char>();
            new Queue<SInstance>();
            new Stack<SInstance>();
            new Dictionary<int, byte[]>();
            new Dictionary<string, byte[]>();
            new Dictionary<string, HotUpdateBehaviour>();
            new Dictionary<int, HotUpdateBehaviour>();
            new Dictionary<byte, SInstance>();
            new Dictionary<sbyte, SInstance>();
            new Dictionary<short, SInstance>();
            new Dictionary<ushort, SInstance>();
            new Dictionary<int, SInstance>();
            new Dictionary<uint, SInstance>();
            new Dictionary<long, SInstance>();
            new Dictionary<ulong, SInstance>();
            new Dictionary<double, SInstance>();
            new Dictionary<float, SInstance>();
            new Dictionary<bool, SInstance>();
            new Dictionary<char, SInstance>();
            new Dictionary<string, SInstance>();
            new Dictionary<byte, SInstance[]>();
            new Dictionary<sbyte, SInstance[]>();
            new Dictionary<short, SInstance[]>();
            new Dictionary<ushort, SInstance[]>();
            new Dictionary<int, SInstance[]>();
            new Dictionary<uint, SInstance[]>();
            new Dictionary<long, SInstance[]>();
            new Dictionary<ulong, SInstance[]>();
            new Dictionary<double, SInstance[]>();
            new Dictionary<float, SInstance[]>();
            new Dictionary<bool, SInstance[]>();
            new Dictionary<char, SInstance[]>();
            new Dictionary<string, SInstance[]>();
            new SortedDictionary<byte, SInstance>();
            new SortedDictionary<sbyte, SInstance>();
            new SortedDictionary<short, SInstance>();
            new SortedDictionary<ushort, SInstance>();
            new SortedDictionary<int, SInstance>();
            new SortedDictionary<uint, SInstance>();
            new SortedDictionary<long, SInstance>();
            new SortedDictionary<ulong, SInstance>();
            new SortedDictionary<double, SInstance>();
            new SortedDictionary<float, SInstance>();
            new SortedDictionary<bool, SInstance>();
            new SortedDictionary<char, SInstance>();
            new SortedDictionary<string, SInstance>();
            new SortedDictionary<byte, SInstance[]>();
            new SortedDictionary<sbyte, SInstance[]>();
            new SortedDictionary<short, SInstance[]>();
            new SortedDictionary<ushort, SInstance[]>();
            new SortedDictionary<int, SInstance[]>();
            new SortedDictionary<uint, SInstance[]>();
            new SortedDictionary<long, SInstance[]>();
            new SortedDictionary<ulong, SInstance[]>();
            new SortedDictionary<double, SInstance[]>();
            new SortedDictionary<float, SInstance[]>();
            new SortedDictionary<bool, SInstance[]>();
            new SortedDictionary<char, SInstance[]>();
            new SortedDictionary<string, SInstance[]>();
            new Dictionary<SInstance, byte>();
			new Dictionary<SInstance, sbyte>();
			new Dictionary<SInstance, short>();
			new Dictionary<SInstance, ushort>();
			new Dictionary<SInstance, int>();
			new Dictionary<SInstance, uint>();
			new Dictionary<SInstance, long>();
			new Dictionary<SInstance, ulong>();
			new Dictionary<SInstance, double>();
			new Dictionary<SInstance, float>();
			new Dictionary<SInstance, bool>();
			new Dictionary<SInstance, char>();
			new Dictionary<SInstance, string>();
            new Dictionary<SInstance, HotUpdateBehaviour>();
            new Dictionary<SInstance, SInstance>();
            new SortedDictionary<SInstance, byte>();
            new SortedDictionary<SInstance, sbyte>();
            new SortedDictionary<SInstance, short>();
            new SortedDictionary<SInstance, ushort>();
            new SortedDictionary<SInstance, int>();
            new SortedDictionary<SInstance, uint>();
            new SortedDictionary<SInstance, long>();
            new SortedDictionary<SInstance, ulong>();
            new SortedDictionary<SInstance, double>();
            new SortedDictionary<SInstance, float>();
            new SortedDictionary<SInstance, bool>();
            new SortedDictionary<SInstance, char>();
            new SortedDictionary<SInstance, string>();
            new SortedDictionary<SInstance, SInstance>();
            new SortedDictionary<HotUpdateBehaviour, SInstance>();
            new SortedDictionary<SInstance, byte[]>();
            new SortedDictionary<SInstance, sbyte[]>();
            new SortedDictionary<SInstance, short[]>();
            new SortedDictionary<SInstance, ushort[]>();
            new SortedDictionary<SInstance, int[]>();
            new SortedDictionary<SInstance, uint[]>();
            new SortedDictionary<SInstance, long[]>();
            new SortedDictionary<SInstance, ulong[]>();
            new SortedDictionary<SInstance, double[]>();
            new SortedDictionary<SInstance, float[]>();
            new SortedDictionary<SInstance, bool[]>();
            new SortedDictionary<SInstance, char[]>();
            new SortedDictionary<SInstance, string[]>();
            new SortedDictionary<SInstance, SInstance[]>();
            new SortedDictionary<HotUpdateBehaviour, SInstance[]>();
            new AheadOfTime();
#pragma warning restore CS0168
        }
#if UNITY_EDITOR
        /// <summary>
        /// It's for test
        /// </summary>
        public static void DebugStep()
        {
			Debug.Log("DebugStep");
        }
#endif
        /// <summary>
        /// Get platform string base on current platform. e.g. Android, iOS, WebGL...
        /// </summary>
        public static string GetPlatformString()
        {
#if UNITY_STANDALONE_OSX
            return "StandaloneOSX";
#elif UNITY_STANDALONE_WIN
            return "StandaloneWindows";
#elif UNITY_STANDALONE_LINUX
            return "StandaloneLinux64";
#elif UNITY_WII
            return "WiiU";
#elif UNITY_IOS
            return "iOS";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_PS4
            return "PS4";
#elif UNITY_PS5
            return "PS5";
#elif UNITY_XBOXONE
            return "XboxOne";
#elif UNITY_TIZEN
            return "Tizen";
#elif UNITY_TVOS
            return "tvOS";
#elif UNITY_WSA || UNITY_WSA_10_0 || UNITY_WINRT
            return "WSAPlayer";
#elif UNITY_WEBGL
            return "WebGL";
#else
            return "";
#endif
        }
        public static JSONData Games { get; set; } = null;
#if UNITY_EDITOR
        /// <summary>
        /// This function using in editor for you play the scene directly, and the code or AssetBundle may be not initialized
        /// </summary>
        public static void InitForEditor()
        {
            if (instance == null)
            {
                instance = (new GameObject("HotUpdateManager")).AddComponent<HotUpdateManager>();
                //In editor, will load the hot update binary file directly
                string strFileName = Application.dataPath + "/C#Like/output/code.bytes";
                if (File.Exists(strFileName))
                {
                    Debug.Log($"Try load {strFileName} in editor, success = {Load(File.ReadAllBytes(strFileName))}");
                }
                //In editor, will load AssetBundle here DELAY. you CAN'T using AssetBundle immediately.
                string strFile = Application.dataPath + "/C#Like/Editor/Config.txt";
                string strProductName = "";
                if (File.Exists(strFile))
                {
                    string[] lines = File.ReadAllLines(strFile);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("ProductName:"))
                        {
                            strProductName = line.Substring(12);
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(strProductName))
                    strProductName = Application.productName;
                strFile = Path.Combine(Application.streamingAssetsPath, "AssetBundles/" + strProductName + "/" + GetPlatformString() + "/config.json");
                if (File.Exists(strFile))
                    ResourceManager.Init(strFile);
            }
        }
#endif
        /// <summary>
        /// Initialize HotUpdateManager with a URL.
        /// URL priority : 
        /// 1. In UnityEditor, FORCE using 'StreamingAssets/AssetBundles/games[Platform].json' if the 'automatic compile' in C#Like Setting panel was checked.
        /// 2. Using the value of 'url' you just input.
        /// 3. Using the value of 'Download Path' in C#Like Setting panel.
        /// 4. Using 'StreamingAssets/AssetBundles/games[Platform].json', that mean you don't have a server and don't need hot update?
        /// </summary>
        /// <param name="url">The config JSON that contains your games information
        /// </param>
        public static void Init(string url = null)
        {
            if (instance == null)
                instance = (new GameObject("HotUpdateManager")).AddComponent<HotUpdateManager>();
            instance.StartCoroutine(instance.CoroutineLoad(url));
        }
        IEnumerator CoroutineLoad(string url)
        {
            //1 In editor, force using the file in StreamingAssets if isAutoPackScriptWhenPlay was checked!
#if UNITY_EDITOR    
            string strFile = Application.dataPath + "/C#Like/Editor/Config.txt";
            bool isAutoPackScriptWhenPlay = true;
            if (File.Exists(strFile))
            {
                string[] lines = File.ReadAllLines(strFile);
                foreach (string line in lines)
                {
                    if (line.StartsWith("*isAutoPackScriptWhenPlay:"))
                    {
                        isAutoPackScriptWhenPlay = Convert.ToBoolean(line.Substring(26).Trim());
                        break;
                    }
                }
            }
            if (isAutoPackScriptWhenPlay)
                url = "StreamingAssets/AssetBundles/games" + GetPlatformString() + ".json";
#endif
            //2 Using the config file in Resources/CSharpLikeConfig.json
            if (string.IsNullOrEmpty(url))
            {
                TextAsset ta = Resources.Load<TextAsset>("CSharpLikeConfig");
                if (ta != null)
                {
                    JSONData json = KissJson.ToJSONData(ResourceManager.GetUTF8String(ta.bytes));
                    string strPlatform = GetPlatformString();
                    if (json.ContainsKey(strPlatform))
                        url = json[strPlatform];
                }
            }
            //3 Using the file in StreamingAssets
            if (string.IsNullOrEmpty(url))
                url = "StreamingAssets/AssetBundles/games" + GetPlatformString() + ".json";
            if (url.StartsWith("StreamingAssets/"))
                url = Path.Combine(Application.streamingAssetsPath, url.Substring(16));
            do
            {
                Debug.Log($"Requesting {url}");
                using (UnityWebRequest uwr = UnityWebRequest.Get(url))
                {
                    yield return uwr.SendWebRequest();
                    if (string.IsNullOrEmpty(uwr.error))
                    {
                        Games = KissJson.ToJSONData(ResourceManager.GetUTF8String(uwr.downloadHandler.data));
                        break;
                    }
                    else
                    {
                        Debug.LogError($"Requesting {url} with error {uwr.error}");
                        yield return new WaitForSeconds(5f);
                    }
                }
            } while (true);
        }
    }
}

