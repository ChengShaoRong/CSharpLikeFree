//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Sample for chat room.
    /// Show the most simple usage of WebSocket.
    /// </summary>
    public class SampleChatRoom : LikeBehaviour
    {
        /// <summary>
        /// This the singleton of SampleChatRoom for quick accept in other modular.
        /// You also can use HotUpdateManager.GetHotUpdate("Assets/C#Like/Sample/SampleChatRoom.prefab") instead.
        /// </summary>
        public static SampleChatRoom Instance { get; set; }
        void OnDestroy()
        {
            if (SampleSocket.Instance.IsConnected())
            {
                //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.ChatRoomExit);
                JSONData jsonData = JSONData.NewDictionary();
                jsonData["packetType"] = "ChatRoomExit";
                SampleSocket.Instance.Send(jsonData);
            }
            Instance = null;
        }
        void Start()
        {
            Instance = this;
            //initialize self name use in chat room
            strSelfName = "Guest"+ Random.Range(1, 10000000);
            //Initialize the WebSocket
            //SampleSocket.Instance = new SampleSocket();
            //Before call SampleSocket.Instance we should set the follow value!
            if (!SampleSocket.Instance.IsConnected())//check whether Socket was connected
                SampleSocket.Instance.Connect();
            else if (SampleSocket.account != null)
                EnterChatRoom();
        }
        Queue<GameObject> msgs = new Queue<GameObject>();
        void AddMessage(string msg)
        {
            HotUpdateManager.NewInstanceGameObject("Assets/C#Like/Sample/SampleChatRoomOneMSG.prefab",
                        (GameObject go) =>
                        {
                            msgs.Enqueue(go);
                            if (msgs.Count > 20)
                                GameObject.Destroy(msgs.Dequeue());
                            go.GetComponent<Text>().text = msg;
                        },
                        GetGameObject("MsgContent").transform,
                        Vector3.zero);
        }
        string strSelfName;
        /// <summary>
        /// The click event of the "Back" button
        /// </summary>
        void OnClickBack()
        {
            HotUpdateManager.Show("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");//back to SampleCSharpLikeHotUpdate
            HotUpdateManager.Hide("Assets/C#Like/Sample/SampleChatRoom.prefab", true);//close and delete self
        }
        /// <summary>
        /// The click event of the "Send" button
        /// </summary>
        void OnClickSend()
        {
            string msg = GetComponent<InputField>("Input").text.Trim();
            if (string.IsNullOrEmpty(msg))
            {
                Debug.Log("message is empty");
                return;
            }
            if (msg.Length > 200)
            {
                Debug.Log("message too long");
                return;
            }
            //Clear the input text.
            GetComponent<InputField>("Input").text = "";
            //Send message to server.
            //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.ChatRoomSend);
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["packetType"] = "ChatRoomSend";
            jsonData["Msg"] = msg;
            SampleSocket.Instance.Send(jsonData);
        }
        public void EnterChatRoom()
        {
            //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.ChatRoomEnter);
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["packetType"] = "ChatRoomEnter";
            jsonData["Nickname"] = strSelfName;
            SampleSocket.Instance.Send(jsonData);
        }
        /// <summary>
        /// When WebSocket closed will call this function
        /// </summary>
        public void OnClose(string msg)
        {
            Debug.Log("OnClose:"+msg);
        }
        public void OnEnterChatRoom(JSONData jsonData)
        {
            //AddMessage($"{jsonData["time"]} : {jsonData["msg"]}");
            AddMessage(string.Format("{0} : {1}", jsonData["time"], jsonData["msg"]));
        }
        public void OnExitChatRoom(JSONData jsonData)
        {
            //AddMessage($"{jsonData["time"]} : {jsonData["msg"]}");
            AddMessage(string.Format("{0} : {1}", jsonData["time"], jsonData["msg"]));
        }
        public void OnGetHistoryMsg(JSONData jsonData)
        {
            JSONData history = jsonData["history"];
            for(int i=0; i<history.Count; i++)
            {
                JSONData oneMsg = history[i];
                //AddMessage($"{oneMsg["time"]} : [{oneMsg["nickname"]}] {oneMsg["msg"]}");
                AddMessage(string.Format("{0} : [{1}] {2}", oneMsg["time"], oneMsg["nickname"], oneMsg["msg"]));
            }
        }
        public void OnReceiveMessage(JSONData jsonData)
        {
            //AddMessage($"{jsonData["time"]} : [{jsonData["nickname"]}] {jsonData["msg"]}");
            AddMessage(string.Format("{0} : [{1}] {2}", jsonData["time"], jsonData["nickname"], jsonData["msg"]));
        }
    }
}