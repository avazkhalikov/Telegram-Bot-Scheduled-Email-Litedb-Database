using botspace;
using System;
using System.Timers;
using Topshelf;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace TopshelfConsole
{
    //ONE MORE IMPORTANT NOTE! SERVICE ON SERVER DOES NOT LIKE //Console.WRITELINE! OR ANY //Console.
    //USE IFDEBUG!

    //*** how to see db data on the server? you can install studio or get a copy of the file and see it in local!

    //how does it work?
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

    public class TownCrier
    {
        readonly Timer _timer;
        public TownCrier()
        {
            //this circles around and runs every second!
            _timer = new Timer(1000) { AutoReset = true };
            _timer.Elapsed += (sender, eventArgs) => doAll();  //------------ actions start here .............                                   
        }
        public void Start() { _timer.Start(); }
        public void Stop() { _timer.Stop(); }

        #region
        public bool whentoEmail()
        {
           
            var now = DateTime.Now.DayOfWeek; 

            switch (now)
            {
                case DayOfWeek.Monday:
                    if (DateTime.Now.TimeOfDay.Hours == 20
                       && DateTime.Now.TimeOfDay.Minutes == 00
                       && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    return false; 

                case DayOfWeek.Tuesday:
                    if (DateTime.Now.TimeOfDay.Hours == 11
                      && DateTime.Now.TimeOfDay.Minutes == 40
                      && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    if (DateTime.Now.TimeOfDay.Hours ==11
                     && DateTime.Now.TimeOfDay.Minutes == 42
                     && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    if (DateTime.Now.TimeOfDay.Hours == 11
                     && DateTime.Now.TimeOfDay.Minutes == 44
                     && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    return false;
                case DayOfWeek.Wednesday:
                    if (DateTime.Now.TimeOfDay.Hours == 20
                      && DateTime.Now.TimeOfDay.Minutes == 00
                      && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    return false;
                case DayOfWeek.Thursday:
                    if (DateTime.Now.TimeOfDay.Hours == 14
                      && DateTime.Now.TimeOfDay.Minutes == 50
                      && DateTime.Now.TimeOfDay.Seconds == 00)
                    {

                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);

                        return true;
                    }
                    if (DateTime.Now.TimeOfDay.Hours == 14
                    && DateTime.Now.TimeOfDay.Minutes == 55
                    && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    if (DateTime.Now.TimeOfDay.Hours == 14
                    && DateTime.Now.TimeOfDay.Minutes == 58
                    && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    if (DateTime.Now.TimeOfDay.Hours == 18
                    && DateTime.Now.TimeOfDay.Minutes == 00
                    && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    return false;
                case DayOfWeek.Friday:
                    if (DateTime.Now.TimeOfDay.Hours == 20
                      && DateTime.Now.TimeOfDay.Minutes == 00
                      && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    return false;
                case DayOfWeek.Saturday:
                    if (DateTime.Now.TimeOfDay.Hours == 20
                      && DateTime.Now.TimeOfDay.Minutes == 00
                      && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    return false;
                case DayOfWeek.Sunday:
                    if (DateTime.Now.TimeOfDay.Hours == 20
                      && DateTime.Now.TimeOfDay.Minutes == 00
                      && DateTime.Now.TimeOfDay.Seconds == 00)
                    {
                        EmailHelper.WriteMessageLog("Emailing time: " + DateTime.Now);
                        return true;
                    }
                    return false;
                default: return false; 
            }

        }

        #endregion
        public void doAll()
        {
            if (whentoEmail())
            {               
                try
                {
                    //2. get Messages
                    BotRepository br = new BotRepository();
                    DateTime today = DateTime.Now;
                    //get active and 6 day range emails!
                    var col = br.GetAll().Query().Where(a => a.CreatedDate < today
                                                        && a.CreatedDate > today.AddDays(-6)
                                                        && a.IsActive);

                    List<BotMessage> list = col.ToList();
                  
                    if (list != null && list.Count > 0)
                    {
                        //update messages to be sent!
                        foreach (var c in list)
                        {
                            c.IsActive = false;
                            br.Update(c);
                        }
                    }

                    //3. create aggregated message.
                    var sb = new StringBuilder();
                    string contactNumber; 
                    if (list != null && list.Count > 0)
                    {
                        foreach (var message in list)
                        {
                            contactNumber = "Anonymous messenger ";
                            if (message.ContactNumber != null) {

                                contactNumber =" ContactNumber: " + message.ContactNumber; 
                            }
                            if (message.user != null)
                            { 
                               contactNumber=contactNumber+ 
                                                          " " + message.user.FirstName!=null ? message.user.FirstName + " " : ""
                                                          + message.user.LastName != null ? message.user.LastName + " " : ""
                                                          +  message.user.telegramID != null ? "telegram id:" + message.user.telegramID + " " : ""
                                                          +  message.user.PhoneNumber != null ? "phone: " + message.user.PhoneNumber + "\n " : "\n"; 
                            }

                            sb.Append(contactNumber
                                + " date sent: " + message.CreatedDate + "message: " + message.Message + "\n");
                        }

                    }
                    //3. email them.
                    if (sb != null && sb.Length>10) //at least 10 characters must exist in order to email!
                    {
                        EmailHelper.WriteMessageLog("Sending email now with message: " + DateTime.Now);
                        EmailHelper.SendEmail("rtbot@wiut.uz", "akhalikov@wiut.uz", "mail.wiut.uz", 25, true,
                       "rtbot", "", "Weekly Telegram Messages", sb.ToString());
                    }
                    else
                    {
                        EmailHelper.WriteMessageLog("Sending email now NO message: " + DateTime.Now);
                        EmailHelper.SendEmail("rtbot@wiut.uz", "akhalikov@wiut.uz", "mail.wiut.uz", 25, true,
                    "rtbot", "", "Weekly Telegram Messages", "No messages has been submitted this week");
                    }
                }
                catch (Exception ex)
                {
                    EmailHelper.WriteErrorLog(ex); 
                }
            }


        }


        
    }

  

    class Program
    {
        static void Main(string[] args)
        {
            var rc = HostFactory.Run(x =>                                   //1
            {
                x.Service<TownCrier>(s =>                                   //2
                {
                    s.ConstructUsing(name => new TownCrier());                //3
                    s.WhenStarted(tc => tc.Start());                         //4
                    s.WhenStopped(tc => tc.Stop());                          //5
                });
                x.RunAsLocalSystem();                                       //6

                x.SetDescription("Email Sender Service");                   //7
                x.SetDisplayName("avazemailser");                                  //8
                x.SetServiceName("avazemailser");                                  //9
            });                                                             //10

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  //11
            Environment.ExitCode = exitCode;
        }
    }
}
