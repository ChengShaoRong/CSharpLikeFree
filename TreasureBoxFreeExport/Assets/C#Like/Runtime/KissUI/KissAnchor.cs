/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    [ExecuteInEditMode]
	/// <summary>
	/// The CanvasScaler component:
	/// "UI Scale Mode" : "Scale With Screen Size" 
	/// "Screen Match Mode" : "Match Width Or Height" 
	/// If it's portrait orientation game we set:
	/// "Match" : "0" 
	/// If it's landscape orientation game we set:
	/// "Match" : "1"
	/// </summary>
	public class KissAnchor : MonoBehaviour
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
		RectTransform mTransParent;
		RectTransform mTrans;
        void Start()
		{
			Canvas canvas = gameObject.GetComponentInParent<Canvas>();
			if (canvas != null)
				mTransParent = canvas.GetComponent<RectTransform>();
			mTrans = transform as RectTransform;
			SetPosition();
		}

		void LateUpdate()
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
			if (mTransParent == null)
				return;
			float width = mTransParent.rect.width;
			float height = mTransParent.rect.height;
			
			switch (side)
			{
				case Side.BottomLeft:
					mTrans.anchoredPosition = new Vector2(-width / 2f, -height / 2f);
					break;
				case Side.Left:
					transform.localPosition = new Vector2(- width / 2f, 0f);
					break;
				case Side.TopLeft:
					transform.localPosition = new Vector2(- width / 2f, height / 2f);
					break;
				case Side.Top:
					transform.localPosition = new Vector2(0f, height / 2f);
					break;
				case Side.TopRight:
					transform.localPosition = new Vector2(width / 2f, height / 2f);
					break;
				case Side.Right:
					transform.localPosition = new Vector2(width / 2f, 0f);
					break;
				case Side.BottomRight:
					transform.localPosition = new Vector2(width / 2f, -height / 2f);
					break;
				case Side.Bottom:
					transform.localPosition = new Vector2(0f, - height / 2f);
					break;
				case Side.Center:
					transform.localPosition = new Vector3(0f, 0f);
					break;
			}
		}
	}
}

