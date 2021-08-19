using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace botspace
{  

    public class BotRepository//:IBotRepository
    {
        private readonly LiteDatabase _context;
        public BotRepository()
        {
            //get this from config.
            ConnectionString connectionString = new ConnectionString(@"Filename=C:\Temp\MyData2.db; Connection=shared; Password=123");
           // connectionString.Password = "123";
          //  connectionString.Filename = @"C:\Temp\MyData.db; Connection=Shared";
            

            _context = new LiteDatabase(connectionString);
        }

        public ILiteCollection<BotMessage> GetAll()
        {
            return _context.GetCollection<BotMessage>("botmessages");            
        }

        public BotMessage  FindMessageByUser(int telegramID)
        {
            return GetAll()
                         .Include(x => x.user)
                         .FindById(telegramID);          
        }
        public ILiteQueryable<BotMessage> GetLiteQuery()
        {
            return GetAll().Query();
        }
        public BotMessage GetById(BsonValue id)
        {
            return GetAll().FindById(id);
        }
        public void Insert(BotMessage BotMessage)
        {
            GetAll().Insert(BotMessage);
            
        }
        public bool Update(BotMessage BotMessage)
        {
           return GetAll().Update(BotMessage);
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
