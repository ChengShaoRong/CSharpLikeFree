//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike.Subclass;

namespace CSharpLike
{
    /// <summary>
    /// Example test C# function in hot update script.
    /// </summary>
    public partial class SampleCSharp : LikeBehaviour
    {
        void TestClass()
        {
            Debug.LogError("Test class: Not supported 'constructor(this/base)/destructor/class inherit/virtual function' in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // constructor(this/base)/destructor: use normal function and call it by manual.
            // class inherit/virtual function: don't use it, C language can do everything even it have no class, you can do it too.
            // That cause your code not so object-oriented only, you still can have a class and can inherit some interfaces.

            //test class
            Mammals cow = new Mammals(4, 4, "cow");
            Debug.Log("cow:" + cow.GetInfo());
            //test interface
            IAnimal bull = new Mammals(0, 4, "bull");
            bull.female = false;
            Debug.Log("bull:female=" + bull.female + ", CanUseTool=" + bull.CanUseTool());
        }
    }
}