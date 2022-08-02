using System;
using UnhollowerRuntimeLib;
using UnityEngine;

namespace Blaze.Utils.Managers
{
    public static class ComponentManager
    {
        public static void RegisterIl2Cpp<t>() where t : Component
        {
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<t>();
            }
            catch (Exception ex) 
            {
                Logs.Error("RegisterIl2Cpp", ex);
            }
        }
    }
}
