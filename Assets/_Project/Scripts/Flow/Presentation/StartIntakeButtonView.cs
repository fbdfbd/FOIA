using FOIA.Flow.Runtime;
using FOIA.Graph.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace FOIA.Flow.Presentation
{
    [RequireComponent(typeof(Button))]
    public sealed class StartIntakeButtonView : MonoBehaviour
    {
        [SerializeField] private FoiaProcessSystem processSystem;
        [SerializeField] private ProcessStateStore processState;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            if (processSystem == null)
            {
                processSystem = GraphSceneLookup.FindFirst<FoiaProcessSystem>();
            }

            if (processState == null)
            {
                processState = GraphSceneLookup.FindFirst<ProcessStateStore>();
            }
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnClicked);

            if (processState != null)
            {
                processState.StateChanged += Refresh;
            }

            Refresh();
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClicked);

            if (processState != null)
            {
                processState.StateChanged -= Refresh;
            }
        }

        private void OnClicked()
        {
            processSystem.StartIntake();
        }

        private void Refresh()
        {
            button.interactable = processState != null
                && !string.IsNullOrEmpty(processState.EquippedStaffId)
                && processState.CurrentDocument == null;
        }
    }
}
