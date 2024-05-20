using System;
using System.Collections.Generic;

namespace onlineChat2.Models.DB_Models;

public partial class Themcategorye
{
    public string Id { get; set; } = null!;

    public string? Category { get; set; }

    /// <summary>
    /// jur, tech
    /// </summary>
    public string? TypeTheme { get; set; }
}
