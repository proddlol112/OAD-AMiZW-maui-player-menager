using System.Text.Json;
using MauiApp3.Model;

namespace MauiApp3.Data;

public class PlayerRepository
{
    private readonly string _filePath;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public List<Player> Players { get; private set; }

    public PlayerRepository()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "players.json");
        Players = LoadPlayers();
    }

    public void Add(Player player)
    {
        Players.Add(player);
        SavePlayers();
    }

    private List<Player> LoadPlayers()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var savedPlayers = JsonSerializer.Deserialize<List<Player>>(File.ReadAllText(_filePath));
                if (savedPlayers is not null)
                    return savedPlayers;
            }
        }
        catch (JsonException)
        {
            // A damaged save file should not prevent the app from opening.
        }
        catch (IOException)
        {
            // Fall back to the starter entries if the save cannot be read.
        }

        var starterPlayers = new List<Player>
        {
            new() { Id = 1, Name = "test", Level = 1500 },
            new() { Id = 31, Name = "noname", Level = 4021 }
        };
        Players = starterPlayers;
        SavePlayers();
        return starterPlayers;
    }

    private void SavePlayers()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        var temporaryPath = _filePath + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(Players, JsonOptions));
        File.Move(temporaryPath, _filePath, overwrite: true);
    }
}
