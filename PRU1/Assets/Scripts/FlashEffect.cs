using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
public class FlashEffect : MonoBehaviour
{
    private float _duration = 0.2f;
    private int _flashEffectAmount = Shader.PropertyToID("_FlashAmount");
    private int _color = Shader.PropertyToID("_FlashColor");
    [SerializeField] private RendererType rendererType = RendererType.Sprite;
    private Material _material;
    private float _lerpAmount;
    [ColorUsage(true, true)]
    private Color myColor;
    public enum RendererType {
        Sprite = 0,
        Tilemap = 1
    }
    private void Awake()
    {
        if (rendererType == RendererType.Sprite) 
        _material = GetComponent<SpriteRenderer>().material;
        else 
        _material = GetComponent<TilemapRenderer>().material;
    }
    public void CallFlash(float intensity, float duration, Color color)
    {
        _duration = duration / 2;
        _lerpAmount = 0f;
        _material.SetColor(_color, color);
        DOTween.To(GetLerpValue, SetLerpValue, 1f * intensity, _duration).SetEase(Ease.OutExpo).OnUpdate(OnLerpUpdate).OnComplete(OnLerpComplete);
    }
    private void OnLerpUpdate()
    {
        _material.SetFloat(_flashEffectAmount, GetLerpValue());
    }
    private void OnLerpComplete()
    {
        DOTween.To(GetLerpValue, SetLerpValue, 0f, _duration).OnUpdate(OnLerpUpdate);
    }

    private float GetLerpValue()
    {
        return _lerpAmount;
    }
    private void SetLerpValue(float newValue)
    {
        _lerpAmount = newValue;
    }
}
