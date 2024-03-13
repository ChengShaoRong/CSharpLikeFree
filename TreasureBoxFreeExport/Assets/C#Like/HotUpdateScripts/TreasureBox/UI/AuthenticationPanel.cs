//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace TreasureBox
{
    /// <summary>
    /// Authentication panel
    /// </summary>
    public class AuthenticationPanel : LikeBehaviour
    {
        public class AuthenticationManager
        {
            class AuthenticationInfo
            {
                public string name;
                public string password;
                public long time;
                public string SortKey()
                {
                    return (100000000000 - time).ToString() + "_" + name;
                }
                //Free version not support customize 'get/set'
                //public string SortKey => $"{100000000000 - time}_{name}";
            }
            SortedDictionary<string, AuthenticationInfo> mAccounts;
            bool mSavePassword = true;
            public bool GetSavePassword()
            {
                return mSavePassword;
            }
            public void SetSavePassword(bool value)
            {
                if (mSavePassword != value)
                {
                    mSavePassword = value;
                    Save();
                }
            }
            //Free version not support customize 'get/set'
            //public bool SavePassword
            //{
            //    get => mSavePassword; 
            //    set
            //    {
            //        if (mSavePassword != value)
            //        {
            //            mSavePassword = value;
            //            Save();
            //        }
            //    }
            //}
            public AuthenticationManager()
            {
                mAccounts = new SortedDictionary<string, AuthenticationInfo>();
                mSavePassword = MyPlayerPrefs.GetInt("AccountSavePassword", 1) > 0;
                int count = MyPlayerPrefs.GetInt("AccountCount", 0);
                for (int i = 1; i <= count; i++)
                {
                    AuthenticationInfo ai = new AuthenticationInfo();
                    ai.name = MyPlayerPrefs.GetString("AccountName" + i, "");
                    ai.password = (mSavePassword ? MyPlayerPrefs.GetString("AccountPW" + i, "") : "");
                    ai.time = Convert.ToInt64(MyPlayerPrefs.GetString("AccountTime" + i, "")); ;
                    //Free version not support
                    //AuthenticationInfo ai = new AuthenticationInfo()
                    //{
                    //    name = MyPlayerPrefs.GetString("AccountName" + i, ""),
                    //    password = mSavePassword ? MyPlayerPrefs.GetString("AccountPW" + i, "") : "",
                    //    time = Convert.ToInt64(MyPlayerPrefs.GetString("AccountTime" + i, ""))
                    //};
                    mAccounts[ai.SortKey()] = ai;
                }
            }
            void Save()
            {
                MyPlayerPrefs.SetInt("AccountCount", mAccounts.Count);
                int i = 1;
                foreach (AuthenticationInfo one in mAccounts.Values)
                {
                    MyPlayerPrefs.SetString("AccountName" + i, one.name);
                    MyPlayerPrefs.SetString("AccountPW" + i, mSavePassword ? one.password : "");
                    MyPlayerPrefs.SetString("AccountTime" + i, one.time.ToString());
                    ++i;
                }
                MyPlayerPrefs.Save();
            }
            AuthenticationInfo Get(string loginName)
            {
                foreach(AuthenticationInfo one in mAccounts.Values)
                {
                    if (one.name == loginName)
                        return one;
                }
                return null;
            }
            AuthenticationInfo GetGuest()
            {
                foreach (AuthenticationInfo one in mAccounts.Values)
                {
                    if (one.name.Length == 32)
                        return one;
                }
                return null;
            }
            public void Add(string loginName, string password)
            {
                if (string.IsNullOrEmpty(loginName))
                    return;
                AuthenticationInfo ai = Get(loginName);
                if (ai != null)
                {
                    mAccounts.Remove(ai.SortKey());
                    ai.password = password;
                    ai.time = Global.GetCurrentTimestamp();
                }
                else
                {
                    ai = GetGuest();
                    if (ai == null)
                    {
                        ai = new AuthenticationInfo();
                        ai.name = loginName;
                        ai.password = password;
                        ai.time = Global.GetCurrentTimestamp();
                        //ai = new AuthenticationInfo()
                        //{
                        //    name = loginName,
                        //    password = password,
                        //    time = Global.GetCurrentTimestamp()
                        //};
                    }
                    else
                    {
                        mAccounts.Remove(ai.SortKey());
                        ai.name = loginName;
                        ai.password = password;
                        ai.time = Global.GetCurrentTimestamp();
                    }
                }
                mAccounts[ai.SortKey()] = ai;
                Save();
            }
            public string GetPassword(string loginName)
            {
                AuthenticationInfo ai = Get(loginName);
                return ai != null ? ai.password : "";
            }
            public string GuestLoginName()
            {
                //get
                //{
                    AuthenticationInfo ai = GetGuest();
                    return ai != null ? ai.name : "";
                //}
            }
            public string CreateGuest()
            {
                AuthenticationInfo ai = new AuthenticationInfo();
                ai.name = Global.GetDeviceUniqueIdentifier();
                ai.password = "";
                ai.time = Global.GetCurrentTimestamp();
                //AuthenticationInfo ai = new AuthenticationInfo()
                //{
                //    name = Global.GetDeviceUniqueIdentifier(),
                //    password = "",
                //    time = Global.GetCurrentTimestamp()
                //};
                Save();
                return ai.name;
            }
            public void Remove(string loginName)
            {
                AuthenticationInfo ai = Get(loginName);
                if (ai != null)
                {
                    mAccounts.Remove(ai.SortKey());
                    Save();
                }
            }
            public List<string> GetAccounts()
            {
                List<string> accounts = new List<string>();
                foreach (AuthenticationInfo ai in mAccounts.Values)
                    accounts.Add(ai.name);
                return accounts;
            }
        }
        public static int UIType_LoginUI = 0;
        public static int UIType_RegisterUI = 1;
        public static int UIType_ChangePasswordUI = 2;
        //Free version not support enum
        //public enum UIType
        //{
        //    LoginUI,
        //    RegisterUI,
        //    ChangePasswordUI
        //}
        int mCurrentType = AuthenticationPanel.UIType_LoginUI;
        public static AuthenticationManager authenticationManager;
        [SerializeField]
        GameObject goBtGuestLogin;
        [SerializeField]
        GameObject goTweenUIs;
        [SerializeField]
        TMP_Text textTitle;
        [SerializeField]
        TMP_Text textRegisterOrBind;
        [SerializeField]
        Toggle toggleLoginRemember;
        [SerializeField]
        RectTransform rectTransformBtLogin;
        [SerializeField]
        TMP_InputField inputLoginName;
        [SerializeField]
        TMP_InputField inputLoginPassword;
        [SerializeField]
        TMP_Dropdown dropdownLoginNameHistory;
        void Start()
        {
            Debug.Log("AuthenticationPanel:Start");
            authenticationManager = new AuthenticationManager();
            Global.LocalizeText(gameObject);
            textTitle.text = Global.GetString("LT_Auth_Title" + (int)mCurrentType);
            textRegisterOrBind.text = Global.GetString(authenticationManager.GuestLoginName().Length == 0 ? "LT_Auth_bt_Register" : "LT_Auth_bt_Bind");

            //Refresh the data to UI
            toggleLoginRemember.isOn = authenticationManager.GetSavePassword();
            List<string> accounts = authenticationManager.GetAccounts();
            if (accounts.Count > 0
                && authenticationManager.GuestLoginName().Length == 0)
            {
                rectTransformBtLogin.anchoredPosition = new Vector2(0f, -50f);
                goBtGuestLogin.SetActive(false);
            }
            string loginName = (accounts.Count > 0) ? accounts[0] : "";
            inputLoginName.text = loginName;
            inputLoginPassword.text = authenticationManager.GetPassword(loginName);
            dropdownLoginNameHistory.AddOptions(accounts);
        }
        void OnClickClose()
        {
            Global.HidePanel("AuthenticationPanel", true);
        }

        void OnClickShowUI(int value)
        {
            int newValue = value;
            if (mCurrentType != newValue)
            {
                mCurrentType = newValue;
                Vector2 pos;
                if (newValue == AuthenticationPanel.UIType_LoginUI)
                    pos = new Vector2(-825f, 150f);
                else if (newValue == AuthenticationPanel.UIType_RegisterUI)
                    pos = new Vector2(-275f, 150f);
                else
                    pos = new Vector2(-1375f, 150f);
                KissTweenAnchoredPosition.Tween(goTweenUIs, 0.3f, pos);
                //KissTweenAnchoredPosition.Tween(goTweenUIs, 0.3f, newValue switch
                //{
                //    UIType.LoginUI => new Vector2(-825f, 150f),
                //    UIType.RegisterUI => new Vector2(-275f, 150f),
                //    _ => new Vector2(-1375f, 150f),
                //});
            }
        }
        void OnTweener(KissTweenBase tween)
        {
            textTitle.text = Global.GetString("LT_Auth_Title" + (int)mCurrentType);
        }
        void OnClickGuestLogin()
        {
            Debug.Log("OnClickGuestLogin");
            string strName = authenticationManager.GuestLoginName();
            if (strName == "")
                strName = authenticationManager.CreateGuest();
            WaitingPanel.Show(-1f, null, true);
            JSONData post = JSONData.NewDictionary();
            post["name"] = strName;
            Global.Sign(post);
            behaviour.HttpPost(Global.Url + "AuthGuestLogin", post,
                (string callback, string error) =>
                {
                    WaitingPanel.Hide();
                    Debug.Log("callback=" + callback + ",error=" + error);
                    //Debug.Log($"callback={callback},error={error}");
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show(error, "", "", null);
                        return;
                    }
                    JSONData callbackJSON = KissJson.ToJSONData(callback);
                    if (callbackJSON["code"] == 0)
                    {
                        Global.HidePanel("AuthenticationPanel", true);

                        authenticationManager.Add(strName, "");

                        JSONData loginData = JSONData.NewDictionary();
                        loginData["uid"] = callbackJSON["uid"];
                        loginData["token"] = callbackJSON["token"];
                        MessageTips.Show("LT_Auth_LoginSuccess");
                        LoginPanel.Instance.StartLogin(loginData);
                    }
                    else
                    {
                        MessageBox.Show(callbackJSON["error"], "", "", null);
                    }
                });
        }
        void OnClickLogin()
        {
            Debug.Log("OnClickLogin");
            string strName = inputLoginName.text.Trim();
            string strPasswrod = inputLoginPassword.text.Trim();
            if (strName.Length < 6 || strName.Length > 30)
            {
                MessageTips.Show("LT_Auth_NameLengthInvalid");
                return;
            }
            if (strPasswrod.Length < 6 || strPasswrod.Length > 30)
            {
                MessageTips.Show("LT_Auth_PasswordLengthInvalid");
                return;
            }
            WaitingPanel.Show(-1f, null, true);
            JSONData post = JSONData.NewDictionary();
            post["name"] = strName;
            post["password"] = Global.HashPassword(strPasswrod);
            Global.Sign(post);
            behaviour.HttpPost(Global.Url + "AuthLogin", post,
                (callback, error) =>
                {
                    WaitingPanel.Hide();
                    Debug.Log("callback=" + callback + ",error=" + error);
                    //Debug.Log($"callback={callback},error={error}");
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show(error, "", "", null);
                        return;
                    }
                    JSONData callbackJSON = KissJson.ToJSONData(callback);
                    if (callbackJSON["code"] == 0)
                    {
                        Global.HidePanel("AuthenticationPanel", true);

                        authenticationManager.Add(strName, strPasswrod);

                        JSONData loginData = JSONData.NewDictionary();
                        loginData["uid"] = callbackJSON["uid"];
                        loginData["token"] = callbackJSON["token"];
                        MessageTips.Show("LT_Auth_LoginSuccess");
                        LoginPanel.Instance.StartLogin(loginData);
                    }
                    else
                    {
                        MessageBox.Show(callbackJSON["error"], "", "", null);
                    }
                });
        }
        /// <summary>
        /// Toggle for save password
        /// </summary>
        void OnToggleValueChanged(int value)
        {
            Debug.Log("OnToggleValueChanged:"+ value);
        }
        /// <summary>
        /// Dropdown for login name
        /// </summary>
        void OnDropdownValueChanged(int value)
        {
            string strNew = dropdownLoginNameHistory.options[value].text.Trim();
            inputLoginName.text = strNew;
            inputLoginPassword.text = MyPlayerPrefs.GetString("LoginPW_" + strNew, "");
        }
        [SerializeField]
        TMP_InputField inputRegisterName;
        [SerializeField]
        TMP_InputField inputRegisterPassword;
        [SerializeField]
        TMP_InputField inputRegisterPassword2;
        void OnClickRegisterOrBind()
        {
            Debug.Log("OnClickRegisterOrBind");
            string strName = inputRegisterName.text.Trim();
            string strPasswrod = inputRegisterPassword.text.Trim();
            string strPasswrod2 = inputRegisterPassword2.text.Trim();
            if (strName.Length < 6 || strName.Length > 30)
            {
                MessageTips.Show("LT_Auth_NameLengthInvalid");
                return;
            }
            if (strPasswrod.Length < 6 || strPasswrod.Length > 30)
            {
                MessageTips.Show("LT_Auth_PasswordLengthInvalid");
                return;
            }
            if (strPasswrod != strPasswrod2)
            {
                MessageTips.Show("LT_Auth_PasswordConfirmInvalid");
                return;
            }
            WaitingPanel.Show(-1f, null, true);
            JSONData post = JSONData.NewDictionary();
            post["name"] = strName;
            post["password"] = Global.HashPassword(strPasswrod);
            post["guest"] = authenticationManager.GuestLoginName();
            Global.Sign(post);
            behaviour.HttpPost(Global.Url+ "AuthRegister", post, 
                (string callback, string error) =>
                {
                    WaitingPanel.Hide();
                    Debug.Log("callback=" + callback + ",error=" + error);
                    //Debug.Log($"callback={callback},error={error}");
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show(error, "", "", null);
                        return;
                    }
                    JSONData callbackJSON = KissJson.ToJSONData(callback);
                    Debug.Log("callbackJSON=" + callbackJSON.ToJson(true));
                    //Debug.Log($"callbackJSON={callbackJSON.ToJson(true)}");
                    if (callbackJSON["code"] == 0)
                    {
                        Global.HidePanel("AuthenticationPanel", true);

                        authenticationManager.Add(strName, strPasswrod);

                        JSONData loginData = JSONData.NewDictionary();
                        loginData["uid"] = callbackJSON["uid"];
                        loginData["token"] = callbackJSON["token"];
                        MessageTips.Show("LT_Auth_RegisterSuccess");
                        LoginPanel.Instance.StartLogin(loginData);
                    }
                    else
                    {
                        MessageBox.Show(callbackJSON["error"], "", "", null);
                    }
                });
        }
        [SerializeField]
        TMP_InputField inputChangePasswordOld;
        [SerializeField]
        TMP_InputField inputChangePasswordNew;
        [SerializeField]
        TMP_InputField inputChangePasswordNew2;
        void OnClickChangePassword()
        {
            Debug.Log("OnClickChangePassword");
            string strName = inputLoginName.text.Trim();
            string strOld = inputChangePasswordOld.text.Trim();
            string strNew = inputChangePasswordNew.text.Trim();
            string strNew2 = inputChangePasswordNew2.text.Trim();
            if (strName.Length < 6 || strName.Length > 30)
            {
                MessageTips.Show("LT_Auth_NameLengthInvalid");
                return;
            }
            if (strOld.Length < 6 || strOld.Length > 30)
            {
                MessageTips.Show("LT_Auth_PasswordLengthInvalid");
                return;
            }
            if (strNew.Length < 6 || strNew.Length > 30)
            {
                MessageTips.Show("LT_Auth_PasswordLengthInvalid");
                return;
            }
            if (strNew != strNew2)
            {
                MessageTips.Show("LT_Auth_PasswordConfirmInvalid");
                return;
            }
            if (strOld == strNew)
            {
                MessageTips.Show("LT_Auth_PasswordSame");
                return;
            }
            WaitingPanel.Show(-1f, null, true);
            JSONData post = JSONData.NewDictionary();
            post["name"] = strName;
            post["passwordOld"] = Global.HashPassword(strOld);
            post["passwordNew"] = Global.HashPassword(strNew);
            Global.Sign(post);
            behaviour.HttpPost(Global.Url + "AuthChangePassword", post,
                (string callback, string error) =>
                {
                    WaitingPanel.Hide();
                    Debug.Log("callback=" + callback + ",error=" + error);
                    //Debug.Log($"callback={callback},error={error}");
                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show(error, "", "", null);
                        return;
                    }
                    JSONData callbackJSON = KissJson.ToJSONData(callback);
                    if (callbackJSON["code"] == 0)
                    {
                        authenticationManager.Add(strName, strNew);

                        inputChangePasswordOld.text = "";
                        inputChangePasswordNew.text = "";
                        inputChangePasswordNew2.text = "";
                        OnClickShowUI(0);
                        MessageTips.Show("LT_Auth_ChangePasswordSuccess");
                    }
                    else
                    {
                        MessageBox.Show(callbackJSON["error"], "", "", null);
                    }
                });
        }
    }
}