/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodePlusValue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value r = codes[0].Execute(content);
                CSL_Content.Value valueNew = new CSL_Content.Value();
                valueNew.type = r.type;
                valueNew.breakBlock = r.breakBlock;
                valueNew.value = r.value;
                content.OutStack(this);

                return valueNew;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "(+)|";
            }
#endif
        }
    }
}