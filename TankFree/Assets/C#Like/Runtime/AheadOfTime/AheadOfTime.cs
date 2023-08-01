/*
 *           C#Like
 * Copyright © 2022-2023 RongRong
 * It's automatic generate by CSharpLikeEditor,don't modify this file.
 */

namespace CSharpLike
{
    namespace Internal
    {
        /// <summary>
        /// Ahead-of-time compile for generic type in ScriptingBackend with IL2CPP
        /// </summary>
        public class AheadOfTime
        {
            public AheadOfTime()
            {
                Dump(typeof(UnityEngine.Application));
                Dump(typeof(UnityEngine.AudioClip));
                Dump(typeof(UnityEngine.AudioSource));
                Dump(typeof(UnityEngine.AudioType));
                Dump(typeof(UnityEngine.Camera));
                Dump(typeof(UnityEngine.Canvas));
                Dump(typeof(UnityEngine.Collider));
                Dump(typeof(UnityEngine.Collider[]));
                Dump(typeof(UnityEngine.Color));
                Dump(typeof(UnityEngine.ColorUtility));
                Dump(typeof(UnityEngine.GameObject));
                Dump(typeof(UnityEngine.HideInInspector));
                Dump(typeof(UnityEngine.Input));
                Dump(typeof(UnityEngine.LayerMask));
                Dump(typeof(UnityEngine.Mathf));
                Dump(typeof(UnityEngine.MeshRenderer));
                Dump(typeof(UnityEngine.MeshRenderer[]));
                Dump(typeof(UnityEngine.ParticleSystem));
                Dump(typeof(UnityEngine.ParticleSystem[]));
                Dump(typeof(UnityEngine.ParticleSystem.MainModule));
                Dump(typeof(UnityEngine.Physics));
                Dump(typeof(UnityEngine.Quaternion));
                Dump(typeof(UnityEngine.Random));
                Dump(typeof(UnityEngine.Rigidbody));
                Dump(typeof(UnityEngine.RuntimePlatform));
                Dump(typeof(UnityEngine.SceneManagement.SceneManager));
                Dump(typeof(UnityEngine.Time));
                Dump(typeof(UnityEngine.Transform[]));
                Dump(typeof(UnityEngine.UI.Image));
                Dump(typeof(UnityEngine.UI.Slider));
                Dump(typeof(UnityEngine.UI.Text));
                Dump(typeof(UnityEngine.Vector3));
            }
            void Dump(System.Type type)
            {
                UnityEngine.Debug.Log(type.Name);
            }
        }
    }
}