//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------

using CSharpLike;
using UnityEngine;

namespace TreasureBox
{
    public class SignInCsv
    {
        public int day;
        public int itemId;
        public int itemCount;
        public int vip;


        public static SignInCsv Get(int day)
        {
            return KissCSV.Get("SignInCsv", day) as SignInCsv;
        }
        public static void AsyncLoad()
        {
            ResourceManager.LoadCsvAsync(typeof(SignInCsv), Global.GetCsvFullName("SignIn"), "day");
        }
        public static void Load()
        {
            KissCSV.Load(typeof(SignInCsv), "SignInCsv", "day", ResourceManager.LoadAsset<TextAsset>(Global.GetCsvFullName("SignIn")).text);
        }
    }
}