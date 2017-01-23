using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Model;
using TiroApp.Views;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Platform.Services.Media;

namespace TiroApp.Pages.Mua
{
    public class MuaSignupPage : ContentPage
    {
        private int currentStep = 1;
        private string businessName;
        private GeoCoordinate lastLocation;
        private MediaFile photoFile;

        private StackLayout bottomLayout;
        private CustomLabel mainText;

        //private CustomEntry phoneEntry;
        private PhoneNumberEntry phoneEntry;
        private CustomEntry codeEntry;
        private CustomEntry addressEntry;
        private CustomEntry emailEntry;
        private CustomEntry pswdEntry;
        private CustomEntry pswdConfirmEntry;
        private CustomEntry fnameEntry;
        private CustomEntry lnameEntry;
        private CustomEntry instagramEntry;        
        private Editor aboutEntry;
        private ActivityIndicator spinner;
        private string currentCode;
        private bool isEntryFocused = false;
        private RelativeLayout main;
        private CustomEntry travelEntry;
        private ButtonGroup tierGroup;
        private ButtonGroup travelGroup;

        public MuaSignupPage(string businessName)
        {
            this.businessName = businessName;
            Utils.SetupPage(this);
            BuildLayout();
        }

        private void BuildLayout()
        {
            main = new RelativeLayout();
            var blackout = new BoxView();
            blackout.Color = Props.BlackoutColor;
            this.Content = main;

            mainText = new CustomLabel();
            mainText.Text = "get started by \r\n creating an account";
            mainText.TextColor = Color.White;
            mainText.FontSize = 28;
            mainText.FontFamily = UIUtils.FONT_BEBAS_REGULAR;
            mainText.HorizontalTextAlignment = TextAlignment.Center;
            mainText.HorizontalOptions = LayoutOptions.CenterAndExpand;

            var mainTextHolder = new StackLayout();
            mainTextHolder.Children.Add(mainText);

            var imageTop = new Image();
            imageTop.Source = ImageSource.FromResource("TiroApp.Images.w1.png");
            imageTop.Aspect = Aspect.AspectFill;
            main.Children.Add(imageTop, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((p) => { return p.Width; }),
                Constraint.RelativeToParent((p) => { return p.Width * 1.2; }));

            main.Children.Add(blackout, Constraint.Constant(0), Constraint.Constant(0),
                Constraint.RelativeToParent((p) => { return p.Width; }),
                Constraint.RelativeToParent((p) => { return p.Width * 1.2; }));

            var imageArrowBack = new Image();
            imageArrowBack.Source = ImageSource.FromResource("TiroApp.Images.ArrowBack.png");
            imageArrowBack.GestureRecognizers.Add(new TapGestureRecognizer(OnBack));
            main.Children.Add(imageArrowBack
                , Constraint.Constant(10)
                , Constraint.Constant(30)
                , Constraint.Constant(20)
                , Constraint.Constant(20));

            main.Children.Add(mainTextHolder, Constraint.Constant(0),
                Constraint.RelativeToParent((p) =>
                {
                    return p.Height - Utils.GetControlSize(bottomLayout).Height - Utils.GetControlSize(mainTextHolder).Height - 20;
                })
                , Constraint.RelativeToParent((p) => { return p.Width; }));

            bottomLayout = new StackLayout();
            main.Children.Add(bottomLayout, Constraint.Constant(0),
                Constraint.RelativeToParent((p) =>
                {
                    return isEntryFocused ? 0 : p.Height - Utils.GetControlSize(bottomLayout).Height;
                }), Constraint.RelativeToParent((p) => { return p.Width; }));

            BuildStep1();

            main.ForceLayout();
        }

