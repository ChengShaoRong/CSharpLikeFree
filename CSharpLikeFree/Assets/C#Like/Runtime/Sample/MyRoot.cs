/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    [ExecuteInEditMode]
	/// <summary>
	/// This is a script used to keep the game object scaled to resolution
	/// </summary>
	public class MyRoot : MonoBehaviour
	{
		public static MyRoot root;
#if UNITY_EDITOR
		void OnValidate() { Start(); }
#endif
		/// <summary>
		/// resolution width.
		/// </summary>
		public int resolutionWidth = 720;

		/// <summary>
		/// resolution height
		/// </summary>
		public int resolutionHeight = 1560;

		/// <summary>
		/// fit resolution width, or else fit resolution height
		/// </summary>
		public bool fitWidth = false;

		/// <summary>
		/// resolution scale
		/// </summary>
		public float scale
		{
			get
			{
				return fitWidth ? (Screen.width / (float)resolutionWidth) : (Screen.height / (float)resolutionHeight);
			}
		}
		void Start()
		{
			root = this;
			UpdateScale();
		}

		void Update()
		{
			UpdateScale();
		}

		void UpdateScale()
		{
			float fScale = scale;
			transform.localScale = new Vector3(fScale, fScale, fScale);
		}
        /// <summary>
        /// Get mouse position with scale.
        /// </summary>
        public static Vector3 GetMousePosition()
        {
            float scale = root.scale;
            Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            pos.x /= scale;
            pos.y /= scale;
            pos.x -= Screen.width / scale / 2f;
            pos.y -= Screen.height / scale / 2f;
            return pos;
        }
    }
}

