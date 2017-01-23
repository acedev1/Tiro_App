using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class ButtonGroup : StackLayout
    {
        private List<string> _buttonNames;
        private List<Button> _buttonList;

        public event EventHandler<int> OnButtonClicked;

        public ButtonGroup()
        {
            this.Orientation = StackOrientation.Horizontal;
            this.Spacing = 10;
        }

        public List<string> Buttons
        {
            set
            {
                _buttonNames = value;
                BuildLayout();
            }
            get
            {
                return _buttonNames;
            }
        }

        public int SelectedIndex { get; private set; } = 0;

        private void BuildLayout()
        {
            this.Children.Clear();
            _buttonList = new List<Button>();
            if (_buttonNames != null && _buttonNames.Count != 0)
            {
                for (var index = 0; index < _buttonNames.Count; index++)
                {
                    var button = new Button()
                    {
                        Text = _buttonNames[index],
                        FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                        HeightRequest = 40,
                        FontSize = 14,
                        BorderRadius = 5                        
                    };
                    button.TextColor = (index == SelectedIndex) ? Color.White : Color.FromHex("787878");
                    button.BackgroundColor = (index == SelectedIndex) ? Props.ButtonColor : Color.FromHex("E7E7E7");
                    button.Clicked += ButtonClicked;
                    _buttonList.Add(button);
                    this.Children.Add(button);
                }
            }
        }

        private void ButtonClicked(object sender, EventArgs e)
        {
            SelectedIndex = _buttonList.FindIndex(b => b == sender);
            for (var index = 0; index < _buttonList.Count; index++)
            {
                _buttonList[index].TextColor = (index == SelectedIndex) ? Color.White : Color.FromHex("787878");
                _buttonList[index].BackgroundColor = (index == SelectedIndex) ? Props.ButtonColor : Color.FromHex("E7E7E7");
            }
            OnButtonClicked?.Invoke(this, SelectedIndex);
        }
    }
}
