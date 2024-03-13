//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using TreasureBox;

namespace CSharpLike
{
    /// <summary>
    /// Sample for how to use Socket and WebSocket.
    /// All functions in this class run in main thread.
    /// </summary>
    public class MySocket
    {
        /// <summary>
        /// Using WebSocket or using Socket.
        /// WebGL will force to using WebSocket!
        /// You should set this value before Call MySocket.Instance.
        /// </summary>
        public static bool usingWebSocket = false;
        /// <summary>
        /// The URI of the WebSocket if using WebSocket or in WebGL.
        /// You should set this value before Call MySocket.Instance.
        /// </summary>
        public static string webSocketURI = "ws://127.0.0.1:9000";
        /// <summary>
        /// The host of the Socket if using Socket and not in WebGL.
        /// You should set this value before Call MySocket.Instance.
        /// </summary>
        public static string socketHost = "127.0.0.1";
        /// <summary>
        /// The port of the Socket if using Socket and not in WebGL.
        /// You should set this value before Call MySocket.Instance.
        /// </summary>
        public static int socketPort = 9001;
        /// <summary>
        /// The RSA public key of the Socket if using Socket and not in WebGL.
        /// You should set this value before Call MySocket.Instance.
        /// That can be found in /Assets/C#Like/Editor/RSAPublicKey.txt 
        /// after you click button 'Generate RSA' in C#Like setting panel.
        /// If you don't want to use security socket, you can set it to "";
        /// </summary>
        public static string socketRSAPublicKey = "<RSAKeyValue><Modulus>y2eiX2AVHrOJZ08ZeSmN5Tu4H9wLRJZemV8XFeeVRYLMyYz2sJCtfSO3RHSpfQPZWEFeMP6NuJoZjfLMGlXludn2lOVJDzx6kp8QLJyIjLh3iaKDDwqIesSZg9/KBnxkJQKGColmP/1JXSRCkIDYzHFx259/KWuZCdoV7IixIuJNb2O6/6LsMYTpwbZ97AJut+BBBATn706yM35XWgInf57OLuGMB773c8NBjp7lFEXfujqa/6eGHYfGmOMNM2YOhuNtgzdMy/lL/rrKrPIh3eVYCUEB7h4bbaKcYMzM9wpI41bhhpVc7V6bQ9mUSLjRqBplEGI/K0eyCLPr3A53DQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        //public static string socketRSAPublicKey = "";

        ///// <summary>
        ///// The RSA public key of the WebSocket if using WebSocket and webSocketURI NOT start with 'wss'.
        ///// You should set this value before Call MySocket.Instance.
        ///// That can be found in /Assets/C#Like/Editor/RSAPublicKey.txt 
        ///// after you click button 'Generate RSA' in C#Like setting panel.
        ///// If you don't want to use security webSocket, you can set it to "";
        ///// </summary>
        //public static string webSocketRSAPublicKey = "<RSAKeyValue><Modulus>y2eiX2AVHrOJZ08ZeSmN5Tu4H9wLRJZemV8XFeeVRYLMyYz2sJCtfSO3RHSpfQPZWEFeMP6NuJoZjfLMGlXludn2lOVJDzx6kp8QLJyIjLh3iaKDDwqIesSZg9/KBnxkJQKGColmP/1JXSRCkIDYzHFx259/KWuZCdoV7IixIuJNb2O6/6LsMYTpwbZ97AJut+BBBATn706yM35XWgInf57OLuGMB773c8NBjp7lFEXfujqa/6eGHYfGmOMNM2YOhuNtgzdMy/lL/rrKrPIh3eVYCUEB7h4bbaKcYMzM9wpI41bhhpVc7V6bQ9mUSLjRqBplEGI/K0eyCLPr3A53DQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        ////public static string webSocketRSAPublicKey = "";
        /// <summary>
        /// The instance of WebSocket/Socket.
        /// Before call this, you should set the value of
        /// usingWebSocket/webSocketURI/socketHost/socketPort first!
        /// </summary>
        public static MySocket Instance()
        {
            if (mInstance == null)
                mInstance = new MySocket();
            return mInstance;
        }
        //Free version not support customize 'get/set'
        //public static MySocket Instance
        //{
        //    get
        //    {
        //        if (mInstance == null)
        //            mInstance = new MySocket();
        //        return mInstance;
        //    }
        //    set
        //    {
        //        mInstance = value;
        //    }
        //}
        static int mServerId = -1;
        public static MySocket GetSocket(int serverId)
        {
            if (mServerId != serverId)
            {
                mServerId = serverId;
                ServerListCsv csv = ServerListCsv.Get(mServerId);
                if (csv == null)
                {
                    MessageTips.Show("LT_Auth_CSVError");
                    return null;
                }
                usingWebSocket = true;
                webSocketURI = csv.webSocket;
                socketHost = csv.socketIp;
                socketPort = csv.socketPort;
                if (mInstance != null)
                {
                    mInstance.Close();
                    mInstance.Destroy();
                }
                mInstance = new MySocket();
            }
            return Instance();
        }
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
        //Free version not support customize 'get/set'
        //public bool IsConnected
        //{
        //    get
        //    {
        //        if (clientWebSocket != null)
        //            return clientWebSocket.IsConnected;
        //        else if (clientSocket != null)
        //            return clientSocket.IsConnected;
        //        return false;
        //    }
        //}
        public static JSONData loginJSON;

