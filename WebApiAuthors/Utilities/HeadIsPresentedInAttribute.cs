using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebApiAuthors.Utilities
{
    public class HeadIsPresentedInAttribute : Attribute, IActionConstraint
    {
        private readonly string head;
        private readonly string value;

        public HeadIsPresentedInAttribute(string head, string value)
        {
            this.head = head;
            this.value = value;
        }
        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var heads = context.RouteContext.HttpContext.Request.Headers;

            if (!heads.ContainsKey(head))
            {
                return false;
            }


            return string.Equals(heads[head], value, StringComparison.OrdinalIgnoreCase);
        }
    }
}
