/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeLambda : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value value = new CSL_Content.Value();
                value.type = typeof(CSL_DeleLambda);
                value.value = new CSL_DeleLambda(content, (codes[0] as CSL_CodeBlock).codes, codes[1]);
                content.OutStack(this);
                return value;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "()=>{}|";
            }
#endif
        }
    }
}