//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;

namespace AircraftBattle
{
    /// <summary>
    /// Bullet
    /// </summary>
    public class Bullet : LikeBehaviour
    {
        [System.NonSerialized]
        public int player = 0;
        [System.NonSerialized]
        public int damage = 10;
        [System.NonSerialized]
        public float speedX = 100f;
        [System.NonSerialized]
        public float speedY = 100f;
        /// <summary>is normal state</summary>
        bool mIsNormalState = true;

        void Update(float deltaTime)
        {
            //Debug.Log("Update " + deltaTime.ToString());
            if (mIsNormalState)
            {
                //update position
                transform.localPosition = new Vector3(transform.localPosition.x + deltaTime * speedX,
                    transform.localPosition.y + deltaTime * speedY,
                    0f);
                //check whether out of view
                if (!BattleField.InView(transform.localPosition))
                {
                    Destroy();
                }
            }
        }

        public void Fire(JSONData data, Transform transformTarget)
        {
            ChangeState(true);
            speedX = data["speedX"];
            speedY = data["speedY"];
            player = data["player"];
            damage = data["damage"];
            if (transformTarget != null)//fire to target
            {
                //Recalculate speed
                float speed = Mathf.Sqrt(Mathf.Pow(speedX, 2f) + Mathf.Pow(speedY, 2f));
                float angle = Mathf.Atan2(transformTarget.localPosition.x - transform.localPosition.x, transformTarget.localPosition.y - transform.localPosition.y);
                speedX = speed* Mathf.Sin(angle);
                speedY = speed* Mathf.Cos(angle);
            }
            transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(0-speedX, speedY) * Mathf.Rad2Deg); 
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            HotUpdateBehaviour hubTarget = col.gameObject.GetComponent<HotUpdateBehaviour>();
            if (hubTarget != null)
            {
                if (hubTarget.name == "Enemy")//player' bullet hit the enemy
                {
                    Enemy enemy = hubTarget.ScriptInstance as Enemy;
                    if (enemy != null && enemy.player != player)
                    {
                        enemy.ChangeHP(damage);
                        Explode();
                    }
                }
                else if (hubTarget.name == "Player")//enemy' bullet hit the player's Aircraft
                {
                    Aircraft aircraft = hubTarget.ScriptInstance as Aircraft;
                    if (aircraft != null && aircraft.player != player)
                    {
                        aircraft.ChangeHP(damage);
                        Explode();
                    }
                }
            }
        }
        /// <summary>
        /// Change the bullet state.
        /// We are just show two different pictures here.
        /// You can override it in your sub class.
        /// </summary>
        /// <param name="bNormalState">normal state or else explode state</param>
        public virtual void ChangeState(bool bNormalState)
        {
            mIsNormalState = bNormalState;
            //gameObjects[gameObjects_NormalState].SetActive(bNormalState);
            //gameObjects[gameObjects_ExplodeState].SetActive(!bNormalState);
        }
        /// <summary>
        /// The bullet explode.
        /// You can override it in your sub class
        /// </summary>
        public virtual void Explode()
        {
            //call "Destroy" function after 0.1 seconds
            behaviour.MemberCallDelay("Destroy", 0.1f);
        }
        /// <summary>
        /// Process after destroy(push into pool)
        /// </summary>
        void Destroy()
        {
            HotUpdateManager.PushToPool(behaviour);
        }

    }
}