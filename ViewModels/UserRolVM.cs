using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RestauranteApp.ViewModels;

public class UserRolVM
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? RolActual { get; set; }
    public bool LockoutEnabled { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public bool EstaSuspendido => LockoutEnabled && LockoutEnd.HasValue && LockoutEnd > DateTimeOffset.UtcNow;
}

public class UserListVM
{
    public List<UserRolVM> Usuarios { get; set; } = new List<UserRolVM>();
    public SelectList? Roles { get; set; }
}