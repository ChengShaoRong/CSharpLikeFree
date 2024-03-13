/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeTypeCheck : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);

                var v = codes[0].Execute(content);
                CSL_TypeBase type = content.vm.GetType(v.type);
                CSL_Content.Value value = new CSL_Content.Value();
                value.type = typeof(bool);
                value.value = type.ConvertTo(content, v.value, targetType) != null;
                content.OutStack(this);

                return value;
            }

            CSL_Type targetType;
            public override void Read(CSL_StreamRead stream)
            {
                base.Read(stream);
                stream.Read(out targetType);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "is (" + targetType.Name + ")";
            }
#endif
        }
    }
}