        private void BuildStep1()
        {
            mainText.Text = "get started by \r\n creating an account";
            bottomLayout.Children.Clear();
            bottomLayout.Orientation = StackOrientation.Vertical;
            bottomLayout.Spacing = 0;
            bottomLayout.BackgroundColor = Color.White;

            fnameEntry = UIUtils.MakeEntry("First Name", UIUtils.FONT_SFUIDISPLAY_BOLD);
            bottomLayout.Children.Add(fnameEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            lnameEntry = UIUtils.MakeEntry("Last Name", UIUtils.FONT_SFUIDISPLAY_BOLD);
            bottomLayout.Children.Add(lnameEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());            

            var addressACEntry = new AutoCompleteView();
            addressEntry = addressACEntry.EntryText;
            addressEntry.FontFamily = UIUtils.FONT_SFUIDISPLAY_BOLD;
            addressEntry.HeightRequest = 45;
            addressEntry.FontSize = 17;
            addressEntry.BackgroundColor = Color.White;
            addressEntry.Margin = new Thickness(20, 10, 20, 0);
            addressEntry.Placeholder = "Location/Address";
            addressEntry.PlaceholderColor = Color.FromHex("787878");
            addressEntry.TextColor = Color.Black;
            bottomLayout.Children.Add(addressACEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());
            var sh = new PlaceSearchHelper(addressACEntry);
            sh.OnSelected += (o, p) => { lastLocation = p; };

            phoneEntry = new PhoneNumberEntry();
            bottomLayout.Children.Add(phoneEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            emailEntry = UIUtils.MakeEntry("Email", UIUtils.FONT_SFUIDISPLAY_BOLD);
            bottomLayout.Children.Add(emailEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            pswdEntry = UIUtils.MakeEntry("Password", UIUtils.FONT_SFUIDISPLAY_BOLD);
            pswdEntry.IsPassword = true;
            bottomLayout.Children.Add(pswdEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            pswdConfirmEntry = UIUtils.MakeEntry("Confirm Password", UIUtils.FONT_SFUIDISPLAY_BOLD);
            pswdConfirmEntry.IsPassword = true;
            bottomLayout.Children.Add(pswdConfirmEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            var continueButton = UIUtils.MakeButton("CONTINUE", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            continueButton.Clicked += OnContinue;
            continueButton.Margin = new Thickness(0, 40, 0, 0);
            bottomLayout.Children.Add(continueButton);

            bottomLayout.ForceLayout();
            currentStep = 1;
            MakeEntryVisibleWithKeybord(new Entry[] { phoneEntry.NumberEntry, addressEntry, emailEntry, pswdEntry, pswdConfirmEntry, lnameEntry, fnameEntry });
        }

        private void BuildStep2()
        {
            currentStep = 2;
            bottomLayout.Children.Clear();
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
            bottomLayout.Children.Add(label);

            var phoneNumberLabel = new CustomLabel() {
                Text = phoneEntry.PhoneNumber,
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20, 10, 20, 0),
                HeightRequest = 70
            };
            bottomLayout.Children.Add(phoneNumberLabel);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            codeEntry = UIUtils.MakeEntry("Confirmation code", UIUtils.FONT_SFUIDISPLAY_BOLD);
            bottomLayout.Children.Add(codeEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            var continueButton = UIUtils.MakeButton("CONTINUE", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            continueButton.Clicked += OnContinue;
            bottomLayout.Children.Add(continueButton);

            bottomLayout.ForceLayout();
            MakeEntryVisibleWithKeybord(new Entry[] { codeEntry });
        }

        private void BuildStep3()
        {
            currentStep = 3;
            mainText.Text = "Tell us a little bit more \r\n about your bisness";
            bottomLayout.Children.Clear();
            var label = new CustomLabel()
            {
                Text = "What is your typical price range",
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20)
            };
            bottomLayout.Children.Add(label);
            tierGroup = new ButtonGroup();
            tierGroup.Buttons = new List<string> { "5K - 10K", "10K - 25K", "25K+" };
            tierGroup.HorizontalOptions = LayoutOptions.Center;
            tierGroup.Margin = new Thickness(0, 10, 0, 20);
            bottomLayout.Children.Add(tierGroup);

            var label2 = new CustomLabel()
            {
                Text = "Are you willing to travel to the customer's location?",
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20)
            };
            bottomLayout.Children.Add(label2);
            travelGroup = new ButtonGroup();
            travelGroup.Buttons = new List<string> { "Yes", "No" };
            travelGroup.HorizontalOptions = LayoutOptions.Center;
            travelGroup.Margin = new Thickness(0, 10, 0, 20);
            bottomLayout.Children.Add(travelGroup);

            var label3 = new CustomLabel()
            {
                Text = "How much will you charge to travel?",
                TextColor = Color.Black,
                FontSize = 16,
                FontFamily = UIUtils.FONT_SFUIDISPLAY_REGULAR,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(20)
            };
            bottomLayout.Children.Add(label3);
            travelEntry = UIUtils.MakeEntry("Travel charge", UIUtils.FONT_SFUIDISPLAY_BOLD);
            travelEntry.Keyboard = Keyboard.Numeric;
            bottomLayout.Children.Add(travelEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            var continueButton = UIUtils.MakeButton("CONTINUE", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            continueButton.Clicked += OnContinue;
            bottomLayout.Children.Add(continueButton);

            bottomLayout.ForceLayout();
        }

        private void BuildStep4()
        {
            mainText.Text = "create a portfolio by \r\n linking to your instagram \r\n account";
            bottomLayout.Children.Clear();
            bottomLayout.Orientation = StackOrientation.Vertical;
            bottomLayout.Spacing = 0;
            bottomLayout.BackgroundColor = Color.White;

            instagramEntry = UIUtils.MakeEntry("Instagram account name", UIUtils.FONT_SFUIDISPLAY_BOLD);
            bottomLayout.Children.Add(instagramEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            var label = new CustomLabel()
            {
                FontFamily = UIUtils.FONT_SFUIDISPLAY_MEDIUM,
                HeightRequest = 45,
                FontSize = 17,
                Margin = new Thickness(20, 10, 20, 0),
                TextColor = Color.FromHex("787878"),
                Text = "Tell your customers a little about yourself"
            };
            bottomLayout.Children.Add(label);

            aboutEntry = new Editor();
            aboutEntry.HeightRequest = 90;
            aboutEntry.FontFamily = UIUtils.FONT_SFUIDISPLAY_BOLD;
            aboutEntry.FontSize = 17;
            aboutEntry.BackgroundColor = Color.White;
            aboutEntry.Margin = new Thickness(20, 10, 20, 0);
            aboutEntry.TextColor = Color.Black;
            bottomLayout.Children.Add(aboutEntry);
            bottomLayout.Children.Add(UIUtils.MakeSeparator());

            var photoButton = UIUtils.MakeButton("CHOOSE A PROFILE PICTURE", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            photoButton.BackgroundColor = Color.FromHex("352E4F");
            photoButton.Clicked += PhotoButton_Clicked;
            photoButton.Margin = new Thickness(0, 40, 0, 0);
            bottomLayout.Children.Add(photoButton);

            var continueButton = UIUtils.MakeButton("FINISH", UIUtils.FONT_SFUIDISPLAY_REGULAR);
            continueButton.Clicked += OnContinue;
            bottomLayout.Children.Add(continueButton);

            bottomLayout.ForceLayout();
            currentStep = 4;
            MakeEntryVisibleWithKeybord(new Entry[] { instagramEntry });
        }

        private async void PhotoButton_Clicked(object sender, EventArgs e)
        {
            var cameraOpts = new CameraMediaStorageOptions();
            cameraOpts.PercentQuality = 50;
            cameraOpts.MaxPixelDimension = 1200;

            var result = await UIUtils.ShowSelectList("Take a photo", "Choose from library", this);

            System.Threading.Tasks.Task<MediaFile> taskMedia = null;
            if (result == 1)
            {
                taskMedia = DependencyService.Get<IMediaPicker>().TakePhotoAsync(cameraOpts);
            }
            else if (result == 2)
            {
                taskMedia = DependencyService.Get<IMediaPicker>().SelectPhotoAsync(cameraOpts);
            }
            else
            {
                return;
            }
            taskMedia.ContinueWith((t, o) =>
            {
                if (t.IsCanceled || t.Result == null)
                {
                    return;
                }
                photoFile = t.Result;
            }, null);
        }

        private void OnContinue(object sender, EventArgs e)
        {
            if (currentStep == 1)
            {
                bool valid = UIUtils.ValidateEntriesWithEmpty(new Entry[] { phoneEntry.NumberEntry, addressEntry, emailEntry, pswdEntry, pswdConfirmEntry, lnameEntry, fnameEntry }, this);
                if (valid)
                {
                    if (pswdEntry.Text != pswdConfirmEntry.Text)
                    {
                        UIUtils.ShowMessage("Confirm password is not valid", this);
                        return;
                    }
                    BuildStep2();
                    DataGate.VerifyPhoneNumber(phoneEntry.PhoneNumber, (res) =>
                    {
                        if (res.Code == ResponseCode.OK)
                        {
                            currentCode = res.Result.Trim('"');
#if DEBUG
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                codeEntry.Text = currentCode;
                            });
#endif
                        }
                    });
                }
            }
            else if (currentStep == 2)
            {
                if (currentCode == codeEntry.Text)
                {
                    BuildStep3();
                }
                else
                {
                    UIUtils.ShowMessage("Confirmation code is not valid", this);
                }
            }
            else if (currentStep == 3)
            {
                BuildStep4();
            }
            else
            {
                if (string.IsNullOrEmpty(instagramEntry.Text))
                {
                    UIUtils.ShowMessage("Please enter instagram account", this);
                    return;
                }
                spinner = UIUtils.ShowSpinner(this);
                var instagramHelper = new InstagramHelper(instagramEntry.Text, this);
                instagramHelper.OnImagesLoad += OnInstagramImagesLoad;
                instagramHelper.Start();
            }
        }

        private void OnBack(View v)
        {
            if (currentStep == 1)
            {
                Navigation.PopAsync();
            }
            else
            {
                BuildStep1();
            }
        }

        private void OnInstagramImagesLoad(object sender, string ipictures)
        {
            var data = new Dictionary<string, object>();
            data.Add("AboutMe", aboutEntry.Text);
            data.Add("Address", addressEntry.Text);
            data.Add("Email", emailEntry.Text);
            data.Add("FirstName", fnameEntry.Text);
            data.Add("Instagram", instagramEntry.Text);
            data.Add("LastName", lnameEntry.Text);
            data.Add("BusinessName", businessName);
            if (lastLocation != null)
            {
                data.Add("LocationLat", lastLocation.Latitude.ToString(Utils.EnCulture));
                data.Add("LocationLon", lastLocation.Longitude.ToString(Utils.EnCulture));
            }
            data.Add("Password", Ext.MD5.GetMd5String(pswdEntry.Text));
            data.Add("Phone", phoneEntry.PhoneNumber);
            data.Add("Tier", tierGroup.SelectedIndex + 1);
            data.Add("IsTravelAvailable", travelGroup.SelectedIndex == 0);
            int travelCharge = 0;
            int.TryParse(travelEntry.Text, out travelCharge);
            data.Add("TravelCharge", travelCharge);
            if (ipictures != null)
            {
                data.Add("Pictures", ipictures);
            }
            else
            {
                UIUtils.ShowMessage("Can not load images from instagram", this);
            }

            DataGate.MuaSignupJson(data, (response) =>
            {
                if (response.Code == ResponseCode.OK && !string.IsNullOrEmpty(response.Result))
                {
                    Newtonsoft.Json.Linq.JObject jobj = null;
                    try
                    {
                        jobj = Newtonsoft.Json.Linq.JObject.Parse(response.Result);
                    }
                    catch { }
                    if (jobj != null && jobj["Id"] != null)
                    {
                        var muaId = (string)jobj["Id"];
                        //GlobalStorage.Settings.MuaId = muaId;
                        //GlobalStorage.SaveAppSettings();
                        //Notification.CrossPushNotificationListener.RegisterPushNotification();
                        if (this.photoFile != null)
                        {
                            DataGate.UploadImage(true, this.photoFile.Source, (imgr) =>
                            {
                                UIUtils.HideSpinner(this, spinner);
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Utils.ShowPageFirstInStack(this, new UnderReviewPage());
                                });
                            });
                        }
                        else
                        {
                            UIUtils.HideSpinner(this, spinner);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Utils.ShowPageFirstInStack(this, new UnderReviewPage());
                            });
                        }
                    }
                    else
                    {
                        UIUtils.HideSpinner(this, spinner);
                        UIUtils.ShowMessage("Signup failed. Try another email", this);
                    }
                }
                else
                {
                    UIUtils.HideSpinner(this, spinner);
                    UIUtils.ShowMessage("Login failed. Try later", this);
                }
            });
        }

        private void MakeEntryVisibleWithKeybord(IEnumerable<Entry> list)
        {
            foreach(var entry in list)
            {
                entry.Focused += OnEntryFocusChanged;
                entry.Unfocused += OnEntryFocusChanged;
            }
        }

        private void OnEntryFocusChanged(object sender, FocusEventArgs e)
        {
            isEntryFocused = e.IsFocused;
            main.ForceLayout();
            //if (e.IsFocused)
            //{
            //    var dy = this.Content.Height - bottomLayout.Height;
            //    bottomLayout.TranslateTo(0, -dy, 100);
            //}
            //else
            //{
            //    bottomLayout.TranslateTo(0, 0, 100);
            //}
        }
    }
}
