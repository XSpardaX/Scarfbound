using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolGraphBuilder : MonoBehaviour
{
    public PatrolNode startNode;
    public bool bidirectionalEdges = true;
    public bool snapPositionsToNavMesh = true;
    public float navMeshSampleDistance = 2f;

    public PatrolGraph BuildGraph()
    {
        PatrolGraph graph = new PatrolGraph();

        Dictionary<PatrolNode, int> nodeIds = new Dictionary<PatrolNode, int>();
        PatrolNode[] sceneNodes = GetComponentsInChildren<PatrolNode>(includeInactive: true);

        for (int i = 0; i < sceneNodes.Length; i++)
        {
            nodeIds[sceneNodes[i]] = i;
        }

        foreach (PatrolNode sceneNode in sceneNodes)
        {
            Vector3 position = sceneNode.transform.position;
            if (snapPositionsToNavMesh && NavMesh.SamplePosition(position, out NavMeshHit hit, navMeshSampleDistance, NavMesh.AllAreas))
            {
                position = hit.position;
            }

            graph.AddNode(nodeIds[sceneNode], position, sceneNode.waitTime);
        }

        foreach (KeyValuePair<PatrolNode, int> entry in nodeIds)
        {
            PatrolNode sourceNode = entry.Key;
            int sourceId = entry.Value;

            if (sourceNode.neighbors == null) continue;

            foreach (PatrolNode neighbor in sourceNode.neighbors)
            {
                if (neighbor == null || !nodeIds.TryGetValue(neighbor, out int targetId)) 
                    continue;
                graph.AddEdge(sourceId, targetId, bidirectionalEdges);
            }
        }

        if (nodeIds.TryGetValue(startNode, out int startId))
        {
            graph.SetStartNode(startId);
        }

        return graph;
    }
}
