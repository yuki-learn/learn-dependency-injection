using System;

namespace Ploeh.Samples.Commerce.Domain
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PermittedRoleAttribute : Attribute
    {
        public readonly Role Role;

        public PermittedRoleAttribute(Role role)
        {
            this.Role = role;
        }
    }
}