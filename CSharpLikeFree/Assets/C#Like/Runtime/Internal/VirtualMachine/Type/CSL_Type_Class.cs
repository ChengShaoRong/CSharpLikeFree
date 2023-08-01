/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CSharpLike
{
    namespace Internal
    {
        public enum FunctionType : uint
        {
            None,
            Pubulic = 1 << 0,
            Private = 1 << 1,
            Internal = 1 << 2,
            Protected = 1 << 3,
            Abstract = 1 << 4,
            Const = 1 << 5,
            Event = 1 << 6,
            Extern = 1 << 7,
            New = 1 << 8,
            Override = 1 << 9,
            Virtual = 1 << 10,
            Static = 1 << 11,
            Coroutine = 1<< 12,
            Unsafe = 1 << 13,
            Volatile = 1 << 14,
            Readonly = 1 << 15,
            Sealed = 1 << 16,
        }
        public class SType : CSL_TypeFunctionBase
        {
            public SType(string keyword, string _namespace, string filename, bool bInterface)
            {
                Name = keyword;
                Namespace = _namespace;
                this.filename = filename;
                this.bInterface = bInterface;
            }
            public string filename
            {
                get;
                set;
            }
            public Dictionary<int, string> fileNames;
            public List<string> attributes;
            public bool IsDefined(string attr)
            {
                return attributes != null && attributes.Contains(attr);
            }
            public string GetFileName(int index)
            {
                string str;
                if (fileNames.TryGetValue(index, out str))
                    return str;
                return filename;
            }
            public CSL_Type GetBaseType(CSL_VirtualMachine vm)
            {
                CSL_TypeBase it = vm.GetTypeByKeywordQuiet(FullName);
                return ((CSL_Type_Class)it).GetBaseType();
            }
            public bool bInterface
            {
                get;
                private set;
            }
            #region impl type
            public string FullName
            {
                get
                {
                    if (string.IsNullOrEmpty(Namespace))
                        return Name;
                    else
                        return Namespace + "." + Name;
                }
            }

            public string Namespace
            {
                get;
                private set;
            }

            public string Name
            {
                get;
                private set;

            }

            #endregion

            #region Script IMPL
            CSL_Content contentMemberCalc = null;
            public CSL_Content.Value New()
            {
                CSL_Content content = HotUpdateManager.vm.CreateContent();
                var v = New(content, new List<CSL_Content.Value>());
                SInstance si = v.value as SInstance;
                content.CallThis = si;
                content.CallType = si.type;
                si.content = content;
                return v;
            }
            public override CSL_Content.Value New(CSL_Content content, List<CSL_Content.Value> _params)
            {
                if (contentMemberCalc == null)
                    contentMemberCalc = new CSL_Content(content.vm);
                if (parents == null)
                {
                    parents = new List<CSL_Type>();
                    CSL_Type p = GetBaseType(content.vm);
                    while(!p.isBaseObject)
                    {
                        parents.Add(p);
                        SType st = p;
                        if (st != null)
                            p = st.GetBaseType(content.vm);
                        else
                        {
                            Type t = p;
                            p = t.BaseType;
                        }
                    } 
                }
                NewStatic(content.vm);
                CSL_Value_ScriptValue sv = new CSL_Value_ScriptValue();
                sv.value_type = this;
                sv.value_value = NewInstance(content, this);
                SInstance si = sv.value_value;
                si.parents = new List<SInstance>();
                List<SInstance> sis = si.parents;
                foreach (CSL_Type p in parents)
                {
                    SType t = p;
                    if (t == null)
                    {
                        Type tParent = p;
                        if (tParent != typeof(object))
                        {
                            si.parentType = tParent;
                            si.parentFunc = (CSL_RegHelper_TypeFunction)content.vm.GetType(p).function;
                            si.parentObj = tParent.Assembly.CreateInstance(tParent.FullName);
                        }
                        break;
                    }
                    else
                    {
                        si.parent = NewInstance(content, t);
                        si = si.parent;
                        sis.Add(si);
                    }
                }
                Function func = GetFunction(FullName);
                //if (parents.Count > 0)
                //{
                //    for (int i = 0; i < parents.Count; i++)
                //    {
                //        SType t = parents[i];
                //        if (t != null)
                //        {
                //            si = sis[i];
                //            foreach (var item in si.members)
                //            {
                //                if (!sv.value_value.members.ContainsKey(item.Key))
                //                    sv.value_value.members.Add(item.Key, item.Value);
                //                for(int j=0; j<i; j++)
                //                {
                //                    SInstance _si = sis[j];
                //                    if (!_si.members.ContainsKey(item.Key))
                //                        _si.members.Add(item.Key, item.Value);
                //                }
                //            }
                //        }
                //    }
                //}
                if (func != null)
                    MemberCall(content, sv.value_value, FullName, _params);
                return CSL_Content.Value.FromICLS_Value(sv);
            }
            public void Deconstruction(SInstance si)
            {
            }
            SInstance NewInstance(CSL_Content content, SType t)
            {
                SInstance si = new SInstance();
                si.type = t;
                foreach (KeyValuePair<string, Member> i in t.members)
                {
                    if (i.Value.bStatic == false)
                    {
                        if (i.Value.defValue == null)
                        {
                            si.members[i.Key] = new CSL_Content.Value();
                            si.members[i.Key].type = i.Value.type.type;
                            si.members[i.Key].value = i.Value.type.DefValue;
                        }
                        else
                        {
                            var value = i.Value.defValue.Execute(contentMemberCalc);
                            if (i.Value.type.type != value.type)
                            {
                                si.members[i.Key] = new CSL_Content.Value();
                                si.members[i.Key].type = i.Value.type.type;
                                si.members[i.Key].value = content.vm.GetType(value.type).ConvertTo(content, value.value, i.Value.type.type);
                            }
                            else
                            {
                                si.members[i.Key] = value;
                            }

                        }
                    }
                }
                return si;
            }

            public Dictionary<string, CSL_Content.Value> GetStaticMemberInstance()
            {
                NewStatic(HotUpdateManager.vm);
                return staticMemberInstance;
            }
            void NewStatic(CSL_VirtualMachine vm)
            {
                if (contentMemberCalc == null)
                    contentMemberCalc = new CSL_Content(vm);
                if (staticMemberInstance == null)
                {
                    staticMemberInstance = new Dictionary<string, CSL_Content.Value>();
                    foreach (var i in members)
                    {
                        if (i.Value.bStatic == true)
                        {
                            if (i.Value.defValue == null)
                            {
                                staticMemberInstance[i.Key] = new CSL_Content.Value();

                                staticMemberInstance[i.Key].type = i.Value.type.type;
                                staticMemberInstance[i.Key].value = i.Value.type.DefValue;
                            }
                            else
                            {
                                var value = i.Value.defValue.Execute(contentMemberCalc);
                                if (i.Value.type.type != value.type)
                                {
                                    staticMemberInstance[i.Key] = new CSL_Content.Value();
                                    staticMemberInstance[i.Key].type = i.Value.type.type;
                                    staticMemberInstance[i.Key].value = vm.GetType(value.type).ConvertTo(contentMemberCalc, value.value, i.Value.type.type);
                                }
                                else
                                {
                                    staticMemberInstance[i.Key] = value;
                                }
                            }
                        }
                    }
                }
            }

            public override CSL_Content.Value StaticCall(CSL_Content contentParent, string function, List<CSL_Content.Value> _params)
            {
                return StaticCall(contentParent, function, _params, null);
            }
            public override CSL_Content.Value StaticCall(CSL_Content contentParent, string function, List<CSL_Content.Value> _params, MethodCache cache)
            {
                if (cache != null)
                {
                    cache.cachefail = true;
                }
                NewStatic(contentParent.vm);
                Function func = this.GetFunction(function);
                if (func != null)
                {
                    if (func.bStatic)
                    {
                        CSL_Content content = new CSL_Content(contentParent.vm);

                        contentParent.InStack(content);
                        content.CallType = this;
                        content.CallThis = null;
                        content.function = func;
                        for (int i = 0; i < func._paramtypes.Count; i++)
                        {
                            if (i< _params.Count)
                                content.DefineAndSet(func._paramnames[i], func._paramtypes[i].type, _params[i].value);
                        }
                        CSL_Content.Value value = null;
                        if (func.code != null)
                        {
                            if (func.code is CSL_CodeBlock)
                                value = func.code.Execute(content);
                            else
                            {
                                content.DepthAdd();
                                value = func.code.Execute(content);
                                content.DepthRemove();
                            }
                            if (value != null)
                                value.breakBlock = CSL_BreakBlock.None;
                        }
                        contentParent.OutStack(content);

                        return value;
                    }
                }
                else if (members.ContainsKey(function))
                {
                    if (members[function].bStatic == true)
                    {
                        Delegate dele = staticMemberInstance[function].value as Delegate;
                        if (dele != null)
                        {
                            CSL_Content.Value value = new CSL_Content.Value();
                            value.type = null;
                            object[] objs = new object[_params.Count];
                            for (int i = 0; i < _params.Count; i++)
                            {
                                objs[i] = _params[i].value;
                            }
                            value.value = dele.DynamicInvoke(objs);
                            if (value.value != null)
                                value.type = value.value.GetType();
                            value.breakBlock = CSL_BreakBlock.None;
                            return value;
                        }
                    }
                }
                throw new NotImplementedException();
            }

            public override CSL_Content.Value StaticValueGet(CSL_Content content, string valuename)
            {
                NewStatic(content.vm);

                if (staticMemberInstance.ContainsKey(valuename))
                {
                    CSL_Content.Value v = new CSL_Content.Value();
                    v.type = staticMemberInstance[valuename].type;
                    v.value = staticMemberInstance[valuename].value;
                    return v;
                }
                throw new NotImplementedException();
            }

            public override bool StaticValueSet(CSL_Content content, string valuename, object value)
            {
                NewStatic(content.vm);
                if (staticMemberInstance.ContainsKey(valuename))
                {
                    if (value != null && value.GetType() != (Type)members[valuename].type.type)
                    {
                        if (value is SInstance)
                        {
                            if ((value as SInstance).type != (SType)members[valuename].type.type)
                            {
                                value = content.vm.GetType((value as SInstance).type).ConvertTo(content, value, members[valuename].type.type);
                            }
                        }
                        else if (value is CSL_DeleEvent)
                        {

                        }
                        else
                        {
                            value = content.vm.GetType(value.GetType()).ConvertTo(content, value, members[valuename].type.type);
                        }
                    }
                    staticMemberInstance[valuename].value = value;
                    return true;
                }
                throw new NotImplementedException();
            }
            public override CSL_Content.Value MemberCall(CSL_Content contentParent, object object_this, string func, List<CSL_Content.Value> _params)
            {
                return MemberCall(contentParent, object_this, func, _params, null);
            }
            public override CSL_Content.Value MemberCall(CSL_Content contentParent, object object_this, string function, List<CSL_Content.Value> _params, MethodCache cache)
            {
                CSL_VirtualMachine.bPrintError = true;
                if (cache != null)
                {
                    cache.cachefail = true;
                }
                SInstance si = object_this as SInstance;
                if (_params == null) _params = new List<CSL_Content.Value>();
                if (si.parentType == typeof(LikeBehaviour))
                {
                    CSL_Content.Value value = null;
                    LikeBehaviour behaviour = (LikeBehaviour)si.parentObj;
                    switch (function)
                    {
                        case "behaviour":
                            value = new CSL_Content.Value();
                            value.type = typeof(LikeBehaviour);
                            value.value = behaviour.behaviour;
                            return value;
                        case "gameObject":
                            value = new CSL_Content.Value();
                            value.type = typeof(GameObject);
                            value.value = behaviour.gameObject;
                            return value;
                        case "transform":
                            value = new CSL_Content.Value();
                            value.type = typeof(Transform);
                            value.value = behaviour.transform;
                            return value;
                        case "enabled":
                            value = new CSL_Content.Value();
                            value.type = typeof(bool);
                            value.value = behaviour.enabled;
                            return value;
                        case "isActiveAndEnabled":
                            value = new CSL_Content.Value();
                            value.type = typeof(bool);
                            value.value = behaviour.isActiveAndEnabled;
                            return value;
                        case "ToString":
                            value = new CSL_Content.Value();
                            value.type = typeof(string);
                            value.value = behaviour.behaviour.ToString();
                            return value;
                        case "GetInt":
                        case "SetInt":
                        case "GetBoolean":
                        case "SetBoolean":
                        case "GetSprite":
                        case "SetSprite":
                        case "GetDouble":
                        case "SetDouble":
                        case "GetFloat":
                        case "SetFloat":
                        case "GetColor":
                        case "SetColor":
                        case "GetVector3":
                        case "SetVector3":
                        case "GetString":
                        case "SetString":
                        case "GetGameObject":
                        case "SetGameObject":
                        case "GetTextAsset":
                        case "SetTextAsset":
                        case "GetMaterial":
                        case "SetMaterial":
                        case "GetTexture":
                        case "SetTexture":
                        case "GetAudioClip":
                        case "SetAudioClip":
                        case "StartCoroutine":
                        case "StopCoroutine":
                        case "StopAllCoroutines":
                        case "MemberCall":
                        case "MemberCallDelay":
                            return contentParent.CallThis.parentFunc.MemberCall(contentParent, si.parentObj, function, _params);
                        default:
                            if (function.StartsWith("GetComponent<"))
                                return contentParent.CallThis.parentFunc.MemberCall(contentParent, si.parentObj, function, _params);
                            break;
                    }
                }
                Function func = GetFunction(function);
                if (func != null)
                {
                    if (func.bStatic == false)
                    {
                        CSL_Content content = new CSL_Content(contentParent.vm);
                        contentParent.InStack(content);
                        content.CallType = this;
                        content.CallThis = si;
                        content.function = func;
                        for (int i = 0; i < func._paramtypes.Count; i++)
                        {
                            if (i< _params.Count)
                                content.DefineAndSet(func._paramnames[i], func._paramtypes[i].type, _params[i].value);
                        }
                        CSL_Content.Value value = null;
                        var funcobj = func;
                        if (bInterface)
                        {
                            content.CallType = si.type;
                            funcobj = si.type.GetFunction(function);
                        }
                        if (funcobj.code != null)
                        {
                            if (funcobj.code is CSL_CodeBlock)
                                value = funcobj.code.Execute(content);
                            else
                            {
                                content.DepthAdd();
                                value = funcobj.code.Execute(content);
                                content.DepthRemove();
                            }
                            if (value != null)
                                value.breakBlock = CSL_BreakBlock.None;
                        }
                        contentParent.OutStack(content);

                        return value;
                    }
                }
                else if (members.ContainsKey(function))
                {
                    if (members[function].bStatic == false)
                    {
                        Delegate dele = si.members[function].value as Delegate;
                        if (dele != null)
                        {
                            CSL_Content.Value value = new CSL_Content.Value();
                            value.type = null;
                            object[] objs = new object[_params.Count];
                            for (int i = 0; i < _params.Count; i++)
                            {
                                objs[i] = _params[i].value;
                            }
                            value.value = dele.DynamicInvoke(objs);
                            if (value.value != null)
                                value.type = value.value.GetType();
                            value.breakBlock = CSL_BreakBlock.None;
                            return value;
                        }
                    }

                }
                if (function == "GetType")
                {
                    CSL_Content.Value value = new CSL_Content.Value();
                    value.type = contentParent.CallType;
                    value.value = contentParent.CallType;
                    return value;
                }
                else
                {
                    if (si.parentObj != null)
                    {
                        return si.parentFunc.MemberCall(contentParent, object_this, function, _params);
                    }
                }

                throw new NotImplementedException();
            }

            public override CSL_Content.Value MemberValueGet(CSL_Content content, object object_this, string valuename)
            {
                SInstance sin = object_this as SInstance;
                if (sin.members.ContainsKey(valuename))
                {
                    CSL_Content.Value v = new CSL_Content.Value();
                    v.type = sin.members[valuename].type;
                    v.value = sin.members[valuename].value;
                    return v;
                }
                if (sin.parentObj != null && sin.parentObj.GetType() == typeof(LikeBehaviour))
                    return sin.parentFunc.MemberValueGet(content, sin.parentObj, valuename);
                throw new NotImplementedException();
            }

            public override bool MemberValueSet(CSL_Content content, object object_this, string valuename, object value)
            {
                SInstance sin = object_this as SInstance;
                if (sin.members.ContainsKey(valuename))
                {
                    if (value != null && value.GetType() != (Type)sin.members[valuename].type)
                    {
                        if (value is SInstance)
                        {
                            if ((value as SInstance).type != (SType)sin.members[valuename].type)
                            {
                                value = content.vm.GetType((value as SInstance).type).ConvertTo(content, value, sin.members[valuename].type);
                            }
                        }
                        else if (value is CSL_DeleEvent)
                        {

                        }
                        else
                        {
                            value = content.vm.GetType(value.GetType()).ConvertTo(content, value, sin.members[valuename].type);
                        }
                    }
                    sin.members[valuename].value = value;
                    return true;
                }
                if (sin.parentObj != null && sin.parentObj.GetType() == typeof(LikeBehaviour))
                    return sin.parentFunc.MemberValueSet(content, sin.parentObj, valuename, value);
                throw new NotImplementedException();
            }

            public override CSL_Content.Value IndexGet(CSL_Content content, object object_this, object key)
            {
                throw new NotImplementedException();
            }

            public override void IndexSet(CSL_Content content, object object_this, object key, object value)
            {
                throw new NotImplementedException();
            }
            #endregion


            public class Function
            {
                public CSL_TypeBase type;
                public List<string> attributes;
                public FunctionType functionType = FunctionType.None;
                public bool bStatic
                {
                    get
                    {
                        return (functionType & FunctionType.Static) > 0;
                    }
                    set
                    {
                        if (value)
                            functionType &= (~FunctionType.Static);
                        else
                            functionType |= FunctionType.Static;
                    }
                }
                public string keyword
                {
                    get;
                    set;
                }
                public List<string> _paramnames = new List<string>();
                public List<CSL_TypeBase> _paramtypes = new List<CSL_TypeBase>();
                public CSL_Code code;
                public int fileIndex;

                public void Read(CSL_StreamRead stream)
                {
                    stream.Read(out type);
                    functionType = (FunctionType)stream.ReadUInt16();
                    keyword = stream.ReadString();
                    stream.Read(out _paramnames);
                    stream.Read(_paramtypes);
                    stream.ReadNull(out code);
                    stream.Read(out fileIndex);
                    stream.ReadNull(out attributes);
                }
            }

            public class Member
            {
                public CSL_TypeBase type;
                public FunctionType functionType = FunctionType.None;
                public List<string> attributes;
                public bool bStatic
                {
                    get
                    {
                        return (functionType & FunctionType.Static) > 0;
                    }
                    set
                    {
                        if (value)
                            functionType &= (~FunctionType.Static);
                        else
                            functionType |= FunctionType.Static;
                    }
                }
                public string keyword
                {
                    get;
                    set;
                }
                public CSL_Code defValue;
                public int fileIndex;

                public void Read(CSL_StreamRead stream)
                {
                    stream.Read(out type);
                    functionType = (FunctionType)stream.ReadUInt16();
                    keyword = stream.ReadString();
                    stream.ReadNull(out defValue);
                    stream.Read(out fileIndex);
                    stream.ReadNull(out attributes);
                }
            }

            public Dictionary<string, Function> functions = new Dictionary<string, Function>();
            public Dictionary<string, Member> members = new Dictionary<string, Member>();
            public Dictionary<string, Dictionary<Type, Delegate>> deles = new Dictionary<string, Dictionary<Type, Delegate>>();
            public Dictionary<string, CSL_Content.Value> staticMemberInstance = null;
            public List<CSL_Type> parents = null;

            Dictionary<string, bool> defineds = new Dictionary<string, bool>();
            public bool IsMemberDefined(string name, string attr)
            {
                bool bDeined;
                if (defineds.TryGetValue(name+"*m*"+attr, out bDeined))
                    return bDeined;
                Member member = GetMember(name);
                bDeined = (member != null && member.attributes != null && member.attributes.Contains(attr));
                defineds[name + "*m*" + attr] = bDeined;
                return bDeined;
            }
            public Member GetMember(string name)
            {
                return GetMember(name, HotUpdateManager.vm);
            }
            public Member GetMember(string name, CSL_VirtualMachine vm)
            {
                Member member;
                if (members.TryGetValue(name, out member))
                    return member;
                CSL_Type ct = GetBaseType(vm);
                if (ct.isBaseObject)
                    return null;
                SType st = ct;
                return st.GetMember(name, vm);
            }
            public Function GetFunction(string strFunctionName)
            {
                Function func;
                if (functions.TryGetValue(strFunctionName, out func))
                    return func;
                return null;
            }

            public override string ToString()
            {
                return FullName;
            }
        }
        public class SInstance
        {
            public SInstance parent;
            public object parentObj;
            public Type parentType;
            public CSL_RegHelper_TypeFunction parentFunc;
            public List<SInstance> parents;
            public SType type;
            public Dictionary<string, CSL_Content.Value> members = new Dictionary<string, CSL_Content.Value>();
            public Dictionary<string, Dictionary<Type, Delegate>> deles = new Dictionary<string, Dictionary<Type, Delegate>>();
            public bool inited = false;
            public CSL_Content content;
            public new SType GetType()
            {
                return type;
            }
            public bool Exist(string funcName)
            {
                return type.GetFunction(funcName) != null;
            }
            public object MemberCall(string funcName)
            {
                try
                {
                    return type.MemberCall(content, this, funcName, null).value;
                }
                catch
                {
                    return null;
                }
            }
        }

        public class CSL_Type_Class : CSL_TypeBase
        {
            public CSL_Type_Class(string keyword, bool bInterface, string filename, string _namespace)
            {
                this.Keyword = keyword;
                this.Namespace = _namespace;
                type = new SType(keyword, _namespace, filename, bInterface);
            }
            public void SetBaseType(List<CSL_TypeBase> types)
            {
                this.types = types;
            }
            public List<CSL_TypeBase> GetBaseTypes()
            {
                return types;
            }
            public CSL_Type GetBaseType()
            {
                if (types != null)
                {
                    foreach (CSL_TypeBase one in types)
                    {
                        if (!one.type.bInterface)
                        {
                            return one.type;
                        }
                    }
                }
                CSL_Type type = typeof(object);
                return type;
            }
            protected List<CSL_TypeBase> types;
            public override string Keyword
            {
                get;
                protected set;
            }
            public override string Namespace
            {
                get;
                protected set;
            }
            public override string FullName
            {
                get
                {
                    return Namespace + "." + Keyword;
                }
            }
            public override CSL_Type type
            {
                get;
                protected set;
            }

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {

                CSL_TypeBase type = content.vm.GetType(targetType);
                if (this.type == type.type || (Type)targetType == typeof(object)) return src;
                if (IsParentType(type))
                    return src;

                throw new NotImplementedException();
            }

            bool IsParentType(CSL_TypeBase type)
            {
                if (types == null || types.Count == 0)
                    return false;
                if (types.Contains(type))
                    return true;
                for(int i=0; i< types.Count; i++)
                {
                    CSL_Type_Class p = (CSL_Type_Class)types[i];
                    if (p != null && p.IsParentType(type))
                        return true;
                }
                return false;
            }

            public override bool MathLogic(CSL_Content content, TokenLogic code, object left, CSL_Content.Value right)
            {
                if (code == TokenLogic.Equal)
                {
                    if (left == null || right.type == null)
                    {
                        return left == right.value;
                    }
                    else
                    {
                        return left == right.value;
                    }
                }
                else if (code == TokenLogic.NotEqual)
                {
                    if (left == null || right.type == null)
                    {
                        return left != right.value;
                    }
                    else
                    {
                        return left != right.value;
                    }
                }
                throw new NotImplementedException();
            }

            public void Read(CSL_StreamRead stream)
            {
                SType st = type;
                int count = stream.ReadInt32();
                if (count > 0)
                {
                    types = new List<CSL_TypeBase>();
                    for (int i = 0; i < count; i++)
                        types.Add(stream.ReadIType());
                }
                stream.Read(out st.fileNames);
                count = stream.ReadInt32();
                for (int i=0; i<count; i++)
                {
                    string key = stream.ReadString();
                    SType.Function func = new SType.Function();
                    func.Read(stream);
                    st.functions.Add(key, func);
                }
                count = stream.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    string key = stream.ReadString();
                    SType.Member member = new SType.Member();
                    member.Read(stream);
                    st.members.Add(key, member);
                }
                stream.ReadNull(out st.attributes);
            }
            public override CSL_TypeFunctionBase function
            {
                get
                {
                    return (SType)type;
                }
            }
        }
    }
}