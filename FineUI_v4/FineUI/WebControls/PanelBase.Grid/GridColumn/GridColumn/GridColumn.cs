
#region Comment

/*
 * Project：    FineUI
 * 
 * FileName:    GridColumn.cs
 * CreatedOn:   2008-05-19
 * CreatedBy:   30372245@qq.com
 * 
 * 
 * Description：
 *      ->
 *   
 * History：
 *      ->
 * 
 * 
 * 
 * 
 */

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Globalization;
using System.Data;
using System.Reflection;
using System.Web.UI.WebControls;


namespace FineUI
{
    /// <summary>
    /// 表格列基类（抽象类）
    /// </summary>
    [ToolboxItem(false)]
    [ParseChildren(true)]
    [PersistChildren(false)]
    [DefaultProperty("HeaderText")]
    public abstract class GridColumn : ControlBase
    {
        #region Grid/ColumnIndex

        private Grid _grid;

        /// <summary>
        /// 表格对象
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("表格对象")]
        public Grid Grid
        {
            get
            {
                if (_grid == null)
                {
                    _grid = GetParentGrid();
                }
                return _grid;
            }
        }

        private Grid GetParentGrid()
        {
            if (Parent is Grid)
            {
                return (Grid)Parent;
            }
            else
            {
                return ResolveParentGrid(Parent as GridGroupColumn);
            }
        }

