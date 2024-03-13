//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        public Text strTestMessage;
        void Start()
        {
            strTestMessage.text = "This's content have too much message. Check the log in Console panel please.";
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
            TestOther();
        }
        void OnClickBack()
        {
            HotUpdateManager.Show("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleCSharpLikeHotUpdate.prefab");//back to SampleCSharpLikeHotUpdate
            HotUpdateManager.Hide("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleC#.prefab", true);//delete self
        }
    }
}