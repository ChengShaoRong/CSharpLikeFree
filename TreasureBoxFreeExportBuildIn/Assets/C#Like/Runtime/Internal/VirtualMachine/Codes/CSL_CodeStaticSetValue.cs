/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeStaticSetValue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                var value = codes[0].Execute(content);
                type.function.StaticValueSet(content, memberName, value.value);
                content.OutStack(this);
                return null;
            }
            CSL_TypeBase type;
            string memberName;

            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out type);
                stream.Read(out memberName);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "StaticSetvalue|" + type.Keyword + "." + memberName;
            }
#endif
        }
    }
}