//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;

namespace AircraftBattle
{
    /// <summary>
    /// Aircraft
    /// </summary>
    public class Aircraft : LikeBehaviour
    {
        /// <summary>
        /// same with MonoBehaviour Start
        /// </summary>
        void Start()
        {
            gameObject.name = "Player";
            SetInt("player", 1);
            bullets = KissJson.ToJSONData(GetString("defaultBullets"));
        }
        void OnEnable()
        {
            SetInt("hp", GetInt("hpMax"));
        }

        JSONData bullets;

        /// <summary>
        /// fire the bullets
        /// </summary>
        public void FireBullet(float deltaTime)
        {
            for(int i=0; i< bullets.Count; i++)
            {
                JSONData bullet = bullets[i];
                float time = bullet["time"] + deltaTime;
                if (time >= bullet["rate"])
                {
                    bullet["time"] = time - bullet["rate"];
                    HotUpdateManager.NewInstance("Assets/C#Like/Sample/AircraftBattle/Bullet" + (bullet["ClassID"] - 100) + ".prefab",
                        (object obj) =>
                        {
                            (obj as Bullet).Fire(bullet, null);
                        },
                        transform.parent,
                        new Vector3(bullet["startX"] + transform.localPosition.x, bullet["startY"] + transform.localPosition.y));
                }
                else
                    bullet["time"] = time;
            }
        }

        public void Update(float deltaTime)
        {
            //Debug.Log("Update " + deltaTime.ToString());
            if (mStart)
            {
                //follow the touch position
                FollowTouch();
                //fire bullet
                FireBullet(deltaTime);
            }
        }

        /// <summary>
        /// whether game started
        /// </summary>
        public bool mStart = true;

        public void ChangeState(bool value)
        {
            mStart = value;
        }
        Vector3 currentVelocity = Vector3.zero;
        public void FollowTouch()
        {
            SampleHowToUseModifier.currentVelocity = currentVelocity;
            transform.localPosition = SampleHowToUseModifier.SmoothDamp(transform.localPosition,
                BattleField.GetMousePositionInView(), 0.15f);
            currentVelocity = SampleHowToUseModifier.currentVelocity;
        }

        /// <summary>
        /// Change the aircraft hp
        /// </summary>
        /// <param name="damage">how many damage</param>
        public void ChangeHP(int change)
        {
            if (change == 0)
                return;
            //int oldHp = GetInt("hp");
            int hp = Mathf.Clamp(GetInt("hp") + change, 0, GetInt("hpMax"));
            SetInt("hp", hp);
            //Debug.LogError(gameObject.name + " hp change " + change.ToString() + " final" + oldHp.ToString() + " -> " + hp.ToString());
            //refresh the hp interface,and will game over while hp = 0 in BattleField
            BattleField.instance.RefreshHP();
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            //Debug.LogError(col.gameObject.name + " OnTriggerEnter2D " + gameObject.name);
            HotUpdateBehaviour hubTarget = col.gameObject.GetComponent<HotUpdateBehaviour>();
            if (hubTarget != null)
            {
                if (hubTarget.name == "Enemy")//hit the Enemy
                    ChangeHP(hubTarget.GetInt("damage"));
                else if (hubTarget.name == "Money"
                    && hubTarget.GetBoolean("active", true))//hit the Money
                {
                    SetInt("money", GetInt("money") + hubTarget.GetInt("money"));
                    //push the money to the pool
                    hubTarget.MemberCall("OnCollect", transform);
                }
            }
        }
    }
}