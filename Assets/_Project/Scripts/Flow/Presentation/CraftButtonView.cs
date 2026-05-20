using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Flow.Presentation
{
    [RequireComponent(typeof(Button))]
    public sealed class CraftButtonView : MonoBehaviour
    {
        [SerializeField] private FoiaProcessSystem processSystem;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            if (processSystem == null)
            {
                processSystem = GraphSceneLookup.FindFirst<FoiaProcessSystem>();
            }
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            processSystem.Craft();
        }
    }
}
