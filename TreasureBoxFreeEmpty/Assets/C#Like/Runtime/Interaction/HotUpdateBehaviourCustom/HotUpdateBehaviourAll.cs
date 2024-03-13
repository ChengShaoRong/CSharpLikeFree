/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// This's the HotUpdateBehaviour include all API in MonoBehaviour.
    /// "Awake/OnEnable/Start/OnDisable/OnDestroy"(They are include in HotUpdateBehaviour).
    /// This's a backup plan in case for you forgot add your custom HotUpdateBehaviour,we don't recommend use it.
    /// (the unity MonoBehaviour API in https://docs.unity3d.com/ScriptReference/MonoBehaviour.html).
    /// </summary>
	[HelpURL("https://www.csharplike.com/HotUpdateBehaviour.html")]
    public class HotUpdateBehaviourAll : HotUpdateBehaviour
    {
        [Tooltip("Whether the FixUpdate function is enable.")]
        public bool enableFixedUpdate = false;
        void FixedUpdate()
        {
            if (enableFixedUpdate)
                helper.MemberCall("FixedUpdate");
        }
        [Tooltip("Whether the LateUpdate function is enable.")]
        public bool enableLateUpdate = false;
        void LateUpdate()
        {
            if (enableLateUpdate)
                helper.MemberCall("LateUpdate");
        }
        void OnAnimatorIK(int layerIndex)
        {
            helper.MemberCall("OnAnimatorIK", layerIndex);
        }
        void OnAnimatorMove()
        {
            helper.MemberCall("OnAnimatorMove");
        }
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
        void OnAudioFilterRead(float[] data, int channels)
        {
            helper.MemberCall("OnAudioFilterRead", data, channels);
        }
        void OnBecameInvisible()
        {
            helper.MemberCall("OnBecameInvisible");
        }
        void OnBecameVisible()
        {
            helper.MemberCall("OnBecameVisible");
        }
        void OnCollisionEnter(Collision collision)
        {
            helper.MemberCall("OnCollisionEnter", collision);
        }
        void OnCollisionEnter2D(Collision2D col)
        {
            helper.MemberCall("OnCollisionEnter2D", col);
        }
        void OnCollisionExit(Collision other)
        {
            helper.MemberCall("OnCollisionExit", other);
        }
        void OnCollisionExit2D(Collision2D other)
        {
            helper.MemberCall("OnCollisionExit2D", other);
        }
        void OnCollisionStay(Collision collisionInfo)
        {
            helper.MemberCall("OnCollisionStay", collisionInfo);
        }
        void OnCollisionStay2D(Collision2D collision)
        {
            helper.MemberCall("OnCollisionStay2D", collision);
        }
        void OnConnectedToServer()
        {
            helper.MemberCall("OnConnectedToServer");
        }
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            helper.MemberCall("OnControllerColliderHit", hit);
        }
#if !UNITY_2018_2_OR_NEWER
        void OnDisconnectedFromServer(NetworkDisconnection info)
        {
            helper.MemberCall1("OnDisconnectedFromServer", info);
        }
#endif
        void OnDrawGizmos()
        {
            helper.MemberCall("OnDrawGizmos");
        }
        void OnDrawGizmosSelected()
        {
            helper.MemberCall("OnDrawGizmosSelected");
        }
#if !UNITY_2018_2_OR_NEWER
        void OnFailedToConnect(NetworkConnectionError error)
        {
            helper.MemberCall("OnFailedToConnect", error);
        }
#endif
#if !UNITY_2018_2_OR_NEWER
        void OnFailedToConnectToMasterServer(NetworkConnectionError error)
        {
            helper.MemberCall("OnFailedToConnectToMasterServer", error);
        }
#endif
        [Tooltip("Whether the OnGUI function is enable.")]
        public bool enableOnGUI = false;
        void OnGUI()
        {
            if (enableOnGUI)
                helper.MemberCall("OnGUI");
        }
        void OnJointBreak(float breakForce)
        {
            helper.MemberCall("OnJointBreak", breakForce);
        }
        void OnJointBreak2D(Joint2D brokenJoint)
        {
            helper.MemberCall("OnJointBreak2D", brokenJoint);
        }
