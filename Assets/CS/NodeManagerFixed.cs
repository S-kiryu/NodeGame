// ノードマネージャークラス
using System.Collections.Generic;
using UnityEngine;

public class NodeManagerFixed : MonoBehaviour
{
    [Header("Node Creation")]
    public GameObject nodePrefab;
    public KeyCode createNodeKey = KeyCode.Space;
    public KeyCode connectModeKey = KeyCode.C;

    private bool connectMode = false;
    private NodeFixed selectedNode = null;
    private List<NodeFixed> allNodes = new List<NodeFixed>();

    void Start()
    {
        Debug.Log("NodeManagerFixed started!");
    }

    void Update()
    {
        // スペースキーでノードを作成
        if (Input.GetKeyDown(createNodeKey))
        {
            Debug.Log("Space key pressed! Creating node...");
            CreateNodeAtMousePosition();
        }

        // Cキーで接続モードの切り替え
        if (Input.GetKeyDown(connectModeKey))
        {
            connectMode = !connectMode;
            Debug.Log("Connect Mode: " + connectMode);
        }

        // 接続モードでの処理
        if (connectMode && Input.GetMouseButtonDown(0))
        {
            HandleNodeConnection();
        }
    }

    void CreateNodeAtMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // カメラからの距離

        Camera cam = Camera.main;
        if (cam == null)
            cam = FindFirstObjectByType<Camera>();

        if (cam == null)
        {
            Debug.LogError("No camera found!");
            return;
        }

        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        Debug.Log("Creating node at world position: " + worldPos);

        GameObject nodeObj = new GameObject("Node_" + allNodes.Count);
        nodeObj.transform.position = worldPos;
        nodeObj.AddComponent<NodeFixed>();

        NodeFixed node = nodeObj.GetComponent<NodeFixed>();
        if (node != null)
        {
            allNodes.Add(node);
            Debug.Log("Node created successfully. Total nodes: " + allNodes.Count);
        }
    }

    void HandleNodeConnection()
    {
        Camera cam = Camera.main;
        if (cam == null)
            cam = FindFirstObjectByType<Camera>();

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            NodeFixed clickedNode = hit.collider.GetComponent<NodeFixed>();

            if (clickedNode != null)
            {
                if (selectedNode == null)
                {
                    selectedNode = clickedNode;
                    Debug.Log("Selected node: " + selectedNode.name);
                }
                else if (selectedNode != clickedNode)
                {
                    selectedNode.ConnectTo(clickedNode);
                    selectedNode = null;
                }
                else
                {
                    selectedNode = null;
                    Debug.Log("Deselected node");
                }
            }
        }
    }

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 250, 100), "");
        GUI.Label(new Rect(15, 15, 240, 20), "Space: Create Node at Mouse");
        GUI.Label(new Rect(15, 35, 240, 20), "C: Toggle Connect Mode");
        GUI.Label(new Rect(15, 55, 240, 20), "Connect Mode: " + connectMode);
        GUI.Label(new Rect(15, 75, 240, 20), "Total Nodes: " + allNodes.Count);

        if (selectedNode != null)
        {
            GUI.Label(new Rect(15, 95, 240, 20), "Selected: " + selectedNode.name);
        }
    }
}