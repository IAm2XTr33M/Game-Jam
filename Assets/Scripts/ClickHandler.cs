using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ClickableObject clickable = hit.collider.GetComponent<ClickableObject>();
                if (clickable != null)
                {
                    clickable.HandleClick();
                }
            }
        }
    }
}
