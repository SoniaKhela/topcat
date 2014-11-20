using System.Collections.Generic;

namespace Catalogue.Data.Roles
{
    public static class ResponsiblePartyRoles
    {
        // https://wiki.ceh.ac.uk/display/cehigh/Responsible+organisation

        public static List<string> Allowed = new List<string>
            {
                "resourceProvider",
                "custodian",
                "owner",
                "user",
                "distributor",
                "originator",
                "pointOfContact",
                "principalInvestigator",
                "processor",
                "publisher",
                "author",
            };
    }
}
