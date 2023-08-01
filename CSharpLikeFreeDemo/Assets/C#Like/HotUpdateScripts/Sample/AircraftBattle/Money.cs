//--------------------------
//           C#Like
// Copyright © 2022-2023 RongRong. All right reserved.
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
        void Start()
        {
            gameObject.name = "Money";
        }
        void OnEnable()
        {
            GetBoolean("active", true);
            flyTime = 0.2f;
        }

        Vector3 currentVelocity = Vector3.zero;
        Transform transformTo = null;
        public void OnCollect(Transform to)
        {
            GetBoolean("active", false);
            transformTo = to;
        }
        float flyTime;
        void Update()
        {
            if (transformTo != null && BattleField.instance.player != null)
            {
                SampleHowToUseModifier.currentVelocity = currentVelocity;
                transform.localPosition = SampleHowToUseModifier.SmoothDamp(transform.localPosition,
                        transformTo.localPosition, flyTime);
                currentVelocity = SampleHowToUseModifier.currentVelocity;
                if (Vector3.Distance(transform.localPosition, transformTo.localPosition) < 10f)
                {
                    BattleField.instance.AddMoney(GetInt("money"));//same with 'HotUpdateManager.getHotUpdate("BattleField").MemberCall("AddMoney", GetInt("money"));'
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