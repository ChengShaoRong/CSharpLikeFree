//It's a single cs file due to C#Like FREE version not support inherit class. 
//You have to merge this code to your project very carefully if you had modified this file.
using System;
using System.Collections.Generic;
using TreasureBox;
using UnityEngine;

namespace CSharpLike
{
	public class SignIn
	{
		[KissJsonDontSerialize]
		List<int> mSignInDays = null;
		[KissJsonDontSerialize]
		public List<int> SignInDays()
		{
			//get
			//{
				if (mSignInDays == null)
					mSignInDays = Global.StringToList(signInList);
				return mSignInDays;
			//}
		}
		[KissJsonDontSerialize]
		public bool ShowSpot()
        {
			return !SignInDays().Contains(DateTime.Now.Day);

		}
		//public bool ShowSpot => (!SignInDays.Contains(DateTime.Now.Day));

		public int acctId;
		public int month;
		public string signInList;
		public static SignIn ToSignIn(JSONData jsonData)
		{
			return (SignIn)KissJson.ToObject(typeof(SignIn), jsonData);
		}
		public static List<SignIn> ToSignIns(JSONData jsonData)
		{
			List<object> objs = KissJson.ToObjects(typeof(SignIn), jsonData);
			List<SignIn> signIns = new List<SignIn>();
			foreach (object obj in objs)
				signIns.Add((SignIn)obj);
			return signIns;
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
			mSignInDays = null;
		}
		public void OnAcctIdChanged()
		{
			//Add your code here.
		}
		public void OnMonthChanged()
		{
			//Add your code here.
		}
		public void OnSignInListChanged()
		{
			//Add your code here.
		}
		public void OnDeleted()
		{
			//Add your code here.
		}

		public void NotifyValuesChanged()
		{
			if (CSL_Utils.CheckSendMask(_sendMask_, 1UL)) OnAcctIdChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 2UL)) OnMonthChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 4UL)) OnSignInListChanged();
			if (_sendMask_ > 0) OnChanged();
		}
	}
}
