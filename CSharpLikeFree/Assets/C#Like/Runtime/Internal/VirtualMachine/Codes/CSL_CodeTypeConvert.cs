/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeTypeConvert : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);

                var right = codes[0].Execute(content);
                CSL_TypeBase type = content.vm.GetType(right.type);
                CSL_Content.Value value = new CSL_Content.Value();
                value.type = targetType;
                value.value = type.ConvertTo(content, right.value, targetType);

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
                return "as (" + targetType.Name + ")";
            }
#endif
        }
    }
}