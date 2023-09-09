using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace App.Menu
{
    public enum SideBarItemType
    {
        Divider,
        Heading,
        NavItem
    }
    public class SideBarItem
    {
        public string Title { get; set; }
        public bool IsActive { get; set; }

        public SideBarItemType Type { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }
        public string Area { get; set; }
        public string AwesomeIcon { get; set; }

        public List<SideBarItem> Items { get; set; }

        public string collapseID { get; set;}

        public string GetLink(IUrlHelper urlHelper)
        {
            return urlHelper.Action(Action, Controller, new { area = Area});
        }

        public string RenderHtml(IUrlHelper urlHelper)
        {
            var html = new StringBuilder();

            if (Type == SideBarItemType.Divider)
            {
                html.Append("<hr class=\"sidebar-divider my-2\">");
            }
            else if(Type == SideBarItemType.Heading)
            {
                html.Append(@$"
                    <div class=""sidebar-heading"">
                        {Title}
                    </div>
                ");
            }
            else if(Type == SideBarItemType.NavItem)
            {
                if (Items == null)
                {
                    var url = GetLink(urlHelper);
                    var icon = (AwesomeIcon == null) ? "" : $"<i class=\"{AwesomeIcon}\"></i>";

                    var cssClass = "nav-item";
                    if (IsActive) cssClass += " active";

                    html.Append(@$"
                        <li class=""{cssClass}"">
                            <a class=""nav-link"" href=""{url}"">
                                {icon}
                                <span>{Title}</span>
                            </a>
                        </li>
                    ");
                }
                else
                {
                    var cssClass = "nav-item";
                    if (IsActive) cssClass += " active";

                    var collapseCss = "collapse";
                    if (IsActive) collapseCss += " show";

                    var icon = (AwesomeIcon == null) ? "" : $"<i class=\"{AwesomeIcon}\"></i>";

                    var ItemMenu = "";

                    foreach (var item in Items)
                    {
                        var urlItem = item.GetLink(urlHelper);
                        var cssItem = "collapse-item";
                        if (item.IsActive) cssItem += " active";
                        ItemMenu += $"<a class=\"{cssItem}\" href=\"{urlItem}\">{item.Title}</a>";
                    }

                    html.Append(@$"
                        <li class=""{cssClass}"">
                            <a class=""nav-link collapsed"" href=""#"" data-toggle=""collapse"" data-target=""#{collapseID}""
                                aria-expanded=""true"" aria-controls=""{collapseID}"">
                                    {icon}
                                <span>{Title}</span>
                            </a>
                            <div id=""{collapseID}"" class=""{collapseCss}"" aria-labelledby=""headingTwo"" data-parent=""#accordionSidebar"">
                                <div class=""bg-white py-2 collapse-inner rounded"">
                                    {ItemMenu}
                                </div>
                            </div>
                        </li>
                    ");
                }
            }

            return html.ToString();
        }

    }
}