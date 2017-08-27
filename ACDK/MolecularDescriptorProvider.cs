using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ACDK
{
    [Guid("E2A67919-116D-40F2-8ADB-671ADAFA7B78")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMolecularDescriptorProvider
    {
        [DispId(0x2001)]
        IMolecularDescriptor GetDescriptor(string name);
    }

    [Guid("28779F21-5679-476A-BEB5-D8BA960D9727")]
    [ComDefaultInterface(typeof(IMolecularDescriptorProvider))]
    public class MolecularDescriptorProvider
        : IMolecularDescriptorProvider
    {
        [DispId(0x2001)]
        public IMolecularDescriptor GetDescriptor(string name)
        {
            string className = "ACDK." + name;
            var type = Assembly.GetExecutingAssembly().GetType(className);
            var ctor = type.GetConstructor(Type.EmptyTypes);
            var obj = ctor.Invoke(new object[0]);
            return (IMolecularDescriptor)obj;
        }
    }
}
