/*
 * KissTween : Keep It Simple Stupid Tween
 * Copyright © 2023 RongRong. All right reserved.
 */
using UnityEngine;
using UnityEngine.UI;

namespace CSharpLike
{
    public abstract class KissTweenBase : MonoBehaviour
    {
        public enum Method
        {
            Linear,
            EaseIn,
            EaseOut,
            EaseInOut,
            BounceIn,
            BounceOut,
            Spring,
        }

        public enum Style
        {
            Once,
            Loop,
            PingPong,
        }

        public delegate void OnFinished(KissTweenBase tween);

        /// <summary>
        /// Delegate for finished event.
        /// </summary>
        public OnFinished onFinished;

        [Tooltip("The tweening mehtod used")]
        public Method method = Method.Linear;

        [Tooltip("Once or Loop or PingPong")]
        public Style style = Style.Once;

        [Tooltip("Whether the tween will ignore the timescale")]
        public bool ignoreTimeScale = false;

        [Tooltip("How long will the tweener wait before starting the tween")]
        public float delay = 0f;

        [Tooltip("How long is the duration of the tween")]
        public float duration = 1f;

        [Tooltip("When the tweening finished will notify the specify GameObject. If you don't need it, keep 'callWhenFinished' or 'eventReceiver' empty.")]
        public GameObject eventReceiver;

        /// <summary>
        /// You may specify function name and params if eventReceiver is HotUpdateBehaviour component, or only specify function name.
        /// First paramater is this Tweener object.
        /// Call member method without 0~N param, you can call mothed with params in prefab or scene.
        /// If method with params, support param type string/int/uint/ulong/long/float/double/bool/char.
        /// e.g.
        /// string type : Func("your string") = Func(this, "your string");
        /// char type : Func('A') = Func(this, "A");
        /// int type : Func(123) = Func(this, 123);
        /// uint type : Func(123u) = Func(this, 123u);
        /// ulong type : Func(123UL) = Func(this, 123UL);
        /// long type : Func(123L) = Func(this, 123L);
        /// float type : Func(123.321f) = Func(this, 123.321f);
        /// double type : Func(123D) = Func(this, 123D);
        /// bool type : Func(true) = Func(this, true);
        /// multiple types : Func("str", false, 123, 321.4f) = Func(this, "str", false, 123, 321.4f);
        /// </summary>
        [Tooltip("You may specify function name and params if eventReceiver is HotUpdateBehaviour component, or only specify function name. \ne.g. Input Func(\"str\", false, 123, 321.4f) ,the eventReceiver will execute Func(this, \"str\", false, 123, 321.4f); \nAnd the first paramater alway is this Tweener object.\nIf you don't need it, keep 'callWhenFinished' or 'eventReceiver' empty.")]
        public string callWhenFinished = "OnTweener";


        /// <summary>
        /// Manually activate the tweening
        /// </summary>
        public void Play(bool forward)
        {
            mAmountPerDelta = Mathf.Abs(amountPerDelta);
            if (!forward) mAmountPerDelta = -mAmountPerDelta;
            enabled = true;
            Update();
        }

        /// <summary>
        /// Manually reset the tweener's state to the beginning.
        /// </summary>
        public void Reset()
        {
            mStarted = false;
            mFactor = (mAmountPerDelta < 0f) ? 1f : 0f;
            Sample(mFactor);
        }
        /// <summary>
        /// Manually force set the factor
        /// </summary>
        public void SetFactor(float newFactor)
        {
            mFactor = Mathf.Clamp01(newFactor);
        }
        /// <summary>
        /// Play it reverse
        /// </summary>
        public void PlayReverse(bool bReset2End = true)
        {
            if (bReset2End)
            {
                mFactor = 1f;
                mStarted = false;
                Sample(mFactor);
            }
            Play(false);
        }
        /// <summary>
        /// Play it forward
        /// </summary>
        public void PlayForward(bool bReset2Start = true)
        {
            if (bReset2Start)
            {
                mFactor = 0f;
                mStarted = false;
                Sample(mFactor);
            }
            Play(true);
        }

        /// <summary>
        /// Manually start the tweening process, reversing its direction.
        /// </summary>
        public void Toggle()
        {
            if (mFactor > 0f)
            {
                mAmountPerDelta = -amountPerDelta;
            }
            else
            {
                mAmountPerDelta = Mathf.Abs(amountPerDelta);
            }
            enabled = true;
        }

        /// <summary>
        /// Tweening logic should go here.
        /// </summary>
        protected abstract void OnUpdate(float factor);

        /// <summary>
        /// Cached transform
        /// </summary>
        protected Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }
        /// <summary>
        /// Cached Graphic component
        /// </summary>
        protected Graphic cachedGraphic { get { if (mGraphic == null) mGraphic = GetComponent<Graphic>(); return mGraphic; } }
        /// <summary>
        /// Cached Camera component
        /// </summary>
        protected Camera cachedCamera { get { if (mCamera == null) mCamera = GetComponent<Camera>(); return mCamera; } }
        /// <summary>
        /// Cached Scrollbar component
        /// </summary>
        protected Scrollbar cachedScrollbar { get { if (mScrollbar == null) mScrollbar = GetComponent<Scrollbar>(); return mScrollbar; } }
        /// <summary>
        /// Cached Text component
        /// </summary>
        protected Text cachedText { get { if (mText == null) mText = GetComponent<Text>(); return mText; } }

