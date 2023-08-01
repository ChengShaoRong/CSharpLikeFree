//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using UnityEngine;

namespace CSharpLike
{
    /// <summary>
    /// test for class in 'ExampleC#_Class.cs'
    /// </summary>
    namespace Subclass
    {
        public interface IAnimal
        {
            int feet { get; set; }
            bool female { get; set; }
            bool CanUseTool();
        }

        public class Mammals : IAnimal
        {
            public int breasts;
            public string name;

            public int feet { get; set; }
            public bool female { get; set; }

            public Mammals(int breasts, int feet, string name)
            {
                Debug.Log("Mammals(" + breasts + "," + feet + "," + name + ")");
                this.breasts = breasts;
            }

            public string GetInfo()
            {
                return "Mammals:" + name + " with " + feet + " feet and " + breasts + " breasts";
            }
            public bool CanUseTool()
            {
                return false;
            }
        }
    }
    /// <summary>
    /// test for namespace in 'ExampleC#_UsingAndNamespace.cs'
    /// </summary>
    namespace SubclassEx
    {
        /// <summary>
        /// this's test namespace with CSharpLike.Subclass.Mammals
        /// </summary>
        public class Mammals
        {
            public int breasts = 2;
            public int eyes = 2;
            public string TestNameSpace()
            {
                return "Mammals:breasts:" + breasts + ", eyes:" + eyes;
            }
        }
        public class Toys
        {
            public string name;
            public Toys(string name)
            {
                this.name = name;
            }
        }
    }
    /// <summary>
    /// test for namespace in 'ExampleC#_UsingAndNamespace.cs'
    /// </summary>
    namespace SubclassEx2
    {
        public class TestNamespace
        {
            public static string GetTestString()
            {
                return "Test string for namespace";
            }
        }
    }
}