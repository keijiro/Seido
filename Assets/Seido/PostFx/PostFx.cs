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
        [SerializeField] float _gradientFrequency = 1;
        [SerializeField] float _gradientSpeed = 1;

        #endregion

        #region Private variables

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;
        float _time;

        #endregion

        #region MonoBehaviour methods

        void OnDestroy()
        {
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
        }

        void Update()
        {
            if (Application.isPlaying)
                _time += Time.deltaTime * _gradientSpeed;
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            if (_gradient != null)
            {
                _material.SetVector("_GradientA", _gradient.coeffsA);
                _material.SetVector("_GradientB", _gradient.coeffsB);
                _material.SetVector("_GradientC", _gradient.coeffsC2);
                _material.SetVector("_GradientD", _gradient.coeffsD2);
            }

            _material.SetFloat("_Frequency", _gradientFrequency);
            _material.SetFloat("_LocalTime", _time);

            Graphics.Blit(source, destination, _material, 0);
        }

        #endregion
    }
}
