//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

namespace TreasureBox
{
    /// <summary>
    /// Sign in panel
    /// </summary>
    public class SignInPanel : LikeBehaviour
    {
        public class SignInIcon : LikeBehaviour
        {
            SignInCsv csv;
            public static SignInIcon NewInstance(Transform transformParent, int day)
            {
                GameObject go = ResourceManager.Instantiate<GameObject>(Global.GetPrefabFullName("SignInPanel.SignInIcon"), transformParent);
                if (go == null)
                    return null;
                SignInIcon signInIcon = go.GetComponent<HotUpdateBehaviour>().ScriptInstance as SignInIcon;
                signInIcon.csv = SignInCsv.Get(day);
                return signInIcon;
            }
            [SerializeField]
            KissImage imageFrame;
            [SerializeField]
            KissImage imageIcon;
            [SerializeField]
            TMP_Text textCount;
            [SerializeField]
            TMP_Text textName;
            [SerializeField]
            TMP_Text textVip;
            [SerializeField]
            GameObject goReceive;
            [SerializeField]
            GameObject goSpot;
            void Start()
            {
                ItemCsv itemCsv = ItemCsv.Get(csv.itemId);
                //Frame
                imageFrame.SpriteName = itemCsv.frame;
                //Icon
                imageIcon.SpriteName = itemCsv.icon;
                //Count
                textCount.text = ""+ csv.itemCount;
                //Name
                textName.text = string.Format(Global.GetString("LT_SignIn_Day"), csv.day);
                //Vip
                textVip.text = "vip" + csv.vip + " x10";
                //textVip.text = $"vip{csv.vip} x10";

                RefreshUI();
            }
            public void RefreshUI()
            {
                SignIn signIn = MySocket.account.signIn;
                goReceive.SetActive(signIn.SignInDays().Count >= csv.day);
                goSpot.SetActive(signIn.ShowSpot() && signIn.SignInDays().Count == csv.day - 1);
            }
            public void OnClick()
            {
                SignIn signIn = MySocket.account.signIn;
                if (!signIn.ShowSpot())//Today reward was token
                {
                    Debug.Log("SignInIcon:OnClick: Today reward was token");
                    return;
                }
                if (signIn.SignInDays().Count == csv.day - 1)
                {
                    //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.SignInForGift);
                    JSONData jsonData = JSONData.NewDictionary();
                    jsonData["packetType"] = "SignInForGift";
                    MySocket.Instance().Send(jsonData);
                }
                else
                    Debug.Log("SignInIcon:OnClick: Not click today");
            }
        }
        List<SignInIcon> mSignInIcons = new List<SignInIcon>();
        [SerializeField]
        Transform transformIcons;
        void Start()
        {
            Global.LocalizeText(gameObject);
            DateTime dtNow = DateTime.Now;
            int dayEnd = (new DateTime(dtNow.Year, dtNow.Month, 1)).AddMonths(1).AddDays(-1).Day;
            for(int i=1; i<= dayEnd; i++)
            {
                mSignInIcons.Add(SignInIcon.NewInstance(transformIcons, i));
            }
        }
        //Free version not support Coroutine
        //IEnumerator Start()
        //{
        //    Global.LocalizeText(gameObject);
        //    yield return null;
        //    DateTime dtNow = DateTime.Now;
        //    int dayEnd = (new DateTime(dtNow.Year, dtNow.Month, 1)).AddMonths(1).AddDays(-1).Day;
        //    for (int i = 1; i <= dayEnd; i++)
        //    {
        //        mSignInIcons.Add(SignInIcon.NewInstance(transformIcons, i));
        //        yield return new WaitForSeconds(0.02f);
        //    }
        //}

        public void RefreshUI()
        {
            foreach (SignInIcon signInIcon in mSignInIcons)
                signInIcon.RefreshUI();
        }

        void OnClickClose()
        {
            Global.HidePanel("SignInPanel", true);
        }
    }
}