using Spark.Model;
using System.Collections.ObjectModel;

namespace Spark.Interface
{
    public interface IReceiveMessage
    {
        ObservableCollection<TextMessageDetailInfo> opreateInfo();
    }
}
