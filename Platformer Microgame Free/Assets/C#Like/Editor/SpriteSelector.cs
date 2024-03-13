/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEditor;

namespace CSharpLike
{
	public class SpriteSelector : ScriptableWizard
	{
		public static void Show(string name, Sprite[] sprites, string[] spriteNames, UnityEngine.Events.UnityAction<string> callback, string curName)
		{
			if (instance != null)
			{
				instance.Close();
				instance = null;
			}
			SpriteSelector spriteSelector = DisplayWizard<SpriteSelector>("Select a Sprite");
			spriteSelector.mSprites = sprites;
			spriteSelector.mSpriteNames = spriteNames;
			spriteSelector.mShowName = name;
			spriteSelector.mCallback = callback;
			spriteSelector.mCurName = curName;
		}
		string mCurName;
		string mShowName;
		Sprite[] mSprites;
		string[] mSpriteNames;
		static public SpriteSelector instance;
		void OnEnable() { instance = this; }
		void OnDisable() { instance = null; }

		UnityEngine.Events.UnityAction<string> mCallback;

		Vector2 mPos = Vector2.zero;
		void OnGUI()
		{
			bool close = false;
			GUILayout.Label(mShowName);

			if (mSprites == null || mSprites.Length == 0)
			{
				GUILayout.Label("The atlas doesn't have a sprite");
				return;
			}

			float size = 80f;
			float padded = size + 10f;
			int screenWidth = (int)EditorGUIUtility.currentViewWidth;
			int columns = Mathf.FloorToInt(screenWidth / padded);
			if (columns < 1) columns = 1;

			int offset = 0;
			Rect rect = new Rect(10f, 0, size, size);

			GUILayout.Space(10f);
			mPos = GUILayout.BeginScrollView(mPos);
			int rows = 1;

			GUIStyle fontStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
			GUIStyle fontStyleSelect = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
			while (offset < mSprites.Length)
			{
				GUILayout.BeginHorizontal();
				{
					int col = 0;
					rect.x = 10f;

					bool isPlaying = Application.isPlaying;
					for (; offset < mSprites.Length; ++offset)
					{
						Sprite sprite = mSprites[offset];
						if (sprite == null) continue;
						string strName = mSpriteNames[offset];

						if (GUI.Button(rect, ""))
						{
							mCallback(strName);
							close = true;
						}
						UnityEditor.UI.SpriteDrawUtilityUGUI.DrawSprite(sprite, rect, Color.white);
						bool isCurrent = mCurName == strName;
						GUI.contentColor = isCurrent ? Color.green : Color.white;
						GUI.Label(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), strName, isCurrent ? fontStyleSelect : fontStyle);
						GUI.backgroundColor = isCurrent ? Color.red : Color.white;
						GUI.Box(new Rect(rect.x, rect.y + rect.height, rect.width, 32f), GUIContent.none);
						GUI.contentColor = Color.white;
						GUI.backgroundColor = Color.white;

						if (++col >= columns)
						{
							++offset;
							break;
						}
						rect.x += padded;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(padded);
				rect.y += padded + 26;
				++rows;
			}
			GUILayout.Space(rows * 26);
			GUILayout.EndScrollView();

			if (close) Close();
		}
	}
}
