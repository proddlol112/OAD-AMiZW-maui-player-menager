using MauiApp3.Commands;
using MauiApp3.Model;
using MauiApp3.Queries;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiApp3.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly GetPlayerByIdQueryHandler _getPlayerByIdQueryHandler;
    private readonly GetPlayersQueryHandler _getPlayersQueryHandler;
    private readonly AddPlayerCommandHandler _addPlayerCommandHandler;
    private string _playerName = "";
    private string _playerLevel = "";
    private string _playerId = "";
    private string _searchResult = "";
    private string _statusMessage = "";

    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<Player> Players { get; } = new();
    public int PlayerCount => Players.Count;
    public string PlayerName { get => _playerName; set => SetField(ref _playerName, value); }
    public string PlayerLevel { get => _playerLevel; set => SetField(ref _playerLevel, value); }
    public string PlayerId { get => _playerId; set => SetField(ref _playerId, value); }
    public string SearchResult { get => _searchResult; set => SetField(ref _searchResult, value); }
    public string StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }
    public ICommand AddPlayerCommandButton { get; }
    public ICommand FindPlayerCommandButton { get; }

    public MainViewModel(GetPlayerByIdQueryHandler getPlayerByIdQueryHandler,
        GetPlayersQueryHandler getPlayersQueryHandler,
        AddPlayerCommandHandler addPlayerCommandHandler)
    {
        _getPlayerByIdQueryHandler = getPlayerByIdQueryHandler;
        _getPlayersQueryHandler = getPlayersQueryHandler;
        _addPlayerCommandHandler = addPlayerCommandHandler;
        AddPlayerCommandButton = new Command(AddPlayer);
        FindPlayerCommandButton = new Command(FindPlayer);
        LoadPlayers();
    }

    private void AddPlayer()
    {
        var name = PlayerName.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            StatusMessage = "Wpisz nazwę gracza.";
            return;
        }
        if (!int.TryParse(PlayerLevel, out var level) || level < 0)
        {
            StatusMessage = "Poziom musi być liczbą całkowitą równą 0 lub większą.";
            return;
        }

        _addPlayerCommandHandler.Handle(new AddPlayerCommand(name, level));
        LoadPlayers();
        PlayerName = "";
        PlayerLevel = "";
        StatusMessage = $"Dodano gracza {name}.";
    }

    private void FindPlayer()
    {
        if (!int.TryParse(PlayerId, out var id) || id < 1)
        {
            SearchResult = "Wpisz prawidłowe ID gracza.";
            return;
        }

        var player = _getPlayerByIdQueryHandler.Handle(new GetPlayerByIdQuery(id));
        SearchResult = player is null
            ? $"Nie znaleziono gracza o ID {id}."
            : $"{player.Name} — poziom {player.Level}";
    }

    private void LoadPlayers()
    {
        Players.Clear();
        foreach (var player in _getPlayersQueryHandler.Handle(new GetPlayersQuery()))
            Players.Add(player);
        OnPropertyChanged(nameof(PlayerCount));
    }

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
