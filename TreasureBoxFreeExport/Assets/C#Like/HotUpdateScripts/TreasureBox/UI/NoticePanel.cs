//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace TreasureBox
{
    /// <summary>
    /// Notice panel
    /// </summary>
    public class NoticePanel : LikeBehaviour
    {
        /// <summary>
        /// The checkbox for notice classification
        /// </summary>
        public class NoticeTitle : LikeBehaviour
        {
            JSONData notice;
            NoticePanel panel;
            public static NoticeTitle NewInstance(NoticePanel panel, Transform transformParent, JSONData notice, int index)
            {
                GameObject go = ResourceManager.Instantiate<GameObject>(Global.GetPrefabFullName("NoticePanel.NoticeTitle"), transformParent);
                if (go == null)
                    return null;
                NoticeTitle nt = HotUpdateBehaviour.GetComponentByType(go, typeof(NoticeTitle)) as NoticeTitle;
                nt.notice = notice;
                nt.panel = panel;
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.group = transformParent.GetComponent<ToggleGroup>();
                if (index == 0)
                    toggle.isOn = true;
                nt.RefreshUI();
                return nt;
            }

            [SerializeField]
            TMP_Text textTitle;
            void RefreshUI()
            {
                textTitle.text = notice["title"];
            }

            void OnToggleValueChanged(bool value)
            {
                if (value)
                {
                    panel.OnToggleValueChanged(notice);
                }
            }
        }
        void Start()
        {
            Global.LocalizeText(gameObject);
            //Free version
            GetServerNoticeForFreeVersion();

            //Full version
            //StartCoroutine("CorGetServerNotice");
        }
        void GetServerNoticeForFreeVersion()
        {
            WaitingPanel.Show(-1f, null, true);
            behaviour.HttpGet(Global.Url + "GetServerNotice", (callbackText, error) =>
            {
                WaitingPanel.Hide();
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError("GetServerNotice:error=" + error);
                    MessageBox2.Show("LT_Notice_GetServerNoticeFail", "LT_Tips",
                        "LT_Reconnect", () =>
                        {
                            GetServerNoticeForFreeVersion();
                        },
                        "LT_Quit", () =>
                        {
                            OnClickClose();
                        }
                    );
                    return;
                }
                Debug.Log("uwr.downloadHandler.text = "+callbackText);
                JSONData json = KissJson.ToJSONData(callbackText);
                if (json == null)
                {
                    MessageBox2.Show("LT_Notice_GetServerNoticeFail", "LT_Tips",
                        "LT_Reconnect", () =>
                        {
                            GetServerNoticeForFreeVersion();
                        },
                        "LT_Quit", () =>
                        {
                            OnClickClose();
                        }
                    );
                    return;
                }
                if (json.ContainsKey("error"))
                {
                    MessageBox2.Show(json["error"], "LT_Tips",
                        "LT_Reconnect", () =>
                        {
                            GetServerNoticeForFreeVersion();
                        },
                        "LT_Quit", () =>
                        {
                            OnClickClose();
                        }
                    );
                    return;
                }
                ShowServerNoticeForFreeVersion(json);
            });
        }
        //FREE version not support Coroutine
        //IEnumerator CorGetServerNotice()
        //{
        //    WaitingPanel.Show();
        //    using (UnityWebRequest uwr = UnityWebRequest.Get("http://127.0.0.1:9002/GetServerNotice"))
        //    {
        //        uwr.timeout = 10;
        //        yield return uwr.SendWebRequest();
        //        WaitingPanel.Hide();
        //        if (!string.IsNullOrEmpty(uwr.error))
        //        {
        //            Debug.LogError($"GetServerNotice:error={uwr.error}");
        //            MessageBox2.Show("LT_Notice_GetServerNoticeFail", "LT_Tips",
        //                "LT_Reconnect", () =>
        //                {
        //                    StartCoroutine("CorGetServerNotice");
        //                },
        //                "LT_Quit", () =>
        //                {
        //                    OnClickClose();
        //                }
        //            );
        //            yield break;
        //        }
        //        Debug.Log($"uwr.downloadHandler.text = {uwr.downloadHandler.text}");
        //        JSONData json = KissJson.ToJSONData(uwr.downloadHandler.text);
        //        StartCoroutine("CorShowServerNotice", json);
        //    }
        //}
        [SerializeField]
        Transform transformTitle;
        void ShowServerNoticeForFreeVersion(JSONData json)
        {
            int index = 0;
            foreach (JSONData one in json["notices" + Global.GetLanguage()].Value as List<JSONData>)
            {
                NoticeTitle.NewInstance(this, transformTitle, one, index++);
            }
        }
        //FREE version not support Coroutine
        //IEnumerator CorShowServerNotice(JSONData json)
        //{
        //    int index = 0;
        //    foreach (JSONData one in json["notices" + Global.Language].Value as List<JSONData>)
        //    {
        //        NoticeTitle.NewInstance(this, transformTitle, one, index++);
        //        yield return new WaitForSeconds(0.02f);
        //    }
        //}
        void OnClickClose()
        {
            Global.HidePanel("NoticePanel", true);
        }
        [SerializeField]
        TMP_Text textContent;
        void OnToggleValueChanged(JSONData notice)
        {
            textContent.text = notice["content"];
        }
    }
}