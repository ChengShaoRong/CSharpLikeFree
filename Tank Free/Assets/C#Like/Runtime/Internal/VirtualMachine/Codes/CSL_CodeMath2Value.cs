/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeMath2Value : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value result = new CSL_Content.Value();
                CSL_Content.Value left = codes[0].Execute(content);
                result.value = content.vm.GetType(left.type).Math2Value(content, mathOp, left.value, codes[1].Execute(content), out result.type);
                content.OutStack(this);
                return result;
            }
            string mathOp;

            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out mathOp);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "Math2Value|a" + mathOp + "b";
            }
#endif
        }
    }
}