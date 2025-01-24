using DG.Tweening;
using UnityEngine;
public class FlashEffect : MonoBehaviour
{
    private float _duration = 0.2f;
    private int _flashEffectAmount = Shader.PropertyToID("_FlashAmount");
    private int _color = Shader.PropertyToID("_FlashColor");

    private Material _material;
    private float _lerpAmount;
    [ColorUsage(true, true)]
    public Color myColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

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
