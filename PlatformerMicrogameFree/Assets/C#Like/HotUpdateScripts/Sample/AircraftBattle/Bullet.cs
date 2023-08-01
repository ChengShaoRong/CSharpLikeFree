//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
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
        /// <summary>is normal state</summary>
        bool mIsNormalState = true;

        void Update(float deltaTime)
        {
            //Debug.Log("Update " + deltaTime.ToString());
            if (mIsNormalState)
            {
                //update position
                transform.localPosition = new Vector3(transform.localPosition.x + deltaTime * GetFloat("speedX"),
                    transform.localPosition.y + deltaTime * GetFloat("speedY"),
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
            SetFloat("speedX", data["speedX"]);
            SetFloat("speedY", data["speedY"]);
            SetInt("player", data["player"]);
            SetInt("damage", data["damage"]);
            if (transformTarget != null)//fire to target
            {
                //Recalculate speed
                float speed = Mathf.Sqrt(Mathf.Pow(GetFloat("speedX"), 2f) + Mathf.Pow(GetFloat("speedY"), 2f));
                float angle = Mathf.Atan2(transformTarget.localPosition.x - transform.localPosition.x, transformTarget.localPosition.y - transform.localPosition.y);
                SetFloat("speedX", speed * Mathf.Sin(angle));
                SetFloat("speedY", speed * Mathf.Cos(angle));
            }
            transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(0f-GetFloat("speedX"), GetFloat("speedY")) * Mathf.Rad2Deg); 
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            HotUpdateBehaviour hubTarget = col.gameObject.GetComponent<HotUpdateBehaviour>();
            if (hubTarget != null
                && hubTarget.GetInt("player") != GetInt("player"))// player vs enemy.The player's bullet and aircraft was set 'player' value '1'
            {
                if (hubTarget.name == "Enemy")//player' bullet hit the enemy
                {
                    hubTarget.MemberCall("ChangeHP", GetInt("damage"));
                    Explode();
                }
                else if (hubTarget.name == "Player")//enemy' bullet hit the player's Aircraft
                {
                    hubTarget.MemberCall("ChangeHP", GetInt("damage"));
                    Explode();
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