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
        public class CSL_Content
        {
            public CSL_Content Clone()
            {
                CSL_Content content = new CSL_Content(vm);
                foreach (var c in values)
                {
                    content.values.Add(c.Key, c.Value);
                }
                content.CallThis = CallThis;
                content.CallType = CallType;
                content.member = member;
                content.function = function;

                return content;
            }
            public CSL_VirtualMachine vm;
            public CSL_Content(CSL_VirtualMachine vm, bool bCoroutine = false)
            {
                this.vm = vm;
                stackCode = new Stack<CSL_Code>();
                stackContent = new Stack<CSL_Content>();
            }
            public SType.Function function
            {
                private get;
                set;
            }
            public SType.Member member
            {
                private get;
                set;
            }
            public string DebugValue
            {
                get
                {
                    string strout = "";
                    if (values.Count > 0)
                    {
                        foreach (var v in values)
                            strout += "  " + v.Key + "=" + v.Value.ToString() + "\n";
                    }
                    return "";
                }
            }
            
            public string DebugInfo
            {
                get
                {
                    string strout = "";
                    if (CallType != null)
                    {
                        strout += "class:" + CallType.FullName + " ";
                    }
                    if (function != null)
                    {
                        strout += "function:" + function.keyword + " ";
                        if (CallType != null)
                            strout += "file:" + CallType.GetFileName(function.fileIndex) + " ";
                    }
                    else if (member != null)
                    {
                        strout += "member:" + member.keyword + " ";
                        if (CallType != null && function != null)
                            strout += "file:" + CallType.GetFileName(function.fileIndex) + " ";
                    }
                    if (stackCode.Count > 0)
                        strout += "line:" + stackCode.Peek().line + "";

                    return strout;
                }
            }
            public string StackInfo
            {
                get
                {
                    string str = DebugInfo + "\n";
                    CSL_Content contentParent = parent;
                    while (contentParent != null)
                    {
                        str += contentParent.DebugInfo + "\n";
                        contentParent = contentParent.parent;
                    }
                    return str;
                }
            }
            public Stack<CSL_Code> stackCode
            {
                get;
                private set;
            }
            public Stack<CSL_Content> stackContent
            {
                get;
                private set;
            }
            CSL_Content parent
            {
                get;
                set;
            }
            public void InStack(CSL_Content content)
            {
                if (stackContent.Count > 0 && stackContent.Peek() == content)
                {
                    throw new Exception("InStackContent error");
                }
                content.parent = this;
                stackContent.Push(content);
            }
            public void OutStack(CSL_Content content)
            {
                if (stackContent.Peek() != content)
                {
                    throw new Exception("OutStackContent error:" + content.ToString() + " err:" + stackContent.Peek().ToString());
                }
                stackContent.Pop();
            }
            public void InStack(CSL_Code code)
            {
                if (stackCode.Count > 0 && stackCode.Peek() == code)
                {
                    throw new Exception("InStack error");
                }
                stackCode.Push(code);
            }
            public void OutStack(CSL_Code code)
            {
                if (stackCode.Peek() != code)
                {
                    throw new Exception("OutStack error:" + code.ToString() + " err:" + stackCode.Peek().ToString());
                }
                stackCode.Pop();
            }

            public class Value
            {
                public CSL_Type type;
                public object value;
                public CSL_BreakBlock breakBlock = CSL_BreakBlock.None;
                public Value() { }
                public bool IsTrue => (value is bool) ? (bool)value : (value != null);
                public static Value FromICLS_Value(CSL_Value value)
                {
                    Value v = new Value();
                    v.type = value.type;
                    v.value = value.Value;
                    return v;
                }
                public static Value One
                {
                    get
                    {
                        if (g_one == null)
                        {
                            g_one = new Value();
                            g_one.type = typeof(int);
                            g_one.value = (int)1;
                        }
                        return g_one;
                    }
                }
                public static Value OneMinus
                {
                    get
                    {
                        if (g_oneM == null)
                        {
                            g_oneM = new Value();
                            g_oneM.type = typeof(int);
                            g_oneM.value = (int)-1;
                        }
                        return g_oneM;
                    }
                }
                public static Value Void
                {
                    get
                    {
                        if (g_void == null)
                        {
                            g_void = new Value();
                            g_void.type = typeof(void);
                            g_void.value = null;
                        }
                        return g_void;
                    }
                }
                public static Value Null
                {
                    get
                    {
                        if (g_null == null)
                        {
                            g_null = new Value();
                            g_null.type = typeof(object);
                            g_null.value = null;
                        }
                        return g_null;
                    }
                }
                public static Value False
                {
                    get
                    {
                        if (g_false == null)
                        {
                            g_false = new Value();
                            g_false.type = typeof(bool);
                            g_false.value = false;
                        }
                        return g_false;
                    }
                }
                public static Value True
                {
                    get
                    {
                        if (g_true == null)
                        {
                            g_true = new Value();
                            g_true.type = typeof(bool);
                            g_true.value = true;
                        }
                        return g_true;
                    }
                }
                static Value g_true = null;
                static Value g_false = null;
                static Value g_null = null;
                static Value g_one = null;
                static Value g_oneM = null;
                static Value g_void = null;

                public override string ToString()
                {
                    if (type == null)
                    {
                        return "<null>" + value;
                    }
                    return "<" + type.ToString() + ">" + value;
                }
                public Value(CSL_Type _type, object _value)
                {
                    type = _type;
                    value = _value;
                }

                public bool needConvertTo(Type targetType)
                {
                    return targetType != (Type)type || value is CSL_DeleLambda || (value is JSONData && targetType != typeof(JSONData));
                }
                public bool needConvertTo(System.Reflection.ParameterInfo pi)
                {
                    return needConvertTo(pi.ParameterType);
                }
            }

            public Dictionary<string, Value> values = new Dictionary<string, Value>();
            public void Define(string name, CSL_Type type)
            {
                if (values.ContainsKey(name)) throw new Exception(name + " was define");
                Value v = new Value();
                v.type = type;
                values[name] = v;
                if (tvalues.Count > 0)
                {
                    tvalues.Peek().Add(name);
                }
            }
            public void Set(string name, object value)
            {
                Value retV;
                bool bFind = values.TryGetValue(name, out retV);
                if (!bFind)
                {
                    if (CallType != null)
                    {
                        SType.Member retM = CallType.GetMember(name, vm);
                        if (retM != null)
                        {
                            if (retM.bStatic)
                            {
                                if (CallType.staticMemberInstance.ContainsKey(name))
                                    CallType.staticMemberInstance[name].value = value;
                                else if (CallThis != null && CallThis.members.ContainsKey(name))
                                    CallThis.members[name].value = value;
                            }
                            else
                            {
                                CallThis.members[name].value = value;
                            }
                            return;
                        }
                    }
                    string err = CallType.Name + "\n";
                    foreach (var m in CallType.members)
                    {
                        err += m.Key + ",";
                    }
                    throw new Exception("not define value:" + name + "," + err);

                }
                if ((Type)retV.type == typeof(CSL_Type_Var.var) && value != null)
                    retV.type = value.GetType();
                retV.value = value;
            }

            public void DefineAndSet(string name, CSL_Type type, object value)
            {
                if (values.ContainsKey(name))
                    throw new Exception(type.ToString() + ":" + name + " was defined");
                Value v = new Value();
                v.type = type;
                v.value = value;
                values[name] = v;
                if (tvalues.Count > 0)
                {
                    tvalues.Peek().Add(name);
                }
            }
            public Value Get(string name)
            {
                Value v = GetQuiet(name);
                if (v == null)
                    throw new Exception(name + " not define");
                return v;
            }
            public Value GetQuiet(string name)
            {
                Value retV;
                bool bFind = values.TryGetValue(name, out retV);
                if (bFind)
                    return retV;
                switch (name)
                {
                    case "this":
                        {
                            Value v = new Value();
                            v.type = CallType;
                            v.value = CallThis;
                            return v;
                        }
                    case "behaviour":
                        {
                            Value v = new Value();
                            v.type = typeof(HotUpdateBehaviour);
                            v.value = (CallThis.parentObj as LikeBehaviour).behaviour;
                            return v;
                        }
                    case "gameObject":
                        if (CallThis.parentObj is LikeBehaviour)
                        {
                            Value v = new Value();
                            v.type = typeof(UnityEngine.GameObject);
                            v.value = (CallThis.parentObj as LikeBehaviour).gameObject;
                            return v;
                        }
                        break;
                    case "transform":
                        if (CallThis.parentObj is LikeBehaviour)
                        {
                            Value v = new Value();
                            v.type = typeof(UnityEngine.Transform);
                            v.value = (CallThis.parentObj as LikeBehaviour).transform;
                            return v;
                        }
                        break;
                    case "enabled":
                        if (CallThis.parentObj is LikeBehaviour)
                        {
                            Value v = new Value();
                            v.type = typeof(bool);
                            v.value = (CallThis.parentObj as LikeBehaviour).enabled;
                            return v;
                        }
                        break;
                    case "isActiveAndEnabled":
                        if (CallThis.parentObj is LikeBehaviour)
                        {
                            Value v = new Value();
                            v.type = typeof(bool);
                            v.value = (CallThis.parentObj as LikeBehaviour).isActiveAndEnabled;
                            return v;
                        }
                        break;
                    case "GetType":
                        if (CallThis.parentObj is LikeBehaviour)
                        {
                            Value v = new Value();
                            v.type = CallType;
                            v.value = CallType;
                            return v;
                        }
                        break;
                    default:
                        break;
                }

                SType st = CallType;
                retV = GetQuietByCallType(st, name);
                while (retV == null)
                {
                    if(st == null)
                        UnityEngine.Debug.LogError($"Not exist '{name}'");
                    CSL_Type t = st.GetBaseType(vm);
                    st = t;
                    if (st == null)
                        break;
                    retV = GetQuietByCallType(st, name);
                    if (t.isBaseObject)
                        break;
                }
                return retV;
            }
            Value GetQuietByCallType(SType st, string name)
            {
                if (st == null)
                    return null;
                SType.Member retM;
                bool bFind = st.members.TryGetValue(name, out retM);
                if (bFind)
                {
                    if (retM.bStatic)
                    {
                        return st.staticMemberInstance[name];
                    }
                    else
                    {
                        if (CallThis != null && CallThis.members.ContainsKey(name))
                            return CallThis.members[name];
                    }
                }
                if (st.functions.ContainsKey(name))
                {
                    Value v = new Value();
                    CSL_DeleFunction dele = new CSL_DeleFunction(st, this.CallThis, name);

                    v.value = dele;
                    v.type = typeof(CSL_DeleFunction);
                    return v;

                }
                else
                {
                    foreach (var one in st.functions)
                    {
                        if (one.Key.StartsWith(name) && name.Length < one.Key.Length && one.Key[name.Length] == '(')
                        {
                            Value v = new Value();
                            CSL_DeleFunction dele = new CSL_DeleFunction(st, this.CallThis, one.Key);
                            v.value = dele;
                            v.type = typeof(CSL_DeleFunction);
                            return v;
                        }
                    }
                }
                return null;
            }

            public Stack<List<string>> tvalues = new Stack<List<string>>();
            public void DepthAdd()
            {
                tvalues.Push(new List<string>());
            }
            public void DepthRemove()
            {
                List<string> list = tvalues.Pop();
                foreach (var v in list)
                {
                    values.Remove(v);
                }
            }

            public SType CallType;
            public SInstance CallThis;
        }
    }
}