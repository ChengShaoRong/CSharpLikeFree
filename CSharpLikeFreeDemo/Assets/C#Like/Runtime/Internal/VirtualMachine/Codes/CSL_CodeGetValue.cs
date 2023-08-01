/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_CodeGetValue : CSL_Code
        {
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                var value = content.Get(value_name);
                content.OutStack(this);
                return value;
            }

            public string value_name;
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
                stream.Read(out value_name);
            }

#if UNITY_EDITOR
            public override string ToString()
            {
                return "GetValue|" + value_name;
            }
#endif
        }
    }
}