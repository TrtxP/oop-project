using ClassLibraryATM.Delegates;

namespace ClassLibraryATM.Interfaces
{
    public interface IAtmEventPublisher
    {
        event AtmEventHandler<AuthenticatedEventArgs>? Authenticated;
        event AtmEventHandler<BalanceCheckedEventArgs>? BalanceChecked;
        event AtmEventHandler<WithdrawCompletedEventArgs>? WithdrawCompleted;
        event AtmEventHandler<DepositCompletedEventArgs>? DepositCompleted;
        event AtmEventHandler<TransferCompletedEventArgs>? TransferCompleted;

        void PublishAuthenticated(object sender, AuthenticatedEventArgs e);
        void PublishBalanceChecked(object sender, BalanceCheckedEventArgs e);
        void PublishWithdrawCompleted(object sender, WithdrawCompletedEventArgs e);
        void PublishDepositCompleted(object sender, DepositCompletedEventArgs e);
        void PublishTransferCompleted(object sender, TransferCompletedEventArgs e);
    }
}
