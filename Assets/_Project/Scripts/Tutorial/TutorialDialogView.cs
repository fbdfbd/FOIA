using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OneMoreSpoon.View.UI
{
    public sealed class TutorialDialogView : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;
        [SerializeField] private GameObject mainMenuButton;

        private void Awake()
        {
            mainMenuButton.SetActive(false);
        }

        public void ShowMessage(string message)
        {
            messageText.text = message;
        }

        public void ShowComplete(string message)
        {
            messageText.text = message;
            mainMenuButton.SetActive(true);
        }

        public void MoveToMain()
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}