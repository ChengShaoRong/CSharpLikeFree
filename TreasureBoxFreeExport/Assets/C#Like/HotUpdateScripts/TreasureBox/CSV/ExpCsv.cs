//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------

using CSharpLike;
using UnityEngine;

namespace TreasureBox
{
    public class ExpCsv
    {
        public int lv;
        public int type;
        public int exp;

        public static int GetExp(int lv, int type)
        {
            ExpCsv csv = Get(lv, type);
            if (csv != null)
                return csv.exp;
            return 99999999;
        }
        public static ExpCsv Get(int lv, int type)
        {
            return KissCSV.Get("ExpCsv", lv.ToString(), type.ToString()) as ExpCsv;
        }
        public static void AsyncLoad()
        {
            ResourceManager.LoadCsvAsync(typeof(ExpCsv), Global.GetCsvFullName("Exp"), "lv", 
                (error) =>
                {
                    Debug.Log("Load Exp done");
                }, "type");
        }
        public static void Load()
        {
            KissCSV.Load(typeof(ExpCsv), "ExpCsv", "lv", ResourceManager.LoadAsset<TextAsset>(Global.GetCsvFullName("Exp")).text, "type");
        }
    }
}