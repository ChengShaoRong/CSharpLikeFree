/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeLoopFor : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                content.DepthAdd();
                CSL_Code codeInit = codes[0];
                if (codeInit != null) codeInit.Execute(content);

                CSL_Code codeCondition = codes[1];
                CSL_Code codeBlock = codes[2];
                CSL_Content.Value vrt = null;
                while(true)
                {
                    if (codeCondition != null && !codeCondition.Execute(content).IsTrue) break;

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

                    for(int i=3; i< codes.Count; i++)
                    {
                        if (codes[i] != null)
                        {
                            content.DepthAdd();
                            codes[i].Execute(content);
                            content.DepthRemove();
                        }
                    }
                }
                content.DepthRemove();
                content.OutStack(this);
                return vrt;
            }
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
                stream.ReadNullEx(codes);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "For|";
            }
#endif
        }
    }
}