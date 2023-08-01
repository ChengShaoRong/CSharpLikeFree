//It's a single cs file due to C#Like FREE version not support inherit class. 
//You have to merge this code to your project very carefully if you had modified this file.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    public class Account
    {
        public int uid;
        public int acctType;
        public DateTime createTime;
        public string name;
        public string password;
        public string nickname;
        public int money;
        public int score;
        public DateTime scoreTime;
        public DateTime lastLoginTime;
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
        public void OnCallbackObjectItems(List<Item> data)
        {
            //We print the item that just changed, you may do some update to the UI
            foreach (Item item in data)
                Debug.Log("OnCallbackObjectItems:changed item:" + item.ToString());
            //We print all item count
            Debug.Log("OnCallbackObjectItems:now all item count = " + items.Count);
        }
        public void OnCallbackDeleteItems(List<Item> data)
        {
            //We print the item that just deleted, you may do some update to the UI
            foreach (Item item in data)
                Debug.Log("OnCallbackDeleteItems:delete item:" + item.ToString());
            //We print all item count
            Debug.Log("OnCallbackDeleteItems:now all item count = " + items.Count);
        }
        public void OnCallbackObjectMails(List<Mail> data)
        {
            //We print the item that just changed, you may do some update to the UI
            foreach (Mail mail in data)
                Debug.Log("OnCallbackObjectMails:changed mail:" + mail.ToString());
            //We print all mail count
            Debug.Log("OnCallbackObjectMails:now all mail count = " + mails.Count);
        }
        public void OnCallbackDeleteMails(List<Mail> data)
        {
            //We print the item that just deleted, you may do some update to the UI
            foreach (Mail mail in data)
                Debug.Log("OnCallbackObjectMails:deleted mail:" + mail.ToString());
            //We print all mail count
            Debug.Log("OnCallbackObjectMails:now all mail count = " + mails.Count);
        }
        public void OnCallbackObjectSignIn()
        {
            Debug.Log("OnCallbackObjectSignIn:" + signIn.ToString());
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
            else if (name == "items")
            {
                List<Item> _items_ = Item.ToItems(jsonData["obj"]);
                foreach (Item _one_ in _items_)
                {
                    items[_one_.itemId] = _one_;
                    _one_.NotifyValuesChanged();
                }
                OnCallbackObjectItems(_items_);
            }
            else if (name == "mails")
            {
                List<Mail> _mails_ = Mail.ToMails(jsonData["obj"]);
                foreach (Mail _one_ in _mails_)
                {
                    mails[_one_.uid] = _one_;
                }
                OnCallbackObjectMails(_mails_);
            }
            else if (name == "signIn")
            {
                signIn = SignIn.ToSignIn(jsonData["obj"]);
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
            else if (name == "items")
            {
                List<Item> _deletes_ = new List<Item>();
                foreach (int _itemId_ in ids)
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
            else if (name == "mails")
            {
                List<Mail> _deletes_ = new List<Mail>();
                foreach (int _uid_ in ids)
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
            else if (name == "signIn")
            {
                signIn.Clear();
                OnCallbackDeleteSignIn();
                signIn = null;
            }
            else
                Debug.LogError("CB_Object unsupported name " + name);
        }
        [KissJsonDontSerialize]
        public Dictionary<int, Item> items = new Dictionary<int, Item>();
        public void SetItems(List<Item> items)
        {
            foreach (Item one in items)
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
        public Dictionary<int, Mail> mails = new Dictionary<int, Mail>();
        public void SetMails(List<Mail> mails)
        {
            foreach (Mail one in mails)
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
        public SignIn signIn = null;
        public void OnChanged()
        {
            SampleCSharpLikeHotUpdate.strTips = "Account:" + ToString();
        }
        public void OnUidChanged()
        {
            //Add your code here.
        }
        public void OnNameChanged()
        {
            //Add your code here.
        }
        public void OnNicknameChanged()
        {
            //Add your code here.
        }
        public void OnMoneyChanged()
        {
            //Add your code here.
        }
        public void OnScoreChanged()
        {
            //Add your code here.
        }
        public void OnLastLoginTimeChanged()
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
            if (CSL_Utils.CheckSendMask(_sendMask_, 2)) OnNameChanged();
            if (CSL_Utils.CheckSendMask(_sendMask_, 4)) OnNicknameChanged();
            if (CSL_Utils.CheckSendMask(_sendMask_, 8)) OnMoneyChanged();
            if (CSL_Utils.CheckSendMask(_sendMask_, 16)) OnScoreChanged();
            if (CSL_Utils.CheckSendMask(_sendMask_, 32)) OnLastLoginTimeChanged();
            if (_sendMask_ > 0) OnChanged();
        }
    }
}
