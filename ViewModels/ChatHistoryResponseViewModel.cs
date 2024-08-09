using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.ViewModels
{
    public class ChatHistoryResponseViewModel
    {
        public string CreatedDate { get; set; }
        public string Text { get; set; }
        public string DocumentPath { get; set; }
        public string Type { get; set; }
        public string ImagePath { get; set; }
        public int MessageId { get; set; }
        public string ReceiverId { get; set; }
        //public string SenderId { get; set; }
        public commonData To { get; set; }
        public commonData From { get; set; }
    }

    public class commonData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
    }
}
