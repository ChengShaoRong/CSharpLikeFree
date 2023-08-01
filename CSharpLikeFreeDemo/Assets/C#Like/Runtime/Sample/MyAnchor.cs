/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using UnityEngine;

namespace CSharpLike
{
    [ExecuteInEditMode]
	/// <summary>
	/// This script can be used to anchor an object to the side or corner of the screen, panel, or a widget.
	/// </summary>
	public class MyAnchor : MonoBehaviour
	{
		public enum Side
		{
			BottomLeft,
			Left,
			TopLeft,
			Top,
			TopRight,
			Right,
			BottomRight,
			Bottom,
			Center,
		}
		public Side side = Side.Center;
		public bool runOnlyOnce = true;

#if UNITY_EDITOR
		void OnValidate() { Start(); }
#endif
		MyRoot myRoot;
		void Start()
		{
			myRoot = gameObject.GetComponentInParent<MyRoot>();
			SetPosition();
		}

		void Update()
		{
			if (!runOnlyOnce)
				SetPosition();
#if UNITY_EDITOR
			else
				SetPosition();
#endif
		}

		void SetPosition()
		{
			float scale = myRoot != null ? myRoot.scale : 1f;
			float width = Screen.width / scale;
			float height = Screen.height / scale;
			switch (side)
			{
				case Side.BottomLeft:
					transform.localPosition = new Vector3(- width / 2f, - height / 2f, transform.localPosition.z);
					break;
				case Side.Left:
					transform.localPosition = new Vector3(- width / 2f, 0f, transform.localPosition.z);
					break;
				case Side.TopLeft:
					transform.localPosition = new Vector3(- width / 2f, height / 2f, transform.localPosition.z);
					break;
				case Side.Top:
					transform.localPosition = new Vector3(0f, height / 2f, transform.localPosition.z);
					break;
				case Side.TopRight:
					transform.localPosition = new Vector3(width / 2f, height / 2f, transform.localPosition.z);
					break;
				case Side.Right:
					transform.localPosition = new Vector3(width / 2f, 0f, transform.localPosition.z);
					break;
				case Side.BottomRight:
					transform.localPosition = new Vector3(width / 2f, -height / 2f, transform.localPosition.z);
					break;
				case Side.Bottom:
					transform.localPosition = new Vector3(0f, - height / 2f, transform.localPosition.z);
					break;
				case Side.Center:
					transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);
					break;
			}
		}
	}
}

