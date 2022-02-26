using UnityEngine;

public class Ringworld : MonoBehaviour
{
    public float spinningSpeed = 10;

    private void Update()
    {
        transform.Rotate(Vector3.right, spinningSpeed * Time.deltaTime);
    }
}
