using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TiroApp.Views
{
    public class RatingLayout: StackLayout
    {
        private bool isEditable;

        public RatingLayout()
        {
            this.Orientation = StackOrientation.Horizontal;
            this.IsEditable = false;
            BuildLayout();
        }

        public bool IsEditable
        {
            get
            {
                return isEditable;
            }
            set
            {
                isEditable = value;
                BuildLayout();
            }
        }

        public object Rating
        {
            get
            {
                return GetValue(RatingProperty);
            }
            set
            {
                SetValue(RatingProperty, value);
            }
        }

        public static readonly BindableProperty RatingProperty =
            BindableProperty.Create<RatingLayout, object>(rl => rl.Rating, 0
                , BindingMode.Default, propertyChanged: OnRatingChanged );

        private static void OnRatingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var r = bindable as RatingLayout;
            r.BuildLayout();
        }

        private void BuildLayout()
        {
            this.Children.Clear();
            var rating = Convert.ToDouble(Rating);
            int count = 0;
            for (int i = 1; i <= rating; i++)
            {
                this.Children.Add(GetStar("starFill"));
                count++;
            }
            if (rating - count > 0)
            {
                this.Children.Add(GetStar("starHalfFill"));
                count++;
            }
            for (int i = count; i < Props.RatingMax; i++)
            {
                this.Children.Add(GetStar("star"));
            }
        }

        private Image GetStar(string name)
        {
            var star = new Image();
            star.SetValue(UIUtils.TagProperty, this.Children.Count + 1);
            star.Source = ImageSource.FromResource($"TiroApp.Images.{name}.png");
            star.HeightRequest = this.HeightRequest;
            star.WidthRequest = star.HeightRequest;
            if (isEditable)
            {
                star.GestureRecognizers.Add(new TapGestureRecognizer(OnImageClick));
            }
            return star;
        }

        private void OnImageClick(View image, object arg2)
        {
            int rating = (int)image.GetValue(UIUtils.TagProperty);
            Rating = rating;
        }
    }
}
