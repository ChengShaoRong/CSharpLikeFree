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
        public class CSL_RegHelper_TypeFunction : CSL_TypeFunctionBase
        {
            Type type;
            public CSL_RegHelper_TypeFunction(Type type)
            {
                this.type = type;
            }
            public override CSL_Content.Value New(CSL_Content content, List<CSL_Content.Value> _params)
            {

                List<Type> types = new List<Type>();
                List<object> objparams = new List<object>();
                if (_params != null)
                {
                    foreach (var p in _params)
                    {
                        types.Add(p.type);
                        objparams.Add(p.value);
                    }
                }
                CSL_Content.Value value = new CSL_Content.Value();
                value.type = type;
                var con = this.type.GetConstructor(types.ToArray());
                if (con == null)
                {
                    value.value = Activator.CreateInstance(this.type);
                }
                else
                {
                    var ps = con.GetParameters();
                    List<object> newObjs = new List<object>();
                    for (int i = 0; i < ps.Length; i++)
                    {
                        if (IsParams(ps[i]))
                        {
                            //List<object> os = new List<object>();
                            //for (int j = i; j < objparams.Count; j++)
                            //    os.Add(objparams[j]);
                            //newObjs.Add(os.ToArray());
                            Type t = ps[i].ParameterType;
                            if (objparams.Count - i == 1 && objparams[i] != null && objparams[i].GetType() == t)
                            {
                                newObjs.Add(objparams[i]);
                            }
                            else
                            {
                                Array array = Array.CreateInstance(t.Assembly.GetType(t.FullName.Replace("[]", "")), objparams.Count - i);
                                for (int j = i; j < objparams.Count; j++)
                                    array.SetValue(objparams[j], j - i);
                                newObjs.Add(array);
                            }
                        }
                        else
                        {
                            newObjs.Add(objparams[i]);
                        }
                    }
                    object[] objs = newObjs.ToArray();
                    value.value = con.Invoke(objs);
                    for (int i = 0; i < ps.Length && i < _params.Count; i++)
                    {
                        if (ps[i].ParameterType.Name.Contains("&"))
                            _params[i].value = objs[i];
                    }
                }
                return value;
            }
            public override CSL_Content.Value StaticCall(CSL_Content content, string function, List<CSL_Content.Value> _params)
            {
                return StaticCall(content, function, _params, null);
            }
            public override CSL_Content.Value StaticCall(CSL_Content content, string function, List<CSL_Content.Value> _params, MethodCache cache)
            {
                bool needConvert = false;
                List<object> _oparams = new List<object>();
                List<Type> types = new List<Type>();
                bool bEm = false;
                foreach (CSL_Content.Value p in _params)
                {
                    _oparams.Add(p.value);
                    if ((SType)p.type != null)
                    {
                        types.Add(typeof(object));
                    }
                    else
                    {
                        if (p.type == null)
                        {
                            bEm = true;

                        }
                        types.Add(p.type);
                    }
                }
                System.Reflection.MethodInfo targetop = null;
                if (!bEm)
                {
                    targetop = type.GetMethod(function, types.ToArray());
                }
                targetop = type.GetMethod(function, types.ToArray());
                if (targetop == null)
                {
                    if (function[function.Length - 1] == '>')
                    {
                        int sppos = function.IndexOf('<', 0);
                        string tfunc = function.Substring(0, sppos);
                        string strparam = function.Substring(sppos + 1, function.Length - sppos - 2);
                        string[] sf = strparam.Split(',');
                        Type[] gtypes = new Type[sf.Length];
                        for (int i = 0; i < sf.Length; i++)
                        {
                            gtypes[i] = content.vm.GetTypeByKeyword(sf[i]).type;
                        }
                        targetop = FindTMethod(type, tfunc, _params, gtypes);
                    }
                    if (targetop == null && !bEm)
                    {
                        Type ptype = type.BaseType;
                        while (ptype != null)
                        {
                            targetop = ptype.GetMethod(function, types.ToArray());
                            if (targetop != null) break;
                            var t = content.vm.GetType(ptype);
                            try
                            {
                                CSL_Content.Value value = t.function.StaticCall(content, function, _params, cache);
                                if (cache != null)
                                {
                                    cache.info = targetop;
                                    cache.slow = needConvert;
                                    cache.returnType = targetop.ReturnType;
                                }
                                return value;
                            }
                            catch
                            {

                            }
                            ptype = ptype.BaseType;
                        }

                    }
                }
                if (targetop == null)
                {
                    targetop = GetMethodSlow(content, true, function, types, _oparams);
                    needConvert = true;
                }

                if (targetop == null)
                {
                    targetop = type.GetMethod(function);//ignore param type
                    if (targetop == null)
                        throw new Exception("not exist function:" + type.ToString() + "." + function);
                }
                if (cache != null)
                {
                    cache.info = targetop;
                    cache.slow = needConvert;
                    cache.returnType = targetop.ReturnType;
                }


                CSL_Content.Value v = new CSL_Content.Value();

                object[] objs = ModifyParam(targetop, content, _params).ToArray();
                //try
                //{
                    v.value = targetop.Invoke(null, objs);
                //}
                //catch (Exception e)
                //{
                //    foreach(object o in objs)
                //    {
                //        Type ttt = o.GetType();
                //    }
                //    throw;
                //}
                /*//process default value
                var ps = targetop.GetParameters();
                var os = _oparams.ToArray();
                if (ps.Length != os.Length)
                {
                    if (ps.Length > os.Length)
                    {
                        for (int i = os.Length; i< ps.Length; i++)
                        {
                            var p = ps[i];
                            if (p.HasDefaultValue)
                            {
                                _oparams.Add(p.DefaultValue);
                            }
                            else
                            {
                                throw new Exception(type.ToString() + "." + function + ":TargetParameterCountException: Number of parameters specified does not match the expected number. less than!");
                            }
                        }
                    }
                    else
                    {
                        if (ps.Length <= 0 || !IsParams(ps[ps.Length - 1]))
                            throw new Exception(type.ToString() + "." + function + ":TargetParameterCountException: Number of parameters specified does not match the expected number. more than!");
                    }
                }

                List<object> newObjs = new List<object>();
                for (int i = 0; i < ps.Length; i++)
                {
                    object o = _oparams[i];
                    if (o != null && ps[i].ParameterType != o.GetType())
                    {
                        switch(ps[i].ParameterType.Name)
                        {
                            case "Int64": newObjs.Add(Convert.ToInt64(o)); break;
                            case "UInt64": newObjs.Add(Convert.ToUInt64(o)); break;
                            case "Int32": newObjs.Add(Convert.ToInt32(o)); break;
                            case "UInt32": newObjs.Add(Convert.ToUInt32(o)); break;
                            case "Int16": newObjs.Add(Convert.ToInt16(o)); break;
                            case "UInt16": newObjs.Add(Convert.ToInt16(o)); break;
                            case "Byte": newObjs.Add(Convert.ToByte(o)); break;
                            case "SByte": newObjs.Add(Convert.ToSByte(o)); break;
                            case "Single": newObjs.Add(Convert.ToSingle(o)); break;
                            case "Double": newObjs.Add(Convert.ToDouble(o)); break;
                            default: newObjs.Add(o); break;
                        }
                    }
                    else
                        newObjs.Add(o);
                }
                object[] objs = newObjs.ToArray();
                v.value = targetop.Invoke(null, objs);*/
                v.type = targetop.ReturnType;
                return v;
            }
            static bool IsParams(System.Reflection.ParameterInfo param)
            {
                return param.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0;
            }

            public override CSL_Content.Value StaticCallCache(CSL_Content content, List<CSL_Content.Value> _params, MethodCache cache)
            {
                List<object> _oparams = new List<object>();
                List<Type> types = new List<Type>();
                foreach (var p in _params)
                {
                    _oparams.Add(p.value);
                    if ((SType)p.type != null)
                    {
                        types.Add(typeof(object));
                    }
                    else
                    {
                        types.Add(p.type);
                    }
                }
                var targetop = cache.info;
                if (cache.slow)
                {
                    var pp = targetop.GetParameters();
                    for (int i = 0; i < pp.Length; i++)
                    {
                        if (i >= _params.Count)
                        {
                            _oparams.Add(pp[i].DefaultValue);
                        }
                        else
                        {
                            if (pp[i].ParameterType != (Type)_params[i].type)
                            {
                                _oparams[i] = content.vm.GetType(_params[i].type).ConvertTo(content, _oparams[i], pp[i].ParameterType);
                            }
                        }
                    }
                }
                CSL_Content.Value v = new CSL_Content.Value();
                object[] objs = ModifyParam(targetop, content, _params).ToArray();
                //try
                //{
                    v.value = targetop.Invoke(null, objs);
                //}
                //catch(Exception e)
                //{
                //    throw;
                //}
                /*var ps = targetop.GetParameters();
                List<object> newObjs = new List<object>();
                for (int i = 0; i < ps.Length; i++)
                {
                    object o = _oparams[i];
                    if (o != null && ps[i].ParameterType != o.GetType())
                    {
                        switch (ps[i].ParameterType.Name)
                        {
                            case "Int64": newObjs.Add(Convert.ToInt64(o)); break;
                            case "UInt64": newObjs.Add(Convert.ToUInt64(o)); break;
                            case "Int32": newObjs.Add(Convert.ToInt32(o)); break;
                            case "UInt32": newObjs.Add(Convert.ToUInt32(o)); break;
                            case "Int16": newObjs.Add(Convert.ToInt16(o)); break;
                            case "UInt16": newObjs.Add(Convert.ToInt16(o)); break;
                            case "Byte": newObjs.Add(Convert.ToByte(o)); break;
                            case "SByte": newObjs.Add(Convert.ToSByte(o)); break;
                            case "Single": newObjs.Add(Convert.ToSingle(o)); break;
                            case "Double": newObjs.Add(Convert.ToDouble(o)); break;
                            default: newObjs.Add(o); break;
                        }
                    }
                    else
                        newObjs.Add(o);
                }
                object[] objs = newObjs.ToArray();
                v.value = targetop.Invoke(null, objs);*/
                v.type = cache.returnType;
                return v;
            }

            public override CSL_Content.Value StaticValueGet(CSL_Content content, string valuename)
            {
                var v = MemberValueGet(content, null, valuename);
                if (v == null)
                {
                    if (type.BaseType != null)
                    {
                        return content.vm.GetType(type.BaseType).function.StaticValueGet(content, valuename);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    return v;
                }
            }

            public override bool StaticValueSet(CSL_Content content, string valuename, object value)
            {

                bool b = MemberValueSet(content, null, valuename, value);
                if (!b)
                {
                    if (type.BaseType != null)
                    {
                        content.vm.GetType(type.BaseType).function.StaticValueSet(content, valuename, value);
                        return true;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    return b;
                }
            }
            Dictionary<int, System.Reflection.MethodInfo> cacheT;//= new Dictionary<string, System.Reflection.MethodInfo>();
            System.Reflection.MethodInfo FindTMethod(Type type, string func, IList<CSL_Content.Value> _params, Type[] gtypes)
            {
                string hashkey = func + "_" + _params.Count + ":";
                foreach (var p in _params)
                {
                    hashkey += p.type.ToString();
                }
                foreach (var t in gtypes)
                {
                    hashkey += t.ToString();
                }
                int hashcode = hashkey.GetHashCode();
                if (cacheT != null)
                {
                    if (cacheT.ContainsKey(hashcode))
                    {
                        return cacheT[hashcode];
                    }
                }
                var ms = type.GetMethods();
                foreach (var t in ms)
                {
                    if (t.Name == func && t.IsGenericMethodDefinition)
                    {
                        var pp = t.GetParameters();
                        if (pp.Length != _params.Count) continue;
                        bool match = true;
                        for (int i = 0; i < pp.Length; i++)
                        {
                            if (pp[i].ParameterType.IsGenericType) continue;
                            if (pp[i].ParameterType.IsGenericParameter) continue;
                            if (pp[i].ParameterType != (Type)_params[i].type)
                            {
                                match = false;
                                break;
                            }
                        }
                        if (match)
                        {
                            var targetop = t.MakeGenericMethod(gtypes);

                            if (cacheT == null)
                                cacheT = new Dictionary<int, System.Reflection.MethodInfo>();
                            cacheT[hashcode] = targetop;
                            return targetop;
                        }
                    }
                }
                return null;
            }
            public override CSL_Content.Value MemberCall(CSL_Content content, object object_this, string function, List<CSL_Content.Value> _params)
            {
                return MemberCall(content, object_this, function, _params, null);
            }
            public override CSL_Content.Value MemberCall(CSL_Content content, object object_this, string function, List<CSL_Content.Value> _params, MethodCache cache)
            {
                CSL_VirtualMachine.bPrintError = true;
                bool needConvert = false;
                List<Type> types = new List<Type>();
                List<object> _oparams = new List<object>();
                bool bEm = false;
                foreach (CSL_Content.Value p in _params)
                {
                    _oparams.Add(p.value);
                    if ((SType)p.type != null)
                    {
                        types.Add(typeof(object));
                    }
                    else
                    {
                        if (p.type == null)
                        {
                            bEm = true;
                        }
                        types.Add(p.type);
                    }
                }

                System.Reflection.MethodInfo targetop = null;
                if (!bEm)
                {
                    targetop = type.GetMethod(function, types.ToArray());
                }
                CSL_Content.Value v = new CSL_Content.Value();
                if (targetop == null)
                {
                    if (function[function.Length - 1] == '>')
                    {
                        int sppos = function.IndexOf('<', 0);
                        string tfunc = function.Substring(0, sppos);
                        string strparam = function.Substring(sppos + 1, function.Length - sppos - 2);
                        string[] sf = strparam.Split(',');
                        Type[] gtypes = new Type[sf.Length];
                        for (int i = 0; i < sf.Length; i++)
                        {
                            gtypes[i] = content.vm.GetTypeByKeyword(sf[i]).type;
                            if (gtypes[i] == null)
                                throw new Exception($"not exist '{sf[i]}', that may be is a hot update class");
                        }
                        targetop = FindTMethod(type, tfunc, _params, gtypes);
                        var ps1 = targetop.GetParameters();
                        for (int i = 0; i < Math.Min(ps1.Length, _oparams.Count); i++)
                        {
                            if (ps1[i].ParameterType != (Type)_params[i].type)
                            {
                                _oparams[i] = content.vm.GetType(_params[i].type).ConvertTo(content, _oparams[i], ps1[i].ParameterType);
                            }
                        }
                    }
                    else
                    {
                        if (!bEm)
                        {
                            foreach (var s in type.GetInterfaces())
                            {
                                targetop = s.GetMethod(function, types.ToArray());
                                if (targetop != null) break;
                            }
                        }
                        if (targetop == null)
                        {
                            targetop = GetMethodSlow(content, false, function, types, _oparams);
                            needConvert = true;
                        }
                        if (targetop == null)
                        {
                            throw new Exception("not exist function:" + type.ToString() + "." + function);
                        }
                    }
                }
                if (cache != null)
                {
                    cache.info = targetop;
                    cache.slow = needConvert;
                    cache.returnType = targetop.ReturnType;
                }

                if (targetop == null)
                {
                    throw new Exception("not exist function:" + type.ToString() + "." + function);
                }
                object[] objs = ModifyParam(targetop, content, _params).ToArray();
                v.value = targetop.Invoke(object_this, objs);
                /*var ps = targetop.GetParameters();
                List<object> newObjs = new List<object>();
                for (int i = 0; i < ps.Length; i++)
                {
                    if (i < _params.Count)
                    {
                        if (ps[i].ParameterType != _params[i].type)
                            _oparams[i] = content.vm.GetType(_params[i].type).ConvertTo(content, _oparams[i], ps[i].ParameterType);
                        else if (_params[i].type == typeof(UnityEngine.Events.UnityAction) && _oparams[i] != null && _oparams[i].GetType() != typeof(UnityEngine.Events.UnityAction))
                            _oparams[i] = content.vm.GetType(_oparams[i].GetType()).ConvertTo(content, _oparams[i], ps[i].ParameterType);
                    }
                    newObjs.Add(_oparams[i]);
                }
                //try
                //{
                    if (function == "Invoke" && object_this.GetType() == typeof(CSL_DeleLambda))
                    {
                        UnityEngine.Events.UnityAction unityAction = (CSL_DeleLambda)object_this;
                        if (unityAction != null)
                            unityAction.Invoke();
                        else
                        {
                            Action action = (CSL_DeleLambda)object_this;
                            if (action != null)
                                action.Invoke();
                        }
                        v.value = null;
                    }
                    else
                    {
                        object[] objs = newObjs.ToArray();
                        v.value = targetop.Invoke(object_this, objs);
                    }*/
                    v.type = targetop.ReturnType;
                //}
                //catch
                //{
                //    int iiii = 0;
                //}
                return v;
            }

            Dictionary<string, IList<System.Reflection.MethodInfo>> slowCache = null;

            System.Reflection.MethodInfo GetMethodSlow(CSL_Content content, bool bStatic, string funcname, IList<Type> types, IList<object> _params)
            {
                if (slowCache == null)
                {
                    System.Reflection.MethodInfo[] ms = this.type.GetMethods();
                    slowCache = new Dictionary<string, IList<System.Reflection.MethodInfo>>();
                    foreach (var m in ms)
                    {
                        string name = m.IsStatic ? "s=" + m.Name : m.Name;
                        if (slowCache.ContainsKey(name) == false)
                        {
                            slowCache[name] = new List<System.Reflection.MethodInfo>();
                        }
                        slowCache[name].Add(m);
                    }
                }
                IList<System.Reflection.MethodInfo> minfo = null;

                if (!slowCache.TryGetValue(bStatic ? "s=" + funcname : funcname, out minfo))
                    return null;

                foreach (var m in minfo)
                {
                    List<object> myparams = new List<object>(_params);
                    bool match = true;
                    var pp = m.GetParameters();
                    if (pp.Length < types.Count)
                    {
                        match = false;
                        continue;
                    }
                    for (int i = 0; i < pp.Length; i++)
                    {
                        if (i >= types.Count)
                        {
                            if (!pp[i].IsOptional)
                            {
                                match = false;
                                break;
                            }
                            else
                            {
                                myparams.Add(pp[i].DefaultValue);
                                continue;
                            }
                        }
                        else
                        {
                            if (pp[i].ParameterType == types[i]) continue;

                            try
                            {
                                if (types[i] == null && !pp[i].ParameterType.IsValueType)
                                {
                                    continue;
                                }
                                myparams[i] = content.vm.GetType(types[i]).ConvertTo(content, _params[i], pp[i].ParameterType);
                                if (myparams[i] == null)
                                {
                                    match = false;
                                    break;
                                }
                            }
                            catch
                            {
                                match = false;
                                break;
                            }
                        }
                    }
                    if (!match)
                    {
                        continue;
                    }
                    else
                    {
                        for (int i = 0; i < myparams.Count; i++)
                        {
                            if (i < _params.Count)
                            {
                                _params[i] = myparams[i];
                            }
                            else
                            {
                                _params.Add(myparams[i]);
                            }
                        }
                        return m;
                    }
                }

                if (minfo.Count == 1)
                    return minfo[0];
                foreach (var m in minfo)
                {
                    var pp = m.GetParameters();
                    if (pp.Length == _params.Count)
                    {
                        //myparams.Clear();
                        for (int i = 0; i < pp.Length; i++)
                        {
                            _params[i] = content.vm.GetType(types[i]).ConvertTo(content, _params[i], pp[i].ParameterType);
                        }
                        return m;
                    }
                }

                return null;
            }
            public override CSL_Content.Value MemberCallCache(CSL_Content content, object object_this, List<CSL_Content.Value> _params, MethodCache cache)
            {
                List<Type> types = new List<Type>();
                List<object> _oparams = new List<object>();
                foreach (var p in _params)
                {
                    _oparams.Add(p.value);
                    if ((SType)p.type != null)
                    {
                        types.Add(typeof(object));
                    }
                    else
                    {
                        types.Add(p.type);
                    }
                }

                var targetop = cache.info;
                CSL_Content.Value v = new CSL_Content.Value();
                List<object> newObjs = ModifyParam(cache.info, content, _params);
                object[] objs = newObjs.ToArray();
                v.value = targetop.Invoke(object_this, objs);
                v.type = cache.returnType;
                return v;
            }
            public static List<object> ModifyParam(System.Reflection.MethodBase mi, CSL_Content content, List<CSL_Content.Value> _params)
            {
                var ps = mi.GetParameters();
                List<object> newObjs = new List<object>();
                List<object> _oparams = new List<object>();
                int paramCount = _params.Count;
                for (int i = 0; i < paramCount; i++)
                {
                    _oparams.Add(_params[i].value);
                }
                int length = ps.Length;
                for (int i = paramCount; i < length; i++)
                {
                    _oparams.Add(ps[i].DefaultValue);
                }
                for (int i = 0; i < length; i++)
                {
                    System.Reflection.ParameterInfo pi = ps[i];
                    object obj = _oparams[i];
                    Type t = pi.ParameterType;
                    if (IsParams(pi))
                    {
                        paramCount = _params.Count;
                        if (paramCount - i == 1 && obj != null && obj.GetType() == t)
                        {
                            newObjs.Add(obj);
                        }
                        else
                        {
                            paramCount = _oparams.Count;
                            Array array = Array.CreateInstance(t.Assembly.GetType(t.FullName.Replace("[]", "")), paramCount - i);
                            for (int j = i; j < paramCount; j++)
                                array.SetValue(_oparams[j], j - i);
                            newObjs.Add(array);
                        }
                        break;
                    }
                    else
                    {
                        if (i < _params.Count)
                        {
                            CSL_Content.Value value = _params[i];
                            bool proccessed = false;
                            if (obj != null && obj.GetType() != t)
                            {
                                if (obj.GetType() == typeof(JSONData))
                                {
                                    System.Reflection.MethodInfo _mi = JSONData.GetImplicit(t);
                                    if (_mi != null)
                                    {
                                        newObjs.Add(_mi.Invoke(null, new object[] { obj }));
                                        proccessed = true;
                                    }
                                }
                            }
                            if (!proccessed)
                            {
                                switch (t.Name)
                                {
                                    case "Int64": newObjs.Add(Convert.ToInt64(obj)); break;
                                    case "UInt64": newObjs.Add(Convert.ToUInt64(obj)); break;
                                    case "Int32": newObjs.Add(Convert.ToInt32(obj)); break;
                                    case "UInt32": newObjs.Add(Convert.ToUInt32(obj)); break;
                                    case "Int16": newObjs.Add(Convert.ToInt16(obj)); break;
                                    case "UInt16": newObjs.Add(Convert.ToInt16(obj)); break;
                                    case "Byte": newObjs.Add(Convert.ToByte(obj)); break;
                                    case "SByte": newObjs.Add(Convert.ToSByte(obj)); break;
                                    case "Single": newObjs.Add(Convert.ToSingle(obj)); break;
                                    case "Double": newObjs.Add(Convert.ToDouble(obj)); break;
                                    case "SInstance": newObjs.Add(obj); break;
                                    default:
                                        if (value.needConvertTo(pi))
                                            obj = content.vm.GetType(value.type).ConvertTo(content, obj, pi.ParameterType);
                                        newObjs.Add(obj); 
                                        break;
                                }
                            }
                        }
                        else
                            newObjs.Add(obj);
                    }
                }
                return newObjs;
            }

            class MemberValueCache
            {
                public int type = 0;
                public System.Reflection.FieldInfo finfo;
                public System.Reflection.MethodInfo minfo;
                public System.Reflection.EventInfo einfo;
            }
            Dictionary<string, MemberValueCache> memberValuegetCaches = new Dictionary<string, MemberValueCache>();
            public override CSL_Content.Value MemberValueGet(CSL_Content content, object object_this, string valuename)
            {
                MemberValueCache c;
                if (!memberValuegetCaches.TryGetValue(valuename, out c))
                {
                    c = new MemberValueCache();
                    memberValuegetCaches[valuename] = c;
                    c.finfo = type.GetField(valuename);
                    if (c.finfo == null)
                    {
                        c.minfo = type.GetMethod("get_" + valuename);
                        if (c.minfo == null)
                        {
                            c.einfo = type.GetEvent(valuename);
                            if (c.einfo == null)
                            {
                                c.type = -1;
                                return null;
                            }
                            else
                            {
                                c.type = 3;
                            }
                        }
                        else
                        {
                            c.type = 2;
                        }
                    }
                    else
                    {
                        c.type = 1;
                    }
                }

                if (c.type < 0) return null;
                CSL_Content.Value v = new CSL_Content.Value();
                switch (c.type)
                {
                    case 1:
                        v.value = c.finfo.GetValue(object_this);
                        v.type = c.finfo.FieldType;
                        break;
                    case 2:
                        v.value = c.minfo.Invoke(object_this, null);
                        v.type = c.minfo.ReturnType;
                        break;
                    case 3:
                        v.value = new CSL_DeleEvent(object_this, c.einfo);
                        v.type = c.einfo.EventHandlerType;
                        break;

                }
                return v;
            }

            Dictionary<string, MemberValueCache> memberValuesetCaches = new Dictionary<string, MemberValueCache>();

            public override bool MemberValueSet(CSL_Content content, object object_this, string valuename, object value)
            {
                MemberValueCache c;
                if (!memberValuesetCaches.TryGetValue(valuename, out c))
                {
                    c = new MemberValueCache();
                    memberValuesetCaches[valuename] = c;
                    c.finfo = type.GetField(valuename);
                    if (c.finfo == null)
                    {
                        c.minfo = type.GetMethod("set_" + valuename);
                        if (c.minfo == null)
                        {
                            if (type.GetMethod("add_" + valuename) != null)
                            {
                                c.type = 3;
                            }
                            else
                            {
                                c.type = -1;
                                return false;
                            }
                        }

                        else
                        {
                            c.type = 2;
                        }
                    }
                    else
                    {
                        c.type = 1;
                    }
                }

                if (c.type < 0)
                    return false;

                if (c.type == 1)
                {
                    if (value != null && value.GetType() != c.finfo.FieldType)
                    {

                        value = content.vm.GetType(value.GetType()).ConvertTo(content, value, c.finfo.FieldType);
                    }
                    c.finfo.SetValue(object_this, value);
                }
                else if (c.type == 2)
                {
                    var ptype = c.minfo.GetParameters()[0].ParameterType;
                    if (value != null && value.GetType() != ptype)
                    {

                        value = content.vm.GetType(value.GetType()).ConvertTo(content, value, ptype);
                    }
                    c.minfo.Invoke(object_this, new object[] { value });
                }
                return true;
            }

            class stIndexCache
            {
                public System.Reflection.MethodInfo indexGetCache;
                public Type indexGetCachetypeindex;
                public Type indexGetCacheType;
            }
            Dictionary<Type, stIndexCache> indexCaches = new Dictionary<Type, stIndexCache>();
            stIndexCache GetIndexGetCache(Type t)
            {
                if (t == null)
                    return null;
                stIndexCache ic;
                if (indexCaches.TryGetValue(t, out ic))
                    return ic;
                ic = new stIndexCache();
                ic.indexGetCache = type.GetMethod("get_Item", new Type[] { t });
                if (ic.indexGetCache == null)
                {
                    ic.indexGetCache = type.GetMethod("GetValue", new Type[] { typeof(int) });
                }
                if (ic.indexGetCache != null)
                {
                    ic.indexGetCacheType = ic.indexGetCache.ReturnType;
                    ic.indexGetCachetypeindex = ic.indexGetCache.GetParameters()[0].ParameterType;
                    return ic;
                }
                return null;
            }
            Type GetType(object obj)
            {
                if (obj == null)
                    return null;
                Type t = obj.GetType();
                if (t == typeof(CSL_Content.Value))
                {
                    CSL_Content.Value v = (obj as CSL_Content.Value);
                    t = v.type;
                    if (t == null && v.type != null)
                        t = typeof(SInstance);
                }
                return t;
            }
            public override CSL_Content.Value IndexGet(CSL_Content content, object object_this, object key)
            {
                Type t = GetType(key);
                if (key == null)
                    return CSL_Content.Value.Null;
                stIndexCache ic = GetIndexGetCache(t);
                if (ic != null)
                {
                    CSL_Content.Value v = new CSL_Content.Value();
                    //v.type = ic.indexGetCacheType;
                    if (key.GetType() == typeof(CSL_Content.Value))
                    {
                        CSL_Content.Value v2 = key as CSL_Content.Value;
                        if (v2.type != ic.indexGetCachetypeindex)
                            v2.value = content.vm.GetType(v2.type).ConvertTo(content, v2.value, (CSL_Type)ic.indexGetCachetypeindex);
                        v.value = ic.indexGetCache.Invoke(object_this, new object[] { v2.value });
                    }
                    else
                    {
                        if (key != null && key.GetType() != ic.indexGetCachetypeindex)
                            key = content.vm.GetType(key.GetType()).ConvertTo(content, key, (CSL_Type)ic.indexGetCachetypeindex);
                        v.value = ic.indexGetCache.Invoke(object_this, new object[] { key });
                    }
                    t = GetType(v.value);
                    if (t != null)
                        v.type = t;
                    return v;
                }
                return CSL_Content.Value.Null;
            }

            class stIndexSetCache
            {
                public System.Reflection.MethodInfo indexSetCache;
                public Type indexSetCachetype1;
                public Type indexSetCachetype2;
                public bool indexSetCachekeyfirst = true;
            }
            Dictionary<string, stIndexSetCache> indexSetCaches = new Dictionary<string, stIndexSetCache>();
            stIndexSetCache GetIndexSetCache(Type tKey, Type tValue)
            {
                stIndexSetCache isc;
                if (indexSetCaches.TryGetValue(tKey.FullName+tValue.FullName, out isc))
                    return isc;
                isc = new stIndexSetCache();
                isc.indexSetCache = type.GetMethod("set_Item", new Type[] { tKey, tValue });
                if (isc.indexSetCache == null)
                {
                    isc.indexSetCache = type.GetMethod("set_Item", new Type[] { tKey, typeof(object) });
                    if (isc.indexSetCache == null)
                    {
                        isc.indexSetCache = type.GetMethod("set_Item", new Type[] { tKey, typeof(JSONData) });
                        if (isc.indexSetCache == null)
                            isc.indexSetCache = type.GetMethod("set_Item");
                    }
                }
                if (isc.indexSetCache == null)
                {
                    isc.indexSetCache = type.GetMethod("SetValue", new Type[] { typeof(object), typeof(int) });
                    isc.indexSetCachekeyfirst = false;
                }
                var pp = isc.indexSetCache.GetParameters();
                isc.indexSetCachetype1 = pp[0].ParameterType;
                isc.indexSetCachetype2 = pp[1].ParameterType;
                indexSetCaches[tKey.FullName + tValue.FullName] = isc;
                return isc;
            }
            public override void IndexSet(CSL_Content content, object object_this, object key, object value)
            {
                Type tKey = GetType(key);
                if (tKey == null)
                    return;
                Type tValue = GetType(value);
                if (tValue == null)
                    return;
                stIndexSetCache ic = GetIndexSetCache(tKey, tValue);
                if (ic.indexSetCachekeyfirst)
                {
                    if (key != null && key.GetType() != ic.indexSetCachetype1)
                        key = content.vm.GetType(key.GetType()).ConvertTo(content, key, (CSL_Type)ic.indexSetCachetype1);
                    if (value != null && value.GetType() != ic.indexSetCachetype2)
                        value = content.vm.GetType(value.GetType()).ConvertTo(content, value, (CSL_Type)ic.indexSetCachetype2);
                    CSL_Content.Value v = value as CSL_Content.Value;
                    if (v != null)
                        ic.indexSetCache.Invoke(object_this, new object[] { key, v.value });
                    else
                        ic.indexSetCache.Invoke(object_this, new object[] { key, value });
                }
                else
                {
                    if (value != null && value.GetType() != ic.indexSetCachetype1)
                        value = content.vm.GetType(value.GetType()).ConvertTo(content, value, (CSL_Type)ic.indexSetCachetype1);
                    if (key != null && key.GetType() != ic.indexSetCachetype2)
                        key = content.vm.GetType(key.GetType()).ConvertTo(content, key, (CSL_Type)ic.indexSetCachetype2);

                    CSL_Content.Value v = value as CSL_Content.Value;
                    if (v != null)
                        ic.indexSetCache.Invoke(object_this, new object[] { v.value, key });
                    else
                        ic.indexSetCache.Invoke(object_this, new object[] { value, key });
                }
            }

            public virtual bool hasMemberCall(string func)
            {
                return (type.GetMethod(func) != null);
            }
        }


        public class RegHelper_Type : CSL_TypeBase
        {
            public static RegHelper_Type MakeType(Type type, string keyword)
            {
                if (!type.IsSubclassOf(typeof(Delegate)))
                {
                    return new RegHelper_Type(type, keyword, false);
                }
                var method = type.GetMethod("Invoke");
                var pp = method.GetParameters();
                if (method.ReturnType == typeof(void))
                {
                    if (pp.Length == 0)
                    {
                        return new CSL_RegHelper_DeleAction(type, keyword);
                    }
                    else if (pp.Length == 1)
                    {
                        var gtype = typeof(CSL_RegHelper_DeleAction<>).MakeGenericType(new Type[] { pp[0].ParameterType });
                        return gtype.GetConstructors()[0].Invoke(new object[] { type, keyword }) as RegHelper_Type;
                    }
                    else if (pp.Length == 2)
                    {
                        var gtype = typeof(CSL_RegHelper_DeleAction<,>).MakeGenericType(new Type[] { pp[0].ParameterType, pp[1].ParameterType });
                        return (gtype.GetConstructors()[0].Invoke(new object[] { type, keyword }) as RegHelper_Type);
                    }
                    else if (pp.Length == 3)
                    {
                        var gtype = typeof(CSL_RegHelper_DeleAction<,,>).MakeGenericType(new Type[] { pp[0].ParameterType, pp[1].ParameterType, pp[2].ParameterType });
                        return (gtype.GetConstructors()[0].Invoke(new object[] { type, keyword }) as RegHelper_Type);
                    }
                    else
                    {
                        throw new Exception("too much param,only support 3.");
                    }
                }
                else
                {
                    Type gtype = null;
                    if (pp.Length == 0)
                    {
                        gtype = typeof(CSL_RegHelper_DeleNonVoidAction<>).MakeGenericType(new Type[] { method.ReturnType });
                    }
                    else if (pp.Length == 1)
                    {
                        gtype = typeof(CSL_RegHelper_DeleNonVoidAction<,>).MakeGenericType(new Type[] { method.ReturnType, pp[0].ParameterType });
                    }
                    else if (pp.Length == 2)
                    {
                        gtype = typeof(CSL_RegHelper_DeleNonVoidAction<,,>).MakeGenericType(new Type[] { method.ReturnType, pp[0].ParameterType, pp[1].ParameterType });
                    }
                    else if (pp.Length == 3)
                    {
                        gtype = typeof(CSL_RegHelper_DeleNonVoidAction<,,,>).MakeGenericType(new Type[] { method.ReturnType, pp[0].ParameterType, pp[1].ParameterType, pp[2].ParameterType });
                    }
                    else
                    {
                        throw new Exception("too much param,only support 3.");
                    }
                    return (gtype.GetConstructors()[0].Invoke(new object[] { type, keyword }) as RegHelper_Type);

                }


            }
            public void Set(Type type, string setkeyword)
            {
                function = new CSL_RegHelper_TypeFunction(type);
                if (setkeyword != null)
                {
                    Keyword = setkeyword.Replace(" ", "");
                }
                else
                {
                    Keyword = type.Name;
                }
                this.type = type;
                this._type = type;
            }
            public RegHelper_Type()
            {

            }
            protected RegHelper_Type(Type type, string setkeyword, bool dele)
            {

                function = new CSL_RegHelper_TypeFunction(type);
                if (setkeyword != null)
                {
                    Keyword = setkeyword.Replace(" ", "");
                }
                else
                {
                    Keyword = type.Name;
                }
                this.type = type;
                _type = type;
            }
            public override string Keyword
            {
                get;
                protected set;
            }
            public override string Namespace
            {
                get { return type.NameSpace; }
            }
            public override string FullName
            {
                get
                {
                    return type.FullName;
                }
            }
            public override CSL_Type type
            {
                get;
                protected set;
            }
            public Type _type;

            public override object ConvertTo(CSL_Content content, object src, CSL_Type targetType)
            {
                Type targettype = (Type)targetType;
                if (_type == targettype) return src;

                if (_type.IsEnum)
                {
                    if ((Type)targetType == typeof(int))
                        return System.Convert.ToInt32(src);
                    else if ((Type)targetType == typeof(uint))
                        return System.Convert.ToUInt32(src);
                    else if ((Type)targetType == typeof(short))
                        return System.Convert.ToInt16(src);
                    else if ((Type)targetType == typeof(ushort))
                        return System.Convert.ToUInt16(src);
                    else
                    {
                        return System.Convert.ToInt32(src);
                    }
                }
                else if (targettype != null && targettype.IsEnum)
                {
                    return Enum.ToObject(targettype, src);

                }
                if (_type.FullName.StartsWith("CSharpLike.Internal.CSL_Type_Nullable")
                    /*|| _type.FullName.StartsWith("System.Nullable")*/)
                {
                    if (src == null)
                        return default;
                    var m = _type.GetMethod("get_Value", new Type[] { });
                    if (m != null)
                        return m.Invoke(src, new object[] { });
                }
                var ms = _type.GetMethods();
                foreach (var m in ms)
                {
                    if ((m.Name == "op_Implicit" || m.Name == "op_Explicit") && m.ReturnType == targettype)
                    {
                        return m.Invoke(null, new object[] { src });
                    }
                }
                if (targettype != null)
                {
                    if (targettype.IsAssignableFrom(_type))
                        return src;
                    if (src != null && targettype.IsInstanceOfType(src))
                        return src;
                    
                    var m = targettype.GetMethod("op_Implicit", new Type[] { _type });
                    if (m == null)
                        m = targettype.GetMethod("op_Explicit", new Type[] { _type });
                    if (m == null && _type == typeof(CSL_Content.Value))
                    {
                        CSL_Content.Value v = src as CSL_Content.Value;
                        Type t = v.value.GetType();
                        if (t == targettype)
                            return src;
                        m = targettype.GetMethod("op_Implicit", new Type[] { t });
                        if (m != null)
                        {
                            v.value = m.Invoke(null, new object[] { v.value });
                            v.type = targettype;
                            return v;
                        }
                    }
                    else if (m != null)
                        return m.Invoke(null, new object[] { src });
                }
                else
                {
                    return src;
                }

                return null;
            }
            System.Reflection.MethodInfo tryGetMethodInfo(object left, CSL_Content.Value right, string name)
            {
                System.Reflection.MethodInfo call = null;
                Type t1 = left != null ? left.GetType() : _type;
                Type t2 = right.type;
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        switch (i)
                        {
                            case 0: call = t1.GetMethod(name, new Type[2] { t1, t2 }); break;
                            case 1: call = t2.GetMethod(name, new Type[2] { t1, t2 }); break;
                            case 2: call = t1.GetMethod(name); break;
                            case 3: call = t2.GetMethod(name); break;
                        }
                        if (call != null)
                            return call;
                    }
                    catch
                    {
                    }
                }
                return call;
            }

            public override object Math2Value(CSL_Content content, string code, object left, CSL_Content.Value right, out CSL_Type returntype)
            {
                returntype = type;
                if (returntype == typeof(JSONData))
                {
                    if (left is string || right.type == typeof(string))
                        returntype = typeof(string);
                    else
                    {
                        Type typeLeft = left.GetType();
                        if (typeLeft == typeof(JSONData))
                            typeLeft = (left as JSONData).ValueType;
                        Type typeRight = right.type;
                        if (typeRight == typeof(JSONData))
                            typeRight = (right.value as JSONData).ValueType;
                        returntype = CSL_NumericTypeUtils.GetReturnType_Math2Value(typeLeft, typeRight);
                    }
                }
                System.Reflection.MethodInfo call = null;
                switch(code)
                {
                    case "+":
                        if ((Type)right.type == typeof(string))
                        {
                            returntype = typeof(string);
                            if (left == null)
                                return "null" + right.value;
                            else
                                return left.ToString() + right.value;
                        }
                        else if (left is string)
                        {
                            returntype = typeof(string);
                            if (right.value == null)
                                return left.ToString() + "null";
                            else
                                return left.ToString() + right.value.ToString();
                        }
                        call = tryGetMethodInfo(left, right, "op_Addition");
                        if (call == null)
                            return CSL_NumericTypeUtils.Decimal2TargetType(returntype, CSL_NumericTypeUtils.GetDecimalValue(_type, left) + CSL_NumericTypeUtils.GetDecimalValue(right.type, right.value));
                        break;
                    case "-":
                        call = tryGetMethodInfo(left, right, "op_Subtraction");
                        if (call == null)
                            return CSL_NumericTypeUtils.Decimal2TargetType(returntype, CSL_NumericTypeUtils.GetDecimalValue(_type, left) - CSL_NumericTypeUtils.GetDecimalValue(right.type, right.value));
                        break;
                    case "*":
                        call = tryGetMethodInfo(left, right, "op_Multiply");
                        if (call == null)
                            return CSL_NumericTypeUtils.Decimal2TargetType(returntype, CSL_NumericTypeUtils.GetDecimalValue(_type, left) * CSL_NumericTypeUtils.GetDecimalValue(right.type, right.value));
                        break;
                    case "/":
                        call = tryGetMethodInfo(left, right, "op_Division");
                        if (call == null)
                            return CSL_NumericTypeUtils.Decimal2TargetType(returntype, CSL_NumericTypeUtils.GetDecimalValue(_type, left) / CSL_NumericTypeUtils.GetDecimalValue(right.type, right.value));
                        break;
                    case "%":
                        call = tryGetMethodInfo(left, right, "op_Modulus");
                        if (call == null)
                            return CSL_NumericTypeUtils.Decimal2TargetType(returntype, CSL_NumericTypeUtils.GetDecimalValue(_type, left) % CSL_NumericTypeUtils.GetDecimalValue(right.type, right.value));
                        break;
                    default:
                        throw new NotImplementedException("not support:"+ code);
                }

                try
                {
                    return call.Invoke(null, new object[] { CSL_NumericTypeUtils.GetDecimalValue(_type, left), CSL_NumericTypeUtils.GetDecimalValue(right.type, right.value) });
                }
                catch
                {
                    return call.Invoke(null, new object[] { left, right.value });
                }
            }
            bool ProcessJSONDataMathLogic(TokenLogic code, object left, CSL_Content.Value right, bool bLeft, bool bRight)
            {
                object objLeft = bLeft ? (left as JSONData).Value : left;
                object objRight = bRight ? (right.value as JSONData).Value : right.value;
                switch (code)
                {
                    case TokenLogic.More: return CSL_NumericTypeUtils.GetDecimalValue(objLeft.GetType(), objLeft) > CSL_NumericTypeUtils.GetDecimalValue(objRight.GetType(), objRight);
                    case TokenLogic.Less: return CSL_NumericTypeUtils.GetDecimalValue(objLeft.GetType(), objLeft) < CSL_NumericTypeUtils.GetDecimalValue(objRight.GetType(), objRight);
                    case TokenLogic.MoreEqual: return CSL_NumericTypeUtils.GetDecimalValue(objLeft.GetType(), objLeft) >= CSL_NumericTypeUtils.GetDecimalValue(objRight.GetType(), objRight);
                    case TokenLogic.LessEqual: return CSL_NumericTypeUtils.GetDecimalValue(objLeft.GetType(), objLeft) <= CSL_NumericTypeUtils.GetDecimalValue(objRight.GetType(), objRight);
                    case TokenLogic.Equal:
                        if (left == null || right.type == null)
                            return left == right.value;
                        return CSL_NumericTypeUtils.GetDecimalValue(objLeft.GetType(), objLeft) == CSL_NumericTypeUtils.GetDecimalValue(objRight.GetType(), objRight);
                    case TokenLogic.NotEqual:
                        if (left == null || right.type == null)
                            return left == right.value;
                        return CSL_NumericTypeUtils.GetDecimalValue(objLeft.GetType(), objLeft) != CSL_NumericTypeUtils.GetDecimalValue(objRight.GetType(), objRight);
                    default:
                        throw new Exception("not support TokenLogic:" + code);
                }
            }

            public override bool MathLogic(CSL_Content content, TokenLogic code, object left, CSL_Content.Value right)
            {
                System.Reflection.MethodInfo call = null;
                bool bLeftJSONData = left != null && left.GetType() == typeof(JSONData);
                bool bRightJSONData = right.type == typeof(JSONData);
                if (bLeftJSONData || bRightJSONData)//special process
                    return ProcessJSONDataMathLogic(code, left, right, bLeftJSONData, bRightJSONData);
                switch(code)
                {
                    case TokenLogic.More: call = _type.GetMethod("op_GreaterThan"); break;
                    case TokenLogic.Less: call = _type.GetMethod("op_LessThan"); break;
                    case TokenLogic.MoreEqual: call = _type.GetMethod("op_GreaterThanOrEqual"); break;
                    case TokenLogic.LessEqual: call = _type.GetMethod("op_LessThanOrEqual"); break;
                    case TokenLogic.Equal:
                        if (left == null || right.type == null)
                            return left == right.value;
                        call = _type.GetMethod("op_Equality");
                        if (call == null)
                            return left.Equals(right.value);
                        break;
                    case TokenLogic.NotEqual:
                        if (left == null || right.type == null)
                            return left != right.value;
                        call = _type.GetMethod("op_Inequality");
                        if (call == null)
                            return !left.Equals(right.value);
                        break;
                }
                return (bool)call.Invoke(null, new object[] { left, right.value });
            }

            public override CSL_TypeFunctionBase function
            {
                get;
                protected set;
            }
        }
    }
}