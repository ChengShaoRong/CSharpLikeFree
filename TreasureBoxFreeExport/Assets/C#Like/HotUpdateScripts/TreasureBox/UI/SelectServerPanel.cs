//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

namespace TreasureBox
{
    /// <summary>
    /// Select server panel
    /// </summary>
    public class SelectServerPanel : LikeBehaviour
    {
        public static int ServerClassificationType_Recommend = 0;
        public static int ServerClassificationType_Mine = 1;
        public static int ServerClassificationType_Normal = 2;
        //Free version not support enum
        //public enum ServerClassificationType
        //{
        //    Recommend,
        //    Mine,
        //    Normal
        //}
        /// <summary>
        /// The checkbox for server classification
        /// </summary>
        public class ServerClassification : LikeBehaviour
        {
            int serverClassificationType;
            int serverIdFrom;
            int serverIdTo;
            SelectServerPanel panel;
            public static ServerClassification NewInstance(SelectServerPanel panel, Transform transformParent, int serverClassificationType, int serverIdFrom, int serverIdTo, int index)
            {
                GameObject go = ResourceManager.Instantiate<GameObject>(Global.GetPrefabFullName("SelectServerPanel.ServerClassification"), transformParent);
                if (go == null)
                    return null;
                ServerClassification sc = HotUpdateBehaviour.GetComponentByType(go, typeof(ServerClassification)) as ServerClassification;
                sc.serverClassificationType = serverClassificationType;
                sc.serverIdFrom = serverIdFrom;
                sc.serverIdTo = serverIdTo;
                sc.panel = panel;
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.group = transformParent.GetComponent<ToggleGroup>();
                if (index == 0)
                    toggle.isOn = true;
                sc.RefreshUI();
                return sc;
            }

            [SerializeField]
            TMP_Text textClassificationType;
            void RefreshUI()
            {
                if (serverClassificationType == SelectServerPanel.ServerClassificationType_Recommend)
                    textClassificationType.text = Global.GetString("LT_Server_tg_Recommend");
                else if (serverClassificationType == SelectServerPanel.ServerClassificationType_Mine)
                    textClassificationType.text = Global.GetString("LT_Server_tg_My");
                else
                    textClassificationType.text = string.Format(Global.GetString("LT_Server_tg_Class"), serverIdFrom, serverIdTo);
                //textClassificationType.text = (serverClassificationType switch {
                //    ServerClassificationType.Recommend => Global.GetString("LT_Server_tg_Recommend"),
                //    ServerClassificationType.Mine => Global.GetString("LT_Server_tg_My"),
                //    _ => string.Format(Global.GetString("LT_Server_tg_Class"), serverIdFrom, serverIdTo)
                //});
            }

            void OnToggleValueChanged(bool value)
            {
                if (value)
                {
                    List<int> serverIds = new List<int>();
                    if (serverClassificationType == SelectServerPanel.ServerClassificationType_Recommend)
                    {
                        foreach (int id in Global.mRecommendServerIds)
                            serverIds.Add(id);
                    }
                    else if (serverClassificationType == SelectServerPanel.ServerClassificationType_Mine)
                    {
                        for (int i = serverIdFrom; i < serverIdTo; i++)
                        {
                            if (LoginPanel.serverListStates.ContainsKey(i))
                            {
                                if (LoginPanel.serverListStates[i] != LoginPanel.ServerState_Close && MyPlayerPrefs.GetString("ServerInfo_" + i, "") != "")
                                    serverIds.Add(i);
                            }
                            //Free version not support 'out'
                            //if (LoginPanel.serverListStates.TryGetValue(i, out int state))
                            //{
                            //    if (state != LoginPanel.ServerState_Close && MyPlayerPrefs.GetString("ServerInfo_" + i, "") != "")
                            //        serverIds.Add(i);
                            //}
                        }
                    }
                    else if (serverClassificationType == SelectServerPanel.ServerClassificationType_Normal)
                    {
                        for (int i = serverIdFrom; i < serverIdTo; i++)
                        {
                            if (LoginPanel.serverListStates.ContainsKey(i))
                            {
                                if (LoginPanel.serverListStates[i] != LoginPanel.ServerState_Close)
                                    serverIds.Add(i);
                            }
                            //Free version not support 'out'
                            //if (LoginPanel.serverListStates.TryGetValue(i, out int state))
                            //{
                            //    if (state != LoginPanel.ServerState_Close)
                            //        serverIds.Add(i);
                            //}
                        }
                    }
                    //Free version not support switch
                    //switch(serverClassificationType)
                    //{
                    //    case ServerClassificationType.Recommend:
                    //        foreach (int id in Global.mRecommendServerIds)
                    //            serverIds.Add(id);
                    //        break;
                    //    case ServerClassificationType.Mine:
                    //        for (int i = serverIdFrom; i < serverIdTo; i++)
                    //        {
                    //            if (LoginPanel.serverListStates.TryGetValue(i, out int state))
                    //            {
                    //                if (state != LoginPanel.ServerState_Close && MyPlayerPrefs.GetString("ServerInfo_" + i) != "")
                    //                    serverIds.Add(i);
                    //            }
                    //        }
                    //        break;
                    //    case ServerClassificationType.Normal:
                    //        for (int i = serverIdFrom; i < serverIdTo; i++)
                    //        {
                    //            if (LoginPanel.serverListStates.TryGetValue(i, out int state))
                    //            {
                    //                if (state != LoginPanel.ServerState_Close)
                    //                    serverIds.Add(i);
                    //            }
                    //        }
                    //        break;
                    //}
                    panel.OnServerClassificationChanged(serverIds);
                }
            }
        }
        /// <summary>
        /// One server infomation
        /// </summary>
        public class ServerInfo : LikeBehaviour
        {
            int id;
            public static ServerInfo NewInstance(Transform transformParent, int id, int index)
            {
                GameObject go = ResourceManager.Instantiate<GameObject>(Global.GetPrefabFullName("SelectServerPanel.ServerInfo"), transformParent);
                if (go == null)
                    return null;
                ServerInfo si = HotUpdateBehaviour.GetComponentByType(go, typeof(ServerInfo)) as ServerInfo;
                si.id = id;
                si.RefreshUI();
                return si;
            }

