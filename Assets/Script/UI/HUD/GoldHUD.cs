using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.UI.HUD
{
    public class GoldHUD : MonoBehaviour
    {
        [SerializeField] private Text goldText;

        private void Update()
        {
            if (goldText == null || GameSession.Instance == null) return;
            goldText.text = GameSession.Instance.Gold.ToString();
        }
    }
}
