using System.Collections;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{   
    private bool _isPausing;
    public void Stop(float duration) 
    {
        if (_isPausing) return; 
        Time.timeScale = 0f;
        StartCoroutine(Wait(duration));
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator Wait(float duration)
    {
        _isPausing = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        _isPausing = false;
    }
}
