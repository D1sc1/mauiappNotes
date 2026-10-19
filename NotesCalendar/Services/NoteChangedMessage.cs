using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NotesCalendar.Services;

/// <summary>Отправляется после сохранения или удаления заметки; значение — день заметки.</summary>
public sealed class NoteChangedMessage(DateTime date) : ValueChangedMessage<DateTime>(date);
