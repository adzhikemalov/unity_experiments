using UnityEngine;

namespace DefaultNamespace
{
    public class GameSettings:MonoBehaviour
    {
        public Material BulletMaterial;
        public Material HPMaterial;
        public static Material BulletMaterialInstance;
        public static Material HPBarMaterialInstance;

        private void Awake()
        {
            BulletMaterialInstance = BulletMaterial;
            HPBarMaterialInstance = HPMaterial;
        }
    }
}