using UnityEngine;

namespace FOIA.Presentation
{
    public sealed class PocKeyboardTestRunner : MonoBehaviour
    {
        [SerializeField] private ProcessRunController processRunController;

        private void Awake()
        {
            if (processRunController == null)
                processRunController = FindFirstObjectByType<ProcessRunController>();
        }

        private void Update()
        {
            if (UnityEngine.InputSystem.Keyboard.current == null)
                return;

            if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
                processRunController?.RunTestProcess();
        }
    }
}
