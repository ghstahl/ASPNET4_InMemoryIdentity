using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using P5.CassandraStore.Settings;

namespace P5.CassandraStore.DAO
{
    public class CassandraResult<T>
    {
        public bool DaoResult { get; private set; }
        public CassandraResult(bool daoResult,T successResult)
        {
            Result = successResult;
            DaoResult = daoResult;
        }
        public T Result { get; private set; }
    }
   public class CassandraDao : ICassandraDAO, IDisposable
   {
        private  Cluster _cluster;

       private Cluster Cluster
       {
           get { return _cluster ?? (_cluster = Connect()); }
       }

        private ISession _session;

        private ISession Session
        {
            get
            {
                if (_session == null)
                {
                    try
                    {
                        _session = Cluster.Connect(CassandraConfig.KeySpace);
                    }
                    catch (Exception e)
                    {
                        _session = null;
                        throw;
                    }
                }
                return _session;

            }
        }

        private CassandraConfig CassandraConfig { get; set; }

        public CassandraDao(CassandraConfig cassandraConfig )
        {
            CassandraConfig = cassandraConfig;
        }

        private Cluster Connect()
        {
            try
            {
                QueryOptions queryOptions = new QueryOptions()
                    .SetConsistencyLevel(ConsistencyLevel.One);
                var builder = Cassandra.Cluster.Builder();
                builder.AddContactPoints(CassandraConfig.ContactPoints);

                if (!string.IsNullOrEmpty(CassandraConfig.Credentials.UserName) &&
                    !string.IsNullOrEmpty(CassandraConfig.Credentials.Password))
                {
                    builder.WithCredentials(
                        CassandraConfig.Credentials.UserName,
                        CassandraConfig.Credentials.Password);
                }
                builder.WithQueryOptions(queryOptions);
                Cluster cluster = builder.Build();

                return cluster;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<ISession> GetSessionAsync()
        {
            var task = Task.Run(() => Session);
            // do other stuff
            var myOutput = await task;
            // some more stuff
            return myOutput;
        }

        public static async Task<bool> TruncateTablesAsync(ISession session,IEnumerable<string> tables,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                IMapper mapper = new Mapper(session);
                cancellationToken.ThrowIfCancellationRequested();

                string template = "truncate {0}";
                foreach (string cql in tables.Select(table => string.Format(template, table)))
                {
                    await mapper.ExecuteAsync(cql);
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

       public void Dispose()
       {
           _session = null;
       }

       public static CassandraResult<bool> DoAction(Action action)
       {
           bool daoResult = false;
           try
           {
               action();
               return new CassandraResult<bool>(true, true);
           }
           catch (NoHostAvailableException nhae)
           {
               // Handle any exceptions as you wish.
           }
           catch (DriverException driverEx)
           {
               // Handle any exceptions as you wish.
           }
           catch (Exception e)
           {
               // Handle any exceptions as you wish.
           }
           return new CassandraResult<bool>(false, false);

       }
       public static CassandraResult<bool> DoAction<T1>(Action<T1> action, T1 arg1)
       {
           return DoAction(() => action(arg1));
       }
       public static CassandraResult<bool> DoAction<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
       {
           return DoAction(() => action(arg1, arg2));
       }
       public static CassandraResult<bool> DoAction<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
       {
           return DoAction(() => action(arg1, arg2, arg3));
       }
       public static CassandraResult<bool> DoAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
       {
           return DoAction(() => action(arg1, arg2, arg3, arg4));
       }
       public static CassandraResult<bool> DoAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
       {
           return DoAction(() => action(arg1, arg2, arg3, arg4, arg5));
       }
       public static async Task<CassandraResult<Task>> DoActionAsync(Action action)
       {
           bool daoResult = false;
           try
           {
               // Call the action at the appropriate time.
               var task = Task.Run(()=>action);
               var result = await task;
               return new CassandraResult<Task>(true,task);
           }
           catch (NoHostAvailableException nhae)
           {
               // Handle any exceptions as you wish.
           }
           catch (DriverException driverEx)
           {
               // Handle any exceptions as you wish.
           }
           catch (Exception e)
           {
               // Handle any exceptions as you wish.
           }
           return new CassandraResult<Task>(daoResult, null);

       }
       public static async Task<CassandraResult<Task>> DoActionAsync<T1>(Action<T1> action, T1 arg1)
       {
           return await DoActionAsync(() => action(arg1));
       }
       public static async Task<CassandraResult<Task>> DoActionAsync<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
       {
           return await DoActionAsync(() => action(arg1,arg2));
       }
       public static async Task<CassandraResult<Task>> DoActionAsync<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3)
       {
           return await DoActionAsync(() => action(arg1, arg2, arg3));
       }
       public static async Task<CassandraResult<Task>> DoActionAsync<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
       {
           return await DoActionAsync(() => action(arg1, arg2, arg3, arg4));
       }
       public static async Task<CassandraResult<Task>> DoActionAsync<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
       {
           return await DoActionAsync(() => action(arg1, arg2, arg3, arg4, arg5));
       }
       public static CassandraResult<TResult> DoAction<TResult>(Func<TResult> func, TResult defaultResult)
       {
           bool daoResult = false;
           try
           {
               // Call the action at the appropriate time.
               var result = func();
               return new CassandraResult<TResult>(true, result);
           }
           catch (NoHostAvailableException nhae)
           {
               // Handle any exceptions as you wish.
           }
           catch (DriverException driverEx)
           {
               // Handle any exceptions as you wish.
           }
           catch (Exception e)
           {
               // Handle any exceptions as you wish.
           }
           return new CassandraResult<TResult>(daoResult, defaultResult);

       }

       public static CassandraResult<TResult> DoAction<T1, TResult>(Func<T1, TResult> func, TResult defaultResult, T1 arg1)
       {
           return DoAction(() => func(arg1), defaultResult);
       }
       public static CassandraResult<TResult> DoAction<T1, T2, TResult>(Func<T1, T2, TResult> func, TResult defaultResult, T1 arg1, T2 arg2)
       {
           return DoAction(() => func(arg1, arg2), defaultResult);
       }
       public static CassandraResult<TResult> DoAction<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, TResult defaultResult, T1 arg1, T2 arg2, T3 arg3)
       {
           return DoAction(() => func(arg1, arg2, arg3), defaultResult);
       }
       public static CassandraResult<TResult> DoAction<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, TResult defaultResult, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
       {
           return DoAction(() => func(arg1, arg2, arg3, arg4), defaultResult);
       }
       public static CassandraResult<TResult> DoAction<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, TResult defaultResult, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
       {
           return DoAction(() => func(arg1, arg2, arg3, arg4, arg5), defaultResult);
       }
       public static async Task<CassandraResult<TResult>> DoActionAsync<TResult>(Func<TResult> func, TResult defaultResult)
       {
           bool daoResult = false;
           try
           {
               // Call the action at the appropriate time.
               var task = Task.Run(func);
               var result = await task;
               return new CassandraResult<TResult>(true, result);
           }
           catch (NoHostAvailableException nhae)
           {
               // Handle any exceptions as you wish.
           }
           catch (DriverException driverEx)
           {
               // Handle any exceptions as you wish.
           }
           catch (Exception e)
           {
               // Handle any exceptions as you wish.
           }
           return new CassandraResult<TResult>(daoResult,defaultResult);

       }


       public static async Task<CassandraResult<TResult>> DoActionAsync<T1, TResult>(Func<T1, TResult> func, TResult defaultResult, T1 arg1)
       {
           return await DoActionAsync(() => func(arg1), defaultResult);
       }
       public static async Task<CassandraResult<TResult>> DoActionAsync<T1, T2, TResult>(Func<T1, T2, TResult> func, TResult defaultResult, T1 arg1, T2 arg2)
       {
           return await DoActionAsync(() => func(arg1, arg2), defaultResult);
       }
       public static async Task<CassandraResult<TResult>> DoActionAsync<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func, TResult defaultResult, T1 arg1, T2 arg2, T3 arg3)
       {
           return await DoActionAsync(() => func(arg1, arg2, arg3), defaultResult);
       }
       public static async Task<CassandraResult<TResult>> DoActionAsync<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func, TResult defaultResult, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
       {
           return await DoActionAsync(() => func(arg1, arg2, arg3, arg4), defaultResult);
       }
       public static async Task<CassandraResult<TResult>> DoActionAsync<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func, TResult defaultResult, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
       {
           return await DoActionAsync(() => func(arg1, arg2, arg3, arg4, arg5), defaultResult);
       }



   }
}