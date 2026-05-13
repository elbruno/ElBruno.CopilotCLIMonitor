using System.Globalization;
using ElBruno.CopilotCLIMonitor.Core.Models;

namespace ElBruno.CopilotCLIMonitor.Services;

public static class LocalizedText
{
    private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> LanguageMap =
        new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["en"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Notification"] = "Notification",
                ["TaskCompleted"] = "Task Completed",
                ["ApprovalRequired"] = "Approval Required",
                ["Error"] = "Error",
                ["Warning"] = "Warning",
                ["BuildCompleted"] = "Build Completed",
                ["TestCompleted"] = "Test Completed",
                ["WorkflowCompleted"] = "Workflow Completed",
                ["LongRunningTaskWarning"] = "Long-Running Task",
                ["HookFailed"] = "Hook Failed"
            },
            ["es"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Notification"] = "Notificación",
                ["TaskCompleted"] = "Tarea completada",
                ["ApprovalRequired"] = "Aprobación requerida",
                ["Error"] = "Error",
                ["Warning"] = "Advertencia",
                ["BuildCompleted"] = "Compilación completada",
                ["TestCompleted"] = "Pruebas completadas",
                ["WorkflowCompleted"] = "Flujo completado",
                ["LongRunningTaskWarning"] = "Tarea de larga duración",
                ["HookFailed"] = "Error del hook"
            },
            ["fr"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["Notification"] = "Notification",
                ["TaskCompleted"] = "Tâche terminée",
                ["ApprovalRequired"] = "Approbation requise",
                ["Error"] = "Erreur",
                ["Warning"] = "Avertissement",
                ["BuildCompleted"] = "Build terminé",
                ["TestCompleted"] = "Tests terminés",
                ["WorkflowCompleted"] = "Workflow terminé",
                ["LongRunningTaskWarning"] = "Tâche longue",
                ["HookFailed"] = "Échec du hook"
            }
        };

    public static string Get(string key, CultureInfo? culture = null)
    {
        var language = (culture ?? CultureInfo.CurrentUICulture).TwoLetterISOLanguageName;
        if (LanguageMap.TryGetValue(language, out var labels) && labels.TryGetValue(key, out var localized))
        {
            return localized;
        }

        return LanguageMap["en"].TryGetValue(key, out var english) ? english : key;
    }

    public static string GetEventTypeLabel(EventType eventType, CultureInfo? culture = null) =>
        Get(eventType.ToString(), culture);
}
