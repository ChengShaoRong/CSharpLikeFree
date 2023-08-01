/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeThrow : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value rv = new CSL_Content.Value();

                var v = codes[0].Execute(content);
                {
                    rv.type = v.type;
                    rv.value = v.value;
                }
                Exception err = v.value as Exception;
                if (err != null)
                {
                    throw err;
                }
                else
                {
                    throw new Exception(v.ToString());
                }
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "throw";
            }
#endif
        }
    }
}