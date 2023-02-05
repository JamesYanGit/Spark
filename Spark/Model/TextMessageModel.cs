using System.Collections.ObjectModel;

namespace Spark.Model
{
    public class TextMessageModel
    {
        public string HashCode { get; set; }
        public string ReceiveStatus { get; set; }
        public ObservableCollection<TextMessageDetailInfo> MessageDetail { get; set; }
    }
}
