using System;
using System.Collections.Generic;

namespace onlineChat2.Models.DB_Models;

public partial class Chat
{
    public string Id { get; set; } = null!;

    /// <summary>
    /// юридические(jur) или техничесские(tech) вопросы
    /// </summary>
    public string? TypeTheme { get; set; }

    /// <summary>
    /// последний сохраненный чат html
    /// </summary>
    public string? Chat1 { get; set; }

    /// <summary>
    /// гражданин кто обращался
    /// </summary>
    public string? User { get; set; }

    /// <summary>
    /// админ кто отвечал, в случае чата кто ответил последним
    /// </summary>
    public string? Admin { get; set; }

    /// <summary>
    /// new, waiting, closed
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// тема обращения если feedback
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// время обращения, если чат то время первого сообщения
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// последнее обновление
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// вопрос если feedback
    /// </summary>
    public string? Question { get; set; }

    /// <summary>
    /// ответ если feedback
    /// </summary>
    public string? Answer { get; set; }

    /// <summary>
    /// onlinechat, feedback, telegram
    /// </summary>
    public string? MsgSource { get; set; }

    /// <summary>
    /// язык обращения - ru, ky, en
    /// </summary>
    public string? LangMsg { get; set; }

    public virtual User? AdminNavigation { get; set; }

    public virtual User? UserNavigation { get; set; }
}
