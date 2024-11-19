using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public static class Cancellation
{
    public static async UniTask<T> WithCancellation<T>(this UniTask<T> task, CancellationToken cancellationToken) {

        var tcs = new UniTaskCompletionSource<object>();

        using (cancellationToken.Register(state => {

            ((UniTaskCompletionSource<object>)state).TrySetResult(null);

        },
        tcs)) {

            var (idx, successTask, failTask) = await UniTask.WhenAny(task, tcs.Task);
            
            if (idx == 1) {

                throw new OperationCanceledException(cancellationToken);
            
            }

            return successTask;
        }

    }

    public static async UniTask WithCancellation(this UniTask task, CancellationToken cancellationToken) {

        var tcs = new UniTaskCompletionSource();

        using (cancellationToken.Register(state => {

            ((UniTaskCompletionSource)state).TrySetResult();

        },
        tcs)) {

            var idx = await UniTask.WhenAny(task, tcs.Task);

            if (idx == 1) {

                Debug.Log("Task Cancel");

                throw new OperationCanceledException(cancellationToken);

            }

            await task;
        }

    }

}
