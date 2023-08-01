/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeLoopIf : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value value = null;
                if (codes[0].Execute(content).IsTrue)
                {
                    CSL_Code codeBlockIf = codes[1];
                    if (codeBlockIf != null)
                    {
                        if (codeBlockIf is CSL_CodeBlock)
                        {
                            value = codeBlockIf.Execute(content);
                        }
                        else
                        {
                            content.DepthAdd();
                            value = codeBlockIf.Execute(content);
                            content.DepthRemove();
                        }
                    }
                }
                else if (codes.Count > 2)
                {
                    CSL_Code codeBlockElse = codes[2];
                    if (codeBlockElse != null)
                    {
                        if (codeBlockElse is CSL_CodeBlock)
                        {
                            value = codeBlockElse.Execute(content);
                        }
                        else
                        {
                            content.DepthAdd();
                            value = codeBlockElse.Execute(content);
                            content.DepthRemove();
                        }
                    }
                }
                content.OutStack(this);
                return value;
            }
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
                stream.ReadNullEx(codes);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "If|";
            }
#endif
        }
    }
}