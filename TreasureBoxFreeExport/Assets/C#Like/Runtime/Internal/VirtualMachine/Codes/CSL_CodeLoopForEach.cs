/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeLoopForEach : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                content.DepthAdd();
                CSL_CodeDefine codeDefine = codes[0] as CSL_CodeDefine;
                codeDefine.Execute(content);

                System.Collections.IEnumerable emu = codes[1].Execute(content).value as System.Collections.IEnumerable;

                CSL_Code codeBlock = codes[2];

                var it = emu.GetEnumerator();
                CSL_Content.Value vrt = null;
                while (it.MoveNext())
                {
                    content.Set(codeDefine.value_names[0], it.Current);
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
                            if (bbreak)
                                break;
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
                return "ForEach|";
            }
#endif
        }
    }
}