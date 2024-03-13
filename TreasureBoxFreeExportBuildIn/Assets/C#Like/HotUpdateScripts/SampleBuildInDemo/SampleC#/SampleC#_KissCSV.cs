//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike.SubclassEx3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    public partial class SampleCSharp : LikeBehaviour
    {
        public class TestCsv
        {
            public int id;
            public string name;
            public List<int> testInts;
            public List<string> testStrings;
            public List<float> testFloats;
            public Dictionary<string, int> testStringIntDicts;
            public Dictionary<int, bool> testIntBooleanDicts;
        }
        void TestKissCSV()
        {
            string strPrint;
            //We recommend read data from CSV file with class or struct.
            //First step: define a class or struct
            // e.g. we define TestCsv class
            //Second step: Load it into memory
            KissCSV.Load(typeof(TestCsv), "TestCsv", "id", ResourceManager.LoadAsset<TextAsset>("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/CSV/TestCsv.csv").text);
            //final step: Get the data in class or struct by unique key
            TestCsv csv = KissCSV.Get("TestCsv", "1") as TestCsv;
            if (csv != null)//If not exist "1" in columnName "id" will return null.
            {
                Debug.Log("id=" + csv.id);//output id=1
                Debug.Log("name=" + csv.name);//output name=test name
                strPrint = "testInts=";
                foreach(var one in csv.testInts)
                    strPrint += one + ",";
                Debug.Log(strPrint);//output testInts=1,2,
                strPrint = "testStringIntDicts=";
                foreach (var one in csv.testStringIntDicts)
                    strPrint += "{" + one.Key + "," + one.Value + "},";
                Debug.Log(strPrint);//output testStringIntDicts={ab,2},{cd,4},
            }

            Debug.LogError("Test TestKissCSV:");
            //First step: Initialize all the CSV files after your app initialize done.
            //Just call it ONE time before get values from it. 
            //You can call it again to reload the CSV file if you need to.
            SimpleKissCSV.LoadWithFileContent("Item.csv", "id", ResourceManager.LoadAsset<TextAsset>("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/CSV/Item.csv").text);

            //Final step; get kinds of values from the CSV file.
            strPrint = "keys=";
            List<int> keys = SimpleKissCSV.GetIntListKeys("Item.csv");
            foreach (var one in keys)
                strPrint += one + ",";
            Debug.Log(strPrint);//output keys=100,101,
            Debug.Log("name="+ SimpleKissCSV.GetString("Item.csv", "100", "name"));//output keys=output name=test" \nname
            Debug.Log("maxStack=" + SimpleKissCSV.GetInt("Item.csv", "100", "maxStack"));//output maxStack=8888
            Debug.Log("testSingle=" + SimpleKissCSV.GetSingle("Item.csv", "100", "testSingle"));//output testSingle=0.5
            Debug.Log("testDouble=" + SimpleKissCSV.GetDouble("Item.csv", "100", "testDouble"));//output testDouble=888888888888
            Debug.Log("testBoolean=" + SimpleKissCSV.GetBoolean("Item.csv", "100", "testBoolean"));//output testBoolean=True
            List<string> testStringList = SimpleKissCSV.GetStringList("Item.csv", "100", "testStringList");
            strPrint = "testStringList=";
            foreach (var one in testStringList)
                strPrint += one + ",";
            Debug.Log(strPrint);//output testStringList=Hello world,Hi,RongRong
            List<int> testIntList = SimpleKissCSV.GetIntList("Item.csv", "100", "testIntList");
            strPrint = "testIntList=";
            foreach (var one in testIntList)
                strPrint += one + ",";
            Debug.Log(strPrint);//output testIntList=2,3
            Dictionary<string, string> testStringStringDictionary = SimpleKissCSV.GetStringStringDictionary("Item.csv", "100", "testStringStringDictionary");
            strPrint = "testIntList=";
            foreach (var one in testStringStringDictionary)
                strPrint += "{" + one.Key + "," + one.Value + "},";
            Debug.Log(strPrint);//output testStringStringDictionary={aa,b},{cc,d}
            Dictionary<int, int> testIntIntDictionary = SimpleKissCSV.GetIntIntDictionary("Item.csv", "100", "testIntIntDictionary");
            strPrint = "testIntList=";
            foreach (var one in testIntIntDictionary)
                strPrint += "{" + one.Key + "," + one.Value + "},";
            Debug.Log(strPrint);//output testIntIntDictionary={1,2},{3,4},{5,6}
        }
    }
}