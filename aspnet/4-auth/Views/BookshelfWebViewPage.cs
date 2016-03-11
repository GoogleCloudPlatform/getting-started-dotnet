using System.Web.Mvc;
using GoogleCloudSamples.Models;

namespace GoogleCloudSamples.Views
{
    public abstract class BookshelfWebViewPage<TModel> : WebViewPage<TModel>
    {
        public User CurrentUser => new User(this.User);
    }

    public abstract class BookshelfWebViewPage : WebViewPage { }
}