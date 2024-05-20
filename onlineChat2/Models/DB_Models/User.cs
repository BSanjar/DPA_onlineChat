using System;
using System.Collections.Generic;

namespace onlineChat2.Models.DB_Models;

public partial class User
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? LastIp { get; set; }

    public string? Password { get; set; }

    /// <summary>
    /// admin, jur, tech, user
    /// </summary>
    public string? Role { get; set; }

    public string? PhoneNumber { get; set; }

    public virtual ICollection<Chat> ChatAdminNavigations { get; set; } = new List<Chat>();

    public virtual ICollection<Chat> ChatUserNavigations { get; set; } = new List<Chat>();
}
