//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        void TestThread()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                Debug.LogError("Test Thread: Not supported 'lock syntax' in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
                // We provide the following solutions if you not update to full version:
                // lock syntax: Do it in not hot update script. Or don't use lock (May cause multi-threading problem).

                //easy use for thread with no param
                HotUpdateManager.CreateThread(TestThreadRunLoop);

                //Task.Run
                Task.Run(() =>
                {
                    Debug.LogError("Task.Run as lambda start " + DateTime.Now);
                    Thread.Sleep(2000);
                    Debug.LogError("Task.Run as lambda end " + DateTime.Now);
                });
                Task.Run(TestTaskRun);
            }
            else
                Debug.LogError("Test Thread:WEBGL can't use thread.");
        }
        void TestTaskRun()
        {
            Debug.LogError("Task.Run as delegate start " + DateTime.Now);
            Thread.Sleep(4000);
            Debug.LogError("Task.Run as delegate end " + DateTime.Now);
        }
        /// <summary>
        /// Call by Button Component of 'ButtonTestThread' in prefab.
        /// </summary>
        void OnClickTestThread()
        {
            strTestMessage.text = "OnClickTestThread";
            //easy use for thread with param
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["id"] = Random.Range(1, 1000);
            jsonData["data"] = JSONData.NewList();
            jsonData["data"].Add("dump1");
            jsonData["data"].Add("dump2");
            List<int> testParams = new List<int>();
            testParams.Add(123);
            jsonData.SetObjectExtern("TestExternMsg", testParams);//Set extern params to thread
            HotUpdateManager.CreateThread(DoSomeWorkInThread,//This's the function which call in thread
                jsonData,//This's the param which call in thread and callback to main thread.
                behaviour,//the current behaviour.let it null if you don't care the callback
                OnDoSomeWorkInThreadDone);//This's the function which callback in main thread.let it null if you don't care the callback

            //let 'TestThreadRunLoop' print log by change the value
            countInThread = Random.Range(1, 1000);
        }
        bool bExist = false;
        void OnDestroy()
        {
            bExist = true;//notify thread to stop while main thread about to exist
        }
        int countInThread = 0;
        volatile int countLoopThread = 0;//test 'volatile' keyword
        /// <summary>
        /// work in thread with param
        /// </summary>
        void TestThreadRunLoop()
        {
            Debug.LogError("TestThreadRunLoop start");
            while (!bExist)//until this component destroy
            {
                if (countInThread > 0)
                {
                    Debug.LogError("TestThreadRunLoop:countInThread=" + countInThread);
                    countInThread = 0;
                }
                if ((++countLoopThread) >= 10000)
                    countLoopThread = 0;
                Thread.Sleep(200);//sleep 0.2 second in the loop
            }
            Debug.LogError("TestThreadRunLoop end");
        }
        /// <summary>
        /// work in thread with no param
        /// </summary>
        void DoSomeWorkInThread(object obj)
        {
            //We transfer params by 'JSONData'.
            //You can storage your data in the JSONData
            JSONData jsonData = obj as JSONData;

            List<int> testParams = jsonData.GetObjectExtern("TestExternMsg") as List<int>;//test extern param
            Debug.Log("DoSomeWorkInThread testParams.Count=" + testParams.Count);
            //do some work (you can't accept the component of unity in this thread, such as 'PlayerPrefs', just same as in native C#.)
            Debug.Log("DoSomeWorkInThread(" + jsonData + ") start at " + DateTime.Now);
            long count = 0;
            int id = jsonData["id"];
            for (int i = 0; i < 10000; i++)
            {
                id += jsonData.Count;//Don't direct use 'jsonData["id"] += jsonData.Count;', because JSONData is not efficient in hot update script in this block which loop 10000 times.
                Interlocked.Increment(ref count);
            }
            jsonData["id"] = id;
            Debug.Log("DoSomeWorkInThread(" + jsonData + ") end at " + DateTime.Now);

            //callback to main thread for show the result
            HotUpdateManager.OnThreadDone(jsonData);//mark work done and send to main thread
        }
        /// <summary>
        /// refresh UI in main thread when thread done
        /// </summary>
        void OnDoSomeWorkInThreadDone(object obj)
        {
            JSONData jsonData = obj as JSONData;
            strTestMessage.text = "OnDoSomeWorkInThreadDone:"+ jsonData["id"];
            Debug.Log("OnDoSomeWorkInThreadDone:" + jsonData["id"]);
        }
    }
}