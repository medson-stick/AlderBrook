using UnityEngine;
using UnityEngine.InputSystem;

// Finds and interacts with nearby IInteractive objects.
public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactRange = 1.25f;
    [SerializeField] private LayerMask interactableLayer;

    // Starts an interaction when the Interact action is pressed.
    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;
        TryInteract();
    }

    // Finds the closest interactable and triggers it.
    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactableLayer);

        IInteractive closestInteractable = null;
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            IInteractive interactable = hit.GetComponent<IInteractive>();
            if (interactable == null) continue;

            float distance = Vector2.Distance(transform.position, hit.ClosestPoint(transform.position));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestInteractable = interactable;
            }
        }

        if (closestInteractable != null) closestInteractable.OnInteract();
    }
}