        private Grid ResolveParentGrid(GridGroupColumn groupColumn)
        {
            if (groupColumn != null)
            {
                if (groupColumn.Parent is Grid)
                {
                    return (Grid)groupColumn.Parent;
                }
                else
                {
                    return ResolveParentGrid(groupColumn.Parent as GridGroupColumn);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 列索引
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("列索引")]
        public int ColumnIndex
        {
            get
            {
                return Grid.AllColumns.IndexOf(this);
            }
        }

        #endregion

        #region SortField

        ///// <summary>
        ///// 当前列的排序表达式
        ///// </summary>
        //[Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public string SortExpression
        //{
        //    get
        //    {
        //        return String.Format("{0} {1}", SortField, SortDirection);
        //    }
        //}

        //public string _sortDirection = "ASC";

        ///// <summary>
        ///// 排序方向
        ///// </summary>
        //[Browsable(false)]
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public string SortDirection
        //{
        //    get
        //    {
        //        return _sortDirection;
        //    }
        //    set
        //    {
        //        _sortDirection = value;
        //    }
        //}

        private string _sortField = String.Empty;

        /// <summary>
        /// 排序字段
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue("")]
        [Description("排序字段")]
        public string SortField
        {
            get
            {
                return _sortField;
            }
            set
            {
                _sortField = value;
            }
        }

        #endregion

        #region Properties

        private bool _hidden = false;

        /// <summary>
        /// 是否隐藏列
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue(false)]
        [Description("是否隐藏列")]
        public override bool Hidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                _hidden = value;
            }
        }



        private string _columnID = String.Empty;

        /// <summary>
        /// 列ID（如果没有设置，则为ClientID）
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue("")]
        [Description("列ID（如果没有设置，则为ClientID）")]
        public string ColumnID
        {
            get
            {
                if (String.IsNullOrEmpty(_columnID))
                {
                    return ClientID;
                }
                return _columnID;
            }
            set
            {
                _columnID = value;
            }
        }


        private string _headerText = String.Empty;
        /// <summary>
        /// 标题栏显示的文字
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue("")]
        [Description("标题栏显示的文字")]
        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                _headerText = value;
            }
        }

        private string _headerToolTip = String.Empty;
        /// <summary>
        /// 标题栏文字的提示文本
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue("")]
        [Description("标题栏文字的提示文本")]
        public string HeaderToolTip
        {
            get
            {
                return _headerToolTip;
            }
            set
            {
                _headerToolTip = value;
            }
        }

        private ToolTipType _headerTooltipType = ToolTipType.Qtip;
        /// <summary>
        /// 标题栏文字的提示文本类型
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue(ToolTipType.Qtip)]
        [Description("标题栏文字的提示文本类型")]
        public ToolTipType HeaderToolTipType
        {
            get
            {
                return _headerTooltipType;
            }
            set
            {
                _headerTooltipType = value;
            }
        }


        private Unit _width = Unit.Empty;
        /// <summary>
        /// 列宽度
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue(typeof(Unit), "")]
        [Description("列宽度")]
        public virtual Unit Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }


        /// <summary>
        /// 控制子控件的尺寸（表格列使用HBox布局）
        /// </summary>
        [Category(CategoryName.LAYOUT)]
        [DefaultValue(0)]
        [Description("控制子控件的尺寸（表格列使用HBox布局）")]
        public int BoxFlex
        {
            get
            {
                object obj = XState["BoxFlex"];
                return obj == null ? 0 : (int)obj;
            }
            set
            {
                XState["BoxFlex"] = value;
            }
        }


        private bool _expandUnusedSpace = false;

        /// <summary>
        /// 此列会扩展所有未使用的宽度
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue(false)]
        [Description("此列会扩展所有未使用的宽度")]
        public bool ExpandUnusedSpace
        {
            get
            {
                return _expandUnusedSpace;
            }
            set
            {
                _expandUnusedSpace = value;
            }
        }


        private TextAlign _textalign = TextAlign.Left;

        /// <summary>
        /// 文本的排列位置
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue(TextAlign.Left)]
        [Description("文本的排列位置")]
        public TextAlign TextAlign
        {
            get
            {
                return _textalign;
            }
            set
            {
                _textalign = value;
            }
        }

        private bool _enableHeaderMenu = true;
        /// <summary>
        /// 启用表头菜单
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue(true)]
        [Description("启用表头菜单")]
        public virtual bool EnableHeaderMenu
        {
            get
            {
                return _enableHeaderMenu;
            }
            set
            {
                _enableHeaderMenu = value;
            }
        }


        private bool _enableColumnHide = true;
        /// <summary>
        /// 启用隐藏列功能
        /// </summary>
        [Category(CategoryName.OPTIONS)]
        [DefaultValue(true)]
        [Description("启用隐藏列功能")]
        public virtual bool EnableColumnHide
        {
            get
            {
                return _enableColumnHide;
            }
            set
            {
                _enableColumnHide = value;
            }
        }


        /// <summary>
        /// 表格单元格容器样式类
        /// </summary>
        internal virtual string InnerCls
        {
            get
            {
                return String.Empty;
            }
        }


        #endregion

        #region virtual GetColumnValue/GetColumnState/PersistState

        /// <summary>
        /// 取得列头渲染后的HTML
        /// </summary>
        /// <returns>渲染后的HTML</returns>
        internal virtual string GetHeaderValue()
        {
            return String.IsNullOrEmpty(HeaderText) ? "&nbsp;" : HeaderText;
        }

        /// <summary>
        /// 取得列渲染后的HTML
        /// </summary>
        /// <param name="row">表格行实例</param>
        /// <returns>渲染后的HTML</returns>
        internal virtual string GetColumnValue(GridRow row)
        {
            return String.Empty;
        }


        /// <summary>
        /// 本列是否需要保存状态（目前只有CheckBoxField实现了这个定义）
        /// </summary>
        internal virtual bool PersistState
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 获取列的状态
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        internal virtual object GetColumnState(GridRow row)
        {
            return null;
        }

        #endregion

        #region OnPreRender

        /// <summary>
        /// 渲染 HTML 之前调用（AJAX回发）
        /// </summary>
        protected override void OnAjaxPreRender()
        {
            // 表格列控件不监视列属性的改变
        }

        /// <summary>
        /// 渲染 HTML 之前调用（页面第一次加载或者普通回发）
        /// </summary>
        protected override void OnFirstPreRender()
        {
            base.OnFirstPreRender();

            
            if (this is TemplateField && (this as TemplateField).RenderAsRowExpander)
            {
                // 行扩展列需要特殊处理
            }
            else
            {
                JsObjectBuilder columnBuilder = new JsObjectBuilder();

                if (this is RowNumberField)
                {
                    OB.AddProperty("xtype", "rownumberer");
                }

                OB.AddProperty("text", GetHeaderValue());

                if (!String.IsNullOrEmpty(HeaderToolTip))
                {
                    OB.AddProperty("tooltip", HeaderToolTip);
                    OB.AddProperty("tooltipType", ToolTipTypeName.GetName(HeaderToolTipType));
                }

                if (Hidden)
                {
                    OB.AddProperty("hidden", true);
                }

                //if (!String.IsNullOrEmpty(InnerCls))
                //{
                //    OB.AddProperty("innerCls", InnerCls);
                //}

                if (Grid.AllowSorting)
                {
                    if (String.IsNullOrEmpty(SortField))
                    {
                        OB.AddProperty("sortable", false);
                    }
                    else
                    {
                        OB.AddProperty("sortable", true);
                    }
                }
                else
                {
                    OB.AddProperty("sortable", false);
                }


                if (PersistState)
                {
                    OB.AddProperty("x_persistState", true);
                    OB.AddProperty("x_persistStateType", "checkbox");
                }



                //If not specified, the column's index is used as an index into the Record's data Array.
                OB.AddProperty("dataIndex", ColumnID);
                OB.AddProperty("id", ColumnID);

                if (TextAlign != TextAlign.Left)
                {
                    OB.AddProperty("align", TextAlignName.GetName(TextAlign));
                }

                if (Width != Unit.Empty)
                {
                    OB.AddProperty("width", Width.Value);
                }
                else if (BoxFlex != 0)
                {
                    OB.AddProperty("flex", BoxFlex);
                }

                if (ExpandUnusedSpace)
                {
                    OB.AddProperty("flex", 1);
                }

                if (Grid.EnableHeaderMenu)
                {
                    if (EnableHeaderMenu)
                    {
                        OB.AddProperty("menuDisabled", false);
                    }
                    else
                    {
                        OB.AddProperty("menuDisabled", true);
                    }
                }
                else
                {
                    OB.AddProperty("menuDisabled", true);
                }

                if (EnableColumnHide)
                {
                    OB.AddProperty("hideable", true);
                }
                else
                {
                    OB.AddProperty("hideable", false);
                }
            }


        }

        #endregion



    }
}



