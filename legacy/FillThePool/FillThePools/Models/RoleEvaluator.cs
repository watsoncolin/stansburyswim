using System.Web.Security;

namespace FillThePool.Web.Models
{
    public class RoleEvaluator
    {
        public bool CanEdit
        {
            get
            {
                return Roles.IsUserInRole("Admin");
            }
        }

        public bool CanDelete
        {
            get
            {
                return Roles.IsUserInRole("Admin");
            }
        }
    }
}