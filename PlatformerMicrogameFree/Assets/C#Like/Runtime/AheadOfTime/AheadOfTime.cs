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
                Dump(typeof(System.Collections.Generic.List<object>));
                Dump(typeof(System.Collections.Generic.List<UnityEngine.Vector3>));
                Dump(typeof(System.Collections.Generic.Queue<UnityEngine.GameObject>));
                Dump(typeof(System.DayOfWeek));
                Dump(typeof(System.Guid));
                Dump(typeof(System.Threading.Interlocked));
                Dump(typeof(System.Threading.Thread));
                Dump(typeof(UnityEngine.Application));
                Dump(typeof(UnityEngine.Collider2D));
                Dump(typeof(UnityEngine.EventSystems.EventTriggerType));
                Dump(typeof(UnityEngine.GameObject));
                Dump(typeof(UnityEngine.GUI));
                Dump(typeof(UnityEngine.GUIStyle));
                Dump(typeof(UnityEngine.Mathf));
                Dump(typeof(UnityEngine.PlayerPrefs));
                Dump(typeof(UnityEngine.Random));
                Dump(typeof(UnityEngine.Rect));
                Dump(typeof(UnityEngine.RuntimePlatform));
                Dump(typeof(UnityEngine.SceneManagement.SceneManager));
                Dump(typeof(UnityEngine.Screen));
                Dump(typeof(UnityEngine.TextAsset));
                Dump(typeof(UnityEngine.Time));
                Dump(typeof(UnityEngine.UI.Image));
                Dump(typeof(UnityEngine.UI.InputField));
                Dump(typeof(UnityEngine.UI.Text));
                Dump(typeof(UnityEngine.Vector2));
                Dump(typeof(UnityEngine.Vector3));
                Dump(typeof(UnityEngine.Vector4));
            }
            void Dump(System.Type type)
            {
                UnityEngine.Debug.Log(type.Name);
            }
        }
    }
}