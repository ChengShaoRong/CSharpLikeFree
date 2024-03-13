//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Example for interactive prefab data
    /// </summary>
    public class SampleInteractivePrefabData : LikeBehaviour
    {
        [Tooltip("This the Text content for show message")]//Test attribute : [Tooltip("XXXX")], same with MonoBehaviour
        [SerializeField]//Test attribute : [SerializeField], same with MonoBehaviour
        Text textMsg;
        [Range(1, 10)]//Test attribute for List : [Range(X,Y)], same with MonoBehaviour
        public List<int> listInts;//Test List field
        [System.NonSerialized]//Test attribute : [System.NonSerialized], same with MonoBehaviour
        public int iNonValue;
        [HideInInspector]//Test attribute : [HideInInspector], same with MonoBehaviour
        public int iHideValue;
        public string strValue;
        public char charValue;
        public long longValue;
        public ulong ulongValue;
        [Range(1, 5)]//Test attribute : [Range(X,Y)]
        public int intValue = 2;//Test with default value, same with MonoBehaviour
        public uint uintValue;
        public short shortValue;
        public ushort ushortValue;
        public byte byteValue;
        public sbyte sbyteValue;
        public float floatValue;
        public double doubleValue;
        public bool boolValue;
        public Color colorValue = Color.green;//Test with default value, same with MonoBehaviour
        public Color32 color32Value;
        public JSONData JSONDataValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
        public Vector4 vector4Value;
        public Vector2Int vector2IntValue;
        public Vector3Int vector3IntValue;
        public Matrix4x4 matrix4x4Value;
        public Rect rectValue;
        public RectInt rectIntValue;
        public Bounds boundsValue;
        public BoundsInt boundsIntValue;
        public Quaternion quaternionValue;
        public LayerMask layerMaskValue;


        public int iValueA = 1234567;
        public GameObject buttonStart;
        public TextAsset textAsset;
        void Start()
        {
            //the instance of the HotUpdateBehaviour which inherit the MonoBehaviour.
            Debug.Log("HotUpdateBehaviour=" + behaviour);
            //same with the gameObject of MonoBehaviour
            Debug.Log("gameObject.name=" + gameObject.name);
            //same with the transform of MonoBehaviour
            Debug.Log("transform.localPosition=" + transform.localPosition);

            Debug.Log("integer value set in prefab:iValue=" + iValueA);
            //Modify the integer value, It will refresh the value in editor.
            //You also can modify it in editor when running.
            iValueA = 321;
            //Verify the integer value after modify
            Debug.Log("integer value after modify:iValue=" + iValueA);

            //We can accept the Unity Component(GameObject/TextAsset/Material/Texture/AudioClip) which binding in prefab(key by string)
            //Here are the example for get the 'GameObject' which bind at the gameObjects in HotUpdateBehaviour 
            buttonStart.SetActive(true);

            //Here are the example for get the UnityEngine.UI.Text in prefab.
            //You can get 'runtime' component which bind at the gameObjects in HotUpdateBehaviour.
            //You also can get any type which can get by GameObject.GetComponent<T>() in MonoBehaviour.
            //such as Image/Button/Slider/MeshRenderer/...

            //Here are the example for get TextAsset 'resource' which bind in prefab.
            //Such as we bind some TextAsset resource to prefab, that can't drag into GameObject at editor.
            Debug.Log("Load TextAsset:" + textAsset.text);
            textMsg.text = textAsset.text;
        }
        void OnClick()
        {
            iValueA++;
            textMsg.text = DateTime.Now.ToString() + " : iValueA=" + iValueA;
        }

        void OnClickBack()
        {
            HotUpdateManager.Show("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleCSharpLikeHotUpdate.prefab");//back to SampleCSharpLikeHotUpdate
            HotUpdateManager.Hide("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleInteractivePrefabData.prefab");//close self
        }
    }
}