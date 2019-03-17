using System.Collections.Generic;
using FillThePool.Models;

namespace FillThePool.Web.ViewModels
{
    public class UserRegistrationViewModel : ViewModelBase
    {
        public IEnumerable<Schedule> Schedules { get; set; }
        public IEnumerable<Student> Students { get; set; }
    }
}