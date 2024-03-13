//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using CSharpLike;
using UnityEngine;

namespace TreasureBox
{
    public class CharacterCsv
    {
        //Free version not support enum
        ///// <summary>
        ///// 角色类型
        ///// </summary>
        //public enum CharacterType
        //{
        //    /// <summary>
        //    /// 战士
        //    /// </summary>
        //    Warrior,
        //    /// <summary>
        //    /// 法师
        //    /// </summary>
        //    Mage,
        //    /// <summary>
        //    /// 刺客
        //    /// </summary>
        //    Rogue,
        //    /// <summary>
        //    /// 牧师
        //    /// </summary>
        //    Prisst,
        //}
        public int id;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name()
        {
            //get
            //{
                return Global.GetLanguage() == "EN" ? nameEN : nameCN;
            //}
        }
        /// <summary>
        /// EN名称
        /// </summary>
        public string nameEN;
        /// <summary>
        /// CN名称
        /// </summary>
        public string nameCN;
        /// <summary>
        /// 图标id
        /// </summary>
        public string icon;
        /// <summary>
        /// 角色类型
        /// </summary>
        public int Type;
        //public CharacterType Type;

        /// <summary>
        /// Physical Attack 物理攻击力
        /// </summary>
        public int ATK;
        /// <summary>
        /// Physical defense 物理防御
        /// </summary>
        public int DEF;
        /// <summary>
        /// Magic attack 魔法攻击力
        /// </summary>
        public int MAG;
        /// <summary>
        /// Magic defence 魔法防御
        /// </summary>
        public int MDEF;
        /// <summary>
        /// Speed 速度
        /// </summary>
        public int SPD;
        /// <summary>
        /// Health Point 生命
        /// </summary>
        public int HP;
        /// <summary>
        /// Hit 命中
        /// </summary>
        public int HIT;
        /// <summary>
        /// Evasion 闪避
        /// </summary>
        public int EVD;
        /// <summary>
        /// Critical Rate 暴击率
        /// </summary>
        public int CRT;
        /// <summary>
        /// Critical Damage 暴击倍数
        /// </summary>
        public int CRTD;

        public static CharacterCsv Get(int id)
        {
            return KissCSV.Get("CharacterCsv", id) as CharacterCsv;
        }
        public static void AsyncLoad()
        {
            ResourceManager.LoadCsvAsync(typeof(CharacterCsv), Global.GetCsvFullName("Character"), "id");
        }
        public static void Load()
        {
            KissCSV.Load(typeof(CharacterCsv), "CharacterCsv", "id", ResourceManager.LoadAsset<TextAsset>(Global.GetCsvFullName("Character")).text);
        }
    }
}