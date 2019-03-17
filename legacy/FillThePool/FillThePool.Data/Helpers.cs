using System;
using System.Linq;

namespace FillThePool.Data
{
    public static class Helpers
    {
        public static int CheckBalance(int idUser)
        {
	        var unit = new ApplicationUnit();
	        return unit.Transactions.GetAll().Any(x => x.User.UserId == idUser 
			&& x.CreatedOn > new DateTime(2016, 5, 1) && !x.Description.Contains("PENDING")) ?
				unit.Transactions.GetAll().Where(x => x.User.UserId == idUser && x.CreatedOn > new DateTime(2016, 5, 1) && !x.Description.Contains("PENDING")).Sum(x => x.LessonCredit) : 0;
        }

		public static bool HasPurchased(int idUser)
		{
			var unit = new ApplicationUnit();
			return unit.Transactions.GetAll().Any(x => x.User.UserId == idUser && !x.Description.Contains("PENDING"));
		}
	}
}
