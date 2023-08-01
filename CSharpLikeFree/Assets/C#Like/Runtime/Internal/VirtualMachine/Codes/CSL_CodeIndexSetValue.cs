/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeIndexSetValue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                var parent = codes[0].Execute(content);
                if (parent == null || parent.value == null)
                {
                    throw new Exception("null object:" + codes[0].ToString() + ":" + ToString());
                }
                var key = codes[1].Execute(content);
                var value = codes[2].Execute(content);
                var type = content.vm.GetType(parent.type);
                type.function.IndexSet(content, parent.value, key.value, value);
                content.OutStack(this);
                return null;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "IndexSet[]=|";
            }
#endif
        }
    }
}