/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// This's an example for you want call OnCollisionEnter2D/OnCollisionExit2D/OnCollisionStay2D API in MonoBehaviour.
    /// It's not hot update script,but you can use OnCollisionEnter2D/OnCollisionExit2D/OnCollisionStay2D in your hot update scripts.
    /// You can duplicate it to Add other special API in MonoBehaviour
    /// except "Awake/OnEnable/Start/OnDisable/OnDestroy"(They are include in HotUpdateBehaviour).
    /// (more unity MonoBehaviour API in https://docs.unity3d.com/ScriptReference/MonoBehaviour.html).
    /// </summary>
	[HelpURL("https://www.csharplike.com/HotUpdateBehaviour.html")]
    public class HotUpdateBehaviourUpdateCollision2D : HotUpdateBehaviour
	{
        /// <summary>
        /// more detail in https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionEnter2D.html
        /// Sent when an incoming collider makes contact with this object's collider (2D physics only).
        /// Further information about the collision is reported in the Collision2D parameter passed during the call. If you don't need this information then you can declare OnCollisionEnter2D without the parameter.
        /// Note: Collision events will be sent to disabled MonoBehaviours, to allow enabling Behaviours in response to collisions.
        /// </summary>
        void OnCollisionEnter2D(Collision2D col)
        {
            helper.MemberCall("OnCollisionEnter2D", col);
        }

        /// <summary>
        /// more detail in https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnCollisionExit2D.html
        /// Sent when a collider on another object stops touching this object's collider (2D physics only).
        /// Further information about the objects involved is reported in the Collision2D parameter passed during the call.If you don't need this information then you can declare OnCollisionExit2D without the parameter.
        /// Note: Collision events will be sent to disabled MonoBehaviours, to allow enabling Behaviours in response to collisions.
        /// </summary>
        void OnCollisionExit2D(Collision2D other)
        {
            helper.MemberCall("OnCollisionExit2D", other);
        }
        void OnCollisionStay2D(Collision2D collision)
        {
            helper.MemberCall("OnCollisionStay2D", collision);
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

