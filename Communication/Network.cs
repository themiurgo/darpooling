using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication
{
    /// <summary>
    /// Abstract class representing a generic node in the network. A node is
    /// characterized by a location and a name.
    /// </summary>
    /// 
    public abstract class Node
    {
        private Location location;
        private string nodeName;

        public Node(string nodeName, Location nodeLocation)
        {
            this.location = nodeLocation;
            this.nodeName = nodeName;
        }

        public String NodeName
        {
            get { return nodeName; }
            set { nodeName = value;}
        }

        public Location Location
        {
            get { return location; }
            set { location = value; }
        }
    }



    public class UserNode : Node
    {
        public UserNode(string nodeName, Location nodeLocation) :
            base(nodeName, nodeLocation)
        {
        }

    }

    public class ServiceNode : Node
    {
        private List<ServiceNode> neighbours;
        private List<UserNode> localUsers;

        public ServiceNode(string nodeName, Location nodeLocation) :
            base(nodeName, nodeLocation)
        {
            neighbours = new List<ServiceNode>();
            localUsers = new List<UserNode>();
        }

        
        public bool hasNeighbour(ServiceNode node)
        {
            return neighbours.Contains(node);
        }

        
        public void addNeighbour(ServiceNode neighbourNode)
        {
            if (!hasNeighbour(neighbourNode))
            {
                neighbours.Add(neighbourNode);
                neighbourNode.addNeighbour(this);
            }
        }

        
        public void removeNeighbour(ServiceNode neighbour)
        {
            neighbours.Remove(neighbour);
        }

        
        public bool hasUser(UserNode node)
        {
            return localUsers.Contains(node);
        }

        
        public void addUser(UserNode node)
        {
            localUsers.Add(node);
        }

        
        public void removeUser(UserNode node)
        {
            localUsers.Remove(node);
        }

        
        //Properties
        public int NumNeighbours
        {
            get { return neighbours.Count; }
        }

    }

    public class QueryMessage
    {
        private UserNode issuer;
        private ServiceNode answerTo;

        public QueryMessage(UserNode issuer)
        {
            this.issuer = issuer;
        }

    }

    public class SearchTripQueryMessage : QueryMessage
    {
        private Location tripSource, tripDestination;

        public SearchTripQueryMessage(UserNode issuer,
            Location source, Location destination) :
            base(issuer)
        {
            this.tripSource = source;
            this.tripDestination = destination;
        }
    }

    public class InsertTripQueryMessage : QueryMessage
    {
        public InsertTripQueryMessage(UserNode issuer, Trip trip) :
            base(issuer)
        {
        }
    }
}