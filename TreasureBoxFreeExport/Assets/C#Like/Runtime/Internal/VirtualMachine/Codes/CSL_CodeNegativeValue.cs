/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeNegativeValue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);

                CSL_Content.Value r = codes[0].Execute(content);
                CSL_Content.Value valueNew = new CSL_Content.Value();
                valueNew.type = r.type;
                valueNew.breakBlock = r.breakBlock;
                CSL_TypeBase type = content.vm.GetType(r.type);

                valueNew.value = type.Math2Value(content, "*", r.value, CSL_Content.Value.OneMinus, out r.type);
                content.OutStack(this);

                return r;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "(-)|";
            }
#endif
        }
    }
}