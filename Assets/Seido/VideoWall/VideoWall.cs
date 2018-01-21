using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

namespace Seido
{
    public class VideoWall : MonoBehaviour
    {
        [SerializeField] VideoClip _clip;
        [SerializeField] float[] _cuePoints = new float[] { 0 };
        [SerializeField] Transform _wallRoot;

        VideoPlayer _player;
        Renderer[] _renderers;
        MaterialPropertyBlock _sheet;

        void OnEnable()
        {
            // Create a video player.
            _player = gameObject.AddComponent<VideoPlayer>();
            _player.hideFlags = HideFlags.NotEditable;

            // Video player configuration
            _player.playOnAwake = false;
            _player.audioOutputMode = VideoAudioOutputMode.None;
            _player.renderMode = VideoRenderMode.APIOnly;
            _player.waitForFirstFrame = false;

            // Prepare the clip for playing.
            _player.clip = _clip;
            _player.Prepare();

            // Enumerate the wall renderers.
            _renderers = _wallRoot.GetComponentsInChildren<Renderer>();

            // Initialize the material sheet.
            _sheet = new MaterialPropertyBlock();
        }

        void OnDisable()
        {
            // Stop playing back and destroy the video player.
            _player.Stop();
            Destroy(_player);
            _player = null;

            // Hide the wall renderers.
            foreach (var r in _renderers) if (r != null) r.enabled = false;

            _renderers = null;
            _sheet = null;
        }

        void Update()
        {
            // Close if the Q key was pressed down.
            if (Input.GetKeyDown(KeyCode.Q))
            {
                gameObject.SetActive(false);
                return;
            }

            // Update the wall renderers (only when the video texture is ready).
            if (_player.texture != null)
            {
                _sheet.SetTexture("_MainTex", _player.texture);
                foreach (var r in _renderers)
                {
                    r.enabled = true;
                    r.SetPropertyBlock(_sheet);
                }
            }

            // Playback start
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _player.time = _cuePoints[0];
                _player.Play();
            }

            // Cue point jump
            for (var i = 0; i < _cuePoints.Length; i++)
            {
                if ((i < 9 && Input.GetKeyDown(KeyCode.Alpha1 + i)) ||
                    (i == 9 && Input.GetKeyDown(KeyCode.Alpha0)))
                {
                    _player.time = _cuePoints[i];
                    _player.Play();
                    break;
                }
            }

            // Playback speed adjustment
            if (Input.GetKey(KeyCode.Z))
                _player.playbackSpeed = 0.75f;
            else if (Input.GetKey(KeyCode.X))
                _player.playbackSpeed = 1.5f;
            else
                _player.playbackSpeed = 1;
        }
    }
}
