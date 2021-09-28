using hexapp_api_cs.Helpers.Authentication;
using System;
using System.Collections.Generic;

#nullable disable

namespace hexapp_api_cs.Models.Authentication
{
    public partial class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? ActiveFlag { get; set; }

        public User(int userId, string username, string hashedPassword, string firstName, string lastName, bool? activeFlag)
        {
            UserId = userId;
            Username = username;
            HashedPassword = hashedPassword;
            FirstName = firstName;
            LastName = lastName;
            ActiveFlag = activeFlag;
        }

        public static User FromCreate(UserCreate create)
        {
            return new User(
                0,
                create.Username,
                AuthenticationHelpers.EncrpytPassword(create.Password),
                create.FirstName,
                create.LastName,
                true
            );
        }

        public static User FromUpdate(User original, UserUpdate update)
        {
            original.HashedPassword = AuthenticationHelpers.EncrpytPassword(update.Password);
            original.FirstName = update.FirstName;
            original.LastName = update.LastName;
            original.ActiveFlag = update.ActiveFlag;

            return original;
        }
    }

    public class UserCreate
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class UserUpdate
    {
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? ActiveFlag { get; set; }
    }
}
