//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        /// <summary>
        /// test function overloading and function param default value
        /// </summary>
        void TestOverloadingAndDefaultValue()
        {
            Debug.LogError("Test OverloadingAndDefaultValue: Not supported function overloading and function param default value in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // function overloading: use different function name.
            // param default value: remove that default value.
        }
    }
}