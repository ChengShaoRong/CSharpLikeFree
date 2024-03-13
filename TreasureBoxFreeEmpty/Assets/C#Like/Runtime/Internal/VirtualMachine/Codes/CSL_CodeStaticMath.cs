/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeStaticMath : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);

                var getvalue = type.function.StaticValueGet(content, memberName);

                CSL_Content.Value vright = CSL_Content.Value.One;
                if (codes.Count > 0)
                {
                    vright = codes[0].Execute(content);
                }
                CSL_Content.Value vout = new CSL_Content.Value();
                var mtype = content.vm.GetType(getvalue.type);
                vout.value = mtype.Math2Value(content, mathOp, getvalue.value, vright, out vout.type);

                type.function.StaticValueSet(content, memberName, vout.value);

                content.OutStack(this);
                return vout;
            }

            CSL_TypeBase type;
            string memberName;
            string mathOp;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out type);
                stream.Read(out memberName);
                stream.Read(out mathOp);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "StaticMath|" + type.Keyword + "." + memberName + " |" + mathOp;
            }
#endif
        }
    }
}