//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AircraftBattle
{
    public class BattleField : LikeBehaviour
    {
        public static BattleField instance;
        static CanvasScaler mCanvasScaler;
        void Start()
        {
            instance = this;
            mCanvasScaler = gameObject.GetComponentInParent<CanvasScaler>();
            view = Vector4.zero;
            Debug.Log("Resolution=" + mCanvasScaler.referenceResolution.ToString());
            float resolutionWidth = mCanvasScaler.referenceResolution.x;
            view.y = resolutionWidth / 2f + 150f;
            view.x -= view.y;
            view.w = resolutionWidth * Screen.height / Screen.width / 2f + 200f;
            view.z -= view.w;

            viewAircraft = Vector4.zero;
            viewAircraft.y = resolutionWidth / 2f + 50f;
            viewAircraft.x -= viewAircraft.y;
            viewAircraft.w = resolutionWidth * Screen.height / Screen.width / 2f + 90f;
            viewAircraft.z -= viewAircraft.w;
        }

        public GameObject goBattle;
        public string strMap;
        public GameObject goButtonStart;
        public int money;
        public int score;
        public Text textHP;
        public Image imageBarHP;
        public Text textMoney;
        public Text textScore;

        /// <summary>
        /// the player's aircraft
        /// </summary>
        [System.NonSerialized]
        public Aircraft player;

        /// <summary>
        /// the enemys show in the battle
        /// </summary>
        List<Enemy> enemys = new List<Enemy>();
        public void RemoveEnemy(Enemy enemy)
        {
            enemys.Remove(enemy);
        }

        /// <summary>
        /// Distance from start position
        /// </summary>
        public static float flyDistance = 0f;

        /// <summary>
        /// max distance of the battle field
        /// </summary>
        public static float maxDistance = -8000f;


        /// <summary>
        /// the view of the battle field.
        /// use for check game object inside the battle field.
        /// x:left.
        /// y:right.
        /// w:top.
        /// z:bottom
        /// </summary>
        public static Vector4 view;

        /// <summary>
        /// the view of the battle field(for aircraft).
        /// use for check game object inside the battle field.
        /// x:left.
        /// y:right.
        /// w:top.
        /// z:bottom
        /// </summary>
        public static Vector4 viewAircraft;

        /// <summary>
        /// check the position in the view
        /// </summary>
        /// <param name="pos">position</param>
        public static bool InView(Vector3 pos)
        {
            return (pos.x > view.x && pos.x < view.y && pos.y < (view.w - flyDistance) && pos.y > (view.z - flyDistance));
        }

        public static bool EnemyInView(Vector3 pos)
        {
            return (pos.y < (view.w - flyDistance) && pos.y > (view.z - flyDistance));
        }
        /// <summary>
        /// Get mouse position with scale.
        /// </summary>
        public static Vector3 GetMousePosition()
        {
            float scale = mCanvasScaler.scaleFactor;
            Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
            pos.x /= scale;
            pos.y /= scale;
            pos.x -= Screen.width / scale / 2f;
            pos.y -= Screen.height / scale / 2f;
            return pos;
        }

        /// <summary>
        /// get the aircraft position in the view
        /// </summary>
        public static Vector3 GetMousePositionInView()
        {
            Vector3 mousePos = GetMousePosition();
            mousePos.y -= (flyDistance - 50f);

            if (mousePos.x < viewAircraft.x)
                mousePos.x = viewAircraft.x;
            else if (mousePos.x > viewAircraft.y)
                mousePos.x = viewAircraft.y;

            if (mousePos.y > (viewAircraft.w - flyDistance))
                mousePos.y = (viewAircraft.w - flyDistance);
            else if (mousePos.y < (viewAircraft.z - flyDistance))
                mousePos.y = (viewAircraft.z - flyDistance);

            return mousePos;
        }

        /// <summary>
        /// the aircraft fly speed(the battle map move speed)
        /// </summary>
        float flySpeed = 0f;

        /// <summary>
        /// same with MonoBehaviour OnClick
        /// </summary>
        public void OnClickStart()
        {
            //load battle
            if (!LoadBattle(strMap))
                return;
            //hide the "Start Game" button
            goButtonStart.SetActive(false);
            //create an Aircraft
            HotUpdateManager.NewInstance("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/AircraftBattle/Aircraft0.prefab",
                (object obj) =>
                {
                    player = obj as Aircraft;
                    RefreshHP();
                },
                goBattle.transform, new Vector3(0f, -600f, 0f));
        }

        JSONData mapInfo;

        JSONData enemyInfo;
        /// <summary>
        /// Load enemys info from JSON string(or you load it from server)
        /// </summary>
        bool LoadBattle(string strJsonMap)
        {
            mapInfo = KissJson.ToJSONData(strJsonMap);
            flyDistance = 0f;
            maxDistance = mapInfo["maxDistance"];
            flySpeed = mapInfo["flySpeed"];
            enemyInfo = mapInfo["enemys"];
            activeEnemyTime = mapInfo["activeEnemyTime"];
            activeEnemyMax = mapInfo["activeEnemyMax"];
            Debug.Log("enemys.Count=" + enemyInfo.Count.ToString());
            Update(0f);
            score = 0;
            AddScore(0);
            money = 0;
            AddMoney(0);
            return true;
        }

        void ClearBattleField()
        {
            GameObject.Destroy(player.gameObject);
            player = null;
            foreach (var item in enemys)
                HotUpdateManager.PushToPool(item.behaviour);
            enemys.Clear();
            //clear money
            HotUpdateBehaviour[] moneys = goBattle.GetComponentsInChildren<HotUpdateBehaviour>();
            foreach(var item in moneys)
            {
                if (item.name == "Money")
                    item.MemberCall("PushToPool");
            }
        }

        void ActiveEnemy()
        {
            activeEnemyTime -= 0.1f;//next time faster 0.1 seconds;
            if (activeEnemyTime <= 1f)
                activeEnemyTime = 1f;
            JSONData randomEnemy = enemyInfo[Random.Range(0, enemyInfo.Count)];
            HotUpdateManager.NewInstance("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/AircraftBattle/Enemy" + (randomEnemy["ClassID"] - 200) + ".prefab",
                (object obj) =>
                {
                    enemys.Add(obj as Enemy);
                },
                goBattle.transform,
                new Vector3(Random.Range(randomEnemy["startX1"], randomEnemy["startX2"]), view.w  - flyDistance + 50f));
        }

        float mDeltaTime = 0f;
        float activeEnemyTime;
        int activeEnemyMax;
        void Update(float deltaTime)
        {
            if (flySpeed <= 0f)
                return;
            if (flyDistance > maxDistance)
            {
                flyDistance -= flySpeed * deltaTime;
                goBattle.transform.localPosition = new Vector3(0f, flyDistance, 0f);

                mDeltaTime += deltaTime;
                if (mDeltaTime > activeEnemyTime)
                {
                    mDeltaTime = 0f;
                    if (enemys.Count < activeEnemyMax)
                        ActiveEnemy();
                }
            }
            else
            {
                OnGameOver();
            }
        }

        void OnGameOver()
        {
            Debug.Log("Game Over");
            flySpeed = 0f;
            goButtonStart.SetActive(true);
            ClearBattleField();
        }

        public void RefreshHP()
        {
            //Debug.Log("RefreshHP " + hpPercent.ToString());
            if (player == null)
                return;
            float hpPercent = player.hp / (float)player.hpMax;
            textHP.text = player.hp + "/" + player.hpMax;
            imageBarHP.fillAmount = hpPercent;
            if (hpPercent <= 0f)
                OnGameOver();
        }

        public void AddMoney(int money)
        {
            //Debug.Log("RefreshMoney " + money);
            this.money += money;
            textMoney.text = "Money:" + this.money;
        }

        public void AddScore(int score)
        {
            //Debug.Log("RefreshScore " + score);
            this.score += score;
            textScore.text = "Score:" + this.score;
        }
        void OnClickBack()
        {
            HotUpdateManager.Show("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/SampleCSharpLikeHotUpdate.prefab");//back to SampleCSharpLikeHotUpdate
            HotUpdateManager.Hide("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/AircraftBattle/BattleField.prefab", true);//delete self
        }
    }
}