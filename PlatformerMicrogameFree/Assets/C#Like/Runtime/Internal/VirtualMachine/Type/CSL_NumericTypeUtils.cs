/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_NumericTypeUtils
        {
            public static object TryConvertTo<OriginalType>(object src, CSL_Type targetType, out bool convertSuccess) where OriginalType : struct
            {
                convertSuccess = true;
                try
                {
                    if (typeof(JSONData) == targetType)
                    {
                        Type t = targetType;
                        var m = t.GetMethod("op_Implicit", new Type[] { typeof(OriginalType) });
                        return m.Invoke(null, new object[] { src });
                    }
                    decimal srcValue = GetDecimalValue(typeof(OriginalType), src);
                    return Decimal2TargetType(targetType, srcValue);
                }
                catch (Exception)
                {
                    convertSuccess = false;
                    return null;
                }
            }
            public static object Math2Value<LeftType>(string opCode, object left, CSL_Content.Value right, out CSL_Type returntype, out bool math2ValueSuccess) where LeftType : struct
            {

                math2ValueSuccess = true;

                try
                {
                    decimal leftValue = GetDecimalValue(typeof(LeftType), left);
                    decimal rightValue = GetDecimalValue(right.type, right.value);
                    decimal finalValue = 0;

                    switch (opCode)
                    {
                        case "+":
                            finalValue = leftValue + rightValue;
                            break;
                        case "-":
                            finalValue = leftValue - rightValue;
                            break;
                        case "*":
                            finalValue = leftValue * rightValue;
                            break;
                        case "/":
                            finalValue = leftValue / rightValue;
                            break;
                        case "%":
                            finalValue = leftValue % rightValue;
                            break;
                        default:
                            throw new Exception("Invalid math operation::opCode = " + opCode);
                    }

                    returntype = GetReturnType_Math2Value(typeof(LeftType), right.type);
                    return Decimal2TargetType(returntype, finalValue);

                }
                catch (Exception)
                {
                    math2ValueSuccess = false;
                    returntype = null;
                    return null;
                }
            }

            public static bool MathLogic<LeftType>(TokenLogic logicCode, object left, CSL_Content.Value right, out bool mathLogicSuccess)
            {
                mathLogicSuccess = true;
                try
                {
                    decimal leftValue = GetDecimalValue(typeof(LeftType), left);
                    decimal rightValue = GetDecimalValue(right.type, right.value);

                    switch (logicCode)
                    {
                        case TokenLogic.Equal:
                            return leftValue == rightValue;
                        case TokenLogic.Less:
                            return leftValue < rightValue;
                        case TokenLogic.LessEqual:
                            return leftValue <= rightValue;
                        case TokenLogic.More:
                            return leftValue > rightValue;
                        case TokenLogic.MoreEqual:
                            return leftValue >= rightValue;
                        case TokenLogic.NotEqual:
                            return leftValue != rightValue;
                        default:
                            throw new Exception("Invalid logic operation::logicCode = " + logicCode.ToString());
                    }
                }
                catch (Exception)
                {
                    mathLogicSuccess = false;
                    return false;
                }
            }
            public static decimal GetDecimalValue(Type type, object value)
            {
                switch (type.Name)
                {
                    case "Double": return (decimal)Convert.ToDouble(value);
                    case "Single": return (decimal)Convert.ToSingle(value);
                    case "Int64": return Convert.ToInt64(value);
                    case "UInt64": return Convert.ToUInt64(value);
                    case "Int32": return Convert.ToInt32(value);
                    case "UInt32": return Convert.ToUInt32(value);
                    case "Int16": return Convert.ToInt16(value);
                    case "UInt16": return Convert.ToUInt16(value);
                    case "Byte": return Convert.ToByte(value);
                    case "SByte": return Convert.ToSByte(value);
                    case "Char": return Convert.ToChar(value);
                    case "JSONData":
                        {
                            object v = (value as JSONData).Value;
                            return GetDecimalValue(v.GetType(), v);
                        }
                    default:
                        throw new Exception("unknown decimal type..." + type.Name);
                }
            }

            public static object Decimal2TargetType(Type type, decimal value)
            {
                switch(type.Name)
                {
                    case "Double": return (double)value;
                    case "Single": return (float)value;
                    case "Int64": return (long)value;
                    case "UInt64": return (ulong)value;
                    case "Int32": return (int)value;
                    case "UInt32": return (uint)value;
                    case "Int16": return (short)value;
                    case "UInt16": return (ushort)value;
                    case "Byte": return (byte)value;
                    case "SByte": return (sbyte)value;
                    case "Char": return (char)value;
                    default: throw new Exception("unknown target type...:" + type.Name);
                }

            }

            public static Type GetReturnType_Math2Value(Type leftType, Type rightType)
            {

                int ltIndex = _TypeList.IndexOf(leftType);
                int rtIndex = _TypeList.IndexOf(rightType);

                //0. double and float
                if (ltIndex == T_Double || rtIndex == T_Double)
                {
                    return typeof(double);
                }
                if (ltIndex == T_Float || rtIndex == T_Float)
                {
                    return typeof(float);
                }

                //1. ulong
                if (ltIndex == T_ULong || rtIndex == T_ULong)
                {
                    return typeof(ulong);
                }

                //2. long
                if (ltIndex == T_Long || rtIndex == T_Long)
                {
                    return typeof(long);
                }

                //3. int and uint are long.
                if ((ltIndex == T_Int && rtIndex == T_UInt) || (ltIndex == T_UInt && rtIndex == T_Int))
                {
                    return typeof(long);
                }

                //4. uint and none int are uint.
                if ((ltIndex == T_UInt && rtIndex != T_Int) || (rtIndex == T_UInt && ltIndex != T_Int))
                {
                    return typeof(uint);
                }

                //5 int
                return typeof(int);
            }

            private static List<Type> _TypeList = new List<Type>(new Type[]{
                        typeof(double),
                        typeof(float),
                        typeof(long),
                        typeof(ulong),
                        typeof(int),
                        typeof(uint),
                        typeof(short),
                        typeof(ushort),
                        typeof(sbyte),
                        typeof(byte),
                        typeof(char)
                    });

            private const int T_Double = 0;
            private const int T_Float = 1;
            private const int T_Long = 2;
            private const int T_ULong = 3;
            private const int T_Int = 4;
            private const int T_UInt = 5;
        }
    }
}