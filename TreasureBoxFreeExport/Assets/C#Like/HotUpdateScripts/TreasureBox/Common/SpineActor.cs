//--------------------------
//           C#Like
// Copyright © 2022-2024 RongRong. All right reserved.
//--------------------------
//using UnityEngine;
//using CSharpLike;
//using Spine.Unity;
//using System.Collections;
//using UnityEngine.UI;

using CSharpLike;

namespace TreasureBox
{
    public class SpineActor : LikeBehaviour
    {
        public int i = 0;
    }
}
//namespace TreasureBox
//{
//    /// <summary>
//    /// Show a spine actor, and it will random Animation in random seconds,
//    /// and will change Animation if you click the actor.
//    /// The Jump and Fail Animation just for demo, you should use Rigidbody.
//    /// </summary>
//    public class SpineActor : LikeBehaviour
//    {
//        public enum SpineActorAnimation
//        {
//            none,
//            idle,
//            attack,
//            crouch,
//            jump,
//            fall,
//            headTurn,
//            run,
//            walk,
//        }
//        SpineActorAnimation mAnimation = SpineActorAnimation.none;
//        public SpineActorAnimation ActorAnimation
//        {
//            get => mAnimation;
//            set
//            {
//                if (mAnimation != value)
//                {
//                    mAnimation = value;
//                    Spine.TrackEntry te = null; 
//                    switch (mAnimation)
//                    {
//                        case SpineActorAnimation.idle:
//                            mSkeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
//                            break;
//                        case SpineActorAnimation.attack:
//                            te = mSkeletonGraphic.AnimationState.SetAnimation(0, "attack", false);
//                            te.End += OnEnd;
//                            break;
//                        case SpineActorAnimation.crouch:
//                            te = mSkeletonGraphic.AnimationState.SetAnimation(0, "crouch", false);
//                            te.End += OnEnd;
//                            break;
//                        case SpineActorAnimation.jump:
//                            te = mSkeletonGraphic.AnimationState.SetAnimation(0, "jump", true);
//                            break;
//                        case SpineActorAnimation.fall:
//                            te = mSkeletonGraphic.AnimationState.SetAnimation(0, "fall", true);
//                            break;
//                        case SpineActorAnimation.run:
//                            mSkeletonGraphic.AnimationState.SetAnimation(0, "run", true);
//                            break;
//                        case SpineActorAnimation.walk:
//                            mSkeletonGraphic.AnimationState.SetAnimation(0, "walk", true);
//                            break;
//                        case SpineActorAnimation.headTurn:
//                            te = mSkeletonGraphic.AnimationState.SetAnimation(0, "head-turn", false);
//                            te.End += OnEnd;
//                            break;
//                    }
//                }
//            }
//        }
//        public void CancelMove()
//        {
//            if (mTweenPosition.enabled)
//            {
//                KissTweenAnchoredPosition.Tween(gameObject, 0f, mTransform.anchoredPosition);
//            }
//        }
//        public SpineActorAnimation MoveAnimation { get; set; }
//        public float MoveSpeed = 200f;
//        public Vector2 AnchoredPosition
//        {
//            get => mTransform.anchoredPosition;
//            set
//            {
//                if (mTransform.anchoredPosition != value)
//                {
//                    ActorAnimation = MoveAnimation;
//                    Vector2 current = mTransform.anchoredPosition;
//                    mTweenPosition.from = current;
//                    mTweenPosition.to = value;
//                    Flip = current.x > value.x;
//                    mTweenPosition.duration = Mathf.Abs(value.x - current.x) / MoveSpeed;
//                    mTweenPosition.PlayForward();
//                }
//            }
//        }
//        public bool Flip
//        {
//            get
//            {
//                return (mSkeletonGraphic != null && mSkeletonGraphic.Skeleton.ScaleX < 0f);
//            }
//            set
//            {
//                if (mSkeletonGraphic != null)
//                    mSkeletonGraphic.Skeleton.ScaleX = value ? -1 : 1;
//            }
//        }
//        public void SimulationJump(float jumpHeight, bool fallbackAfterJump = true, float jumpSpeed = 300f)
//        {
//            Vector2 current = (mTweenJump.transform as RectTransform).anchoredPosition;
//            ActorAnimation = SpineActorAnimation.jump;
//            Vector2 target = new Vector2(current.x, current.y + jumpHeight);
//            mTweenJump.onFinished = null;
//            mTweenJump.onFinished += (tween) =>
//            {
//                if (fallbackAfterJump)
//                {
//                    target = new Vector2(current.x, 0f);
//                    mTweenJump.from = current;
//                    mTweenJump.to = target;
//                    mTweenJump.duration = Mathf.Abs(current.y - target.y) / jumpSpeed;
//                    mTweenJump.onFinished = null;
//                    mTweenJump.onFinished += (tween2) =>
//                    {
//                        ResetToDefaultAnimation();
//                    };
//                    mTweenJump.PlayForward();
//                }
//                else
//                {
//                    ResetToDefaultAnimation();
//                }
//            };
//            mTweenJump.from = current;
//            mTweenJump.to = target;
//            mTweenJump.duration = Mathf.Abs(current.y - target.y) / jumpSpeed;
//            mTweenJump.PlayForward();
//        }
//        public void SimulationFall(float fallHeight, float fallSpeed = 300f)
//        {
//            Vector2 current = (mTweenJump.transform as RectTransform).anchoredPosition;
//            ActorAnimation = SpineActorAnimation.fall;
//            Vector2 target = new Vector2(current.x, current.y - fallHeight);
//            mTweenJump.onFinished = null;
//            mTweenJump.onFinished += (tween) =>
//            {
//                ResetToDefaultAnimation();
//            };
//            mTweenJump.from = current;
//            mTweenJump.to = target;
//            mTweenJump.duration = Mathf.Abs(current.y - target.y) / fallSpeed;
//            mTweenJump.PlayForward();
//        }

