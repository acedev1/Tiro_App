using ImageCircle.Forms.Plugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TiroApp.Model;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using XLabs.Platform.Services.Media;

namespace TiroApp.Pages
{
    public class EditInfoPage : ContentPage
    {
        private RelativeLayout root;
        private StackLayout main;
        private Entry firstName;
        private Entry lastName;
        private Entry phoneNumber;
        private Entry businessName;
        private Entry address;
        private Entry aboutMe;
        private Entry travelEntry;
        private Client _client;
        private MediaFile photoFile;
        private GeoCoordinate lastLocation;
        private Image profileImage;

        public EditInfoPage(Client client)
        {
            _client = client;
            Utils.SetupPage(this);
            BuildLayout();
        }

        private void BuildLayout()
        {
            main = new StackLayout();
            main.Spacing = 0;
            main.BackgroundColor = Color.White;

            var scrollableStack = new StackLayout();

            var header = UIUtils.MakeHeader(this, "You");
            main.Children.Add(header);
            main.Children.Add(UIUtils.MakeSeparator(true));

            profileImage = new CircleImage();
            profileImage.Source = _client is Customer ? _client.CustomerImage : ((MuaArtist)_client).ArtistImage;
            profileImage.HorizontalOptions = LayoutOptions.CenterAndExpand;
            profileImage.HeightRequest = 100;
            profileImage.Aspect = Aspect.AspectFill;
            profileImage.WidthRequest = profileImage.HeightRequest;
            profileImage.Margin = new Thickness(0, 10, 0, 10);
            profileImage.GestureRecognizers.Add(new TapGestureRecognizer(async v => {
                await TakeImage();
                if (profileImage != null)
                {
                    var source = ImageSource.FromStream(() => photoFile.Source);
                    profileImage.Source = null;
                    profileImage.Source = source;
                }
            }));
            main.Children.Add(profileImage);
            main.Children.Add(UIUtils.MakeSeparator(true));

            firstName = UIUtils.MakeEntry("First Name", UIUtils.FONT_SFUIDISPLAY_BOLD);
            firstName.Text = _client.FirstName;
            scrollableStack.Children.Add(firstName);
            scrollableStack.Children.Add(UIUtils.MakeSeparator());
            lastName = UIUtils.MakeEntry("Last Name", UIUtils.FONT_SFUIDISPLAY_BOLD);
            lastName.Text = _client.LastName;
            scrollableStack.Children.Add(lastName);
            scrollableStack.Children.Add(UIUtils.MakeSeparator());
            //phoneNumber = UIUtils.MakeEntry("Mobile Phone", UIUtils.FONT_SFUIDISPLAY_BOLD);
            //phoneNumber.Keyboard = Keyboard.Telephone;
            //phoneNumber.Text = _client.PhoneNumber;
            //main.Children.Add(phoneNumber);
            //main.Children.Add(UIUtils.MakeSeparator());

            if (_client is MuaArtist)
            {
                var artist = _client as MuaArtist;
                businessName = UIUtils.MakeEntry("Business Name", UIUtils.FONT_SFUIDISPLAY_BOLD);
                businessName.Text = artist.BusinessName;
                scrollableStack.Children.Add(businessName);
                scrollableStack.Children.Add(UIUtils.MakeSeparator());

                aboutMe = UIUtils.MakeEntry("About Me", UIUtils.FONT_SFUIDISPLAY_BOLD);
                aboutMe.Text = artist.AboutMe;
                scrollableStack.Children.Add(aboutMe);
                scrollableStack.Children.Add(UIUtils.MakeSeparator());

                var addressACEntry = new AutoCompleteView();
                address = addressACEntry.EntryText;
                address.Text = artist.Location.Address;
                address.FontFamily = UIUtils.FONT_SFUIDISPLAY_BOLD;
                address.HeightRequest = 45;
                address.FontSize = 17;
                address.BackgroundColor = Color.White;
                address.Margin = new Thickness(20, 10, 20, 0);
                address.Placeholder = "Location/Address";
                address.PlaceholderColor = Color.FromHex("787878");
                address.TextColor = Color.Black;
                scrollableStack.Children.Add(addressACEntry);
                scrollableStack.Children.Add(UIUtils.MakeSeparator());
                var sh = new PlaceSearchHelper(addressACEntry);
                sh.OnSelected += (o, p) => { lastLocation = p; };

                travelEntry = UIUtils.MakeEntry("Travel charge", UIUtils.FONT_SFUIDISPLAY_BOLD);
                travelEntry.Keyboard = Keyboard.Numeric;
                travelEntry.Text = artist.TravelCharge.ToString();
                scrollableStack.Children.Add(travelEntry);
            }

            var scrollView = new ScrollView {
                Content = scrollableStack
            };
            main.Children.Add(scrollView);

            var saveButton = UIUtils.MakeButton("SAVE", UIUtils.FONT_SFUIDISPLAY_MEDIUM);
            saveButton.VerticalOptions = LayoutOptions.EndAndExpand;
            saveButton.Clicked += OnSaveButtonClicked;
            main.Children.Add(saveButton);

            root = new RelativeLayout();
            root.Children.Add(main, Constraint.Constant(0), Constraint.Constant(0)
                , Constraint.RelativeToParent(p => p.Width)
                , Constraint.RelativeToParent(p => p.Height));

            Content = root;
        }

        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            var spinner = UIUtils.ShowSpinner(this);
            _client.FirstName = CheckInfoRow(_client.FirstName, firstName);
            _client.LastName = CheckInfoRow(_client.LastName, lastName);
            _client.PhoneNumber = CheckInfoRow(_client.PhoneNumber, phoneNumber);
            if (_client is MuaArtist)
            {
                (_client as MuaArtist).BusinessName = CheckInfoRow((_client as MuaArtist).BusinessName, businessName);
                (_client as MuaArtist).AboutMe = CheckInfoRow((_client as MuaArtist).AboutMe, aboutMe);
                int travelCharge = 0;
                if (int.TryParse(travelEntry.Text, out travelCharge))
                {
                    (_client as MuaArtist).TravelCharge = travelCharge;
                }
                CheckAddress();
                DataGate.MuaSetInfo((_client as MuaArtist), resp =>
                {
                    if (resp.Code == ResponseCode.OK && resp.Result == "true")
                    {
                        if (this.photoFile != null)
                        {
                            DataGate.UploadImage((_client is MuaArtist), this.photoFile.Source, (imgr) =>
                            {
                                UIUtils.HideSpinner(this, spinner);
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    UIUtils.ShowMessage("Info was edited successfully.", this);
                                    Navigation.PopAsync();
                                });
                            });
                        }
                        else
                        {
                            UIUtils.HideSpinner(this, spinner);
                            UIUtils.ShowMessage("Info was edited successfully.", this, () =>
                            {
                                Navigation.PopAsync();
                            });
                        }
                    }
                    else
                    {
                        UIUtils.HideSpinner(this, spinner);
                        UIUtils.ShowServerUnavailable(this);
                    }
                });
            }
            else
            {
                DataGate.CustomerSetInfo((_client as Customer), resp =>
                {
                    if (resp.Code == ResponseCode.OK && resp.Result == "true")
                    {
                        if (this.photoFile != null)
                        {
                            DataGate.UploadImage((_client is MuaArtist), this.photoFile.Source, (imgr) =>
                            {
                                UIUtils.HideSpinner(this, spinner);
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    UIUtils.ShowMessage("Info was edited successfully.", this);
                                    Navigation.PopAsync();
                                });
                            });
                        }
                        else
                        {
                            UIUtils.HideSpinner(this, spinner);
                            UIUtils.ShowMessage("Info was edited successfully.", this);
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PopAsync();
                            });
                        }
                    }
                    else
                    {
                        UIUtils.HideSpinner(this, spinner);
                        UIUtils.ShowServerUnavailable(this);
                    }
                });
            }
        }

        private async Task TakeImage()
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
            await taskMedia.ContinueWith((t, o) =>
            {
                if (t.IsCanceled || t.Result == null)
                {
                    return;
                }
                photoFile = t.Result;
            }, null);
        }
        private string CheckInfoRow(string text, Entry entry)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.Text) && !entry.Text.Equals(text))
            {
                return entry.Text;
            }
            return text;
        }
        private void CheckAddress()
        {
            var artist = _client as MuaArtist;
            if (!string.IsNullOrEmpty(address.Text) && !address.Text.Equals(artist.Location.Address) && lastLocation != null)
            {
                artist.Location.Address = address.Text;
                artist.Location.Lat = lastLocation.Latitude;
                artist.Location.Lon = lastLocation.Longitude;
            }
        }
    }
}
