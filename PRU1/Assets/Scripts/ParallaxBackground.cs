using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    
    public GameObject camera;
    [SerializeField] private float _length, _startPos, _parallaxEffect;

    void Start()
    {
        _startPos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float temp = camera.transform.position.x * (1 - _parallaxEffect);
        float distance = camera.transform.position.x * _parallaxEffect;

        transform.position = new Vector3(_startPos + distance, transform.position.y, transform.position.z);
        if (temp > _startPos + _length) _startPos += _length;
        else if (temp < _startPos - _length) _startPos -= _length;
    }
}
