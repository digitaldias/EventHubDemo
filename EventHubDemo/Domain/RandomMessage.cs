using System.Text;
using System.Text.Json;

namespace Domain;

public class RandomMessage
{
    public Participant? Participant { get; set; }

    public byte[] AsByteArray()
    {
        var serialized = JsonSerializer.Serialize(this);
        return Encoding.UTF8.GetBytes(serialized);
    }
}
