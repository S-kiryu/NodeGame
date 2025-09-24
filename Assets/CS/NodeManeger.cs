using UnityEngine;
using System.Collections.Generic;

// �m�[�h�N���X
public class NodeFixed : MonoBehaviour
{
    [Header("Node Settings")]
    public Color nodeColor = Color.white;
    public float nodeRadius = 0.5f;

    // �ڑ�����Ă���m�[�h�̃��X�g
    private List<NodeFixed> connectedNodes = new List<NodeFixed>();
    // ���̃m�[�h����o�Ă��郉�C�������_���[�̃��X�g
    private List<LineRenderer> outgoingLines = new List<LineRenderer>();

    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();

        // �m�[�h�̌����ڂ�ݒ�
        SetupNodeVisual();

        Debug.Log("Node created: " + name);
    }

    void SetupNodeVisual()
    {
        // �����̃R���|�[�l���g���`�F�b�N
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MeshFilter filter = GetComponent<MeshFilter>();
        SphereCollider collider = GetComponent<SphereCollider>();

        // �R���|�[�l���g�������ꍇ�͒ǉ�
        if (renderer == null) renderer = gameObject.AddComponent<MeshRenderer>();
        if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
        if (collider == null) collider = gameObject.AddComponent<SphereCollider>();

        // �v���~�e�B�u�̋��̃��b�V�����擾
        GameObject tempSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        filter.mesh = tempSphere.GetComponent<MeshFilter>().sharedMesh;

        // �ꎞ�I�ȋ��̂��폜
        DestroyImmediate(tempSphere);

        // �}�e���A�����쐬
        Material mat = new Material(Shader.Find("Standard"));
        if (mat.shader == null)
        {
            mat = new Material(Shader.Find("Diffuse"));
        }
        mat.color = nodeColor;
        renderer.material = mat;

        // �X�P�[����ݒ�
        transform.localScale = Vector3.one * nodeRadius;
    }

    // ���̃m�[�h�Ɛڑ�
    public void ConnectTo(NodeFixed targetNode)
    {
        if (targetNode == null || targetNode == this || connectedNodes.Contains(targetNode))
            return;

        // �o�����̐ڑ����m��
        connectedNodes.Add(targetNode);
        targetNode.connectedNodes.Add(this);

        // ���C�������_���[���쐬
        CreateLine(targetNode);

        Debug.Log("Connected " + name + " to " + targetNode.name);
    }

    // �ڑ�������
    public void DisconnectFrom(NodeFixed targetNode)
    {
        if (targetNode == null || !connectedNodes.Contains(targetNode))
            return;

        connectedNodes.Remove(targetNode);
        targetNode.connectedNodes.Remove(this);

        // �Ή����郉�C�����폜
        RemoveLine(targetNode);
    }

    // ���C�������_���[���쐬
    void CreateLine(NodeFixed targetNode)
    {
        GameObject lineObject = new GameObject("Line_" + name + "_to_" + targetNode.name);
        lineObject.transform.SetParent(transform);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        // ���C�������_���[�̐ݒ�
        line.material = new Material(Shader.Find("Standard"));
        //line.color = Color.cyan;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 2;
        line.useWorldSpace = true;

        // �ʒu��ݒ�
        line.SetPosition(0, transform.position);
        line.SetPosition(1, targetNode.transform.position);

        outgoingLines.Add(line);
    }

    // ���C�����폜
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
        // ���C���̈ʒu���X�V
        UpdateLinePositions();
    }

    // ���ׂẴ��C���̈ʒu���X�V
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

    // �}�E�X����Ńm�[�h���ړ�
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

    // �ڑ�����Ă���m�[�h�̃��X�g���擾
    public List<NodeFixed> GetConnectedNodes()
    {
        return new List<NodeFixed>(connectedNodes);
    }
}

