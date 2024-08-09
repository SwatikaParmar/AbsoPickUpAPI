using AnfasAPI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.IServices
{
    public interface ITwilioManager
    {
        Task SendMessage(string body, string to);
        Task<TwilioVerificationResult> StartVerificationAsync(string phoneNumber, string channel);

        Task<TwilioVerificationResult> CheckVerificationAsync(string phoneNumber, string code);
    }
}
