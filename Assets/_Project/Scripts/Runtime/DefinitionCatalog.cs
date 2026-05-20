using System.Collections.Generic;
using FOIA.Core;
using FOIA.Definitions;
using UnityEngine;

namespace FOIA.Runtime
{
    [CreateAssetMenu(menuName = "FOIA/Runtime/Definition Catalog")]
    public sealed class DefinitionCatalog : ScriptableObject
    {
        [SerializeField] private ComplaintDefinition[] complaints;
        [SerializeField] private NodeDefinition[] nodes;
        [SerializeField] private EdgeDefinition[] edges;
        [SerializeField] private EdgeBlockDefinition[] edgeBlocks;
        [SerializeField] private CraftingRecipeDefinition[] recipes;
        [SerializeField] private StaffDefinition[] staff;
        [SerializeField] private AgencyDefinition[] agencies;

        private Dictionary<ComplaintTypeId, ComplaintDefinition> complaintMap;
        private Dictionary<EdgeId, EdgeDefinition> edgeMap;
        private Dictionary<EdgeBlockId, EdgeBlockDefinition> edgeBlockMap;
        private Dictionary<StaffId, StaffDefinition> staffMap;
        private Dictionary<AgencyId, AgencyDefinition> agencyMap;

        public IReadOnlyList<ComplaintDefinition> Complaints => complaints;
        public IReadOnlyList<NodeDefinition> Nodes => nodes;
        public IReadOnlyList<EdgeDefinition> Edges => edges;
        public IReadOnlyList<EdgeBlockDefinition> EdgeBlocks => edgeBlocks;
        public IReadOnlyList<CraftingRecipeDefinition> Recipes => recipes;
        public IReadOnlyList<StaffDefinition> Staff => staff;
        public IReadOnlyList<AgencyDefinition> Agencies => agencies;

        public ComplaintDefinition GetComplaint(ComplaintTypeId id)
        {
            EnsureMaps();
            return complaintMap[id];
        }

        public EdgeDefinition GetEdge(EdgeId id)
        {
            EnsureMaps();
            return edgeMap[id];
        }

        public EdgeBlockDefinition GetEdgeBlock(EdgeBlockId id)
        {
            EnsureMaps();
            return edgeBlockMap[id];
        }

        public StaffDefinition GetStaff(StaffId id)
        {
            EnsureMaps();
            return staffMap[id];
        }

        public AgencyDefinition GetAgency(AgencyId id)
        {
            EnsureMaps();
            return agencyMap[id];
        }

        private void EnsureMaps()
        {
            if (edgeMap != null)
                return;

            complaintMap = new Dictionary<ComplaintTypeId, ComplaintDefinition>();
            edgeMap = new Dictionary<EdgeId, EdgeDefinition>();
            edgeBlockMap = new Dictionary<EdgeBlockId, EdgeBlockDefinition>();
            staffMap = new Dictionary<StaffId, StaffDefinition>();
            agencyMap = new Dictionary<AgencyId, AgencyDefinition>();

            foreach (var complaint in complaints)
                complaintMap[complaint.Id] = complaint;

            foreach (var edge in edges)
                edgeMap[edge.Id] = edge;

            foreach (var block in edgeBlocks)
                edgeBlockMap[block.Id] = block;

            foreach (var member in staff)
                staffMap[member.Id] = member;

            foreach (var agency in agencies)
                agencyMap[agency.Id] = agency;
        }
    }
}
