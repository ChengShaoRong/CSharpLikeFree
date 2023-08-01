/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        class CSL_Type_Long : RegHelper_Type
        {
            public CSL_Type_Long()
                : base(typeof(long), "long", false)
            {
            }
            public override string FullName
            {
                get { return "long"; }
            }

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                bool convertSuccess = false;
                object convertedObject = CSL_NumericTypeUtils.TryConvertTo<long>(src, targetType, out convertSuccess);
                if (convertSuccess)
                {
                    return convertedObject;
                }

                return base.ConvertTo(content, src, targetType);
            }

            public override object Math2Value(CSL_Content content, string code, object left, CSL_Content.Value right, out CSL_Type returntype)
            {
                bool math2ValueSuccess = false;
                object value = CSL_NumericTypeUtils.Math2Value<long>(code, left, right, out returntype, out math2ValueSuccess);
                if (math2ValueSuccess)
                {
                    return value;
                }

                return base.Math2Value(content, code, left, right, out returntype);
            }

            public override bool MathLogic(CSL_Content content, TokenLogic code, object left, CSL_Content.Value right)
            {
                bool mathLogicSuccess;
                bool value = CSL_NumericTypeUtils.MathLogic<long>(code, left, right, out mathLogicSuccess);
                if (mathLogicSuccess)
                {
                    return value;
                }

                return base.MathLogic(content, code, left, right);
            }

            public override object DefValue
            {
                get { return (long)0; }
            }
        }
    }
}