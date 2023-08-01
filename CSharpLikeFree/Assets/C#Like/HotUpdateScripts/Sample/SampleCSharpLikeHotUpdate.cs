//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// The main interface in the hot update script.
    /// In demo, we show this interface while load the hot update script done.
    /// </summary>
    public class SampleCSharpLikeHotUpdate : LikeBehaviour
    {
        /// <summary>
        /// the tips string show in label content
        /// </summary>
        public static string strTips = "";
        void Start()
        {
            //Set the socket config from the setting get from server
            SampleSocket.usingWebSocket = true;
            SampleSocket.webSocketURI = "wss://www.csharplike.com:10000"; //SampleCSharpLike.configJSON["serverInfo"]["WebSocketURI"];
            SampleSocket.socketHost = "www.csharplike.com"; //SampleCSharpLike.configJSON["serverInfo"]["SocketHost"];
            SampleSocket.socketPort = 9001; //SampleCSharpLike.configJSON["serverInfo"]["SocketPort"];
            //Start connect if was not connected
            SampleSocket.Init();
            if (!SampleSocket.Instance.IsConnected())//check whether Socket was connected
                SampleSocket.Instance.Connect();
        }

        void OnGUI()
        {
            GUI.Label(new Rect(50, 100, 400, 180), strTips);
            GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
            fontStyle.fontSize = 24;
            if (GUI.Button(new Rect(100, 200, 300, 64), "Hello World", fontStyle))
            {
                HotUpdateManager.Hide("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");
                HotUpdateManager.Show("Assets/C#Like/Sample/SampleHelloWorld.prefab",
                    (HotUpdateBehaviour behaviour) =>
                    {
                        Debug.Log("Assets/C#Like/Sample/SampleHelloWorld Show return");
                    },
                    transform.parent);
            }
            if (GUI.Button(new Rect(100, 280, 300, 64), "Interactive Prefab", fontStyle))
            {
                HotUpdateManager.Hide("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");
                HotUpdateManager.Show("Assets/C#Like/Sample/SampleInteractivePrefabData.prefab",
                    (HotUpdateBehaviour behaviour) =>
                    {
                        Debug.Log("Assets/C#Like/Sample/SampleInteractivePrefabData Show return");
                    },
                    transform.parent);
            }
            if (GUI.Button(new Rect(100, 360, 300, 64), "Test C#", fontStyle))
            {
                HotUpdateManager.Hide("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");
                HotUpdateManager.Show("Assets/C#Like/Sample/SampleC#.prefab",
                    (HotUpdateBehaviour behaviour) =>
                    {
                        Debug.Log("Sample/SampleC# Show return");
                    },
                    transform.parent);
            }
            if (GUI.Button(new Rect(100, 440, 300, 64), "Aircraft Battle", fontStyle))
            {
                HotUpdateManager.Hide("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");
                HotUpdateManager.Show("Assets/C#Like/Sample/AircraftBattle/BattleField.prefab",
                    (HotUpdateBehaviour behaviour) =>
                    {
                        Debug.Log("Assets/C#Like/Sample/AircraftBattle/BattleField Show return");
                    },
                    transform.parent);
            }
            if (GUI.Button(new Rect(100, 520, 300, 64), "Chat Room", fontStyle))
            {
                HotUpdateManager.Hide("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");
                HotUpdateManager.Show("Assets/C#Like/Sample/SampleChatRoom.prefab",
                    (HotUpdateBehaviour behaviour) =>
                    {
                        Debug.Log("Assets/C#Like/Sample/SampleChatRoom Show return");
                    },
                    transform.parent);
            }
            if (GUI.Button(new Rect(100, 600, 300, 64), "Exit", fontStyle))
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}