/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CSharpLike
{
    namespace Internal
    {
        class CSL_Type_JSONData : CSL_TypeBase
        {
            public CSL_Type_JSONData()
            {
                function = new CSL_RegHelper_TypeFunction(typeof(JSONData));
            }
            public override string Keyword
            {
                get { return "JSONData"; }
            }
            public override string Namespace
            {
                get { return ""; }
            }
            public override string FullName
            {
                get { return "JSONData"; }
            }
            public override CSL_Type type
            {
                get { return typeof(JSONData); }
            }

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                Type targettype = targetType;
                if (targettype == type)
                    return src;
                Type selftype = type;
                if (caches.Count == 0)
                {
                    var mis = selftype.GetMethods(); 
                    foreach (var one in mis)
                    {
                        if (one.Name == "op_Implicit")
                        {
                            var ps = one.GetParameters();
                            if (ps != null && ps.Length == 1 && ps[0].ParameterType == selftype)
                                caches[content.vm.GetType(one.ReturnType).type] = one;
                        }
                    }
                }
                if (caches.TryGetValue(targetType, out MethodInfo mi))
                {
                    if (mi != null)
                        return mi.Invoke(null, new object[] { src });
                }
                if (((Type)targetType).IsAssignableFrom(typeof(JSONData)))
                {
                    return src;
                }
                if (targettype != null)
                {
                    if (!caches2.TryGetValue(targetType, out mi))
                    {
                        mi = targettype.GetMethod("op_Implicit", new Type[] { selftype });
                        if (mi == null)
                            mi = targettype.GetMethod("op_Explicit", new Type[] { selftype });
                        caches[targetType] = mi;
                    }
                    if (mi != null)
                        return mi.Invoke(null, new object[] { src });
                }
                return src;
            }
            static Dictionary<CSL_Type, MethodInfo> caches = new Dictionary<CSL_Type, MethodInfo>();
            static Dictionary<CSL_Type, MethodInfo> caches2 = new Dictionary<CSL_Type, MethodInfo>();

            public override object Math2Value(CSL_Content content, string code, object left, CSL_Content.Value right, out CSL_Type returntype)
            {
                bool math2ValueSuccess;
                object value = CSL_NumericTypeUtils.Math2Value<JSONData>(code, left, right, out returntype, out math2ValueSuccess);
                if (math2ValueSuccess)
                {
                    return value;
                }

                return base.Math2Value(content, code, left, right, out returntype);
            }

            public override bool MathLogic(CSL_Content content, TokenLogic code, object left, CSL_Content.Value right)
            {
                bool mathLogicSuccess;
                bool value = CSL_NumericTypeUtils.MathLogic<JSONData>(code, left, right, out mathLogicSuccess);
                if (mathLogicSuccess)
                {
                    return value;
                }

                return base.MathLogic(content, code, left, right);
            }
            public override CSL_TypeFunctionBase function
            {
                get;
                protected set;
            }
            public override object DefValue
            {
                get { return null; }
            }
        }
    }
}