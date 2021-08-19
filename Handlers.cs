using botspace;
using botspace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;


namespace Telegram.Bot.Echo
{
    public class Handlers
    {


    
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            ////Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            ////Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {                
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery),
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message),               
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
        {
            ////Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }
        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            var data = callbackQuery.Data;

            await botClient.SendTextMessageAsync(
               chatId: callbackQuery.Message.Chat.Id,
               text: $"Received {callbackQuery.Data}");
        }


        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {

            //I want to console write if I'm debugging! It would be better, if you had logged, not console dependency!
            //#if DEBUG
            //            Console.OutputEncoding = System.Text.Encoding.UTF8;
            //            Console.WriteLine($"Receive message type: {message.Type}");
            //            Console.WriteLine($"Receive message text: {message.Text}");
            //            Console.WriteLine($"Receive message text: {message.From.FirstName + "," + message.From.LastName + " : " + message.From.Id}");
            //#else
            //#endif

            if (message.Type == MessageType.Contact)
               await SendMessage(botClient, message, message.ReplyToMessage.Text);


            if (message.Type != MessageType.Text)
                return;
         

            var action = (message.Text.Split(' ').First()) switch
            {
               
                "/sendmessage" => GetFeedback(botClient, message),
                "/menu" => SendReplyKeyboard(botClient, message),
               
                "Рус" => SendMethodToSend(botClient, message, "Рус"),
                "Eng" => SendMethodToSend(botClient, message, "Eng"),
                "Uzb" => SendMethodToSend(botClient, message, "Uzb"),
                "Регистрация" => SendContactShare(botClient, message, "Рус"),
                "Contact" => SendContactShare(botClient, message, "Eng"),
                "Контакт" => SendContactShare(botClient, message, "Uzb"),
             
                "Анонимно" => SendMessage(botClient, message, "Рус"),
                "Anonymously" => SendMessage(botClient, message, "Eng"),
                "Аноним" => SendMessage(botClient, message, "Uzb"),
                
                _ => messageToSend(botClient, message)
            };
            var sentMessage = await action;
            ////Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

           static async Task<Message> messageToSend(ITelegramBotClient botClient, Message message)
            {
                string usage = "\n"; 
                //here I record in db!
                //I record only if the length of the message is longer than 50 chars!
               
                if (!string.IsNullOrEmpty(message.Text) && message.Text.Length > 50 && message.Text.Length < 2050)
                {
                    SaveMessageToDb(message);
                    usage = "\n" +
                    "------------------------------------------------\n" +
                    " Thank You, your message has been sent! \n" +
                    " Rahmat, sizning xabaringiz yuborildi!\n" +
                    " Спасибо! Ваше сообщение было отправлено!\n" +
                    "------------------------------------------------"
                    ;
                }
                else {

                 usage = "\n" +
                 "------------------------------------------------\n" +
                 " The message to be sent is too short. Please, add more details. \n" +
                 " Yuboriladigan xabar juda qisqa. Iltimos, qo'shimcha ma'lumotlarni qo'shib qo'ying.\n" +
                 " Отправляемое сообщение слишком короткое. Пожалуйста, добавьте подробности.\n" +
                 "------------------------------------------------"
                 ;

                }


                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: usage,
                                                            replyMarkup: new ReplyKeyboardRemove());
            }



            static async Task<Message> sendToRussianMenuOfMessageSendMethod(ITelegramBotClient botClient, Message message)
            {
                await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                // Simulate longer running task
                await Task.Delay(500);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Анонимно", "ruMethod_1"),
                        InlineKeyboardButton.WithCallbackData("Регистрация", "ruMethod_2"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Eng", "3")
                       // InlineKeyboardButton.WithCallbackData("4", "22"),
                    },
                });

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "Choose Language/Выберите Язык/Tilni tanlang",
                                                            replyMarkup: inlineKeyboard);
            }
            static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
            {

                //here I record in db!
                //I record only if the length of the message is longer than 50 chars!
                
                if (!string.IsNullOrEmpty(message.Text) && message.Text.Length > 50)
                {
                    SaveMessageToDb(message);
                }




                const string usage = "\n" +
                                     "------------------------------------------------\n" +
                                     "/menu   -  Комманды/Commands \n" +
                                     "------------------------------------------------"
                                     ;

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: usage,
                                                            replyMarkup: new ReplyKeyboardRemove());
            }


            static async Task<Message> SendContactShare(ITelegramBotClient botClient, Message message, string lang)
            {
                string textDisplay = "Номер телефона";
                 

                if (lang == "Eng")
                {
                    textDisplay = "Phone Number";
                    
                }
                if (lang == "Uzb")
                {

                   
                    textDisplay = "Телефон ракамингиз";

                }
                
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    KeyboardButton.WithRequestContact(textDisplay),
                });

                


                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                     text: lang,
                                                     replyMarkup: RequestReplyKeyboard);       

            }

            //this is recieved either after contact has been shared or anonymous!
            static async Task<Message> SendMessage(ITelegramBotClient botClient, Message message, string lang)
            {

                if (message.Contact != null)
                {
                    SaveContactUserToDb(message); 
                }
                
                string textDisplay = "Спасибо,можете отправить текст обращения"; 

                if (lang == "Eng")
                {
                    textDisplay = "Thank You, you may send your message";
                }
                if (lang == "Uzb")
                {
                    textDisplay = "Рахмат, мурожжатингизни юборишингиз мумкин"; 

                }

               
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: textDisplay,
                                                            replyMarkup: new ReplyKeyboardRemove());
            }

            static async Task<Message> GetFeedback(ITelegramBotClient botClient, Message message)
            {
                //  await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);


                return await botClient.SendTextMessageAsync(
                                      chatId: message.Chat,
                                      text: "\nАгар мурожаатингизга жавоб олишни истасангиз, алоқа маълумотларингизни қолдиринг/ " +
                                      "\nОставьте свои контактные данные, если хотите получить ответ на Ваше обращение/ " +
                                      "Please, leave your contact details, if you would like to receive a response to your message ",
                                      parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
                                  );


            }

            //this method is called when /menu
            static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message)
            {
                var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Рус", "Uzb", "Eng" }
                    })
                {
                    ResizeKeyboard = true
                };

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "Choose",
                                                            replyMarkup: replyKeyboardMarkup);
            }

            //this method is called when /Рус Uzb
            static async Task<Message> SendMethodToSend(ITelegramBotClient botClient, Message message, string lang)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup; 
                
                   //default is russian.  
                    replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Анонимно", "Регистрация"}
                    })
                    {
                        ResizeKeyboard = true
                    };
               
                if (lang == "Uzb")
                {
                    replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Аноним", "Контакт" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                }
                if (lang == "Eng")
                {
                    replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "Anonymously", "Contact" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                }

                

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "...",
                                                            replyMarkup: replyKeyboardMarkup);
            }

             static void SaveMessageToDb(Message message)
            {
                
               var br = new BotRepository();

                //if he shared his contact number, we get it to attach to message
                string contactNumber = "-------";
                string firstName="";
                string lastName="";
                try
                {
                    var col = br.GetLiteQuery().Where(x => x.user.telegramID == message.From.Id && x.ContactNumber != null).FirstOrDefault();
                    if (col != null)
                    {
                        contactNumber = col.ContactNumber;
                        if (col.user != null)
                        {
                            firstName = col.user.FirstName;
                            lastName = col.user.LastName;
                        }
                    }
                }
                catch (Exception ex)
                {
                    EmailHelper.WriteErrorLog(ex);
                }

                var botUser = new botspace.User
                {
                    telegramID = message.From.Id,
                    PhoneNumber=contactNumber,
                    FirstName=firstName,
                    LastName=lastName
                };


                var botmessage = new BotMessage
                {
                    ContactNumber = contactNumber,
                    Message = message.Text,
                    MessageType = "text",
                    CreatedDate = DateTime.Now,
                    user = botUser,
                    IsActive = true
                };
                try
                {
                    br.Insert(botmessage);
                    br.Dispose();

                }
                catch (Exception ex)
                {
                    EmailHelper.WriteErrorLog(ex); 
                
                }
            }

            static void SaveContactUserToDb(Message message)
            {
                var ur = new UserRepository();
                var col = ur.GetAll();

                var botUser = new botspace.User
                {
                    FirstName = message.Contact.FirstName,
                    LastName = message.Contact.LastName,
                    PhoneNumber = message.Contact.PhoneNumber,
                    telegramID = message.From.Id

                };
                try
                {

                    ur.Insert(botUser);
                    ur.Dispose();
                }
                catch (Exception ex)
                {
                    EmailHelper.WriteErrorLog(ex);

                }
            }
        }

       

    

    }
    }
