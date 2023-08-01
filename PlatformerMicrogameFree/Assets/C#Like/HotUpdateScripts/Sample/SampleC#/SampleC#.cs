//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        void Start()
        {
            GetComponent<Text>("TestMessage").text = "This's content have too much message. Check the log in Console panel please.";
            TestClass();
            TestDelegateAndLambda();
            TestMathExpression();
            TestLoop();
            TestGetSetAccessor();
            TestThread();
            TestUsingAndNamespace();
            TestMacroAndRegion();
            TestEnum();
            TestModifier();
            TestOverloadingAndDefaultValue();
            TestException();
            TestKeyword();
            TestKissJson();
            TestKissCSV();
        }
        void OnClickBack()
        {
            HotUpdateManager.Show("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");//back to SampleCSharpLikeHotUpdate
            HotUpdateManager.Hide("Assets/C#Like/Sample/SampleC#.prefab", true);//delete self
        }
    }
}