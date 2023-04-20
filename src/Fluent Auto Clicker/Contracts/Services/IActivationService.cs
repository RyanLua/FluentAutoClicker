namespace Fluent_Auto_Clicker.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
