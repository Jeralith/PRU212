using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public List<GameObject> screens;
    private int _previousScreenIndex;
    public int _currentScreenIndex = -1;
    public void UpdateScreenIndex(int newScreenIndex)
    {
        _currentScreenIndex = newScreenIndex;
        UpdateScreenActivation();
        if (_previousScreenIndex != _currentScreenIndex)
        StartCoroutine(ExecuteFreeze(0f, 0.5f));
        _previousScreenIndex = _currentScreenIndex;
    }
    public void UpdateScreenActivation()
    {
        for (int i = 0; i < screens.Count; i++)
        {
            if (i >= _currentScreenIndex - 1 && i <= _currentScreenIndex + 1)
            {
                screens[i].SetActive(true);
                
            }
            else 
            {
                screens[i].SetActive(false);
            }
        }
    }
    private IEnumerator ExecuteFreeze(float timeScale, float duration)
    {
        var original = 0.9f;
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = original;
    }
}
