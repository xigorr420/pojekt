Dokumentacja projektu
1. Wymagania systemowe
1.1. Wymagania sprzętowe
Procesor: Minimum 2 GHz, zalecany 4-rdzeniowy procesor.
RAM: Minimum 4 GB, zalecane 8 GB.
Dysk twardy: Minimum 500 MB wolnego miejsca.
1.2. Wymagania programowe
System operacyjny: Windows 10/11 lub Linux.
.NET SDK: Minimum wersja 6.0.
Przeglądarka internetowa: Google Chrome, Mozilla Firefox, Microsoft Edge.
2. Instalacja aplikacji
2.1. Pobranie projektu z repozytorium Git
Sklonuj repozytorium projektu:
git clone https://github.com/xigorr420/project.git
Przejdź do katalogu projektu:
2.2. Instalacja zależności
Przygotuj środowisko uruchamiając polecenie:
dotnet restore
Upewnij się, że zainstalowane są wymagane pakiety NuGet:
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Sqlite
Microsoft.EntityFrameworkCore.Tools 
Możesz zainstalować je ręcznie za pomocą poleceń:
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
2.3. Konfiguracja bazy danych
W pliku appsettings.json skonfiguruj połączenie z bazą SQLite:
"ConnectionStrings": {
  		  "DefaultConnection": "Data Source=app.db"
}
Baza danych SQLite zostanie automatycznie utworzona i połączona podczas pierwszego uruchomienia aplikacji.
Wykonaj migrację bazy danych:
dotnet ef database update
Uwaga: Upewnij się, że narzędzie EF Core Tools jest zainstalowane:
3. Konfiguracja testowych użytkowników
3.1. Dodawanie użytkowników
W bazie danych istnieje tabela Users, do której można dodać użytkowników testowych. Przygotowano poniższą listę:
ID
Login
Hasło
Rola
1
admin@example.com
admin123
Admin
2
user@example.com
user123
User

4. Uruchamianie aplikacji
Otwórz terminal w katalogu projektu (folder w którym jest pojekt.csproj).
Wykonaj komendę:
dotnet run
Aplikacja uruchomi się na domyślnym porcie 5179 (http://localhost:5179). W przypadku zmian portu można je zdefiniować w appsettings.json jeżeli powodowałoby to problemy.
5. Opis działania aplikacji
5.1. Moduł zarządzania użytkownikami
Rejestracja nowych użytkowników: Widok User/Register.cshtml umożliwia tworzenie kont użytkowników.
Logowanie: Widok User/Login.cshtml pozwala użytkownikom zalogować się do systemu.
Zarządzanie danymi użytkownika: Widok User/Update.cshtml umożliwia edycję danych użytkownika.
5.2. Moduł zarządzania produktami
Dodawanie produktów: Formularz Home/AddProduct.cshtml umożliwia administratorom dodawanie nowych produktów do bazy danych.
Edytowanie produktów: Widok Home/Edit.cshtml umożliwia edycję istniejących danych produktów.
Przegląd produktów: Widok Home/Index.cshtml wyświetla listę dostępnych produktów.
5.3. Moduł zamówień
Przegląd zamówień: Widok Orders/Index.cshtml pokazuje historię zamówień.
Edytowanie zamówień: Widok Orders/Edit.cshtml umożliwia zmianę szczegółów zamówień.
5.4. Układ i nawigacja
Główny layout aplikacji znajduje się w pliku Views/Shared/_Layout.cshtml, a stylizacja jest zawarta w Views/Shared/_Layout.cshtml.css.

6. Graficzna reprezentacja połączeń między tabelami
UserDetails
Users
Orders
Products
 UserId
 PhoneNumber
 City
 Street
 HouseNumber
  ID
  Name
  Email
  Password
  Role
Id
UserId
ProductId
Quantity
TotalPrice
Status
ProductId
Name
Description
Category
Quantity
Price




Relacje
User - UserDetails (1:1)
Każdy użytkownik ma jedne szczegóły użytkownika.
User - Orders (1:Wielu)
Jeden użytkownik może mieć wiele zamówień.
Product - Orders (1:Wielu)
Jeden produkt może być zawarty w wielu zamówieniach.
8. Uwagi końcowe
Wszystkie widoki są odpowiednio dostosowane dla użytkowników różnych ról (np. administrator vs zwykły użytkownik).
W przypadku problemów z połączeniem z bazą danych należy sprawdzić poprawność łańcucha połączenia w appsettings.json.
Autor: Igor Książek



