using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class ShockwaveManager : MonoBehaviour
{
    [SerializeField] private float _shockwaveTime = 0.75f;
    private static int _shockwaveStrength = Shader.PropertyToID("_ShockwaveStrength");
    [SerializeField] private float _strength = -0.05f;
    private Material _material;
    private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _multiplier;
    private static int _waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");
    [SerializeField] private GameObject _player;
    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }

    public void CallShockwave() 
    {
        transform.position = _player.transform.position;
        StartCoroutine(ShockwaveAction(-0.1f, 1f));   
    }
    private IEnumerator ShockwaveAction(float startPos, float endPos) 
    {
        _material.SetFloat(_waveDistanceFromCenter, startPos);
        _material.SetFloat(_shockwaveStrength, _strength);
        float lerpedAmount = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < _shockwaveTime / _multiplier)
        {
            elapsedTime += Time.deltaTime;
            lerpedAmount = Mathf.Lerp(startPos, endPos, elapsedTime / _shockwaveTime);
            _material.SetFloat(_waveDistanceFromCenter, lerpedAmount);
            lerpedAmount = Mathf.Lerp(_strength, 0f, (elapsedTime / _shockwaveTime) * _multiplier);
            _material.SetFloat(_shockwaveStrength, lerpedAmount);
            yield return null; 
        }
    }
    
}
