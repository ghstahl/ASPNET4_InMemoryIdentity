using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CustomClientCredentialHost.Areas.Admin.Controllers;
using IdentityServer3.Core.Models;

namespace CustomClientCredentialHost.Areas.Admin.Models
{
    public class ScopeNewModel: Scope
    {

        private readonly List<ScopeTypeElement> _ScopeTypeElements;

        public ScopeNewModel()
        {
            _ScopeTypeElements = new List<ScopeTypeElement>
            {
                new ScopeTypeElement() {Name = "Resource", ScopeType = ScopeType.Resource},
                new ScopeTypeElement() {Name = "Identity", ScopeType = ScopeType.Identity}
            };
        }
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Scope Type")]
        public ScopeType SelectedScopeType { get; set; }

        public IEnumerable<SelectListItem> ScopeTypes
        {
            get { return new SelectList(_ScopeTypeElements, "ScopeType", "Name"); }
        }
    }
}