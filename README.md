# OAD-AMiZW Maui Player Manager

Wydawca: **proddlol112**

## Uruchomienie w Windows

1. Uruchom `setup_maui_windows.bat`, aby przygotować środowisko.
2. Uruchom `run_windows.bat`, aby skompilować i otworzyć aplikację.

Aplikacja pozwala dodawać graczy, wyświetlać listę, wyszukiwać gracza po ID, edytować jego nazwę i poziom oraz usuwać go z listy. Gracze są zapisywani w pliku JSON w katalogu danych aplikacji, więc pozostają po jej zamknięciu.

## Edycja i usuwanie gracza

W wierszu gracza wybierz **Edytuj**, aby otworzyć formularz zmian. Zapisz zmiany albo anuluj edycję. Przycisk **Usuń** usuwa wybranego gracza i zapisuje listę na dysku.

## Dokumentacja kodu

### Klasa `Player`

Reprezentuje gracza przechowywanego na liście.

| Właściwość | Typ | Opis |
| --- | --- | --- |
| `Id` | `int` | Unikalny identyfikator gracza. |
| `Name` | `string` | Nazwa gracza. |
| `Level` | `int` | Poziom gracza. |

### Klasa `PlayerRepository`

Zarządza kolekcją graczy oraz jej zapisem i odczytem z pliku JSON.

| Funkcja | Opis | Parametry | Zwraca |
| --- | --- | --- | --- |
| `Add(Player player)` | Dodaje gracza i zapisuje listę. | `player`, `Player`, dodawany gracz. | `void`, brak. |
| `Update(Player player)` | Aktualizuje nazwę i poziom gracza o podanym ID. | `player`, `Player`, obiekt z ID i nowymi danymi. | `bool`, czy znaleziono i zapisano gracza. |
| `Remove(int id)` | Usuwa gracza o podanym ID i zapisuje listę. | `id`, `int`, ID usuwanego gracza. | `bool`, czy znaleziono i usunięto gracza. |

### Klasa `MainViewModel`

Obsługuje dane widoku, walidację formularzy oraz akcje dodawania, wyszukiwania, edycji i usuwania graczy. `SelectPlayerForEdit` wypełnia formularz danymi wybranego gracza, `SavePlayerEdit` sprawdza formularz i zapisuje zmiany, `CancelPlayerEdit` zamyka edycję, a `DeletePlayer` usuwa wskazanego gracza.

Jeśli repozytorium zostało pobrane wcześniej, zaktualizuj je poleceniem `git pull`, a następnie ponownie uruchom konfigurację, jeśli brakuje `run_windows.bat`.
