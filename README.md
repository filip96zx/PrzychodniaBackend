# Deploy

Strona pod którą można zobaczyć projekt [Clinic](http://vps-63e72677.vps.ovh.net).\
http://vps-63e72677.vps.ovh.net

## Konta użytkowników


Lekarz: `lekarz@email.pl`, hasło: `Haslo123.`,\
pacjent: `pacjent@email.pl` hasło: `Haslo123.`,\
admina login: `admin@email.pl`  hasło: `Haslo123.`

## Instrukcja

### `admin`-Admin, Panel Admina

Wyszukiwarka umożliwia filtrowanie po wszystkich właściwościach użytkowników, także po rolach.\
Z listy użytkowników moża edytować dane użytkownika, edytować role.\
W aplikacji są wbudowane trzy role: admin, user, doctor. Odpowiadają one typom kont.\
Do konta z rolą `doctor` należy także przypisać specjalizacje pod którymi będzie można planować wizyty.\
Specjalizacje lekarskie są także rolami które `admin` może tworzyć lub usuwać jeżeli żaden użytkownik nie ma przypisanej danej specjalizacji. 


### `doctor`- Lekarz, Panel Lekarza

Lekarz ma podgląd swoich wizyt w danym tygodniu i możliwość przełączania się między tygodniami.\
Nowe wizyty dodaje się formularzem pod wybranym dniem, gdzie należy ustawić godzinę i wybrać specjalizację pod którą ma być wizyta. po zakończeniu dodawania nowych wizyt należy zapisać je w bazie danych przyciskiem `Zapisz nowe wizyty`.\
W wizycie na którą ktoś się zarejestrował można przejść do szczegółów, w modalu który się wyświetli można: wystawić recepte, przesłać wyniki badań lub wysyłać wiadomości do pacjenta.
Istnieje możliwość odwołania lub zakończenia wizyty, w tym momencie zmiana ta ma charakter informacyjny nie niesie za sobą innych następstw niż wyświetlanie odpowiednio w historii wizyt lub  liście zaplanowanych wizyt na koncie pacjenta.\
\
Tworzenie wizyt w przeszłym terminie celowo nie zostało ogarniczone do ułatwienia testowania funkcjonalności.


### `user`- Pacjent

#### Rejestracja wizyt

Należy wybrać typ wizyty i tydzień, opcjonalnie można wybrać konkretnego lekarza którego wizyt szukamy.\
Z wyświetlonych wizyt moża zarejestrować się na wolne terminy przyciskiem `zarejestuj`.\
Rejestracja jest ograniczona do jednej wizyt danego typu u danego lekarza w danym dniu.\
Rejestracja na wizyty w przeszłym terminie celowo nie została ogarniczona do ułatwienia testowania funkcjonalności.

#### Zapanowane wizyty

Lista zaplanowanych wizyt, ze szczegółów wizyty można odczytać dane lekarza, recepty, wyniki badań, a także pisać wiadomości do lekarza.\
Można wypisać się z zarezerwowanej wizyty przyciskiem `anuluj rezerwację` w szczegółach wizyty w zakładce `informacje`.

#### Historia wizyt

Lista zakończonych lub anulowanych przez lekarza wizyt, ze szczegółów wizyty można odczytać dane lekarza, recepty, wyniki badań, a także pisać wiadomości do lekarza.


## Rejestracja konta

Rejestracja za pomocą formularza po przejsciu do logowania następnie naciśnięciu przycisku `rejestracja`.

## przypomnienie hasła

Przypomnienie hasła za pomocą formularza po przejsciu do logowania następnie naciśnięciu odnośnika `Przypomnij hasło`, jeżeli na podany email jest zarejestrowane konto zostanie wysłana wiadomość z linkiem do zrestartowania hasła.
