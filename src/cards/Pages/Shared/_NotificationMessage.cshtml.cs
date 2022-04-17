namespace cards;

public class NotificationMessageModel
{
    public NotificationMessageModel(Level notificationLevel, string message)
    {
        NotificationLevel = notificationLevel;
        Message = message;
    }

    public Level NotificationLevel { get; set; }
    public string Message { get; set; }

    public enum Level
    {
        Info,
        Warning,
        Danger
    }
}