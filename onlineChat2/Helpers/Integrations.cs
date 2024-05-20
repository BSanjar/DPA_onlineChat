using Microsoft.Win32;
using Newtonsoft.Json;
using onlineChat2.Models;
using onlineChat2.Models.DB_Models;
using System.Text;

namespace onlineChat2.Helpers
{
    
    public class Integrations
    {
        private readonly appSettings _appSettings;
        public Integrations(appSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<bool> newOnlineChatMsg(tempChat chatInfo)
        {
            try
            {
                messageRMQ message = new messageRMQ();
                string processId = Guid.NewGuid().ToString();

                string typeTheme = "вопрос юридического характера";

                if(chatInfo.typeTheme !="jur") 
                {
                    typeTheme = "вопрос технического характера";
                }

                //добавляю в очередь оповещения
                message = new messageRMQ();
                message.id = Guid.NewGuid().ToString();
                message.msgSub = "Новое сообщение в онлайн чате";
                message.msg = "\U0001F4ACНовое сообщение в онлайн чате:\n\nПоступило сообщение от гражданина " + chatInfo.name+"\n"+ typeTheme+ "\nдля переписки с клиентом переходите по ссылке: http://192.168.88.211:3014/Admin/show/9071917c-5a4f-4d39-b2a8-bd6ff0215e28";
                message.network = "telegram";
                message.registry = "";
                message.created_at = DateTime.Now.ToString();
                message.msg_status = "1";
                message.processid = processId;
                message.receiver_acc = _appSettings.telegram_chanel_id;
                message.sended_at = "";
                message.sender_acc = "RegistryDPA_BOT";

                var resEmail = await sendToQuoue(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
                return false;
            }
        }


        public async Task<bool> sendToQuoue(messageRMQ message)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Сериализуем объект message в JSON
                    string json = JsonConvert.SerializeObject(message);

                    // Создаем HttpContent из JSON
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    // Отправляем POST запрос к API
                    HttpResponseMessage response = await client.PostAsync(_appSettings.urltoProducerRmQ, content);
                    //HttpResponseMessage response = new HttpResponseMessage();

                    // Проверяем статус код ответа
                    if (response.IsSuccessStatusCode)
                    {
                        // Ваш запрос успешно обработан
                        return true;
                    }
                    else
                    {
                        // Обработка ошибки, например, логирование
                        Console.WriteLine($"Ошибка при отправке сообщения. Код ответа: {response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Обработка других ошибок, например, логирование
                Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> sendToEmail(Chat chatInfo)
        {
            try
            {
                messageRMQ message = new messageRMQ();
                string processId = Guid.NewGuid().ToString();              

                //добавляю в очередь оповещения
                message = new messageRMQ();
                message.id = Guid.NewGuid().ToString();
                message.msgSub = "Официальный ответ от ГАЗПД на ваше письмо";
                message.msg = chatInfo.Answer;
                message.network = "email";
                message.registry = "";
                message.created_at = DateTime.Now.ToString();
                message.msg_status = "1";
                message.processid = processId;
                message.receiver_acc = chatInfo.UserNavigation.Email;
                message.sended_at = "";
                message.sender_acc = "reestrtest46@gmail.com";

                var resEmail = await sendToQuoue(message);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
                return false;
            }
        }
    }
}
