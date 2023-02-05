using Spark.Model;

namespace Spark.Interface
{
    public interface ISendMessage
    {
        void opreateInfo();
        void sendBackMsg(TextMessageModel newText);
        void reSendMsg();
    }
}
