/*
 * Copyright (C) 2007 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace NCDK.Common.Base
{
    /// <summary>
    /// A transformation from one object to another. For example, a 
    /// <c>StringToIntegerFunction</c> may implement
    /// <c>Function&lt;string,int&gt;</c> and transform integers in
    /// <c>string</c> format to <c>int</c> format.
    ///
    /// <para>The transformation on the source object does not necessarily result in
    /// an object of a different type.  For example, a
    /// <c>FarenheitToCelsiusFunction</c> may implement
    /// <c>Function&lt;float,float&gt;</c>.
    /// </para>
    /// <para>Implementations which may cause side effects upon evaluation are strongly
    /// encouraged to state this fact clearly in their API documentation.
    /// </para>
    /// </summary>
    /// <typeparam name="F">the type of the function input</typeparam>
    /// <typeparam name="T">the type of the function output</typeparam>
    // @author Kevin Bourrillion
    // @author Scott Bonneau
    public interface Function<F, T>
    {
        /// <summary>
        /// Applies the function to an object of type <typeparamref name="F"/>, resulting in an object
        /// of type <typeparamref name="T"/>.  Note that types <typeparamref name="F"/> and <typeparamref name="T"/> may or may not
        /// be the same.
        /// </summary>
        /// <param name="from">the source object</param>
        /// <returns>the resulting object</returns>
        T Apply(F from);
    }
}
