/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
#if UNITY_EDITOR || !UNITY_WEBGL
using System;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_ClientWebSocketBase : MonoBehaviour
        {
            public Uri uri;
            public ClientWebSocket clientWebSocket = new ClientWebSocket();
            void OnDestroy()
            {
                if (clientWebSocket != null && clientWebSocket.State == WebSocketState.Open)
                {
                    _ = _Close();
                }
            }
            protected enum MsgType : byte
            {
                Open,
                Close,
                Error,
                Message,
                Binary,
            }
            protected struct Msg
            {
                public Msg(MsgType type)
                {
                    this.type = type;
                    str = null;
                    binary = null;
                }
                public Msg(MsgType type, string str)
                {
                    this.type = type;
                    this.str = str;
                    binary = null;
                }
                public Msg(MsgType type, byte[] binary)
                {
                    this.type = type;
                    str = null;
                    this.binary = binary;
                }
                public MsgType type;
                public string str;
                public byte[] binary;
            }
            protected Queue<Msg> msgs = new Queue<Msg>();
            object lockObj = new object();
            void EnqueueMsg(Msg msg)
            {
                lock (lockObj)
                {
                    msgs.Enqueue(msg);
                }
            }
            protected Msg DequeueMsg()
            {
                lock (lockObj)
                {
                    return msgs.Dequeue();
                }
            }
            protected ICryptoTransform encryptor;
            protected ICryptoTransform decryptor;
            public string RSAPublicKey = "";
            protected void _Connect()
            {
                clientClose = false;
                if (clientWebSocket != null)
                    clientWebSocket.Dispose();
                clientWebSocket = new ClientWebSocket();
                _ = clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                Thread.Sleep(16);
                while (clientWebSocket.State == WebSocketState.Connecting)
                {
                    Thread.Sleep(16);
                }
                //await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                Debug.LogWarning("clientWebSocket.State=" + clientWebSocket.State);
                switch (clientWebSocket.State)
                {
                    case WebSocketState.Open:
                        EnqueueMsg(new Msg(MsgType.Open));
                        break;
                    case WebSocketState.None:
                        EnqueueMsg(new Msg(MsgType.Error, "WebSocketState.None"));
                        return;
                    case WebSocketState.CloseSent:
                        EnqueueMsg(new Msg(MsgType.Error, "WebSocketState.None"));
                        return;
                    case WebSocketState.CloseReceived:
                        EnqueueMsg(new Msg(MsgType.Error, "WebSocketState.CloseReceived"));
                        return;
                    case WebSocketState.Closed:
                        EnqueueMsg(new Msg(MsgType.Error, "WebSocketState.Closed"));
                        return;
                    case WebSocketState.Aborted:
                        EnqueueMsg(new Msg(MsgType.Error, "WebSocketState.Aborted"));
                        return;
                    default:
                        EnqueueMsg(new Msg(MsgType.Error, "unknown"));
                        return;
                }
                if (!noNeedRSA)
                {
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
                    ClearSendQueue();
                    AddToSendQueueEx(Convert.ToBase64String(finalBuff));
                }
                _ = ReceiveAsync();
                _ = SendAsync();
            }
            public bool noNeedRSA = true;
            async Task ReceiveAsync()
            {
                Thread.Sleep(16);
                //Debug.LogWarning("start ReceiveAsync");
                byte[] tempBuff = new byte[_buffSize];
                ArraySegment<byte> buff = new ArraySegment<byte>(tempBuff);
                WebSocketReceiveResult wrr;
                List<byte[]> tempBuffs = new List<byte[]>();
                do
                {
                    try
                    {
                        wrr = await clientWebSocket.ReceiveAsync(buff, CancellationToken.None);
                        switch (wrr.MessageType)
                        {
                            case WebSocketMessageType.Text:
                                if (wrr.EndOfMessage)
                                {
                                    if (tempBuffs.Count == 0)
                                    {
                                        if (noNeedRSA)
                                            EnqueueMsg(new Msg(MsgType.Message, System.Text.Encoding.UTF8.GetString(tempBuff, 0, wrr.Count)));
                                        else
                                        {
                                            byte[] tempBuff2 = Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(tempBuff, 0, wrr.Count));
                                            EnqueueMsg(new Msg(MsgType.Message, System.Text.Encoding.UTF8.GetString(decryptor.TransformFinalBlock(tempBuff2, 0, tempBuff2.Length))));
                                        }
                                    }
                                    else
                                    {
                                        byte[] newBuff = new byte[wrr.Count];
                                        Array.Copy(tempBuff, 0, newBuff, 0, wrr.Count);
                                        tempBuffs.Add(newBuff);
                                        int sumLength = 0;
                                        foreach (byte[] oneBuff in tempBuffs)
                                            sumLength += oneBuff.Length;
                                        newBuff = new byte[sumLength];
                                        int tempLength = 0;
                                        foreach (byte[] oneBuff in tempBuffs)
                                        {
                                            Array.Copy(oneBuff, 0, newBuff, tempLength, oneBuff.Length);
                                            tempLength += oneBuff.Length;
                                        }
                                        if (noNeedRSA)
                                            EnqueueMsg(new Msg(MsgType.Message, System.Text.Encoding.UTF8.GetString(newBuff, 0, newBuff.Length)));
                                        else
                                        {
                                            byte[] tempBuff2 = Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(newBuff, 0, newBuff.Length));
                                            EnqueueMsg(new Msg(MsgType.Message, System.Text.Encoding.UTF8.GetString(decryptor.TransformFinalBlock(tempBuff2, 0, tempBuff2.Length))));
                                        }
                                    }
                                }
                                else
                                {
                                    byte[] newBuff = new byte[wrr.Count];
                                    Array.Copy(tempBuff, 0, newBuff, 0, wrr.Count);
                                    tempBuffs.Add(newBuff);
                                }
                                
                                break;
                            //case WebSocketMessageType.Binary:
                            //    if (wrr.EndOfMessage)
                            //    {
                            //        if (tempBuffs.Count == 0)
                            //        {
                            //            byte[] newBuff;
                            //            if (noNeedRSA)
                            //            {
                            //                newBuff = new byte[wrr.Count];
                            //                Array.Copy(tempBuff, 0, newBuff, 0, wrr.Count);
                            //                EnqueueMsg(new Msg(MsgType.Binary, newBuff));
                            //            }
                            //            else
                            //            {
                            //                newBuff = decryptor.TransformFinalBlock(tempBuff, 0, wrr.Count);
                            //                EnqueueMsg(new Msg(MsgType.Binary, newBuff));
                            //            }
                            //        }
                            //        else
                            //        {
                            //            byte[] newBuff = new byte[wrr.Count];
                            //            Array.Copy(tempBuff, 0, newBuff, 0, wrr.Count);
                            //            tempBuffs.Add(newBuff);
                            //            int sumLength = 0;
                            //            foreach (byte[] oneBuff in tempBuffs)
                            //                sumLength += oneBuff.Length;
                            //            newBuff = new byte[sumLength];
                            //            int tempLength = 0;
                            //            foreach (byte[] oneBuff in tempBuffs)
                            //            {
                            //                Array.Copy(oneBuff, 0, newBuff, tempLength, oneBuff.Length);
                            //                tempLength += oneBuff.Length;
                            //            }
                            //            if (!noNeedRSA)
                            //                newBuff = decryptor.TransformFinalBlock(newBuff, 0, newBuff.Length);
                            //            EnqueueMsg(new Msg(MsgType.Binary, newBuff));
                            //        }
                            //    }
                            //    else
                            //    {
                            //        byte[] newBuff = new byte[wrr.Count];
                            //        Array.Copy(tempBuff, 0, newBuff, 0, wrr.Count);
                            //        tempBuffs.Add(newBuff);
                            //    }
                            //    break;
                        }
                    }
                    catch (Exception ex)
                    {
                        EnqueueMsg(new Msg(MsgType.Error, ex.Message));
                        clientClose = true;
                        clientWebSocket = new ClientWebSocket();
                        break;
                    }
                }
                while (clientWebSocket.State == WebSocketState.Open);
                if (!clientClose)
                    EnqueueMsg(new Msg(MsgType.Close, clientWebSocket.CloseStatusDescription));
                Debug.LogWarning("end ReceiveAsync");
            }
            async Task SendAsync()
            {
                Debug.LogWarning("start SendAsync");
                string msg = null;
                //byte[] msgB = null;
                while (clientWebSocket.State == WebSocketState.Open)
                {
                    try
                    {

                        if (sendingMsg.Count > 0)
                        {
                            lock (objLockSend)
                            {
                                msg = sendingMsg.Dequeue();
                            }
                        }
                        //else if (sendingMsgB.Count > 0)
                        //{
                        //    lock (objLockSend)
                        //    {
                        //        msgB = sendingMsgB.Dequeue();
                        //    }
                        //}
                        else
                        {
                            Thread.Sleep(16);
                            continue;
                        }
                        if (msg != null)
                        {
                            //Debug.LogWarning("start Send text");
                            byte[] buff = System.Text.Encoding.UTF8.GetBytes(msg);
                            msg = null;
                            if (buff.Length <= _buffSize)
                            {
                                var seg = new ArraySegment<byte>(buff);
                                await clientWebSocket.SendAsync(seg, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            else
                            {
                                int count = 0;
                                while (count + _buffSize < buff.Length)
                                {
                                    var seg2 = new ArraySegment<byte>(buff, count, _buffSize);
                                    await clientWebSocket.SendAsync(seg2, WebSocketMessageType.Text, false, CancellationToken.None);
                                    count += _buffSize;
                                }
                                var seg = new ArraySegment<byte>(buff, count, buff.Length - count);
                                await clientWebSocket.SendAsync(seg, WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                            //Debug.LogWarning("end Send text");
                        }
                        //else if (msgB != null)
                        //{
                        //    Debug.LogWarning("start Send b");
                        //    if (!noNeedRSA)
                        //        msgB = encryptor.TransformFinalBlock(msgB, 0, msgB.Length);
                        //    if (msgB.Length <= _buffSize)
                        //    {
                        //        var seg = new ArraySegment<byte>(msgB);
                        //        await clientWebSocket.SendAsync(seg, WebSocketMessageType.Binary, true, CancellationToken.None);
                        //    }
                        //    else
                        //    {
                        //        int count = 0;
                        //        while (count + _buffSize < msgB.Length)
                        //        {
                        //            var seg2 = new ArraySegment<byte>(msgB, count, _buffSize);
                        //            await clientWebSocket.SendAsync(seg2, WebSocketMessageType.Binary, false, CancellationToken.None);
                        //            count += _buffSize;
                        //        }
                        //        var seg = new ArraySegment<byte>(msgB, count, msgB.Length - count);
                        //        await clientWebSocket.SendAsync(seg, WebSocketMessageType.Binary, true, CancellationToken.None);
                        //    }
                        //    msgB = null;
                        //    Debug.LogWarning("end Send b");
                        //}
                    }
                    catch (Exception ex)
                    {
                        EnqueueMsg(new Msg(MsgType.Error, ex.Message));
                        clientClose = true;
                        clientWebSocket = new ClientWebSocket();
                        break;
                    }
                }
                Debug.LogWarning("end SendAsync");
            }
            bool clientClose = false;

            protected async Task _Close(WebSocketCloseStatus wcs = WebSocketCloseStatus.NormalClosure)
            {
                //Debug.LogWarning("start Close");
                clientClose = true;
                if (clientWebSocket != null)
                {
                    await clientWebSocket.CloseAsync(wcs, wcs.ToString(), CancellationToken.None);
                    EnqueueMsg(new Msg(MsgType.Close, clientWebSocket.CloseStatusDescription));
                }
                else
                    EnqueueMsg(new Msg(MsgType.Close, "socket null"));
                //Debug.LogWarning("end Close");
            }
            static int _buffSize = 1024 * 10;//10KB
            public static void SetBuffSize(int buffSize)
            {
                _buffSize = buffSize;
            }
            Queue<string> sendingMsg = new Queue<string>();
            //Queue<byte[]> sendingMsgB = new Queue<byte[]>();
            object objLockSend = new object();
            protected void AddToSendQueue(string msg)
            {
                lock (objLockSend)
                {
                    if (!noNeedRSA)
                    {
                        byte[] buff = System.Text.Encoding.UTF8.GetBytes(msg);
                        msg = Convert.ToBase64String(encryptor.TransformFinalBlock(buff, 0, buff.Length));
                    }
                    sendingMsg.Enqueue(msg);
                }
            }
            protected void AddToSendQueueEx(string msg)
            {
                lock (objLockSend)
                {
                    sendingMsg.Enqueue(msg);
                }
            }
            protected void AddToSendQueue(byte[] binarys)
            {
                lock (objLockSend)
                {
                    sendingMsg.Enqueue(Convert.ToBase64String(binarys));//We send binary as base64 string
                    //sendingMsgB.Enqueue(binarys);
                }
            }
            protected void ClearSendQueue()
            {
                lock (objLockSend)
                {
                    sendingMsg.Clear();
                    //sendingMsgB.Clear();
                }
            }
        }
    }
}
#endif
