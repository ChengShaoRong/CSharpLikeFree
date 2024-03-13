/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System.Collections.Generic;

namespace CSharpLike
{
    namespace Internal
    {
        public enum CSL_BreakBlock : byte
        {
            None,
            Continue,
            Break,
            Return,
        }
        public enum CSL_CodeType : byte
        {
            Block = 0,
            Define,
            GetValue,
            SetValue,
            Math_Math2Value = 10,
            Math_Math2ValueAndOr,
            Math_Math2ValueLogic,
            Math_Math3Value,
            Math_NegativeLogic,
            Math_NegativeValue,
            Math_PlusValue,
            Math_NotValue,
            Math_SelfOp,
            Math_SelfOpWithValue,
            Math_TypeCheck,
            Math_TypeConvert,
            Math_TypeOf,
            Func_Function = 30,
            Func_FunctionNew,
            Func_FunctionNewArray,
            Func_IndexFind,
            Func_IndexSetValue,
            Func_Lambda,
            Func_MemberFind,
            Func_MemberFunction,
            Func_MemberMath,
            Func_SetValue,
            Func_StaticFind,
            Func_StaticFunction,
            Func_StaticMath,
            Func_StaticSetValue,
            Func_Throw,
            Loop_Break = 60,
            Loop_Continue,
            Loop_DoWhile,
            Loop_For,
            Loop_ForEach,
            Loop_If,
            Loop_Return,
            Loop_While,
            Value_Null = 80,
            Value_String,
            Value_Boolean,
            Value_Char,
            Value_Single,
            Value_Double,
            Value_Byte,
            Value_SByte,
            Value_Int16,
            Value_UInt16,
            Value_Int32,
            Value_UInt32,
            Value_Int64,
            Value_UInt64,
        }
        public abstract class CSL_Value : CSL_Code
        {
            public virtual CSL_Type type
            {
                get;
                set;
            }
            public virtual object Value
            {
                get;
                set;
            }
            public bool IsTrue
            {
                get
                {
                    object value = Value;
                    return (value is bool) ? (bool)value : (value != null);
                }
            }
        }

        public abstract class CSL_Code
        {
            public List<CSL_Code> codes = new List<CSL_Code>();
            public ushort line;
            public abstract CSL_Content.Value Execute(CSL_Content content);
            public virtual void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
                stream.Read(codes);
            }
        }
    }
}