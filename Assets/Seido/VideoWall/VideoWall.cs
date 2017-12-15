using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class VideoWall : MonoBehaviour
{
    VideoPlayer _player;

    void OnEnable()
    {
        _player = GetComponent<VideoPlayer>();
        _player.Prepare();
    }

    void OnDisable()
    {
        _player.Stop();
        _player = null;
    }

    void Update()
    {
        const float kFPS = 29.97f;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _player.time = 600 / kFPS;
            _player.Play();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            _player.time = 2447 / kFPS;
            _player.Play();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            _player.time = 4845 / kFPS;
            _player.Play();
        }

        if (Input.GetKey(KeyCode.Z))
            _player.playbackSpeed = 0.75f;
        else if (Input.GetKey(KeyCode.X))
            _player.playbackSpeed = 1.5f;
        else
            _player.playbackSpeed = 1;
    }

    float FrameToTime(int frame)
    {
        return frame / 29.97f;
    }
}
