using UnityEngine;
using Klak.Chromatics;

namespace Seido
{
    class SceneController : MonoBehaviour
    {
        #region Editable properties

        [SerializeField] CosineGradient[] _gradients;
        [SerializeField] MeshRenderer _mainRender;
        [SerializeField] GameObject[] _effectGroups;
        [SerializeField] GameObject[] _videoPlayers;

        #endregion

        #region Private variables and methods

        static readonly KeyCode[] _fxKeys = {
            KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, 
            KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8,
            KeyCode.Alpha9, KeyCode.Alpha0, KeyCode.Minus, KeyCode.Equals
        };

        static readonly KeyCode[] _gradientKeys = {
            KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T,
            KeyCode.Y, KeyCode.U, KeyCode.I, KeyCode.O, KeyCode.P
        };

        static readonly KeyCode[] _wallFxKeys = {
            KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G,
            KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L
        };

        FxController[] _fxControllers;
        WallFx[] _wallFx;
        PostFx _postFx;

        bool CheckVideoPlayerActive()
        {
            foreach (var go in _videoPlayers)
                if (go.activeInHierarchy) return true;
            return false;
        }

        #endregion

        #region MonoBehaviour implementation

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
                Display.displays[displayCount - 2].Activate();
                Display.displays[displayCount - 1].Activate();
            }

            // Initialize the effect controllers.
            _fxControllers = new FxController[_effectGroups.Length];
            for (var i = 0; i < _effectGroups.Length; i++)
                _fxControllers[i] = new FxController(_effectGroups[i]);

            // Retrieve the reference to the wall fx.
            _wallFx = FindObjectsOfType<WallFx>();

            // Retrieve the reference to the main camera post fx.
            _postFx = FindObjectOfType<PostFx>();
        }

        void Update()
        {
            // Update the effect groups.
            foreach (var fx in _fxControllers) fx.Update();

            // Disable the main render quad and key input while playing videos.
            var videoPlaying = CheckVideoPlayerActive();
            _mainRender.enabled = !videoPlaying;
            if (videoPlaying) return;

            // Key input: Video players (function keys)
            for (var i = 0; i < _videoPlayers.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.F1 + i))
                {
                    _videoPlayers[i].SetActive(true);
                    // Disable wall fx.
                    foreach (var fx in _wallFx) fx.effectType = 0;
                    break;
                }
            }

            // Key input: Effect groups (alpha numeric keys)
            for (var i = 0; i < _fxControllers.Length; i++)
            {
                if (Input.GetKeyDown(_fxKeys[i]))
                    _fxControllers[i].Toggle();
            }

            // Key input: Gradients (QWERTY row)
            for (var i = 0; i < _gradients.Length; i++)
            {
                if (Input.GetKeyDown(_gradientKeys[i]))
                {
                    _postFx.gradient = _gradients[i];
                    break;
                }
            }

            // Key input: Wall effects (ASDF row)
            for (var i = 0; i < _wallFx.Length; i++)
            {
                if (Input.GetKeyDown(_wallFxKeys[i]))
                {
                    foreach (var fx in _wallFx) fx.effectType = i;
                    break;
                }
            }
        }

        #endregion

        #region Effect group controller class

        class FxController
        {
            GameObject _root;
            Kvant.SprayMV[] _sprays;
            Kvant.SwarmMV[] _swarms;
            Kvant.Warp[] _warps;
            Kvant.Line[] _lines;
            BoidController _boids;

            float _swarmWidth;
            float _lineScale;

            bool _active;
            float _throttle;

            public void Toggle()
            {
                _active = !_active;
                _throttle = Mathf.Clamp01(_throttle);

                if (_boids != null)
                    if (_active) _boids.SpawnAll(); else _boids.KillAll();
            }

            public FxController(GameObject root)
            {
                _root = root;

                _sprays = root.GetComponentsInChildren<Kvant.SprayMV>();
                _swarms = root.GetComponentsInChildren<Kvant.SwarmMV>();
                _warps = root.GetComponentsInChildren<Kvant.Warp>();
                _lines = root.GetComponentsInChildren<Kvant.Line>();
                _boids = root.GetComponent<BoidController>();

                if (_swarms.Length > 0) _swarmWidth = _swarms[0].lineWidth;
                if (_lines.Length > 0) _lineScale = _lines[0].baseScale;

                _active = false;
                _throttle = -10;
            }

            public void Update()
            {
                _throttle += (_active ? 0.6f : -0.6f) * Time.deltaTime;

                var clamped = Mathf.Clamp01(_throttle);
                foreach (var fx in _sprays) fx.throttle = clamped;
                foreach (var fx in _warps) fx.throttle = clamped;
                foreach (var fx in _lines) fx.baseScale = _lineScale * clamped;

                foreach (var fx in _swarms)
                {
                    fx.lineWidth = _swarmWidth * clamped * clamped;
                    fx.throttle = clamped;
                }

                _root.SetActive(_throttle > -10);
            }
        }

        #endregion
    }
}
