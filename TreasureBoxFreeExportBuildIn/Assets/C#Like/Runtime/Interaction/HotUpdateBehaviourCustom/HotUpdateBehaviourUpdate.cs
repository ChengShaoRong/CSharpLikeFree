/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// This's an HotUpdateBehaviour with Update API of MonoBehaviour.
    /// "Awake/OnEnable/Start/OnDisable/OnDestroy"(They are include in HotUpdateBehaviour).
    /// (more unity MonoBehaviour API in https://docs.unity3d.com/ScriptReference/MonoBehaviour.html).
    /// </summary>
	[HelpURL("https://www.csharplike.com/HotUpdateBehaviour.html")]
    public class HotUpdateBehaviourUpdate : HotUpdateBehaviour
    {
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

