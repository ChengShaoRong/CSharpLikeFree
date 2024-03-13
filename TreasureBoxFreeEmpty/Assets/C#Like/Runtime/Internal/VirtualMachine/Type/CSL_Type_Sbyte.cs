/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        class CSL_Type_Sbyte : RegHelper_Type
        {
            public CSL_Type_Sbyte()
                : base(typeof(sbyte), "sbyte", false)
            {

            }
            public override string FullName
            {
                get { return "sbyte"; }
            }

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                bool convertSuccess;
                object convertedObject = CSL_NumericTypeUtils.TryConvertTo<sbyte>(src, targetType, out convertSuccess);
                if (convertSuccess)
                {
                    return convertedObject;
                }

                return base.ConvertTo(content, src, targetType);
            }

            public override object Math2Value(CSL_Content content, string code, object left, CSL_Content.Value right, out CSL_Type returntype)
            {
                bool math2ValueSuccess;
                object value = CSL_NumericTypeUtils.Math2Value<sbyte>(code, left, right, out returntype, out math2ValueSuccess);
                if (math2ValueSuccess)
                {
                    return value;
                }

                return base.Math2Value(content, code, left, right, out returntype);
            }

            public override bool MathLogic(CSL_Content content, TokenLogic code, object left, CSL_Content.Value right)
            {
                bool mathLogicSuccess;
                bool value = CSL_NumericTypeUtils.MathLogic<sbyte>(code, left, right, out mathLogicSuccess);
                if (mathLogicSuccess)
                {
                    return value;
                }

                return base.MathLogic(content, code, left, right);
            }

            public override object DefValue
            {
                get { return (sbyte)0; }
            }
        }
    }
}