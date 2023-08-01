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
        /// Support unity build-in define symbols and user custom define symbols
        /// ("Edit"->"Project Settings"->"Player"->"Scripting Define Symbols").
        /// just same with unity,except 'UNITY_EDITOR/UNITY_EDITOR_WIN/UNITY_EDITOR_OSX/UNITY_EDITOR_LINUX'
        /// only work both in 'debug mode' and in editor.
        /// </summary>
        void TestMacroAndRegion()
        {
            Debug.LogError("Test TestMacroAndRegion: Not supported 'macro' and 'region' in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // macro: define a variable in hot update script.
            // region: don't use region (Your code look not so tidy in editor only).

            // test macro (simulate)
            // in FREE version
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                Debug.Log("Test macro (simulate): is UNITY_WEBGL");
            else
                Debug.Log("Test macro (simulate): is not UNITY_WEBGL");
            //// in full version
//#if UNITY_WEBGL
//            Debug.Log("Test macro: is UNITY_WEBGL");
//#else
//            Debug.Log("Test macro: is not UNITY_WEBGL");
//#endif
        }
    }
}