using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class CRTEffect : MonoBehaviour
{
    [Header("Monochrome")]
    [SerializeField, Range(0f, 1f)]   private float monochrome      = 0.8f;
    [SerializeField]                  private Color phosphorTint     = new Color(0.65f, 1f, 0.65f);

    [Header("Scanlines")]
    [SerializeField, Range(0f, 1f)]   private float scanlineIntensity = 0.25f;
    [SerializeField, Range(50f, 800f)] private float scanlineCount   = 300f;

    [Header("Screen Shape")]
    [SerializeField, Range(0f, 0.5f)] private float curvature        = 0.08f;
    [SerializeField, Range(0f, 1f)]   private float vignetteIntensity = 0.45f;

    [Header("Noise")]
    [SerializeField, Range(0f, 0.15f)] private float noiseAmount     = 0.04f;

    private Material _material;

    private static readonly int MonochromeID       = Shader.PropertyToID("_MonochromeStrength");
    private static readonly int TintID             = Shader.PropertyToID("_PhosphorTint");
    private static readonly int ScanlineIntensityID = Shader.PropertyToID("_ScanlineIntensity");
    private static readonly int ScanlineCountID    = Shader.PropertyToID("_ScanlineCount");
    private static readonly int CurvatureID        = Shader.PropertyToID("_CurvatureAmount");
    private static readonly int VignetteID         = Shader.PropertyToID("_VignetteIntensity");
    private static readonly int NoiseID            = Shader.PropertyToID("_NoiseAmount");

    private void OnEnable()
    {
        EnsureMaterial();
    }

    private void OnDisable()
    {
        DestroyImmediate(_material);
        _material = null;
    }

    private void EnsureMaterial()
    {
        if (_material != null)
            return;

        Shader shader = Shader.Find("Hidden/CRTEffect");
        if (shader == null)
        {
            Debug.LogWarning("[CRTEffect] Shader 'Hidden/CRTEffect' not found.");
            return;
        }

        _material = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        EnsureMaterial();

        if (_material == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        _material.SetFloat(MonochromeID,        monochrome);
        _material.SetColor(TintID,              phosphorTint);
        _material.SetFloat(ScanlineIntensityID, scanlineIntensity);
        _material.SetFloat(ScanlineCountID,     scanlineCount);
        _material.SetFloat(CurvatureID,         curvature);
        _material.SetFloat(VignetteID,          vignetteIntensity);
        _material.SetFloat(NoiseID,             noiseAmount);

        Graphics.Blit(src, dest, _material);
    }
}
