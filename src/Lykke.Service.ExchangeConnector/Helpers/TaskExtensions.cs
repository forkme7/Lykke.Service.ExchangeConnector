﻿namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(
            this Task<T> task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
                if (task != await Task.WhenAny(task, tcs.Task))
                    throw new OperationCanceledException(cancellationToken);
            return await task;
        }

        public static async Task<bool> AwaitWithTimeout(this Task task, int timeoutMs)
        {
            await Task.WhenAny(Task.Delay(timeoutMs), task);
            return task.IsCompleted;
        }
    }
}
