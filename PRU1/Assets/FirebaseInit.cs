using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseInit : MonoBehaviour
{
    private DatabaseReference dbReference;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;

                FirebaseDatabase.GetInstance(app).SetPersistenceEnabled(false);
                dbReference = FirebaseDatabase.GetInstance(app, "https://pru212-935c1-default-rtdb.asia-southeast1.firebasedatabase.app/").RootReference;

                Debug.Log("Firebase is ready!");
            }
            else
            {
                Debug.LogError("Firebase setup failed.");
            }
        });
    }
}