using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClinifyMobile.ViewModels.Base;

/// <summary>
/// ViewModel base com INotifyPropertyChanged, IsBusy e SetProperty.
/// Todos os ViewModels herdam desta classe.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    // ─── INotifyPropertyChanged ──────────────────────────────────────────────

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value)) return false;
        backingField = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    // ─── IsBusy ──────────────────────────────────────────────────────────────

    private bool _isBusy;
    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            SetProperty(ref _isBusy, value);
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }
    public bool IsNotBusy => !_isBusy;

    // ─── Title ───────────────────────────────────────────────────────────────

    private string _title = string.Empty;
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    // ─── Error ───────────────────────────────────────────────────────────────

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            SetProperty(ref _errorMessage, value);
            OnPropertyChanged(nameof(HasError));
        }
    }
    public bool HasError => !string.IsNullOrEmpty(_errorMessage);

    protected void ClearError() => ErrorMessage = string.Empty;

    // ─── Safe Execute ────────────────────────────────────────────────────────

    /// <summary>
    /// Executa uma operação async com controle de IsBusy e tratamento de erro.
    /// </summary>
    protected async Task ExecuteAsync(Func<Task> operation, string? errorPrefix = null)
    {
        if (IsBusy) return;
        IsBusy = true;
        ClearError();
        try
        {
            await operation();
        }
        catch (Services.ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"{errorPrefix ?? "Erro"}: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
