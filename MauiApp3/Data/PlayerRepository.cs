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

    /// <summary>Updates the name and level of an existing player.</summary>
    /// <param name="player">Player containing the ID and updated values.</param>
    /// <returns>True when the player exists and was saved; otherwise false.</returns>
    public bool Update(Player player)
    {
        var existing = Players.FirstOrDefault(item => item.Id == player.Id);
        if (existing is null) return false;
        existing.Name = player.Name;
        existing.Level = player.Level;
        SavePlayers();
        return true;
    }

    /// <summary>Removes a player from the saved list.</summary>
    /// <param name="id">ID of the player to remove.</param>
    /// <returns>True when a player was removed; otherwise false.</returns>
    public bool Remove(int id)
    {
        var player = Players.FirstOrDefault(item => item.Id == id);
        if (player is null) return false;
        Players.Remove(player);
        SavePlayers();
        return true;
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
