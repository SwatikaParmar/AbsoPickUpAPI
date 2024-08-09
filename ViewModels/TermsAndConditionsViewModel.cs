using System;
using System.Collections.Generic;

namespace AnfasAPI.ViewModels
{
    public class TermsAndConditionsViewModel
    {
        public string TermsAndConditionsContent { get; set; }

    }

    public class UpdateTermsAndConditionsViewModel
    {

        public int termsAndConditionsId { get; set; }
        public string TermsAndConditionsContent { get; set; }

    }

    public class TermsAndConditionsAdminViewModel
    {
        public int termsAndConditionsId { get; set; }
        public string TermsAndConditionsContent { get; set; }

    }

}
