using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Darpooling
{
    class User
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
    }

    // 
    // Odio VS!

    class Path
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
        }
    }
}