#if !UNITY_2018_2_OR_NEWER
        void OnMasterServerEvent(MasterServerEvent masterServerEvent)
        {
            helper.MemberCall("OnMasterServerEvent", masterServerEvent);
        }
#endif
        void OnMouseDown()
        {
            helper.MemberCall("OnMouseDown");
        }
        void OnMouseDrag()
        {
            helper.MemberCall("OnMouseDrag");
        }
        void OnMouseEnter()
        {
            helper.MemberCall("OnMouseEnter");
        }
        void OnMouseOver()
        {
            helper.MemberCall("OnMouseOver");
        }
        void OnMouseExit()
        {
            helper.MemberCall("OnMouseExit");
        }
        void OnMouseUp()
        {
            helper.MemberCall("OnMouseUp");
        }
        void OnMouseUpAsButton()
        {
            helper.MemberCall("OnMouseUpAsButton");
        }
#if !UNITY_2018_2_OR_NEWER
        void OnNetworkInstantiate(NetworkMessageInfo info)
        {
            helper.MemberCall("OnNetworkInstantiate", info);
        }
#endif
        void OnParticleCollision(GameObject other)
        {
            helper.MemberCall("OnParticleCollision", other);
        }
        void OnParticleSystemStopped()
        {
            helper.MemberCall("OnParticleSystemStopped");
        }
        void OnParticleTrigger()
        {
            helper.MemberCall("OnParticleTrigger");
        }
        void OnParticleUpdateJobScheduled()
        {
            helper.MemberCall("OnParticleUpdateJobScheduled");
        }
#if !UNITY_2018_2_OR_NEWER
        void OnPlayerConnected(NetworkPlayer player)
        {
            helper.MemberCall("OnPlayerConnected", player);
        }
#endif
#if !UNITY_2018_2_OR_NEWER
        void OnPlayerDisconnected(NetworkPlayer player)
        {
            helper.MemberCall("OnPlayerDisconnected", player);
        }
#endif
        void OnPostRender()
        {
            helper.MemberCall("OnPostRender");
        }
        void OnPreCull()
        {
            helper.MemberCall("OnPreCull");
        }
        void OnPreRender()
        {
            helper.MemberCall("OnPreRender");
        }
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            helper.MemberCall("OnRenderImage", src, dest);
        }
        void OnRenderObject()
        {
            helper.MemberCall("OnRenderObject");
        }
#if !UNITY_2018_2_OR_NEWER
        void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            helper.MemberCall2("OnSerializeNetworkView", stream, info);
        }
#endif
        void OnServerInitialized()
        {
            helper.MemberCall("OnServerInitialized");
        }
        void OnTransformChildrenChanged()
        {
            helper.MemberCall("OnTransformChildrenChanged");
        }
        void OnTransformParentChanged()
        {
            helper.MemberCall("OnTransformParentChanged");
        }
        void OnTriggerEnter(Collider other)
        {
            helper.MemberCall("OnTriggerEnter", other);
        }
        void OnTriggerEnter2D(Collider2D other)
        {
            helper.MemberCall("OnTriggerEnter2D", other);
        }
        void OnTriggerExit(Collider other)
        {
            helper.MemberCall("OnTriggerExit", other);
        }
        void OnTriggerExit2D(Collider2D other)
        {
            helper.MemberCall("OnTriggerExit2D", other);
        }
        void OnTriggerStay(Collider other)
        {
            helper.MemberCall("OnTriggerStay", other);
        }
        void OnTriggerStay2D(Collider2D other)
        {
            helper.MemberCall("OnTriggerStay2D", other);
        }
#if UNITY_EDITOR
        void OnValidate()
        {
            helper.MemberCall("OnValidate");
        }
#endif
        void OnWillRenderObject()
        {
            helper.MemberCall("OnWillRenderObject");
        }
#if UNITY_EDITOR
        void Reset()
        {
            helper.MemberCall("Reset");
        }
#endif

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

