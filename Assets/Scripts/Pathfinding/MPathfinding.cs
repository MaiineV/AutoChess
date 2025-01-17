using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MPathfinding : MonoBehaviour
{
    public static MPathfinding instance;
    private Path _actualPath;
    private MNode _origenNode;
    private MNode _targetNode;
    private MNode _actualNode;
    public float searchingRange;
    
    private HashSet<MNode> _closeNodes = new HashSet<MNode>();
    private PriorityQueue<MNode> _openNodes = new PriorityQueue<MNode>();

    private void Awake()
    {
        instance = this;
    }

    public Path GetPath(Vector3 origen, Vector3 target)
    {
        _actualPath = new Path();
        _closeNodes = new HashSet<MNode>();
        _openNodes = new PriorityQueue<MNode>();

        _origenNode = GetClosestNode(origen);
        _targetNode = GetClosestNode(target);

        _actualNode = _origenNode;

        AStar();

        return _actualPath;
    }

    private void AStar()
    {
        if (_actualNode == null)
        {
            Debug.Log("ACA DEBERIA CRASHEAR!");
            return;
        }

        _closeNodes.Add(_actualNode);
        _actualNode.nodeColor = Color.green;

        var watchdog = 10000;
        var checkingNodes = new Queue<MNode>();

        while (_actualNode != _targetNode && watchdog > 0)
        {
            watchdog--;

            for (var i = 0; i < _actualNode.NeighboursCount(); i++)
            {
                var node = _actualNode.GetNeighbor(i);
                node.nodeColor = Color.magenta;
                if (_closeNodes.Contains(node) || !OnSight(_actualNode.transform.position, node.transform.position)) continue;

                node.previousNode = _actualNode;
                node.SetWeight(_actualNode.GetWeight() + 1 +
                               Vector3.Distance(node.transform.position, _targetNode.transform.position));

                _openNodes.Enqueue(node);
                checkingNodes.Enqueue(node);
            }

            if (checkingNodes.Count > 0)
            {
                var cheaperNode = checkingNodes.Dequeue();
                while (checkingNodes.Count > 0)
                {
                    if (cheaperNode == _targetNode) break;

                    var actualNode = checkingNodes.Dequeue();

                    if (actualNode.GetWeight() < cheaperNode.GetWeight())
                        cheaperNode = actualNode;
                }

                _actualNode = cheaperNode;
            }
            else
            {
                _actualNode = _openNodes.Dequeue();
            }


            _closeNodes.Add(_actualNode);
        }

        ThetaStar();
    }

    private void ThetaStar()
    {
        var stack = new Stack();
        _actualNode = _targetNode;
        stack.Push(_actualNode);
        var previousNode = _actualNode.previousNode;

        if (previousNode == null) Debug.Log("no existe");
        var watchdog = 10000;
        while (_actualNode != _origenNode && watchdog > 0)
        {
            watchdog--;

            if (previousNode.previousNode && OnSight(_actualNode.transform.position,
                    previousNode.previousNode.transform.position))
            {
                previousNode = previousNode.previousNode;
            }
            else
            {
                _actualNode.previousNode = previousNode;
                _actualNode = previousNode;
                stack.Push(_actualNode);
            }
        }

        watchdog = 10000;
        while (stack.Count > 0 && watchdog > 0)
        {
            watchdog--;

            var nextNode = stack.Pop() as MNode;
            _actualPath.AddNode(nextNode);
        }
    }

    private MNode GetClosestNode(Vector3 t, bool isForAssistant = false)
    {
        var actualSearchingRange = searchingRange;
        var closestNodes = Physics.OverlapSphere(t, actualSearchingRange, LayerManager.LM_NODE);

        var watchdog = 10000;
        while (closestNodes.Length <= 0)
        {
            watchdog--;
            if (watchdog <= 0)
            {
                return null;
            }

            actualSearchingRange += searchingRange;
            closestNodes = Physics.OverlapSphere(t, actualSearchingRange, LayerManager.LM_NODE);
        }

        MNode mNode = null;

        var minDistance = Mathf.Infinity;
        foreach (var node in closestNodes)
        {
            var distance = Vector3.Distance(t, node.transform.position);
            if (distance > minDistance) continue;

            var tempNode = node.gameObject.GetComponent<MNode>();

            if (tempNode == null) continue;

            mNode = tempNode;
            minDistance = distance;
        }

        return mNode;
    }

    public static bool OnSight(Vector3 from, Vector3 to)
    {
        var dir = to - from;
        var ray = new Ray(from, dir);

        return !Physics.Raycast(ray, dir.magnitude, LayerManager.LM_WALL);
    }
}