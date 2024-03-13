//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TreasureBox
{
    /// <summary>
    /// Store global static value
    /// </summary>
	public class Global
    {
        //#region Config
        /// <summary>
        /// HTTP(s) server sign key
        /// </summary>
        static string SignKey = "fdw12wDFoo&9Uh)5sD<Jnia(e5*C8@u!s";
        /// <summary>
        /// HTTP(s) server uri
        /// </summary>
        public static string Url = "https://www.csharplike.com/TreasureBox/";
        //public static string Url = "http://127.0.0.1:9002/";//for local test
        /// <summary>
        /// Config folder of the UI prefab.
        /// start with 'Assets/' and end with '/'
        /// </summary>
        static string mPrefabPath = "Assets/C#Like/HotUpdateResources/TreasureBox/Prefab/";
        /// <summary>
        /// Config folder of the CSV.
        /// start with 'Assets/' and end with '/'
        /// </summary>
        static string mCsvPath = "Assets/C#Like/HotUpdateResources/TreasureBox/CSV/";
        ///// <summary>
        ///// Config folder of the Spine prefab.
        ///// start with 'Assets/' and end with '/'
        ///// </summary>
        //static string mSpinePath = "Assets/C#Like/HotUpdateResources/TreasureBox/Spine/";
        /// <summary>
        /// The TMP_Text name start with this, will be convert to selected Localization string.
        /// </summary>
        static string mTextPrefix = "LT_";
        //#endregion //Config

        public static long GetCurrentTimestamp()
        {
            TimeSpan ts = (DateTime.Now - new DateTime(1970, 1, 1));
            return (long)ts.TotalSeconds;
        }
        /// <summary>
        /// Get a MD5 style UID
        /// </summary>
        public static string GetDeviceUniqueIdentifier()
        {
            return CSL_Utils.GetMD5(SystemInfo.deviceUniqueIdentifier + DateTime.Now.ToString()+UnityEngine.Random.Range(0f, 1000000000f));
        }
        public static string HashPassword(string str)
        {
            return CSL_Utils.GetMD5(str + SignKey);
        }

        /// <summary>
        /// Sign JSONData
        /// </summary>
        public static void Sign(JSONData json)
        {
            SortedDictionary<string, JSONData> sortedDics = new SortedDictionary<string, JSONData>();
            json["_time_"] = GetCurrentTimestamp();
            foreach (var one in json.Value as Dictionary<string, JSONData>)
                sortedDics.Add(one.Key, one.Value);
            string str = "";
            foreach (var one in sortedDics)
            {
                if (str.Length == 0)
                {
                    str = one.Key + "=" + one.Value;
                }
                else
                {
                    str += "&" + one.Key + "=" + one.Value;
                }
            }
            str += SignKey;
            json["_sign_"] = CSL_Utils.GetMD5(str);
        }

        /// <summary>
        /// Localize all the TMP_Text component of this GameObject.
        /// and only effect the name start with specified value, e.g. start with 'LT_'.
        /// Usage: If your UI contain some TMP_Text components need to be convert,
        /// you should call 'Global.LocalizeText(gameObject);' in your Start/Awake function.
        /// </summary>
        /// <param name="go">Specified GameObject</param>
        public static void LocalizeText(GameObject go)
        {
            TMP_Text[] texts = go.GetComponentsInChildren<TMP_Text>(true);
            foreach(TMP_Text text in texts)
            {
                if (text.name.StartsWith(mTextPrefix))
                    text.text = GetString(text.name);
            }
            //We just localize the TMP_Text only, because we suppose not use Text this game.
            //Text[] texts2 = go.GetComponentsInChildren<Text>(true);
            //foreach (Text text in texts2)
            //{
            //    if (text.name.StartsWith(mTextPrefix))
            //        text.text = GetString(text.name);
            //}
        }

        /// <summary>
        /// The recommend server IDs
        /// </summary>
        public static List<int> mRecommendServerIds = new List<int>();
        static int mCurrentSelectServerId = -1;

        public static int GetCurrentSelectServerId()
        {
            if (mCurrentSelectServerId == -1)
            {
                mCurrentSelectServerId = MyPlayerPrefs.GetInt("SelectServerId", 0);
            }
            return mCurrentSelectServerId;
        }
        public static void SetCurrentSelectServerId(int value)
        {
            if (mCurrentSelectServerId != value)
            {
                mCurrentSelectServerId = value;
                MyPlayerPrefs.SetInt("SelectServerId", value);
                MyPlayerPrefs.Save();
            }
        }
        //Free version not support customise get/set implement
        ///// <summary>
        ///// The current selected server ID
        ///// </summary>
        //public static int CurrentSelectServerId
        //{
        //    get
        //    {
        //        if (mCurrentSelectServerId == -1)
        //        {
        //            mCurrentSelectServerId = MyPlayerPrefs.GetInt("SelectServerId", 0);
        //        }
        //        return mCurrentSelectServerId;
        //    }
        //    set
        //    {
        //        if (mCurrentSelectServerId != value)
        //        {
        //            mCurrentSelectServerId = value;
        //            MyPlayerPrefs.SetInt("SelectServerId", value);
        //            MyPlayerPrefs.Save();
        //        }
        //    }
        //}
        static string mLanguage = "";
        public static string GetLanguage()
        {
            if (mLanguage == "")
            {
                mLanguage = MyPlayerPrefs.GetString("SelectLanguage", "");
                if (mLanguage == "")//Use default language
                {
                    if (Application.systemLanguage == SystemLanguage.ChineseSimplified
                        || Application.systemLanguage == SystemLanguage.ChineseTraditional
                        || Application.systemLanguage == SystemLanguage.Chinese)
                        mLanguage = "CN";
                    else
                        mLanguage = "EN";
                }
                Debug.Log("Language is " + mLanguage);
                //Debug.Log($"Language is {mLanguage}");
            }
            return mLanguage;
        }
        public static void SetLanguage(string value)
        {
            if (mLanguage != value)
            {
                mLanguage = value;
                MyPlayerPrefs.SetString("SelectLanguage", value);
                MyPlayerPrefs.Save();
            }
        }
        //Free version not support customise get/set implement
        ///// <summary>
        ///// Current selected language
        ///// </summary>
        //public static string Language
        //{
        //    get
        //    {
        //        if (mLanguage == "")
        //        {
        //            mLanguage = MyPlayerPrefs.GetString("SelectLanguage", "");
        //            if (mLanguage == "")//Use default language
        //            {
        //                switch (Application.systemLanguage)//Suppose we support Chinese and English language
        //                {
        //                    case SystemLanguage.ChineseSimplified:
        //                    case SystemLanguage.ChineseTraditional:
        //                    case SystemLanguage.Chinese:
        //                        mLanguage = "CN";
        //                        break;
        //                    default:
        //                        mLanguage = "EN";
        //                        break;
        //                }
        //            }
        //            Debug.Log($"Language is {mLanguage}");
        //        }
        //        return mLanguage;
        //    }
        //    set
        //    {
        //        if (mLanguage != value)
        //        {
        //            mLanguage = value;
        //            MyPlayerPrefs.SetString("SelectLanguage", value);
        //            MyPlayerPrefs.Save();
        //        }
        //    }
        //}
        /// <summary>
        /// Get localization string
        /// </summary>
        /// <param name="key">The original short string</param>
        public static string GetString(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "";
            return SimpleKissCSV.GetString("LocalizationCsv", key, "content" + GetLanguage(), key);
        }
        /// <summary>
        /// Force quit before show a message box
        /// </summary>
        /// <param name="msg"></param>
        public static void ForceQuit(string msg)
        {
            MessageBox.Show(msg, "", "Quit", () =>
            {
                Application.Quit();
            });
        }
        static Dictionary<string, HotUpdateBehaviour> mCachePanels = new Dictionary<string, HotUpdateBehaviour>();
        /// <summary>
        /// Get a panel with panel name
        /// </summary>
        /// <param name="panelName">The panel name in your prefab folder, case-sensitive</param>
        static HotUpdateBehaviour _GetPanel(string panelName)
        {
            if (mCachePanels.ContainsKey(panelName))
            {
                HotUpdateBehaviour hub = mCachePanels[panelName];
                if (hub != null && hub.gameObject != null)
                    return hub;
            }
            //Free version not support this 'out' and 'try'
            //if (mCachePanels.TryGetValue(panelName, out HotUpdateBehaviour hub))
            //{
            //    try
            //    {
            //        if (hub != null && hub.gameObject != null)
            //            return hub;
            //    }
            //    catch
            //    {
            //        Debug.LogError($"Try accept panel '{panelName}' error, now erase it from cache.");
            //        mCachePanels.Remove(panelName);
            //    }
            //}
            return null;
        }
        /// <summary>
        /// Check whether exist the panel.
        /// </summary>
        /// <param name="panelName">The panel name in your prefab folder, case-sensitive</param>
        public static bool ExistPanel(string panelName)
        {
            return _GetPanel(panelName) != null;
        }
        /// <summary>
        /// Get the panel instance from cache that not removed.
        /// Useage:
        /// LoginPanel loginPanel = Global.GetPanel("LoginPanel") as LoginPanel;
        /// </summary>
        /// <param name="panelName">The panel name in your prefab folder, case-sensitive</param>
        public static object GetPanel(string panelName)
        {
            HotUpdateBehaviour hub = _GetPanel(panelName);
            if (hub != null)
                return hub.ScriptInstance;
            else
                return null;
            //return hub?.ScriptInstance;
        }
        /// <summary>
        /// Get the prefab full name
        /// </summary>
        /// <param name="panelName">The panel name in your prefab folder, case-sensitive, don't end with '.prefab'</param>
        public static string GetPrefabFullName(string prefabName)
        {
            return mPrefabPath + prefabName + ".prefab";
            //Full version
            //return $"{mPrefabPath}{prefabName}.prefab";
        }
        /// <summary>
        /// Get the CSV full name
        /// </summary>
        /// <param name="panelName">The name in your CSV folder, case-sensitive, don't end with '.csv'</param>
        public static string GetCsvFullName(string csvName)
        {
            return mCsvPath + csvName + ".csv";
            //Full version
            //return $"{mCsvPath}{csvName}.csv";
        }
        ///// <summary>
        ///// Get the Spine full name
        ///// </summary>
        ///// <param name="panelName">The spine name in your spine folder, case-sensitive, don't end with '.prefab'</param>
        //public static string GetSpineFullName(string spineName)
        //{
        //    return $"{mSpinePath}{spineName}.prefab";
        //}
        /// <summary>
        /// Get the main root
        /// </summary>
        public static GameObject GetMainRoot()
        {
            GameObject goMainRoot = GameObject.Find("MainRoot");
            if (goMainRoot == null)
                Debug.LogError("Not exist 'MainRoot'");
            return goMainRoot;
        }
        /// <summary>
        /// Show a panel with panel name
        /// </summary>
        /// <param name="panelName">The panel name in your prefab folder, case-sensitive, don't end with '.prefab'</param>
        /// <param name="callback">Callback after script instance</param>
        public static void ShowPanel(string panelName, UnityAction<object> callback)
        {
            HotUpdateBehaviour hub = _GetPanel(panelName);
            if (hub == null)
            {
                string prefabName = mPrefabPath + panelName + ".prefab";
                //Full version
                //string prefabName = $"{mPrefabPath}{panelName}.prefab";
                var req = ResourceManager.LoadAssetAsync<GameObject>(prefabName);
                req.OnCompleted +=
                    (GameObject go) =>
                    {
                        GameObject goMainRoot = GameObject.Find("MainRoot");
                        if (goMainRoot == null)
                        {
                            Debug.LogError("Not exist 'MainRoot'");
                            if (callback != null)
                                callback.Invoke(null);
                            //Full version
                            //callback?.Invoke(null);
                            return;
                        }
                        go = GameObject.Instantiate(go, new Vector3(0f, 0f, 0f), Quaternion.identity);
                        if (go == null)
                        {
                            Debug.LogError("Instantiate '" + prefabName + "' error");
                            if (callback != null)
                                callback.Invoke(null);
                            //Full version
                            //Debug.LogError($"Instantiate '{prefabName}' error");
                            //callback?.Invoke(null);
                            return;
                        }
                        go.transform.SetParent(goMainRoot.transform);
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = Vector3.zero;
                        hub = go.GetComponent<HotUpdateBehaviour>();
                        if (hub != null)
                        {
                            mCachePanels[panelName] = hub;
                            ActivePanel(hub);
                            if (callback != null)
                                callback.Invoke(hub.ScriptInstance);
                            //callback?.Invoke(hub.ScriptInstance);
                        }
                    };
                req.OnError += (string error) =>
                {
                    Debug.LogError("ResourceManager.LoadAssetAsync(\"" + prefabName + "\") occur error : " + error);
                    if (callback != null)
                        callback.Invoke(null);
                    //Full version
                    //Debug.LogError($"ResourceManager.LoadAssetAsync(\"{prefabName}\") occur error : {error}");
                    //callback?.Invoke(null);
                };
            }
            else
            {
                ActivePanel(hub);
                if (callback != null)
                    callback.Invoke(hub.ScriptInstance);
                //callback?.Invoke(hub.ScriptInstance);
            }
        }
        static GameObject GetChildByName(Transform trans, string childName)
        {
            foreach(Transform child in trans)
            {
                if (childName == child.name)
                    return child.gameObject;
            }
            return null;
        }
        static void ActivePanel(HotUpdateBehaviour hub)
        {
            //Active the KissTweenBase if exist
            KissTweenBase ktb = hub.GetComponent<KissTweenBase>();
            if (ktb != null)
            {
                ktb.onFinished = null;
                ktb.PlayForward();
            }
            //Active the KissTweenBase that name 'content' if exist
            GameObject go = GetChildByName(hub.transform, "Content");
            if (go != null)
            {
                ktb = go.GetComponent<KissTweenBase>();
                if (ktb != null)
                {
                    ktb.onFinished = null;
                    ktb.PlayForward();
                }
            }
            //Set this GameObject active
            if (!hub.gameObject.activeSelf)
                hub.gameObject.SetActive(true);
        }
        /// <summary>
        /// Hide the panel with panel name
        /// </summary>
        /// <param name="panelName">The panel name in your prefab folder, case-sensitive</param>
        /// <param name="bDelete">Whether delete that panel or just disactive it.</param>
        public static void HidePanel(string panelName, bool bDelete)
        {
            HotUpdateBehaviour hub = _GetPanel(panelName);
            if (hub != null)
            {
                List<KissTweenBase> tweens = new List<KissTweenBase>();
                KissTweenBase ktb = hub.GetComponent<KissTweenBase>();
                if (ktb != null)
                    tweens.Add(ktb);
                GameObject go = GetChildByName(hub.transform, "Content");
                if (go != null)
                {
                    ktb = go.GetComponent<KissTweenBase>();
                    if (ktb != null)
                        tweens.Add(ktb);
                }
                if (tweens.Count > 0)
                {
                    for (int i = 0; i < tweens.Count; i++)
                    {
                        ktb = tweens[i];
                        if (i == 0)
                            ktb.onFinished += (KissTweenBase tween) =>
                            {
                                if (bDelete)
                                {
                                    mCachePanels.Remove(panelName);
                                    GameObject.Destroy(hub.gameObject);
                                }
                                else
                                {
                                    hub.gameObject.SetActive(false);
                                }
                            };
                        ktb.PlayReverse();
                    }
                }
                else
                {
                    if (bDelete)
                    {
                        mCachePanels.Remove(panelName);
                        GameObject.Destroy(hub.gameObject);
                    }
                    else
                    {
                        hub.gameObject.SetActive(false);
                    }
                }
            }
        }
        /// <summary>
        /// Change Button 
        /// </summary>
        /// <param name="go">Specify GameObject that with Button component</param>
        /// <param name="isEnable">Whether this button can click (Interactable)</param>
        /// <param name="isSpot">Whether show a GameObject name "spot", case-insensitive</param>
        /// <param name="isLock">Whether show a GameObject name "lock", case-insensitive</param>
        public static void ChangeButton(GameObject go, bool isEnable, bool isSpot, bool isLock)
        {
            if (go == null)
                return;
            Button button = go.GetComponent<Button>();
            if (button != null && button.interactable != isEnable)
            {
                button.interactable = isEnable;
                if (button.targetGraphic != null)
                    button.targetGraphic.color = isEnable ? Color.white : Color.gray;
            }
            foreach (GameObject child in go.GetComponentsInChildren<GameObject>(true))
            {
                string str = child.name.ToLower();
                if (str == "spot")
                    child.SetActive(isSpot);
                else if (str == "lock")
                    child.SetActive(isLock);
                //switch (child.name.ToLower())
                //{
                //    case "spot":
                //        child.SetActive(isSpot);
                //        break;
                //    case "lock":
                //        child.SetActive(isLock);
                //        break;
                //}
            }
        }
        public static void ChangeButtonInteractable(GameObject go, bool isEnable)
        {
            if (go == null)
                return;
            Button button = go.GetComponent<Button>();
            if (button != null && button.interactable != isEnable)
            {
                button.interactable = isEnable;
                if (button.targetGraphic != null)
                    button.targetGraphic.color = isEnable ? Color.white : Color.gray;
            }
        }
        public static void DestoryChild(Transform parent)
        {
            foreach(Transform t in parent)
                GameObject.Destroy(t.gameObject);
        }
        public static void DestroyChildImmediate(Transform parent)
        {
            foreach (Transform t in parent)
                GameObject.DestroyImmediate(t.gameObject);
        }
        public static List<int> StringToList(string strs)
        {
            List<int> ret = new List<int>();
            if (!string.IsNullOrEmpty(strs))
            {
                foreach (string str in strs.Split(','))
                {
                    ret.Add(Convert.ToInt32(str));
                }
            }
            return ret;
        }
        public static string ListToString(List<int> list)
        {
            if (list != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (int i in list)
                {
                    sb.Append(i + ",");
                }
                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
            return "";
        }
        public static Dictionary<int, int> StringToDictionary(string strs)
        {
            Dictionary<int, int> ret = new Dictionary<int, int>();
            if (!string.IsNullOrEmpty(strs))
            {
                foreach (string str in strs.Split(' '))
                {
                    string[] strs2 = str.Split(',');
                    if (strs2.Length == 2)
                    {
                        ret[Convert.ToInt32(strs2[0])] = Convert.ToInt32(strs2[1]);
                    }
                }
            }
            return ret;
        }
        public static string DictionaryToString(Dictionary<int, int> dictionary)
        {
            if (dictionary != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var one in dictionary)
                {
                    sb.Append(one.Key + " " + one.Value + ",");
                }
                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
            return "";
        }
    }
}