        /// <summary>
        /// When WebSocket/Socket connect success will call this function
        /// </summary>
        void OnOpen()
        {
            lastError = "";
            Debug.Log("MySocket:OnOpen");
        }
        public string lastError = "";
        /// <summary>
        /// When WebSocket/Socket closed will call this function
        /// </summary>
        void OnClose(string msg)
        {
            lastError = "Closed";
            Debug.Log("MySocket:OnClose:" + msg);
        }
        /// <summary>
        /// When WebSocket/Socket occur error will call this function
        /// </summary>
        void OnError(string msg)
        {
            lastError = "Error:"+msg;
            //just print the message
            Debug.LogError("MySocket:OnError:" + msg);
            if (string.IsNullOrEmpty(msg))
                MessageTips.Show(msg);
        }

        public MySocket()
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
                Debug.LogError("MySocket : Close : clientWebSocket and clientSocket are both null.");
        }
        /// <summary>
        /// Connect to the server
        /// </summary>
        public void Connect()
        {
            lastError = "";
            if (IsConnected())
                return;
            if (clientWebSocket != null)
                clientWebSocket.Connect();
            else if (clientSocket != null)
                clientSocket.Connect();
            else
                Debug.LogError("MySocket : Connect : clientWebSocket and clientSocket are both null.");
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
                Debug.LogError("MySocket : Send : clientWebSocket and clientSocket are both null.");
        }
        /// <summary>
        /// When WebSocket/Socket received JSON data from server will call this function
        /// </summary>
        /// <param name="jsonData">JSON data received from server</param>
        void OnMessage(JSONData jsonData)
        {
            if (Application.isEditor)
                Debug.Log("MySocket : OnMessage : " + jsonData.ToJson(true));
            string packetType = jsonData["packetType"];//int packetType = jsonData["packetType"];//If JSONData.packetIsInteger is true
            if (packetType == "CB_Error")
            {
                MessageBox.Show((string)jsonData["error"], "", "", null);
                if (jsonData["hideWaiting"])
                    WaitingPanel.Hide();
            }
            else if (packetType == "CB_Tips")
            {
                MessageTips.Show((string)jsonData["tips"]);
                if (jsonData["hideWaiting"])
                    WaitingPanel.Hide();
            }
            else if (packetType == "CB_Object")
            {
                if (account != null)
                    account.OnCB_Object(jsonData);
                //account?.OnCB_Object(jsonData);
            }
            else if (packetType == "CB_Delete")
            {
                if (account != null)
                    account.OnCB_Delete(jsonData);
                //account?.OnCB_Delete(jsonData);
            }
            else if (packetType == "CB_GetReward")
            {
                WaitingPanel.Hide();
                Global.ShowPanel("CommonRewardPanel", (obj) =>
                {
                    CommonRewardPanel panel = obj as CommonRewardPanel;
                    if (panel != null)
                        panel.SetRewardData(jsonData["itemIds"], jsonData["itemCounts"]);
                    //(obj as CommonRewardPanel)?.SetRewardData(jsonData["itemIds"], jsonData["itemCounts"]);
                });
            }
            else if (packetType == "CB_AccountLogin")
            {
                account = (Account)KissJson.ToObject(typeof(Account), jsonData["account"]);
                Debug.Log(account.ToString());//You must explicit call ToString()
                Global.ShowPanel("MainScenePanel", null);
                Global.HidePanel("LoginPanel", true);
            }
            else
            {
                Debug.LogError("MySocket : OnMessage : Unknown packetType : " + jsonData["packetType"]);
            }
            //FREE version can't use 'enum', we use string instead.
            //FREE version can't use 'switch', we use if-else instead, although is very low effect when have much packet types.
            //Process different command, assign to different modular.
            //switch ((PacketType)jsonData.GetPacketType(typeof(PacketType)))
            //{
            //    case PacketType.CB_Error:
            //        MessageBox.Show(jsonData["error"], "", "", null);
            //        if (jsonData["hideWaiting"])
            //            WaitingPanel.Hide();
            //        break;
            //    case PacketType.CB_Tips:
            //        MessageTips.Show(jsonData["tips"]);
            //        if (jsonData["hideWaiting"])
            //            WaitingPanel.Hide();
            //        break;
            //    case PacketType.CB_Object:
            //        if (account != null)
            //            account.OnCB_Object(jsonData);
            //        //account?.OnCB_Object(jsonData);
            //        break;
            //    case PacketType.CB_Delete:
            //        if (account != null)
            //            account.OnCB_Delete(jsonData);
            //        //account?.OnCB_Delete(jsonData);
            //        break;
            //    case PacketType.CB_GetReward:
            //        WaitingPanel.Hide();
            //        Global.ShowPanel("CommonRewardPanel", (obj) =>
            //        {
            //            CommonRewardPanel panel = obj as CommonRewardPanel;
            //            if (panel != null)
            //                panel.SetRewardData(jsonData["itemIds"], jsonData["itemCounts"]);
            //            //(obj as CommonRewardPanel)?.SetRewardData(jsonData["itemIds"], jsonData["itemCounts"]);
            //        });
            //        break;
            //    case PacketType.CB_AccountLogin:
            //        account = (Account)KissJson.ToObject(typeof(Account), jsonData["account"]);
            //        Debug.Log(account.ToString());//You must explicit call ToString()
            //        Global.ShowPanel("MainScenePanel");
            //        Global.HidePanel("LoginPanel");
            //        break;
            //    case PacketType.CB_AccountChangeNameAndPassword:
            //        break;
            //    default:
            //        Debug.LogError("MySocket : OnMessage : Unknown packetType : " + jsonData["packetType"]);
            //        break;
            //}
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
            mInstance = null;
        }
        static MySocket mInstance;
        CSL_ClientWebSocket clientWebSocket = null;
        CSL_ClientSocket clientSocket = null;
    }
}