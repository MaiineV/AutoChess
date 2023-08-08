using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path
{
    public readonly List<MNode> _pathList = new List<MNode>();

    public void AddNode(MNode node)
    {
        _pathList.Add(node);
    }

    public MNode GetNextNode()
    {
        if (!_pathList.Any()) return null;
        
        var nextNode = _pathList[0];
        _pathList.Remove(nextNode);
        return nextNode;

    }

    // public MNode CheckNextNode()
    // {
    //     return _pathQueue.Any() ?  _pathQueue.Peek() : null;
    // }

    public int PathCount()
    {
        return _pathList.Count;
    }
}