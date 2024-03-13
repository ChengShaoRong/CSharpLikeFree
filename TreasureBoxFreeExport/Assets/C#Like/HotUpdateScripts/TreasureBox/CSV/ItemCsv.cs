//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------

using CSharpLike;
using UnityEngine;

namespace TreasureBox
{
    public class ItemCsv
    {
        public int id;
        /// <summary>
        /// Get the localization name
        /// </summary>
        public string Name()
        {
            //get
            //{
                return Global.GetLanguage() == "EN" ? nameEN : nameCN;
            //}
        }
        public string nameEN;
        public string nameCN;
        /// <summary>
        /// Get the localization describe
        /// </summary>
        public string Desc()
        {
            //get
            //{
                return Global.GetLanguage() == "EN" ? descEN : descCN;
            //}
        }
        public string descEN;
        public string descCN;
        public string icon;
        public string frame;
        public int itemType;
        public int maxStack;
        public int sellPrice;


        public static ItemCsv Get(int id)
        {
            return KissCSV.Get("ItemCsv", id) as ItemCsv;
        }
        public static void AsyncLoad()
        {
            ResourceManager.LoadCsvAsync(typeof(ItemCsv), Global.GetCsvFullName("Item"), "id");
        }
        public static void Load()
        {
            KissCSV.Load(typeof(ItemCsv), "ItemCsv", "id", ResourceManager.LoadAsset<TextAsset>(Global.GetCsvFullName("Item")).text);
        }
    }
}