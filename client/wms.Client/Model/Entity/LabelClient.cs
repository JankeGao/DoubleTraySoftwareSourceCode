using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace wms.Client.Model.Entity
{
    public class LabelClient : INotifyPropertyChanged
    {
        private string _labelCode;
        public string LabelCode
        {
            get { return _labelCode; }
            set { _labelCode = value; NotifyPropertyChanged(); }
        }

        private string _batchCode;
        public string BatchCode
        {
            get { return _batchCode; }
            set { _batchCode = value; NotifyPropertyChanged(); }
        }

        private decimal? _quantity;
        public decimal? Quantity
        {
            get { return _quantity; }
            set { _quantity = value; NotifyPropertyChanged(); }
        }
        private string _supplyCode;
        public string SupplyCode
        {
            get { return _supplyCode; }
            set { _supplyCode = value; NotifyPropertyChanged(); }
        }
        private string _supplyName;
        public string SupplyName
        {
            get { return _supplyName; }
            set { _supplyName = value; NotifyPropertyChanged(); }
        }
        private string _materialCode;
        public string MaterialCode
        {
            get { return _materialCode; }
            set { _materialCode = value; NotifyPropertyChanged(); }
        }
        private string _materialName;
        public string MaterialName
        {
            get { return _materialName; }
            set { _materialName = value; NotifyPropertyChanged(); }
        }
        private string _materialUrl;
        public string MaterialUrl
        {
            get { return _materialUrl; }
            set { _materialUrl = value; NotifyPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
