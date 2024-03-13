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
        /// <summary>
        /// test math expression:
        /// + - * / % += -= *= /= %= > >= < <= != == && || ! ++ -- is as ?:
        /// </summary>
        void TestMathExpression()
        {
            Debug.LogError("Test MathExpression:Not supported 'bit operation' and Nullable in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // bit operation: use function instead of bit operation.
            // Nullable: don't use Nullable type.

            int i = 1;
            int j = 2;
            int k = 100;
            //test: + - * / %
            Debug.Log("i = 1, j = 2, test i + j = " + (i + j));//output 3
            Debug.Log("i = 1, j = 2, test i - j = " + (i - j));//output -1
            Debug.Log("i = 1, j = 2, test i * j = " + (i * j));//output 2
            Debug.Log("i = 1, j = 2, test i / j = " + (i / j));//output 0
            Debug.Log("i = 1, j = 2, test i % j = " + (i % j));//output 1

            //test: += -= *= /= %=
            k += j;
            Debug.Log("j = 2, k = 100, test k += j then k = " + k);//output 102
            k = 100;
            k -= j;
            Debug.Log("j = 2, k = 100, test k -= j then k = " + k);//output 98
            k = 100;
            k *= j;
            Debug.Log("j = 2, k = 100, test k *= j then k = " + k);//output 200
            k = 100;
            k /= j;
            Debug.Log("j = 2, k = 100, test k /= j then k = " + k);//output 50
            k = 100;
            k %= j;
            Debug.Log("j = 2, k = 100, k %= j then k = " + k);//output 0
            
            //test: > >= < <= != ==
            Debug.Log("i = 1, j = 2, test (i < j) = " + (i < j));//output True
            Debug.Log("i = 1, j = 2, test (i <= j) = " + (i <= j));//output True
            Debug.Log("i = 1, j = 2, test (i == j) = " + (i == j));//output False
            Debug.Log("i = 1, j = 2, test (i != j) = " + (i != j));//output True
            Debug.Log("i = 1, j = 2, test (i > j) = " + (i > j));//output False
            Debug.Log("i = 1, j = 2, test (i >= j) = " + (i >= j));//output False

            //test: && || !
            bool b1 = true;
            bool b2 = false;
            Debug.Log("b1 = true, b2 = false, test (b1 && b2) = " + (b1 && b2));//output False
            Debug.Log("b1 = true, b2 = false, test (b1 || b2) = " + (b1 || b2));//output True
            Debug.Log("b1 = true, test (!b1) = " + (!b1));//output False

            //test: ++ --(include prefix and suffix)
            k = 100;
            Debug.Log("k = 100, test (k++) = " + (k++));//output 100
            Debug.Log("finally k = " + k);//output 101
            k = 100;
            Debug.Log("k = 100, test (++k) = " + (++k));//output 101
            Debug.Log("finally k = " + k);//output 101
            k = 100;
            Debug.Log("k = 100, test (k--) = " + (k--));//output 100
            Debug.Log("finally k = " + k);//output 99
            k = 100;
            Debug.Log("k = 100, test (--k) = " + (--k));//output 99
            Debug.Log("finally k = " + k);//output 99
            // We are not support '++' '--' with index get/set '[]',
            // such as List or Dictionary or JSONData.
            //// List<int> lists = new List<int>();
            //// lists.Add(1);
            //// lists[0]++;//compiler error,suggest use 'lists[0] += 1;' instead
            //// ++lists[0];//runtime error,suggest use 'lists[0] += 1;' instead


            //test convert: 'is' 'as'
            object o = i;
            Debug.Log("int i = 1,object o = i, test (o is int) = " + (o is string));//output False
            Debug.Log("int i = 1,object o = i, test (o as string) = " + (o as string));//output null
        }

    }
}