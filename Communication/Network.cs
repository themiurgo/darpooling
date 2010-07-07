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

    public class UserNode : Node
    {

    }

    public class ServiceNode : Node
    {
        List<ServiceNode> neighbours;
        List<UserNode> localUsers;

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
        public InsertTripQueryMessage(UserNode issuer) :
            base(issuer)
        {
        }
    }
}