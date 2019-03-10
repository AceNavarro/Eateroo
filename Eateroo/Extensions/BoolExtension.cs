using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eateroo.Extensions
{
    public static class BoolExtension
    {
        public static string ToDisplayYesOrNo(this bool value)
        {
            return value ? "Yes" : "No";
        }

        public static IEnumerable<SelectListItem> ToSelectListItemList(bool selectedValue)
        {
            return new SelectList(new []
            {
                new { Value = false, Text = "No" },
                new { Value = true, Text = "Yes" }
            }, "Value", "Text", selectedValue);
        }
    }
}
