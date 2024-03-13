/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// This's an HotUpdateBehaviour with FixedUpdate API of MonoBehaviour.
    /// "Awake/OnEnable/Start/OnDisable/OnDestroy"(They are include in HotUpdateBehaviour).
    /// (more unity MonoBehaviour API in https://docs.unity3d.com/ScriptReference/MonoBehaviour.html).
    /// </summary>
	[HelpURL("https://www.csharplike.com/HotUpdateBehaviour.html")]
    public class HotUpdateBehaviourFixedUpdate : HotUpdateBehaviour
    {
        void FixedUpdate()
        {
            helper.MemberCall("FixedUpdate");
        }
    }
}

