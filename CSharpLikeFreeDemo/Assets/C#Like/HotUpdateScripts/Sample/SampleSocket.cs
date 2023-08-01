//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Sample for how to use Socket and WebSocket.
    /// All functions in this class run in main thread.
    /// </summary>
    public class SampleSocket
    {
        /// <summary>
        /// Using WebSocket or using Socket.
        /// WebGL will force to using WebSocket!
        /// You should set this value before Call SampleSocket.Instance.
        /// </summary>
        public static bool usingWebSocket = false;
        /// <summary>
        /// The URI of the WebSocket if using WebSocket or in WebGL.
        /// You should set this value before Call SampleSocket.Instance.
        /// </summary>
        public static string webSocketURI = "ws://127.0.0.1:9000";
        /// <summary>
        /// The host of the Socket if using Socket and not in WebGL.
        /// You should set this value before Call SampleSocket.Instance.
        /// </summary>
        public static string socketHost = "127.0.0.1";
        /// <summary>
        /// The port of the Socket if using Socket and not in WebGL.
        /// You should set this value before Call SampleSocket.Instance.
        /// </summary>
        public static int socketPort = 9001;
        /// <summary>
        /// The RSA public key of the Socket if using Socket and not in WebGL.
        /// You should set this value before Call SampleSocket.Instance.
        /// That can be found in /Assets/C#Like/Editor/RSAPublicKey.txt 
        /// after you click button 'Generate RSA' in C#Like setting panel.
        /// If you don't want to use security socket, you can set it to "";
        /// </summary>
        public static string socketRSAPublicKey = "<RSAKeyValue><Modulus>y2eiX2AVHrOJZ08ZeSmN5Tu4H9wLRJZemV8XFeeVRYLMyYz2sJCtfSO3RHSpfQPZWEFeMP6NuJoZjfLMGlXludn2lOVJDzx6kp8QLJyIjLh3iaKDDwqIesSZg9/KBnxkJQKGColmP/1JXSRCkIDYzHFx259/KWuZCdoV7IixIuJNb2O6/6LsMYTpwbZ97AJut+BBBATn706yM35XWgInf57OLuGMB773c8NBjp7lFEXfujqa/6eGHYfGmOMNM2YOhuNtgzdMy/lL/rrKrPIh3eVYCUEB7h4bbaKcYMzM9wpI41bhhpVc7V6bQ9mUSLjRqBplEGI/K0eyCLPr3A53DQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        //public static string socketRSAPublicKey = "";

        ///// <summary>
        ///// The RSA public key of the WebSocket if using WebSocket and webSocketURI NOT start with 'wss'.
        ///// You should set this value before Call SampleSocket.Instance.
        ///// That can be found in /Assets/C#Like/Editor/RSAPublicKey.txt 
        ///// after you click button 'Generate RSA' in C#Like setting panel.
        ///// If you don't want to use security webSocket, you can set it to "";
        ///// </summary>
        //public static string webSocketRSAPublicKey = "<RSAKeyValue><Modulus>y2eiX2AVHrOJZ08ZeSmN5Tu4H9wLRJZemV8XFeeVRYLMyYz2sJCtfSO3RHSpfQPZWEFeMP6NuJoZjfLMGlXludn2lOVJDzx6kp8QLJyIjLh3iaKDDwqIesSZg9/KBnxkJQKGColmP/1JXSRCkIDYzHFx259/KWuZCdoV7IixIuJNb2O6/6LsMYTpwbZ97AJut+BBBATn706yM35XWgInf57OLuGMB773c8NBjp7lFEXfujqa/6eGHYfGmOMNM2YOhuNtgzdMy/lL/rrKrPIh3eVYCUEB7h4bbaKcYMzM9wpI41bhhpVc7V6bQ9mUSLjRqBplEGI/K0eyCLPr3A53DQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        ////public static string webSocketRSAPublicKey = "";
        /// <summary>
        /// The instance of WebSocket/Socket.
        /// Before call this, you should set the value of
        /// usingWebSocket/webSocketURI/socketHost/socketPort/socketRSAPublicKey first!
        /// </summary>
        public static SampleSocket Instance
        {
            get;
            set;
        }
        /// <summary>
        /// Initialize SampleSocket because not support custom get/set in FREE version.
        /// </summary>
        public static void Init()
        {
            if (Instance == null)
                Instance = new SampleSocket();
        }
        /// <summary>
        /// The account instance
        /// </summary>
        public static Account account
        {
            get;
            set;
        }
        /// <summary>
        /// Whether the WebSocket/Socket is connected.
        /// </summary>
        public bool IsConnected()
        {
            if (clientWebSocket != null)
                return clientWebSocket.IsConnected;
            else if (clientSocket != null)
                return clientSocket.IsConnected;
            return false;
        }

        /// <summary>
        /// When WebSocket/Socket connect success will call this function
        /// </summary>
        void OnOpen()
        {
            Debug.Log("SampleSocket:OnOpen");
            SampleCSharpLikeHotUpdate.strTips = "SampleSocket:OnOpen";
            //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.AccountLogin);
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["packetType"] = "AccountLogin";
            string name = PlayerPrefs.GetString("myAccountName", "");
            string password = PlayerPrefs.GetString("myAccountPassword", "");
            if (string.IsNullOrEmpty(name))
            {
                name = System.Guid.NewGuid().ToString("N");
                PlayerPrefs.SetString("myAccountName", name);
                PlayerPrefs.Save();
            }
            jsonData["name"] = name;
            jsonData["acctType"] = 0;// (int)Account.AccountType.BuildIn;
            jsonData["password"] = password;
            Send(jsonData);
        }
        /// <summary>
        /// When WebSocket/Socket closed will call this function
        /// </summary>
        void OnClose(string msg)
        {
            Debug.Log("SampleSocket:OnClose:" + msg);
            SampleCSharpLikeHotUpdate.strTips = "SampleSocket:OnClose:" + msg;
            //Notify the ChatRoom modular
            if (SampleChatRoom.Instance != null)
                SampleChatRoom.Instance.OnClose(msg);
        }
        /// <summary>
        /// When WebSocket/Socket occur error will call this function
        /// </summary>
        void OnError(string msg)
        {
            //just print the message
            Debug.LogError("SampleSocket:OnError:" + msg);
            SampleCSharpLikeHotUpdate.strTips = "SampleSocket:OnError:" + msg;
        }

        public SampleSocket()
        {
            Debug.Log("Application.platform=" + Application.platform);
            if (usingWebSocket || Application.platform == RuntimePlatform.WebGLPlayer)//WebGL ONLY support WebSocket
            {
                Debug.Log("usingWebSocket=true");
                //initialize the WebSocket
                GameObject go = new GameObject("ClientWebSocket");
                clientWebSocket = go.AddComponent<CSL_ClientWebSocket>();
                //set callback events of the WebSocket
                clientWebSocket.onOpen += OnOpen;
                clientWebSocket.onClose += OnClose;
                clientWebSocket.onError += OnError;
                clientWebSocket.onMessage += OnMessage;
                //set Uri of the server
                clientWebSocket.SetUri(webSocketURI);
                //clientWebSocket.RSAPublicKey = webSocketRSAPublicKey;
                //clientWebSocket.noNeedRSA = (webSocketURI.StartsWith("wss") || string.IsNullOrEmpty(webSocketRSAPublicKey));
                ////start connect
                //clientWebSocket.Connect();
            }
            else
            {
                Debug.Log("usingWebSocket=false");
                //initialize the Socket
                GameObject go = new GameObject("ClientSocket");
                clientSocket = go.AddComponent<CSL_ClientSocket>();
                //set callback events of the Socket
                clientSocket.onOpen += OnOpen;
                clientSocket.onClose += OnClose;
                clientSocket.onError += OnError;
                clientSocket.onMessage += OnMessage;
                clientSocket.Host = socketHost;
                clientSocket.Port = socketPort;
                clientSocket.RSAPublicKey = socketRSAPublicKey;
                ////start connect
                //clientSocket.Connect();
            }
        }
        /// <summary>
        /// Close the current WebSocket/Socket
        /// </summary>
        public void Close()
        {
            if (clientWebSocket != null)
                clientWebSocket.Close();
            else if (clientSocket != null)
                clientSocket.Close();
            else
                Debug.LogError("SampleSocket : Close : clientWebSocket and clientSocket are both null.");
        }
        /// <summary>
        /// Connect to the server
        /// </summary>
        public void Connect()
        {
            if (clientWebSocket != null)
                clientWebSocket.Connect();
            else if (clientSocket != null)
                clientSocket.Connect();
            else
                Debug.LogError("SampleSocket : Connect : clientWebSocket and clientSocket are both null.");
        }
        /// <summary>
        /// Send JSON data to server
        /// </summary>
        /// <param name="jsonData">JSON data to be send to server</param>
        public void Send(JSONData jsonData)
        {
            if (clientWebSocket != null)
                clientWebSocket.Send(jsonData);
            else if (clientSocket != null)
                clientSocket.Send(jsonData);
            else
                Debug.LogError("SampleSocket : Send : clientWebSocket and clientSocket are both null.");
        }
        /// <summary>
        /// When WebSocket/Socket received JSON data from server will call this function
        /// </summary>
        /// <param name="jsonData">JSON data received from server</param>
        void OnMessage(JSONData jsonData)
        {
            if (Application.isEditor)
                Debug.Log("SampleSocket : OnMessage : " + jsonData.ToJson(true));
            string packetType = jsonData["packetType"];//int packetType = jsonData["packetType"];//If JSONData.packetIsInteger is true
            //FREE version can't use 'enum', we use string instead.
            //FREE version can't use 'switch', we use if-else instead, although is very low effect when have much packet types.
            if (packetType == "CB_Error")
            {
                Debug.LogError(jsonData["error"]);//We print an error log here, you may show a message box to player.
            }
            else if (packetType == "CB_Tips")
            {
                Debug.Log(jsonData["tips"]);//We print an log here, you may show a tip box to player.
            }
            else if (packetType == "CB_Object")
            {
                if (account != null)
                    account.OnCB_Object(jsonData);
                else
                    Debug.LogError("Receive CB_Object packet but account = null.");
            }
            else if (packetType == "CB_Delete")
            {
                if (account != null)
                    account.OnCB_Delete(jsonData);
                else
                    Debug.LogError("Receive CB_Delete packet but account = null.");
            }
            else if (packetType == "CB_ChatRoomEnter")
            {
                if (SampleChatRoom.Instance != null)//Must check the instance whether is null
                    SampleChatRoom.Instance.OnEnterChatRoom(jsonData);
                //You can use the below style instead of SampleChatRoom.Instance.
                //if (HotUpdateManager.GetHotUpdate("Assets/C#Like/Sample/SampleChatRoom.prefab") != null)
                //    HotUpdateManager.GetHotUpdate("Assets/C#Like/Sample/SampleChatRoom.prefab").MemberCall("OnEnterChatRoom", jsonData);
            }
            else if (packetType == "CB_ChatRoomExit")
            {
                if (SampleChatRoom.Instance != null)
                    SampleChatRoom.Instance.OnExitChatRoom(jsonData);
            }
            else if (packetType == "CB_HistoryMsg")
            {
                if (SampleChatRoom.Instance != null)
                    SampleChatRoom.Instance.OnGetHistoryMsg(jsonData);
            }
            else if (packetType == "CB_ChatRoomSend")
            {
                if (SampleChatRoom.Instance != null)
                    SampleChatRoom.Instance.OnReceiveMessage(jsonData);
            }
            else if (packetType == "CB_AccountLogin")
            {
                account = (Account)KissJson.ToObject(typeof(Account), jsonData["account"]);
                Debug.Log(account.ToString());//You must explicit call ToString()
                if (SampleChatRoom.Instance != null)
                    SampleChatRoom.Instance.EnterChatRoom();
                else if (SampleLogin.Instance != null)
                    SampleLogin.Instance.OnLogin(account);
            }
            else if (packetType == "CB_AccountChangeNameAndPassword")
            {
                if (SampleLogin.Instance != null)
                    SampleLogin.Instance.OnLogin(account);
            }
            else
            {
                Debug.LogError("SampleSocket : OnMessage : Unknown packetType : " + packetType);
            }
        }
        /// <summary>
        /// Destroy the WebSocket/Socket instance
        /// </summary>
        public void Destroy()
        {
            if (clientWebSocket != null)
                GameObject.Destroy(clientWebSocket.gameObject);
            else if (clientSocket != null)
                GameObject.Destroy(clientSocket.gameObject);
            clientWebSocket = null;
            clientSocket = null;
            Instance = null;
        }
        CSL_ClientWebSocket clientWebSocket = null;
        CSL_ClientSocket clientSocket = null;
    }
}