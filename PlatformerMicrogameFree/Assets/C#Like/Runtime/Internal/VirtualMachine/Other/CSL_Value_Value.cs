/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_Value_Value<T> : CSL_Value
        {
            public override CSL_Type type
            {
                get { return typeof(T); }
            }

            public T value_value;
            public override object Value
            {
                get
                {
                    return value_value;
                }
                set
                {
                    value_value = (T)value;
                }
            }
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value v = new CSL_Content.Value();
                v.type = type;
                v.value = value_value;
                content.OutStack(this);
                return v;
            }
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
                switch (type.Name)
                {
                    case "String": Value = stream.ReadString(); break;
                    case "Boolean": Value = stream.ReadBoolean(); break;
                    case "Byte": Value = stream.ReadByte(); break;
                    case "SByte": Value = stream.ReadSByte(); break;
                    case "Int16": Value = stream.ReadInt16(); break;
                    case "UInt16": Value = stream.ReadUInt16(); break;
                    case "Int32": Value = stream.ReadInt32(); break;
                    case "UInt32": Value = stream.ReadUInt32(); break;
                    case "Char": Value = stream.ReadChar(); break;
                    case "Single": Value = stream.ReadSingle(); break;
                    case "Double": Value = stream.ReadDouble(); break;
                    case "Int64": Value = stream.ReadInt64(); break;
                    case "UInt64": Value = stream.ReadUInt64(); break;
                    default:
                        throw new NotImplementedException("CSL_Value_Value:" + type.Name);
                }
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return type.Name + "|" + value_value.ToString();
            }
#endif
        }

        public class CSL_Value_ScriptValue : CSL_Value
        {
            ~CSL_Value_ScriptValue()
            {
                if (value_type != null)
                    value_type.Deconstruction(value_value);
            }
            public override CSL_Type type
            {
                get { return value_type; }
            }
            public SType value_type;

            public SInstance value_value;
            public override object Value
            {
                get
                {
                    return value_value;
                }
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return type.Name + "|" + value_value.ToString();
            }
#endif
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value v = new CSL_Content.Value();
                v.type = type;
                v.value = value_value;
                content.OutStack(this);
                return v;
            }
        }

        public class CSL_Value_Null : CSL_Value
        {
            public override CSL_Type type
            {
                get { return null; }
            }
            public override object Value
            {
                get
                {
                    return null;
                }
            }
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value v = new CSL_Content.Value();
                v.type = type;
                v.value = null;
                content.OutStack(this);
                return v;
            }
            public override void Read(CSL_StreamRead stream)
            {
                stream.Read(out line);
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return "null";
            }
#endif
        }

        public class CSL_Value_Object : CSL_Value
        {

            public object value_value;
            public override object Value
            {
                get
                {
                    return value_value;
                }
            }
            public override CSL_Content.Value Execute(CSL_Content content)
            {
                content.InStack(this);
                CSL_Content.Value v = new CSL_Content.Value();
                v.type = type;
                v.value = value_value;
                content.OutStack(this);
                return v;
            }
#if UNITY_EDITOR
            public override string ToString()
            {
                return value_value.ToString();
            }
#endif
        }
    }
}