//It's a single cs file due to C#Like FREE version not support inherit class. 
//You have to merge this code to your project very carefully if you had modified this file.
using System;
using System.Collections.Generic;
using TreasureBox;
using UnityEngine;

namespace CSharpLike
{
	public class Account
	{

		/// <summary>
		/// Vip level from vipExp
		/// </summary>
		public int Vip()
		{
			return VipCsv.GetVip(vipExp);
		}
		//Free version not support customize 'get/set'
		//public int Vip => VipCsv.GetVip(vipExp);
		public List<Mail> SortedMails()
		{
			List<Mail> mails = new List<Mail>();
			SortedDictionary<string, Mail> sortedDic = new SortedDictionary<string, Mail>();
			foreach (Mail mail in MySocket.account.mails.Values)
				sortedDic.Add(mail.SortKey() + UnityEngine.Random.Range(0f, 1f).ToString("F10"), mail);
			foreach (Mail mail in sortedDic.Values)
				mails.Add(mail);
			mails.Reverse();
			return mails;
		}
		//Free version not support customize 'get/set'
		//public List<Mail> SortedMails
		//{
		//	get
		//	{
		//		List<Mail> mails = new List<Mail>();
		//		SortedDictionary<string, Mail> sortedDic = new SortedDictionary<string, Mail>();
		//		foreach (Mail mail in MySocket.account.mails.Values)
		//			sortedDic.Add(mail.SortKey() + UnityEngine.Random.Range(0f, 1f).ToString("F10"), mail);
		//		foreach (Mail mail in sortedDic.Values)
		//			mails.Add(mail);
		//		mails.Reverse();
		//		return mails;
		//	}
		//}

