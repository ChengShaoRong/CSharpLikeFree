/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        class CSL_Type_Int : RegHelper_Type
        {
            public CSL_Type_Int()
                : base(typeof(int), "int", false)
            {

            }
            public override string FullName
            {
                get { return "int"; }
            }

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                bool convertSuccess;
                object convertedObject = CSL_NumericTypeUtils.TryConvertTo<int>(src, targetType, out convertSuccess);
                if (convertSuccess)
                {
                    return convertedObject;
                }

                return base.ConvertTo(content, src, targetType);
            }

            public override object Math2Value(CSL_Content content, string code, object left, CSL_Content.Value right, out CSL_Type returntype)
            {
                bool math2ValueSuccess;
                object value = CSL_NumericTypeUtils.Math2Value<int>(code, left, right, out returntype, out math2ValueSuccess);
                if (math2ValueSuccess)
                {
                    return value;
                }

                return base.Math2Value(content, code, left, right, out returntype);
            }

            public override bool MathLogic(CSL_Content content, TokenLogic code, object left, CSL_Content.Value right)
            {
                bool mathLogicSuccess;
                bool value = CSL_NumericTypeUtils.MathLogic<int>(code, left, right, out mathLogicSuccess);
                if (mathLogicSuccess)
                {
                    return value;
                }

                return base.MathLogic(content, code, left, right);
            }

            public override object DefValue
            {
                get { return 0; }
            }
        }
    }
}