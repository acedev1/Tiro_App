using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Model;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages
{
    public class AccountPage : BasePage
    {
        private Client client;
        public AccountPage(JObject jObj)
        {
            Utils.SetupPage(this);
            this.BackgroundColor = Color.White;            
            client = new Customer(jObj, true);
            AddSideMenu();
        }

        private void BuildLayout()
        {
            this.mainLayout.Children.Add(new AccountLayout(client, this), Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BuildLayout();
        }
    }
}
