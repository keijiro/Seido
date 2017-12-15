using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

namespace Seido
{
    public class VideoWall : MonoBehaviour
    {
        [SerializeField] VideoClip _clip;
        [SerializeField] Camera _targetCamera;
        [SerializeField] float[] _cuePoints = new float[] { 0 };
        [SerializeField] Vector2 _repeat = Vector2.one;
        [SerializeField] Vector2 _scroll = Vector2.zero;

        [HideInInspector, SerializeField] Shader _shader;

        VideoPlayer _player;
        Vector2 _offset;
        float _opacity;
        bool _closing;

        Material _material;
        CommandBuffer _blitCommand;

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

            // Add the blit command to the target camera.
            _blitCommand = new CommandBuffer();
            _blitCommand.name = gameObject.name;
            _blitCommand.Blit(_player.texture, BuiltinRenderTextureType.CameraTarget, _material, 0);

            if (_targetCamera != null)
                _targetCamera.AddCommandBuffer( CameraEvent.AfterImageEffects, _blitCommand);

            // Start fading in.
            _opacity = 0;
            _closing = false;
        }

        void OnDisable()
        {
            // Remove the blit command from the target camera.
            if (_targetCamera != null)
                _targetCamera.RemoveCommandBuffer(CameraEvent.AfterImageEffects, _blitCommand);

            _blitCommand.Release();
            _blitCommand = null;

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
            // Start closing when pressing down the Q key.
            if (Input.GetKeyDown(KeyCode.Q)) _closing = true;

            if (_closing)
            {
                // Fade out and disable itself at the end.
                _opacity -= Time.deltaTime;
                if (_opacity <= 0)
                {
                    gameObject.SetActive(false);
                    return;
                }
            }
            else
            {
                // Fade in
                _opacity = Mathf.Clamp01(_opacity + Time.deltaTime);
            }

            // Tile scrolling
            _offset += Time.deltaTime * _scroll;

            // Update the material properties.
            _material.SetVector("_Repeat", _repeat);
            _material.SetVector("_Offset", _offset);
            _material.SetTexture("_MainTex", _player.texture);
            _material.SetFloat("_Opacity", _opacity);

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
