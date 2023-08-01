/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeMath2ValueLogic : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value result = new CSL_Content.Value();
                result.type = typeof(bool);
                var left = codes[0].Execute(content);
                var right = codes[1].Execute(content);
                if (left.type == null || right.type == null)
                {
                    if (mathop == TokenLogic.Equal)
                    {
                        result.value = left.value == right.value;
                    }
                    if (mathop == TokenLogic.NotEqual)
                    {
                        result.value = left.value != right.value;
                    }
                }
                else if ((Type)left.type == typeof(bool) && (Type)right.type == typeof(bool))
                {
                    if (mathop == TokenLogic.Equal)
                    {
                        result.value = (bool)left.value == (bool)right.value;
                    }
                    else if (mathop == TokenLogic.NotEqual)
                    {
                        result.value = (bool)left.value != (bool)right.value;
                    }
                    else
                    {
                        throw new Exception("bool not support this operator");
                    }
                }
                else
                {
                    result.value = content.vm.GetType(left.type).MathLogic(content, mathop, left.value, right);
                }
                content.OutStack(this);

                return result;
            }
            TokenLogic mathop;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                mathop = (TokenLogic)stream.ReadByte();
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "Math2ValueLogic|a" + mathop + "b";
            }
#endif
        }
    }
}