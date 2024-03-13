//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        /// <summary>
        /// Test other.
        /// </summary>
        void TestOther()
        {
            Debug.LogError("Test Exception: Not supported 'use object initializers'/'inline variable declaration'"
            + "/'expression body for constructors and methods and properties and accessors'"
            + "/'switch expression and discard _'/'collection initializers'"
            + " in FREE version. (Supported in full version)."
            + "Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
        }
    }
}