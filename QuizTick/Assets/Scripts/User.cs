using SQLite4Unity3d;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    [MaxLength(50), Unique]
    public string Username { get; set; }

    public string Password { get; set; } // Hashed password
}
