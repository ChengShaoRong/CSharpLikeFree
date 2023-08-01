/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System.Collections.Generic;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeStaticFunction : CSL_Code
        {
            MethodCache cache = null;
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                List<CSL_Content.Value> _params = new List<CSL_Content.Value>();
                for (int i = 0; i < codes.Count; i++)
                {
                    _params.Add(codes[i].Execute(content));
                }
                CSL_Content.Value value = null;
                if (cache != null && cache.info != null)
                {
                    value = type.function.StaticCallCache(content, _params, cache);
                }
                else
                {
                    cache = new MethodCache();
                    value = type.function.StaticCall(content, functionName, _params, cache);
                }

                content.OutStack(this);
                return value;
            }

            CSL_TypeBase type;
            string functionName;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out type);
                stream.Read(out functionName);
            }
#if UNITY_EDITOR

            public override string ToString()
            {
                return "StaticCall|" + type.Keyword + "." + functionName;
            }
#endif
        }
    }
}