		public int uid;
		public int acctType;
		public string name;
		public DateTime createTime;
		public string nickname;
		public int icon;
		public int money;
		public int diamond;
		public DateTime lastLoginTime;
		public int exp;
		public int lv;
		public int vp;
		public DateTime vpTime;
		public int vipExp;
		public static Account ToAccount(JSONData jsonData)
		{
			return (Account)KissJson.ToObject(typeof(Account), jsonData);
		}
		public static List<Account> ToAccounts(JSONData jsonData)
		{
			List<object> objs = KissJson.ToObjects(typeof(Account), jsonData);
			List<Account> accounts = new List<Account>();
			foreach (object obj in objs)
				accounts.Add((Account)obj);
			return accounts;
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
		public void OnCallbackObjectMails(List<Mail> data)
		{
			//Refresh mail spot in main scene panel
			MainScenePanel mainScenePanel = GetMainScenePanel();
			if (mainScenePanel != null)
				mainScenePanel.RefreshMailSpot();
			//Full version
			//GetMainScenePanel()?.RefreshMailSpot();

			//Refresh mail panel
			MailPanel mailPanel = Global.GetPanel("MailPanel") as MailPanel;
			if (mailPanel != null)
				mailPanel.OnUpdateOrNewMails(data);
			//Full version
			//(Global.GetPanel("MailPanel") as MailPanel)?.OnUpdateOrNewMails(data);
		}
		public void OnCallbackDeleteMails(List<Mail> data)
		{
			MailPanel mailPanel = Global.GetPanel("MailPanel") as MailPanel;
			if (mailPanel != null)
				mailPanel.OnDeleteMails(data);
			//Full version
			//(Global.GetPanel("MailPanel") as MailPanel)?.OnDeleteMails(data);
		}
		public void OnCallbackObjectItems(List<Item> data)
		{
			Debug.Log("OnCallbackObjectItems:now all item count = " + items.Count);
		}
		public void OnCallbackDeleteItems(List<Item> data)
		{
			Debug.Log("OnCallbackDeleteItems:now all item count = " + items.Count);
		}
		public void OnCallbackObjectSignIn()
		{
			//Refresh mail spot in main scene panel
			MainScenePanel mainScenePanel = GetMainScenePanel();
			if (mainScenePanel != null)
				mainScenePanel.RefreshSignInSpot();
			//Full version
			//GetMainScenePanel()?.RefreshSignInSpot();

			//Refresh sign in panel
			SignInPanel signInPanel = Global.GetPanel("SignInPanel") as SignInPanel;
			if (signInPanel != null)
				signInPanel.RefreshUI();
			//Full version
			//(Global.GetPanel("SignInPanel") as SignInPanel)?.RefreshUI();
		}
		public void OnCallbackDeleteSignIn()
		{
			Debug.Log("OnCallbackDeleteSignIn:" + signIn.ToString());
		}
		public void OnCB_Object(JSONData jsonData)
		{
			string name = jsonData["name"];
			if (name == "account")
			{
				ToAccount(jsonData["obj"]);
				NotifyValuesChanged();
			}
			else if (name == "mails")
			{
				List<Mail> _mails_ = Mail.ToMails(jsonData["obj"]);
				foreach(Mail _one_ in _mails_)
				{
					mails[_one_.uid] = _one_;
				}
				OnCallbackObjectMails(_mails_);
			}
			else if (name == "items")
			{
				List<Item> _items_ = Item.ToItems(jsonData["obj"]);
				foreach(Item _one_ in _items_)
				{
					items[_one_.itemId] = _one_;
					_one_.NotifyValuesChanged();
				}
				OnCallbackObjectItems(_items_);
			}
			else if (name == "signIn")
			{
				signIn = SignIn.ToSignIn(jsonData["obj"]);
				signIn.NotifyValuesChanged();
				OnCallbackObjectSignIn();
			}
			else
				Debug.LogError("CB_Object unsupported name " + name);
		}
		public void OnCB_Delete(JSONData jsonData)
		{
			string name = jsonData["name"];
			List<int> ids = jsonData["ids"];
			if (name == "account")
			{
				OnDeleted();
			}
			else if (name == "mails")
			{
				List<Mail> _deletes_ = new List<Mail>();
				foreach(int _uid_ in ids)
				{
					Mail _one_ = GetMail(_uid_);
					if (_one_ != null)
					{
						_deletes_.Add(_one_);
						_one_.Clear();
						mails.Remove(_uid_);
					}
				}
				OnCallbackDeleteMails(_deletes_);
			}
			else if (name == "items")
			{
				List<Item> _deletes_ = new List<Item>();
				foreach(int _itemId_ in ids)
				{
					Item _one_ = GetItem(_itemId_);
					if (_one_ != null)
					{
						_deletes_.Add(_one_);
						_one_.Clear();
						items.Remove(_itemId_);
						_one_.OnDeleted();
					}
				}
				OnCallbackDeleteItems(_deletes_);
			}
			else if (name == "signIn")
			{
				signIn.Clear();
				signIn.OnDeleted();
				OnCallbackDeleteSignIn();
				signIn = null;
			}
			else
				Debug.LogError("CB_Object unsupported name " + name);
		}
		[KissJsonDontSerialize]
		public Dictionary<int, Mail> mails = new Dictionary<int, Mail>();
		public void SetMails(List<Mail> mails)
		{
			foreach(Mail one in mails)
				this.mails[one.uid] = one;
		}
		public Mail GetMail(int uid)
		{
			if (mails.ContainsKey(uid))
				return mails[uid];
			return null;
		}
		public bool RemoveMail(int uid)
		{
			return mails.Remove(uid);
		}
		[KissJsonDontSerialize]
		public Dictionary<int, Item> items = new Dictionary<int, Item>();
		public void SetItems(List<Item> items)
		{
			foreach(Item one in items)
				this.items[one.itemId] = one;
		}
		public Item GetItem(int itemId)
		{
			if (items.ContainsKey(itemId))
				return items[itemId];
			return null;
		}
		public bool RemoveItem(int itemId)
		{
			return items.Remove(itemId);
		}
		[KissJsonDontSerialize]
		public SignIn signIn = null;
		/// <summary>
		/// Get the main scene panel instance. That make the code shorter and tidy.
		/// </summary>
		MainScenePanel GetMainScenePanel()
		{
			return Global.GetPanel("MainScenePanel") as MainScenePanel;

		}
		public void OnChanged()
		{
			//Add your code here.
		}
		public void OnUidChanged()
		{
			//Add your code here.
		}
		public void OnNicknameChanged()
		{
			MainScenePanel mainScenePanel = GetMainScenePanel();
			if (mainScenePanel != null)
				mainScenePanel.RefreshNickname();
			//Full version
			//GetMainScenePanel()?.RefreshNickname();
		}
		public void OnIconChanged()
		{
			MainScenePanel mainScenePanel = GetMainScenePanel();
			if (mainScenePanel != null)
				mainScenePanel.RefreshIcon();
			//Full version
			//GetMainScenePanel()?.RefreshIcon();
		}
		public void OnMoneyChanged()
		{
			MainScenePanel mainScenePanel = GetMainScenePanel();
			if (mainScenePanel != null)
				mainScenePanel.RefreshMoney();
			//Full version
			//GetMainScenePanel()?.RefreshMoney();
		}
		public void OnDiamondChanged()
		{
			MainScenePanel mainScenePanel = GetMainScenePanel();
			if (mainScenePanel != null)
				mainScenePanel.RefreshDiamond();
			//Full version
			//GetMainScenePanel()?.RefreshDiamond();
		}
		public void OnLastLoginTimeChanged()
		{
			//Add your code here.
		}
		public void OnExpChanged()
		{
			MainScenePanel mainScenePanel = GetMainScenePanel();
			if (mainScenePanel != null)
				mainScenePanel.RefreshExp();
			//Full version
			//GetMainScenePanel()?.RefreshExp();
		}
		public void OnLvChanged()
		{
			MainScenePanel mainScenePanel = GetMainScenePanel();
			if (mainScenePanel != null)
				mainScenePanel.RefreshLV();
			//Full version
			//GetMainScenePanel()?.RefreshLV();
		}
		public void OnVpChanged()
		{
			MainScenePanel mainScenePanel = GetMainScenePanel();
			if (mainScenePanel != null)
				mainScenePanel.RefreshVP();
			//Full version
			//GetMainScenePanel()?.RefreshVP();
		}
		public void OnVpTimeChanged()
		{
			//Add your code here.
		}
		public void OnVipExpChanged()
		{
			//Add your code here.
		}
		public void OnDeleted()
		{
			//Add your code here.
		}

		public void NotifyValuesChanged()
		{
			if (CSL_Utils.CheckSendMask(_sendMask_, 1UL)) OnUidChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 2UL)) OnNicknameChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 4UL)) OnIconChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 8UL)) OnMoneyChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 16UL)) OnDiamondChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 32UL)) OnLastLoginTimeChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 64UL)) OnExpChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 128UL)) OnLvChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 256UL)) OnVpChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 512UL)) OnVpTimeChanged();
			if (CSL_Utils.CheckSendMask(_sendMask_, 1024UL)) OnVipExpChanged();
			if (_sendMask_ > 0) OnChanged();
		}
	}
}
