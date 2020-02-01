using System;
using System.Collections.Generic;

namespace NaiveGUI.Subscription
{
    public class SubscriptionManager : List<SubscriptionData>
    {
        public DateTime LastUpdate
        {
            get => lastUpdate;
            set
            {
                lastUpdate = value;
                MainForm.Instance.button_subscription.Text = "Subscriptions (" + lastUpdate.ToShortTimeString() + ")";
                // TODO: MainForm.Instance.UpdateTray();
            }
        }
        private DateTime lastUpdate = DateTime.MinValue;

        public int UpdateInterval = -1;

        public int Update(bool silent = true, bool force = false)
        {
            if(!force && (UpdateInterval < 0 || (DateTime.Now - LastUpdate).TotalSeconds < UpdateInterval))
            {
                return -1;
            }
            int count = 0;
            foreach(var s in this)
            {
                if(s.Update(silent))
                {
                    count++;
                }
            }
            LastUpdate = DateTime.Now;
            MainForm.Instance.Save();
            return count;
        }
    }
}
