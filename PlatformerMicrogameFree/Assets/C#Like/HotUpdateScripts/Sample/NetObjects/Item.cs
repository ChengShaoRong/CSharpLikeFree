//It's a single cs file due to C#Like FREE version not support inherit class. 
//You have to merge this code to your project very carefully if you had modified this file.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    public class Item
    {
        public int uid;
        public int itemId;
        public int acctId;
        public int count;
        public static Item ToItem(JSONData jsonData)
        {
            return (Item)KissJson.ToObject(typeof(Item), jsonData);
        }
        public static List<Item> ToItems(JSONData jsonData)
        {
            List<object> objs = KissJson.ToObjects(typeof(Item), jsonData);
            List<Item> items = new List<Item>();
            foreach (object obj in objs)
                items.Add((Item)obj);
            return items;
        }

        public override string ToString()
        {
            return KissJson.ToJSONData(this).ToJson(true);
        }
        public void Clear()
        {
            KissJson.ClearCache(_uid_);
        }
        string _uid_ = "";
        ulong _sendMask_ = 0;
        public void OnChanged()
        {
            //Add your code here.
        }
        public void OnUidChanged()
        {
            //Add your code here.
        }
        public void OnCountChanged()
        {
            //Add your code here.
        }
        public void OnDeleted()
        {
            //Add your code here.
        }

        public void NotifyValuesChanged()
        {
            if (CSL_Utils.CheckSendMask(_sendMask_, 1)) OnUidChanged();
            if (CSL_Utils.CheckSendMask(_sendMask_, 2)) OnCountChanged();
            if (_sendMask_ > 0) OnChanged();
        }
    }
}
