//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace TreasureBox
{
    /// <summary>
    /// Bag panel
    /// </summary>
    public class BagPanel : LikeBehaviour
    {
        [SerializeField]
        Transform transformItems;

        void Start()
        {
            Global.LocalizeText(gameObject);
            List<Item> items = new List<Item>();
            foreach (Item item in MySocket.account.items.Values)
                items.Add(item);
            foreach (Item item in items)
            {
                ItemIcon.NewInstanceByItem(transformItems, item, ItemIcon.ItemIconOption_ShowAll);
            }
        }
        //Free version not support Coroutine
        //IEnumerator Start()
        //{
        //    Global.LocalizeText(gameObject);
        //    List<Item> items = new List<Item>();
        //    foreach (Item item in MySocket.account.items.Values)
        //        items.Add(item);
        //    yield return null;
        //    foreach (Item item in items)
        //    {
        //        ItemIcon.NewInstanceByItem(transformItems, item, ItemIcon.ItemIconOption_ShowAll);
        //        yield return new WaitForSeconds(0.02f);
        //    }
        //}
        void OnClickClose()
        {
            Global.HidePanel("BagPanel", true);
        }
    }
}