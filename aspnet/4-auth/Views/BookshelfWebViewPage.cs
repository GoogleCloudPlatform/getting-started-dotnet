using System.Web.Mvc;
using GoogleCloudSamples.Models;

namespace GoogleCloudSamples.Views
{
    // [START custom_web_view]
    public abstract class BookshelfWebViewPage<TModel> : WebViewPage<TModel>
    {
        public User CurrentUser => new User(this.User);
    }
    // [END custom_web_view]

    public abstract class BookshelfWebViewPage : WebViewPage { }
}
