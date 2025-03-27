using Cainos.PixelArtPlatformer_VillageProps;
using UnityEngine;

public class ElevatorActivator : MonoBehaviour
{
    [SerializeField] private Elevator elevator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            elevator.ActivateElevator();
        }
    }
}
