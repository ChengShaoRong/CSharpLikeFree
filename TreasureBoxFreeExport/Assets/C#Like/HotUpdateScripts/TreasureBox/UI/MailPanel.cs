//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

namespace TreasureBox
{
    /// <summary>
    /// Mail panel
    /// </summary>
    public class MailPanel : LikeBehaviour
    {
        /// <summary>
        /// The checkbox for notice classification
        /// </summary>
        public class MailTitle : LikeBehaviour
        {
            Mail mail;
            MailPanel panel;
            public static MailTitle NewInstance(MailPanel panel, Transform transformParent, Mail mail)
            {
                GameObject go = ResourceManager.Instantiate<GameObject>(Global.GetPrefabFullName("MailPanel.MailTitle"), transformParent);
                if (go == null)
                    return null;
                MailTitle nt = go.GetComponent<HotUpdateBehaviour>().ScriptInstance as MailTitle;
                nt.mail = mail;
                nt.panel = panel;
                go.GetComponent<Toggle>().group = transformParent.GetComponent<ToggleGroup>();
                return nt;
            }

            [SerializeField]
            TMP_Text textTitle;
            [SerializeField]
            GameObject goRead;
            [SerializeField]
            GameObject goSpot;
            [SerializeField]
            TMP_Text textAppendix;
            [SerializeField]
            TMP_Text textTime;
            void Start()
            {
                RefreshUI();
            }

            public void RefreshUI()
            {
                textTitle.text = mail.title;
                goRead.SetActive(mail.wasRead > 0);
                goSpot.SetActive(mail.ShowSpot());
                if (mail.Appendix().Count > 0)
                    textAppendix.text = string.Format(Global.GetString("LT_Mail_Appendix"), mail.Appendix().Count);
                else
                    textAppendix.gameObject.SetActive(false);
                textTime.text = mail.createTime.ToString("yyyy-MM-dd");
                gameObject.name = mail.SortKey();
            }

