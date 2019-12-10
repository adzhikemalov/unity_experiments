using UnityEngine;

namespace DefaultNamespace
{
    public class GameSettings:MonoBehaviour
    {
        public Material BulletMaterial;
        public Material HPMaterial;
        public FloatingJoystick Joystick;
        public static Material BulletMaterialInstance;
        public static Material HPBarMaterialInstance;
        public static  FloatingJoystick JoystickInstance;

        private void Awake()
        {
            BulletMaterialInstance = BulletMaterial;
            HPBarMaterialInstance = HPMaterial;
            JoystickInstance = Joystick;
        }
    }
}