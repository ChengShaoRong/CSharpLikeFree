//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
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

        void OnGUI()
        {
            GUI.Label(new Rect(50, 100, 400, 180), strTips);
            GUIStyle fontStyle = new GUIStyle(GUI.skin.button);
            fontStyle.fontSize = 24;
            if (GUI.Button(new Rect(100, 200, 300, 64), "Hello World", fontStyle))
            {
                HotUpdateManager.Hide("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleCSharpLikeHotUpdate.prefab");
                HotUpdateManager.Show("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleHelloWorld.prefab",
                    (object obj) =>
                    {
                        Debug.Log("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleHelloWorld Show return");
                    },
                    transform.parent);
            }
            if (GUI.Button(new Rect(100, 280, 300, 64), "Interactive Prefab", fontStyle))
            {
                HotUpdateManager.Hide("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleCSharpLikeHotUpdate.prefab");
                HotUpdateManager.Show("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleInteractivePrefabData.prefab",
                    (object obj) =>
                    {
                        Debug.Log("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleInteractivePrefabData Show return");
                    },
                    transform.parent);
            }
            if (GUI.Button(new Rect(100, 360, 300, 64), "Test C#", fontStyle))
            {
                HotUpdateManager.Hide("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleCSharpLikeHotUpdate.prefab");
                HotUpdateManager.Show("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleC#.prefab",
                    (object obj) =>
                    {
                        Debug.Log("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleC# Show return");
                    },
                    transform.parent);
            }
            if (GUI.Button(new Rect(100, 440, 300, 64), "Aircraft Battle", fontStyle))
            {
                HotUpdateManager.Hide("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleCSharpLikeHotUpdate.prefab");
                HotUpdateManager.Show("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/AircraftBattle/BattleField.prefab",
                    (object obj) =>
                    {
                        Debug.Log("HotUpdateResources/SampleBuildInDemo/AircraftBattle/BattleField Show return");
                    },
                    transform.parent);
            }
            if (GUI.Button(new Rect(100, 520, 300, 64), "Exit", fontStyle))
            {
                HotUpdateManager.ClearAllHotUpdatePrefabs();
                SceneManager.LoadScene(0);
            }
        }
    }
}