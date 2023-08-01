/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System.Collections.Generic;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeFunctionNew : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                List<CSL_Content.Value> list = new List<CSL_Content.Value>();
                foreach (CSL_Code p in codes)
                {
                    if (p != null)
                    {
                        list.Add(p.Execute(content));
                    }
                }
                var value = type.function.New(content, list);
                content.OutStack(this);
                return value;
            }
            CSL_TypeBase type;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out type);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "new|" + type.Keyword + "(params[" + codes.Count + ")";
            }
#endif
        }
    }
}