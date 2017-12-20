using UnityEngine;

namespace Seido
{
    [ExecuteInEditMode]
    public class WallFx : MonoBehaviour
    {
        public float amplitude { get; set; }
        public int effectType { get; set; }

        [SerializeField] Texture2D _flyerTexture;

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;

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
            if (Application.isPlaying) _time += Time.deltaTime;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            _material.SetFloat("_Amplitude", amplitude);
            _material.SetFloat("_LocalTime", _time);
            _material.SetTexture("_FlyerTex", _flyerTexture);

            var pass = Mathf.Clamp(effectType, 0, _material.passCount - 1);
            Graphics.Blit(source, destination, _material, pass);
        }
    }
}
