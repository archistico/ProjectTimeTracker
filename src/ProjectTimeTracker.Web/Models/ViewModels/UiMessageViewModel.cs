namespace ProjectTimeTracker.Web.Models.ViewModels;

public class UiMessageViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string CssClass { get; set; } = "alert-danger";

    public bool HasValue =>
        !string.IsNullOrWhiteSpace(Title) ||
        !string.IsNullOrWhiteSpace(Text);
}