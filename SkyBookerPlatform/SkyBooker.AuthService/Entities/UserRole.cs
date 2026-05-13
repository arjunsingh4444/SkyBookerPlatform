namespace SkyBooker.AuthService.Entities;

// User roles in the system
public enum UserRole
{
    User = 0,    // Normal user who books flights
    Admin = 1    // Admin who can manage flights, seats, etc.
}
