/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeSetValue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value v = codes[0].Execute(content);
                object val = v.value;
                CSL_Type value_type = null;
                SType.Member retM = null;
                if (content.values.ContainsKey(value_name))
                {
                    value_type = content.values[value_name].type;
                }
                else if (content.CallType != null)
                {
                    retM = content.CallType.GetMember(value_name, content.vm);
                    if (retM != null)
                    {
                        value_type = retM.type.type;
                    }
                }
                if ((Type)value_type != typeof(CSL_Type_Var.var) && value_type != v.type)
                {
                    val = content.vm.GetType(v.type).ConvertTo(content, v.value, value_type);
                }

                content.Set(value_name, val);
                content.OutStack(this);
                return null;
            }

            string value_name;

            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out value_name);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "SetValue|" + value_name;
            }
#endif
        }
    }
}