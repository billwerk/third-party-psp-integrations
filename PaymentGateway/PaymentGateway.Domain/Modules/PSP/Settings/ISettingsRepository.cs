using PaymentGateway.Domain.BillwerkSDK.Settings;
using PaymentGateway.Domain.Shared.ValueObjects;

namespace PaymentGateway.Domain.Modules.PSP.Settings;

public interface ISettingsRepository
{
    IMerchantPspSettings GetById(NotEmptyString pspSettingsId);

    IMerchantPspSettings GetDefault();

    void SaveSettings(PspSettings pspSettings);
}
