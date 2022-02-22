using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int x;
    public int z;
    public GameObject vis;
    public int objId;

    public Node(int x=-1, int z=-1, GameObject vis=null)
    {
        x = -1;
        z = -1;
        vis = null;
        objId = 0;
    }

    public static Node nullNode = new Node();

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        if (obj.GetType() == this.GetType()) {
            Node other = (Node)obj;
            return (x == other.x && z == other.z);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ z.GetHashCode();
    }
}
