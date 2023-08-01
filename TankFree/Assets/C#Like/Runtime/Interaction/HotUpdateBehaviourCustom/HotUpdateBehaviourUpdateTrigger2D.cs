/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// This's an example for you want call OnCollisionEnter2D/OnCollisionExit2D/OnTriggerStay2D/Update API in MonoBehaviour.
    /// It's not hot update script,but you can use OnCollisionEnter2D/OnCollisionExit2D/OnTriggerStay2D/Update in your hot update scripts.
    /// You can duplicate it to Add other special API special API in MonoBehaviour
    /// except "Awake/OnEnable/Start/OnDisable/OnDestroy"(They are include in HotUpdateBehaviour).
    /// (more unity MonoBehaviour API in https://docs.unity3d.com/ScriptReference/MonoBehaviour.html).
    /// </summary>
	[HelpURL("https://www.csharplike.com/HotUpdateBehaviour.html")]
    public class HotUpdateBehaviourUpdateTrigger2D : HotUpdateBehaviour
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

        [Tooltip(@"Frames Per Second.
(<=0)less than 0 mean Update is Off(default value is 0);
(>=10000)larger than 10000 mean Update is On, and the ""Update()"" function without a param, just like the MonoBehaviour;
(0,10000)between 0 and 10000 mean Update is On, but the ""Update(float deltaTime)"" function with a param of the deltaTime between last Update time;")]
        public int scriptUpdateFPS = 0;
        [Tooltip("Whether the Update function ignore time scale.Default is false.")]
        public bool scriptUpdateIgnoreTimeScale = false;
        private float deltaTime = 0f;
        void Update()
        {
            if (scriptUpdateFPS <= 0)
                return;
            if (scriptUpdateFPS >= 10000)
                helper.MemberCall("Update");
            else
            {
                if (scriptUpdateIgnoreTimeScale)
                {
                    float dt = Time.realtimeSinceStartup - deltaTime;
                    if (dt > (1f / scriptUpdateFPS))
                    {
                        helper.MemberCall("Update", dt);
                        deltaTime = Time.realtimeSinceStartup;
                    }
                }
                else
                {
                    deltaTime += Time.deltaTime;
                    if (deltaTime > (1.0f / scriptUpdateFPS))
                    {
                        helper.MemberCall("Update", deltaTime);
                        deltaTime = 0f;
                    }
                }
            }
        }
    }
}

