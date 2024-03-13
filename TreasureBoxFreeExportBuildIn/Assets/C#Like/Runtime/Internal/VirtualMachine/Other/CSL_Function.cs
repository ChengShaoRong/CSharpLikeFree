/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System.Collections.Generic;

namespace CSharpLike
{
    namespace Internal
    {
        public interface ICSL_Function
        {
            string keyword
            {
                get;
            }

            CSL_Content.Value Call(CSL_Content content, IList<CSL_Content.Value> param);
        }

        public interface ICSL_FunctionMember
        {
            string keyword
            {
                get;
            }
            CSL_Content.Value Call(CSL_Content content, object objthis, IList<CSL_Content.Value> param);
        }
    }
}
