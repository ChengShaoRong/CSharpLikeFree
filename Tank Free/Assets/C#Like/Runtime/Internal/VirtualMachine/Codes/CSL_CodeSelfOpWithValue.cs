/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeSelfOpWithValue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);

                CSL_Code codeLeft = codes[0];
                var left = codeLeft.Execute(content);
                var right = codes[1].Execute(content);
                CSL_TypeBase type = content.vm.GetType(left.type);
                CSL_Type returnType;
                object value = type.Math2Value(content, mathOp, left.value, right, out returnType);
                value = type.ConvertTo(content, value, left.type);
                left.value = value;
                if (codeLeft is CSL_CodeMemberFind)
                {
                    CSL_CodeMemberFind f = codeLeft as CSL_CodeMemberFind;
                    var ptype = content.vm.GetType(f.parent.type);
                    ptype.function.MemberValueSet(content, f.parent.value, f.membername, value);
                }
                else if (codeLeft is CSL_CodeStaticFind)
                {
                    CSL_CodeStaticFind f = codeLeft as CSL_CodeStaticFind;
                    f.type.function.StaticValueSet(content, f.memberName, value);
                }
                else if (codeLeft is CSL_CodeIndexFind)
                {
                    CSL_CodeIndexFind f = codeLeft as CSL_CodeIndexFind;
                    f.SetValue(content, left);
                }
                content.OutStack(this);

                return null;
            }

            string mathOp;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out mathOp);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "MathSelfOp|" + mathOp;
            }
#endif
        }
    }
}