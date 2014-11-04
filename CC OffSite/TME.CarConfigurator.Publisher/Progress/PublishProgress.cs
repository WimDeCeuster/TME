using System;

namespace TME.CarConfigurator.Publisher.Progress
{
    public class PublishProgress
    {
        public string Message { get; private set; }

        public PublishProgress(string message)
        {
            Message = String.Format("{0} - {1}", DateTime.Now, message);
        }
    }
}