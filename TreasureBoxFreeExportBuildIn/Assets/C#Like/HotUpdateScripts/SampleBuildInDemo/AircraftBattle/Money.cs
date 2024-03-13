//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
using UnityEngine;
using CSharpLike;

namespace AircraftBattle
{
    /// <summary>
    /// Money
    /// </summary>
    public class Money : LikeBehaviour
    {
        public bool IsActive = true;
        public int money = 10;
        void Start()
        {
            gameObject.name = "Money";
        }
        void OnEnable()
        {
            IsActive = true;
            flyTime = 0.2f;
        }

        Vector3 currentVelocity = Vector3.zero;
        Transform transformTo = null;
        public void OnCollect(Transform to)
        {
            IsActive = false;
            transformTo = to;
        }
        float flyTime;
        void Update()
        {
            if (transformTo != null && BattleField.instance.player != null)
            {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition,
                        transformTo.localPosition, ref currentVelocity, flyTime);
                if (Vector3.Distance(transform.localPosition, transformTo.localPosition) < 10f)
                {
                    BattleField.instance.AddMoney(money);//same with 'HotUpdateManager.getHotUpdate("BattleField").MemberCall("AddMoney", GetInt("money"));'
                    PushToPool();
                }
                //make move faster next time
                flyTime -= Time.deltaTime;
                if (flyTime < 0f)
                    flyTime = 0f;
            }
        }

        void PushToPool()
        {
            transformTo = null;
            HotUpdateManager.PushToPool(behaviour);
        }
    }
}