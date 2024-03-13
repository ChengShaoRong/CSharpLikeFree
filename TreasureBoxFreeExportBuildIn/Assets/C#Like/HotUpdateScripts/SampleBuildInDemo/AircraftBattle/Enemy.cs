//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
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
        public int player = 0;
        public int damage = 10;
        public string defaultBullets;
        public int hp = 0;
        public int hpMax = 0;
        public float speedX = 100f;
        public float speedY = 100f;
        public int money = 100;

        /// <summary>is normal state</summary>
        bool mIsNormalState = true;

        void Start()
        {
            gameObject.name = "Enemy";
            bullets = KissJson.ToJSONData(defaultBullets);
        }

        void Update(float deltaTime)
        {
            if (mIsNormalState)
            {
                //process move position
                transform.localPosition = new Vector3(transform.localPosition.x + deltaTime * speedX,
                    transform.localPosition.y + deltaTime * speedY,
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
                    HotUpdateManager.NewInstance("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/AircraftBattle/Bullet" + (bullet["ClassID"] - 100) + ".prefab",
                        (object obj) =>
                        {
                            (obj as Bullet).Fire(bullet, BattleField.instance.player.transform);
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
            hp = hpMax;//must reset hp here because we use pool
        }

        /// <summary>
        /// Change the enemy hp
        /// </summary>
        /// <param name="damage">how many damage</param>
        public void ChangeHP(int change)
        {
            //Debug.LogError("ChangeHp " + change);
            if (change == 0 || hp == 0)
                return;
            hp = Mathf.Clamp(hp + change, 0, hpMax);
            //Debug.LogError(gameObject.name + " hp change " + change.ToString() + " final" + oldHp.ToString() + " -> " + hp.ToString());

            if (hp == 0)
                OnDead();
        }
        /// <summary>
        /// process the dead event
        /// </summary>
        public virtual void OnDead()
        {
            Explode();
            //Create money instance
            HotUpdateManager.NewInstance("Assets/C#Like/HotUpdateResources/SampleBuildInDemo/AircraftBattle/Money0.prefab",
                (object obj) =>
                {
                    (obj as Money).money = money;
                },
                gameObject.transform.parent, transform.localPosition);
            //add score
            BattleField.instance.AddScore(hpMax / 100);
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
            BattleField.instance.RemoveEnemy(this);
        }
    }
}