/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeFunction : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                List<CSL_Content.Value> list = new List<CSL_Content.Value>();
                foreach (CSL_Code p in codes)
                {
                    if (p != null)
                    {
                        CSL_Content.Value value = p.Execute(content);
                        list.Add(value);
                    }
                }
                CSL_Content.Value v = null;

                SType.Function retFunc = null;
                if (content.CallType != null)
                {
                    if (content.CallThis != null && content.CallThis.parentType == typeof(LikeBehaviour))
                    {
                        switch(funcname)
                        {
                            case "behaviour":
                                v.type = typeof(HotUpdateBehaviour);
                                v.value = (content.CallThis.parentObj as LikeBehaviour).behaviour;
                                content.OutStack(this);
                                return v;
                            case "gameObject":
                                v.type = typeof(UnityEngine.GameObject);
                                v.value = (content.CallThis.parentObj as LikeBehaviour).gameObject;
                                content.OutStack(this);
                                return v;
                            case "transform":
                                v.type = typeof(UnityEngine.Transform);
                                v.value = (content.CallThis.parentObj as LikeBehaviour).transform;
                                content.OutStack(this);
                                return v;
                            case "enabled":
                                v.type = typeof(bool);
                                v.value = (content.CallThis.parentObj as LikeBehaviour).enabled;
                                content.OutStack(this);
                                return v;
                            case "isActiveAndEnabled":
                                v.type = typeof(bool);
                                v.value = (content.CallThis.parentObj as LikeBehaviour).isActiveAndEnabled;
                                content.OutStack(this);
                                return v;
                            case "GetInt":
                            case "SetInt":
                            case "GetSprite":
                            case "SetSprite":
                            case "GetBoolean":
                            case "SetBoolean":
                            case "GetDouble":
                            case "SetDouble":
                            case "GetFloat":
                            case "SetFloat":
                            case "GetColor":
                            case "SetColor":
                            case "GetVector3":
                            case "SetVector3":
                            case "GetString":
                            case "SetString":
                            case "GetGameObject":
                            case "SetGameObject":
                            case "GetTextAsset":
                            case "SetTextAsset":
                            case "GetMaterial":
                            case "SetMaterial":
                            case "GetTexture":
                            case "SetTexture":
                            case "GetAudioClip":
                            case "SetAudioClip":
                            case "StartCoroutine":
                            case "StopCoroutine":
                            case "StopAllCoroutines":
                            case "MemberCall":
                            case "MemberCallDelay":
                                v = content.CallThis.parentFunc.MemberCall(content, content.CallThis.parentObj, funcname, list);
                                content.OutStack(this);
                                return v;
                            default:
                                if (funcname.StartsWith("GetComponent<"))
                                {
                                    v = content.CallThis.parentFunc.MemberCall(content, content.CallThis.parentObj, funcname, list);
                                    content.OutStack(this);
                                    return v;
                                }
                                break;
                        }
                    }
                    retFunc = content.CallType.GetFunction(funcname);
                }

                if (retFunc != null)
                {
                    if (retFunc.bStatic)
                    {
                        v = content.CallType.StaticCall(content, funcname, list);
                    }
                    else
                    {
                        v = content.CallType.MemberCall(content, content.CallThis, funcname, list);
                    }
                }


                else
                {
                    v = content.GetQuiet(funcname);
                    if (v != null && v.value is Delegate)
                    {
                        Delegate d = v.value as Delegate;
                        v = new CSL_Content.Value();
                        object[] obja = new object[list.Count];
                        for (int i = 0; i < list.Count; i++)
                        {
                            obja[i] = list[i].value;
                        }
                        v.value = d.DynamicInvoke(obja);
                        if (v.value == null)
                        {
                            v.type = null;
                        }
                        else
                        {
                            v.type = v.value.GetType();
                        }
                    }
                    else
                    {
                        if (content.CallThis != null && funcname == "GetType" && list.Count == 0)
                            v = content.CallType.MemberCall(content, content.CallThis, funcname, list);
                    }
                }
                content.OutStack(this);
                return v;
            }
            string funcname;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out funcname);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "Call|" + funcname + "(params[" + codes.Count + ")";
            }
#endif
        }
    }
}