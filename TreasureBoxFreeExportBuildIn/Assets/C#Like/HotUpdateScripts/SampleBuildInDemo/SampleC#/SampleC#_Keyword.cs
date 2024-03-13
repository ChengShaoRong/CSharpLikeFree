//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;//test using command
using System;
using Random = UnityEngine.Random;//test using alias
using System.Text;
using System.IO;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        /// <summary>
        /// test not classify keyword 
        /// </summary>
        void TestKeyword()
        {
            Debug.LogError("Test Keyword:Not supported '$ sizeof unsafe #pragma #warning #error' in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // '$""' keyword: using string.Format (Your code look not so tidy and easy in editor only).
            // '@""' keyword: direct use "" instead.
            // sizeof and unsafe: direct use integer number.
            // #pragma #warning #error: don't use it.

            //build-in data type,
            //Int32/UInt32/Int64/UInt64/Int16/UInt16/Char/Single/Double/Decimal/Boolean/SByte/Byte/String.
            //equal
            //int/uint/long/ulong/short/ushort/char/float/double/decimal/bool/sbyte/byte/string.
            Int32 i = 123;//equal to 'int i = 123;'
            Debug.Log("test Int32 i = " + i);//output 123

        }

    }
}