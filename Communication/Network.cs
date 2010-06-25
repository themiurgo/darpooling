using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication
{
    public class Node
    {
        Location location;
        string name;

        public String Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
    public class ServiceNode : Node
    {
        List<ServiceNode> neighbours;

        public bool hasNeighbour(ServiceNode node)
        {
            return neighbours.Contains(node);
        }

        public void addNeighbour(ServiceNode node)
        {
            neighbours.Add(node);
            if (neighbours.Contains(node))
            {
                node.addNeighbour(this);
            }
        }

        public void removeNeighbour(ServiceNode node)
        {
            neighbours.Remove(node);
        }
    }


    public class Network
    {
    }

}
