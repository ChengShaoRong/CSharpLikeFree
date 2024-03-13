/*
 *           C#Like
 * Copyright © 2022-2024 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;


namespace CSharpLike
{
    namespace Internal
    {
        public enum TokenLogic : byte
        {
            Less,          // <
            LessEqual,     // <=
            More,          // >
            MoreEqual,     // >=
            Equal,         // ==
            NotEqual       // !=
        }

        public class MethodCache
        {
            public CSL_Type returnType = null;
            public System.Reflection.MethodInfo info = null;
            public bool cachefail = false;
            public bool slow = false;
        }

        public abstract class CSL_TypeFunctionBase
        {
            public abstract CSL_Content.Value New(CSL_Content content, List<CSL_Content.Value> _params);
            public abstract CSL_Content.Value StaticCall(CSL_Content content, string function, List<CSL_Content.Value> _params);
            public abstract CSL_Content.Value StaticCall(CSL_Content content, string function, List<CSL_Content.Value> _params, MethodCache cache);
            public virtual CSL_Content.Value StaticCallCache(CSL_Content content, List<CSL_Content.Value> _params, MethodCache cache)
            {
                throw new NotImplementedException();
            }
            public abstract bool StaticValueSet(CSL_Content content, string valuename, object value);
            public abstract CSL_Content.Value StaticValueGet(CSL_Content content, string valuename);

            public abstract CSL_Content.Value MemberCall(CSL_Content content, object object_this, string func, List<CSL_Content.Value> _params);
            public abstract CSL_Content.Value MemberCall(CSL_Content content, object object_this, string func, List<CSL_Content.Value> _params, MethodCache cache);
            public virtual CSL_Content.Value MemberCallCache(CSL_Content content, object object_this, List<CSL_Content.Value> _params, MethodCache cache)
            {
                throw new NotImplementedException();
            }
            public abstract CSL_Content.Value MemberValueGet(CSL_Content content, object object_this, string valuename);
            public abstract bool MemberValueSet(CSL_Content content, object object_this, string valuename, object value);

            public abstract CSL_Content.Value IndexGet(CSL_Content content, object object_this, object key);
            public abstract void IndexSet(CSL_Content content, object object_this, object key, object value);
        }

        public class CSL_Type
        {
            private CSL_Type(Type type)
            {
                this.type = type;
            }
            private CSL_Type(SType type)
            {
                this.stype = type;
            }
            public static implicit operator Type(CSL_Type m)
            {
                if (m == null) return null;

                return m.type;
            }
            public static implicit operator SType(CSL_Type m)
            {
                if (m == null) return null;

                return m.stype;
            }
            static Dictionary<Type, CSL_Type> types = new Dictionary<Type, CSL_Type>();
            static Dictionary<SType, CSL_Type> stypes = new Dictionary<SType, CSL_Type>();

            public static implicit operator CSL_Type(Type type)
            {
                CSL_Type retT;
                bool bRet = types.TryGetValue(type, out retT);
                if (bRet)
                    return retT;
                else
                {
                    var ct = new CSL_Type(type);
                    types[type] = ct;
                    return ct;
                }
            }
            public static implicit operator CSL_Type(SType type)
            {
                CSL_Type retST;
                bool bRet = stypes.TryGetValue(type, out retST);
                if (bRet)
                    return retST;
                else
                {
                    var ct = new CSL_Type(type);
                    stypes[type] = ct;
                    return ct;
                }
            }
            public bool isBaseObject
            {
                get
                {
                    if (type != null)
                    {
                        return type == typeof(object);
                    }
                    return false;
                }
            }
            public bool bCoroutine
            {
                get
                {
                    return type != null && type == typeof(System.Collections.IEnumerator);
                }
            }

            public override string ToString()
            {
                if (type != null) return type.ToString();
                return stype.ToString();
            }
            Type type;
            SType stype = null;
            public string Name
            {
                get
                {
                    if (type != null) return type.Name;
                    else return stype.Name;
                }
            }
            public string NameSpace
            {
                get
                {
                    if (type != null) return type.Namespace;
                    else return stype.Namespace;
                }
            }
            public string FullName
            {
                get
                {
                    if (type != null) return type.FullName;
                    else return stype.FullName;
                }
            }
            public bool bInterface
            {
                get
                {
                    if (type != null)
                        return type.IsInterface;
                    else
                        return stype.bInterface;
                }
            }
        }

        public class CSL_TypeBase
        {
            public virtual string Keyword
            {
                get;
                protected set;
            }
            public virtual string Namespace
            {
                get;
                protected set;
            }
            public virtual string FullName
            {
                get;
            }
            public virtual CSL_Type type
            {
                get;
                protected set;
            }
            public virtual object DefValue
            {
                get
                {
                    return null;
                }
            }

            public virtual object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                throw new NotImplementedException();
            }
            public virtual object Math2Value(CSL_Content content, string code, object left, CSL_Content.Value right, out CSL_Type returntype)
            {
                throw new NotImplementedException();
            }
            public virtual bool MathLogic(CSL_Content content, TokenLogic code, object left, CSL_Content.Value right)
            {
                throw new NotImplementedException();
            }
            public virtual CSL_TypeFunctionBase function
            {
                get
                {
                    throw new NotImplementedException();
                }
                protected set
                {
                    throw new NotImplementedException();
                }
            }
        }

        public abstract class CSL_TypeDeleBase : RegHelper_Type
        {
            public CSL_TypeDeleBase(Type type, string setkeyword, bool dele) : base(type, setkeyword, dele) { }
            public abstract Delegate CreateDelegate(CSL_VirtualMachine vm, CSL_DeleFunction lambda);
            public abstract Delegate CreateDelegate(CSL_VirtualMachine vm, CSL_DeleLambda lambda);
            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                CSL_TypeDeleBase dele = content.vm.GetType(targetType) as CSL_TypeDeleBase;
                if (src is CSL_DeleFunction)
                    return dele.CreateDelegate(content.vm, src as CSL_DeleFunction);
                else if (src is CSL_DeleLambda)
                    return dele.CreateDelegate(content.vm, src as CSL_DeleLambda);
                return base.ConvertTo(content, src, targetType);
            }
        }
    }
}