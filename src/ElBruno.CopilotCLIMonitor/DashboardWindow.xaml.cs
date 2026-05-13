using System.Collections.ObjectModel;
using System.Windows;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;

namespace ElBruno.CopilotCLIMonitor;

public partial class DashboardWindow : Window
{
    private readonly EventStore _store;
    private readonly ObservableCollection<EventViewModel> _items = [];

    public DashboardWindow(EventStore store)
    {
        _store = store;
        InitializeComponent();
        EventList.ItemsSource = _items;
    }

    public void RefreshEvents(IReadOnlyList<MonitorEvent> events)
    {
        _items.Clear();
        foreach (var e in events.OrderByDescending(x => x.OccurredAt))
            _items.Add(new EventViewModel(e));

        StatusText.Text = $"{_items.Count} event(s)   |   Listening on port {IpcConstants.DefaultPort}";
    }

    private void ClearHistory_Click(object sender, RoutedEventArgs e)
    {
        _store.Clear();
        _items.Clear();
        StatusText.Text = "History cleared.";
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Hide();

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}

/// <summary>Display-ready view model for a MonitorEvent.</summary>
public sealed class EventViewModel
{
    public EventViewModel(MonitorEvent e)
    {
        TimestampDisplay = e.OccurredAt.LocalDateTime.ToString("yyyy-MM-dd HH:mm:ss");
        EventTypeDisplay = e.EventType.ToString();
        Repository = e.Repository ?? string.Empty;
        Branch = e.Branch ?? string.Empty;
        Message = e.Message;
    }

    public string TimestampDisplay { get; }
    public string EventTypeDisplay { get; }
    public string Repository { get; }
    public string Branch { get; }
    public string Message { get; }
}
