using UnityEngine;

public class SimpleReferenceGridScript : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float gridSize;

    private Vector3 startingPosition;

    private void Start()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        SnapGridToCamera();
    }

    private void SnapGridToCamera()
    {
        Vector3 offset = cameraTransform.position - startingPosition;

        float offsetX = offset.x % gridSize;
        float offsetY = offset.y % gridSize;

        transform.position = new Vector3(
            cameraTransform.position.x - offsetX,
            cameraTransform.position.y - offsetY,
            startingPosition.z
        );
    }
}
