using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.Language;

namespace App.Menu
{
    public class AdminSidebarService
    {
        private readonly IUrlHelper UrlHelper;
        public List<SideBarItem> Items { get; set; } = new List<SideBarItem>();

        public AdminSidebarService(IUrlHelperFactory factory, IActionContextAccessor action)
        {
            UrlHelper = factory.GetUrlHelper(action.ActionContext);

            Items.Add(new SideBarItem() { Type = SideBarItemType.Divider});
            Items.Add(new SideBarItem() { Type = SideBarItemType.Heading, Title = "Quản lý chung"});

            Items.Add(new SideBarItem() { 
                Type = SideBarItemType.NavItem,
                Title = "Quản lý User",
                AwesomeIcon = "fas fa-users",
                Controller = "User",
                Action = "Index", 
                Area = "Identity",
            });
            
            Items.Add(new SideBarItem() { 
                Type = SideBarItemType.NavItem,
                Title = "Quản lý Role",
                AwesomeIcon = "far fa-folder",
                Controller = "Role",
                Action = "Index", 
                Area = "Identity",
            });

            Items.Add(new SideBarItem() { 
                Type = SideBarItemType.NavItem,
                Title = "Quản Lý Database",
                AwesomeIcon = "fas fa-database",
                Controller = "DbManage",
                Action = "Index", 
                Area = "",
            });
        }

        public string RenderHtml()
        {
            var html = new StringBuilder();

            foreach (var item in Items)
            {
                html.Append(item.RenderHtml(UrlHelper));
            }

            return html.ToString();
        }

        public void SetActive(string Controller, string Action, string Area)
        {
            foreach (var item in Items)
            {
                if (item.Controller == Controller && item.Action == Action && item.Area == Area)
                {
                    item.IsActive = true; 
                    return;
                }
                else
                {
                    if (item.Items != null)
                    {
                        foreach (var childItem in item.Items)
                        {
                            if (childItem.Controller == Controller && childItem.Action == Action && childItem.Area == Area)
                            {
                                childItem.IsActive = true;
                                item.IsActive = true;
                                return;
                            }
                        }
                    }
                }
            }
        }

    }

}