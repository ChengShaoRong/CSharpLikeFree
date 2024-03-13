//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
//using System.Collections;
//using UnityEngine.Networking;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

namespace TreasureBox
{
    /// <summary>
    /// Login panel
    /// </summary>
    public class LoginPanel : LikeBehaviour
    {
        //Not support enum in FREE version
        //public enum ServerState
        //{
        //    Normal,
        //    Full,
        //    Maintenance,
        //    Close
        //}
        public static int ServerState_Normal = 0;
        public static int ServerState_Full = 1;
        public static int ServerState_Maintenance = 2;
        public static int ServerState_Close = 3;

        /// <summary>
        /// Server list state from server, key is id, value is ServerState
        /// </summary>
        public static Dictionary<int, int> serverListStates = new Dictionary<int, int>();
        public static LoginPanel Instance;

        bool csvLoaded = false;
        bool stateLoaded = false;

        [SerializeField]
        TMP_Dropdown dropdownLanguage;
        [SerializeField]
        TMP_Text textServerId;
        [SerializeField]
        TMP_Text textServerName;
        [SerializeField]
        KissImage imageState;

        //Not support Coroutine in FREE version, using 'OnUpdate' instead
        void Start()
        {
            Instance = this;
            dropdownLanguage.value = (Global.GetLanguage() == "EN" ? 0 : 1);
            onUpdateState = 0;
            GetGetServerList();
            ////Start coroutine to get server list state first
            //StartCoroutine("CorGetServerList");//Just start this coroutine, but not wait for its result here.

            Resources.UnloadUnusedAssets();

            ////Check one CSV file in "csv.ab" whether exist
            //string strLocalization = Global.GetCsvFullName("Localization");
            //if (!ResourceManager.AssetExist(strLocalization))
            //{
            //    Global.ForceQuit($"Not exist '{strLocalization}' in AssetBundle! exist now");
            //    yield break;
            //}
            ////Waiting for the "csv.ab" loaded
            //while (!ResourceManager.AssetLoaded(strLocalization))
            //    yield return new WaitForSecondsRealtime(0.05f);

            //ReloadCSV();
            ////Localize TMP_Text after load the CSV.
            //Global.LocalizeText(gameObject);

            //csvLoaded = true;

            //CheckRefreshUI();
        }
        int onUpdateState = -1;
        float mTimeout = 10f;
        //Not support Coroutine in FREE version, using 'OnUpdate' instead (It's hard to read)
        void Update()
        {
            if (onUpdateState == 0)
            {
                string strLocalization = Global.GetCsvFullName("Localization");
                if (!ResourceManager.AssetExist(strLocalization))
                {
                    Global.ForceQuit("Not exist '" + strLocalization + "' in AssetBundle! exist now");
                    //Global.ForceQuit($"Not exist '{strLocalization}' in AssetBundle! exist now");
                    onUpdateState = -1;
                }
                else
                    onUpdateState = 1;
            }
            else if (onUpdateState == 1)
            {
                if (ResourceManager.AssetLoaded(Global.GetCsvFullName("Localization")))
                    onUpdateState = 2;
            }
            else if (onUpdateState == 2)
            {
                ReloadCSV();
                //Localize TMP_Text after load the CSV.
                Global.LocalizeText(gameObject);

                csvLoaded = true;

                CheckRefreshUI();
                onUpdateState = -1;
            }
            else if (onUpdateState == 3)
            {
                Debug.Log("CorStartLogin: start connect to server");
                //Connect to server
                MySocket.Instance().Connect();
                mTimeout = 10f;
                onUpdateState = 4;
            }
            else if (onUpdateState == 4)
            {
                if (!MySocket.Instance().IsConnected())
                {
                    if (!string.IsNullOrEmpty(MySocket.Instance().lastError))
                    {
                        WaitingPanel.Hide();
                        MessageBox2.Show(MySocket.Instance().lastError, "LT_Tips",
                            "LT_Reconnect", () =>
                            {
                                onUpdateState = 3;
                            },
                            "LT_Quit", () =>
                            {
                                Application.Quit();
                            }
                        );
                        onUpdateState = -1;
                    }
                    else
                    {
                        mTimeout -= Time.deltaTime;
                        if (mTimeout <= 0f)
                        {
                            WaitingPanel.Hide();
                            MessageBox2.Show("LT_Login_Timeout", "LT_Tips",
                                "LT_Reconnect", () =>
                                {
                                    onUpdateState = 3;
                                },
                                "LT_Quit", () =>
                                {
                                    Application.Quit();
                                }
                            );
                            onUpdateState = -1;
                        }
                    }
                }
                else
                    onUpdateState = 5;
            }
            else if (onUpdateState == 5)
            {
                Debug.Log("CorStartLogin: connect to server success, and start send login data.");
                //Send login data
                if (mLoginData != null)
                {
                    //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.AccountLogin);
                    JSONData jsonData = JSONData.NewDictionary();
                    jsonData["packetType"] = "AccountLogin";
                    jsonData["name"] = mLoginData["uid"];
                    jsonData["acctType"] = 0;// (int)Account.AccountType.BuildIn;
                    jsonData["password"] = mLoginData["token"];
                    MySocket.Instance().Send(jsonData);
                    onUpdateState = -1;
                }
            }
        }
        void OnDestroy()
        {
            Instance = null;
        }
        public static void ReloadCSV()
        {
            //Synchronizing load some important csv file, e.g. Localization csv, ServerList csv
            //SimpleKissCSV.LoadWithFileContent("LocalizationCsv", "key", ResourceManager.LoadAsset<TextAsset>("Assets/C#Like/HotUpdateResources/TreasureBox/CSV/Localization.csv").text);
            SimpleKissCSV.LoadWithFileContent("LocalizationCsv", "key", ResourceManager.LoadAsset<TextAsset>(Global.GetCsvFullName("Localization")).text);
            ServerListCsv.Load();

            //Asynchronous load the other csv file. e.g. not using in this panel yet
            //You don't had to all AsyncLoad here, just make sure load them before using them.
            CharacterCsv.AsyncLoad();
            ExpCsv.AsyncLoad();
            ItemCsv.AsyncLoad();
            SignInCsv.AsyncLoad();
        }
        void CheckRefreshUI()
        {
            //Refresh UI after both csv and server list were loaded
            if (csvLoaded && stateLoaded)
            {
                RefreshUI();
                WaitingPanel.Hide();
            }
        }
        void RefreshUI()
        {
            int serverId = Global.GetCurrentSelectServerId();
            if (serverId <= 0
                || !serverListStates.ContainsKey(serverId))
            {
                if (Global.mRecommendServerIds.Count > 0)
                    serverId = Global.mRecommendServerIds[0];
                if (serverId <= 0)
                {
                    MessageTips.Show("LT_InvalidServerID");
                    return;
                }
            }
            SetSelectServerId(serverId);
        }
        //Not support Coroutine in FREE version, using 'OnUpdate' instead (It's hard to read)
        //IEnumerator CorGetServerList()
        //{
        //    WaitingPanel.Show();
        //    //Get the server list state from your server
        //    using (UnityWebRequest uwr = UnityWebRequest.Get(Global.Url + "GetServerList"))
        //    {
        //        uwr.timeout = 10;
        //        yield return uwr.SendWebRequest();
        //        WaitingPanel.Hide();
        //        if (!string.IsNullOrEmpty(uwr.error))
        //        {
        //            Debug.LogError($"CorGetServerList:error={uwr.error}");
        //            MessageBox2.Show("LT_Login_GetServerListFail", "LT_Tips",
        //                "LT_Reconnect", () =>
        //                {
        //                    StartCoroutine("CorGetServerList");
        //                },
        //                "LT_Quit", () =>
        //                {
        //                    Application.Quit();
        //                }
        //            );
        //            yield break;
        //        }
        //        Debug.Log($"uwr.downloadHandler.text = {uwr.downloadHandler.text}");
        //        JSONData json = KissJson.ToJSONData(uwr.downloadHandler.text);
        //        serverListStates = new Dictionary<int, int>();
        //        Global.mRecommendServerIds.Clear();
        //        foreach (JSONData one in json.Value as List<JSONData>)
        //        {
        //            int serverId = one["id"];
        //            int state = one["state"];
        //            if (state < (int)ServerState.Close)//Ignore the closed server
        //            {
        //                serverListStates.Add(serverId, state);
        //                if (one.ContainsKey("recommend") && one["recommend"] > 0)
        //                    Global.mRecommendServerIds.Add(serverId);
        //            }
        //        }

