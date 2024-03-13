//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike;
using UnityEngine;
using UnityEngine.UI;

namespace TreasureBox
{
    /// <summary>
    /// First panel in default scene, for show loading process while AssetBundle may not download yet.
    /// Specifically, at first time to show the scene, MainScenePanel request download some AssetBundle files first.
    /// </summary>
	public class LoadingPanel : LikeBehaviour
    {
        [SerializeField]
        string url;
        [SerializeField]
        RawImage imgBg;
        static LoadingPanel mInstance;//For hide this panel when loading done.
        void Start()
        {
            mInstance = this;
            //Load the texture from web
            ResourceManager.LoadTexture(url, imgBg);
        }
        /// <summary>
        /// Hide this loading panel.
        /// We call this in function 'Start' of MainScenePanel.
        /// </summary>
        public static void Hide()
        {
            if (mInstance != null && mInstance.gameObject != null)
            {
                GameObject.Destroy(mInstance.gameObject);
                mInstance = null;
            }
        }
    }
}

