//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// This class just for show the 'SampleCSharpLikeHotUpdate.prefab' and then destroy self instance.
    /// </summary>
    public class SampleBuildInDemo : LikeBehaviour
    {
        void Start()
        {
            HotUpdateManager.Show("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab",
                (object obj) =>
                {
                    Debug.Log("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate");
                },
                transform.parent);
            GameObject.Destroy(gameObject);
        }
    }
}