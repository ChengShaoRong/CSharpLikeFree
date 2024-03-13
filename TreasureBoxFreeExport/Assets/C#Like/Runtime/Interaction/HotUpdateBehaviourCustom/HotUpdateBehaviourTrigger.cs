/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// This's an example for you want call OnTriggerEnter/OnTriggerExit/OnTriggerStay API in MonoBehaviour.
    /// It's not hot update script,but you can use OnTriggerEnter/OnTriggerExit/OnTriggerStay in your hot update scripts.
    /// You can duplicate it to Add other special API specail API in MonoBehaviour
    /// except "Awake/OnEnable/Start/OnDisable/OnDestroy"(They are include in HotUpdateBehaviour).
    /// (more unity MonoBehaviour API in https://docs.unity3d.com/ScriptReference/MonoBehaviour.html).
    /// </summary>
	[HelpURL("https://www.csharplike.com/HotUpdateBehaviour.html")]
    public class HotUpdateBehaviourTrigger : HotUpdateBehaviour
	{
        void OnTriggerEnter(Collider col)
        {
            helper.MemberCall("OnTriggerEnter", col);
        }

        void OnTriggerExit(Collider other)
        {
            helper.MemberCall("OnTriggerExit", other);
        }
        void OnTriggerStay(Collider other)
        {
            helper.MemberCall("OnTriggerStay", other);
        }
    }
}

