/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeMath2ValueAndOr : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value result;
                if (mathOp == '?')
                {
                    CSL_Content.Value v = codes[0].Execute(content);
                    if (v.value != null)
                    {
                        Type t = v.type;
                        var op = t.GetMethod("get_Value", new Type[] {});
                        if (op != null)
                        {
                            result = new CSL_Content.Value();
                            result.value = op.Invoke(v.value, new object[] { });
                            result.type = content.vm.GetType(op.ReturnParameter.ParameterType).type;
                            content.OutStack(this);
                            return result;
                        }
                        else
                        {
                            content.OutStack(this);
                            return v;
                        }
                    }
                    else
                    {
                        v = codes[1].Execute(content);
                        content.OutStack(this);
                        return v;
                    }
                }
                result = new CSL_Content.Value();
                bool bLeft = GetValue(content, true);
                result.type = typeof(bool);
                if (mathOp == '&')
                {
                    if (!bLeft)
                    {
                        result.value = false;
                    }
                    else
                    {
                        result.value = GetValue(content, false);
                    }
                }
                else if (mathOp == '|')
                {
                    if (bLeft)
                    {
                        result.value = true;
                    }
                    else
                    {
                        result.value = GetValue(content, false);
                    }
                }
                content.OutStack(this);
                return result;

            }
            bool GetValue(CSL_Content content, bool bLeft)
            {
                CSL_Code codeRight = codes[bLeft ? 0 : 1];
                return (codeRight is CSL_Value) ? (codeRight as CSL_Value).IsTrue : codeRight.Execute(content).IsTrue;
            }

            char mathOp;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out mathOp);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "Math2ValueAndOr|a" + mathOp + "b";
            }
#endif
        }
    }
}