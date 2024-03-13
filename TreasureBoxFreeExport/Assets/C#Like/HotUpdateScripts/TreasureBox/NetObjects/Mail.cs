//It's a single cs file due to C#Like FREE version not support inherit class. 
//You have to merge this code to your project very carefully if you had modified this file.
using System;
using System.Collections.Generic;
using TreasureBox;
using UnityEngine;

namespace CSharpLike
{
	public class Mail
	{
		[KissJsonDontSerialize]
		public bool ShowSpot()
		{
			return (wasRead == 0 || (received == 0 && !string.IsNullOrEmpty(appendix)));
		}
		//Free version not support customize 'get/set'
		//public bool ShowSpot => (wasRead == 0 || (received == 0 && !string.IsNullOrEmpty(appendix)));
		[KissJsonDontSerialize]
		public string SortKey()
		{
			return string.Format("{0}{1}", (ShowSpot() ? 1 : 0), createTime.ToString("yyyy-dd-dd HH:mm:ss"));
		}
		//Free version not support customize 'get/set'
		//public string SortKey => $"{(ShowSpot ? 1 : 0)}{createTime.ToString("yyyy-dd-dd HH:mm:ss")}";
		[KissJsonDontSerialize]
		Dictionary<int, int> mAppendix = null;
		[KissJsonDontSerialize]
		public Dictionary<int, int> Appendix()
		{
			//get
			//{
				if (mAppendix == null)
					mAppendix = Global.StringToDictionary(appendix);
				return mAppendix;
			//}
		}

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
