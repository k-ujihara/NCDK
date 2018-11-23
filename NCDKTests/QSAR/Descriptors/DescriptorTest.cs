/* Copyright (C) 2007  Egon Willighagen <egonw@users.sf.net>
 *
 * Contact: cdk-devel@lists.sourceforge.net
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public License
 * as published by the Free Software Foundation; either version 2.1
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using NCDK.Templates;
using System;
using System.Linq;
using System.Reflection;

namespace NCDK.QSAR.Descriptors
{
    /// <summary>
    /// Tests for molecular descriptors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // @cdk.module test-qsar
    public abstract class DescriptorTest<T> : CDKTestCase where T : IDescriptor
    {
        private ConstructorInfo ctor;
        private ParameterInfo[] parametersInfos;
        private readonly object syncctor = new object();

        protected virtual T CreateDescriptor(IAtomContainer mol)
        {
            if (this.ctor == null)
                lock (syncctor)
                {
                    var ctors = typeof(T).GetConstructors();
                    foreach (var ctor in ctors)
                    {
                        var parametersInfos = ctor.GetParameters();
                        if (parametersInfos.Length < 1)
                            continue;
                        var first = parametersInfos.First();
                        if (!typeof(IAtomContainer).IsAssignableFrom(first.ParameterType))
                            continue;
                        var isAllHaveDefault = !parametersInfos.Skip(1).Any(n => !n.HasDefaultValue);
                        if (isAllHaveDefault)
                        {
                            // found it 
                            this.ctor = ctor;
                            this.parametersInfos = parametersInfos;
                            break;
                        }
                    }
                    if (this.ctor == null)
                        throw new Exception($"Could not found {typeof(T).FullName}.{typeof(T).Name}({nameof(IAtomContainer)})");
                }
            var parameters = new object[parametersInfos.Length];
            parameters[0] = mol;
            for (int i = 1; i < parametersInfos.Length; i++)
                parameters[i] = parametersInfos[i].DefaultValue;
            return (T)ctor.Invoke(parameters);
        }

        private T descriptor;
        private readonly object sync = new object();

        protected virtual T Descriptor
        {
            get
            {
                if (descriptor == null)
                    lock (sync)
                    {
                        descriptor = CreateDescriptor(DefaultMolecular);
                    }
                return descriptor;
            }
        }

        protected static IAtomContainer Water { get; } = TestMoleculeFactory.MakeWater();

        protected virtual IAtomContainer DefaultMolecular => Water;
    }
}
