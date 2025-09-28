using SQLite4Unity3d;

public class Score
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Username { get; set; }  // links to User
    public string Language { get; set; }  // e.g., "Python", "Java"
    public string Level { get; set; }     // "Easy", "Medium", "Hard"
    public int Value { get; set; }        // score value
}
