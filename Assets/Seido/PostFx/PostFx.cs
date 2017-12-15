using UnityEngine;
using Klak.Chromatics;

namespace Seido
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class PostFx : MonoBehaviour
    {
        #region Exposed attributes and public methods

        [SerializeField] CosineGradient _gradient;
        [SerializeField] Color _lineColor = Color.black;
        [SerializeField, Range(0, 0.2f)] float _colorThreshold = 0.1f;
        [SerializeField, Range(0, 0.2f)] float _depthThreshold = 0.1f;

        #endregion

        #region Private variables

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;

        #endregion

        #region MonoBehaviour methods

        void OnDestroy()
        {
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            var time = Application.isPlaying ? Time.time : 10.1f;
            _material.SetFloat("_LocalTime", time);

            if (_gradient != null)
            {
                _material.SetVector("_GradientA", _gradient.coeffsA);
                _material.SetVector("_GradientB", _gradient.coeffsB);
                _material.SetVector("_GradientC", _gradient.coeffsC2);
                _material.SetVector("_GradientD", _gradient.coeffsD2);
            }

            _material.SetColor("_LineColor", _lineColor);
            _material.SetFloat("_ColorThreshold", _colorThreshold);
            _material.SetFloat("_DepthThreshold", _depthThreshold);

            Graphics.Blit(source, destination, _material, 0);
        }

        #endregion
    }
}
