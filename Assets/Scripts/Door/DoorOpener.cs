using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField]
    private Animator doorAnimator; // Reference to the door's Animator.
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doorAnimator = GetComponent<Animator>(); // Access the door's Animator.
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player has collided with the door.
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetTrigger("Door_Open");
            }
        }
    }
}