        #region private impl
        bool mStarted = false;
        float mStartTime = 0f;
        float mDuration = 0f;
        float mAmountPerDelta = 1f;
        float mFactor = 0f;

        protected float amountPerDelta
        {
            get
            {
                if (mDuration != duration)
                {
                    mDuration = duration;
                    mAmountPerDelta = Mathf.Abs((duration > 0f) ? 1f / duration : 1000f);
                }
                return mAmountPerDelta;
            }
        }

        void Start() { Update(); }

        /// <summary>
        /// You can specify your code while on start.
        /// </summary>
        protected virtual void OnStart() { }

        public void Update()
        {
            float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            float time = ignoreTimeScale ? Time.unscaledTime : Time.time;

            if (!mStarted)
            {
                mStarted = true;
                mStartTime = time + delay;
                OnStart();
            }

            if (time < mStartTime) return;

            mFactor += amountPerDelta * delta;

            if (style == Style.Loop)
            {
                if (mFactor > 1f)
                {
                    mFactor -= Mathf.Floor(mFactor);
                }
            }
            else if (style == Style.PingPong)
            {
                if (mFactor > 1f)
                {
                    mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
                    mAmountPerDelta = -mAmountPerDelta;
                }
                else if (mFactor < 0f)
                {
                    mFactor = -mFactor;
                    mFactor -= Mathf.Floor(mFactor);
                    mAmountPerDelta = -mAmountPerDelta;
                }
            }

            // If the factor goes out of range and this is a one-time tweening operation, disable the script
            if ((style == Style.Once) && (mFactor > 1f || mFactor < 0f))
            {
                mFactor = Mathf.Clamp01(mFactor);
                Sample(mFactor);
                enabled = false;

                // Notify the listener delegate
                if (onFinished != null) onFinished(this);

                // Notify the event listener target
                if (eventReceiver != null && !string.IsNullOrEmpty(callWhenFinished))
                {
                    HotUpdateBehaviour hotUpdateBehaviour = eventReceiver.GetComponent<HotUpdateBehaviour>();
                    if (hotUpdateBehaviour == null)//Not HotUpdateBehaviour component
                    {
                        eventReceiver.SendMessage(callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        hotUpdateBehaviour.OnTweener(this, callWhenFinished);
                    }
                }
            }
            else Sample(mFactor);
        }


        void OnDisable() { mStarted = false; }

        static float pi2 = Mathf.PI * 2f;
        /// <summary>
        /// Sample the tween at the specified factor.
        /// </summary>
        public void Sample(float factor)
        {
            float val = Mathf.Clamp01(factor);

            switch (method)
            {
                case Method.EaseIn: val = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - val)); break;
                case Method.EaseOut: val = Mathf.Sin(0.5f * Mathf.PI * val); break;
                case Method.EaseInOut: val -= Mathf.Sin(val * pi2) / pi2; break;
                case Method.BounceIn: val = BounceLogic(val); break;
                case Method.BounceOut: val = 1f - BounceLogic(1f - val); break;
                case Method.Spring: val = Mathf.Cos(0.5f * Mathf.PI * (1f - val)); break;
            }

            OnUpdate(val);
        }

        /// <summary>
        /// Main Bounce logic to simplify the Sample function
        /// </summary>
        float BounceLogic(float val)
        {
            if (val < 0.363636f) // 0.363636 = (1/ 2.75)
            {
                val = 7.5685f * val * val;
            }
            else if (val < 0.727272f) // 0.727272 = (2 / 2.75)
            {
                val = 7.5625f * (val -= 0.545454f) * val + 0.75f; // 0.545454f = (1.5 / 2.75) 
            }
            else if (val < 0.909090f) // 0.909090 = (2.5 / 2.75) 
            {
                val = 7.5625f * (val -= 0.818181f) * val + 0.9375f; // 0.818181 = (2.25 / 2.75) 
            }
            else
            {
                val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f; // 0.9545454 = (2.625 / 2.75) 
            }
            return val;
        }

        /// <summary>
        /// Manually start a tweening operation, if not exist create a new one.
        /// </summary>
        static protected T Tween<T>(GameObject go, float duration) where T : KissTweenBase
        {
            T tween = go.GetComponent<T>();
            if (tween == null)
            {
                tween = go.AddComponent<T>();
                tween.style = Style.Once;
                tween.eventReceiver = null;
                tween.callWhenFinished = null;
                tween.onFinished = null;
            }
            tween.mFactor = 0f;
            tween.mAmountPerDelta = Mathf.Abs(tween.mAmountPerDelta);
            tween.duration = duration;
            tween.mStarted = false;
            tween.enabled = true;
            if (duration <= 0f)//Finish immediately
            {
                tween.Sample(1f);
                tween.enabled = false;
            }
            return tween;
        }
        Transform mTrans;
        Graphic mGraphic;
        Camera mCamera;
        Scrollbar mScrollbar;
        Text mText;
        #endregion
    }

}