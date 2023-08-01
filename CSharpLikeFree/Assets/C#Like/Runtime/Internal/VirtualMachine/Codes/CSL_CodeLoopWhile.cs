/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeLoopWhile : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                content.DepthAdd();
                CSL_Code codeWhile = codes[0];
                CSL_Code codeBlock = codes[1];
                CSL_Content.Value vrt = null;
                while (codeWhile.Execute(content).IsTrue)
                {
                    if (codeBlock != null)
                    {
                        if (codeBlock is CSL_CodeBlock)
                        {
                            var v = codeBlock.Execute(content);
                            if (v != null)
                            {
                                if (v.breakBlock > CSL_BreakBlock.Break) vrt = v;
                                if (v.breakBlock > CSL_BreakBlock.Continue) break;
                            }
                        }
                        else
                        {
                            content.DepthAdd();
                            bool bbreak = false;
                            var v = codeBlock.Execute(content);
                            if (v != null)
                            {
                                if (v.breakBlock > CSL_BreakBlock.Break) vrt = v;
                                if (v.breakBlock > CSL_BreakBlock.Continue) bbreak = true;
                            }
                            content.DepthRemove();
                            if (bbreak) break;
                        }
                    }
                }
                content.DepthRemove();
                content.OutStack(this);
                return vrt;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "While|";
            }
#endif
        }
    }
}