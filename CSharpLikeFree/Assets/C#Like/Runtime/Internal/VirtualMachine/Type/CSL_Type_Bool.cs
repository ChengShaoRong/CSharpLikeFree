/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;


namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_Type_Bool : CSL_TypeBase
        {
            public override string Keyword
            {
                get { return "bool"; }
            }
            public override string Namespace
            {
                get { return ""; }
            }
            public override string FullName
            {
                get { return "bool"; }
            }
            public override CSL_Type type
            {
                get { return (typeof(bool)); }
            }

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                Type targettype = targetType;
                if (targettype != null)
                {
                    var m = targettype.GetMethod("op_Implicit", new Type[] { type });
                    if (m == null)
                        m = targettype.GetMethod("op_Explicit", new Type[] { type });
                    if (m != null)
                        return m.Invoke(null, new object[] { src });
                }
                throw new NotImplementedException();
            }
            public override object DefValue
            {
                get { return false; }
            }
        }
    }
}