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
        public class CSL_CodeFunctionNewArray : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                List<object> list = new List<object>();
                int count = codes[0] == null ? (codes.Count - 1) : (int)codes[0].Execute(content).value;
                //if (count <= 0)
                //    throw new Exception("array size can't <= 0");
                CSL_Content.Value vcount = new CSL_Content.Value();
                vcount.type = typeof(int);
                vcount.value = count;
                for (int i = 1; i < codes.Count; i++)
                {
                    list.Add(codes[i].Execute(content).value);
                }
                List<CSL_Content.Value> p = new List<CSL_Content.Value>();
                p.Add(vcount);
                var outvalue = type.function.New(content, p);
                for (int i = 0; i < list.Count; i++)
                {
                    type.function.IndexSet(content, outvalue.value, i, list[i]);
                }
                content.OutStack(this);
                return outvalue;

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