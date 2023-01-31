using System;

namespace ACS.GDPR.com.alexplay.gdpr.Runtime.Service
{
    public interface IGdprService
    {
        public event Action Accepted;
        public event Action Declined;
        public void ShowGdpr(bool remember = true);
    }
}