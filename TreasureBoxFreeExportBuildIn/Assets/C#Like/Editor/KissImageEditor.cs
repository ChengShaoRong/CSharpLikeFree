/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor;
using UnityEditor.UI;

namespace CSharpLike
{
	/// <summary>
	/// Editor class used to edit UI Sprites.
	/// </summary>

	[CustomEditor(typeof(KissImage), true)]
	[CanEditMultipleObjects]
	/// <summary>
	///   Custom Editor for the KissImage Component.
	///   Extend this class to write a custom editor for a component derived from KissImage.
	/// </summary>
	public class KissImageEditor : ImageUGUIEditor
	{
		SerializedProperty m_SpriteAtlas;
		SerializedProperty m_SpriteName;
		GUIContent m_SpriteAtlasContent;

		protected override void OnEnable()
		{

			m_SpriteAtlasContent = EditorGUIUtility.TrTextContent("Sprite Atlas");

			m_SpriteAtlas = serializedObject.FindProperty("m_SpriteAtlas");
			m_SpriteName = serializedObject.FindProperty("m_SpriteName");
			OnSpriteAltlasChanged();
			base.OnEnable();
		}   
		Sprite[] sprites = null;
		string[] spriteNames = null;

		void OnSpriteAltlasChanged()
		{
			SpriteAtlas spriteAtlas = m_SpriteAtlas.objectReferenceValue as SpriteAtlas;
			if (spriteAtlas != null && spriteAtlas.spriteCount > 0)
			{
				int count = spriteAtlas.spriteCount;
				sprites = new Sprite[count];
				count = spriteAtlas.GetSprites(sprites);
				spriteNames = new string[count];
				int currentIndex = 0;
				for (int i = 0; i < count; i++)
				{
					string strName = sprites[i].name;
					if (strName.EndsWith("(Clone)"))
						strName = strName.Substring(0, strName.Length - 7);
					spriteNames[i] = strName;
					if (strName == m_SpriteName.stringValue)
					{
						currentIndex = i;
					}
				}

				m_SpriteName.stringValue = spriteNames[currentIndex];
				sprite = sprites[currentIndex];
			}
			else
				sprite = null;
		}
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(m_SpriteAtlas, m_SpriteAtlasContent);
			SpriteAtlas spriteAtlas = m_SpriteAtlas.objectReferenceValue as SpriteAtlas;
			if (EditorGUI.EndChangeCheck())
				OnSpriteAltlasChanged();
			if (spriteAtlas != null && spriteAtlas.spriteCount > 0)
			{
				if (sprites == null)
					OnSpriteAltlasChanged();
				GUILayout.BeginHorizontal();
				if (GUI.Button(new Rect(0, 22, 100, 20), "Sprite"))
                {
					SpriteSelector.Show(spriteAtlas.name, sprites, spriteNames, (string spriteName) =>
					{
						serializedObject.Update();
						m_SpriteName.stringValue = spriteName;
						OnSpriteAltlasChanged();
						serializedObject.ApplyModifiedProperties();
					}, m_SpriteName.stringValue);
				}
				GUILayout.Space(100f);
				GUILayout.Label(m_SpriteName.stringValue);
				GUILayout.EndHorizontal();
			}
			serializedObject.ApplyModifiedProperties();
			base.OnInspectorGUI();
		}
	}
}
