/*
The MIT License (MIT)
Copyright (c) 2016-2022 Maksim Volkau
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included 
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

namespace PaymentGateway.Shared;

public static class DeclarativeExtensions
{
    /// <summary>
    /// Forward pipe operator.
    /// </summary> 
    public static R To<T, R>(this T x, Func<T, R> map) => map(x);

    /// <summary>
    /// Forward pipe operator with possible additional state.
    /// </summary> 
    public static R To<T, S, R>(this T x, S state, Func<T, S, R> map) => map(x, state);

    /// <summary>
    /// Cast to the R type with the forward pipe operator.
    /// </summary> 
    public static R To<R>(this object x) => (R)x;

    /// <summary>
    /// Forward pipe operator with side effect propagating the original `x` value
    /// </summary> 
    public static T Do<T>(this T x, Action<T> sideEffect)
    {
        sideEffect(x);
        return x;
    }

    /// <summary>
    /// Forward pipe operator with possible additional state + possible state.
    /// </summary> 
    public static T Do<T, S>(this T x, S state, Action<T, S> sideEffect)
    {
        sideEffect(x, state);
        return x;
    }
    
    /// <summary>Convert item into array with single item.</summary>
    public static T[] One<T>(this T one) => new T[1]{ one };
}
