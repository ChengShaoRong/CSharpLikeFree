//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using System;
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
        /// for foreach continue break if-else return while do-while switch-case-default.
        /// All can be mixed together.
        /// </summary>
        void TestLoop()
        {
            Debug.LogError("Test Loop:Not supported custom 'switch-case-default' in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // switch-case-default: direct use "if-else" (Look ugly and a little lower performance if too much branches).

            int x = 1;
            int y = 3;
            int z = 100;

            //test if-else
            if (x < y)
                Debug.Log("test 'if-else': enter 'if'");
            else if (x < z)
                Debug.Log("test 'if-else': enter 'else if'");
            else
                Debug.Log("test 'if-else': enter 'else'");

            List<Vector3> lists = new List<Vector3>();// this generic type should be AOT
            lists.Add(Vector3.zero);
            lists.Add(Vector3.one);
            //test for
            for (int i = 0, j = 2; i < lists.Count; i++, j += i)
            {
                if (i == 2)
                {
                    Debug.Log("test 'continue':");
                    continue;
                }
                Debug.Log("test 'for': lists[" + i + "] = " + lists[i] + ",j = " + j);
            }

            //test foreach
            foreach(var item in lists)
            {
                Debug.Log("test 'foreach': item = " + item);
                if (item.x == 2)
                {
                    Debug.Log("test 'break':");
                    break;
                }
            }

            //test while
            while(x < y)
            {
                x++;
                Debug.Log("test 'while': x = " + x + ", y = " + y);
            }

            //test do-while
            x = 1;
            do
            {
                x++;
                Debug.Log("test 'do-while': x = " + x + ", y = " + y);
            } while (x < y) ;

            //test switch (simulate)
            //in FREE version:
            if (x == 0)
                Debug.Log("test 'switch': enter 0");
            else if (x == 1)
                Debug.Log("test 'switch': enter 1");
            else if (x == 2 || x == 3)
                Debug.Log("test 'switch': enter 2 or 3");
            else
                Debug.Log("test 'switch': enter default");
            ////in full version:
            //switch(x)
            //{
            //    case 0:
            //        Debug.Log("test 'switch': enter 0");
            //        break;
            //    case 1:
            //        Debug.Log("test 'switch': enter 1");
            //        break;
            //    case 2:
            //    case 3:
            //        Debug.Log("test 'switch': enter 2 or 3");
            //        break;
            //    default:
            //        Debug.Log("test 'switch': enter default");
            //        break;
            //}
        }
    }
}