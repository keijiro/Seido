using UnityEngine;
using UnityEngine.Rendering;

namespace Seido
{
    class Tester : MonoBehaviour
    {
        [SerializeField] Texture[] _textureSet1;
        [SerializeField] Texture[] _textureSet2;

        CommandBuffer[] _commands = new CommandBuffer[7];
        Camera[] _cameras = new Camera[7];
        int _cycle;

        void OnDisable()
        {
            ReleaseCommandBuffers();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ReleaseCommandBuffers();

                _cycle = (_cycle + 1) % 3;

                if (_cycle > 0)
                {
                    var set = (_cycle == 1) ? _textureSet1 : _textureSet2;
                    for (var i = 0; i < 7; i++) AddBlitCommand(i, set[i]);
                }
            }
        }

        void ReleaseCommandBuffers()
        {
            for (var i = 0; i < 7; i++)
            {
                if (_commands[i] != null)
                {
                    if (_cameras[i] != null)
                    {
                        _cameras[i].RemoveCommandBuffer(CameraEvent.BeforeImageEffects, _commands[i]);
                        _cameras[i] = null;
                    }

                    _commands[i].Release();
                    _commands[i] = null;
                }
            }
        }

        void AddBlitCommand(int index, Texture texture)
        {
            var target = FindCamera(index);
            if (target == null) return;

            _commands[index] = CreateBlitCommand(texture);
            _cameras[index] = target;

            target.AddCommandBuffer(CameraEvent.BeforeImageEffects, _commands[index]);
        }

        Camera FindCamera(int index)
        {
            foreach (var c in FindObjectsOfType<Camera>())
                if (c.name == "Camera " + (index + 1)) return c;
            return null;
        }

        CommandBuffer CreateBlitCommand(Texture texture)
        {
            var cmd = new CommandBuffer();
            cmd.name = "Tester";
            cmd.Blit(texture, BuiltinRenderTextureType.CurrentActive);
            return cmd;
        }
    }
}
