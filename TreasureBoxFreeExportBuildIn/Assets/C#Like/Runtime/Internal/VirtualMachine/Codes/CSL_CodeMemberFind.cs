/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeMemberFind : CSL_Code
        {
            public CSL_Content.Value parent;
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                parent = codes[0].Execute(content);
                if (parent == null || parent.value == null)
                {
                    if (checkNull
                        || parent.type.FullName.StartsWith("CSharpLike.Internal.CSL_Type_Nullable")
                        /*|| parent.type.FullName.StartsWith("System.Nullable")*/)
                    {
                        content.OutStack(this);
                        return (membername == "HasValue") ? CSL_Content.Value.False : CSL_Content.Value.Null;
                    }
                    throw new Exception("null object:" + codes[0].ToString() + ":" + ToString());
                }
                CSL_TypeFunctionBase typefunction = content.vm.GetType(parent.type).function;
                if (parent.type is object)
                {
                    SInstance s = parent.value as SInstance;
                    if (s != null)
                    {
                        typefunction = s.type;
                    }
                }
                var value = typefunction.MemberValueGet(content, parent.value, membername);
                content.OutStack(this);
                return value;
            }

            public string membername;
            bool checkNull;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out membername);
                stream.Read(out checkNull);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "MemberFind|a." + membername;
            }
#endif
        }
    }
}