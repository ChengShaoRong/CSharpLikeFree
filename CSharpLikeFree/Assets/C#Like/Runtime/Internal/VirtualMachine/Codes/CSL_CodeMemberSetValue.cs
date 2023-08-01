/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeMemberSetValue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                var parent = codes[0].Execute(content);
                if (parent == null || parent.value == null)
                {
                    throw new Exception("null object:" + codes[0].ToString() + ":" + ToString());
                }
                var value = codes[1].Execute(content);
                object setv = value.value;
                var typefunction = content.vm.GetType(parent.type).function;
                if (parent.type is object)
                {
                    SInstance s = parent.value as SInstance;
                    if (s != null)
                    {
                        typefunction = s.type;
                    }
                }
                typefunction.MemberValueSet(content, parent.value, membername, setv);
                content.OutStack(this);
                return null;
            }

            string membername;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out membername);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "MemberSetvalue|a." + membername;
            }
#endif
        }
    }
}