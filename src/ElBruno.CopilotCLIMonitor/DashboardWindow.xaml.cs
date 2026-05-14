using System.Collections.ObjectModel;
using System.Windows;
using ElBruno.CopilotCLIMonitor.Core.Models;
using ElBruno.CopilotCLIMonitor.Core.Services;
using ElBruno.CopilotCLIMonitor.Services;

namespace ElBruno.CopilotCLIMonitor;

public partial class DashboardWindow : Window
{
    private readonly EventStore _store;
    private readonly ObservableCollection<EventViewModel> _items = [];
    private List<MonitorEvent> _allEvents = [];
    private const string AllEventTypes = "All";

    public DashboardWindow(EventStore store)
    {
        _store = store;
        InitializeComponent();
        UiCultureSupport.ApplyFlowDirection(this);
        EventList.ItemsSource = _items;
        EventTypeFilterComboBox.ItemsSource = new[] { AllEventTypes }.Concat(Enum.GetNames<EventType>());
        EventTypeFilterComboBox.SelectedIndex = 0;
    }

    public void RefreshEvents(IReadOnlyList<MonitorEvent> events)
    {
        _allEvents = events.ToList();
        ApplyFilters();
    }

    private void ApplyFilters()
    {
        var filtered = FilterEvents(_allEvents, FilterTextBox.Text, EventTypeFilterComboBox.SelectedItem as string);
        _items.Clear();
        foreach (var e in filtered.OrderByDescending(x => x.OccurredAt))
        {
            _items.Add(new EventViewModel(e));
        }

        StatusText.Text = UiResources.Get("StatusListeningTemplate", _items.Count, IpcConstants.DefaultPort);
    }

    private void ClearHistory_Click(object sender, RoutedEventArgs e)
    {
        _store.Clear();
        _allEvents.Clear();
        _items.Clear();
        StatusText.Text = UiResources.Get("StatusHistoryCleared");
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Hide();

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    private void FilterTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) => ApplyFilters();

    private void EventTypeFilterComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => ApplyFilters();

    public static IReadOnlyList<MonitorEvent> FilterEvents(IEnumerable<MonitorEvent> events, string? textFilter, string? eventTypeFilter)
    {
        var query = events;

        if (!string.IsNullOrWhiteSpace(eventTypeFilter) && !string.Equals(eventTypeFilter, AllEventTypes, StringComparison.Ordinal))
        {
            query = query.Where(e => string.Equals(e.EventType.ToString(), eventTypeFilter, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(textFilter))
        {
            var needle = textFilter.Trim();
            query = query.Where(e =>
                e.Message.Contains(needle, StringComparison.OrdinalIgnoreCase) ||
                (e.Repository?.Contains(needle, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (e.Branch?.Contains(needle, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (e.OriginRepository?.Contains(needle, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        return query.ToList();
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
