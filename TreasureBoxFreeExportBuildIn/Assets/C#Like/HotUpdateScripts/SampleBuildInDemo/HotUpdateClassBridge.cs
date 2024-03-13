//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------


using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// You can consider as this class is a STATIC class, 
    /// as the inteface for you call the hot update functions from the none hot update script.
    /// Usage :
    /// e.g. you want to call function of this class.
    /// 'int c = Add(1,2);', 
    /// you can do it like:
    /// 'int c = (int)HotUpdateBehaviour.GetHotUpdateBridge().MemberCall("Add", 1, 2);'
    /// </summary>
    public class HotUpdateClassBridge : LikeBehaviour
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
        /// <summary>
        /// Call this by 'CSharpLike.KissGame' after default scene was loaded success.
        /// e.g. If your first scene is empty, you can initialize your UI here.
        /// </summary>
        public void OnLoadDefaultSceneDone()
        {
            Debug.Log("CSharpLike.HotUpdateClassBridge:OnLoadDefaultSceneDone()");
            GameObject goMainRoot = GameObject.Find("MainRoot");
            if (goMainRoot == null)
            {
                Debug.LogError("Not exist 'MainRoot'");
                return;
            }
            HotUpdateManager.Show("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleCSharpLikeHotUpdate.prefab",
                    (object obj) =>
                    {
                        Debug.Log("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleCSharpLikeHotUpdate Show return");
                    },
                    goMainRoot.transform);
        }
    }
}