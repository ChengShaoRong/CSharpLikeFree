/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        class CSL_Type_NULL : CSL_TypeBase
        {
            public override string Keyword
            {
                get { return "null"; }
            }
            public override string Namespace
            {
                get { return ""; }
            }
            public override string FullName
            {
                get { return "null"; }
            }
            public override CSL_Type type
            {
                get { return null; }
            }

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                return null;
            }

            public override object Math2Value(CSL_Content content, string code, object left, CSL_Content.Value right, out CSL_Type returntype)
            {

                if ((Type)right.type == typeof(string))
                {
                    returntype = typeof(string);
                    return "null" + right.value;
                }
                throw new NotImplementedException();
            }

            public override bool MathLogic(CSL_Content content, TokenLogic code, object left, CSL_Content.Value right)
            {
                if (code == TokenLogic.Equal)
                {
                    return null == right.value;
                }
                else if (code == TokenLogic.NotEqual)
                {
                    return null != right.value;
                }
                throw new NotImplementedException();
            }
        }
    }
}