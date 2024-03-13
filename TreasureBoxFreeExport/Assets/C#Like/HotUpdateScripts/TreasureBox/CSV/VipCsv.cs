//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------

using CSharpLike;
using System.Collections.Generic;
using UnityEngine;

namespace TreasureBox
{
    public class VipCsv
    {
        public int vip;
        public int exp;
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

        static SortedDictionary<int, int> mVipExp = new SortedDictionary<int, int>();
        public static int GetVip(int exp)
        {
            if (exp == 0)
                return 0;
            int vip = 0;
            foreach(var one in mVipExp)
            {
                if (exp < one.Value)
                    return vip;
                vip = one.Key;
            }
            return vip;
        }
        public static VipCsv Get(int vip)
        {
            return KissCSV.Get("VipCsv", vip) as VipCsv;
        }
        public static void AsyncLoad()
        {
            ResourceManager.LoadCsvAsync(typeof(VipCsv), Global.GetCsvFullName("Vip"), "vip",
                (error) =>
                {
                    InitDic();
                });
        }
        public static void Load()
        {
            KissCSV.Load(typeof(VipCsv), "VipCsv", "vip", ResourceManager.LoadAsset<TextAsset>(Global.GetCsvFullName("Vip")).text);
            InitDic();
        }
        static void InitDic()
        {
            mVipExp = new SortedDictionary<int, int>();
            foreach (var one in KissCSV.GetData("VipCsv").Values)
            {
                VipCsv csv = one as VipCsv;
                mVipExp[csv.vip] = csv.exp;
            }
        }
    }
}