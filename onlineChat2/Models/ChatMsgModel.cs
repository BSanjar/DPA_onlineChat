namespace onlineChat2.Models
{
	public class ChatMsgModel
	{
		public string GroupId { get; set; }
		public string Message { get; set; }
		public string UserSenderId { get; set; }
		public string UserSenderName { get; set; }
		public string SendTime { get; set; }
	}
}
