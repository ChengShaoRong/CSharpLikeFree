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
                Dump(typeof(JSONData));
                Dump(typeof(System.Collections.Generic.Dictionary<string,JSONData>));
                Dump(typeof(System.Collections.Generic.List<JSONData>));
                Dump(typeof(System.Collections.Generic.List<UnityEngine.Vector3>));
                Dump(typeof(System.DayOfWeek));
                Dump(typeof(System.Threading.Interlocked));
                Dump(typeof(System.Threading.Thread));
                Dump(typeof(UnityEngine.Application));
                Dump(typeof(UnityEngine.Bounds));
                Dump(typeof(UnityEngine.BoundsInt));
                Dump(typeof(UnityEngine.Collider2D));
                Dump(typeof(UnityEngine.Color));
                Dump(typeof(UnityEngine.Color32));
                Dump(typeof(UnityEngine.Events.UnityAction<bool>));
                Dump(typeof(UnityEngine.Events.UnityAction<byte>));
                Dump(typeof(UnityEngine.Events.UnityAction<CSharpLike.HotUpdateBehaviour,bool>));
                Dump(typeof(UnityEngine.Events.UnityAction<CSharpLike.HotUpdateBehaviour,int>));
                Dump(typeof(UnityEngine.Events.UnityAction<CSharpLike.HotUpdateBehaviour,JSONData>));
                Dump(typeof(UnityEngine.Events.UnityAction<CSharpLike.HotUpdateBehaviour,string>));
                Dump(typeof(UnityEngine.Events.UnityAction<CSharpLike.HotUpdateBehaviour>));
                Dump(typeof(UnityEngine.Events.UnityAction<double>));
                Dump(typeof(UnityEngine.Events.UnityAction<float>));
                Dump(typeof(UnityEngine.Events.UnityAction<int>));
                Dump(typeof(UnityEngine.Events.UnityAction<JSONData>));
                Dump(typeof(UnityEngine.Events.UnityAction<object>));
                Dump(typeof(UnityEngine.Events.UnityAction<string>));
                Dump(typeof(UnityEngine.Events.UnityAction<System.Collections.Generic.Dictionary<int,bool>>));
                Dump(typeof(UnityEngine.Events.UnityAction<System.Collections.Generic.Dictionary<int,int>>));
                Dump(typeof(UnityEngine.Events.UnityAction<System.Collections.Generic.Dictionary<int,string>>));
                Dump(typeof(UnityEngine.Events.UnityAction<System.Collections.Generic.Dictionary<string,bool>>));
                Dump(typeof(UnityEngine.Events.UnityAction<System.Collections.Generic.Dictionary<string,int>>));
                Dump(typeof(UnityEngine.Events.UnityAction<System.Collections.Generic.Dictionary<string,string>>));
                Dump(typeof(UnityEngine.Events.UnityAction<System.Collections.Generic.List<bool>>));
                Dump(typeof(UnityEngine.Events.UnityAction<System.Collections.Generic.List<int>>));
                Dump(typeof(UnityEngine.Events.UnityAction<System.Collections.Generic.List<string>>));
                Dump(typeof(UnityEngine.Events.UnityAction<UnityEngine.GameObject>));
                Dump(typeof(UnityEngine.Events.UnityAction<UnityEngine.Transform>));
                Dump(typeof(UnityEngine.EventSystems.EventTriggerType));
                Dump(typeof(UnityEngine.GameObject));
                Dump(typeof(UnityEngine.GUI));
                Dump(typeof(UnityEngine.GUIStyle));
                Dump(typeof(UnityEngine.HideInInspector));
                Dump(typeof(UnityEngine.Input));
                Dump(typeof(UnityEngine.LayerMask));
                Dump(typeof(UnityEngine.Mathf));
                Dump(typeof(UnityEngine.Matrix4x4));
                Dump(typeof(UnityEngine.Quaternion));
                Dump(typeof(UnityEngine.Random));
                Dump(typeof(UnityEngine.Rect));
                Dump(typeof(UnityEngine.RectInt));
                Dump(typeof(UnityEngine.RuntimePlatform));
                Dump(typeof(UnityEngine.SceneManagement.SceneManager));
                Dump(typeof(UnityEngine.Screen));
                Dump(typeof(UnityEngine.SerializeField));
                Dump(typeof(UnityEngine.TextAsset));
                Dump(typeof(UnityEngine.Time));
                Dump(typeof(UnityEngine.UI.CanvasScaler));
                Dump(typeof(UnityEngine.UI.Image));
                Dump(typeof(UnityEngine.UI.Text));
                Dump(typeof(UnityEngine.Vector2));
                Dump(typeof(UnityEngine.Vector2Int));
                Dump(typeof(UnityEngine.Vector3));
                Dump(typeof(UnityEngine.Vector3Int));
                Dump(typeof(UnityEngine.Vector4));
            }
            void Dump(System.Type type)
            {
                UnityEngine.Debug.Log(type.Name);
            }
        }
    }
}