        //        stateLoaded = true;
        //        CheckRefreshUI();
        //    }
        //}
        void GetGetServerList()
        {
            behaviour.HttpGet(Global.Url + "GetServerList", (cbText, error) =>
            {
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError("CorGetServerList:error=" + error);
                    //Debug.LogError($"CorGetServerList:error={error}");
                    MessageBox2.Show("LT_Login_GetServerListFail", "LT_Tips",
                        "LT_Reconnect", () =>
                        {
                            GetGetServerList();
                        },
                        "LT_Quit", () =>
                        {
                            Application.Quit();
                        }
                    );
                    return;
                }
                Debug.Log("callback text = " + cbText);
                //Debug.Log($"callback text = {cbText}");
                JSONData json = KissJson.ToJSONData(cbText);
                if (json == null)
                {
                    MessageBox2.Show("LT_Login_GetServerListFail", "LT_Tips",
                        "LT_Reconnect", () =>
                        {
                            GetGetServerList();
                        },
                        "LT_Quit", () =>
                        {
                            Application.Quit();
                        }
                    );
                    return;
                }
                if (json.ContainsKey("error"))
                {
                    MessageBox2.Show(json["error"], "LT_Tips",
                        "LT_Reconnect", () =>
                        {
                            GetGetServerList();
                        },
                        "LT_Quit", () =>
                        {
                            Application.Quit();
                        }
                    );
                    return;
                }
                serverListStates = new Dictionary<int, int>();
                Global.mRecommendServerIds.Clear();
                foreach (JSONData one in json.Value as List<JSONData>)
                {
                    int serverId = one["id"];
                    int state = one["state"];
                    if (state < ServerState_Close)//Ignore the closed server
                    {
                        serverListStates.Add(serverId, state);
                        if (one.ContainsKey("recommend") && one["recommend"] > 0)
                            Global.mRecommendServerIds.Add(serverId);
                    }
                }

                stateLoaded = true;
                CheckRefreshUI();
            });
        }
        void OnClickShowPanel(string panelName)
        {
            Global.ShowPanel(panelName, null);
        }
        void OnClickButtonEnterServer()
        {
            Global.ShowPanel("AuthenticationPanel", null);
        }
        JSONData mLoginData = null;
        public void StartLogin(JSONData data)
        {
            Debug.Log("StartLogin:" + data.ToJson(true) + ",CurrentSelectServerId=" + Global.GetCurrentSelectServerId());
            mLoginData = data;
            MySocket.GetSocket(Global.GetCurrentSelectServerId());
            //StopAllCoroutines();
            //StartCoroutine("CorStartLogin");
            onUpdateState = 3;
            WaitingPanel.Show(-1f, null, true);
        }
        //Not support Coroutine in FREE version, using 'OnUpdate' instead (It's hard to read)
        //IEnumerator CorStartLogin()
        //{
        //    WaitingPanel.Show();
        //    Debug.Log("CorStartLogin: start connect to server");
        //    //Connect to server
        //    MySocket.Instance.Connect();
        //    float fTimeout = 10f;
        //    while (!MySocket.Instance.IsConnected)
        //    {
        //        if (!string.IsNullOrEmpty(MySocket.Instance.lastError))
        //        {
        //            WaitingPanel.Hide();
        //            MessageBox2.Show(MySocket.Instance.lastError, "LT_Tips",
        //                "LT_Reconnect", () =>
        //                {
        //                    StartCoroutine("CorStartLogin");
        //                },
        //                "LT_Quit", () =>
        //                {
        //                    Application.Quit();
        //                }
        //            );
        //            yield break;
        //        }
        //        fTimeout -= 0.1f;                                                       
        //        if (fTimeout <= 0f)
        //        {
        //            WaitingPanel.Hide();
        //            MessageBox2.Show("LT_Login_Timeout", "LT_Tips",
        //                "LT_Reconnect", () =>
        //                {
        //                    StartCoroutine("CorStartLogin");
        //                },
        //                "LT_Quit", () =>
        //                {
        //                    Application.Quit();
        //                }
        //            );
        //            yield break;
        //        }
        //        yield return new WaitForSecondsRealtime(0.1f);
        //    }
        //    Debug.Log("CorStartLogin: connect to server success, and start send login data.");
        //    //Send login data
        //    if (mLoginData != null)
        //    {
        //        JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.AccountLogin);
        //        jsonData["name"] = mLoginData["uid"];
        //        jsonData["acctType"] = (int)Account.AccountType.BuildIn;
        //        jsonData["password"] = mLoginData["token"];
        //        MySocket.Instance.Send(jsonData);
        //    }
        //}

