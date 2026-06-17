using System.Collections.Generic;
using UnityEngine;

// patrol map of nodes and links
public class PatrolGraph
{
    public class Node
    {
        public int Id;
        public Vector3 Position;
        public float WaitTime;
        public readonly List<int> NeighborIds = new List<int>();
    }

    private readonly Dictionary<int, Node> nodes = new Dictionary<int, Node>();

    public int StartNodeId { get; private set; }
    public int NodeCount => nodes.Count;

    // build nodes and edges
    public void Clear()
    {
        nodes.Clear();
        StartNodeId = -1;
    }

    public void AddNode(int id, Vector3 position, float waitTime = 0f)
    {
        nodes[id] = new Node
        {
            Id = id,
            Position = position,
            WaitTime = waitTime
        };
    }

    public void AddEdge(int fromId, int toId, bool bidirectional = true)
    {
        if (!nodes.ContainsKey(fromId) || !nodes.ContainsKey(toId)) return;

        AddDirectedEdge(fromId, toId);

        if (bidirectional)
        {
            AddDirectedEdge(toId, fromId);
        }
    }

    public void SetStartNode(int nodeId)
    {
        if (nodes.ContainsKey(nodeId))
        {
            StartNodeId = nodeId;
        }
    }

    // look up nodes
    public bool TryGetNode(int nodeId, out Node node)
    {
        return nodes.TryGetValue(nodeId, out node);
    }

    public Vector3 GetPosition(int nodeId)
    {
        return nodes.TryGetValue(nodeId, out Node node) ? node.Position : Vector3.zero;
    }

    public float GetWaitTime(int nodeId)
    {
        return nodes.TryGetValue(nodeId, out Node node) ? node.WaitTime : 0f;
    }

    public IReadOnlyList<int> GetNeighborIds(int nodeId)
    {
        if (nodes.TryGetValue(nodeId, out Node node))
        {
            return node.NeighborIds;
        }

        return System.Array.Empty<int>();
    }

    public int FindNearestNodeId(Vector3 worldPosition)
    {
        int nearestId = StartNodeId;
        float nearestDistance = float.MaxValue;

        foreach (Node node in nodes.Values)
        {
            float distance = Vector3.SqrMagnitude(node.Position - worldPosition);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestId = node.Id;
            }
        }

        return nearestId;
    }

    // pick next node at forks
    public int ChooseNextNode(int currentNodeId)
    {
        if (!nodes.TryGetValue(currentNodeId, out Node currentNode))
        {
            return StartNodeId;
        }

        IReadOnlyList<int> neighbors = currentNode.NeighborIds;
        if (neighbors.Count == 0)
        {
            return currentNodeId;
        }

        if (neighbors.Count == 1)
        {
            return neighbors[0];
        }

        return neighbors[Random.Range(0, neighbors.Count)];
    }

    private void AddDirectedEdge(int fromId, int toId)
    {
        List<int> neighbors = nodes[fromId].NeighborIds;
        if (!neighbors.Contains(toId))
        {
            neighbors.Add(toId);
        }
    }
}
