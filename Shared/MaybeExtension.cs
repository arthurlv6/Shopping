using System;
using System.Threading.Tasks;

namespace TSP.Shared
{
    public static class MaybeExtension
    {
        public static Maybe<K> Railway<T, K>(this Maybe<T> result, Func<Maybe<T>, Maybe<K>> func)
        {
            if (result.IsFailure)
                return Maybe.Fail<K>(result.Error,result.Exception);
            return func(result);
        }
        public static async Task<Maybe<K>> RailwayAsync<T, K>(this Maybe<T> result, Func<Maybe<T>, Task<Maybe<K>>> func)
        {
            if (result.IsFailure)
                return Maybe.Fail<K>(result.Error, result.Exception);
            return await func(result);
        }
    }
}
