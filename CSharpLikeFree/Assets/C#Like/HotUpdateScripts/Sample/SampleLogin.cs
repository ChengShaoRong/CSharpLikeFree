//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using System;
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    /// <summary>
    /// Sample for test build-in login and third party login.
    /// </summary>
    public class SampleLogin : LikeBehaviour
    {
        /// <summary>
        /// This the singleton of SampleLogin for quick accept in other modular.
        /// You also can use HotUpdateManager.GetHotUpdate("Assets/C#Like/Sample/SampleLogin.prefab") instead.
        /// </summary>
        public static SampleLogin Instance { get; set; }
        void OnDestroy()
        {
            Instance = null;
        }
        void Start()
        {
            Instance = this;
            //Start connect if was not connected
            SampleSocket.Init();
            if (!SampleSocket.Instance.IsConnected())//check whether Socket was connected
                SampleSocket.Instance.Connect();
            OnLogin(SampleSocket.account);
            string name = PlayerPrefs.GetString("myAccountName", "");
            string password = PlayerPrefs.GetString("myAccountPassword", "");
            if (string.IsNullOrEmpty(name))
            {
                name = System.Guid.NewGuid().ToString("N");
                PlayerPrefs.SetString("myAccountName", name);
                PlayerPrefs.Save();
            }
            GetComponent<InputField>("BuildInLoginName").text = name;
            GetComponent<InputField>("BuildInLoginPasswrod").text = password;
            string uidSDK = PlayerPrefs.GetString("thirdPartyAccountUID", "");
            string tokenSDK = PlayerPrefs.GetString("thirdPartyAccountToken", "");
            if (string.IsNullOrEmpty(uidSDK))
            {
                long timestamp = DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
                uidSDK = "ThirdPartyUID" + timestamp.ToString();
                tokenSDK = CSL_Utils.GetMD5(uidSDK);
                PlayerPrefs.SetString("thirdPartyAccountUID", uidSDK);
                PlayerPrefs.SetString("thirdPartyAccountToken", tokenSDK);
                PlayerPrefs.Save();
            }
            GetComponent<InputField>("ThirdPartyLoginUID").text = uidSDK;
            GetComponent<InputField>("ThirdPartyLoginToken").text = tokenSDK;
        }
        void ShowMsg(string msg)
        {
            GetComponent<Text>("Msg").text = msg;
        }
        void OnClickBuildInLogin()
        {
            string name = GetComponent<InputField>("BuildInLoginName").text.Trim();
            string password = GetComponent<InputField>("BuildInLoginPasswrod").text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                ShowMsg("name can't be empty");
                return;
            }
            if (name.Length < 6 || name.Length > 64)
            {
                ShowMsg("name length error");
                return;
            }
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["packetType"] = "AccountLogin";
            jsonData["acctType"] = 0;// (int)Account.AccountType.BuildIn;
            jsonData["password"] = password;
            SampleSocket.Instance.Send(jsonData);
            PlayerPrefs.SetString("myAccountPassword", password);
            PlayerPrefs.Save();
        }
        public void OnCB_ChangeNameAndPassword(JSONData jsonData)
        {
            SampleSocket.account.password = jsonData["password"];
            SampleSocket.account.name = jsonData["name"];
            PlayerPrefs.SetString("myAccountPassword", SampleSocket.account.password);
            PlayerPrefs.Save();
            ShowMsg("Change password or name");
        }
        void OnClickChangeNameAndPassword()
        {
            string pwOld = GetComponent<InputField>("BuildInChangePasswordOld").text.Trim();
            string pwNew = GetComponent<InputField>("BuildInChangePasswordNew").text.Trim();
            string pwNewConfirm = GetComponent<InputField>("BuildInChangePasswordNewConfirm").text.Trim();
            if (pwOld.Length > 64)//can be empty
            {
                ShowMsg("PasswordOld length error");
                return;
            }
            if (pwNew.Length < 6 || pwNew.Length > 64)
            {
                ShowMsg("PasswordNew length error");
                return;
            }
            if (pwNew != pwNewConfirm)
            {
                ShowMsg("PasswordNew not match");
                return;
            }
            //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.AccountChangeNameAndPassword);
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["packetType"] = "AccountChangeNameAndPassword";
            jsonData["password"] = pwOld;
            jsonData["newPassword"] = pwNew;
            SampleSocket.Instance.Send(jsonData);
        }
        void OnClickThirdPartyLogin()
        {
            //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.AccountLogin);
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["packetType"] = "AccountLogin";
            string uid = GetComponent<InputField>("ThirdPartyLoginUID").text.Trim();
            if (uid.Length < 6 || uid.Length > 64)
            {
                ShowMsg("uid length error");
                return;
            }
            jsonData["name"] = uid;
            jsonData["acctType"] = 1;// (int)Account.AccountType.ThirdParty_Test;
            jsonData["password"] = GetComponent<InputField>("ThirdPartyLoginToken").text;
            SampleSocket.Instance.Send(jsonData);
        }
        /// <summary>
        /// The click event of the "Back" button
        /// </summary>
        void OnClickBack()
        {
            HotUpdateManager.Show("Assets/C#Like/Sample/SampleCSharpLikeHotUpdate.prefab");//back to SampleCSharpLikeHotUpdate
            HotUpdateManager.Hide("Assets/C#Like/Sample/SampleLogin.prefab", true);//close self
        }
        public void OnLogin(Account account)
        {
            if (account != null)
                ShowMsg("Current login account = " + account.ToString());
            else
                ShowMsg("Current login account null");
        }
    }
}