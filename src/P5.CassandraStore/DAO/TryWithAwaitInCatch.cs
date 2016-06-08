using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace P5.CassandraStore.DAO
{
    public class TryWithAwaitInCatchExcpetionHandleResult<TResult>
    {
        public bool RethrowException { get; set; }
        public TResult DefaultResult { get; set; }
    }

    public static class TryWithAwaitInCatch
    {
        public static async Task ExecuteAndHandleErrorAsync(Func<Task> actionAsync,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<Task>>> errorHandlerAsync)
        {
            ExceptionDispatchInfo capturedException = null;
            try
            {
                await actionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                var errorResult = await errorHandlerAsync(capturedException.SourceException).ConfigureAwait(false);
                if (errorResult.RethrowException)
                {
                    capturedException.Throw();
                }
            }
        }

        public static async Task ExecuteAndHandleErrorAsync<T1>(
            Func<T1, Task> actionAsync,
            T1 arg1,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<Task>>> errorHandlerAsync)
        {
            await ExecuteAndHandleErrorAsync(() => actionAsync(arg1), errorHandlerAsync);
        }

        public static async Task ExecuteAndHandleErrorAsync<T1, T2>(
            Func<T1, T2, Task> actionAsync,
            T1 arg1, T2 arg2,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<Task>>> errorHandlerAsync)
        {
            await ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2), errorHandlerAsync);
        }

        public static async Task ExecuteAndHandleErrorAsync<T1, T2, T3>(
            Func<T1, T2, T3, Task> actionAsync,
            T1 arg1, T2 arg2, T3 arg3,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<Task>>> errorHandlerAsync)
        {
            await ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3), errorHandlerAsync);
        }

        public static async Task ExecuteAndHandleErrorAsync<T1, T2, T3, T4>(
            Func<T1, T2, T3, T4, Task> actionAsync,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<Task>>> errorHandlerAsync)
        {
            await ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3, arg4), errorHandlerAsync);
        }

        public static async Task ExecuteAndHandleErrorAsync<T1, T2, T3, T4, T5>(
            Func<T1, T2, T3, T4, T5, Task> actionAsync,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<Task>>> errorHandlerAsync)
        {
            await ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3, arg4, arg5), errorHandlerAsync);
        }

        public static async Task ExecuteAndHandleErrorAsync<T1, T2, T3, T4, T5, T6>(
            Func<T1, T2, T3, T4, T5, T6, Task> actionAsync,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<Task>>> errorHandlerAsync)
        {
            await ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3, arg4, arg5, arg6), errorHandlerAsync);
        }

        public static async Task ExecuteAndHandleErrorAsync<T1, T2, T3, T4, T5, T6, T7>(
            Func<T1, T2, T3, T4, T5, T6, T7, Task> actionAsync,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<Task>>> errorHandlerAsync)
        {
            await
                ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7),
                    errorHandlerAsync);
        }

        public static async Task<TResult> ExecuteAndHandleErrorAsync<TResult>(Func<Task<TResult>> actionAsync,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<TResult>>> errorHandlerAsync)
        {
            ExceptionDispatchInfo capturedException = null;
            try
            {
                var result = await actionAsync().ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            var errorResult = await errorHandlerAsync(capturedException.SourceException).ConfigureAwait(false);
            if (errorResult.RethrowException)
            {
                capturedException.Throw();
            }

            return errorResult.DefaultResult;


        }

        public static async Task<TResult> ExecuteAndHandleErrorAsync<T1, TResult>(Func<T1, Task<TResult>> actionAsync,
            T1 arg1,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<TResult>>> errorHandlerAsync)
        {
            return await ExecuteAndHandleErrorAsync(() => actionAsync(arg1), errorHandlerAsync);
        }

        public static async Task<TResult> ExecuteAndHandleErrorAsync<T1, T2, TResult>(
            Func<T1, T2, Task<TResult>> actionAsync,
            T1 arg1, T2 arg2,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<TResult>>> errorHandlerAsync)
        {
            return await ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2), errorHandlerAsync);
        }

        public static async Task<TResult> ExecuteAndHandleErrorAsync<T1, T2, T3, TResult>(
            Func<T1, T2, T3, Task<TResult>> actionAsync,
            T1 arg1, T2 arg2, T3 arg3,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<TResult>>> errorHandlerAsync)
        {
            return await ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3), errorHandlerAsync);
        }

        public static async Task<TResult> ExecuteAndHandleErrorAsync<T1, T2, T3, T4, TResult>(
            Func<T1, T2, T3, T4, Task<TResult>> actionAsync,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<TResult>>> errorHandlerAsync)
        {
            return await ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3, arg4), errorHandlerAsync);
        }

        public static async Task<TResult> ExecuteAndHandleErrorAsync<T1, T2, T3, T4, T5, TResult>(
            Func<T1, T2, T3, T4, T5, Task<TResult>> actionAsync,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<TResult>>> errorHandlerAsync)
        {
            return await ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3, arg4, arg5), errorHandlerAsync);
        }

        public static async Task<TResult> ExecuteAndHandleErrorAsync<T1, T2, T3, T4, T5, T6, TResult>(
            Func<T1, T2, T3, T4, T5, T6, Task<TResult>> actionAsync,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<TResult>>> errorHandlerAsync)
        {
            return
                await
                    ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3, arg4, arg5, arg6), errorHandlerAsync);
        }

        public static async Task<TResult> ExecuteAndHandleErrorAsync<T1, T2, T3, T4, T5, T6, T7, TResult>(
            Func<T1, T2, T3, T4, T5, T6, T7, Task<TResult>> actionAsync,
            T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7,
            Func<Exception, Task<TryWithAwaitInCatchExcpetionHandleResult<TResult>>> errorHandlerAsync)
        {
            return
                await
                    ExecuteAndHandleErrorAsync(() => actionAsync(arg1, arg2, arg3, arg4, arg5, arg6, arg7),
                        errorHandlerAsync);
        }
    }
}