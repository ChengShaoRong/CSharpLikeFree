//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        void TestGetSetAccessor()
        {
            Debug.LogError("Test TestGetSetAccessor: Not supported 'custom implement get/set accessor' in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // custom implement get/set accessor: direct use function instead.

            //test auto implement get set accessor
            Debug.Log("before set value testGetSetAutoImp = " + testGetSetAutoImp);//output False
            testGetSetAutoImp = true;
            Debug.Log("after set value: testGetSetAutoImp = " + testGetSetAutoImp);//output True
        }

        public bool testGetSetAutoImp { get;set; }
    }
}