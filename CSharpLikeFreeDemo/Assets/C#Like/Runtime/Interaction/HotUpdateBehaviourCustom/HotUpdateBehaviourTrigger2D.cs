/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// This's an example for you want call OnTriggerEnter2D/OnTriggerExit2D/OnTriggerStay2D API in MonoBehaviour.
    /// It's not hot update script,but you can use OnTriggerEnter2D/OnTriggerExit2D/OnTriggerStay2D in your hot update scripts.
    /// You can duplicate it to Add other special API specail API in MonoBehaviour
    /// except "Awake/OnEnable/Start/OnDisable/OnDestroy"(They are include in HotUpdateBehaviour).
    /// (more unity MonoBehaviour API in https://docs.unity3d.com/ScriptReference/MonoBehaviour.html).
    /// </summary>
	[HelpURL("https://www.csharplike.com/HotUpdateBehaviour.html")]
    public class HotUpdateBehaviourTrigger2D : HotUpdateBehaviour
	{
        void OnTriggerEnter2D(Collider2D col)
        {
            helper.MemberCall("OnTriggerEnter2D", col);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            helper.MemberCall("OnTriggerExit2D", other);
        }
        void OnTriggerStay2D(Collider2D other)
        {
            helper.MemberCall("OnTriggerStay2D", other);
        }
    }
}

