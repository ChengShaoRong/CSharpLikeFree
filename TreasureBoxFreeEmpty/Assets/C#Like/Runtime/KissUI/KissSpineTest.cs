///*
// *           C#Like
// * Copyright © 2022-2024 RongRong. All right reserved.
// */
//using Spine;
//using Spine.Unity;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.U2D;
//using UnityEngine.UI;
//using Spine.Unity.AttachmentTools;

//namespace CSharpLike
//{
//	public class KissSpineTest : MonoBehaviour
//	{
//        SkeletonGraphic skeletonGraphic;
//        private void Start()
//        {
//            skeletonGraphic = GetComponent<SkeletonGraphic>();
//        }
//        float time = 0f;
//        private void Update()
//        {
//            time += Time.deltaTime;

//            if (time> 5f)
//            {
//                time = 0f;
//                switch (Random.Range(0, 3))
//                {
//                    case 0:
//                        skeletonGraphic.AnimationState.SetAnimation(0, "idle", true);
//                        break;
//                    case 1:
//                        skeletonGraphic.AnimationState.SetAnimation(0, "attack", true);
//                        break;
//                    case 2:
//                        skeletonGraphic.AnimationState.SetAnimation(0, "run", true);
//                        break;
//                }
//            }
//        }
//    }
//}

