#pragma checksum "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5811707677681a05f901e8482c2eb086487f5db0"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_MainTable), @"mvc.1.0.view", @"/Views/Home/MainTable.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\_ViewImports.cshtml"
using WebApplication25;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\_ViewImports.cshtml"
using WebApplication25.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
using WebApplication25.ModelView;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
using System.Text;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"5811707677681a05f901e8482c2eb086487f5db0", @"/Views/Home/MainTable.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b55aa78452a949133b8eb62d95aa6084980a23b6", @"/Views/_ViewImports.cshtml")]
    public class Views_Home_MainTable : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<MainModelView>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", "search", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "MainTable", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-controller", "Home", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 9 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
Write(Html.ActionLink("Show main table", "MainTable", "Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 10 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
Write(Html.ActionLink("Show ip table", "IpTable", "Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n");
#nullable restore
#line 11 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
Write(Html.ActionLink("Show files table", "FilesTable", "Home"));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n   \r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5811707677681a05f901e8482c2eb086487f5db05610", async() => {
                WriteLiteral("\r\n    ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "5811707677681a05f901e8482c2eb086487f5db05872", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.InputTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper);
                __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.InputTypeName = (string)__tagHelperAttribute_0.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
#nullable restore
#line 15 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For = ModelExpressionProvider.CreateModelExpression(ViewData, __model => __model.Search);

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-for", __Microsoft_AspNetCore_Mvc_TagHelpers_InputTagHelper.For, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n    <input type =\"submit\" />\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n\r\n");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5811707677681a05f901e8482c2eb086487f5db08997", async() => {
                WriteLiteral("\r\n<select name=\"filters\" multiple> \r\n");
#nullable restore
#line 21 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
     foreach(var i in Model.Filters)
{
    if (i != null)
    {
        

#line default
#line hidden
#nullable disable
                WriteLiteral("            ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5811707677681a05f901e8482c2eb086487f5db09589", async() => {
#nullable restore
#line 26 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                          Write(i);

#line default
#line hidden
#nullable disable
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                BeginWriteTagHelperAttribute();
#nullable restore
#line 26 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
               WriteLiteral(i);

#line default
#line hidden
#nullable disable
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = __tagHelperStringValueBuffer;
                __tagHelperExecutionContext.AddTagHelperAttribute("value", __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n");
#nullable restore
#line 27 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
}
}

#line default
#line hidden
#nullable disable
                WriteLiteral("</select>\r\n<input type=\"submit\" />\r\n");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Controller = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral(@"

  <table class=""table table-striped"">
              <thead>
    <tr>
    
      <th scope=""col"">Id</th>
      <th scope=""col"">Date</th>
      <th scope=""col"">Time</th>
       <th scope=""col"">Request type</th>
      
        <th scope=""col"">Path</th>
        <th scope=""col"">Ip</th>
    
        <th scope=""col"">Company name</th>
         <th scope=""col"">Data volume</th>
         <th scope=""col"">Request result</th>
           <th scope=""col"">Datetime logged</th>
      
  
  
    </tr>
  </thead>
        
");
#nullable restore
#line 55 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
         foreach(var i in Model.MainTables)
        {
          

#line default
#line hidden
#nullable disable
            WriteLiteral("               <tr>\r\n                   \r\n                  <td>\r\n                      \r\n                      ");
#nullable restore
#line 62 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(i.Id);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n                  <td>\r\n                      ");
#nullable restore
#line 65 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(i.DateTime.Date.ToShortDateString());

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n                  <td>\r\n                      ");
#nullable restore
#line 68 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(i.DateTime.ToLongTimeString());

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n                  <td>\r\n                      ");
#nullable restore
#line 71 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(i.RequestType);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n               \r\n                  <td class =\"Path\">\r\n                      ");
#nullable restore
#line 75 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(i.FilesInfo?.Path);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n                  <td>\r\n                      ");
#nullable restore
#line 78 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(Encoding.ASCII.GetString(i._IPinfo.IPAddress));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n                  <td>\r\n                      ");
#nullable restore
#line 81 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(i._IPinfo?.CompanyName);

#line default
#line hidden
#nullable disable
            WriteLiteral(" ?? ");
#nullable restore
#line 81 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                                            Write(string.Empty);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n                  <td>\r\n                      ");
#nullable restore
#line 84 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(i.DataVolume);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n                  <td>\r\n                      ");
#nullable restore
#line 87 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(i.RequestResult);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n                     <td>\r\n                      ");
#nullable restore
#line 90 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"
                 Write(i.DateTimeLog);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                  </td>\r\n                \r\n               </tr>\r\n");
#nullable restore
#line 94 "C:\Users\user\source\repos\WebApplication25\WebApplication25\Views\Home\MainTable.cshtml"

           
                
        }

#line default
#line hidden
#nullable disable
            WriteLiteral("           </table>\r\n\r\n\r\n\r\n");
            DefineSection("Scripts", async() => {
                WriteLiteral(@"
    <script src=""~lib/jquery/dist/jquery.js""></script>
    <script>
        $('th').click(function(){
    var table = $(this).parents('table').eq(0)
    var rows = table.find('tr:gt(0)').toArray().sort(comparer($(this).index()))
    this.asc = !this.asc
    if (!this.asc){rows = rows.reverse()}
    for (var i = 0; i < rows.length; i++){table.append(rows[i])}
})
function comparer(index) {
    return function(a, b) {
        var valA = getCellValue(a, index), valB = getCellValue(b, index)
        return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.toString().localeCompare(valB)
    }
}
function getCellValue(row, index){ return $(row).children('td').eq(index).text() }
    </script>
");
            }
            );
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<MainModelView> Html { get; private set; }
    }
}
#pragma warning restore 1591
