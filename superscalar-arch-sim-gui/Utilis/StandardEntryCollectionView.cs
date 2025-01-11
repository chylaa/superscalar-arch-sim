using superscalar_arch_sim.RV32.Hardware.Pipeline;
using superscalar_arch_sim_gui.UserControls;
using superscalar_arch_sim_gui.UserControls.CustomControls;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.Utilis
{
    public static partial class StandardControls
    {
        public class StandardEntryCollectionView<TCollection, TEntry> : UserControl, IBindUpdate 
            where TCollection:IInstructionEntryCollection<TEntry> 
            where TEntry:IUniqueInstructionEntry
        {
            private const System.Reflection.BindingFlags UpdateBindingPropertyAccessFlags
                = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase;

            [Description("Names of columns to hide when adjusting visible columns on binding complete.")]
            public System.Collections.Generic.List<string> HideColumns { get; } 
            public Color BackColorOnMarkedEmpty { get; set; } = ControlPaint.LightLight(Color.IndianRed);
            public TCollection BindedCollection { get; set; }

            public Color SpecialRowColor { get; set; } = Color.Transparent;
            public int SpecialColorRowIndex { get; set; } = -1;

            protected ContextMenuDisplayStyleSelection CustomContextMenu { get; }
            protected ToolTip InstructionIndexTooltip { get; } 

            /// <summary>Names of columns to format using selected fromatting from <see cref="CustomContextMenu"/>.</summary>
            [Description("Names of columns to format using selected fromatting from CustomContextMenu.")]
            protected string[] StylingColumns { get; set; }
            protected DataGridViewColumn MarkedEmptyInvisibleColumn { get; set; }
            protected DataGridView BaseDataGridView { get; set; }

            public StandardEntryCollectionView() 
            {
                ContextMenuStrip = (CustomContextMenu = new ContextMenuDisplayStyleSelection());
                CustomContextMenu.OnCheckedStyleChanged += delegate { Refresh(); };
                InstructionIndexTooltip = new ToolTip();
                HideColumns = new System.Collections.Generic.List<string>() { nameof(IUniqueInstructionEntry.InstructionIndex) };
            }

            private void BaseDataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
            {
                if (e.Clicks == 1 && false == string.IsNullOrEmpty(InstructionIndexTooltip.GetToolTip(this)))
                {
                    InstructionIndexTooltip.SetToolTip(this, string.Empty);
                    InstructionIndexTooltip.Hide(this);
                }
            }
            private void BaseDataGridView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
            {
                if (BaseDataGridView != null && BindedCollection != null && e.RowIndex >= 0 && BaseDataGridView.Rows.Count > e.RowIndex)
                {
                    if (BaseDataGridView.Rows[e.RowIndex].Visible && BindedCollection.Count > e.RowIndex)
                    {
                        if (string.IsNullOrEmpty(InstructionIndexTooltip.GetToolTip(this)))
                        {
                            var entry = BindedCollection.ElementAt(e.RowIndex);
                            string iidxstr = $"Entry {entry.Tag}: II={entry.InstructionIndex} " + (entry.MarkedEmpty ? string.Empty : $"| ({entry.IR32})");
                            Point point = PointToClient(Cursor.Position); point.Offset(10, 10);
                            InstructionIndexTooltip.Show(iidxstr, this, point, 5000);
                        }
                        else
                        {
                            InstructionIndexTooltip.SetToolTip(this, string.Empty);
                            InstructionIndexTooltip.Hide(this);
                        }
                    }
                }
            }
            public void InitializeBaseDataGrid(DataGridView view)
            {
                BaseDataGridView = view;
                StdDataGridView.InitStdDataGridView(BaseDataGridView, AdjustVisibleColumns);
                BaseDataGridView.CellFormatting += GUIUtilis.FormatRegister32OnDataGridViewCellFormatting;
                BaseDataGridView.CellFormatting += DataGridView_CellFormatting;
                BaseDataGridView.CellMouseDoubleClick += BaseDataGridView_CellMouseDoubleClick;
                BaseDataGridView.CellMouseClick += BaseDataGridView_CellMouseClick;
                SpecialRowColor = BaseDataGridView.DefaultCellStyle.BackColor;
            }
            protected void SetRowBackcolor(DataGridViewRow row, Color backcolor)
            {
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    row.Cells[i].Style.BackColor = backcolor;
                }
            }
            protected virtual void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
            {
                var column = BaseDataGridView.Columns[e.ColumnIndex];
                
                if (column.Index == 0 && e.RowIndex < BaseDataGridView.RowCount) // only do this once
                {
                    var row = BaseDataGridView.Rows[e.RowIndex];
                    bool empty = Convert.ToBoolean(BaseDataGridView[MarkedEmptyInvisibleColumn.Index, e.RowIndex].Value);
                    
                    Color bcolor = (e.RowIndex == SpecialColorRowIndex) 
                        ? SpecialRowColor 
                        : (empty) 
                        ? BackColorOnMarkedEmpty 
                        : BaseDataGridView.DefaultCellStyle.BackColor;
                    
                    SetRowBackcolor(row, bcolor);
                } 
                else if (StylingColumns.Contains(column.DataPropertyName)) // Columns to format to selected style
                {
                    if (e.Value != null)
                    {
                        StrConverter.StringStyle selectedFormat = CustomContextMenu.ValueFormat;
                        e.Value = StrConverter.FormatValue(selectedFormat, unchecked((uint)((int)e.Value)), hexsize: -1);
                        e.FormattingApplied = true;
                    }
                }

            }
            /// <summary>
            /// For <paramref name="sender"/> being <see cref="BaseDataGridView"/>:
            /// <br></br>- Sets <see cref="DataGridViewColumn.Visible"/> in <see cref="BaseDataGridView"/> base on <see cref="HideColumns"/> property. 
            /// <br></br>- Sets <see cref="MarkedEmptyInvisibleColumn"/> property base on <see cref="IUniqueInstructionEntry.MarkedEmpty"/> name.
            /// <br></br>Note: Should be called once as callback for <see cref="DataGridView.DataBindingComplete"/> <see langword="event"/>
            /// </summary>
            protected virtual void AdjustVisibleColumns(object sender, EventArgs _)
            {
                if (sender == BaseDataGridView)
                {
                    string[] hideEmpty = new string[] { nameof(IUniqueInstructionEntry.MarkedEmpty) };
                    string[] hide = HideColumns is null ? hideEmpty : HideColumns.Concat(hideEmpty).ToArray();
                    GUIUtilis.SetDataGridColumnsVisible(BaseDataGridView, visible: false, hide);
                    
                    for (int i = (BaseDataGridView.ColumnCount - 1); i >= 0; i--)
                    {
                        if (BaseDataGridView.Columns[i].Name == nameof(IUniqueInstructionEntry.MarkedEmpty))
                        {
                            MarkedEmptyInvisibleColumn = BaseDataGridView.Columns[i];
                            break;
                        }
                    }
                }
            }

            public virtual void BindEntryCollection(TCollection dataSource)
            {
                BaseDataGridView.AutoGenerateColumns = true;
                BindedCollection = dataSource;
                StdDataGridView.BindToDataGrid(BaseDataGridView, this, nameof(BindedCollection), DataSourceUpdateMode.Never);
                BaseDataGridView.DataBindings.Clear();
                BaseDataGridView.AutoGenerateColumns = false;
                BaseDataGridView.DataSource = null;

                AdjustVisibleColumns(BaseDataGridView, EventArgs.Empty);
                AddFirstRow();
                UpdateFillRowsFromCollection(BindedCollection);
            }

            private void AddFirstRow()
            {
                if (BaseDataGridView.RowCount == 0)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(BaseDataGridView);
                    BaseDataGridView.Rows.Add(row);
                }
            }
            private void FillOrAddRow(IUniqueInstructionEntry entry, int? rowIdx)
            {
                if (rowIdx is null)
                {
                    rowIdx = BaseDataGridView.Rows.AddCopy(0);
                }
                DataGridViewRow row = BaseDataGridView.Rows[rowIdx.Value];
                for (int i = 0; i < BaseDataGridView.ColumnCount; i++)
                {
                    var column = BaseDataGridView.Columns[i];
                    if (column.Visible || (MarkedEmptyInvisibleColumn == column))
                    {
                        var name = column.DataPropertyName ?? column.Name;
                        var bindingFlag = UpdateBindingPropertyAccessFlags;
                        var property = entry.GetType().GetProperty(name, bindingFlag);
                        row.Cells[i].Value = property.GetValue(entry, null);
                    }
                }
            }
            public void UpdateFillRowsFromCollection(TCollection collection)
            {
                int? rowIdx = BaseDataGridView.RowCount;
                while(collection.Count() < BaseDataGridView.RowCount)
                {
                    BaseDataGridView.Rows.RemoveAt((int)(--rowIdx));
                }
                rowIdx = 0;
                foreach (IUniqueInstructionEntry entry in collection)
                {
                    FillOrAddRow(entry, (rowIdx < BaseDataGridView.RowCount) ? rowIdx : null);
                    ++rowIdx;
                }
            }

            public virtual void UpdateBindings()
            {
                if (BindedCollection != null)
                {
                    UpdateFillRowsFromCollection(BindedCollection);
                }
                //StdDataGridView.UpdateBinding(BaseDataGridView);
            }

        }
    }
}
