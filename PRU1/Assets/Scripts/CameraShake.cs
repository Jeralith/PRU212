using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
public class CameraShake : MonoBehaviour
{
    // public IEnumerator Shake(float duration, float strength) 
    // {
    //     Vector3 originalPos = transform.localPosition;
    //     float elapsedTime = 0f;
    //     while (elapsedTime < duration) 
    //     {
    //         float x = Random.Range(-1f, 1f) * strength;
    //         float y = Random.Range(-1f, 1f) * strength;
    //         transform.position = new Vector3(x, y, -1f);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;

    //     }
    //     transform.localPosition = originalPos;
    // }

    public static CameraShake instance;
    [SerializeField] private float _globalShakeSource = 1f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void Shake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(_globalShakeSource);
    }
}
