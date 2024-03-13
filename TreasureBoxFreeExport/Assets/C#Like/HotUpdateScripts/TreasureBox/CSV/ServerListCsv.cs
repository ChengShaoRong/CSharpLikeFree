//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------

using CSharpLike;
using UnityEngine;

namespace TreasureBox
{
    public class ServerListCsv
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
        public string webSocket;
        public string socketIp;
        public int socketPort;


        public static ServerListCsv Get(int id)
        {
            return KissCSV.Get("ServerListCsv", id) as ServerListCsv;
        }
        public static void AsyncLoad()
        {
            ResourceManager.LoadCsvAsync(typeof(ServerListCsv), Global.GetCsvFullName("ServerList"), "lv");
        }
        public static void Load()
        {
            KissCSV.Load(typeof(ServerListCsv), "ServerListCsv", "id", ResourceManager.LoadAsset<TextAsset>(Global.GetCsvFullName("ServerList")).text);
        }
    }
}