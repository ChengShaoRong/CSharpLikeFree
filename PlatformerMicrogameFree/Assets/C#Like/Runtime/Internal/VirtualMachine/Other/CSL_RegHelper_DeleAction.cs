/*
 *           C#Like
 * Copyright © 2022-2023 RongRong. All right reserved.
 */
using System;

namespace CSharpLike
{
    namespace Internal
    {
        public class CSL_RegHelper_DeleAction : CSL_TypeDeleBase
        {

            public CSL_RegHelper_DeleAction(Type type, string setkeyword)
                : base(type, setkeyword, true)
            {

            }

            public override object Math2Value(CSL_Content content, string code, object left, CSL_Content.Value right, out CSL_Type returntype)
            {
                returntype = null;

                if (left is CSL_DeleEvent)
                {
                    CSL_DeleEvent info = left as CSL_DeleEvent;
                    Delegate calldele = null;

                    object rightValue = right.value;
                    if (rightValue is CSL_DeleFunction)
                    {
                        if (code == "+")
                        {
                            calldele = CreateDelegate(content.vm, rightValue as CSL_DeleFunction);
                        }
                        else if (code == "-")
                        {
                            calldele = CreateDelegate(content.vm, rightValue as CSL_DeleFunction);
                        }
                    }
                    else if (rightValue is CSL_DeleLambda)
                    {
                        if (code == "+")
                        {
                            calldele = CreateDelegate(content.vm, rightValue as CSL_DeleLambda);
                        }
                        else if (code == "-")
                        {
                            calldele = CreateDelegate(content.vm, rightValue as CSL_DeleLambda);
                        }
                    }
                    else if (rightValue is Delegate)
                    {
                        calldele = rightValue as Delegate;
                    }

                    if (code == "+")
                    {
                        info._event.AddEventHandler(info.source, calldele);
                        return info;
                    }
                    else if (code == "-")
                    {
                        info._event.RemoveEventHandler(info.source, calldele);
                        return info;
                    }

                }
                else if (left is Delegate || left == null)
                {
                    Delegate info = left as Delegate;
                    Delegate calldele = null;
                    if (right.value is CSL_DeleFunction)
                        calldele = CreateDelegate(content.vm, right.value as CSL_DeleFunction);
                    else if (right.value is CSL_DeleLambda)
                        calldele = CreateDelegate(content.vm, right.value as CSL_DeleLambda);
                    else if (right.value is Delegate)
                        calldele = right.value as Delegate;
                    if (code == "+")
                    {
                        return Delegate.Combine(info, calldele); ;
                    }
                    else if (code == "-")
                    {
                        return Delegate.Remove(info, calldele);
                    }
                }
                return new NotSupportedException();
            }

            public override Delegate CreateDelegate(CSL_VirtualMachine vm, CSL_DeleFunction delefunc)
            {
                CSL_DeleFunction _func = delefunc;
                Delegate _dele = delefunc.CacheFunction(_type, null);
                if (_dele != null) return _dele;
                Action dele = () =>
                {
                    var func = _func.calltype.functions[_func.functionName];
                    if (func.code != null)
                    {
                        CSL_VirtualMachine.bPrintError = true;
                        CSL_Content content = new CSL_Content(vm);
                        content.DepthAdd();
                        content.CallThis = _func.callthis;
                        content.CallType = _func.calltype;
                        content.function = func;

                        func.code.Execute(content);
                        content.DepthRemove();
                    }
                };
                Delegate d = dele as Delegate;
                if ((Type)type != typeof(Action))
                {
                    _dele = Delegate.CreateDelegate(type, d.Target, d.Method);
                }
                else
                {
                    _dele = dele;
                }
                return delefunc.CacheFunction(_type, _dele);
            }

            public override Delegate CreateDelegate(CSL_VirtualMachine vm, CSL_DeleLambda lambda)
            {
                CSL_Content content = lambda.content.Clone();
                var expr = lambda.code;
                Action dele = () =>
                {
                    if (expr != null)
                    {
                        CSL_VirtualMachine.bPrintError = true;
                        content.DepthAdd();


                        expr.Execute(content);

                        content.DepthRemove();
                    }
                };
                Delegate d = dele as Delegate;
                if ((Type)type != typeof(Action))
                {
                    return Delegate.CreateDelegate(type, d.Target, d.Method);
                }
                else
                {
                    return dele;
                }
            }
        }
    }

}