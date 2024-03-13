/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;
using System.Reflection;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeNotValue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);

                CSL_Content.Value r = codes[0].Execute(content);
                CSL_Content.Value valueNew = new CSL_Content.Value();
                valueNew.type = r.type;
                valueNew.breakBlock = r.breakBlock;
                Type t = r.type;
                if (t != null)
                {
                    MethodInfo mi = t.GetMethod("op_OnesComplement", new Type[] { t });
                    if (mi != null)
                        valueNew.value = mi.Invoke(null, new object[1] { r.value });
                    else
                    {
                        switch(t.Name)
                        {
                            case "Int64": valueNew.value = ~(Int64)r.value; break;
                            case "UInt64": valueNew.value = ~(UInt64)r.value; break;
                            case "UInt32": valueNew.value = ~(UInt32)r.value; break;
                            case "Int16": valueNew.value = ~(Int16)r.value; break;
                            case "UInt16": valueNew.value = ~(UInt16)r.value; break;
                            case "Byte": valueNew.value = ~(Byte)r.value; break;
                            case "SByte": valueNew.value = ~(SByte)r.value; break;
                            default: valueNew.value = ~(Int32)r.value; break;
                        }
                    }
                }
                content.OutStack(this);

                return valueNew;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "(~)|";
            }
#endif
        }
    }
}