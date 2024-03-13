//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TreasureBox
{
    /// <summary>
    /// Item icon for common.
    /// </summary>
	public class ItemIcon : LikeBehaviour
    {
        public static int ItemIconOption_ShowCount = 1;
        public static int ItemIconOption_ShowName = 2;
        public static int ItemIconOption_ShowDetailOnPress = 4;
        public static int ItemIconOption_ShowAll = 7;
        public static int ItemIconOption_Default = 5;
        //Not support enum in FREE version
        //public enum ItemIconOption
        //{
        //    ShowCount = 1,
        //    ShowName = 2,
        //    ShowDetailOnPress = 4,

        //    ShowAll = ShowCount | ShowName | ShowDetailOnPress,
        //    Default = ShowCount | ShowDetailOnPress,
        //}
        /// <summary>
        /// Item instance.
        /// </summary>
        Item item = null;
        /// <summary>
        /// Whether show ItemDetail when OnClick event.
        /// </summary>
        public int option = 5;// ItemIconOption_Default;
        public static ItemIcon NewInstance(Transform transformParent, int itemId, int itemCount, int option)
        {
            //New a dump Item instance
            Item item = new Item();
            item.itemId = itemId;
            item.count = itemCount;
            return NewInstanceByItem(transformParent, item, option);
        }
        public static ItemIcon NewInstanceByItem(Transform transformParent, Item item, int _option)
        {
            GameObject go = ResourceManager.Instantiate<GameObject>(Global.GetPrefabFullName("ItemIcon"), transformParent);
            if (go == null)
                return null;
            ItemIcon itemIcon = go.GetComponent<HotUpdateBehaviour>().ScriptInstance as ItemIcon;
            itemIcon.item = item;
            itemIcon.option = _option;
            return itemIcon;
        }
        [SerializeField]
        KissImage imageFrame;
        [SerializeField]
        KissImage imageIcon;
        [SerializeField]
        TMP_Text textCount;
        [SerializeField]
        TMP_Text textName;
        void Start()
        {
            RefreshUI();
        }
        public void RefreshUI()
        {
            //Frame
            imageFrame.SpriteName = item.GetCsv().frame;
            //Icon
            imageIcon.SpriteName = item.GetCsv().icon;
            //Count
            textCount.gameObject.SetActive(option == ItemIconOption_ShowCount
                || option == ItemIconOption_ShowAll
                || option == ItemIconOption_Default);
            //FREE version not support bit operation
            //textCount.gameObject.SetActive((int)(option & ItemIconOption.ShowCount) > 0);
            textCount.text = item.count + "";
            //Name
            textName.gameObject.SetActive(option == ItemIconOption_ShowName
                || option == ItemIconOption_ShowAll);
            //textName.gameObject.SetActive((int)(option & ItemIconOption.ShowName) > 0);
            textName.text = item.GetCsv().Name();
        }
        public void SetEventOnClick(HotUpdateBehaviour eventReceivier, string funcName)
        {
            mEventReceivier = eventReceivier;
            mOnClick = funcName;

        }
        HotUpdateBehaviour mEventReceivier;
        string mOnClick = "";
        void OnClick()
        {
            Debug.Log("ItemIcon OnClick");
            if (mEventReceivier != null)
                mEventReceivier.MemberCall(mOnClick);
        }
        void OnPointerDown(BaseEventData eventData)
        {
            if (option == ItemIconOption_ShowCount
                || option == ItemIconOption_ShowAll
                || option == ItemIconOption_Default)
            //if ((int)(option & ItemIconOption.ShowDetailOnPress) > 0)
            {
                ItemDetail.NewInstance(item);
            }
        }
    }
}

