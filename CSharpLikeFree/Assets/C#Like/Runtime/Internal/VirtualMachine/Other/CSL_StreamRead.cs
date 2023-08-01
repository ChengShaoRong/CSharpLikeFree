/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;


namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_StreamRead
        {
            public CSL_StreamRead(byte[] buff)
            {
                this.buff = buff;
                pos = 0;
            }
            public CSL_StreamRead(int maxSize = 1024 * 1024)
            {
                buff = new byte[maxSize];
                pos = 0;
            }
            public byte[] buff;
            public int pos;
            public byte ReadByte()
            {
                byte ret = buff[pos];
                ++pos;
                return ret;
            }
            public void Read(out byte value)
            {
                value = buff[pos];
                ++pos;
            }
            public bool ReadBoolean()
            {
                bool ret = buff[pos] > 0;
                ++pos;
                return ret;
            }
            public sbyte ReadSByte()
            {
                sbyte ret = (sbyte)buff[pos];
                pos += 1;
                return ret;
            }
            public void Read(out bool value)
            {
                value = buff[pos] > 0;
                ++pos;
            }
            public char ReadChar()
            {
                char ret = BitConverter.ToChar(buff, pos);
                pos += 2;
                return ret;
            }
            public void Read(out char value)
            {
                value = BitConverter.ToChar(buff, pos);
                pos += 2;
            }
            public short ReadInt16()
            {
                short ret = BitConverter.ToInt16(buff, pos);
                pos += 2;
                return ret;
            }
            public void Read(out short value)
            {
                value = BitConverter.ToInt16(buff, pos);
                pos += 2;
            }
            public ushort ReadUInt16()
            {
                ushort ret = BitConverter.ToUInt16(buff, pos);
                pos += 2;
                return ret;
            }
            public void Read(out ushort value)
            {
                value = BitConverter.ToUInt16(buff, pos);
                pos += 2;
            }
            public int ReadInt32()
            {
                int ret = BitConverter.ToInt32(buff, pos);
                pos += 4;
                return ret;
            }
            public void Read(out int value)
            {
                value = BitConverter.ToInt32(buff, pos);
                pos += 4;
            }
            public uint ReadUInt32()
            {
                uint ret = BitConverter.ToUInt32(buff, pos);
                pos += 4;
                return ret;
            }
            public void Read(out uint value)
            {
                value = BitConverter.ToUInt32(buff, pos);
                pos += 4;
            }
            public long ReadInt64()
            {
                long ret = BitConverter.ToInt64(buff, pos);
                pos += 8;
                return ret;
            }
            public void Read(out long value)
            {
                value = BitConverter.ToInt64(buff, pos);
                pos += 8;
            }
            public ulong ReadUInt64()
            {
                ulong ret = BitConverter.ToUInt64(buff, pos);
                pos += 8;
                return ret;
            }
            public void Read(out ulong value)
            {
                value = BitConverter.ToUInt64(buff, pos);
                pos += 8;
            }
            public double ReadDouble()
            {
                double ret = BitConverter.ToDouble(buff, pos);
                pos += 8;
                return ret;
            }
            public void Read(out double value)
            {
                value = BitConverter.ToDouble(buff, pos);
                pos += 8;
            }
            public float ReadSingle()
            {
                float ret = BitConverter.ToSingle(buff, pos);
                pos += 4;
                return ret;
            }
            public void Read(out float value)
            {
                value = BitConverter.ToSingle(buff, pos);
                pos += 4;
            }
            public byte[] ReadBytes()
            {
                int length = ReadInt32();
                if (length > 0)
                {
                    byte[] ret = new byte[length];
                    Array.Copy(buff, pos, ret, 0, length);
                    return ret;
                }
                else
                    return new byte[0];
            }
            public void Read(out byte[] value)
            {
                value = ReadBytes();
            }
            public string ReadString()
            {
                ushort length = ReadUInt16();
                if (length > 0)
                {
                    string ret = Encoding.UTF8.GetString(buff, pos, length);
                    pos += length;
                    return ret;
                }
                else
                    return "";
            }
            public void Read(out string value)
            {
                value = ReadString();
            }
            public ushort[] ReadUInt16s()
            {
                int length = ReadInt32();
                ushort[] ret = new ushort[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadUInt16();
                return ret;
            }
            public void Read(out ushort[] value)
            {
                value = ReadUInt16s();
            }
            public short[] ReadInt16s()
            {
                int length = ReadInt32();
                short[] ret = new short[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadInt16();
                return ret;
            }
            public void Read(out short[] value)
            {
                value = ReadInt16s();
            }
            public string[] ReadStrings()
            {
                int length = ReadInt32();
                string[] ret = new string[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadString();
                return ret;
            }
            public void Read(out string[] value)
            {
                value = ReadStrings();
            }
            public uint[] ReadUInt32s()
            {
                int length = ReadInt32();
                uint[] ret = new uint[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadUInt32();
                return ret;
            }
            public void Read(out uint[] value)
            {
                value = ReadUInt32s();
            }
            public int[] ReadInt32s()
            {
                int length = ReadInt32();
                int[] ret = new int[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadInt32();
                return ret;
            }
            public void Read(out int[] value)
            {
                value = ReadInt32s();
            }
            public ulong[] ReadUInt64s()
            {
                int length = ReadInt32();
                ulong[] ret = new ulong[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadUInt64();
                return ret;
            }
            public void Read(out ulong[] value)
            {
                value = ReadUInt64s();
            }
            public long[] ReadInt64s()
            {
                int length = ReadInt32();
                long[] ret = new long[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadInt64();
                return ret;
            }
            public void Read(out long[] value)
            {
                value = ReadInt64s();
            }
            public char[] ReadChars()
            {
                int length = ReadInt32();
                char[] ret = new char[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadChar();
                return ret;
            }
            public void Read(out char[] value)
            {
                value = ReadChars();
            }
            public bool[] ReadBooleans()
            {
                int length = ReadInt32();
                bool[] ret = new bool[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadBoolean();
                return ret;
            }
            public void Read(out bool[] value)
            {
                value = ReadBooleans();
            }
            public decimal[] ReadDecimals()
            {
                int length = ReadInt32();
                decimal[] ret = new decimal[length];
                for (int i = 0; i < length; i++)
                    ret[i] = (decimal)ReadDouble();
                return ret;
            }
            public void Read(out decimal[] value)
            {
                value = ReadDecimals();
            }
            public float[] ReadSingles()
            {
                int length = ReadInt32();
                float[] ret = new float[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadSingle();
                return ret;
            }
            public void Read(out float[] value)
            {
                value = ReadSingles();
            }
            public double[] ReadDoubles()
            {
                int length = ReadInt32();
                double[] ret = new double[length];
                for (int i = 0; i < length; i++)
                    ret[i] = ReadDouble();
                return ret;
            }
            public void Read(out double[] value)
            {
                value = ReadDoubles();
            }
            public List<string> ReadStringList()
            {
                int length = ReadInt32();
                List<string> ret = new List<string>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString());
                return ret;
            }
            public void Read(out List<string> value)
            {
                value = ReadStringList();
            }
            public List<ushort> ReadUInt16List()
            {
                int length = ReadInt32();
                List<ushort> ret = new List<ushort>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadUInt16());
                return ret;
            }
            public void Read(out List<ushort> value)
            {
                value = ReadUInt16List();
            }
            public List<short> ReadInt16List()
            {
                int length = ReadInt32();
                List<short> ret = new List<short>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt16());
                return ret;
            }
            public void Read(out List<short> value)
            {
                value = ReadInt16List();
            }
            public List<uint> ReadUInt32List()
            {
                int length = ReadInt32();
                List<uint> ret = new List<uint>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadUInt32());
                return ret;
            }
            public void Read(out List<uint> value)
            {
                value = ReadUInt32List();
            }
            public List<int> ReadInt32List()
            {
                int length = ReadInt32();
                List<int> ret = new List<int>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32());
                return ret;
            }
            public void Read(out List<int> value)
            {
                value = ReadInt32List();
            }
            public List<ulong> ReadUInt64List()
            {
                int length = ReadInt32();
                List<ulong> ret = new List<ulong>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadUInt64());
                return ret;
            }
            public void Read(out List<ulong> value)
            {
                value = ReadUInt64List();
            }
            public List<long> ReadInt64List()
            {
                int length = ReadInt32();
                List<long> ret = new List<long>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt64());
                return ret;
            }
            public void Read(out List<long> value)
            {
                value = ReadInt64List();
            }
            public List<char> ReadCharList()
            {
                int length = ReadInt32();
                List<char> ret = new List<char>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadChar());
                return ret;
            }
            public void Read(out List<char> value)
            {
                value = ReadCharList();
            }
            public List<float> ReadSingleList()
            {
                int length = ReadInt32();
                List<float> ret = new List<float>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadSingle());
                return ret;
            }
            public void Read(out List<float> value)
            {
                value = ReadSingleList();
            }
            public List<double> ReadDoubleList()
            {
                int length = ReadInt32();
                List<double> ret = new List<double>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadDouble());
                return ret;
            }
            public void Read(out List<double> value)
            {
                value = ReadDoubleList();
            }
            public List<byte> ReadByteList()
            {
                int length = ReadInt32();
                List<byte> ret = new List<byte>();
                for (int i = 0; i < length; i++)
                    ret.Add(buff[pos+i]);
                pos += length;
                return ret;
            }
            public void Read(out List<byte> value)
            {
                value = ReadByteList();
            }
            public Dictionary<int, int> ReadInt32Int32Dict()
            {
                int length = ReadInt32();
                Dictionary<int, int> ret = new Dictionary<int, int>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadInt32());
                return ret;
            }
            public void Read(out Dictionary<int, int> value)
            {
                value = ReadInt32Int32Dict();
            }
            public Dictionary<int, uint> ReadInt32UInt32Dict()
            {
                int length = ReadInt32();
                Dictionary<int, uint> ret = new Dictionary<int, uint>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadUInt32());
                return ret;
            }
            public void Read(out Dictionary<int, uint> value)
            {
                value = ReadInt32UInt32Dict();
            }
            public Dictionary<int, short> ReadInt32Int16Dict()
            {
                int length = ReadInt32();
                Dictionary<int, short> ret = new Dictionary<int, short>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadInt16());
                return ret;
            }
            public void Read(out Dictionary<int, short> value)
            {
                value = ReadInt32Int16Dict();
            }
            public Dictionary<int, bool> ReadInt32BooleanDict()
            {
                int length = ReadInt32();
                Dictionary<int, bool> ret = new Dictionary<int, bool>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadBoolean());
                return ret;
            }
            public void Read(out Dictionary<int, bool> value)
            {
                value = ReadInt32BooleanDict();
            }
            public Dictionary<int, ushort> ReadInt32UInt16Dict()
            {
                int length = ReadInt32();
                Dictionary<int, ushort> ret = new Dictionary<int, ushort>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadUInt16());
                return ret;
            }
            public void Read(out Dictionary<int, ushort> value)
            {
                value = ReadInt32UInt16Dict();
            }
            public Dictionary<int, long> ReadInt32Int64Dict()
            {
                int length = ReadInt32();
                Dictionary<int, long> ret = new Dictionary<int, long>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadInt64());
                return ret;
            }
            public void Read(out Dictionary<int, long> value)
            {
                value = ReadInt32Int64Dict();
            }
            public Dictionary<int, ulong> ReadInt32UInt64Dict()
            {
                int length = ReadInt32();
                Dictionary<int, ulong> ret = new Dictionary<int, ulong>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadUInt64());
                return ret;
            }
            public void Read(out Dictionary<int, ulong> value)
            {
                value = ReadInt32UInt64Dict();
            }
            public Dictionary<int, float> ReadInt32SingleDict()
            {
                int length = ReadInt32();
                Dictionary<int, float> ret = new Dictionary<int, float>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadSingle());
                return ret;
            }
            public void Read(out Dictionary<int, float> value)
            {
                value = ReadInt32SingleDict();
            }
            public Dictionary<int, byte> ReadInt32ByteDict()
            {
                int length = ReadInt32();
                Dictionary<int, byte> ret = new Dictionary<int, byte>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadByte());
                return ret;
            }
            public void Read(out Dictionary<int, byte> value)
            {
                value = ReadInt32ByteDict();
            }
            public Dictionary<int, string> ReadInt32StringDict()
            {
                int length = ReadInt32();
                Dictionary<int, string> ret = new Dictionary<int, string>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadString());
                return ret;
            }
            public void Read(out Dictionary<int, string> value)
            {
                value = ReadInt32StringDict();
            }
            public Dictionary<int, byte[]> ReadInt32BytesDict()
            {
                int length = ReadInt32();
                Dictionary<int, byte[]> ret = new Dictionary<int, byte[]>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadInt32(), ReadBytes());
                return ret;
            }
            public void Read(out Dictionary<int, byte[]> value)
            {
                value = ReadInt32BytesDict();
            }
            public Dictionary<string, int> ReadStringInt32Dict()
            {
                int length = ReadInt32();
                Dictionary<string, int> ret = new Dictionary<string, int>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadInt32());
                return ret;
            }
            public void Read(out Dictionary<string, int> value)
            {
                value = ReadStringInt32Dict();
            }
            public Dictionary<string, uint> ReadStringUInt32Dict()
            {
                int length = ReadInt32();
                Dictionary<string, uint> ret = new Dictionary<string, uint>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadUInt32());
                return ret;
            }
            public void Read(out Dictionary<string, uint> value)
            {
                value = ReadStringUInt32Dict();
            }
            public Dictionary<string, short> ReadStringInt16Dict()
            {
                int length = ReadInt32();
                Dictionary<string, short> ret = new Dictionary<string, short>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadInt16());
                return ret;
            }
            public void Read(out Dictionary<string, bool> value)
            {
                value = ReadStringBooleanDict();
            }
            public Dictionary<string, bool> ReadStringBooleanDict()
            {
                int length = ReadInt32();
                Dictionary<string, bool> ret = new Dictionary<string, bool>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadBoolean());
                return ret;
            }
            public void Read(out Dictionary<string, short> value)
            {
                value = ReadStringInt16Dict();
            }
            public Dictionary<string, ushort> ReadStringUInt16Dict()
            {
                int length = ReadInt32();
                Dictionary<string, ushort> ret = new Dictionary<string, ushort>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadUInt16());
                return ret;
            }
            public void Read(out Dictionary<string, ushort> value)
            {
                value = ReadStringUInt16Dict();
            }
            public Dictionary<string, long> ReadStringInt64Dict()
            {
                int length = ReadInt32();
                Dictionary<string, long> ret = new Dictionary<string, long>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadInt64());
                return ret;
            }
            public void Read(out Dictionary<string, long> value)
            {
                value = ReadStringInt64Dict();
            }
            public Dictionary<string, ulong> ReadStringUInt64Dict()
            {
                int length = ReadInt32();
                Dictionary<string, ulong> ret = new Dictionary<string, ulong>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadUInt64());
                return ret;
            }
            public void Read(out Dictionary<string, ulong> value)
            {
                value = ReadStringUInt64Dict();
            }
            public Dictionary<string, float> ReadStringSingleDict()
            {
                int length = ReadInt32();
                Dictionary<string, float> ret = new Dictionary<string, float>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadSingle());
                return ret;
            }
            public void Read(out Dictionary<string, float> value)
            {
                value = ReadStringSingleDict();
            }
            public Dictionary<string, byte> ReadStringByteDict()
            {
                int length = ReadInt32();
                Dictionary<string, byte> ret = new Dictionary<string, byte>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadByte());
                return ret;
            }
            public void Read(out Dictionary<string, byte> value)
            {
                value = ReadStringByteDict();
            }
            public Dictionary<string, string> ReadStringStringDict()
            {
                int length = ReadInt32();
                Dictionary<string, string> ret = new Dictionary<string, string>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadString());
                return ret;
            }
            public void Read(out Dictionary<string, string> value)
            {
                value = ReadStringStringDict();
            }
            public Dictionary<string, byte[]> ReadStringBytesDict()
            {
                int length = ReadInt32();
                Dictionary<string, byte[]> ret = new Dictionary<string, byte[]>();
                for (int i = 0; i < length; i++)
                    ret.Add(ReadString(), ReadBytes());
                return ret;
            }
            public void Read(out Dictionary<string, byte[]> value)
            {
                value = ReadStringBytesDict();
            }
            public CSL_Code ReadCode()
            {
                CSL_Code ret;
                CSL_CodeType ct = (CSL_CodeType)ReadByte();
                switch (ct)
                {
                    case CSL_CodeType.Block: ret = new CSL_CodeBlock(); break;
                    case CSL_CodeType.Define: ret = new CSL_CodeDefine(); break;
                    case CSL_CodeType.GetValue: ret = new CSL_CodeGetValue(); break;
                    case CSL_CodeType.SetValue: ret = new CSL_CodeSetValue(); break;
                    case CSL_CodeType.Math_Math2Value: ret = new CSL_CodeMath2Value(); break;
                    case CSL_CodeType.Math_Math2ValueAndOr: ret = new CSL_CodeMath2ValueAndOr(); break;
                    case CSL_CodeType.Math_Math2ValueLogic: ret = new CSL_CodeMath2ValueLogic(); break;
                    case CSL_CodeType.Math_Math3Value: ret = new CSL_CodeMath3Value(); break;
                    case CSL_CodeType.Math_NegativeLogic: ret = new CSL_CodeNegativeLogic(); break;
                    case CSL_CodeType.Math_NegativeValue: ret = new CSL_CodeNegativeValue(); break;
                    case CSL_CodeType.Math_PlusValue: ret = new CSL_CodePlusValue(); break;
                    case CSL_CodeType.Math_NotValue: ret = new CSL_CodeNotValue(); break;
                    case CSL_CodeType.Math_SelfOp: ret = new CSL_CodeSelfOp(); break;
                    case CSL_CodeType.Math_SelfOpWithValue: ret = new CSL_CodeSelfOpWithValue(); break;
                    case CSL_CodeType.Math_TypeCheck: ret = new CSL_CodeTypeCheck(); break;
                    case CSL_CodeType.Math_TypeConvert: ret = new CSL_CodeTypeConvert(); break;
                    case CSL_CodeType.Math_TypeOf: ret = new CSL_CodeTypeOf(); break;
                    case CSL_CodeType.Loop_Break: ret = new CSL_CodeLoopBreak(); break;
                    case CSL_CodeType.Loop_Continue: ret = new CSL_CodeLoopContinue(); break;
                    case CSL_CodeType.Loop_DoWhile: ret = new CSL_CodeLoopDowhile(); break;
                    case CSL_CodeType.Loop_For: ret = new CSL_CodeLoopFor(); break;
                    case CSL_CodeType.Loop_ForEach: ret = new CSL_CodeLoopForEach(); break;
                    case CSL_CodeType.Loop_If: ret = new CSL_CodeLoopIf(); break;
                    case CSL_CodeType.Loop_Return: ret = new CSL_CodeLoopReturn(); break;
                    case CSL_CodeType.Loop_While: ret = new CSL_CodeLoopWhile(); break;
                    case CSL_CodeType.Func_Throw: ret = new CSL_CodeThrow(); break;
                    case CSL_CodeType.Func_Function: ret = new CSL_CodeFunction(); break;
                    case CSL_CodeType.Func_FunctionNew: ret = new CSL_CodeFunctionNew(); break;
                    case CSL_CodeType.Func_FunctionNewArray: ret = new CSL_CodeFunctionNewArray(); break;
                    case CSL_CodeType.Func_IndexFind: ret = new CSL_CodeIndexFind(); break;
                    case CSL_CodeType.Func_IndexSetValue: ret = new CSL_CodeIndexSetValue(); break;
                    case CSL_CodeType.Func_Lambda: ret = new CSL_CodeLambda(); break;
                    case CSL_CodeType.Func_MemberFind: ret = new CSL_CodeMemberFind(); break;
                    case CSL_CodeType.Func_MemberFunction: ret = new CSL_CodeMemberFunction(); break;
                    case CSL_CodeType.Func_MemberMath: ret = new CSL_CodeMemberMath(); break;
                    case CSL_CodeType.Func_SetValue: ret = new CSL_CodeMemberSetValue(); break;
                    case CSL_CodeType.Func_StaticFind: ret = new CSL_CodeStaticFind(); break;
                    case CSL_CodeType.Func_StaticFunction: ret = new CSL_CodeStaticFunction(); break;
                    case CSL_CodeType.Func_StaticMath: ret = new CSL_CodeStaticMath(); break;
                    case CSL_CodeType.Func_StaticSetValue: ret = new CSL_CodeStaticSetValue(); break;
                    case CSL_CodeType.Value_Null: ret = new CSL_Value_Null(); break;
                    case CSL_CodeType.Value_String: ret = new CSL_Value_Value<string>(); break;
                    case CSL_CodeType.Value_Boolean: ret = new CSL_Value_Value<bool>(); break;
                    case CSL_CodeType.Value_UInt32: ret = new CSL_Value_Value<uint>(); break;
                    case CSL_CodeType.Value_Int32: ret = new CSL_Value_Value<int>(); break;
                    case CSL_CodeType.Value_UInt16: ret = new CSL_Value_Value<ushort>(); break;
                    case CSL_CodeType.Value_Int16: ret = new CSL_Value_Value<short>(); break;
                    case CSL_CodeType.Value_SByte: ret = new CSL_Value_Value<sbyte>(); break;
                    case CSL_CodeType.Value_Byte: ret = new CSL_Value_Value<byte>(); break;
                    case CSL_CodeType.Value_Char: ret = new CSL_Value_Value<char>(); break;
                    case CSL_CodeType.Value_Single: ret = new CSL_Value_Value<float>(); break;
                    case CSL_CodeType.Value_Double: ret = new CSL_Value_Value<double>(); break;
                    case CSL_CodeType.Value_Int64: ret = new CSL_Value_Value<long>(); break;
                    case CSL_CodeType.Value_UInt64: ret = new CSL_Value_Value<ulong>(); break;
                    default:
                        throw new NotImplementedException(ct.ToString());
                }
                ret.Read(this);
                return ret;
            }
            public void Read(out CSL_Code value)
            {
                value = ReadCode();
            }
            public void ReadNull(out CSL_Code value)
            {
                value = ReadBoolean() ? ReadCode() : null;
            }
            public void Read(List<CSL_Code> buff)
            {
                ushort count = ReadUInt16();
                for (int i = 0; i < count; i++)
                    buff.Add(ReadCode());
            }
            public void ReadNull(out List<CSL_Code> buff)
            {
                if (ReadBoolean())
                {
                    buff = new List<CSL_Code>();
                    ushort count = ReadUInt16();
                    for (int i = 0; i < count; i++)
                        buff.Add(ReadCode());
                }
                else
                    buff = null;
            }
            public void ReadNull(out List<string> buff)
            {
                if (ReadBoolean())
                {
                    int length = ReadInt32();
                    buff = new List<string>();
                    for (int i = 0; i < length; i++)
                        buff.Add(ReadString());
                }
                else
                    buff = null;
            }
            public void ReadNullEx(List<CSL_Code> buff)
            {
                ushort count = ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    if (ReadBoolean())
                        buff.Add(ReadCode());
                    else
                        buff.Add(null);
                }
            }
            public void Read(List<CSL_TypeBase> buff)
            {
                ushort count = ReadUInt16();
                for (int i = 0; i < count; i++)
                    buff.Add(ReadIType());
            }
            public void Read(Dictionary<string, CSL_Code> buff)
            {
                ushort count = ReadUInt16();
                for (int i = 0; i < count; i++)
                    buff.Add(ReadString(), ReadCode());
            }
            public void Read(Dictionary<string, List<CSL_Code>> buff)
            {
                ushort count = ReadUInt16();
                for (int i = 0; i < count; i++)
                {
                    string key = ReadString();
                    List<CSL_Code> code = new List<CSL_Code>();
                    Read(code);
                    buff.Add(key, code);
                }
            }
            public void Read(out Dictionary<int, List<CSL_Code>> buff)
            {
                if (ReadBoolean())
                {
                    buff = new Dictionary<int, List<CSL_Code>>();
                    ushort count = ReadUInt16();
                    for (int i = 0; i < count; i++)
                    {
                        int key = ReadInt32();
                        List<CSL_Code> code = new List<CSL_Code>();
                        Read(code);
                        buff.Add(key, code);
                    }
                }
                else
                    buff = null;
            }
            string ReadTypeString()
            {
                return ReadBoolean() ? types[ReadUInt16()] : ReadString();
            }
            public CSL_Type ReadType()
            {
                return HotUpdateManager.vm.GetMyIType(ReadTypeString()).type;
            }
            public CSL_TypeBase ReadIType()
            {
                return HotUpdateManager.vm.GetMyIType(ReadTypeString());
            }
            public List<string> types = new List<string>();
            public void Read(out CSL_TypeBase value)
            {
                value = HotUpdateManager.vm.GetMyIType(ReadTypeString());
            }
            public void Read(out CSL_Type value)
            {
                value = HotUpdateManager.vm.GetMyIType(ReadTypeString()).type;
            }
        }
    }
}