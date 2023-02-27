namespace Domain;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
public class Participant
{
    public string _id { get; set; }
    public int Index { get; set; }
    public string? Guid { get; set; }
    public bool IsActive { get; set; }
    public string? Balance { get; set; }
    public string? Picture { get; set; }
    public int Age { get; set; }
    public string? EyeColor { get; set; }
    public Name? Name { get; set; }
    public string? Company { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? About { get; set; }
    public string? Registered { get; set; }
    public string? Latitude { get; set; }
    public string? Longitude { get; set; }
    public string[]? Tags { get; set; }
    public int[]? Range { get; set; }
    public Friend[]? Friends { get; set; }
    public string? Greeting { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.