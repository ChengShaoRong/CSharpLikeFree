/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace CSharpLike
{
	[AddComponentMenu("KissUI/KissImage")]
	public class KissImage : ImageUGUI
	{
		/// <summary>
		/// The sprite name of this KissImage in SpriteAtlas
		/// </summary>
		public string SpriteName
		{
			get
			{
				return m_SpriteName;
			}
			set
			{
				if (m_SpriteName != value)
				{
					sprite = m_SpriteAtlas.GetSprite(value);
					if (sprite == null)
					{
						Debug.LogError($"KissImage '{gameObject.name}' not exist Sprite name '{value}' in SpriteAtlas '{m_SpriteAtlas.name}'");
						m_SpriteName = "";
					}
					else
						m_SpriteName = value;
				}
			}
		}
		#region internal impl
		[SerializeField] SpriteAtlas m_SpriteAtlas;
		[SerializeField] string m_SpriteName;
		protected override void Awake()
		{
			SetSprite();
		}
		void SetSprite()
		{
			if (m_SpriteAtlas != null && !string.IsNullOrEmpty(m_SpriteName))
			{
				sprite = m_SpriteAtlas.GetSprite(m_SpriteName);
				if (sprite == null)
					m_SpriteName = "";
			}
		}
#if UNITY_EDITOR
        protected override void OnValidate()
		{
			SetSprite();
		}
#endif
        #endregion
    }
}

