using Telegram.Bot.Echo;
using botspace;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Echo;
using System.Threading;
using System.Threading.Tasks;



namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private static TelegramBotClient? Bot;
        CancellationTokenSource cts;

        #region Bot Tester
        [TestMethod()]
        public async Task messageToSendTest()
        {

            Bot = new TelegramBotClient("telegram token here ********");

            var cts = new CancellationTokenSource();

            //STEP 1:  check if the message is sent and saved in database!
            //it does save in db! but message is not saved! since Chat object is null!
            //NO must split into two! check if message sent with no error! otherwise, throuwing object reference not found for chat
            
            //assuming telegram bot is running!
            {
                Update update = new Update();                

                Telegram.Bot.Types.User u = new Telegram.Bot.Types.User();
                u.Id = 159153053;  //sample telegram id.
                u.Username = "test159153053"; 
                Message message = new Message();
                message.From = u;      


                message.Text = @"test messge to save in db! test messge to save in db! test messge to save in db! test messge to save in db! test messge to save in db!";
                update.Message = message;

                await Handlers.HandleUpdateAsync(Bot, update, cts.Token);                

            }

            //STEP 2:
            //now read the database and see if it recieved the message!
            //records in database must be increased by 1.
           



        }

        #endregion



        #region Database Insert random data, get back data and email tests.

        //UDEMY: so the tester's job sometimes seeding random data! that may require some coding too!!

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        //testing help class to send an email.
        [TestMethod]
        public void SendEmail()
        {
            //TODO: change here 
            TopshelfConsole.EmailHelper.SendEmail("rmasimov@gmail.uz", "akhalikov@gmail.uz", "mail.gmail.uz", 25, true,
                "rov", "sug", "goodJob", "this isthe body test");
        
        }

        public string RandomPhrase()
        {
            var sb = new StringBuilder();
            int n = new Random(10).Next(15, 55);
            for (int i = 0;  i < n; i++)
            {
                sb.Append(" "+ RandomString(n)); 
            }

            return sb.ToString();
        }

        [TestMethod]
        public void SeedTestData()
        {
            BotRepository br = new BotRepository();
            var col = br.GetAll();
            var oldCount = col.Count();


            int numberOfMessage = 25;
                      
            //this seeds different phrases for different dates!
            for (int i = 0; i < numberOfMessage; i++)
            {
                var botUser = new botspace.User
                {
                    telegramID = new Random().Next(15, 55)
                };

                int n = new Random().Next(-15, 3);
                var botmessage = new BotMessage
                {
                    ContactNumber = "11-22-33",
                    Message = RandomPhrase(),
                    MessageType = "text",
                    CreatedDate = DateTime.Now.AddDays(n),
                    user = botUser
                };
                br.Insert(botmessage);
            }

            
            var newCount = col.Count();
            Assert.AreEqual(oldCount + numberOfMessage, newCount); 

        }

            [TestMethod]
        public void TestGetMessageWithinDateRange()
        {
            BotRepository br = new BotRepository();
            DateTime today = DateTime.Now;
            var col = br.GetAll().Query().Where(a => a.CreatedDate < today && a.CreatedDate > today.AddDays(-6));
            var list = col.ToList();
           

            //another way, check the opposite if any came in the list.
            Assert.IsFalse(list.Any(x=>x.CreatedDate > today && x.CreatedDate < today.AddDays(-5))); 
        }




        [TestMethod]
        public void TestMethod1()
        {
            //https://github.com/mbdavid/LiteDB.Studio  studio to visually look at the data!
            //compiled/built: run from here C:\VS-Projects2021\LiteDB.Studio-master\LiteDB.Studio\bin\Debug  

            BotRepository br = new BotRepository();
            var col = br.GetAll();

            var botUser = new botspace.User
            {
                telegramID = 123
            };

            var botmessage = new BotMessage
            {
                ContactNumber = "11-22-33",
                Message = "test me 1",
                MessageType = "text",
                CreatedDate = DateTime.Now,
                user = botUser
            };


            br.Insert(botmessage);

            var message = br.FindMessageByUser(123);

            Assert.AreEqual("11-22-33", message.ContactNumber);

          
        }



        [TestMethod]
        public void TestMethodSample()
        {
            //none testable code, just to run and debug!

            litedbsample ls = new litedbsample();

            ls.rebuildDb("123", "123");
            ls.test("123");

        }
   }

    #endregion
}
