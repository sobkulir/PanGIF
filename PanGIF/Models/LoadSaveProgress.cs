namespace PanGIF.Models
{
    class LoadSaveProgress : ModelBase
    {
        private bool _isActive = false;
        private int _value = 0;
        private int _total = 0;

        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (value != _isActive)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Total
        {
            get => _total;
            set
            {
                if (value != _total)
                {
                    _total = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Value
        {
            get => _value;
            set
            {
                if (value != _value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
        public LoadSaveProgress()
        {

        }
    }
}
