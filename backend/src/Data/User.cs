namespace Database.Data;

public sealed class User(
    string subject,
    string name
)
: Entity
{
    public string Subject { get; private set; } = subject;
    public string Name { get; private set; } = name;

    public void Update(
        string name
    )
    {
        Name = name;
    }
}