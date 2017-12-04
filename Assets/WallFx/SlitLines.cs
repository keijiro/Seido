using UnityEngine;

namespace Seido
{
    [ExecuteInEditMode]
    public class SlitLines : MonoBehaviour
    {
        [SerializeField] Color _color1 = Color.black;
        [SerializeField] Color _color2 = Color.white;
        [SerializeField] float _frequency = 10;
        [SerializeField] float _speed = 1;
        [SerializeField] float _width = 0.1f;

        public float width {
            get { return _width; }
            set { _width = value; }
        }

        [SerializeField, HideInInspector] Shader _shader;

        Material _material;
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
            _material.SetFloat("_LocalTime", _time);
            _material.SetFloat("_Frequency", _frequency);
            _material.SetFloat("_Width", _width);

            Graphics.Blit(source, destination, _material, 0);
        }
    }
}
