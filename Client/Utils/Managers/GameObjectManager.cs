using System.Collections.Generic;
using UnityEngine;

namespace Blaze.Utils.Managers
{
    internal static class GameObjectManager
    {
        internal static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                return gameObject.AddComponent<T>();
            }
            return component;
        }

        internal static List<T> FindAllComponentsInGameObject<T>(GameObject gameObject, bool includeInactive = true, bool searchParent = true, bool searchChildren = true) where T : class
        {
            List<T> components = new();
            foreach (T component in gameObject.GetComponents<T>())
            {
                components.Add(component);
            }
            if (searchParent == true)
            {
                foreach (T component in gameObject.GetComponentsInParent<T>(includeInactive))
                {
                    components.Add(component);
                }
            }
            if (searchChildren == true)
            {
                foreach (T component in gameObject.GetComponentsInChildren<T>(includeInactive))
                {
                    components.Add(component);
                }
            }
            return components;
        }
    }
}
