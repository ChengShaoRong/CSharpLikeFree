/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeBlock : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                content.DepthAdd();

                CSL_Content.Value value = null;
                foreach (CSL_Code code in codes)
                {
                    if (code != null)
                    {
                        try
                        {
                            value = code.Execute(content);
                        }
                        catch (Exception err)
                        {
                            if (CSL_VirtualMachine.bPrintError)
                            {
                                CSL_VirtualMachine.bPrintError = false;
                                UnityEngine.Debug.LogError("Error MSG:" + err.Message + " Stack info:\n" + content.StackInfo + "\nOriginal stack info:\n" + err.StackTrace);
                            }
                            throw;
                        }
                    }

                    if (value != null && value.breakBlock != CSL_BreakBlock.None) break;
                }
                content.DepthRemove();
                content.OutStack(this);
                return value;
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                return "Block|";
            }
#endif
        }
    }
}