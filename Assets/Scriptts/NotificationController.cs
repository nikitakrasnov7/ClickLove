using System.Linq;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public enum NotificationType 
{
    None,
    Buy,
    Reward,
    NewLevel
}

public class NotificationController : MonoBehaviour
{
    [SerializeField] NotificationPanel[] notificationPanels;

    public void OpenNotification(NotificationType type,string hint)
    {
        var panel = notificationPanels.FirstOrDefault(t=>t.NotificationType == type);
        if (panel == null) return;

        panel.UpdateTextHint(hint);
        panel.OpenWindow();
    }
}
