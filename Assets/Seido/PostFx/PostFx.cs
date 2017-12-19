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

        public CosineGradient gradient {
            get { return _gradient; }
            set { _gradient = value; }
        }

        #endregion

        #region Private variables

        [SerializeField, HideInInspector] Shader _shader;
        Material _material;

        Vector4 _gCoeffsA, _gCoeffsB, _gCoeffsC2, _gCoeffsD2;
        float _time;

        void InitializeGCoeffs()
        {
            if (_gradient != null)
            {
                _gCoeffsA = _gradient.coeffsA;
                _gCoeffsB = _gradient.coeffsB;
                _gCoeffsC2 = _gradient.coeffsC2;
                _gCoeffsD2 = _gradient.coeffsD2;
            }
        }

        #endregion

        #region MonoBehaviour methods

        void OnDestroy()
        {
            if (Application.isPlaying)
                Destroy(_material);
            else
                DestroyImmediate(_material);
        }

        void Start()
        {
            InitializeGCoeffs();
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                // Play mode: Interpolates the gradient coefficients.
                if (_gradient != null)
                {
                    var exp = Mathf.Exp(-1.5f * Time.deltaTime);
                    _gCoeffsA = Vector4.Lerp(_gradient.coeffsA, _gCoeffsA, exp);
                    _gCoeffsB = Vector4.Lerp(_gradient.coeffsB, _gCoeffsB, exp);
                    _gCoeffsC2 = Vector4.Lerp(_gradient.coeffsC2, _gCoeffsC2, exp);
                    _gCoeffsD2 = Vector4.Lerp(_gradient.coeffsD2, _gCoeffsD2, exp);
                }

                // Update the gradient time parameter.
                _time += _gradientSpeed * Time.deltaTime;
            }
            else
            {
                // Edit mode: Always shows the initial state.
                InitializeGCoeffs();
                _time = 0;
            }
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            // Lazy initialization of the material object.
            if (_material == null)
            {
                _material = new Material(_shader);
                _material.hideFlags = HideFlags.DontSave;
            }

            _material.SetVector("_GradientA", _gCoeffsA);
            _material.SetVector("_GradientB", _gCoeffsB);
            _material.SetVector("_GradientC", _gCoeffsC2);
            _material.SetVector("_GradientD", _gCoeffsD2);
            _material.SetFloat("_Frequency", _gradientFrequency);
            _material.SetFloat("_LocalTime", _time);

            Graphics.Blit(source, destination, _material, 0);
        }

        #endregion
    }
}
