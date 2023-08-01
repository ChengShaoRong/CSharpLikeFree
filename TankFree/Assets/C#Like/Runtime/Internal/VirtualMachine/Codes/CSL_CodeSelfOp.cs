/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeSelfOp : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);

                CSL_Content.Value v = content.Get(valueName);
                CSL_TypeBase type = content.vm.GetType(v.type);
                CSL_Type returntype;
                if (pre)
                {
                    object value = type.Math2Value(content, mathOp, v.value, CSL_Content.Value.One, out returntype);
                    value = type.ConvertTo(content, value, v.type);
                    content.Set(valueName, value);

                    content.OutStack(this);
                    return v;
                }
                else
                {
                    CSL_Content.Value newValue = new CSL_Content.Value();
                    newValue.type = v.type;
                    newValue.value = v.value;

                    object value = type.Math2Value(content, mathOp, v.value, CSL_Content.Value.One, out returntype);
                    value = type.ConvertTo(content, value, v.type);
                    content.Set(valueName, value);

                    content.OutStack(this);
                    return newValue;
                }
            }

            string valueName;
            string mathOp;
            bool pre = false;
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
                stream.Read(out valueName);
                stream.Read(out mathOp);
                stream.Read(out pre);
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                return "MathSelfOp|" + valueName + mathOp;
            }
#endif
        }
    }
}