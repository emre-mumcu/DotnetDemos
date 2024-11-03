using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DemoAuthenticate.AppLib;

// https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/authoring?view=aspnetcore-8.0
// https://github.com/aspnet/Mvc/blob/release/2.2/src/Microsoft.AspNetCore.Mvc.TagHelpers/TagHelperOutputExtensions.cs#L166

[RestrictChildren("accordion-item")]
[HtmlTargetElement("accordion", TagStructure = TagStructure.NormalOrSelfClosing)]
public class AccordionTagHelper : TagHelper
{	
	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
        string? accordionId = output.Attributes
            .FirstOrDefault(attribute => attribute.Name == "id")
            ?.Value?.ToString()
            ??
            Guid.NewGuid().ToString()
        ;

        context.Items.Add("AccordionId", accordionId);

		output.TagName = "div";
		output.TagMode = TagMode.StartTagAndEndTag;
		output.AddClass("accordion", HtmlEncoder.Default);	
	}
}