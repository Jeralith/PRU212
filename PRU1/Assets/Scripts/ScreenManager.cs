using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public List<GameObject> screens;
    public int _currentScreenIndex = -1;
    public void UpdateScreenIndex(int newScreenIndex)
    {
        _currentScreenIndex = newScreenIndex;
        UpdateScreenActivation();
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
}
