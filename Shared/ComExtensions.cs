using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public static class ComExtensions
    {
        public static Object Action<Object>(this Object @this, Action<Object> action)
        {
            action(@this);

            return @this;
        }
        public static T Map<S, T>(this S @this, Func<S, T> func)
        {
            return func(@this);
        }
    }
}