//        void OnTweenerPosition(KissTweenBase tweener)
//        {
//            if ((mTweenJump.transform as RectTransform).anchoredPosition.y <= 0.01f)
//                ActorAnimation = DefaultAnimation;
//        }

//        void ResetToDefaultAnimation()
//        {
//            if (mTweenPosition.enabled)
//                ActorAnimation = MoveAnimation;
//            else
//                ActorAnimation = DefaultAnimation;
//        }

//        public SpineActorAnimation DefaultAnimation { get; set; }
//        void OnEnd(Spine.TrackEntry trackEntry)
//        {
//            ActorAnimation = DefaultAnimation;
//        }
//        SkeletonGraphic mSkeletonGraphic;
//        RectTransform mTransform;
//        KissTweenAnchoredPosition mTweenPosition;
//        KissTweenAnchoredPosition mTweenJump;
//        void Awake()
//        {
//            mTransform = gameObject.GetComponent<RectTransform>();
//            mTweenPosition = gameObject.GetComponent<KissTweenAnchoredPosition>();
//            mTweenJump = GetComponent<KissTweenAnchoredPosition>("actor");
//            MoveAnimation = SpineActorAnimation.walk;
//        }
//        static SpineActor _NewInstance(Transform parentTrans)
//        {
//            GameObject go = ResourceManager.Instantiate<GameObject>(Global.GetPrefabFullName("SpineActor"), parentTrans);
//            if (go == null)
//            {
//                Debug.LogError("SpineActor:_NewInstance(\"SpineActor\"), Instantiate null");
//                return null;
//            }
//            HotUpdateBehaviour hub = go.GetComponent<HotUpdateBehaviour>();
//            if (hub == null)
//            {
//                Debug.LogError("SpineActor:_NewInstance(\"SpineActor\"), not exist HotUpdateBehaviour component");
//                return null;
//            }
//            SpineActor spineActor = hub.ScriptInstance as SpineActor;
//            if (spineActor == null)
//            {
//                Debug.LogError("SpineActor:_NewInstance(\"SpineActor\"), not exist SpineActor component");
//                return null;
//            }
//            go.transform.parent = parentTrans.transform;
//            go.transform.localScale = Vector3.one;
//            go.transform.localPosition = Vector3.zero;

