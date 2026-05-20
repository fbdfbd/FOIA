using FOIA.Definitions;
using FOIA.Runtime;
using UnityEngine;
using VContainer;

namespace FOIA.Presentation
{
    public sealed class PocStateBootstrapper : MonoBehaviour
    {
        [SerializeField] private StarterInventoryEntry[] startingBlocks;

        private ProcessBoardState boardState;

        [Inject]
        public void Construct(ProcessBoardState boardState)
        {
            this.boardState = boardState;
        }

        private void Start()
        {
            if (boardState == null)
                return;

            foreach (var entry in startingBlocks)
            {
                if (entry.block != null)
                    boardState.AddBlock(entry.block.Id, entry.amount);
            }
        }
    }
}
