using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace DME.Base.Cache.MemoryCached
{
    /* This Binder is implemented to work around the dynamical load of assemblies
     * from the application directory by the binary formatter. If you are running
     * a COM instance the binary formatter will look for the assembly in the
     * directory where COM is executed.
     * If you are not running COM this binder may be removed.
     */
    class DME_Binder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type tyType = null;

            string sShortAssemblyName = assemblyName.Split(',')[0];
            Assembly[] ayAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly ayAssembly in ayAssemblies)
            {
                if (sShortAssemblyName == ayAssembly.FullName.Split(',')[0])
                {
                    tyType = ayAssembly.GetType(typeName);
                    break;
                }
            }
            return tyType;
        }
    }
}
