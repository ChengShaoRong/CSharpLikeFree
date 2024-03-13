/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;
using UnityEngine;
using System.Net.WebSockets;
using System.Threading;
using UnityEngine.Events;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
#if !UNITY_EDITOR && UNITY_WEBGL
using System.Runtime.InteropServices;
using System.Security.Cryptography;
#endif

namespace CSharpLike
{

    [HelpURL("https://www.csharplike.com/CSL_ClientWebSocket.html")]
#if UNITY_EDITOR || !UNITY_WEBGL
    public class CSL_ClientWebSocket : Internal.CSL_ClientWebSocketBase
    {
        /// <summary>
        /// The action will be call when connect success.
        /// </summary>
        public UnityAction onOpen;
        /// <summary>
        /// The action will be call when WebSocket closed.
        /// </summary>
        public UnityAction<string> onClose;
        /// <summary>
        /// The action will be call when received string message.
        /// </summary>
        public UnityAction<JSONData> onMessage;
        /// <summary>
        /// The action will be call when received binary message.
        /// </summary>
        public UnityAction<byte[]> onBinary;
        /// <summary>
        /// The action will be call when received error(Connect/Close/Send/Abort action may cause some errors).
        /// </summary>
        public UnityAction<string> onError;

        /// <summary>
        /// Set the Uri of the server.
        /// </summary>
        /// <param name="uri">Which uri to be connect</param>
        public void SetUri(Uri uri)
        {
            base.uri = uri;
        }
        /// <summary>
        /// Set the Uri of the server.
        /// </summary>
        /// <param name="uri">Which Uri to be connect</param>
        public void SetUri(string uri)
        {
            base.uri = new Uri(uri);
        }
        void Start()
        {
            DontDestroyOnLoad(gameObject);//make sure don't destroy while load scene
        }
        float nextMessageTime = float.MaxValue;
        void CheckSendHearbeat()
        {
            if (IsConnected
                && nextMessageTime < Time.unscaledTime
                && Application.internetReachability != NetworkReachability.NotReachable)
            {
                MarkSendHearbeat(false);
                AddToSendQueue("{}");//"{}" mean heartbeat
            }
        }
        void MarkSendHearbeat(bool bDisable)
        {
            nextMessageTime = bDisable ? float.MaxValue : (Time.unscaledTime + 100f);//we send hearbeat every 100 seconds
        }
        /// <summary>
        /// Callback the action in main thread using LateUpdate.
        /// </summary>
        void LateUpdate()
        {
            Msg msg;
            while (msgs.Count > 0)
            {
                msg = DequeueMsg();
                switch (msg.type)
                {
                    case MsgType.Message: MarkSendHearbeat(false); onMessage?.Invoke(KissJson.ToJSONData(msg.str)); break;
                    case MsgType.Open: MarkSendHearbeat(false); onOpen?.Invoke(); break;
                    case MsgType.Close: MarkSendHearbeat(true); onClose?.Invoke(msg.str); break;
                    case MsgType.Error: onError?.Invoke(msg.str); break;
                    case MsgType.Binary: onBinary?.Invoke(msg.binary); break;
                }
            }
            CheckSendHearbeat();
        }
        /// <summary>
        /// Connect to the server.
        /// if success will callback onOpen action.
        /// if failed will callback onError action.
        /// </summary>
        public void Connect()
        {
            Task.Run(_Connect);
        }
        /// <summary>
        /// Whether the WebSocket connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return clientWebSocket != null && clientWebSocket.State == WebSocketState.Open;
            }
        }
        /// <summary>
        /// Send JSONData to server
        /// </summary>
        /// <param name="jsonData">the JSONData to be send to server, that will convert to string and send to server</param>
        public void Send(JSONData jsonData)
        {
            Send(jsonData.ToJson());
        }
        /// <summary>
        /// Send string message to server
        /// </summary>
        /// <param name="msg">the string message to be send to server</param>
        public void Send(string msg)
        {
            if (!IsConnected)
            {
                ClearSendQueue();
                Connect();
            }
            //Debug.Log("Send msg " + msg);
            AddToSendQueue(msg);//We send as binary here.
        }
        /// <summary>
        /// Send binary message to server
        /// </summary>
        /// <param name="msg">the binary message to be send to server</param>
        public void Send(byte[] msg)
        {
            if (!IsConnected)
            {
                ClearSendQueue();
                Connect();
            }
            //Debug.Log("Send bytes "+msg.Length);
            AddToSendQueue(msg);
        }
        /// <summary>
        /// Close the connected socket,
        /// will callback onClose action.
        /// </summary>
        public void Close()
        {
            MarkSendHearbeat(true);
            Task.Run(async () =>
            {
                await _Close();
            });
        }
    }
