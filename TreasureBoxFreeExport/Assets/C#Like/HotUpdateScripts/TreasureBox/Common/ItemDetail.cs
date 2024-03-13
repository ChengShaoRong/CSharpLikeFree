//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike;
using TMPro;
using UnityEngine;

namespace TreasureBox
{
    /// <summary>
    /// Show item detail when click ItemIcon.
    /// </summary>
	public class ItemDetail : LikeBehaviour
    {
        /// <summary>
        /// Item instance.
        /// </summary>
        Item item = null;
        public static ItemDetail NewInstance(Item item)
        {
            GameObject goRoot = Global.GetMainRoot();
            if (goRoot == null)
                return null;
            GameObject go = ResourceManager.Instantiate<GameObject>(Global.GetPrefabFullName("ItemDetail"), goRoot.transform);
            if (go == null)
                return null;
            ItemDetail itemDetail = go.GetComponent<HotUpdateBehaviour>().ScriptInstance as ItemDetail;
            itemDetail.item = item;
            itemDetail.RefreshUI();
            RectTransform pos = go.GetComponent<RectTransform>();
            float x = 0f;
            float y = 0f;
            //Show it next to the ItemIcon

            pos.anchoredPosition = new Vector2(x, y);
            return itemDetail;
        }

        public static Vector3 Local2Local(GameObject from, GameObject to)
        {
            Vector3 pos = from.transform.parent.TransformPoint(from.transform.localPosition);
            pos = to.transform.parent.InverseTransformPoint(pos);
            return pos;
        }
        [SerializeField]
        KissImage imageFrame;
        [SerializeField]
        KissImage imageIcon;
        [SerializeField]
        KissImage imageItemType;
        [SerializeField]
        TMP_Text textItemTypeText;
        [SerializeField]
        TMP_Text textName;
        [SerializeField]
        TMP_Text textCount;
        [SerializeField]
        TMP_Text textDesc;
        public void RefreshUI()
        {
            //Frame
            imageFrame.SpriteName = item.GetCsv().frame;
            //Icon
            imageIcon.SpriteName = item.GetCsv().icon;
            //ItemType
            imageItemType.SpriteName = "itemType" + item.GetCsv().itemType;
            textItemTypeText.text = Global.GetString("LT_ItemDetail_ItemType" + item.GetCsv().itemType);
            //Name
            textName.text = item.GetCsv().Name();
            //Count
            int count = 0;
            Item itemSelf = MySocket.account.GetItem(item.itemId);
            if (itemSelf != null)
                count = itemSelf.count;
            textCount.text = string.Format(Global.GetString("LT_ItemDetail_Count"), count);
            //Desc
            textDesc.text = item.GetCsv().Desc();
        }
    }
}

