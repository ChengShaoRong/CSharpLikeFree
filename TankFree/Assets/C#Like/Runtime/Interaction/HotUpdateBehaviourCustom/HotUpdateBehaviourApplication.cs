/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// This's an HotUpdateBehaviour with OnApplicationFocus/OnApplicationPause/OnApplicationQuit API of MonoBehaviour.
    /// "Awake/OnEnable/Start/OnDisable/OnDestroy"(They are include in HotUpdateBehaviour).
    /// (more unity MonoBehaviour API in https://docs.unity3d.com/ScriptReference/MonoBehaviour.html).
    /// </summary>
	[HelpURL("https://www.csharplike.com/HotUpdateBehaviour.html")]
    public class HotUpdateBehaviourApplication : HotUpdateBehaviour
    {
        void OnApplicationFocus(bool hasFocus)
        {
            helper.MemberCall("OnApplicationFocus", hasFocus);
        }
        void OnApplicationPause(bool pauseStatus)
        {
            helper.MemberCall("OnApplicationPause", pauseStatus);
        }
        void OnApplicationQuit()
        {
            helper.MemberCall("OnApplicationQuit");
        }
    }
}

