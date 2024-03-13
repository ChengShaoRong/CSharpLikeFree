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
        void TestKissJson()
        {
            Debug.LogError("Test TestKissJson:You can't use @ keyword to define JSON string and using Nullable type and using bit operation with JSONData in hot update script in FREE version. (Supported in full version). Strongly recommended update to full version C#Like: https://assetstore.unity.com/packages/slug/222256");
            // We provide the following solutions if you not update to full version:
            // @ keyword: direct use "" instead of @"".
            // bit operation: use function instead of bit operation.
            // Nullable: don't use Nullable type.

            //Build-In-Type:
            //byte/sbyte/char/bool/short/ushort/int/uint/long/ulong/double/float/string,
            //and byte?/sbyte?/char?/bool?/short?/ushort?/int?/uint?/long?/ulong?/double?/float?,
            //and List<Build-In-Type>,
            //and Dictionary<string, Build-In-Type>

            //Direct assign value 'both' between Build-In-Type and JSONData.
            //You can direct assign Build-In-Type value to JSONData, Or direct assign JSONData value to Build-In-Type.

            //test Build-In-Type => JSONData
            JSONData iData = 2;
            Debug.Log("JSONData iData = 2;  test iData = " + iData);//output 2
            JSONData fData = 88.8f;
            Debug.Log("JSONData fData = 88.8f;  test fData = " + fData);//output 88.8
            List<string> listValue = new List<string>();
            listValue.Add("test list str1");
            listValue.Add("test list str2");
            JSONData listData = listValue;
            Debug.Log("JSONData listData = listValue;  test listData = " + listData);//output ["test list str1","test list str2"]
            Dictionary<string, int> dictValue = new Dictionary<string, int>();
            dictValue.Add("key1", 11);
            dictValue.Add("key2", 22);
            JSONData dictData = dictValue;
            Debug.Log("JSONData dictData = dictValue;  test dictData = " + dictData);//output {"key1":11,"key2":22}

            //test JSONData =>  Build-In-Type
            int iValue = iData;
            Debug.Log("int iValue = iData;  test iValue = " + iValue);//output 2
            float fValue = fData;
            Debug.Log("float fValue = fData;  test fValue = " + fValue);//output 88.8
            List<string> listValue2 = listData;
            string strTemp = "";
            foreach (var str in listValue2)
                strTemp += str + ",";
            Debug.Log("List<string> listValue2 = listData;  test listValue2 = " + strTemp);//output test list str1,test list str2,
            Dictionary<string, int> dictValue2 = dictData;
            strTemp = "";
            foreach (var item in dictValue2)
                strTemp += "(" + item.Key + "=" + item.Value + ")";
            Debug.Log("Dictionary<string, int> dictValue2 = dictData;  test dictValue2 = " + strTemp);//output (key1=11)(key2=22)

            //test JSON string => JSONData
            string strJson = "{\"str\":\"{test str\",\"i\":11,\"j\":2.3,\"k\":[3,null,{\"m\":true}],\"l\":{\"x\":1,\"y\":\"abc\"}}";
            JSONData data = KissJson.ToJSONData(strJson);
            //accept JSONData by ["key"] and [index]
            Debug.Log("JSON string => JSONData; test data[\"str\"] = " + data["str"]);//output {test str
            Debug.Log("JSON string => JSONData; test data[\"i\"] = " + data["i"]);//output 11
            Debug.Log("JSON string => JSONData; test data[\"j\"] = " + data["j"]);//output 2.3
            Debug.Log("JSON string => JSONData; test data[\"k\"] = " + data["k"]);//output [3,null,{"m":true}]
            Debug.Log("JSON string => JSONData; test data[\"l\"] = " + data["l"]);//output {"x":1,"y":"abc"}
            Debug.Log("JSON string => JSONData; test data[\"k\"][0] = " + data["k"][0]);//output 3
            Debug.Log("JSON string => JSONData; test data[\"l\"][\"y\"] = " + data["l"]["y"]);//output abc
            Debug.Log("JSON string => JSONData; test data[\"k\"][2][\"m\"] = " + data["k"][2]["m"]);//output true

            //test Math Expression between JSONData with Build-In-Type
            JSONData exprData1 = 2;
            JSONData exprData2 = 3;
            JSONData exprData3 = exprData1 * exprData2;
            Debug.Log("test Math Expression;  exprData3 = exprData1 * exprData2; exprData3 = " + exprData3);//output 6
            exprData3 = exprData1 - exprData2;
            Debug.Log("test Math Expression;  exprData3 = exprData1 - exprData2; exprData3 = " + exprData3);//output -1
            exprData3 *= exprData2;//exprData3=-1;exprData2=3
            Debug.Log("test Math Expression;  exprData3 *= exprData2; exprData3 = " + exprData3);//output -3

            iData = 2;
            if (iData > 1)
                Debug.Log("test Math Expression;  iData = 2, enter if (iData > 1)");//output
            else
                Debug.Log("test Math Expression;  iData = 2, not enter if (iData > 1)");

            //test JSONData => JSON string
            JSONData listData3 = JSONData.NewDictionary();
            listData3.Add("key1", 10);         //add data like Dictionary use function 'Add(key,value)'
            listData3["key2"] = "test string"; //add data like Dictionary use index set 'this[]'
            listData3["key3"] = JSONData.NewList(); //we add a list
            if (listData3.ContainsKey("key3"))  //make sure the key 'key3' exist if you don't know whether exist
                listData3["key3"].Add(1);       //add some data to the list
            listData3["key3"].Insert(0,"string2"); //insert some data to the list,we don't check the key 'key3' exist because we just know it exist!
            listData3["key4"] = JSONData.NewDictionary(); //we add a Dictionary
            listData3["key4"]["x"] = 1;
            listData3["key4"]["y"] = 2;
            listData3["key4"]["z"] = 3;
            Debug.Log("test JSONData => JSON string; strJson = " + listData3.ToJson());//output {"key1":10,"key2":"test string","key3":["string2",1],"key4":{"x":1,"y":2,"z":3}}

            //test looping through the List or Dictionary in JSONData
            foreach (var item in listData3["key3"].Value as List<JSONData>)
            {
                Debug.Log("test looping through the List in JSONData using foreach; listData3[\"key3\"].Value = " + item);//output string2/output 1
            }
            List<JSONData> tempList = listData3["key3"].Value as List<JSONData>;
            for (int i=0; i< tempList.Count; i++)
                Debug.Log("test looping through the List in JSONData using for; listData3[\"key3\"].Value = " + tempList[i]);//output string2/output 1
            foreach (var item in listData3["key4"].Value as Dictionary<string, JSONData>)
            {
                Debug.Log("test looping through the Dictionary in JSONData using foreach; listData3[\"key4\"], Key=" + item.Key + ",Value=" + item.Value);//output Key=x,Value=1/output Key=y,Value=2/output Key=z,Value=3
            }
            
            //test JSON string => class/struct
            strJson = "{\"str\":\"{test str\",\"i\":11,\"j\":1,\"z\":2,\"k\":[3,1,7],\"datas\":{\"aa\":{\"id\":1,\"name\":\"aaa\",\"v2\":{\"x\":1,\"y\":2},\"info\":[\"a\",\"xd\",\"dt\"],\"maps\":{\"x\":1,\"y\":2}},\"bb\":{\"id\":2,\"name\":\"bbb\",\"v2\":{\"x\":3,\"y\":4},\"info\":[\"x\",\"x3d\",\"ddt\"],\"maps\":{\"x\":2,\"y\":3}}},\"data\":{\"id\":3,\"name\":\"ccc\",\"v2\":{\"x\":3,\"y\":1},\"info\":[\"ya\",\"xyd\",\"drt\"],\"maps\":{\"x\":3,\"y\":4}}}";
            TestJsonData testJsonData = (TestJsonData)KissJson.ToObject(typeof(TestJsonData), strJson);//JSON string => class
            Debug.Log(testJsonData.str);//output Null
            Debug.Log(testJsonData.i);//output 11
            //"j":"Monday" or "j":"1" or "j":1 are both identified as 'DayOfWeek.Monday'
            //recommend use "j":1 because ToJson output as number
            Debug.Log(testJsonData.j);//output Monday
            Debug.Log((int)testJsonData.j);//output 1
            foreach (var item in testJsonData.k)
                Debug.Log(item);//output 3/output 1/output 7
            foreach(var datas in testJsonData.datas)
            {
                Debug.Log(datas.Key);//output aa/output bb
                Debug.Log(datas.Value.v2);//output (1.0, 2.0)/output (3.0, 4.0)
            }
            
            //test class => JSON string
            strTemp = KissJson.ToJson(testJsonData);//class => JSON string
            Debug.Log(strTemp);//output {"i":11,"j":1,"z":2,"k":[3,1,7],"datas":{"aa":{"id":1,"name":"aaa","v2":{"x":1,"y":2},"info":["a","xd","dt"],"maps":{"x":1,"y":2}},"bb":{"id":2,"name":"bbb","v2":{"x":3,"y":4},"info":["x","x3d","ddt"],"maps":{"x":2,"y":3}}},"data":{"id":3,"name":"ccc","v2":{"x":3,"y":1},"info":["ya","xyd","drt"],"maps":{"x":3,"y":4}}}
        }
    }
    namespace SubclassEx3
    {
        /// <summary>
        /// test class <=> JSON
        /// </summary>
        public class TestJsonDataSub
        {
            public string name;
            public Vector2 v2;//you can add other class/struct type,such as Color/Rect/Vector3/...
            public List<string> info;
            public Dictionary<string, int> maps;
        }
        //mark the class as KissJsonDontSerialize,will ignore while serialize JSON
        [KissJsonDontSerialize]
        public class TestJsonDataSub2
        {
            public int id;
        }
        /// <summary>
        /// test class <=> JSON
        /// </summary>
        public class TestJsonData
        {
            [KissJsonDontSerialize]
            public string str;//will ignore while serialize JSON because be mark as KissJsonDontSerialize
            public int i;
            public DayOfWeek j;//test enum of not hot update
            public List<int> k;
            public Dictionary<string, TestJsonDataSub> datas;//test Dictionary for class/struct
            public TestJsonDataSub data;//test single class/struct
            public TestJsonDataSub2 data2;//will ignore while serialize JSON because the class 'TestJsonDataSub2' mark as KissJsonDontSerialize
        }
    }
}