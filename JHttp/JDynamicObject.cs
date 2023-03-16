using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JHttp
{
    public class JDynamicObject : DynamicObject
    {
        private Dictionary<string, object> _myDic = null;
        
        public static dynamic Create(string json)
        {
            try
            {
                var val = JObject.Parse(json);
                var obj = JObj2Dy(val);

                return (JDynamicObject)obj;
            }
            catch
            {
                return json;
            }
        }

        private JDynamicObject(object obj)
        {
            _myDic = (Dictionary<string, object>)obj;
        }

        /// <summary>
        /// json对象转dic
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        private static object JObj2Dy(JToken jobj)
        {
            if (jobj == null) return null;


            var thistype = jobj.GetType();
            var jobjtype = typeof(JObject);
            var jarrtype = typeof(JArray);
            var jvaltype = typeof(JValue);
            if (thistype == jobjtype)
            {
                var result = new Dictionary<string, object>();
                foreach (var item in (JObject)jobj)
                {
                    result[item.Key] = JObj2Dy(item.Value);
                }
                return new JDynamicObject(result); ;
            }
            else if (thistype == jarrtype)
            {
                var result = new List<object>();
                foreach (var item in (JArray)jobj)
                {
                    result.Add(JObj2Dy(item));
                }
                return result;
            }
            else if (thistype == jvaltype)
            {
                return ((JValue)jobj).Value;
            }
            return null;
        }

        //
        // 摘要:
        //     Returns the enumeration of all dynamic member names.
        //
        // 返回结果:
        //     A sequence that contains dynamic member names.
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var names = _myDic.Keys;
            return names;
            //return base.GetDynamicMemberNames();
        }
        //
        // 摘要:
        //     Provides a System.Dynamic.DynamicMetaObject that dispatches to the dynamic virtual
        //     methods. The object can be encapsulated inside another System.Dynamic.DynamicMetaObject
        //     to provide custom behavior for individual actions. This method supports the Dynamic
        //     Language Runtime infrastructure for language implementers and it is not intended
        //     to be used directly from your code.
        //
        // 参数:
        //   parameter:
        //     The expression that represents System.Dynamic.DynamicMetaObject to dispatch to
        //     the dynamic virtual methods.
        //
        // 返回结果:
        //     An object of the System.Dynamic.DynamicMetaObject type.
        public override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return base.GetMetaObject(parameter);
        }
        //
        // 摘要:
        //     Provides implementation for binary operations. Classes derived from the System.Dynamic.DynamicObject
        //     class can override this method to specify dynamic behavior for operations such
        //     as addition and multiplication.
        //
        // 参数:
        //   binder:
        //     Provides information about the binary operation. The binder.Operation property
        //     returns an System.Linq.Expressions.ExpressionType object. For example, for the
        //     sum = first + second statement, where first and second are derived from the DynamicObject
        //     class, binder.Operation returns ExpressionType.Add.
        //
        //   arg:
        //     The right operand for the binary operation. For example, for the sum = first
        //     + second statement, where first and second are derived from the DynamicObject
        //     class, arg is equal to second.
        //
        //   result:
        //     The result of the binary operation.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            return base.TryBinaryOperation(binder, arg, out result);
        }
        //
        // 摘要:
        //     Provides implementation for type conversion operations. Classes derived from
        //     the System.Dynamic.DynamicObject class can override this method to specify dynamic
        //     behavior for operations that convert an object from one type to another.
        //
        // 参数:
        //   binder:
        //     Provides information about the conversion operation. The binder.Type property
        //     provides the type to which the object must be converted. For example, for the
        //     statement (String)sampleObject in C# (CType(sampleObject, Type) in Visual Basic),
        //     where sampleObject is an instance of the class derived from the System.Dynamic.DynamicObject
        //     class, binder.Type returns the System.String type. The binder.Explicit property
        //     provides information about the kind of conversion that occurs. It returns true
        //     for explicit conversion and false for implicit conversion.
        //
        //   result:
        //     The result of the type conversion operation.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            var res = base.TryConvert(binder, out result);

            return res;
        }
        //
        // 摘要:
        //     Provides the implementation for operations that initialize a new instance of
        //     a dynamic object. This method is not intended for use in C# or Visual Basic.
        //
        // 参数:
        //   binder:
        //     Provides information about the initialization operation.
        //
        //   args:
        //     The arguments that are passed to the object during initialization. For example,
        //     for the new SampleType(100) operation, where SampleType is the type derived from
        //     the System.Dynamic.DynamicObject class, args[0] is equal to 100.
        //
        //   result:
        //     The result of the initialization.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            return base.TryCreateInstance(binder, args, out result);
        }
        //
        // 摘要:
        //     Provides the implementation for operations that delete an object by index. This
        //     method is not intended for use in C# or Visual Basic.
        //
        // 参数:
        //   binder:
        //     Provides information about the deletion.
        //
        //   indexes:
        //     The indexes to be deleted.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            throw new System.NotImplementedException();
        }
        //
        // 摘要:
        //     Provides the implementation for operations that delete an object member. This
        //     method is not intended for use in C# or Visual Basic.
        //
        // 参数:
        //   binder:
        //     Provides information about the deletion.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            throw new System.NotImplementedException();
        }
        //
        // 摘要:
        //     Provides the implementation for operations that get a value by index. Classes
        //     derived from the System.Dynamic.DynamicObject class can override this method
        //     to specify dynamic behavior for indexing operations.
        //
        // 参数:
        //   binder:
        //     Provides information about the operation.
        //
        //   indexes:
        //     The indexes that are used in the operation. For example, for the sampleObject[3]
        //     operation in C# (sampleObject(3) in Visual Basic), where sampleObject is derived
        //     from the DynamicObject class, indexes[0] is equal to 3.
        //
        //   result:
        //     The result of the index operation.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a run-time exception is thrown.)
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (!(indexes != null && indexes.Length == 1 && indexes[0].GetType() == typeof(string)))
            {
                result = null;
            }
            else
            {
                result = _myDic.G((string)indexes[0]);
            }

            return true;
        }
        //
        // 摘要:
        //     Provides the implementation for operations that get member values. Classes derived
        //     from the System.Dynamic.DynamicObject class can override this method to specify
        //     dynamic behavior for operations such as getting a value for a property.
        //
        // 参数:
        //   binder:
        //     Provides information about the object that called the dynamic operation. The
        //     binder.Name property provides the name of the member on which the dynamic operation
        //     is performed. For example, for the Console.WriteLine(sampleObject.SampleProperty)
        //     statement, where sampleObject is an instance of the class derived from the System.Dynamic.DynamicObject
        //     class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies
        //     whether the member name is case-sensitive.
        //
        //   result:
        //     The result of the get operation. For example, if the method is called for a property,
        //     you can assign the property value to result.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a run-time exception is thrown.)
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _myDic.ContainsKey(binder.Name) ? _myDic[binder.Name] : null;
            return true;
        }
        //
        // 摘要:
        //     Provides the implementation for operations that invoke an object. Classes derived
        //     from the System.Dynamic.DynamicObject class can override this method to specify
        //     dynamic behavior for operations such as invoking an object or a delegate.
        //
        // 参数:
        //   binder:
        //     Provides information about the invoke operation.
        //
        //   args:
        //     The arguments that are passed to the object during the invoke operation. For
        //     example, for the sampleObject(100) operation, where sampleObject is derived from
        //     the System.Dynamic.DynamicObject class, args[0] is equal to 100.
        //
        //   result:
        //     The result of the object invocation.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }
        //
        // 摘要:
        //     Provides the implementation for operations that invoke a member. Classes derived
        //     from the System.Dynamic.DynamicObject class can override this method to specify
        //     dynamic behavior for operations such as calling a method.
        //
        // 参数:
        //   binder:
        //     Provides information about the dynamic operation. The binder.Name property provides
        //     the name of the member on which the dynamic operation is performed. For example,
        //     for the statement sampleObject.SampleMethod(100), where sampleObject is an instance
        //     of the class derived from the System.Dynamic.DynamicObject class, binder.Name
        //     returns "SampleMethod". The binder.IgnoreCase property specifies whether the
        //     member name is case-sensitive.
        //
        //   args:
        //     The arguments that are passed to the object member during the invoke operation.
        //     For example, for the statement sampleObject.SampleMethod(100), where sampleObject
        //     is derived from the System.Dynamic.DynamicObject class, args[0] is equal to 100.
        //
        //   result:
        //     The result of the member invocation.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return base.TryInvokeMember(binder, args, out result);
        }
        //
        // 摘要:
        //     Provides the implementation for operations that set a value by index. Classes
        //     derived from the System.Dynamic.DynamicObject class can override this method
        //     to specify dynamic behavior for operations that access objects by a specified
        //     index.
        //
        // 参数:
        //   binder:
        //     Provides information about the operation.
        //
        //   indexes:
        //     The indexes that are used in the operation. For example, for the sampleObject[3]
        //     = 10 operation in C# (sampleObject(3) = 10 in Visual Basic), where sampleObject
        //     is derived from the System.Dynamic.DynamicObject class, indexes[0] is equal to
        //     3.
        //
        //   value:
        //     The value to set to the object that has the specified index. For example, for
        //     the sampleObject[3] = 10 operation in C# (sampleObject(3) = 10 in Visual Basic),
        //     where sampleObject is derived from the System.Dynamic.DynamicObject class, value
        //     is equal to 10.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (!(indexes != null && indexes.Length == 1 && indexes[0].GetType() == typeof(string)))
            {
                return false;
            }

            _myDic[(string)indexes[0]] = value;

            return true;
        }
        //
        // 摘要:
        //     Provides the implementation for operations that set member values. Classes derived
        //     from the System.Dynamic.DynamicObject class can override this method to specify
        //     dynamic behavior for operations such as setting a value for a property.
        //
        // 参数:
        //   binder:
        //     Provides information about the object that called the dynamic operation. The
        //     binder.Name property provides the name of the member to which the value is being
        //     assigned. For example, for the statement sampleObject.SampleProperty = "Test",
        //     where sampleObject is an instance of the class derived from the System.Dynamic.DynamicObject
        //     class, binder.Name returns "SampleProperty". The binder.IgnoreCase property specifies
        //     whether the member name is case-sensitive.
        //
        //   value:
        //     The value to set to the member. For example, for sampleObject.SampleProperty
        //     = "Test", where sampleObject is an instance of the class derived from the System.Dynamic.DynamicObject
        //     class, the value is "Test".
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _myDic[binder.Name] = value;
            return true;
        }
        //
        // 摘要:
        //     Provides implementation for unary operations. Classes derived from the System.Dynamic.DynamicObject
        //     class can override this method to specify dynamic behavior for operations such
        //     as negation, increment, or decrement.
        //
        // 参数:
        //   binder:
        //     Provides information about the unary operation. The binder.Operation property
        //     returns an System.Linq.Expressions.ExpressionType object. For example, for the
        //     negativeNumber = -number statement, where number is derived from the DynamicObject
        //     class, binder.Operation returns "Negate".
        //
        //   result:
        //     The result of the unary operation.
        //
        // 返回结果:
        //     true if the operation is successful; otherwise, false. If this method returns
        //     false, the run-time binder of the language determines the behavior. (In most
        //     cases, a language-specific run-time exception is thrown.)
        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            return base.TryUnaryOperation(binder, out result);
        }
    }

    internal static class JDicExtend
    {
        public static T2 G<T1, T2>(this Dictionary<T1, T2> dic, T1 key)
        {
            return dic.ContainsKey(key) ? dic[key] : default(T2);
        }
        public static void D<T1, T2>(this Dictionary<T1, T2> dic, T1 key)
        {
            if (!dic.ContainsKey(key)) return;
            dic.Remove(key);
        }
    }
}
