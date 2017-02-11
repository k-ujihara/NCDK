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
using System;

namespace NCDK.Common.Base
{
    /// <summary>
    /// Determines a true or false value for a given input.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // @author Kevin Bourrillion
    // @since 2 (imported from Google Collections Library)
    public interface Predicate<T>
    {
        /// <summary>
        /// Returns the result of applying this predicate to <paramref name="inout"/>. This method is <i>generally
        /// expected</i>, but not absolutely required, to have the following properties:
        /// <list type="bullet">
        /// <item>Its execution does not cause any observable side effects.</item>
        /// <item>The computation is <i>consistent with equals</i>; that is, 
        /// <see cref="Object.Equals(object, object)"/> implies that <c> predicate.Apply(a) == predicate.Apply(b)</c>.</item>
        /// </list>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="input"/> is <see langword="null"/> and this predicate does not accept null arguments</exception>
        bool Apply(T input);
    }
}