using UnityEngine;

namespace Seido
{
    [ExecuteInEditMode]
    public class WallFx : MonoBehaviour
    {
        [SerializeField] Color _color1 = Color.black;
        [SerializeField] Color _color2 = Color.white;
        [SerializeField] float _speed = 1;

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;

        public float amplitude { get; set; }

        static int _instanceCount;

        float _time;

        void OnDestroy()
        {
            if (_material != null)
            {
                if (Application.isPlaying)
                    Destroy(_material);
                else
                    DestroyImmediate(_material);
            }
        }

        void Start()
        {
            _time = _instanceCount++;
        }

        void Update()
        {
            if (Application.isPlaying) _time += Time.deltaTime * _speed;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            _material.SetColor("_Color1", _color1);
            _material.SetColor("_Color2", _color2);
            _material.SetFloat("_Amplitude", amplitude);
            _material.SetFloat("_LocalTime", _time + _instanceCount);

            Graphics.Blit(source, destination, _material, 0);
        }
    }
}
