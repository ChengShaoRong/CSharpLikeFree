/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        class CSL_Type_String : CSL_TypeBase
        {
            public CSL_Type_String()
            {
                function = new CSL_RegHelper_TypeFunction(typeof(string));
            }
            public override string Keyword
            {
                get { return "string"; }
            }
            public override string Namespace
            {
                get { return ""; }
            }
            public override string FullName
            {
                get { return "string"; }
            }
            public override CSL_Type type
            {
                get { return typeof(string); }
            }

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                if ((Type)targetType == typeof(string)) return src;
                if ((Type)targetType == typeof(void))
                {
                    return null;
                }
                if (((Type)targetType).IsAssignableFrom(typeof(string)))
                {
                    return src;
                }
                Type targettype = targetType;
                if (targettype != null)
                {
                    var m = targettype.GetMethod("op_Implicit", new Type[] { type });
                    if (m == null)
                        m = targettype.GetMethod("op_Explicit", new Type[] { type });
                    if (m != null)
                        return m.Invoke(null, new object[] { src });
                }
                return null;
            }

            public override object Math2Value(CSL_Content content, string code, object left, CSL_Content.Value right, out CSL_Type returntype)
            {
                returntype = typeof(string);
                if (code == "+")
                {
                    if (right.value == null)
                    {
                        return (string)left + "null";
                    }
                    else
                    {
                        return (string)left + right.value.ToString();
                    }
                }
                throw new NotImplementedException();
            }

            public override bool MathLogic(CSL_Content content, TokenLogic code, object left, CSL_Content.Value right)
            {
                if (code == TokenLogic.Equal)
                {
                    return (string)left == (string)right.value;
                }
                else if (code == TokenLogic.NotEqual)
                {
                    return (string)left != (string)right.value;
                }
                throw new NotImplementedException();
            }
            public override CSL_TypeFunctionBase function
            {
                get;
                protected set;
            }
            public override object DefValue
            {
                get { return null; }
            }
        }
    }
}