using UnityEngine;
using System.Collections;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Common Singleton implementation
    /// </summary>
    public class Singleton<T> where T : Singleton<T>, new()
    {
        // --- Static Members ---------------------------------------------------------------------------
        /// <summary> Singleton instance </summary>
        public static T instance
        {
            get
            {
                if (_instance == null)
                    CreateInstance();
                return _instance;
            }
        }
        private static T _instance = null;

        // --- Static Methods ---------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        /// <summary> Creates a new instance </summary>
        public static void CreateInstance()
        {
            if (_instance == null)
            {
                _instance = new T();
                _instance.OnInstanceCreated();
            }
#if UNITY_EDITOR
            else
                throw new UnityException("Instance already created");
#endif
        }

        // -----------------------------------------------------------------------------------
        /// <summary> Sets the instance to null </summary>
        public static void DestroyInstance()
        {
            _instance = null;
        }

        // --- Private Methods --------------------------------------------------------------------------
        protected Singleton() { }
    
        // -----------------------------------------------------------------------------------
        protected virtual void OnInstanceCreated() { }
    }
}
