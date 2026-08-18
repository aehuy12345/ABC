using UnityEngine;
using Game.Core;

namespace Game.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string characterSelectSceneName = "02_CharacterSelect";

        public void OnStartClicked()
        {
            SceneLoader.LoadScene(characterSelectSceneName);
        }

        public void OnQuitClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
