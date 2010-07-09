using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Communication
{
    /// <summary>
    /// Abstract class representing a generic node in the network. A node is
    /// characterized by a location and a username.
    /// </summary>
    public abstract class Node
    {
        private Location location;
        private string username;

        public Node(Location nodeLocation, string username)
        {
            this.location = nodeLocation;
            this.username = username;
        }

        public String Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
            }
        }

        public Location Location
        {
            get { return location; }
            set { location = value; }
        }
    }

    public class UserNode : Node
    {
        public UserNode(Location nodeLocation, string username) :
            base(nodeLocation, username)
        {
        }

    }

    public class ServiceNode : Node
    {
        private List<ServiceNode> neighbours;
        private List<UserNode> localUsers;

        public ServiceNode(Location nodeLocation, string username) :
            base(nodeLocation, username)
        {
        }

        public bool hasNeighbour(ServiceNode node)
        {
            return neighbours.Contains(node);
        }

        public void addNeighbour(ServiceNode neighbourNode)
        {
            neighbours.Add(neighbourNode);
            if (neighbours.Contains(neighbourNode))
            {
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