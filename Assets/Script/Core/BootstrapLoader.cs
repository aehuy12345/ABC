using UnityEngine;

namespace Game.Core
{
    /// <summary>
    /// Gắn trên GameObject [Systems] (hoặc 1 object riêng) trong scene 00_Bootstrap.
    /// Dùng Start() (không phải Awake()) để chuyển scene, vì Unity đảm bảo mọi Awake()
    /// trong scene hiện tại (kể cả GameSession/PoolManager/AudioManager) đã chạy xong
    /// trước khi bất kỳ Start() nào chạy — nhờ vậy Instance của các singleton chắc chắn
    /// đã sẵn sàng trước khi scene MainMenu load và có thể cần dùng tới chúng.
    /// </summary>
    public class BootstrapLoader : MonoBehaviour
    {
        [SerializeField] private string nextSceneName = "01_MainMenu";

        private void Start()
        {
            SceneLoader.LoadScene(nextSceneName);
        }
    }
}
