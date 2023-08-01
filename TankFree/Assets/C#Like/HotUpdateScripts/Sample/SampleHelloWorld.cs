//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using System;
using System.Collections;

namespace CSharpLike
{
    /// <summary>
    /// Example for HelloWorld.
    /// Show the most simple usage of the Unity periodic functions.(support all MonoBehaviour event,but we just show the some of them.)
    /// And the usage of coroutine
    /// </summary>
    public class SampleHelloWorld : LikeBehaviour
    {
        void Awake()
        {
            gameObject.name = "I'm HotUpdateBehaviour";
            Debug.Log("Awake:same with MonoBehaviour");
        }
        void OnEnable()
        {
            Debug.Log("OnEnable:same with MonoBehaviour");
        }

        //in FREE version:
        int stepStart = 0;
        void Start()
        {
            if (stepStart == 0)
            {
                Debug.Log("Start:same with MonoBehaviour but can't use 'coroutine'");
                Debug.LogError("Test Coroutine: You can't use 'coroutine' in hot update script in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
                // We provide the following solutions if you not update to full version:
                // coroutine: using MemberCallDelay to simulate coroutine (Only simulate 'yield return new WaitForSeconds(float seconds);' and 'yield return null;').
                // The coroutine is great invention, it make our code logic clear and tidy, you should not give up it.
                Debug.LogError("Start:step0: " + DateTime.Now);
                stepStart++;
                MemberCallDelay("Start", 2f);
            }
            else if (stepStart == 1)
            {
                Debug.LogError("Start:step1: " + DateTime.Now);
                stepStart++;
                MemberCallDelay("Start");
            }
            else if (stepStart == 2)
            {
                Debug.LogError("Start:step2: " + DateTime.Now);
                stepStart++;
                SubCoroutine("test", 123, 321f);
            }
            else if (stepStart == 3)
            {
                Debug.LogError("Start:step3: " + DateTime.Now);
                stepStart = 0;
            }
        }
        int stepSubCoroutine = 0;
        void SubCoroutine(string str, int iValue, float fValue)
        {
            if (stepSubCoroutine == 0)
            {
                Debug.LogError("SubCoroutine:SubCoroutine(" + str + "," + iValue + "," + ") " + DateTime.Now);
                stepSubCoroutine++;
                MemberCallDelay("SubCoroutine", str, iValue, fValue, 2f);
            }
            else if (stepSubCoroutine == 1)
            {
                Debug.LogError("SubCoroutine:end " + DateTime.Now);
                MemberCallDelay("Start");
                stepSubCoroutine = 0;
            }
        }
        ////in full version:
        //IEnumerator Start()
        //{
        //    Debug.Log("Start:same with MonoBehaviour");
        //    Debug.LogError("Start:step0: " + DateTime.Now);
        //    yield return new WaitForSeconds(2f);
        //    Debug.LogError("Start:step1: " + DateTime.Now);
        //    yield return null;
        //    Debug.LogError("Start:step2: " + DateTime.Now);
        //    yield return StartCoroutine("SubCoroutine", "test", 123, 321f);
        //    Debug.LogError("Start:step3: " + DateTime.Now);
        //}
        //IEnumerator SubCoroutine(string str, int iValue, float fValue)
        //{
        //    Debug.LogError("SubCoroutine:SubCoroutine(" + str + "," + iValue + "," + ") " + DateTime.Now);
        //    //In full version, supported all kind of coroutine, not just WaitForSeconds.
        //    yield return new WaitForSeconds(2f);
        //    Debug.LogError("SubCoroutine:end " + DateTime.Now);
        //}
        void FixedUpdate()
        {
            Debug.Log("FixedUpdate:same with MonoBehaviour");
        }
        float angle = 0f;
        void Update()
        {
            //Debug.Log("LateUpdate:same with MonoBehaviour");
            angle += Time.deltaTime * 50f;
            transform.localEulerAngles = new Vector3(0f, angle, 0f);
        }
        void LateUpdate()
        {
            Debug.Log("LateUpdate:same with MonoBehaviour");
        }
        void OnGUI()
        {
            //Debug.Log("OnGUI:same with MonoBehaviour");
            if (GUI.Button(new Rect(0, 0, 64, 64), "Back"))
            {
                HotUpdateManager.Show("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");//back to SampleCSharpLikeHotUpdate
                HotUpdateManager.Hide("Assets/C#Like/Sample/SampleHelloWorld.prefab");//close self
            }
        }
        void OnDisable()
        {
            Debug.Log("OnDisable:same with MonoBehaviour");
        }
        void OnDestroy()
        {
            Debug.Log("OnDestroy:same with MonoBehaviour");
        }
    }
}