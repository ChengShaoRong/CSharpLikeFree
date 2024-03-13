//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
using System.Collections.Generic;

namespace TreasureBox
{
    /// <summary>
    /// Common reward panel
    /// </summary>
    public class CommonRewardPanel : LikeBehaviour
    {
        [SerializeField]
        Transform transformRewards;

        void Start()
        {
            Global.LocalizeText(gameObject);
        }
        public void SetRewardData(List<int> itemIds, List<int> itemCounts)
        {
            //Free version
            for (int i = 0; i < itemIds.Count; i++)
            {
                ItemIcon.NewInstance(transformRewards, itemIds[i], itemCounts[i], ItemIcon.ItemIconOption_ShowAll);
            }

            //Full version
            //StartCoroutine("CorSetRewardData", itemIds, itemCounts);
        }
        //FREE version not support Coroutine
        //IEnumerator CorSetRewardData(List<int> itemIds, List<int> itemCounts)
        //{
        //    for (int i = 0; i < itemIds.Count; i++)
        //    {
        //        ItemIcon.NewInstance(transformRewards, itemIds[i], itemCounts[i], ItemIcon.ItemIconOption.ShowAll);
        //        yield return null;
        //    }
        //}
        void OnClickClose()
        {
            Global.HidePanel("CommonRewardPanel", true);
        }
    }
}