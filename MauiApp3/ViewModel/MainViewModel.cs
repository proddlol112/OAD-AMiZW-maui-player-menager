using MauiApp3.Commands;
using MauiApp3.Data;
using MauiApp3.Model;
using MauiApp3.Queries;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MauiApp3.ViewModel;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly GetPlayersQueryHandler _getPlayersQueryHandler;
    private readonly AddPlayerCommandHandler _addPlayerCommandHandler;
    private readonly PlayerRepository _playerRepository;
    private string _playerName = "";
    private string _playerLevel = "";
    private string _playerId = "";
    private string _searchResult = "";
    private string _statusMessage = "";
    private string _editPlayerName = "";
    private string _editPlayerLevel = "";
    private int? _editingPlayerId;

    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<Player> Players { get; } = new();
    public int PlayerCount => Players.Count;
    public string PlayerName { get => _playerName; set => SetField(ref _playerName, value); }
    public string PlayerLevel { get => _playerLevel; set => SetField(ref _playerLevel, value); }
    public string PlayerId { get => _playerId; set => SetField(ref _playerId, value); }
    public string SearchResult { get => _searchResult; set => SetField(ref _searchResult, value); }
    public string StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }
    public string EditPlayerName { get => _editPlayerName; set => SetField(ref _editPlayerName, value); }
    public string EditPlayerLevel { get => _editPlayerLevel; set => SetField(ref _editPlayerLevel, value); }
    public bool IsEditing => _editingPlayerId.HasValue;
    public ICommand AddPlayerCommandButton { get; }
    public ICommand FindPlayerCommandButton { get; }
    public ICommand SelectPlayerForEditCommand { get; }
    public ICommand SavePlayerEditCommand { get; }
    public ICommand CancelPlayerEditCommand { get; }
    public ICommand DeletePlayerCommand { get; }

    public MainViewModel(GetPlayersQueryHandler getPlayersQueryHandler,
        AddPlayerCommandHandler addPlayerCommandHandler,
        PlayerRepository playerRepository)
    {
        _getPlayersQueryHandler = getPlayersQueryHandler;
        _addPlayerCommandHandler = addPlayerCommandHandler;
        _playerRepository = playerRepository;
        AddPlayerCommandButton = new Command(AddPlayer);
        FindPlayerCommandButton = new Command(FindPlayer);
        SelectPlayerForEditCommand = new Command<Player>(SelectPlayerForEdit);
        SavePlayerEditCommand = new Command(SavePlayerEdit);
        CancelPlayerEditCommand = new Command(CancelPlayerEdit);
        DeletePlayerCommand = new Command<Player>(DeletePlayer);
        LoadPlayers();
    }

    private void SelectPlayerForEdit(Player? player)
    {
        if (player is null) return;
        _editingPlayerId = player.Id;
        EditPlayerName = player.Name;
        EditPlayerLevel = player.Level.ToString();
        OnPropertyChanged(nameof(IsEditing));
        StatusMessage = $"Edytujesz gracza {player.Name}.";
    }

    private void SavePlayerEdit()
    {
        if (!_editingPlayerId.HasValue) return;
        var name = EditPlayerName.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            StatusMessage = "Wpisz nazwę gracza.";
            return;
        }
        if (!int.TryParse(EditPlayerLevel, out var level) || level < 0)
        {
            StatusMessage = "Poziom musi być liczbą całkowitą równą 0 lub większą.";
            return;
        }
        if (!_playerRepository.Update(new Player { Id = _editingPlayerId.Value, Name = name, Level = level }))
        {
            StatusMessage = "Nie znaleziono gracza do edycji.";
            CancelPlayerEdit();
            return;
        }
        CancelPlayerEdit();
        LoadPlayers();
        StatusMessage = $"Zapisano zmiany gracza {name}.";
    }

    private void CancelPlayerEdit()
    {
        _editingPlayerId = null;
        EditPlayerName = "";
        EditPlayerLevel = "";
        OnPropertyChanged(nameof(IsEditing));
    }

    private void DeletePlayer(Player? player)
    {
        if (player is null) return;
        if (!_playerRepository.Remove(player.Id)) return;
        if (_editingPlayerId == player.Id) CancelPlayerEdit();
        LoadPlayers();
        StatusMessage = $"Usunięto gracza {player.Name}.";
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
        if (!int.TryParse(PlayerId.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) || id < 1)
        {
            SearchResult = "Wpisz prawidłowe ID gracza.";
            return;
        }

        // Search the same refreshed player list that is shown on screen.
        LoadPlayers();
        var player = Players.FirstOrDefault(item => item.Id == id);
        SearchResult = player is null
            ? $"Nie znaleziono gracza o ID {id}. Dostępne ID: {string.Join(", ", Players.Select(item => item.Id))}."
            : $"ID {player.Id}: {player.Name} — poziom {player.Level}";
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
