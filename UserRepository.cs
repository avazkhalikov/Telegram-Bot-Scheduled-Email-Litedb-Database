using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace botspace
{
    

    public class UserRepository//:IBotRepository
    {
        private readonly LiteDatabase _context;
        public UserRepository()
        {
            //get this from config.
            ConnectionString connectionString = new ConnectionString(@"Filename=C:\Temp\MyData2.db; Connection=shared; Password=123");
          //  connectionString.Password = "123";
           // connectionString.Filename = @"C:\Temp\MyData.db";

            _context = new LiteDatabase(connectionString);
        }

        public ILiteCollection<User> GetAll()
        {
            return _context.GetCollection<User>("botmessages");            
        }

        public User FindMessageByUser(int telegramID)
        {
            return GetAll()
                         //.Include(x => x.Id)
                         .FindById(telegramID);          
        }
        public ILiteQueryable<User> GetLiteQuery()
        {
            return GetAll().Query();
        }
        public User GetById(BsonValue id)
        {
            return GetAll().FindById(id);
        }
        public void Insert(User User)
        {
            GetAll().Insert(User);
            
        }
        public bool Update(User User)
        {
           return GetAll().Update(User);
        }
        public bool Delete(BsonValue id)
        {
            return GetAll().Delete(id);
        }
        public int DeleteAll()
        {
            return GetAll().DeleteAll();
        }
      
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                   // _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

      
    }
}
