/*
 *           C#Like
 * Copyright © 2022-2024 RongRong
 * It's automatic generate by CSharpLikeEditor,don't modify this file.
 */
using System;

namespace CSharpLikeEditor
{
    public class GetSizeOf
    {
        //build in data type no need to allow 'unsafe' code:
        //such as int/uint/long/ulong/short/ushort/char/float/double/decimal/bool/sbyte/byte
        //The define symbol open by the check box of Menu/Edit/Project Setting/Player/Other Settings/Allow 'unsafe' Code
        //The define symbol close by the remove it from Menu/Edit/Project Settings/Player/Other Settings/Scripting Define Symbols
#if _CSHARP_LIKE_ALLOW_UNSAFE_CODE_
        public unsafe static int GetSize(Type type)
        {
            switch (type.FullName)
            {
                default:
                    UnityEngine.Debug.LogError("not exist sizeof(" + type.FullName + ") and temporary return 0.Now automatic generate it, and you must recompile again will get the right value.");
                    return 0;
            }
        }
#else
        public static int GetSize(Type type)
        {
            throw new Exception("You are not allow unsafe setting.You should modify Menu/Edit/Project Setting/Player/Other Settings/Allow 'unsafe' Code.");
        }
#endif
    }
}