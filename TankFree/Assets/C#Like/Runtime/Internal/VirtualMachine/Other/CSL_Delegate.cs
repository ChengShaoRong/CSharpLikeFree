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
        public class CSL_DeleEvent
        {
            public CSL_DeleEvent(object source, System.Reflection.EventInfo _event)
            {
                this.source = source;
                this._event = _event;
            }

            public object source;
            public System.Reflection.EventInfo _event;
        }

        public class CSL_DeleFunction
        {
            public CSL_DeleFunction(SType stype, SInstance _this, string function)
            {
                calltype = stype;
                callthis = _this;
                functionName = function;
            }

            public SType calltype;
            public SInstance callthis;
            public string functionName;
            public Delegate CacheFunction(Type deletype, Delegate dele)
            {
                if (dele == null)
                {
                    Delegate v = null;
                    Dictionary<Type, Delegate> caches;
                    if (callthis != null)
                    {
                        if (callthis.deles.TryGetValue(functionName, out caches))
                        {
                            caches.TryGetValue(deletype, out v);
                        }
                    }
                    else
                    {
                        if (calltype.deles.TryGetValue(functionName, out caches))
                        {
                            caches.TryGetValue(deletype, out v);
                        }
                    }
                    return v;
                }
                else
                {
                    Dictionary<Type, Delegate> caches;
                    if (callthis != null)
                    {
                        if (!callthis.deles.TryGetValue(functionName, out caches))
                        {
                            caches = new Dictionary<Type, Delegate>();
                            callthis.deles[functionName] = caches;
                        }

                        caches[deletype] = dele;
                    }
                    else
                    {
                        if (!calltype.deles.TryGetValue(functionName, out caches))
                        {
                            caches = new Dictionary<Type, Delegate>();
                            calltype.deles[functionName] = caches;
                        }

                        caches[deletype] = dele;

                    }
                    return dele;
                }

            }
        }

        public class CSL_DeleLambda
        {
            public CSL_DeleLambda(CSL_Content content, IList<CSL_Code> param, CSL_Code code)
            {
                this.content = content.Clone();
                this.code = code;
                foreach (var p in param)
                {
                    CSL_CodeGetValue v1 = p as CSL_CodeGetValue;
                    CSL_CodeDefine v2 = p as CSL_CodeDefine;
                    if (v1 != null)
                    {
                        paramTypes.Add(null);
                        paramNames.Add(v1.value_name);
                    }
                    else if (v2 != null)
                    {
                        paramTypes.Add(v2.value_type);
                        paramNames.Add(v2.value_names[0]);
                    }
                    else
                    {
                        throw new Exception("DeleLambda param invalid");
                    }
                }
            }

            public List<Type> paramTypes = new List<Type>();
            public List<string> paramNames = new List<string>();
            public CSL_Content content;
            public CSL_Code code;
        }
    }
}