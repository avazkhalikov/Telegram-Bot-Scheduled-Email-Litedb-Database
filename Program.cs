using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Echo;
using Topshelf;
using System.Timers;

namespace botspace
{
    
    public class TelegramBotStarter {

        readonly System.Timers.Timer _timer;

               

        public TelegramBotStarter()
        {           
        }         

        private static TelegramBotClient? Bot;
        CancellationTokenSource cts;
        public void TelegramBotStartNow()
        {           

           cts  = TelegramBotStart();

        }

        private CancellationTokenSource TelegramBotStart()
        {
            Bot = new TelegramBotClient("****your telegram token here********");
         
            var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            Bot.StartReceiving(new DefaultUpdateHandler(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync),
                               cts.Token);

           // //Console.WriteLine($"Start listening for @{me.Username}");
           // //Console.ReadLine();           
            return cts;
        }

        public void Start() { TelegramBotStartNow(); }
        public void Stop()
        {  // Send cancellation request to stop bot
            cts.Cancel();
        }

    }

    class Program
    {
        //ONE MORE IMPORTANT NOTE! SERVICE ON SERVER DOES NOT LIKE CONSOLE.WRITELINE!
        //USE IFDEBUG!

        //how does it work!
        //-> telegram bot is registered in telegram. token recieved.
        //-> telegram bot runs as a windows service. Handler.cs responsible for messaging.
        //-> all the messages are saved in database. LiteDB, one file based db.
        //-> topsheflconsole project runs every second and checks the schedule whentoemail() and gets from
        //-> database -5 days data and emails it to certain addresses.
        //
        //PUBLISH from VS:
        //notes, during the publish make sure that: Configuration=>Self Deployment to get all the dlls. and X64CPU!
        //
        //INSTALL:
        // to install go there and under //Console as Administrator, type "topshelfconsole.exe install"  under published folder!
        //used the following documentation to implement https://topshelf.readthedocs.io/en/latest/configuration/quickstart.html 

        //topshelf WORKED and it is the prefered method to use! Hangfire is good, but I left it.
        public static async Task Main()
        {
           
            var rc = HostFactory.Run(x =>                                   //1
            {
                x.Service<TelegramBotStarter> (s =>                                   //2
                {
                    s.ConstructUsing(name => new TelegramBotStarter());                //3
                    s.WhenStarted(tc => tc.Start());                         //4
                    s.WhenStopped(tc => tc.Stop());                          //5
                });
                x.RunAsLocalSystem();                                       //6

                x.SetDescription("Avaz Telegram Bost Host");                   //7
                x.SetDisplayName("Avaz Telegram Bot Service");                                  //8
                x.SetServiceName("avaztelegramserv");                                  //9
            });                                                             //10

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  //11
            Environment.ExitCode = exitCode;
           
            
        }    
               
    }
}
