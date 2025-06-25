// File: src/Domain/Constants/Permissions.cs
using System.Collections.Generic;
using System.Reflection;

namespace Domain.Constants
{
    public static class Permissions
    {
        public static class Students
        {
            public const string View = "Permissions.Students.View";
            public const string Create = "Permissions.Students.Create";
            public const string Edit = "Permissions.Students.Edit";
            public const string Delete = "Permissions.Students.Delete";
        }

        public static class Teachers
        {
            public const string View = "Permissions.Teachers.View";
            public const string Create = "Permissions.Teachers.Create";
            public const string Edit = "Permissions.Teachers.Edit";
            public const string Delete = "Permissions.Teachers.Delete";
        }

        public static class Branches
        {
            public const string View = "Permissions.Branches.View";
            public const string Create = "Permissions.Branches.Create";
            public const string Edit = "Permissions.Branches.Edit";
            public const string Delete = "Permissions.Branches.Delete";
        }

        public static class Classes
        {
            public const string View = "Permissions.Classes.View";
            public const string Create = "Permissions.Classes.Create";
            public const string Edit = "Permissions.Classes.Edit";
            public const string Delete = "Permissions.Classes.Delete";
        }
        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Edit = "Permissions.Roles.Edit";
        }

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }

        /// <summary>
        /// A helper method to get all defined permissions.
        /// </summary>
        /// <returns>A list of all permission strings.</returns>
        public static List<string> GetAllPermissions()
        {
            var allPermissions = new List<string>();
            var nestedTypes = typeof(Permissions).GetNestedTypes();

            foreach (var type in nestedTypes)
            {
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                foreach (var field in fields)
                {
                    var propertyValue = field.GetValue(null);
                    if (propertyValue is not null)
                        allPermissions.Add(propertyValue.ToString());
                }
            }

            return allPermissions;
        }
    }
}