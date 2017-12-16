using UnityEngine;
using Klak.Chromatics;

namespace Seido
{
    class SceneController : MonoBehaviour
    {
        [SerializeField] CosineGradient[] _gradients;
        [SerializeField] GameObject[] _videoPlayers;

        PostFx _postFx;

        static readonly KeyCode[] _gradientKeys = {
            KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T,
            KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P
        };

        bool CheckVideoPlayerActive()
        {
            foreach (var go in _videoPlayers)
                if (go.activeInHierarchy) return true;
            return false;
        }

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

            _postFx = FindObjectOfType<PostFx>();
        }

        void Update()
        {
            if (CheckVideoPlayerActive()) return;

            for (var i = 0; i < _videoPlayers.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.F1 + i))
                {
                    _videoPlayers[i].SetActive(true);
                    break;
                }
            }

            for (var i = 0; i < _gradients.Length; i++)
            {
                if (Input.GetKeyDown(_gradientKeys[i]))
                {
                    _postFx.gradient = _gradients[i];
                    break;
                }
            }
        }
    }
}
