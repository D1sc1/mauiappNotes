# Заметки с календарём (.NET MAUI)

Приложение для заметок, привязанных к датам. Работает на Android, iOS, macOS (Mac Catalyst) и Windows.

## Возможности

- Календарь месяца (6 недель × 7 дней) с учётом первого дня недели и языка системы.
- Навигация: «‹ ›» — месяц, «« »» — год; нажатие на заголовок («Октябрь 2026 ▾») открывает выбор месяца и года (1900–2200).
- Точка под числом — в этот день есть заметки; выделены сегодняшний и выбранный дни.
- Список заметок выбранного дня: «весь день» сверху, затем по времени.
- Создание и редактирование заметки: заголовок, текст, дата, необязательное время.
- Удаление: свайпом влево в списке или кнопкой на экране редактирования (с подтверждением).
- Цветные метки (6 цветов): выбираются на экране заметки, показываются полоской в списке и цветными точками под числом в календаре (до трёх, серая — заметка без метки).
- Поиск (кнопка «Поиск» вверху): по заголовку и тексту всех заметок без учёта регистра, слова через пробел должны встречаться все; фильтр по цвету метки (повторное нажатие снимает фильтр).
- Кнопка «Сегодня», светлая и тёмная темы.
- Данные хранятся локально в SQLite (`notes.db3` в каталоге данных приложения).

## Структура

```
NotesCalendar/
├── Models/        Note, CalendarDay, CalendarGrid (сетка месяца), MonthItem, NotePalette/ColorOption (метки)
├── Services/      INoteRepository / NoteRepository (SQLite), NoteChangedMessage
├── ViewModels/    CalendarViewModel, NoteEditViewModel, SearchViewModel (CommunityToolkit.Mvvm)
├── Views/         CalendarPage, NoteEditPage, SearchPage
├── Resources/     цвета, стили, иконка, заставка
└── Platforms/     Android, iOS, MacCatalyst, Windows
```

Архитектура — MVVM: страницы получают ViewModel через DI, навигация — Shell (маршрут `noteedit`
с параметрами `date` или `id`), после сохранения календарь узнаёт об изменении через `WeakReferenceMessenger`.

## Требования и запуск

- .NET SDK 10 и workload MAUI: `dotnet workload install maui`
- Android: `dotnet build -t:Run -f net10.0-android NotesCalendar/NotesCalendar.csproj`
- Windows: `dotnet build -t:Run -f net10.0-windows10.0.19041.0 NotesCalendar/NotesCalendar.csproj`
- iOS / Mac Catalyst — только на macOS с Xcode.

Или откройте `NotesCalendar.sln` в Visual Studio 2022/2026 либо VS Code с расширением .NET MAUI.

Идентификатор приложения (`ApplicationId`) в `NotesCalendar.csproj` замените на свой перед публикацией.