            [SerializeField]
            TMP_Text textID;
            [SerializeField]
            TMP_Text textName;
            [SerializeField]
            TMP_Text textInfo;
            [SerializeField]
            KissImage imageState;
            void RefreshUI()
            {
                textID.text = string.Format(Global.GetString("LT_Server_id"), id);
                textName.text = ServerListCsv.Get(id).Name();
                string str = MyPlayerPrefs.GetString("ServerInfo_"+id, "");
                if (!string.IsNullOrEmpty(str))
                    textInfo.text = str;
                int state = LoginPanel.serverListStates[id];
                if (state == LoginPanel.ServerState_Normal)
                    imageState.color = Color.green;
                else if (state == LoginPanel.ServerState_Full)
                    imageState.color = Color.red;
                else
                    imageState.color = Color.white;
                //Free version not support switch
                //imageState.color = (state switch
                //{
                //    LoginPanel.ServerState.Normal => Color.green,
                //    LoginPanel.ServerState.Full => Color.red,
                //    _ => Color.white,
                //});
            }

            void OnClick()
            {
                Global.HidePanel("SelectServerPanel", true);
                LoginPanel.Instance.SetSelectServerId(id);
            }
        }
        [SerializeField]
        Transform transformClassification;
        void Start()
        {
            Global.LocalizeText(gameObject);
            //Show all server classification toggles on the left
            int index = 0;
            ServerClassification.NewInstance(this, transformClassification, ServerClassificationType_Recommend, 0, 0, index++);
            ServerClassification.NewInstance(this, transformClassification, ServerClassificationType_Mine, 0, 0, index++);
            SortedDictionary<int, int> ids = new SortedDictionary<int, int>();
            foreach (int id in LoginPanel.serverListStates.Keys)
            {
                ids[(id - 1) / 10 * 10 + 1] = 0;
            }
            foreach (int id in ids.Keys)
            {
                ServerClassification.NewInstance(this, transformClassification, ServerClassificationType_Normal, id, id + 9, index++);
            }
        }
        //Free version not support Coroutine
        //IEnumerator Start()
        //{
        //    Global.LocalizeText(gameObject);
        //    //Show all server classification toggles on the left
        //    int index = 0;
        //    ServerClassification.NewInstance(this, transformClassification, ServerClassificationType.Recommend, 0, 0, index++);
        //    ServerClassification.NewInstance(this, transformClassification, ServerClassificationType.Mine, 0, 0, index++);
        //    SortedDictionary<int, int> ids = new SortedDictionary<int, int>();
        //    foreach (int id in LoginPanel.serverListStates.Keys)
        //    {
        //        ids[(id - 1) / 10 * 10 + 1] = 0;
        //    }
        //    yield return null;
        //    foreach (int id in ids.Keys)
        //    {
        //        ServerClassification.NewInstance(this, transformClassification, ServerClassificationType.Normal, id, id + 9, index++);
        //        yield return new WaitForSeconds(0.02f);
        //    }
        //}
        void OnClickClose()
        {
            Global.HidePanel("SelectServerPanel", true);
        }
        void OnServerClassificationChanged(List<int> serverIds)
        {
            //Free version
            //Destroy the old server infomation if exist
            Global.DestroyChildImmediate(transformServerInfo);
            //Show all server infomation
            int index = 0;
            foreach (int serverId in serverIds)
            {
                ServerInfo.NewInstance(transformServerInfo, serverId, index++);
            }

            //Full version
            //StartCoroutine("CorOnServerClassificationChanged", serverIds);
        }
        [SerializeField]
        Transform transformServerInfo;
        //Free version not support Coroutine
        //IEnumerator CorOnServerClassificationChanged(List<int> serverIds)
        //{
        //    //Destroy the old server infomation if exist
        //    Global.DestroyChildImmediate(transformServerInfo);
        //    yield return null;
        //    //Show all server infomation
        //    int index = 0;
        //    foreach (int serverId in serverIds)
        //    {
        //        ServerInfo.NewInstance(transformServerInfo, serverId, index++);
        //        yield return new WaitForSeconds(0.02f);
        //    }
        //}
    }
}