using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Member
{
    public int MemberId { get; set; }

    public string Username { get; set; } = null!;

    public string Lozinka { get; set; } = null!;

    public DateTime JoinDate { get; set; }

    public int? RoleId { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public virtual Role? Role { get; set; }
}
