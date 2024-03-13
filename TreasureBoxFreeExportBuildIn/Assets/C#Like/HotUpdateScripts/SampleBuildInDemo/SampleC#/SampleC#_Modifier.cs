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
        /// test modifier for params of function,
        /// include 'ref' 'out' 'in' 'params' keyword.
        /// </summary>
        void TestModifier()
        {
            Debug.LogError("Test Modifier:Not supported params modifier ('ref'/'out'/'in'/'params') in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // In FREE version, you can't use 'ref' 'out' 'in' 'params' keyword in hot update,
            // but you still can call the function in not hot update script with those keywords.
            // We provide the following solutions if you not update to full version:

            // test ref
            Vector3 current = Vector3.zero;
            Vector3 target = Vector3.one;
            Vector3 currentVelocity = Vector3.zero;
            Debug.Log("TestModifier ref:before:current=" + current + ",target=" + target + ",currentVelocity=" + currentVelocity);

            //in FREE version:
            SampleHowToUseModifier.currentVelocity = currentVelocity;
            current = SampleHowToUseModifier.SmoothDamp(current, target, 0.5f);
            currentVelocity = SampleHowToUseModifier.currentVelocity;
            ////in full version:
            //current = Vector3.SmoothDamp(current, target, ref currentVelocity, 0.5f);

            Debug.Log("Test ref:after:current=" + current + ",target=" + target + ",currentVelocity=" + currentVelocity);

            // test out
            Dictionary<string, int> dicts = new Dictionary<string, int>();
            dicts.Add("test", 1);
            int iValue;
            //in FREE version:
            if (SampleHowToUseModifier.TryGetValue(dicts, "test"))
            {
                iValue = SampleHowToUseModifier.value;
                Debug.Log("TestModifier out:iValue=" + iValue);
            }
            ////in full version:
            //if (dicts.TryGetValue("test", out iValue))
            //    Debug.Log("TestModifier out:iValue=" + iValue);

            // test in
            iValue = 100;
            //in FREE version:
            SampleHowToUseModifier.TestModifierIn("test", iValue);//You can ignore 'in' keyword 'SampleHowToUseModifier.ModifierIn("test", iValue);'
            ////in full version:
            //SampleHowToUseModifier.ModifierIn("test", in iValue);//You can ignore 'in' keyword 'SampleHowToUseModifier.ModifierIn("test", iValue);'

            // test params
            //in FREE version:
            SampleHowToUseModifier.TestModifierParams("free version");
            SampleHowToUseModifier.TestModifierParams("free version", "test");
            SampleHowToUseModifier.TestModifierParams("free version", "test", 1);
            SampleHowToUseModifier.TestModifierParams("free version", "test", 1, 0.5f);
            ////in full version:
            //SampleHowToUseModifier.ModifierParams("free version");
            //SampleHowToUseModifier.ModifierParams("free version", "test");
            //SampleHowToUseModifier.ModifierParams("free version", "test", 1);
            //SampleHowToUseModifier.ModifierParams("free version", "test", 1, 0.5f);
        }
    }
}