using System;
using System.Collections.Generic;

namespace onlineChat2.Models.DB_Models;

public partial class Translation
{
    public string Id { get; set; } = null!;

    public string? Ru { get; set; }

    public string? Ky { get; set; }

    public string? En { get; set; }

    public string? Isdeleted { get; set; }
}
