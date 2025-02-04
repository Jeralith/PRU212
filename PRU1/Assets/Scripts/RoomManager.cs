using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject virtualCam;
    public ScreenManager screenManager;
    [SerializeField] private int screenIndex;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.SetActive(true);
            
        } 
        if (other.CompareTag("Player")) {
            screenManager.UpdateScreenIndex(screenIndex);
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            virtualCam.SetActive(false);
        }
    }
}
