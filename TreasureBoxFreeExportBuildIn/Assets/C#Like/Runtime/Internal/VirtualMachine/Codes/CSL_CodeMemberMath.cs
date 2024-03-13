/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeMemberMath : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                var parent = codes[0].Execute(content);
                if (parent == null || parent.value == null)
                {
                    if (checkNull
                        || parent.type.FullName.StartsWith("CSharpLike.Internal.CSL_Type_Nullable")
                        /*|| parent.type.FullName.StartsWith("System.Nullable")*/)
                    {
                        content.OutStack(this);
                        return CSL_Content.Value.Null;
                    }
                    throw new Exception("null object:" + codes[0].ToString() + ":" + ToString());
                }
                var type = content.vm.GetType(parent.type);

                var getvalue = type.function.MemberValueGet(content, parent.value, memberName);

                CSL_Content.Value vright = CSL_Content.Value.One;
                if (codes.Count > 1)
                {
                    vright = codes[1].Execute(content);
                }
                CSL_Content.Value vout = new CSL_Content.Value();
                var mtype = content.vm.GetType(getvalue.type);
                vout.value = mtype.Math2Value(content, mathOp, getvalue.value, vright, out vout.type);

                type.function.MemberValueSet(content, parent.value, memberName, vout.value);

                content.OutStack(this);
                return vout;
            }

            string memberName;
            string mathOp;
            bool checkNull;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out memberName);
                stream.Read(out mathOp);
                stream.Read(out checkNull);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "CLS_Expression_MemberMath|a." + memberName + " |" + mathOp;
            }
#endif
        }
    }
}