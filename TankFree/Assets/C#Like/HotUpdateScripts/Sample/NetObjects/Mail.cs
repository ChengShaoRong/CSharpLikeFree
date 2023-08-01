//It's a single cs file due to C#Like FREE version not support inherit class. 
//You have to merge this code to your project very carefully if you had modified this file.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    public class Mail
    {
        public int uid;
        public int acctId;
        public int senderId;
        public string senderName;
        public string title;
        public string content;
        public string appendix;
        public DateTime createTime;
        public byte wasRead;
        public byte received;
        public static Mail ToMail(JSONData jsonData)
        {
            return (Mail)KissJson.ToObject(typeof(Mail), jsonData);
        }
        public static List<Mail> ToMails(JSONData jsonData)
        {
            List<object> objs = KissJson.ToObjects(typeof(Mail), jsonData);
            List<Mail> mails = new List<Mail>();
            foreach (object obj in objs)
                mails.Add((Mail)obj);
            return mails;
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
    }
}
