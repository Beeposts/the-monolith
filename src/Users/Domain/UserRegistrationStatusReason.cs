namespace Users.Domain;

public enum UserRegistrationStatusReason
{
    Created = 10,
    Invited = 40,
    Confirmed = 41,
    Registered = 70,
    Rejected = 79
}