/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeMath3Value : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value result = new CSL_Content.Value();
                CSL_Content.Value value = codes[0].Execute(content);
                if ((Type)value.type != typeof(bool))
                {
                    throw new Exception("Ternary expression condition must be type of boolean");
                }
                if ((bool)value.value)
                    value = codes[1].Execute(content);
                else
                    value = codes[2].Execute(content);
                result.value = value.value;
                result.type = value.type;
                content.OutStack(this);
                return result;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "Math3Value|a?b:c";
            }
#endif
        }
    }
}