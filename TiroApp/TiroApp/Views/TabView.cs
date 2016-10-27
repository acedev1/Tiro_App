using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class TabView : StackLayout
    {
        private List<string> _titles;
        private Color _lineColor;
        private bool _isInfoPage;
        private int _selectedIndex;
        private bool _isOrderProcess;

        public event EventHandler<int> OnIndexChange;

        public TabView()
            : this(null, Color.Accent)
        {
        }

        public TabView(List<string> titles, Color lineColor, bool isInfoPage = false, bool isOrderProcess = false)
        {
            _isOrderProcess = isOrderProcess;
            _titles = titles;
            _lineColor = lineColor;
            _isInfoPage = isInfoPage;
            SelectedIndex = 0;
            this.Orientation = StackOrientation.Horizontal;
            this.HorizontalOptions = LayoutOptions.Fill;
            if (isOrderProcess)
            {
                BackgroundColor = Props.GrayColor;
            }
            BuildLayout();
        }

        public List<string> Titles
        {
            get
            {
                return _titles;
            }
            set
            {
                _titles = value;
                BuildLayout();
            }
        }

        public Color LineColor
        {
            get
            {
                return _lineColor;
            }
            set
            {
                _lineColor = value;
                BuildLayout();
            }
        }

        public int SelectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            set
            {
                _selectedIndex = value;
                BuildLayout();
            }
        }

        private void BuildLayout()
        {
            if (_titles == null)
            {
                return;
            }

            this.Children.Clear();
            for (var i = 0; i < _titles.Count; i++)
            {
                var label = new CustomLabel();
                label.Text = _titles[i];
                label.TextColor = _isOrderProcess ? Color.Black : _isInfoPage ? Color.Gray : Color.White;
                label.HorizontalTextAlignment = TextAlignment.Center;
                label.VerticalTextAlignment = TextAlignment.Center;
                label.HorizontalOptions = LayoutOptions.FillAndExpand;
                label.HeightRequest = 36;
                label.FontSize = _isInfoPage ? Device.OnPlatform(16, 18, 14) : 28;
                label.FontFamily = _isInfoPage ? UIUtils.FONT_SFUIDISPLAY_REGULAR : UIUtils.FONT_BEBAS_REGULAR;

                var line = new BoxView();
                if (i == SelectedIndex)
                {
                    line.Color = _lineColor;
                }
                line.HeightRequest = 4;
                line.HorizontalOptions = LayoutOptions.FillAndExpand;
                line.VerticalOptions = LayoutOptions.EndAndExpand;

                var tab = new StackLayout();
                tab.Orientation = StackOrientation.Vertical;
                tab.HorizontalOptions = LayoutOptions.FillAndExpand;
                tab.VerticalOptions = LayoutOptions.Fill;
                tab.Children.Add(label);
                tab.Children.Add(line);
                var tapRecognizer = new TapGestureRecognizer();
                tapRecognizer.TappedCallbackParameter = i;
                tapRecognizer.TappedCallback += OnTabTap;
                tab.GestureRecognizers.Add(tapRecognizer);

                this.Children.Add(tab);
            }
            if (this.HeightRequest <= 0)
            {
                this.HeightRequest = 40;
            }
        }

        private void OnTabTap(View v, object o)
        {
            int index = (int)o;
            var boxOld = (BoxView)((StackLayout)this.Children[SelectedIndex]).Children[1];
            var boxNew = (BoxView)((StackLayout)this.Children[index]).Children[1];
            boxOld.Color = Color.Transparent;
            boxNew.Color = _lineColor;

            SelectedIndex = index;
            if (OnIndexChange != null)
            {
                OnIndexChange(this, index);
            }
        }
    }
}
