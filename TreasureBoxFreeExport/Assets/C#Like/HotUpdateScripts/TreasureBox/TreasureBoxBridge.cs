//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------

using CSharpLike;
using UnityEngine;

namespace TreasureBox
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
    public class TreasureBoxBridge : LikeBehaviour
    {
        /// <summary>
        /// This function is for test
        /// </summary>
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
            Debug.Log("TreasureBox.TreasureBoxBridge:OnLoadDefaultSceneDone()");
            //We show the 'LoginPanel' here manually.
            //(You also can put the 'LoginPanel' in scene directly, but can't using 'Global.HidePanel("LoginPanel");' to hide that panel)
            Global.ShowPanel("LoginPanel", null);
        }
    }
}