/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeLoopContinue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value rv = new CSL_Content.Value();
                rv.breakBlock = CSL_BreakBlock.Continue;
                content.OutStack(this);
                return rv;
            }
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "continue";
            }
#endif
        }
    }
}