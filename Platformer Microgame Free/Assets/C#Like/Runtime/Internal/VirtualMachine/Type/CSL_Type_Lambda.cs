/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */

using System;

namespace CSharpLike
{
    namespace Internal
    {
        class CSL_Type_Lambda : CSL_TypeBase
        {
            public CSL_Type_Lambda()
            {
                function = null;
            }
            public override string Keyword
            {
                get { return "()=>"; }
            }
            public override string Namespace
            {
                get { return ""; }
            }
            public override string FullName
            {
                get { return "()=>"; }
            }
            public override CSL_Type type
            {
                get { return typeof(CSL_DeleLambda); }
            }

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                if (src.GetType() == (Type)targetType)
                    return src;
                CSL_TypeDeleBase dele = content.vm.GetType(targetType) as CSL_TypeDeleBase;
                return dele.CreateDelegate(content.vm, src as CSL_DeleLambda);
            }

            public override CSL_TypeFunctionBase function
            {
                get;
                protected set;
            }
        }
    }
}