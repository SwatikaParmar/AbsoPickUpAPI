﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnfasAPI.Helpers
{
    public class PaymentSettings
    {
        public string StripePublicKey { get; set; }
        public string StripePrivateKey { get; set; }
    }
}
