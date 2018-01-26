using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

namespace Seido
{
    public class VideoWall : MonoBehaviour
    {
        enum DisplayMode { Landscape, DoubleVertical }

        [SerializeField] VideoClip _clip;
        [SerializeField] float[] _cuePoints = new float[] { 0 };
        [SerializeField] DisplayMode _displayMode;
        [SerializeField] float _width = 1;
        [SerializeField] Camera[] _targetCameras;

        [HideInInspector, SerializeField] Shader _shader;

        VideoPlayer _player;
        Material _material;
        CommandBuffer _blit1, _blit2;

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

            // Set up the blit shader.
            _material = new Material(_shader);

            var dest = BuiltinRenderTextureType.CameraTarget;
            if (_displayMode == DisplayMode.Landscape)
            {
                // Landscape mode blit command
                _blit1 = new CommandBuffer();
                _blit1.name = gameObject.name;
                _blit1.Blit(_player.texture, dest, _material, 0);

                // Add the blit command to the target cameras.
                foreach (var cam in _targetCameras)
                    cam.AddCommandBuffer(CameraEvent.AfterImageEffects, _blit1);
            }
            else
            {
                // Vertical mode blit command (left half)
                _blit1 = new CommandBuffer();
                _blit1.name = gameObject.name;
                _blit1.Blit(_player.texture, dest, _material, 1);

                // Vertical mode blit command (right half)
                _blit2 = new CommandBuffer();
                _blit2.name = gameObject.name;
                _blit2.Blit(_player.texture, dest, _material, 2);

                // Add the blit command to the target cameras.
                for (var i = 0; i < _targetCameras.Length; i++)
                    _targetCameras[i].AddCommandBuffer(
                        CameraEvent.AfterImageEffects,
                        (i & 1) == 0 ? _blit1 : _blit2
                    );
            }
        }

        void OnDisable()
        {
            // Remove the blit command from the target camera.
            if (_blit1 != null)
            {
                foreach (var cam in _targetCameras)
                    if (cam != null) cam.RemoveCommandBuffer(CameraEvent.AfterImageEffects, _blit1);

                _blit1.Release();
                _blit1 = null;
            }

            if (_blit2 != null)
            {
                foreach (var cam in _targetCameras)
                    if (cam != null) cam.RemoveCommandBuffer(CameraEvent.AfterImageEffects, _blit2);

                _blit2.Release();
                _blit2 = null;
            }

            // Destroy the blit shader material.
            Destroy(_material);
            _material = null;

            // Stop playing back and destroy the video player.
            _player.Stop();
            Destroy(_player);
            _player = null;
        }

        void Update()
        {
            // Close if the Q key was pressed down.
            if (Input.GetKeyDown(KeyCode.Q))
            {
                gameObject.SetActive(false);
                return;
            }

            // Update the material properties.
            _material.SetTexture("_MainTex", _player.texture);
            _material.SetFloat("_Width", _width);

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
