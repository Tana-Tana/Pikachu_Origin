using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private const float CameraSize = 7.8f; // kich thuoc mac dinh cua camera
    
    public void FitCameraToBoard(Vector3 bottomLeft, Vector3 topRight, float paddingRatio = 1f)
    {
        Vector3 center = (bottomLeft + topRight) / 2; // lay vi tri trung tam cua board
        mainCamera.transform.position = new Vector3(center.x, center.y - 0.25f, mainCamera.transform.position.z);

        // size camera
        float width = Mathf.Abs(topRight.x - bottomLeft.x); // tinh khoang cach theo chieu ngang
        float height = Mathf.Abs(topRight.y - bottomLeft.y); // tinh khoang cach theo chieu doc
        float aspectRatio = mainCamera.aspect; // ti le khung hinh cua camera
        float size =
            Mathf.Max((width * paddingRatio) / aspectRatio / 2f,
                (height * paddingRatio) / 2f); // paddingRatio de them khoang trang quanh board

        mainCamera.orthographicSize = Mathf.Max(CameraSize, size);
    }

    public Camera MainCamera => mainCamera;
}