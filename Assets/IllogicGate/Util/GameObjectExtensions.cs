using System.Collections.Generic;
using UnityEngine;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    public static class GameObjectExtensions 
    {
        // --- Static Properties ------------------------------------------------------------------------
        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        public static T GetInterface<T>(this Component self) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new System.Exception(typeof(T).Name + " is not an interface");

            Component[] components = self.GetComponents<Component>();
            foreach (Component c in components)
            {
                T iface = c as T;
                if (iface != null)
                    return iface;
            }

            return null;
        }

        // -----------------------------------------------------------------------------------
        public static T [] GetInterfaces<T>(this Component self) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new System.Exception(typeof(T).Name + " is not an interface");

            Component[] components = self.GetComponents<Component>();
            List<T> ifaces = new List<T>();
            foreach (Component c in components)
            {
                T iface = c as T;
                if (iface != null)
                    ifaces.Add(iface);
            }

            return ifaces.ToArray();
        }
    }
}