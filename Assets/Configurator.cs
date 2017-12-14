using UnityEngine;

namespace Seido
{
    class Configurator : MonoBehaviour
    {
        void Start()
        {
            // Hide cursor at runtime.
            #if !UNITY_EDITOR
            Cursor.visible = false;
            #endif

            // Try to activate the last two displays.
            var displayCount = Display.displays.Length;
            if (displayCount > 1)
            {
                Display.displays[displayCount - 1].Activate();
                Display.displays[displayCount    ].Activate();
            }
        }
    }
}
