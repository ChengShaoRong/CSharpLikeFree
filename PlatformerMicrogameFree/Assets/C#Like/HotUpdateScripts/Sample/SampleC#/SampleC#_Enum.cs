//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using System;
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        void TestEnum()
        {
            Debug.LogError("Test Enum: You can use enum but can't DEFINE enum in hot update script in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // enum: You can direct use integer number instead of define enum in hot update script.
            // In FREE version, you still can use enum which defined in not hot update script.
            Debug.Log("Test use the enum of the normal script: DayOfWeek = " + DayOfWeek.Saturday);//output Saturday
            Debug.Log("Test use the enum of the normal script that inside class: TestClassWithEnum.TestEnumInClass.Abc = " + TestClassWithEnum.TestEnumInClass.Abc);//output Abc
        }
    }
}