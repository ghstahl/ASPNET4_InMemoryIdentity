#if _USE_CASTLE
using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace FlattenedDocument.CassandraStore.DAO
{
    public class CassandraPageInterceptor<T> : IInterceptor
    {
        private readonly CassandraPageProxy<T>  _page;

        public CassandraPageInterceptor(CassandraPageProxy<T> page)
        {
            _page = page;
        }

        public void Intercept(IInvocation invocation)
        {
            Type type = _page.Page.GetType();
            MethodInfo method1 = type.GetMethod(invocation.Method.Name);
            if (method1 != null)
            {
                invocation.ReturnValue = method1.Invoke(_page.Page, invocation.Arguments);
            }
            else
            {
                invocation.Proceed();
            }
        }
    }
}
#endif