using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class CustomerSignupView : StackLayout
    {
        private CustomEntry firstNameEntry;
        private CustomEntry lastNameEntry;
        private CustomEntry phoneNumberEntry;
        private CustomEntry emailEntry;        
        private CustomEntry pswdEntry;
        private CustomEntry codeEntry;
        private Button continueButton;
        private int currentStep = 1;
        private Page page;
        private string currentCode;
        private ActivityIndicator spinner;

        public event EventHandler<ResponseDataJson> OnFinish;

        public CustomerSignupView(Page p, bool isOrder = false)
        {
            this.page = p;

            this.Orientation = StackOrientation.Vertical;
            this.Spacing = 0;
            this.BackgroundColor = Color.White;            

            continueButton = UIUtils.MakeButton("CONTINUE", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            continueButton.Clicked += OnContinue;
            continueButton.Margin = isOrder ? new Thickness(0, 10, 0, 0) : new Thickness(0, 40, 0, 0);

            BuildStep1();
        }

        private void BuildStep1()
        {
            this.Children.Clear();

            firstNameEntry = UIUtils.MakeEntry("First Name", UIUtils.FONT_SFUIDISPLAY_BOLD);
            this.Children.Add(firstNameEntry);
            this.Children.Add(UIUtils.MakeSeparator());

            lastNameEntry = UIUtils.MakeEntry("Last Name", UIUtils.FONT_SFUIDISPLAY_BOLD);
            this.Children.Add(lastNameEntry);
            this.Children.Add(UIUtils.MakeSeparator());

            phoneNumberEntry = UIUtils.MakeEntry("Phone Number", UIUtils.FONT_SFUIDISPLAY_BOLD);
            this.Children.Add(phoneNumberEntry);
            this.Children.Add(UIUtils.MakeSeparator());

            emailEntry = UIUtils.MakeEntry("Email", UIUtils.FONT_SFUIDISPLAY_BOLD);
            this.Children.Add(emailEntry);
            this.Children.Add(UIUtils.MakeSeparator());

            pswdEntry = UIUtils.MakeEntry("Password", UIUtils.FONT_SFUIDISPLAY_BOLD);
            pswdEntry.IsPassword = true;
            this.Children.Add(pswdEntry);
            this.Children.Add(UIUtils.MakeSeparator());

            this.Children.Add(continueButton);

            this.ForceLayout();
            this.currentStep = 1;
        }

        private void BuildStep2()
        {
            this.Children.Clear();

            var label = new CustomLabel()
            {
                Text = "We have sent a confirmation code to the mobile number below. Please enter the confirmation code.",
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20),
                HeightRequest = 70
            };
            this.Children.Add(label);

            phoneNumberEntry.Focused += (o, a) => { codeEntry.Focus(); };
            this.Children.Add(phoneNumberEntry);
            this.Children.Add(UIUtils.MakeSeparator());

            codeEntry = UIUtils.MakeEntry("Confirmation code", UIUtils.FONT_SFUIDISPLAY_BOLD);
            this.Children.Add(codeEntry);
            this.Children.Add(UIUtils.MakeSeparator());

            this.Children.Add(continueButton);

            this.ForceLayout();
            this.currentStep = 2;
        }

        private void OnContinue(object sender, EventArgs e)
        {
            if (currentStep == 1)
            {
                bool valid = UIUtils.ValidateEntriesWithEmpty(new Entry[] { emailEntry, pswdEntry, firstNameEntry, lastNameEntry, phoneNumberEntry }, this.page);
                if (!valid)
                {
                    return;
                }
                BuildStep2();
                DataGate.VerifyPhoneNumber(phoneNumberEntry.Text, (res) =>
                {
                    if (res.Code == ResponseCode.OK)
                    {
                        currentCode = res.Result.Trim('"');
                        //TODO: temp
                        //Device.BeginInvokeOnMainThread(() =>
                        //{
                        //    codeEntry.Text = currentCode;
                        //});
                    }
                });
            }
            else
            {
                if (codeEntry.Text != currentCode)
                {
                    UIUtils.ShowMessage("Confirmation code is not valid", this.page);
                    return;
                }
                var sendData = new Dictionary<string, object>()
                {
                    { "Email", emailEntry.Text },
                    { "FbId", null },
                    { "FirstName", firstNameEntry.Text },
                    { "LastName", lastNameEntry.Text },
                    { "Password" , Ext.MD5.GetMd5String(pswdEntry.Text) },
                    { "Phone", phoneNumberEntry.Text }
                };
                spinner = UIUtils.ShowSpinner((ContentPage)this.page);
                DataGate.CustomerSignupJson(sendData, (data) =>
                {
                    var jobj = JObject.Parse(data.Result);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (data.Code == ResponseCode.OK && jobj["Id"] != null)
                        {
                            GlobalStorage.Settings.CustomerId = (string)jobj["Id"];
                            GlobalStorage.SaveAppSettings();
                        }
                        else
                        {
                            UIUtils.ShowMessage("Signup faled. Try later", this.page);
                        }
                        UIUtils.HideSpinner((ContentPage)this.page, spinner);
                        OnFinish?.Invoke(this, data);
                    });
                });
            }
        }
    }
}
