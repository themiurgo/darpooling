using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication
{

    /// <summary>
    /// Abstract class representing a generic node in the network. A node is
    /// characterized by a (custom) name, geographical name and location.
    /// </summary>
    public abstract class Node
    {
        private string nodeName;
        private string nodeGeoName;
        private Location location;

        public Node(string nodeName)
            : this(nodeName,null,null) {}

        public Node(string nodeName, string geoName)
            : this(nodeName,geoName,null) {}

        public Node(string nodeName, string geoName, Location nodeLocation)
        {
            this.nodeName = nodeName;
            this.nodeGeoName = geoName;
            this.location = nodeLocation;
        }

        public String NodeName
        {
            get { return nodeName; }
            set { nodeName = value;}
        }

        public String NodeGeoName
        {
            get { return nodeGeoName; }
            set { nodeGeoName = value; }
        }

        public Location Location
        {
            get { return location; }
            set { location = value; }
        }
    }



    public class UserNode : Node
    {
        private string userLocationName;
        private User user;

        public UserNode(string userName, Location nodeLocation) :
            base(userName, null, nodeLocation)
        {
        }

        public UserNode(string userName) :
            base(userName)
        {
        }

        public UserNode(User user, string userLocationName) : base(user.UserName)
        {
            this.user = user;
            this.userLocationName = userLocationName;
        }


        public UserNode(string userName, string userLocationName) :
            base(userName)
        {
            this.userLocationName = userLocationName;
        }

        public string UserLocationName
        {
            get { return userLocationName; }
        }

        public User User
        {
            get { return this.user; }
            set { this.user = value; }
        }

    }

    public class ServiceNode : Node
    {
        private List<ServiceNode> neighbours;
        private List<UserNode> localUsers;

        public ServiceNode(string nodeName, string geoName, Location nodeLocation) :
            base(nodeName, geoName, nodeLocation)
        {
            neighbours = new List<ServiceNode>();
            localUsers = new List<UserNode>();
        }

        public List<ServiceNode> Neighbours
        {
            get { return neighbours; }
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