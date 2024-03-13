/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeStaticFind : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                var value = type.function.StaticValueGet(content, memberName);
                content.OutStack(this);
                return value;
            }

            public CSL_TypeBase type;
            public string memberName;
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
                stream.Read(out type);
                stream.Read(out memberName);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "StaticFind|" + type.Keyword + "." + memberName;
            }
#endif
        }
    }
}