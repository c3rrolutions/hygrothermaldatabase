using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace Database.Extensions;

public static class ResultExtensions
{
    [Pure]
    public static bool Failed<TData, TError>(
        this Result<TData, TError> result,
        [NotNullWhen(false)] out TData? data,
        [NotNullWhen(true)] out TError? error
    )
    where TData : class
    where TError : class
    {
        switch (result)
        {
            case Result<TData, TError>.Data dataResult:
                data = dataResult.Value;
                error = null;
                return false;
            case Result<TData, TError>.Error errorResult:
                data = null;
                error = errorResult.Value;
                return true;
            default:
                throw new InvalidOperationException("Impossible!");
        }
    }

    [Pure]
    public static bool Failed<TData, TError>(
        this Result<TData, TError> result,
        [NotNullWhen(true)] out TError? error
    )
    where TError : class
    {
        switch (result)
        {
            case Result<TData, TError>.Data:
                error = null;
                return false;
            case Result<TData, TError>.Error errorResult:
                error = errorResult.Value;
                return true;
            default:
                throw new InvalidOperationException("Impossible!");
        }
    }

    [Pure]
    public static bool Succeeded<TData, TError>(
        this Result<TData, TError> result,
        [NotNullWhen(true)] out TData? data,
        [NotNullWhen(false)] out TError? error
    )
    where TData : class
    where TError : class
    {
        switch (result)
        {
            case Result<TData, TError>.Data dataResult:
                data = dataResult.Value;
                error = null;
                return true;
            case Result<TData, TError>.Error errorResult:
                data = null;
                error = errorResult.Value;
                return false;
            default:
                throw new InvalidOperationException("Impossible!");
        }
    }

    [Pure]
    public static bool Succeeded<TData, TError>(
        this Result<TData, TError> result,
        [NotNullWhen(true)] out TData? data
    )
    where TData : class
    {
        switch (result)
        {
            case Result<TData, TError>.Data dataResult:
                data = dataResult.Value;
                return true;
            case Result<TData, TError>.Error:
                data = null;
                return false;
            default:
                throw new InvalidOperationException("Impossible!");
        }
    }
}