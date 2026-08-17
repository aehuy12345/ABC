using UnityEngine.SceneManagement;

namespace Game.Core
{
    public static class SceneLoader
    {
        public static void LoadScene(string sceneName)
        {
            // Tách riêng thành 1 điểm gọi duy nhất — sau này muốn thêm loading screen
            // hoặc fade-out chỉ cần sửa ở đây, không cần sửa từng nơi gọi SceneManager.
            SceneManager.LoadScene(sceneName);
        }
    }
}
