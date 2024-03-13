/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        class CSL_Type_Object : RegHelper_Type
        {
            public CSL_Type_Object()
                : base(typeof(object), "object", false)
            {
            }
            public override string FullName
            {
                get { return "object"; }
            }
        }
    }
}