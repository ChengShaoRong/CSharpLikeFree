//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;//test using command
using Random = UnityEngine.Random;//test using alias for not hot update class
using System.IO;
using CSharpLike.Subclass;
using CSharpLike.SubclassEx;
using Mammals = CSharpLike.SubclassEx.Mammals;//test using alias for hot update class
using System;
using static CSharpLike.JSONData;//test using static for call the class/enum/struct inside the not hot update class

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        /// <summary>
        /// 4 types using:
        /// using command,
        /// using alias(same class name with different namespace),
        /// using sentence,
        /// using static(class/enum/struct inside the class)
        /// </summary>
        void TestUsingAndNamespace()
        {
            Debug.LogError("Test Using and Namespace: Not supported 'using sentence' in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // using sentence: call .Dispose() manual (Without try-catch-finally in FREE version, you should know that function may be won't be called).

            //confusion between System.Random and UnityEngine.Random
            Debug.Log("test using alias: random number = " + Random.Range(0, 1000) + " at " + DateTime.Now);
            Debug.LogError("Test Namespace:");
            //confusion between CSharpLike.SampleCSharp.SubclassEx.Mammals and CSharpLike.SampleCSharp.SubclassEx2.Mammals
            Mammals mammals = new Mammals();
            Debug.Log("test Namespace: mammals=" + mammals.TestNameSpace());
            Debug.Log("test Namespace: Toys=" + (new Toys("rabbit")).name);
            //Without using,just direct use full namespace
            Debug.Log("test Namespace: direct use full namespace for hot update script: " + SubclassEx2.TestNamespace.GetTestString());
            Debug.Log("test Namespace: direct use full namespace for not hot update script: " + System.Text.Encoding.UTF8.GetBytes("test string"));

            //test using static for call the class/enum/struct inside the not hot update class
            JSONData jsonData = 1;
            //DataType is enum inside the CSharpLike.JSONData, we must set 'using static CSharpLike.JSONData;' in the head.
            //You can't use the FullName 'CSharpLike.JSONData.DataType.DataTypeList' replace 'DataType.DataTypeList', or else will compile error.
            //Because typeof(CSharpLike.JSONData.DataType) full name is 'CSharpLike.JSONData+DataType', not 'CSharpLike.JSONData.DataType'.
            //But we are support the FullName style if that class/enum/struct inside the hot update class.
            if (jsonData.dataType != DataType.DataTypeList)
                Debug.Log("test jsonData.dataType=" + jsonData.dataType);//output DataTypeInt

        }
    }
}