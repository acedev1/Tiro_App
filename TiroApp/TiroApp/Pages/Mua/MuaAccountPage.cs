using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Model;
using TiroApp.Views;
using Xamarin.Forms;

namespace TiroApp.Pages.Mua
{
    public class MuaAccountPage : MuaBasePage
    {
        public MuaAccountPage(JObject jObj)
        {
            Utils.SetupPage(this);
            this.BackgroundColor = Color.White;
            Client client = new MuaArtist(jObj, true);
            BuildLayout(client);
            AddSideMenu();
        }

        private void BuildLayout(Client client)
        {
            this.mainLayout.Children.Add(new AccountLayout(client, this), Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent(p => p.Width), Constraint.RelativeToParent(p => p.Height));
        }
    }
}
