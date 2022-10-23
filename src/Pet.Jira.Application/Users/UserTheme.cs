﻿using Pet.Jira.Domain.Models.Abstract;

namespace Pet.Jira.Application.Users
{
    public class UserTheme : IEntity<string>
    {
        public string Key { get; set; }
        public bool IsDarkMode { get; set; }

        public static UserTheme Create()
        {
            return new UserTheme();
        }
    }
}
