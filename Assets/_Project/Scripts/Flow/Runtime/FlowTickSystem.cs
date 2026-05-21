using System.Collections;
using FOIA.Graph.Runtime;
using UnityEngine;

namespace FOIA.Flow.Runtime
{
    public sealed class FlowTickSystem : MonoBehaviour
    {
        [SerializeField] private float tickInterval = 1.5f;
        [SerializeField] private FoiaProcessSystem processSystem;

        private void Awake()
        {
            if (processSystem == null)
            {
                processSystem = GraphSceneLookup.FindFirst<FoiaProcessSystem>();
            }
        }

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(tickInterval);
                processSystem?.AutoTick();
            }
        }
    }
}
