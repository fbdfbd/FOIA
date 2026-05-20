using UnityEngine;

namespace FOIA.Graph.Runtime
{
    internal static class GraphSceneLookup
    {
        public static T FindFirst<T>() where T : Object
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindFirstObjectByType<T>();
#else
            return Object.FindObjectOfType<T>();
#endif
        }
    }
}
