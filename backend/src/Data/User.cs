namespace Database.Data;

public sealed class User(
    string subject,
    string name
)
: AuditableEntity
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