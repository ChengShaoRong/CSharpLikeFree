/*
 *           C#Like
 * Copyright © 2022-2023 RongRong
 * It's automatic generate by CSharpLikeEditor,don't modify this file.
 */

using System;
using UnityEngine;

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
                Dump(typeof(CSL_Type_Nullable<Vector2>));
                Dump(typeof(System.Collections.Generic.List<CSL_Type_Nullable<int>>));
                Dump(typeof(System.Collections.Generic.List<object>));
                Dump(typeof(System.Collections.Generic.Queue<GameObject>));
                Dump(typeof(System.DayOfWeek));
                Dump(typeof(System.Guid));
                Dump(typeof(System.IO.MemoryStream));
                Dump(typeof(System.IO.Path));
                Dump(typeof(Application));
                Dump(typeof(Collider2D));
                Dump(typeof(UnityEngine.EventSystems.EventTriggerType));
                Dump(typeof(GameObject));
                Dump(typeof(GUI));
                Dump(typeof(GUIStyle));
                Dump(typeof(Mathf));
                Dump(typeof(UnityEngine.Networking.DownloadHandlerScript));
                Dump(typeof(UnityEngine.Networking.UnityWebRequest));
                Dump(typeof(PlayerPrefs));
                Dump(typeof(UnityEngine.Random));
                Dump(typeof(Rect));
                Dump(typeof(RuntimePlatform));
                Dump(typeof(Screen));
                Dump(typeof(TextAsset));
                Dump(typeof(Time));
                Dump(typeof(UnityEngine.UI.Image));
                Dump(typeof(UnityEngine.UI.InputField));
                Dump(typeof(UnityEngine.UI.Text));
                Dump(typeof(Vector2));
                Dump(typeof(Vector3));
                Dump(typeof(Vector4));
                Dump(typeof(WaitForEndOfFrame));
                Dump(typeof(WaitForSeconds));
                Dump(typeof(AudioClip));
                Dump(typeof(AudioSource));
                Dump(typeof(Camera));
                Dump(typeof(Canvas));
                Dump(typeof(Collider));
                Dump(typeof(Collider[]));
                Dump(typeof(Color));
                Dump(typeof(ColorUtility));
                Dump(typeof(GameObject));
                Dump(typeof(HideInInspector));
                Dump(typeof(Input));
                Dump(typeof(LayerMask));
                Dump(typeof(Mathf));
                Dump(typeof(MeshRenderer));
                Dump(typeof(MeshRenderer[]));
                Dump(typeof(ParticleSystem));
                Dump(typeof(ParticleSystem[]));
                Dump(typeof(ParticleSystem.MainModule));
                Dump(typeof(Physics));
                Dump(typeof(Quaternion));
                Dump(typeof(System.Random));
                Dump(typeof(Rigidbody));
                Dump(typeof(UnityEngine.SceneManagement.SceneManager));
                Dump(typeof(Time));
                Dump(typeof(Transform[]));
                Dump(typeof(UnityEngine.UI.Image));
                Dump(typeof(UnityEngine.UI.Slider));
                Dump(typeof(UnityEngine.UI.Text));
                Dump(typeof(Vector3));
                Dump(typeof(WaitForSeconds));
                Dump(typeof(Cinemachine.CinemachineVirtualCamera));
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
                Dump(typeof(UnityEngine.Sprite));
                Dump(typeof(UnityEngine.Sprite[]));
                Dump(typeof(UnityEngine.SpriteRenderer));
                Dump(typeof(UnityEngine.Time));
                Dump(typeof(UnityEngine.Vector2));
                Dump(typeof(UnityEngine.Vector3));
                AssetBundleDump();
            }
            void ScriptDump()
            {

            }
            void AssetBundleDump()
            {
                Dump(typeof(Camera));
                Dump(typeof(Material));
                Dump(typeof(MeshRenderer));
                Dump(typeof(Texture2D));
                Dump(typeof(MeshFilter));
                Dump(typeof(Mesh));
                Dump(typeof(Shader));
                Dump(typeof(TextAsset));
                Dump(typeof(Rigidbody));
                Dump(typeof(MeshCollider));
                Dump(typeof(BoxCollider));
                Dump(typeof(ComputeShader));
                Dump(typeof(AnimationClip));
                Dump(typeof(AudioListener));
                Dump(typeof(AudioSource));
                Dump(typeof(AudioClip));
                Dump(typeof(Cubemap));
                Dump(typeof(Avatar));
                //Dump(typeof(AnimatorController));
                Dump(typeof(RenderSettings));
                Dump(typeof(Light));
                //Dump(typeof(MonoScript));
                Dump(typeof(FlareLayer));
                Dump(typeof(Font));
                Dump(typeof(CapsuleCollider));
                Dump(typeof(SkinnedMeshRenderer));
                Dump(typeof(LightmapSettings));
                //Dump(typeof(NavMeshSettings));
                Dump(typeof(ParticleSystem));
                Dump(typeof(ParticleSystemRenderer));
                Dump(typeof(Sprite));
                Dump(typeof(CanvasRenderer));
                Dump(typeof(Canvas));
                Dump(typeof(RectTransform));
                //Dump(typeof(AudioMixerController));
                //Dump(typeof(AudioMixerGroupController));
                //Dump(typeof(AudioMixerSnapshotController));
                Dump(typeof(LightProbes));
                Dump(typeof(LightingSettings));
                Dump(typeof(CircleCollider2D));
                Dump(typeof(PolygonCollider2D));
                Dump(typeof(CapsuleCollider2D));
                Dump(typeof(BoxCollider2D));
                Dump(typeof(SortingLayer));
                Dump(typeof(UnityEngine.Rendering.SortingGroup));
                Dump(typeof(Grid));
                Dump(typeof(UnityEngine.Tilemaps.Tilemap));
                Dump(typeof(UnityEngine.Tilemaps.TilemapRenderer));
                Dump(typeof(UnityEngine.Tilemaps.TilemapCollider2D));
            }
            void Dump(Type type)
            {
                Debug.Log(type.Name);
            }
        }
    }
}