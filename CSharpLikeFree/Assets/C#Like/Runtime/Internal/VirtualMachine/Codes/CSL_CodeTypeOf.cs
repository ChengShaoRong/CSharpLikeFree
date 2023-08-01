/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeTypeOf : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value value = new CSL_Content.Value();
                Type t = targetType;
                if (t != null)
                {
                    value.type = typeof(Type);
                    value.value = t;
                }
                else
                {
                    value.type = typeof(SType);
                    value.value = (SType)targetType;
                }
                content.OutStack(this);

                return value;
            }

            CSL_Type targetType;
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
                stream.Read(out targetType);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "typeof(" + targetType.Name + ")";
            }
#endif
        }
    }
}