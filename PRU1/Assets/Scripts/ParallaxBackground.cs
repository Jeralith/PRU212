using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    
    public GameObject camera;
    [SerializeField] private float _lengthX, _lengthY, _startPosX, _startPosY, _parallaxEffectX, _parallaxEffectY;
    
    void Start()
    {
        _startPosX = transform.position.x;
        _lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
        _lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    void Update()
    {
        float tempX = camera.transform.position.x * (1 - _parallaxEffectX);
        float distanceX = camera.transform.position.x * _parallaxEffectX;
        float tempY = camera.transform.position.y * (1 - _parallaxEffectY);
        float distanceY = camera.transform.position.y * _parallaxEffectY;

        transform.position = new Vector3(_startPosX + distanceX, _startPosY + distanceY, transform.position.z);
        if (tempX > _startPosX + _lengthX) _startPosX += _lengthX;
        else if (tempX < _startPosX - _lengthX) _startPosX -= _lengthX;

        if (tempY > _startPosY + _lengthY) _startPosY += _lengthY;
        else if (tempY < _startPosY - _lengthY) _startPosY -= _lengthX;
    }
}
