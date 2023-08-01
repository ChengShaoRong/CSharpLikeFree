//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Example for interactive prefab data
    /// </summary>
    public class SampleInteractivePrefabData : LikeBehaviour
    {
        void Start()
        {
            //the instance of the HotUpdateBehaviour which inherit the MonoBehaviour.
            Debug.Log("HotUpdateBehaviour=" + behaviour);
            //same with the gameObject of MonoBehaviour
            Debug.Log("gameObject.name=" + gameObject.name);
            //same with the transform of MonoBehaviour
            Debug.Log("transform.localPosition=" + transform.localPosition);

            //We can get/set value(int/bool/float/double/string) which binding in prefab(key by string).
            //Here are the example for the integer value.
            //Get the integer value which binding in prefab.
            //For example,we blinding the key 'iValueA' with value 123 in prefab.
            Debug.Log("integer value set in prefab:iValue=" + GetInt("iValueA"));//GetBoolean/GetDouble/GetFloat/GetString
            //Modify the integer value, It will refresh the value in editor.
            //You also can modify it in editor when running.
            SetInt("iValueA", 321);//SetBoolean/SetDouble/SetFloat/SetString
            //Verify the integer value after modify
            Debug.Log("integer value after modify:iValue=" + GetInt("iValueA"));

            //We can accept the Unity Component(GameObject/TextAsset/Material/Texture/AudioClip) which binding in prefab(key by string)
            //Here are the example for get the 'GameObject' which bind at the gameObjects in HotUpdateBehaviour 
            GameObject goStart = GetGameObject("ButtonStart");
            goStart.SetActive(true);

            //Here are the example for get the UnityEngine.UI.Text in prefab.
            //You can get 'runtime' component which bind at the gameObjects in HotUpdateBehaviour.
            //You also can get any type which can get by GameObject.GetComponent<T>() in MonoBehaviour.
            //such as Image/Button/Slider/MeshRenderer/...
            Text text = GetComponent<Text>("TextMsg");

            //Here are the example for get TextAsset 'resource' which bind in prefab.
            //Such as we bind some TextAsset resource to prefab, that can't drag into GameObject at editor.
            TextAsset ta = GetTextAsset("LoadTextAsset");//GetMaterial/GetTexture/GetAudioClip
            Debug.Log("Load TextAsset:" + ta.text);
            text.text = ta.text;
        }
        void OnClick()
        {
            SetInt("iValueA", GetInt("iValueA") + 1);
            GetComponent<Text>("TextMsg").text = DateTime.Now.ToString() + " : iValueA=" + GetInt("iValueA");
        }

        void OnClickBack()
        {
            HotUpdateManager.Show("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");//back to SampleCSharpLikeHotUpdate
            HotUpdateManager.Hide("Assets/C#Like/Sample/SampleInteractivePrefabData.prefab");//close self
        }
    }
}