            void OnToggleValueChanged(bool value)
            {
                if (value)
                {
                    panel.OnToggleValueChanged(mail);
                }
            }
        }
        Dictionary<Mail, MailTitle> mMails = new Dictionary<Mail, MailTitle>();
        [SerializeField]
        Transform transformTitle;
        [SerializeField]
        Transform transformAppendix;
        void Start()
        {
            Global.LocalizeText(gameObject);
            Global.DestroyChildImmediate(transformAppendix);
            if (MySocket.account.mails.Count > 0)
            {
                List<Mail> mails = MySocket.account.SortedMails();
                bool bFirst = true;
                foreach (Mail mail in mails)
                {
                    MailTitle mailTitle = MailTitle.NewInstance(this, transformTitle, mail);
                    mMails[mail] = mailTitle;
                    if (bFirst)
                    {
                        bFirst = false;
                        mailTitle.gameObject.GetComponent<Toggle>().isOn = true;
                    }
                }
                ResetCurrentMailIfNotExist();
            }
        }
        //Free version not support Coroutine
        //IEnumerator Start()
        //{
        //    Global.LocalizeText(gameObject);
        //    Global.DestroyChildImmediate(transformAppendix);
        //    yield return null;
        //    if (MySocket.account.mails.Count > 0)
        //    {
        //        List<Mail> mails = MySocket.account.SortedMails();
        //        bool bFirst = true;
        //        foreach (Mail mail in mails)
        //        {
        //            MailTitle mailTitle = MailTitle.NewInstance(this, transformTitle, mail);
        //            mMails[mail] = mailTitle;
        //            if (bFirst)
        //            {
        //                bFirst = false;
        //                mailTitle.gameObject.GetComponent<Toggle>().isOn = true;
        //            }
        //            yield return new WaitForSeconds(0.02f);
        //        }
        //        ResetCurrentMailIfNotExist();
        //    }
        //}
        void ResetCurrentMailIfNotExist()
        {
            if (mCurrentMail != null)
                return;
            if (transformTitle.childCount > 0)
                transformTitle.GetChild(0).GetComponent<Toggle>().isOn = true;
            else
                Global.HidePanel("MailPanel", true);
        }
        void OnClickDeleteAll()
        {
            if (MySocket.account.mails.Count == 0)
                return;
            List<int> uids = new List<int>();
            foreach (Mail mail in MySocket.account.mails.Values)
            {
                if (mail.wasRead > 0)
                {
                    if (mail.received == 0 && mail.Appendix().Count > 0)
                        continue;
                    uids.Add(mail.uid);
                }
            }
            if (uids.Count > 0)
            {
                //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.DeleteMail);
                JSONData jsonData = JSONData.NewDictionary();
                jsonData["packetType"] = "DeleteMail";
                jsonData["uids"] = uids;
                MySocket.Instance().Send(jsonData);
            }
        }
        void OnClickGetAll()
        {
            List<int> uids = new List<int>();
            foreach (Mail mail in MySocket.account.mails.Values)
            {
                if (mail.received == 0 && mail.Appendix().Count > 0)
                    uids.Add(mail.uid);
            }
            if (uids.Count > 0)
            {
                //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.TakeMailAppendix);
                JSONData jsonData = JSONData.NewDictionary();
                jsonData["packetType"] = "TakeMailAppendix";
                jsonData["uids"] = uids;
                MySocket.Instance().Send(jsonData);
            }
        }
        void OnClickGet()
        {
            if (mCurrentMail == null)
                return;
            if (mCurrentMail.received == 1)
            {
                MessageTips.Show("LT_Mail_tip_HadReceived");
                return;
            }
            if (mCurrentMail.Appendix().Count == 0)
            {
                MessageTips.Show("LT_Mail_tip_NoAppendix");
                return;
            }
            //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.TakeMailAppendix);
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["packetType"] = "TakeMailAppendix";
            jsonData["uids"] = new List<int>();
            jsonData["uids"].Add(mCurrentMail.uid);
            //Free version not support this
            //jsonData["uids"] = new List<int>() { mCurrentMail.uid };
            MySocket.Instance().Send(jsonData);
        }
        void OnClickDelete()
        {
            if (mCurrentMail == null)
                return;
            if (mCurrentMail.received == 0 && mCurrentMail.Appendix().Count > 0)
            {
                MessageTips.Show("LT_Mail_tip_DeleteButAppendixWasNotReceived");
                return;
            }
            //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.DeleteMail);
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["packetType"] = "DeleteMail";
            jsonData["uids"] = new List<int>();
            jsonData["uids"].Add(mCurrentMail.uid);
            //jsonData["uids"] = new List<int>() { mCurrentMail.uid };
            MySocket.Instance().Send(jsonData);
        }
        Mail mCurrentMail = null;
        void OnToggleValueChanged(Mail mail)
        {
            if (mCurrentMail != mail)
            {
                mCurrentMail = mail;
                RefreshMailContent(false);
                //Mark this mail was read
                if (mail.wasRead == 0)
                {
                    //JSONData jsonData = JSONData.NewPacket(typeof(PacketType), PacketType.ReadMail);
                    JSONData jsonData = JSONData.NewDictionary();
                    jsonData["packetType"] = "ReadMail";
                    jsonData["uid"] = mail.uid;
                    MySocket.Instance().Send(jsonData);
                }
            }
        }
        [SerializeField]
        TMP_Text textTitle;
        [SerializeField]
        TMP_Text textTime;
        [SerializeField]
        TMP_Text textContent;
        [SerializeField]
        TMP_Text textSenderName;
        [SerializeField]
        GameObject goReceived;
        [SerializeField]
        GameObject goBtGet;
        [SerializeField]
        GameObject goBtDelete;
        void RefreshMailContent(bool bUpdate)
        {
            //Title
            //Time
            //Content
            //Appendix
            //Show get button or show "Received" text
            //Show delete button if was read and appendix was received
            int appendixCount = mCurrentMail.Appendix().Count;
            if (!bUpdate)
            {
                textTitle.text = mCurrentMail.title;
                textTime.text = mCurrentMail.createTime.ToString("yyyy-MM-dd HH:mm:ss");
                textContent.text = mCurrentMail.content;
                textSenderName.text = mCurrentMail.senderName;
                transformAppendix.gameObject.SetActive(appendixCount > 0);
                if (appendixCount > 0)
                {
                    Global.DestroyChildImmediate(transformAppendix);
                    foreach (var item in mCurrentMail.Appendix())
                    {
                        ItemIcon.NewInstance(transformAppendix, item.Key, item.Value, ItemIcon.ItemIconOption_Default);
                    }
                }
            }
            goReceived.SetActive(mCurrentMail.received > 0 && appendixCount > 0);
            goBtGet.SetActive(mCurrentMail.received == 0 && appendixCount > 0);
            goBtDelete.SetActive(appendixCount == 0 || mCurrentMail.received > 0);
        }
        public void OnUpdateOrNewMails(List<Mail> mails)
        {
            foreach (Mail mail in mails)
            {
                if (mMails.ContainsKey(mail))
                {
                    mMails[mail].RefreshUI();
                    if (mCurrentMail == mail)
                        RefreshMailContent(true);
                }
                else
                {
                    mMails[mail] = MailTitle.NewInstance(this, transformTitle, mail);
                }
                //Free version not support 'out'
                //if (mMails.TryGetValue(mail, out MailTitle mailTitle))
                //{
                //    mailTitle.RefreshUI();
                //    if (mCurrentMail == mail)
                //        RefreshMailContent(true);
                //}
                //else
                //{
                //    mMails[mail] = MailTitle.NewInstance(this, transformTitle, mail);
                //}
            }
        }
        public void OnDeleteMails(List<Mail> mails)
        {
            foreach (Mail mail in mails)
            {
                if (mMails.ContainsKey(mail))
                {
                    GameObject.Destroy(mMails[mail].gameObject);
                    mMails.Remove(mail);
                    if (mCurrentMail == mail)
                        mCurrentMail = null;
                }
                //Free version not support 'out'
                //if (mMails.TryGetValue(mail, out MailTitle mailTitle))
                //{
                //    GameObject.Destroy(mailTitle.gameObject);
                //    mMails.Remove(mail);
                //    if (mCurrentMail == mail)
                //        mCurrentMail = null;
                //}
            }
            ResetCurrentMailIfNotExist();
        }
        void OnClickClose()
        {
            Global.HidePanel("MailPanel", true);
        }
    }
}