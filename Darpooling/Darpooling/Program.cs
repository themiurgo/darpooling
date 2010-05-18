using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Darpooling
{
    public class User
    {
        enum Sex {f, m};
        int userId;
        String username;
        String name;
        Sex sex;
        DateTime birthDate;
        String email;
        Boolean smoker;
        DateTime signupDate;
        String whereabouts;

        public User(String name) 
        {
            this.name = name;
        }

        public String Name 
        {
            get 
            {
                return name;
            }
         }
    }

    public class Trip
    {
        Int32 id;
        DateTime departureDateTime;
        DateTime arrivalDateTime;
        String departureName;
        String arrivalName;   
        Boolean smoke;
        Boolean music;
        Double cost;
        User owner;
        int freeSits;
        String notes;
        Boolean modifiable;
    }


    class Program
    {
        static void Main(string[] args)
        {
            User antonio = new User("Antonio");
            User daniele = new User("Daniele");
            Console.WriteLine("Nome utente : {0}", antonio.Name);
            Console.WriteLine("Nome utente : {0}", daniele.Name);
            Console.ReadLine();
        }
    }
};