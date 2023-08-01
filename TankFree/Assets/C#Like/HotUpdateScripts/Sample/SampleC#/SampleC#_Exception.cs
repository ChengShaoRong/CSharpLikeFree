//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using System;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        void TestException()
        {
            Debug.LogError("Test Exception: Not supported 'try-catch-finally' in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            //throw new Exception("test throw except");//You can use 'throw' keyword
        }
    }
}