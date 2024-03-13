//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        public GameObject goTestDelegate;
        public GameObject goTestLambda;
        void TestDelegateAndLambda()
        {
            Debug.LogError("Test delegate and lambda:");
            //test Delegate
            HotUpdateManager.AddEventTrigger(goTestDelegate, EventTriggerType.PointerClick, OnClickDelegate);
            //test Lambda
            HotUpdateManager.AddEventTrigger(goTestLambda, EventTriggerType.PointerClick,
                (eventData) =>//"BaseEventData eventData" can be omitted as "eventData" since version 2024.1
                {
                    strTestMessage.text = "OnClickLambda";
                    Debug.Log("On click lambda :" + eventData);
                });
        }
        void OnClickDelegate(BaseEventData eventData)
        {
            strTestMessage.text = "OnClickDelegate";
            Debug.Log("OnClickDelegate:" + eventData);
        }
        /// <summary>
        /// Call by Button Component of 'ButtonTestBind' in prefab.
        /// This's the most simplest way bind OnClick event by Button.
        /// </summary>
        void OnClickBindButton()
        {
            strTestMessage.text = "OnClickBindButton";
            Debug.Log("OnClickBindButton:");
        }
        /// <summary>
        /// Call by EventTrigger Component of 'ButtonTestBind' in prefab.
        /// It's not recommend because only one EventTriggerType in one LikeBehaviour,and can't change the method name.
        /// Recommend use HotUpdateManager.AddEventTrigger.
        /// </summary>
        void OnPointerEnter(BaseEventData eventData)
        {
            strTestMessage.text = "OnPointerEnter";
            Debug.Log("OnPointerEnter:" + eventData);
        }
    }
}