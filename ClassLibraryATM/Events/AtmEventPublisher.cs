using ClassLibraryATM.Delegates;
using ClassLibraryATM.Interfaces;

namespace ClassLibraryATM.Events
{
    public class AtmEventPublisher : IAtmEventPublisher
    {
        public event AtmEventHandler<AuthenticatedEventArgs>? Authenticated;
        public event AtmEventHandler<BalanceCheckedEventArgs>? BalanceChecked;
        public event AtmEventHandler<WithdrawCompletedEventArgs>? WithdrawCompleted;
        public event AtmEventHandler<DepositCompletedEventArgs>? DepositCompleted;
        public event AtmEventHandler<TransferCompletedEventArgs>? TransferCompleted;

        public void PublishAuthenticated(object sender, AuthenticatedEventArgs e) =>
            Authenticated?.Invoke(sender, e);

        public void PublishBalanceChecked(object sender, BalanceCheckedEventArgs e) =>
            BalanceChecked?.Invoke(sender, e);

        public void PublishWithdrawCompleted(object sender, WithdrawCompletedEventArgs e) =>
            WithdrawCompleted?.Invoke(sender, e);

        public void PublishDepositCompleted(object sender, DepositCompletedEventArgs e) =>
            DepositCompleted?.Invoke(sender, e);

        public void PublishTransferCompleted(object sender, TransferCompletedEventArgs e) =>
            TransferCompleted?.Invoke(sender, e);
    }
}
