using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class GameSettings:MonoBehaviour
    {
        public Material BulletMaterial;
        public Material HPMaterial;
        public FloatingJoystick Joystick;
        public Image ShootingReady;
        public static Material BulletMaterialInstance;
        public static Material HPBarMaterialInstance;
        public static  FloatingJoystick JoystickInstance;
        public static  Image ShootingReadyInstance;
        public static int FrameCount;
        private void Awake()
        {
            BulletMaterialInstance = BulletMaterial;
            HPBarMaterialInstance = HPMaterial;
            JoystickInstance = Joystick;
            ShootingReadyInstance = ShootingReady;
        }

        private void Update()
        {
            FrameCount++;
        }
    }
}