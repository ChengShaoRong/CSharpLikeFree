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
        public class CSL_CodeMemberFunction : CSL_Code
        {
            MethodCache cache = null;
            static Dictionary<string, object> defaultValues = new Dictionary<string, object>();
            public static CSL_Content.Value GetDefaultValue(CSL_Type type)
            {
                CSL_Content.Value value = new CSL_Content.Value();
                value.type = type;
                Type t = type;
                if (defaultValues.TryGetValue(t.FullName, out value.value))
                    return value;
                //if (t.FullName.StartsWith("System.Nullable"))
                //{
                //    var m = t.GetMethod("GetValueOrDefault", new Type[] { });
                //    object o = Activator.CreateInstance(t, new object[] { null });
                //    value.value = m.Invoke(o, null);
                //}
                //else
                //{
                    var mm = t.GetMethod("get_nullType", new Type[] { });
                    Type tt = (Type)mm.Invoke(null, null);
                    var m = tt.GetMethod("GetValueOrDefault", new Type[] { });
                    object o = Activator.CreateInstance(tt, new object[] { null });
                    value.value = m.Invoke(o, null);
                //}
                defaultValues[t.FullName] = value.value;
                return value;
            }
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Code expr_Func = codes[0];
                var parent = expr_Func.Execute(content);
                if (parent == null || parent.value == null)
                {
                    if (checkNull
                        || parent.type.FullName.StartsWith("CSharpLike.Internal.CSL_Type_Nullable")
                        /*|| parent.type.FullName.StartsWith("System.Nullable")*/)
                    {
                        content.OutStack(this);
                        if (functionName == "GetValueOrDefault")
                        {
                            if (codes.Count == 2)
                                return codes[1].Execute(content);
                            return GetDefaultValue(parent.type);
                        }
                        return CSL_Content.Value.Null;
                    }
                    throw new Exception("null object:" + expr_Func.ToString() + ":" + ToString());
                }
                var typefunction = content.vm.GetType(parent.type).function;
                if (parent.type is object)
                {
                    SInstance s = parent.value as SInstance;
                    if (s != null)
                    {
                        typefunction = s.type;
                    }
                }
                List<CSL_Content.Value> _params = new List<CSL_Content.Value>();
                for (int i = 1; i < codes.Count; i++)
                {
                    _params.Add(codes[i].Execute(content));
                }
                CSL_Content.Value value = null;
                if (cache == null || cache.cachefail)
                {
                    cache = new MethodCache();
                    value = typefunction.MemberCall(content, parent.value, functionName, _params, cache);
                }
                else
                {
                    value = typefunction.MemberCallCache(content, parent.value, _params, cache);
                }
                content.OutStack(this);
                return value;
            }

            public string functionName;
            public bool checkNull;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out functionName);
                stream.Read(out checkNull);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "MemberCall|a." + functionName;
            }
#endif
        }
    }
}