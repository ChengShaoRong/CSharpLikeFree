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
                Dump(typeof(Cinemachine.CinemachineVirtualCamera));
                Dump(typeof(System.Collections.Generic.List<UnityEngine.Transform>));
                Dump(typeof(UnityEngine.Animator));
                Dump(typeof(UnityEngine.AudioClip));
                Dump(typeof(UnityEngine.AudioSource));
                Dump(typeof(UnityEngine.Bounds));
                Dump(typeof(UnityEngine.Camera));
                Dump(typeof(UnityEngine.Canvas));
                Dump(typeof(UnityEngine.Canvas[]));
                Dump(typeof(UnityEngine.Collider2D));
                Dump(typeof(UnityEngine.Collision2D));
                Dump(typeof(UnityEngine.ContactFilter2D));
                Dump(typeof(UnityEngine.GameObject));
                Dump(typeof(UnityEngine.GameObject[]));
                Dump(typeof(UnityEngine.Input));
                Dump(typeof(UnityEngine.Mathf));
                Dump(typeof(UnityEngine.MonoBehaviour));
                Dump(typeof(UnityEngine.ParticleSystem));
                Dump(typeof(UnityEngine.Physics2D));
                Dump(typeof(UnityEngine.Random));
                Dump(typeof(UnityEngine.RaycastHit2D));
                Dump(typeof(UnityEngine.RaycastHit2D[]));
                Dump(typeof(UnityEngine.RequireComponent));
                Dump(typeof(UnityEngine.Rigidbody2D));
                Dump(typeof(UnityEngine.SceneManagement.SceneManager));
                Dump(typeof(UnityEngine.Sprite));
                Dump(typeof(UnityEngine.Sprite[]));
                Dump(typeof(UnityEngine.SpriteRenderer));
                Dump(typeof(UnityEngine.Time));
                Dump(typeof(UnityEngine.Vector2));
                Dump(typeof(UnityEngine.Vector3));
            }
            void Dump(System.Type type)
            {
                UnityEngine.Debug.Log(type.Name);
            }
        }
    }
}