//            return spineActor;
//        }
//        void OnLoadSpineDone(GameObject go)
//        {
//            mSkeletonGraphic = go.GetComponentInChildren<SkeletonGraphic>(true);
//            DefaultAnimation = ((mSkeletonGraphic.startingAnimation)
//            switch
//            {
//                "attack" => SpineActorAnimation.attack,
//                "crouch" => SpineActorAnimation.crouch,
//                "walk" => SpineActorAnimation.walk,
//                "run" => SpineActorAnimation.run,
//                "jump" => SpineActorAnimation.jump,
//                "fall" => SpineActorAnimation.fall,
//                "headTurn" => SpineActorAnimation.headTurn,
//                _ => SpineActorAnimation.idle
//            });
//            ActorAnimation = DefaultAnimation;
//            StartCoroutine("CorSimulate");
//            mSkeletonGraphic.materialForRendering.shader = Shader.Find(mSkeletonGraphic.materialForRendering.shader.name);
//            Button bt = go.GetComponent<Button>();
//            bt.onClick.AddListener(()=> { OnClick(); });
//        }

//        IEnumerator CorSimulate()
//        {
//            do
//            {
//                yield return new WaitForSecondsRealtime(Random.Range(5f, 10f));
//                OnClick();
//            }
//            while (true);
//        }
//        public static SpineActor NewInstance(string spineShortName, Transform parentTrans)
//        {
//            SpineActor spineActor = _NewInstance(parentTrans);
//            if (spineActor == null)
//                return null;
//            GameObject go = ResourceManager.Instantiate<GameObject>(Global.GetSpineFullName(spineShortName), spineActor.mTweenJump.transform);
//            if (go == null)
//            {
//                Debug.LogError($"SpineActor:NewInstance(\"{spineShortName}\"), return null");
//                return null;
//            }
//            spineActor.OnLoadSpineDone(go);

//            return spineActor;
//        }
//        public static void NewInstanceAsync(string spineShortName, Transform parentTrans)
//        {
//            SpineActor spineActor = _NewInstance(parentTrans);
//            if (spineActor != null)
//            {
//                var req = ResourceManager.LoadAssetAsync<GameObject>(spineShortName);
//                req.OnCompleted +=
//                    (go) =>
//                    {
//                        go = GameObject.Instantiate(go, new Vector3(0f, 0f, 0f), Quaternion.identity);
//                        if (go == null)
//                        {
//                            Debug.LogError($"SpineActor:NewInstanceAsync '{spineShortName}' error");
//                            return;
//                        }
//                        go.transform.parent = spineActor.mTweenJump.transform;
//                        go.transform.localScale = Vector3.one;
//                        go.transform.localPosition = Vector3.zero;
//                        spineActor.OnLoadSpineDone(go);
//                    };
//                req.OnError += (error) =>
//                {
//                    Debug.LogError($"SpineActor:ResourceManager.LoadAssetAsync(\"{spineShortName}\") occur error : {error}");
//                };
//            }
//        }
//        /// <summary>
//        /// Limit the actor jump/fall/move in the customize distance
//        /// </summary>
//        float limitRange = 100f;
//        void OnClick()
//        {
//            if (ActorAnimation == SpineActorAnimation.idle)
//            {
//                //Random animation
//                switch (Random.Range(0, 6))
//                {
//                    case 0://simulation Jump and Fail animation here.
//                        if ((mTweenJump.transform as RectTransform).anchoredPosition.y < 0f)//Jump back if was fall
//                        {
//                            SimulationJump(limitRange, false);
//                        }
//                        else
//                        {
//                            if (Random.Range(0, 3) == 0)//2/3 jump and 1/3 fall
//                            {
//                                SimulationFall(limitRange);
//                            }
//                            else
//                            {
//                                SimulationJump(limitRange, true);
//                            }
//                        }
//                        break;
//                    case 1:
//                        ActorAnimation = SpineActorAnimation.attack;
//                        break;
//                    case 2:
//                        ActorAnimation = SpineActorAnimation.crouch;
//                        break;
//                    case 3:
//                        ActorAnimation = SpineActorAnimation.headTurn;
//                        break;
//                    case 4:
//                        //1/2 walk, 1/2 run
//                        MoveAnimation = (Random.Range(0, 2) == 0 ? SpineActorAnimation.walk : SpineActorAnimation.run);
//                        AnchoredPosition = new Vector2(Random.Range(0f-limitRange, limitRange), 0f);
//                        break;
//                }
//            }
//        }
//    }
//}