﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FineUI.Examples
{
    public partial class _default : PageBase
    {
        #region Page_Init

        private string menuType = "menu";
        private bool showOnlyNew = false;

        protected void Page_Init(object sender, EventArgs e)
        {
            HttpCookie menuCookie = Request.Cookies["MenuStyle_v4"];
            if (menuCookie != null)
            {
                menuType = menuCookie.Value;
            }

            // 从Cookie中读取是否仅显示最新示例
            HttpCookie menuShowOnlyNew = Request.Cookies["ShowOnlyNew_v4"];
            if (menuShowOnlyNew != null)
            {
                showOnlyNew = Convert.ToBoolean(menuShowOnlyNew.Value);
            }


            // 注册客户端脚本，服务器端控件ID和客户端ID的映射关系
            JObject ids = GetClientIDS(btnExpandAll, btnCollapseAll, windowSourceCode, mainTabStrip, leftRegion, menuSettings);

            if (menuType == "accordion")
            {
                Accordion accordionMenu = InitAccordionMenu();
                ids.Add("mainMenu", accordionMenu.ClientID);
                ids.Add("menuType", "accordion");
            }
            else
            {
                Tree treeMenu = InitTreeMenu();
                ids.Add("mainMenu", treeMenu.ClientID);
                ids.Add("menuType", "menu");
            }

            ids.Add("theme", PageManager.Instance.Theme.ToString());

            // 只在页面第一次加载时注册客户端用到的脚本
            if (!Page.IsPostBack)
            {
                string idsScriptStr = String.Format("window.IDS={0};", ids.ToString(Newtonsoft.Json.Formatting.None));
                PageContext.RegisterStartupScript(idsScriptStr);
            }
        }

        private Accordion InitAccordionMenu()
        {
            Accordion accordionMenu = new Accordion();
            accordionMenu.ID = "accordionMenu";
            accordionMenu.ShowBorder = false;
            accordionMenu.ShowHeader = false;
            leftRegion.Items.Add(accordionMenu);


            XmlDocument xmlDoc = XmlDataSource1.GetXmlDocument();
            XmlNodeList xmlNodes = xmlDoc.SelectNodes("/Tree/TreeNode");
            foreach (XmlNode xmlNode in xmlNodes)
            {
                if (xmlNode.HasChildNodes)
                {
                    AccordionPane accordionPane = new AccordionPane();
                    accordionPane.Title = xmlNode.Attributes["Text"].Value;
                    accordionPane.Layout = Layout.Fit;
                    accordionPane.ShowBorder = false;
                    accordionPane.BodyPadding = "2px 0 0 0";
                    accordionMenu.Items.Add(accordionPane);

                    Tree innerTree = new Tree();
                    innerTree.ShowBorder = false;
                    innerTree.ShowHeader = false;
                    innerTree.EnableIcons = true;
                    innerTree.AutoScroll = true;
                    innerTree.EnableSingleClickExpand = true;
                    accordionPane.Items.Add(innerTree);

                    XmlDocument innerXmlDoc = new XmlDocument();
                    innerXmlDoc.LoadXml(String.Format("<?xml version=\"1.0\" encoding=\"utf-8\" ?><Tree>{0}</Tree>", xmlNode.InnerXml));

                    // 绑定AccordionPane内部的树控件
                    innerTree.NodeDataBound += treeMenu_NodeDataBound;
                    innerTree.PreNodeDataBound += treeMenu_PreNodeDataBound;
                    innerTree.DataSource = innerXmlDoc;
                    innerTree.DataBind();

                    //// 重新设置每个节点的图标
                    //ResolveTreeNode(innerTree.Nodes);
                }
            }

            return accordionMenu;
        }

        private Tree InitTreeMenu()
        {
            Tree treeMenu = new Tree();
            treeMenu.ID = "treeMenu";
            treeMenu.ShowBorder = false;
            treeMenu.ShowHeader = false;
            treeMenu.EnableIcons = true;
            treeMenu.AutoScroll = true;
            treeMenu.EnableSingleClickExpand = true;
            leftRegion.Items.Add(treeMenu);

            // 绑定 XML 数据源到树控件
            treeMenu.NodeDataBound += treeMenu_NodeDataBound;
            treeMenu.PreNodeDataBound += treeMenu_PreNodeDataBound;
            treeMenu.DataSource = XmlDataSource1;
            treeMenu.DataBind();

            //// 重新设置每个节点的图标
            //ResolveTreeNode(treeMenu.Nodes);

            return treeMenu;
        }


        /// <summary>
        /// 树节点的绑定后事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeMenu_NodeDataBound(object sender, TreeNodeEventArgs e)
        {
            string isNewHtml = GetIsNewHtml(e.XmlNode);
            if (!String.IsNullOrEmpty(isNewHtml))
            {
                e.Node.Text += isNewHtml;
            }

            // 如果仅显示最新示例 并且 当前节点不是子节点，则展开当前节点
            if (showOnlyNew && !e.Node.Leaf)
            {
                e.Node.Expanded = true;
            }

        }


        /// <summary>
        /// 树节点的预绑定事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeMenu_PreNodeDataBound(object sender, TreePreNodeEventArgs e)
        {
            // 如果仅显示最新示例
            if (showOnlyNew)
            {
                string isNewHtml = GetIsNewHtml(e.XmlNode);
                if (String.IsNullOrEmpty(isNewHtml))
                {
                    e.Cancelled = true;
                }
            }
        }


        private string GetIsNewHtml(XmlNode node)
        {
            string result = String.Empty;

            XmlAttribute isNewAttr = node.Attributes["IsNew"];
            if (isNewAttr != null)
            {
                if (Convert.ToBoolean(isNewAttr.Value))
                {
                    result = "&nbsp;<span class=\"isnew\">New!</span>";
                }
            }

            return result;
        }



        private JObject GetClientIDS(params ControlBase[] ctrls)
        {
            JObject jo = new JObject();
            foreach (ControlBase ctrl in ctrls)
            {
                jo.Add(ctrl.ID, ctrl.ClientID);
            }

            return jo;
        }

        #endregion

        #region Page_Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitMenuStyleButton();
                InitLangMenuButton();
                InitThemeMenuButton();
                InitMenuShowOnlyNew();

                litVersion.Text = FineUI.GlobalConfig.ProductVersion;
                litOnlineUserCount.Text = Application["OnlineUserCount"].ToString();
            }
        }

        private void InitMenuShowOnlyNew()
        {
            cbxShowOnlyNew.Checked = showOnlyNew;

            if (showOnlyNew)
            {
                leftRegion.Title = "最新示例";
            }
        }


        private void InitMenuStyleButton()
        {
            string menuStyleID = "MenuStyleTree";

            HttpCookie menuStyleCookie = Request.Cookies["MenuStyle_v4"];
            if (menuStyleCookie != null)
            {
                switch (menuStyleCookie.Value)
                {
                    case "menu":
                        menuStyleID = "MenuStyleTree";
                        break;
                    case "accordion":
                        menuStyleID = "MenuStyleAccordion";
                        break;
                }
            }


            SetSelectedMenuID(MenuStyle, menuStyleID);
        }


        private void InitLangMenuButton()
        {
            string langMenuID = "MenuLangZHCN";

            string langValue = PageManager1.Language.ToString().ToLower();
            switch (langValue)
            {
                case "zh_cn":
                    langMenuID = "MenuLangZHCN";
                    break;
                case "zh_tw":
                    langMenuID = "MenuLangZHTW";
                    break;
                case "en":
                    langMenuID = "MenuLangEN";
                    break;
            }


            SetSelectedMenuID(MenuLang, langMenuID);
        }

        private void InitThemeMenuButton()
        {
            string themeMenuID = "MenuThemeBlue";

            string themeValue = PageManager1.Theme.ToString().ToLower();
            switch (themeValue)
            {
                case "blue":
                    themeMenuID = "MenuThemeBlue";
                    break;
                case "gray":
                    themeMenuID = "MenuThemeGray";
                    break;
                case "access":
                    themeMenuID = "MenuThemeAccess";
                    break;
                case "neptune":
                    themeMenuID = "MenuThemeNeptune";
                    break;
            }

            SetSelectedMenuID(MenuTheme, themeMenuID);
        }

        #endregion

        #region Event

        protected void MenuLang_CheckedChanged(object sender, CheckedEventArgs e)
        {
            // 单选框菜单按钮的CheckedChanged事件会触发两次，一次是取消选中的菜单项，另一次是选中的菜单项；
            // 不处理取消选中菜单项的事件，从而防止此函数重复执行两次
            if (!e.Checked)
            {
                return;
            }

            string langValue = FineUI.Language.ZH_CN.ToString();
            string langID = GetSelectedMenuID(MenuLang);

            switch (langID)
            {
                case "MenuLangZHCN":
                    langValue = FineUI.Language.ZH_CN.ToString();
                    break;
                case "MenuLangZHTW":
                    langValue = FineUI.Language.ZH_TW.ToString();
                    break;
                case "MenuLangEN":
                    langValue = FineUI.Language.EN.ToString();
                    break;
            }

            SaveToCookieAndRefresh("Language", langValue);
        }

        protected void MenuTheme_CheckedChanged(object sender, CheckedEventArgs e)
        {
            // 单选框菜单按钮的CheckedChanged事件会触发两次，一次是取消选中的菜单项，另一次是选中的菜单项；
            // 不处理取消选中菜单项的事件，从而防止此函数重复执行两次
            if (!e.Checked)
            {
                return;
            }

            string themeValue = FineUI.Theme.Neptune.ToString();
            string themeID = GetSelectedMenuID(MenuTheme);

            switch (themeID)
            {
                case "MenuThemeNeptune":
                    themeValue = FineUI.Theme.Neptune.ToString();
                    break;
                case "MenuThemeBlue":
                    themeValue = FineUI.Theme.Blue.ToString();
                    break;
                case "MenuThemeGray":
                    themeValue = FineUI.Theme.Gray.ToString();
                    break;
                case "MenuThemeAccess":
                    themeValue = FineUI.Theme.Access.ToString();
                    break;
            }

            SaveToCookieAndRefresh("Theme", themeValue);
        }

        protected void MenuStyle_CheckedChanged(object sender, CheckedEventArgs e)
        {
            // 单选框菜单按钮的CheckedChanged事件会触发两次，一次是取消选中的菜单项，另一次是选中的菜单项；
            // 不处理取消选中菜单项的事件，从而防止此函数重复执行两次
            if (!e.Checked)
            {
                return;
            }

            string menuValue = "menu";
            string menuStyleID = GetSelectedMenuID(MenuStyle);

            switch (menuStyleID)
            {
                case "MenuStyleTree":
                    menuValue = "tree";
                    break;
                case "MenuStyleAccordion":
                    menuValue = "accordion";
                    break;

            }
            SaveToCookieAndRefresh("MenuStyle", menuValue);
        }

        protected void cbxShowOnlyNew_CheckedChanged(object sender, CheckedEventArgs e)
        {
            SaveToCookieAndRefresh("ShowOnlyNew", e.Checked.ToString());
        }

        private string GetSelectedMenuID(MenuButton menuButton)
        {
            foreach (MenuItem item in menuButton.Menu.Items)
            {
                if (item is MenuCheckBox && (item as MenuCheckBox).Checked)
                {
                    return item.ID;
                }
            }
            return null;
        }

        private void SetSelectedMenuID(MenuButton menuButton, string selectedMenuID)
        {
            foreach (MenuItem item in menuButton.Menu.Items)
            {
                MenuCheckBox menu = (item as MenuCheckBox);
                if (menu != null && menu.ID == selectedMenuID)
                {
                    menu.Checked = true;
                }
                else
                {
                    menu.Checked = false;
                }
            }
        }


        private void SaveToCookieAndRefresh(string cookieName, string cookieValue)
        {
            HttpCookie cookie = new HttpCookie(cookieName + "_v4", cookieValue);
            cookie.Expires = DateTime.Now.AddYears(1);
            Response.Cookies.Add(cookie);

            PageContext.Refresh();
        }



        #endregion

        #region Tree

        ///// <summary>
        ///// 重新设置每个节点的图标
        ///// </summary>
        ///// <param name="nodes"></param>
        //private void ResolveTreeNode(TreeNodeCollection nodes)
        //{
        //    foreach (TreeNode node in nodes)
        //    {
        //        if (node.Nodes.Count == 0)
        //        {
        //            if (!String.IsNullOrEmpty(node.NavigateUrl))
        //            {
        //                node.IconUrl = GetIconForTreeNode(node.NavigateUrl);
        //            }
        //        }
        //        else
        //        {
        //            ResolveTreeNode(node.Nodes);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 根据链接地址返回相应的图标
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //private string GetIconForTreeNode(string url)
        //{
        //    url = url.ToLower();
        //    int paramsIndex = url.IndexOf("?");
        //    if (paramsIndex >= 0)
        //    {
        //        url = url.Substring(0, paramsIndex);
        //    }
        //    int lastDotIndex = url.LastIndexOf('.');
        //    if (lastDotIndex >= 0)
        //    {
        //        url = url.Substring(lastDotIndex + 1);
        //    }

        //    string fileType = url;

        //    string iconUrl = "~/res/images/filetype/vs_unknow.png";
        //    if (fileType == "txt")
        //    {
        //        iconUrl = "~/res/images/filetype/vs_txt.png";
        //    }
        //    else if (fileType == "aspx")
        //    {
        //        iconUrl = "~/res/images/filetype/vs_aspx.png";
        //    }
        //    else if (fileType == "htm" || fileType == "html")
        //    {
        //        iconUrl = "~/res/images/filetype/vs_htm.png";
        //    }

        //    return iconUrl;
        //}

        #endregion
    }
}
