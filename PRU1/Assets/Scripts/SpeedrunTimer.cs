using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedrunTimer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Stopwatch stopwatch = new Stopwatch();
    private TMP_Text _text;
    //public bool isEnabled = true;
    void Start()
    {
        stopwatch.Start();
        _text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //_text.enabled = isEnabled;
        _text.text = stopwatch.Elapsed.ToString(@"mm\:ss\.fff");
        
    }
    public void SetEnableTimer(bool value)
    {
        _text.enabled = value;
    }
    public void StopTimer()
    {
        stopwatch.Stop();
    }
    public void ResumeTimer()
    {
        stopwatch.Start();
    }
    public double GetElapsedSeconds()
    {
        return stopwatch.Elapsed.TotalSeconds;
    } 


}
