using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Darpooling
{
    class User
    {
        int userId;

    }


    create table Utenti (
   ID          int primary key auto_increment,
   userName    varchar(20) unique,
   psw         varchar(32) not null,
   nome        varchar(20) not null,
   cognome     varchar(20) not null,
   sesso       enum('f','m') not null,
   dataNascita date not null,
   email       varchar(40) not null unique,
   dataPatente date not null,
   fumatore    boolean not null,
   dataIscriz  date not null,
   localita    varchar(20) not null,
);


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