        void OnDropdownValueChanged(int value)
        {
            string newLanguage = value == 0 ? "EN" : "CN";
            Debug.Log("OnDropdownValueChanged:language:old=" + Global.GetLanguage() + ",new=" + newLanguage);
            //Debug.Log($"OnDropdownValueChanged:language:old={Global.Language},new={newLanguage}");
            if (Global.GetLanguage() != newLanguage)
            {
                Global.SetLanguage(newLanguage);
                Global.LocalizeText(gameObject);
                RefreshUI();
            }
        }
        public void SetSelectServerId(int serverId)
        {
            Global.SetCurrentSelectServerId(serverId);
            int serverState = serverListStates[serverId];
            ServerListCsv csv = ServerListCsv.Get(serverId);
            textServerId.text = string.Format(Global.GetString("LT_Server_id"), serverId);
            textServerName.text = (csv != null ? csv.Name() : "");
            if (serverState == LoginPanel.ServerState_Normal)
                imageState.color = Color.green;
            else if (serverState == ServerState_Full)
                imageState.color = Color.red;
            else
                imageState.color = Color.white;
            //FREE version not support switch
            //imageState.color = (serverState switch
            //{
            //    ServerState.Normal => Color.green,
            //    ServerState.Full => Color.red,
            //    _ => Color.white,
            //});
        }
        /// <summary>
        /// Back to the main scene
        /// </summary>
        void Back2MainScene()
        {
            Time.timeScale = 1f;//Time scale reset to 1
            SceneManager.LoadScene(0);
        }
    }
}