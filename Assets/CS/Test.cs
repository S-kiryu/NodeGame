using UnityEngine;

public class SimpleTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("SimpleTest script started!");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key detected!");

            // 簡単なキューブを作成
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = new Vector3(0, 0, 0);
            cube.name = "TestCube";

            Debug.Log("Cube created at origin");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T key pressed - script is working!");
        }
    }
}