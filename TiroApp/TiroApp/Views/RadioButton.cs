using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class RadioButton : StackLayout
    {
        protected bool _isChecked = false;
        protected Image _image;
        protected CustomLabel _label;
        //protected string imgRadioChecked = "TiroApp.Images.RadioChecked.png";
        //protected string imgRadioUnchecked = "TiroApp.Images.RadioUnchecked.png";
        protected string imgChecked = "TiroApp.Images.checked.png";
        protected string imgUnchecked = "TiroApp.Images.unchecked.png";
        private bool isRadio;

        public event EventHandler OnCheckedChange;

        public RadioButton(bool isRadio = true)
        {
            this.isRadio = isRadio;
            this.Orientation = StackOrientation.Horizontal;
            _image = new Image();
            _image.VerticalOptions = LayoutOptions.Center;
            _image.Source = ImageSource.FromResource(imgUnchecked);
            if (!isRadio)
            {
                _image.HeightRequest = 20;
                _image.WidthRequest = _image.HeightRequest;
            }
            _image.Margin = new Thickness(20, 0);
            _label = new CustomLabel();
            _label.VerticalOptions = LayoutOptions.Center;
            this.Children.Add(_image);
            this.Children.Add(_label);
            this.GestureRecognizers.Add(new TapGestureRecognizer(OnTap));
        }

        private void OnTap(View v)
        {
            if (IsChecked && isRadio)
            {
                return;
            }
            IsChecked = !IsChecked;
            if (OnCheckedChange != null)
            {
                OnCheckedChange(this, EventArgs.Empty);
            }
        }

        protected void RefreshState()
        {
            //if (isRadio)
            //{
            //    _image.Source = ImageSource.FromResource(_isChecked ? imgRadioChecked : imgRadioUnchecked);
            //}
            //else
            //{
                
            //}
            _image.Source = ImageSource.FromResource(_isChecked ? imgChecked : imgUnchecked);
        }

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                RefreshState();
            }
        }

        public string Text
        {
            get
            {
                return _label.Text;
            }
            set
            {
                _label.Text = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return _label.TextColor;
            }
            set
            {
                _label.TextColor = value;
            }
        }

        public string FontFamily
        {
            get
            {
                return _label.FontFamily;
            }
            set
            {
                _label.FontFamily = value;
            }
        }

        public double FontSize
        {
            get
            {
                return _label.FontSize;
            }
            set
            {
                _label.FontSize = value;
            }
        }
    }
}
