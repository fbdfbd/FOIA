using System.Collections.Generic;
using FOIA.Core;
using FOIA.Definitions;
using FOIA.Runtime;
using R3;
using UnityEngine;
using VContainer;

namespace FOIA.Presentation
{
    public sealed class ProcessRunController : MonoBehaviour
    {
        [SerializeField] private ComplaintDefinition testComplaint;
        [SerializeField] private StaffDefinition selectedStaff;
        [SerializeField] private AgencyDefinition selectedAgency;
        [SerializeField] private EdgeDefinition[] path;

        private readonly Subject<ProcessResult> processCompleted = new();
        private ProcessBoardState boardState;
        private ByproductWallet wallet;
        private ProcessSimulator simulator;

        public Observable<ProcessResult> ProcessCompleted => processCompleted;

        public void ConfigureRuntime(ProcessBoardState boardState, ByproductWallet wallet, ProcessSimulator simulator)
        {
            this.boardState = boardState;
            this.wallet = wallet;
            this.simulator = simulator;
        }

        [Inject]
        public void Construct(ProcessBoardState boardState, ByproductWallet wallet, ProcessSimulator simulator)
        {
            this.boardState = boardState;
            this.wallet = wallet;
            this.simulator = simulator;
        }

        public void RunTestProcess()
        {
            if (testComplaint == null || simulator == null)
                return;

            var edgeIds = new List<EdgeId>(path.Length);
            foreach (var edge in path)
            {
                if (edge != null)
                    edgeIds.Add(edge.Id);
            }

            var result = simulator.Run(testComplaint.Id, edgeIds, boardState, selectedStaff, selectedAgency);
            foreach (var effect in result.Effects)
                boardState?.ApplyEffect(effect);

            foreach (var byproduct in result.Byproducts)
                wallet?.Add(byproduct.Key, byproduct.Value);

            processCompleted.OnNext(result);
        }

        private void OnDestroy()
        {
            processCompleted.Dispose();
        }
    }
}
