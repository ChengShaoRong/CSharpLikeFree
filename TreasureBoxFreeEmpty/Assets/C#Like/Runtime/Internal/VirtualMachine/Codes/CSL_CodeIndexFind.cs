/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeIndexFind : CSL_Code
        {
            CSL_Content.Value parent;
            CSL_Content.Value key;
            CSL_TypeBase type;
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                parent = codes[0].Execute(content);
                if (parent == null || parent.value == null)
                {
                    throw new Exception("null object:" + codes[0].ToString() + ":" + ToString());
                }
                key = codes[1].Execute(content);
                type = content.vm.GetType(parent.type);

                var value = type.function.IndexGet(content, parent.value, key.value);
                content.OutStack(this);

                return value;
            }
            public void SetValue(CSL_Content content, CSL_Content.Value value)
            {
                type.function.IndexSet(content, parent.value, key.value, value);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "IndexFind[]|";
            }
#endif
        }
    }
}