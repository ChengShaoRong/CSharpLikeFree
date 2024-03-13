//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

namespace TreasureBox
{
    /// <summary>
    /// Main scene panel, we keep this main scene panel in momery after login success.
    /// </summary>
    public class MainScenePanel : LikeBehaviour
    {
        [SerializeField]
        TMP_Text textNickname;
        public void RefreshNickname()
        {
            textNickname.text = MySocket.account.nickname;
        }
        [SerializeField]
        TMP_Text textLV;
        public void RefreshLV()
        {
            textLV.text = "Lv." + MySocket.account.lv;
        }
        [SerializeField]
        TMP_Text textExpValue;
        [SerializeField]
        Slider sliderExpBar;
        public void RefreshExp()
        {
            int nextExp = ExpCsv.GetExp(MySocket.account.lv + 1, 0);
            textExpValue.text = MySocket.account.exp + "/" + nextExp;
            sliderExpBar.value = MySocket.account.exp / (float)nextExp;
        }
        [SerializeField]
        TMP_Text textMoney;
        public void RefreshMoney()
        {
            textMoney.text = MySocket.account.money + "";
        }
        [SerializeField]
        TMP_Text textDiamond;
        public void RefreshDiamond()
        {
            textDiamond.text = MySocket.account.diamond + "";
        }
        [SerializeField]
        KissImage imageIcon;
        public void RefreshIcon()
        {
            CharacterCsv csv = CharacterCsv.Get(MySocket.account.icon);
            imageIcon.SpriteName = ((csv != null) ? csv.icon : "icon");
        }
        [SerializeField]
        TMP_Text textVP;
        public void RefreshVP()
        {
            textVP.text = MySocket.account.vp + "";
        }
        void Start()
        {
            Global.LocalizeText(gameObject);
            RefreshNickname();
            RefreshLV();
            RefreshExp();
            RefreshVP();
            RefreshMoney();
            RefreshDiamond();
            RefreshIcon();
            WaitingPanel.Hide();

            //Show a spine actor for demo
            //SpineActor.NewInstance("hero-pro", transform);
        }
        void OnClickIcon()
        {
            MessageTips.Show("OnClickIcon");
        }
        void OnClickVP()
        {
            MessageTips.Show("OnClickVP");
        }
        void OnClickMoney()
        {
            MessageTips.Show("OnClickMoney");
        }
        void OnClickDiamond()
        {
            MessageTips.Show("OnClickDiamond");
        }
        void OnClickGift()
        {
            MessageTips.Show("OnClickGift");
        }
        [SerializeField]
        GameObject goBtMailSpot;
        public void RefreshMailSpot()
        {
            bool showSpot = false;
            foreach (Mail mail in MySocket.account.mails.Values)
            {
                if (mail.ShowSpot())
                {
                    showSpot = true;
                    break;
                }
            }
            goBtMailSpot.SetActive(showSpot);
        }
        void OnClickMail()
        {
            if (MySocket.account.mails.Count == 0)
                MessageTips.Show("LT_Mail_tip_NoMail");
            else
                Global.ShowPanel("MailPanel", null);
        }
        void OnClickNotice()
        {
            Global.ShowPanel("NoticePanel", null);
        }
        void OnClickHelp()
        {
            MessageTips.Show("OnClickHelp");
        }
        void OnClickTask()
        {
            MessageTips.Show("OnClickTask");
        }
        [SerializeField]
        GameObject goBtSignInSpot;
        public void RefreshSignInSpot()
        {
            goBtSignInSpot.SetActive(MySocket.account.signIn.ShowSpot());
        }
        void OnClickSignIn()
        {
            if (MySocket.account.signIn == null)//The sign in data may be not send back to client yet.
            {
                MessageTips.Show("LT_Tips_SignInNull");
                return;
            }
            Global.ShowPanel("SignInPanel", null);
        }
        void OnClickAdventure()
        {
            MessageTips.Show("OnClickAdventure");
        }
        void OnClickShop()
        {
            MessageTips.Show("OnClickShop");
        }
        void OnClickArena()
        {
            MessageTips.Show("OnClickArena");
        }
        void OnClickBag()
        {
            if (MySocket.account.items.Count == 0)//The items may be not send back to client yet or had no item.
            {
                MessageTips.Show("LT_Tips_ItemCount0");
                return;
            }
            Global.ShowPanel("BagPanel", null);
        }
    }
}