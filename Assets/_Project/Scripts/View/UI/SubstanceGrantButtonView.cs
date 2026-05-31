using OneMoreSpoon.Game.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace OneMoreSpoon.View.UI
{
    public sealed class SubstanceGrantButtonView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private string substanceId = "person_worker";

        private SubstanceGrantService grantService;

        public void Initialize(SubstanceGrantService grantService)
        {
            this.grantService = grantService;
        }

        private void Awake()
        {
            if (button == null)
                button = GetComponent<Button>();

            if (button != null)
                button.onClick.AddListener(Grant);
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(Grant);
        }

        private void Grant()
        {
            if (grantService == null)
                return;

            var position = spawnPoint != null
                ? (Vector2)spawnPoint.position
                : (Vector2)transform.position;

            grantService.TryGrantSingleAlive(substanceId, position);
        }
    }
}
