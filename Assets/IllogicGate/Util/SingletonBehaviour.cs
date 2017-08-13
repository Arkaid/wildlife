using UnityEngine;
using System.Collections.Generic;

namespace IllogicGate
{
    // --- Class Declaration ------------------------------------------------------------------------
    /// <summary>
    /// Common Singleton implementation (Monobehaviours)
    /// </summary>
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>, new()
    {
        // --- Static Members ---------------------------------------------------------------------------
        /// <summary> Currently running instance. Returns null if the instance hasn't been initialized yet </summary>
        public static T instance
        {
            get
            {
                // try to get it from scene
                if (_instance == null)
                    _instance = Util.GetComponentInScene<T>(true);
                // try to load it from resources
                if (_instance == null)
                    _instance = Instantiate(Resources.Load<T>(typeof(T).Name));
                return _instance;
            }
        }
        private static T _instance;

        // --- Static Methods ---------------------------------------------------------------------------
        

        // --- MonoBehaviour ----------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------
        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
                throw new UnityException("Instance already created");
            _instance = this as T;
        }

        // -----------------------------------------------------------------------------------
        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}
