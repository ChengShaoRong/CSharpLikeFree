/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeNegativeLogic : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);

                CSL_Content.Value value = codes[0].Execute(content);
                CSL_Content.Value newValue = new CSL_Content.Value();
                newValue.type = value.type;
                newValue.breakBlock = value.breakBlock;
                newValue.value = !value.IsTrue;
                content.OutStack(this);

                return newValue;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "(!)|";
            }
#endif
        }
    }
}