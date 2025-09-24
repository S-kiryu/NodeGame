using UnityEngine;
using System.Collections.Generic;

// ノードクラス
public class NodeFixed : MonoBehaviour
{
    [Header("Node Settings")]
    public Color nodeColor = Color.white;
    public float nodeRadius = 0.5f;

    // 接続されているノードのリスト
    private List<NodeFixed> connectedNodes = new List<NodeFixed>();
    // このノードから出ているラインレンダラーのリスト
    private List<LineRenderer> outgoingLines = new List<LineRenderer>();

    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();

        // ノードの見た目を設定
        SetupNodeVisual();

        Debug.Log("Node created: " + name);
    }

    void SetupNodeVisual()
    {
        // 既存のコンポーネントをチェック
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MeshFilter filter = GetComponent<MeshFilter>();
        SphereCollider collider = GetComponent<SphereCollider>();

        // コンポーネントが無い場合は追加
        if (renderer == null) renderer = gameObject.AddComponent<MeshRenderer>();
        if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
        if (collider == null) collider = gameObject.AddComponent<SphereCollider>();

        // プリミティブの球体メッシュを取得
        GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        filter.mesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;

        // 一時的な球体を削除
        DestroyImmediate(tempSphere);

        // マテリアルを作成
        Material mat = new Material(Shader.Find("Standard"));
        if (mat.shader == null)
        {
            mat = new Material(Shader.Find("Diffuse"));
        }
        mat.color = nodeColor;
        renderer.material = mat;

        // スケールを設定
        transform.localScale = Vector3.one * nodeRadius;
    }

    // 他のノードと接続
    public void ConnectTo(NodeFixed targetNode)
    {
        if (targetNode == null || targetNode == this || connectedNodes.Contains(targetNode))
            return;

        // 双方向の接続を確立
        connectedNodes.Add(targetNode);
        targetNode.connectedNodes.Add(this);

        // ラインレンダラーを作成
        CreateLine(targetNode);

        Debug.Log("Connected " + name + " to " + targetNode.name);
    }

    // 接続を解除
    public void DisconnectFrom(NodeFixed targetNode)
    {
        if (targetNode == null || !connectedNodes.Contains(targetNode))
            return;

        connectedNodes.Remove(targetNode);
        targetNode.connectedNodes.Remove(this);

        // 対応するラインを削除
        RemoveLine(targetNode);
    }

    // ラインレンダラーを作成
    void CreateLine(NodeFixed targetNode)
    {
        GameObject lineObject = new GameObject("Line_" + name + "_to_" + targetNode.name);
        lineObject.transform.SetParent(transform);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        // ラインレンダラーの設定
        line.material = new Material(Shader.Find("Standard"));
        //line.color = Color.cyan;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 2;
        line.useWorldSpace = true;

        // 位置を設定
        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetNode.transform.position);

        outgoingLines.Add(line);
    }

    // ラインを削除
    void RemoveLine(NodeFixed targetNode)
    {
        for (int i = outgoingLines.Count - 1; i >= 0; i--)
        {
            LineRenderer line = outgoingLines[i];
            if (line != null && line.name.Contains(targetNode.name))
            {
                DestroyImmediate(line.gameObject);
                outgoingLines.RemoveAt(i);
                break;
            }
        }
    }

    void Update()
    {
        // ラインの位置を更新
        UpdateLinePositions();
    }

    // すべてのラインの位置を更新
    void UpdateLinePositions()
    {
        for (int i = 0; i < outgoingLines.Count && i < connectedNodes.Count; i++)
        {
            if (outgoingLines[i] != null && connectedNodes[i] != null)
            {
                outgoingLines[i].SetPosition(0, transform.position);
                outgoingLines[i].SetPosition(1, connectedNodes[i].transform.position);
            }
        }
    }

    // マウス操作でノードを移動
    void OnMouseDown()
    {
        isDragging = true;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        offset = transform.position - worldPos;

        Debug.Log("Started dragging: " + name);
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = mainCamera.WorldToScreenPoint(transform.position).z;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            transform.position = worldPos + offset;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
        Debug.Log("Stopped dragging: " + name);
    }

    // 接続されているノードのリストを取得
    public List<NodeFixed> GetConnectedNodes()
    {
        return new List<NodeFixed>(connectedNodes);
    }
}

