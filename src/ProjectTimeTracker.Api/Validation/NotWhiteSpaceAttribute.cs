using System.ComponentModel.DataAnnotations;

namespace ProjectTimeTracker.Api.Validation;

/// <summary>
/// Valida che una stringa non sia nulla, vuota o composta solo da spazi bianchi.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class NotWhiteSpaceAttribute : ValidationAttribute
{
    /// <summary>
    /// Inizializza l'attributo con un messaggio di errore di default.
    /// </summary>
    public NotWhiteSpaceAttribute()
        : base("Il campo {0} non può essere vuoto o contenere solo spazi.")
    {
    }

    /// <summary>
    /// Verifica la validità del valore.
    /// </summary>
    /// <param name="value">Valore da validare.</param>
    /// <returns><c>true</c> se il valore è valido; altrimenti <c>false</c>.</returns>
    public override bool IsValid(object? value)
    {
        return value is string text && !string.IsNullOrWhiteSpace(text);
    }
}