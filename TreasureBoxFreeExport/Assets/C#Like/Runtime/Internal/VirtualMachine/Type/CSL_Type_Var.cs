/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_Type_Var : CSL_TypeBase
        {
            public class var
            {
            }
            public override string Keyword
            {
                get { return "var"; }
            }
            public override string Namespace
            {
                get { return ""; }
            }
            public override string FullName
            {
                get { return "var"; }
            }
            public override CSL_Type type
            {
                get { return (typeof(var)); }
            }
        }
    }
}