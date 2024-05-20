using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using onlineChat2.Models;
using onlineChat2.Models.DB_Models;

namespace onlineChat2
{
	public class ChatHub : Hub
	{
		private readonly FeedbackContext _db;
		public ChatHub(FeedbackContext db)
		{
			_db = db;
		}



		//временное хранилище переписек
		private static Dictionary<string, List<ChatMsgModel>> _groupMessages = new Dictionary<string, List<ChatMsgModel>>();

		//временное хранилище участников групп
		private static Dictionary<string, List<string>> _groupMembers = new Dictionary<string, List<string>>();


		public async Task SendToUser(string userId, string message)
		{
			await Clients.User(userId).SendAsync("Receive", message);
		}
		public async Task SendMessage(string message)
		{
			await Clients.All.SendAsync("Receive", message);
		}

		public async Task SendToGroup(ChatMsgModel message)
		{

			try
			{
				await Clients.Group(message.GroupId).SendAsync("Receive", message);
				//если группа существует в хранилище, то добавляю сообщение в хранилище 
				if (_groupMessages.ContainsKey(message.GroupId))
				{
					_groupMessages[message.GroupId].Add(message);
				}
				else //иначе создаю группу в хрнилище и добавляю
				{
					_groupMessages.Add(message.GroupId, new List<ChatMsgModel>() { message });
				}
			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);
			}

		}

		public async Task AddToGroup(string groupName)
		{
			try
			{


				var userId = Context.User.FindFirst("id").Value;

				await Groups.AddToGroupAsync(Context.ConnectionId, groupName);



				if (_groupMembers.ContainsKey(groupName))
				{
					_groupMembers[groupName].Add(Context.ConnectionId);
				}
				else
				{
					_groupMembers.Add(groupName, new List<string> { Context.ConnectionId });
				}


				//если в временной хранилище есть уже история переписек, то беру оттуда
				if (_groupMessages.ContainsKey(groupName))
				{
					var messageHistory = _groupMessages[groupName];
					await Clients.Caller.SendAsync("LoadMessageHistory", messageHistory);
				}
				else // иначе загружаю все из БД в временную хранилищу, если там конечно были переписки ...
				{
					var messageHistoryFromDB = _db.Chats.FirstOrDefault(a => a.Id == groupName);
					if (messageHistoryFromDB != null)
					{
						if (messageHistoryFromDB.Chat1 != null && messageHistoryFromDB.Chat1 != "")
						{
							//десериализация
							List<ChatMsgModel> messages = JsonConvert.DeserializeObject<List<ChatMsgModel>>(messageHistoryFromDB.Chat1);

							//добавляю в временное хранилище
							_groupMessages.Add(groupName, messages);

							await Clients.Caller.SendAsync("LoadMessageHistory", messages);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		public async Task RemoveFromGroup(string groupName)
		{
			var userId = Context.User.FindFirst("id").Value;
			await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
		}


		public async Task leaveChat(string chatId)
		{
			try
			{
				var userId = Context.User.FindFirst("id").Value;
				var chat = _db.Chats.FirstOrDefault(a => a.Id == chatId);

				if (_groupMessages.ContainsKey(chatId))
				{
					//сериализую в json
					string chatsFromTemp = JsonConvert.SerializeObject(_groupMessages[chatId]);

					chat.Chat1 = chatsFromTemp;
					//сохраняю все изменения
					await _db.SaveChangesAsync();

					//выход из группы
					await Groups.RemoveFromGroupAsync(userId, chatId);



					_groupMembers[chatId].Remove(userId);

					if (_groupMembers[chatId].Count == 0)
					{
						//удаляю группу
						_groupMessages.Remove(chatId);
					}
				}
			}
			catch (Exception ex)
			{

			}
		}
	}
}
