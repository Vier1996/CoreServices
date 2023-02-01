using System;

namespace ACS.GDPR.Service
{
    public interface IGdprService
    {
        public event Action Accepted;
        public event Action Declined;
        public void ShowGdpr(bool remember = true);
    }
}