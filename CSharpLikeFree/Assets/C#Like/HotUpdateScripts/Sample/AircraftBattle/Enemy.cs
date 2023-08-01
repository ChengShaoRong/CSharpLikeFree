//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;

namespace AircraftBattle
{
    /// <summary>
    /// Enemy
    /// </summary>
    public class Enemy : LikeBehaviour
    {
        /// <summary>is normal state</summary>
        bool mIsNormalState = true;

        void Start()
        {
            gameObject.name = "Enemy";
            bullets = KissJson.ToJSONData(GetString("defaultBullets"));
        }

        void Update(float deltaTime)
        {
            if (mIsNormalState)
            {
                //process move position
                transform.localPosition = new Vector3(transform.localPosition.x + deltaTime * GetInt("speedX"),
                    transform.localPosition.y + deltaTime * GetInt("speedY"),
                    0f);
                //check in battle view
                if (transform.localPosition.y < (BattleField.view.z - BattleField.flyDistance - 50f))
                {
                    mIsNormalState = false; 
                    behaviour.MemberCallDelay("Destroy", 0.1f);
                    return;
                }

                //process fire bullet
                FireBullet(deltaTime);
            }
        }

        JSONData bullets;

        /// <summary>
        /// fire the bullets
        /// </summary>
        public void FireBullet(float deltaTime)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                JSONData bullet = bullets[i];
                float time = bullet["time"] + deltaTime;
                if (time >= bullet["rate"])
                {
                    bullet["time"] = time - bullet["rate"];
                    HotUpdateManager.NewInstance("Assets/C#Like/Sample/AircraftBattle/Bullet" + (bullet["ClassID"] - 100) + ".prefab",
                        (HotUpdateBehaviour _hub) =>
                        {
                            _hub.MemberCall("Fire",
                            bullet,
                            BattleField.instance.player.transform);
                        },
                        transform.parent,
                        new Vector3(bullet["startX"] + transform.localPosition.x, bullet["startY"] + transform.localPosition.y));
                }
                else
                    bullet["time"] = time;
            }
        }

        void OnEnable()
        {
            ChangeState(true);
            SetInt("hp", GetInt("hpMax"));//must reset hp here because we use pool
        }

        /// <summary>
        /// Change the enemy hp
        /// </summary>
        /// <param name="damage">how many damage</param>
        public void ChangeHP(int change)
        {
            //Debug.LogError("ChangeHp " + change);
            if (change == 0 || GetInt("hp") == 0)
                return;
            //int oldHp = GetInt("hp");
            //int hp = Mathf.Clamp(GetInt("hp") + change, 0, GetInt("hpMax"));
            SetInt("hp", Mathf.Clamp(GetInt("hp") + change, 0, GetInt("hpMax")));
            //Debug.LogError(gameObject.name + " hp change " + change.ToString() + " final" + oldHp.ToString() + " -> " + hp.ToString());

            if (GetInt("hp") == 0)
                OnDead();
        }
        /// <summary>
        /// process the dead event
        /// </summary>
        public virtual void OnDead()
        {
            Explode();
            //Create money instance
            HotUpdateManager.NewInstance("Assets/C#Like/Sample/AircraftBattle/Money0.prefab",
                (HotUpdateBehaviour _hub) =>
                {
                    _hub.SetInt("money", GetInt("money"));
                },
                gameObject.transform.parent, transform.localPosition);
            //add score
            BattleField.instance.AddScore(GetInt("hpMax") / 100);
        }

        /// <summary>
        /// Change the bullet state.
        /// </summary>
        /// <param name="bNormalState">normal state or else explode state</param>
        public void ChangeState(bool bNormalState)
        {
            mIsNormalState = bNormalState;
        }
        /// <summary>
        /// The bullet explode.
        /// </summary>
        public void Explode()
        {
            //change to explode state
            ChangeState(false);
            //call "Destroy" function after 0.1 seconds
            behaviour.MemberCallDelay("Destroy", 0.1f);
        }
        /// <summary>
        /// Process after destroy(push into pool)
        /// </summary>
        public void Destroy()
        {
            HotUpdateManager.PushToPool(behaviour);
            BattleField.instance.RemoveEnemy(behaviour);
        }
    }
}