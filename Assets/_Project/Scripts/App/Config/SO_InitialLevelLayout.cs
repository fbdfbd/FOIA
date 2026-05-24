using UnityEngine;

namespace OneMoreSpoon.App.Config
{
    [CreateAssetMenu(
        fileName = "SO_InitialLevelLayout",
        menuName = "OneMoreSpoon/App/Initial Level Layout")]
    public sealed class SO_InitialLevelLayout : ScriptableObject
    {
        [Header("Nodes")]
        [SerializeField] private InitialNodeSpawn[] initialNodes = new InitialNodeSpawn[0];

        [Header("Substances")]
        [SerializeField] private InitialSubstanceStack[] initialSubstanceStacks = new InitialSubstanceStack[0];

        public InitialNodeSpawn[] InitialNodes => initialNodes;
        public InitialSubstanceStack[] InitialSubstanceStacks => initialSubstanceStacks;
    }
}