#else
    public class CSL_ClientWebSocket : MonoBehaviour
    {
        /// <summary>
        /// SocketState:Connecting
        /// </summary>
        const int CONNECTING = 0;
        /// <summary>
        /// SocketState:Open
        /// </summary>
        const int OPEN = 1;
        /// <summary>
        /// SocketState:Closing
        /// </summary>
        const int CLOSING = 2;
        /// <summary>
        /// SocketState:Closed
        /// </summary>
        const int CLOSED = 3;
        /// <summary>
        /// The action will be call when connect success.
        /// </summary>
        public UnityAction onOpen;
        /// <summary>
        /// The action will be call when WebSocket closed.
        /// </summary>
        public UnityAction<string> onClose;
        /// <summary>
        /// The action will be call when received string message.
        /// </summary>
        public UnityAction<JSONData> onMessage;
        /// <summary>
        /// The action will be call when received binary message.
        /// </summary>
        public UnityAction<byte[]> onBinary;
        /// <summary>
        /// The action will be call when received error(Connect/Close/Send/Abort action may cause some errors).
        /// </summary>
        public UnityAction<string> onError;

        /// <summary>
        /// Set the Uri of the server.
        /// </summary>
        /// <param name="uri">Which uri to be connect</param>
        public void SetUri(Uri uri)
        {
            this.uri = uri;
        }
        /// <summary>
        /// Set the Uri of the server.
        /// </summary>
        /// <param name="uri">Which Uri to be connect</param>
        public void SetUri(string uri)
        {
            this.uri = new Uri(uri);
        }
        public Uri uri;
        void Start()
        {
            DontDestroyOnLoad(gameObject);//make sure don't destroy while load scene
        }
        protected int clientWebSocket = -1;

        [DllImport("__Internal")]
        private static extern int SocketCreate(string url);

        [DllImport("__Internal")]
        private static extern int SocketState(int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketSendBinary(int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern void SocketSendText(int socketInstance, string str);

        [DllImport("__Internal")]
        private static extern bool SocketRecv(int socketInstance, byte[] ptr, int length);

        [DllImport("__Internal")]
        private static extern int SocketRecvLength(int socketInstance);

        [DllImport("__Internal")]
        private static extern void SocketClose(int socketInstance);

        [DllImport("__Internal")]
        private static extern int SocketError(int socketInstance, byte[] ptr);

        /// <summary>
        /// Connect to the server.
        /// if success will callback onOpen action.
        /// if failed will callback onError action.
        /// </summary>
        public void Connect()
        {
            StopAllCoroutines();
            StartCoroutine(CorConnect());
        }
        float nextMessageTime = float.MaxValue;
        void CheckSendHearbeat()
        {
            if (nextMessageTime < Time.unscaledTime
                && Application.internetReachability != NetworkReachability.NotReachable)
            {
                MarkSendHearbeat(false);
                sendStringQueue.Enqueue("{}");//"{}" mean heartbeat
            }
        }
        void MarkSendHearbeat(bool bDisable)
        {
            nextMessageTime = bDisable ? float.MaxValue : (Time.unscaledTime + 100f);
        }
        protected ICryptoTransform encryptor;
        protected ICryptoTransform decryptor;
        string _RSAPublicKey = "";
        public string RSAPublicKey  { get{return _RSAPublicKey;} set{_RSAPublicKey=value;} }
        IEnumerator CorConnect()
        {
            clientWebSocket = SocketCreate(uri.ToString());
            byte[] buff = new byte[1024];
            while (SocketState(clientWebSocket) == CONNECTING)
            {
                if (SocketError(clientWebSocket, buff) > 0)
                {
                    onError?.Invoke(System.Text.Encoding.UTF8.GetString(buff));
                    yield break;
                }
                yield return new WaitForSeconds(0.05f);
            }
            if (SocketState(clientWebSocket) == OPEN)
            {
                MarkSendHearbeat(false);
                onOpen?.Invoke();
            }
            else
            {
                onError?.Invoke("Not connect success.");
                yield break;
            }
            if (!noNeedRSA)
            {
                Debug.Log("start send RSA");
                byte[] key = new byte[32];
                byte[] iv = new byte[16];
                System.Random r = new System.Random();
                r.NextBytes(key);
                r.NextBytes(iv);
                RijndaelManaged rijndaelManaged = new RijndaelManaged();
                encryptor = rijndaelManaged.CreateEncryptor(key, iv);
                decryptor = rijndaelManaged.CreateDecryptor(key, iv);
                RSACryptoServiceProvider crypt = new RSACryptoServiceProvider(2048);
                crypt.FromXmlString(RSAPublicKey);
                byte[] finalBuff = new byte[key.Length + iv.Length];
                Array.Copy(key, 0, finalBuff, 0, key.Length);
                Array.Copy(iv, 0, finalBuff, key.Length, iv.Length);
                finalBuff = crypt.Encrypt(finalBuff, false);
                SocketSendBinary(clientWebSocket, finalBuff, finalBuff.Length);
                Debug.Log("end send RSA");
            }
            while(IsConnected)
            {
                if (SocketError(clientWebSocket, buff) > 0)
                {
                    onError?.Invoke(System.Text.Encoding.UTF8.GetString(buff));
                    yield break;
                }
                try
                {
                    //process receive
                    int length = SocketRecvLength(clientWebSocket);
                    if (length > 0)
                    {
                        byte[] buffReceive = new byte[length];
                        if (SocketRecv(clientWebSocket, buffReceive, length))
                        {
                            onMessage?.Invoke(KissJson.ToJSONData(System.Text.Encoding.UTF8.GetString(noNeedRSA ? buffReceive : decryptor.TransformFinalBlock(buffReceive, 0, length))));
                        }
                        else
                        {
                            onBinary?.Invoke(noNeedRSA ? buffReceive : decryptor.TransformFinalBlock(buffReceive, 0, length));
                        }
                        MarkSendHearbeat(false);
                    }
                    //process send
                    if (sendStringQueue.Count > 0 || sendBinaryQueue.Count > 0)
                        MarkSendHearbeat(false);
                    while (sendStringQueue.Count > 0)
                    {
                        string str = sendStringQueue.Dequeue();
                        SocketSendText(clientWebSocket, str);
                    }
                    while (sendBinaryQueue.Count > 0)
                    {
                        byte[] buffSend = sendBinaryQueue.Dequeue();
                        SocketSendBinary(clientWebSocket, buffSend, buffSend.Length);
                    }
                    CheckSendHearbeat();
                }
                catch (Exception e)
                {
                    onError?.Invoke(e.Message);
                }
                yield return null;
            }
            MarkSendHearbeat(true);
            if (SocketError(clientWebSocket, buff) > 0)
            {
                onError?.Invoke(System.Text.Encoding.UTF8.GetString(buff));
                yield break;
            }
        }
        /// <summary>
        /// Whether the WebSocket connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (clientWebSocket == -1)
                    return false;
                return SocketState(clientWebSocket) == OPEN;
            }
        }
        bool _noNeedRSA = true;
        public bool noNeedRSA { get{ return _noNeedRSA;} set{ _noNeedRSA = value;}}
        /// <summary>
        /// Send JSONData to server
        /// </summary>
        /// <param name="jsonData">the JSONData to be send to server, that will convert to string and send to server</param>
        public void Send(JSONData jsonData)
        {
            Send(jsonData.ToJson());
        }
        Queue<string> sendStringQueue = new Queue<string>();
        /// <summary>
        /// Send string message to server
        /// </summary>
        /// <param name="msg">the string message to be send to server</param>
        public void Send(string msg)
        {
            if (!IsConnected)
            {
                ClearSendQueue();
                Connect();
            }
            sendStringQueue.Enqueue(msg);
        }
        Queue<byte[]> sendBinaryQueue = new Queue<byte[]>();
        /// <summary>
        /// Send binary message to server
        /// </summary>
        /// <param name="msg">the binary message to be send to server</param>
        public void Send(byte[] msg)
        {
            if (!IsConnected)
            {
                ClearSendQueue();
                Connect();
            }
            sendBinaryQueue.Enqueue(msg);
        }
        protected void ClearSendQueue()
        {
            sendStringQueue.Clear();
            sendBinaryQueue.Clear();
        }
        /// <summary>
        /// Close the connected socket,
        /// will callback onClose action.
        /// </summary>
        public void Close()
        {
            MarkSendHearbeat(true);
            SocketClose(clientWebSocket);
            clientWebSocket = -1;
        }
        void OnDestroy()
        {
            if (IsConnected)
                Close();
        }
    }
#endif
}

