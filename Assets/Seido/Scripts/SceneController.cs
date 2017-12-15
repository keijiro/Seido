using UnityEngine;

namespace Seido
{
    class SceneController : MonoBehaviour
    {
        [SerializeField] GameObject[] _videoPlayers;

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

        void Update()
        {
            for (var i = 0; i < _videoPlayers.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.F1 + i))
                {
                    _videoPlayers[i].SetActive(true);
                    break;
                }
            }
        }
    }
}
