using FillThePool.Models;
using System;

namespace FillThePool.Data
{
    public class ApplicationUnit : IDisposable
    {
        private readonly DataContext _context = new DataContext();

        private IRepository<Student> _students;
        private IRepository<Schedule> _schedules;
		private IRepository<Transaction> _transactions;
		private IRepository<EmailTemplate> _emailTemplates;
		private IRepository<Registration> _registrations;
		private IRepository<User> _users;
	    private IRepository<Setting> _settings;
	    private IRepository<PromoCode> _promoCode;
	    private IRepository<Page> _pages;
	    private IRepository<WaitList> _waitList; 

        public IRepository<Student> Students
        {
            get { return _students ?? (_students = new GenericRepository<Student>(_context)); }
        }

        public IRepository<Schedule> Schedules
        {
            get { return _schedules ?? (_schedules = new GenericRepository<Schedule>(_context)); }
        }

		public IRepository<Transaction> Transactions
		{
			get { return _transactions ?? (_transactions = new GenericRepository<Transaction>(_context)); }
		}

		public IRepository<EmailTemplate> EmailTemplates
		{
			get { return _emailTemplates ?? (_emailTemplates = new GenericRepository<EmailTemplate>(_context)); }
		}

		public IRepository<Registration> Registrations
		{
			get { return _registrations ?? (_registrations = new GenericRepository<Registration>(_context)); }
		}

		public IRepository<User> Users
		{
			get { return _users ?? (_users = new GenericRepository<User>(_context)); }
		}

	    public IRepository<Setting> Settings
	    {
		    get { return _settings ?? (_settings = new GenericRepository<Setting>(_context)); }
	    }

		public IRepository<PromoCode> PromoCodes
		{
			get { return _promoCode ?? (_promoCode = new GenericRepository<PromoCode>(_context)); }
		}

		public IRepository<Page> Pages
		{
			get { return _pages ?? (_pages = new GenericRepository<Page>(_context)); }
		}
		public IRepository<WaitList> WaitList
		{
			get { return _waitList ?? (_waitList = new GenericRepository<WaitList>(_context)); }
		} 


	    public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
