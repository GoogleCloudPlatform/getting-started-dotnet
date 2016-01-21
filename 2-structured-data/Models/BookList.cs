using System.Collections.Generic;

namespace GoogleCloudSamples.Models
{
    public class BookList
    {
        public IEnumerable<Book> Books;
        public string NextPageToken;
    }
}
