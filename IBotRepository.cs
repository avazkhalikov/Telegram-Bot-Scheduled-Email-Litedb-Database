using System;
using System.Collections.Generic;
using System.Text;

namespace botspace
{
    public interface IBotRepository
    {
        IEnumerable<BotMessage> GetAll();
        BotMessage GetById(int BotID);
        void Insert(BotMessage BotMessage);
        void Update(BotMessage BotMessage);
        void Delete(int botID);
        void Save();
    }
}
