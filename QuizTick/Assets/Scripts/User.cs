using SQLite4Unity3d;

//user table in db
public class User
{
    [PrimaryKey, AutoIncrement]  //autoincrement = dont manually set in the db
    public int Id { get; set; }

    public string FirstName { get; set; } //store fname

    public string LastName { get; set; } //store lname

    [Unique, MaxLength(100)]  //marked it as unique so two users cannot register with the same email.
    public string Email { get; set; }

    [MaxLength(50), Unique]  //unique as well, so no two users can pick the same username.
    public string Username { get; set; }

    public string Password { get; set; } //store a hashed password (using BCrypt) for security.
}
