/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeLoopReturn : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value rv = new CSL_Content.Value();
                rv.breakBlock = CSL_BreakBlock.Return;
                if (codes.Count > 0 && codes[0] != null)
                {
                    var v = codes[0].Execute(content);
                    {
                        rv.type = v.type;
                        rv.value = v.value;
                    }
                }
                else
                {
                    rv.type = typeof(void);
                }
                content.OutStack(this);
                return rv;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "return";
            }
#endif
        }
    }
}