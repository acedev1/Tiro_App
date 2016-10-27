using System;

namespace TiroApp
{
    public interface IFacebookHelper
    {
        void Start(Action<ResponseDataJson> callback);
    }
}
