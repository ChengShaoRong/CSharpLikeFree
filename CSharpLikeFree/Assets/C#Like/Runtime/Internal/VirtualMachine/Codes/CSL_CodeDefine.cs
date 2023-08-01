/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeDefine : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);

                for (int i = 0; i < codes.Count; i++)
                {
                    CSL_Code code = codes[i];
                    string value_name = value_names[i];
                    if (code != null)
                    {
                        CSL_Content.Value v = code.Execute(content);
                        object val = v.value;
                        if ((Type)value_type == typeof(CSL_Type_Var.var))
                        {
                            if (v.type != null)
                                value_type = v.type;

                        }
                        else if (v.type != value_type)
                        {
                            val = content.vm.GetType(v.type).ConvertTo(content, v.value, value_type);

                        }

                        content.DefineAndSet(value_name, value_type, val);
                    }
                    else
                    {
                        content.Define(value_name, value_type);
                    }
                }
                content.OutStack(this);

                return null;
            }
            public List<string> value_names;
            public CSL_Type value_type;
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
                stream.ReadNullEx(codes);
                stream.Read(out value_names);
                stream.Read(out value_type);
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                string str = "Define|" + value_type.Name;
                foreach (var item in value_names)
                    str += " " + item;
                return str;
            }
#endif
        }
    }
}