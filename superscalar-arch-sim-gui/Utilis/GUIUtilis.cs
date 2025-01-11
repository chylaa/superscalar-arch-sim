using superscalar_arch_sim.RV32.Hardware.Register;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.Utilis
{
    internal static class GUIUtilis
    {
        public static Color ColorBlend(Color color1, Color color2, double ratio)
        {
            int r = (int)Math.Round(color1.R * (1 - ratio) + color2.R * ratio);
            int g = (int)Math.Round(color1.G * (1 - ratio) + color2.G * ratio);
            int b = (int)Math.Round(color1.B * (1 - ratio) + color2.B * ratio);
            return Color.FromArgb(r, g, b);
        }
        public static void AddEnumRangeToComboBox<T>(ComboBox comboBox, T selected = default, Enum skip = null, bool preclear = true) where T : Enum
        {
            if (preclear) comboBox.Items.Clear();
            int i = 0; int selectedIndex = -1; 
            foreach (T e in Enum.GetValues(typeof(T))) {
                if (false == e.Equals(skip)) {
                    comboBox.Items.Add(e);
                    if (selected.Equals(e)) {
                        selectedIndex = i;
                    }
                }
                ++i;
            }
            comboBox.SelectedIndex = selectedIndex;
        }

        public static void SetEnableAllDirectChildren(Control parent, bool enabled, bool withparent = true)
        {
            if(withparent)
                parent.Enabled = enabled;
            
            foreach (Control child in parent.Controls)
                child.Enabled = enabled;
        }

        public static List<T> GetAllDirectChildrenOfType<T>(Control parent)
        {
            return parent.Controls.OfType<T>().ToList();
        }

        public static List<T> RecursivelyGetAllChildrenOfType<T>(Control entry) where T : Control
        {
            List<T> children = new List<T>();
            foreach (Control child in entry.Controls)
            {
                if (child is T typedChild) {
                    children.Add(typedChild);
                }
                children.AddRange(RecursivelyGetAllChildrenOfType<T>(entry:child));
            }
            return children;
        }

        private static List<T> GetControlsByBoundsAndType<T>(bool recursive, Func<Rectangle, Rectangle, bool> boundPredicate, Control container, Rectangle bounds) where T : Control
        {
            List<T> result = new List<T>();
            foreach (Control child in container.Controls)
            {
                if (child is T typedChild && boundPredicate.Invoke(bounds, child.Bounds))
                {
                    result.Add(typedChild);
                }
                if (recursive)
                {
                    result.AddRange(RecursivelyGetAllControlsWithinBounds<T>(child, bounds));
                }
            }
            return result;
        }
        /// <summary>
        /// Creates <see cref="List{T}"/> of <see cref="Control"/>s that are part of <paramref name="container"/> and 
        /// their <see cref="Control.Bounds"/> are entirely contained within <see cref="Rectangle"/> <paramref name="bounds"/>.
        /// </summary>
        public static IEnumerable<T> RecursivelyGetAllControlsWithinBounds<T>(Control container, Rectangle bounds) where T : Control
        {
            var predicate = new Func<Rectangle, Rectangle, bool>((Rectangle x, Rectangle y) => { return x.Contains(y); });
            return GetControlsByBoundsAndType<T>(true, predicate, container, bounds);
        }
        /// <summary>
        /// Creates <see cref="List{T}"/> of <see cref="Control"/>s that are direct children of <paramref name="container"/> <see cref="Control"/> 
        /// (in <see cref="Control.Controls"/>) and their <see cref="Control.Bounds"/> intersects with <see cref="Rectangle"/> <paramref name="bounds"/>.
        /// </summary>
        public static IEnumerable<T> GetAllDirectChildControlsIntersectingWithBounds<T>(Control container, Rectangle bounds) where T : Control
        {
            var predicate = new Func<Rectangle, Rectangle, bool>((Rectangle x, Rectangle y) => { return x.IntersectsWith(y); });
            return GetControlsByBoundsAndType<T>(false, predicate, container, bounds);
        }

        /// <summary>
        /// Invokes <see cref="Binding.ReadValue"/> on <paramref name="bindable"/>
        /// <see cref="IBindableComponent.DataBindings"/> at index <paramref name="bindingIndex"/>.
        /// Reads all bindings if <paramref name="bindingIndex"/> is negative.
        /// </summary>
        /// <param name="bindable">Source component emulating data-binding.</param>
        /// <param name="bindingIndex">Index of <see cref="Binding"/> to read, negative to read all <see cref="IBindableComponent.DataBindings"/>.</param>
        public static void ReadBinding(IBindableComponent bindable, int bindingIndex = 0)
        {
            if (bindable.DataBindings.Count > bindingIndex)
            {
                bindable.DataBindings[0].ReadValue();
            } 
            else if (bindingIndex < 0)
            {
                foreach (Binding binding in bindable.DataBindings)
                {
                    binding.ReadValue();
                }
            }
        }
        /// <summary>
        /// Invokes <see cref="Binding.ReadValue"/> on each of <paramref name="bindables"/>
        /// <see cref="IBindableComponent.DataBindings"/> at index <paramref name="bindingIndex"/>.
        /// Reads all bindings from single <see cref="IBindableComponent"/> if <paramref name="bindingIndex"/> is negative.
        /// </summary>
        /// <param name="bindables">Collectio of source components emulating data-binding.</param>
        /// <param name="bindingIndex">Index of <see cref="Binding"/> to read, negative to read all <see cref="IBindableComponent.DataBindings"/>.</param>
        public static void ReadBinding(IEnumerable<IBindableComponent> bindables, int bindingIndex = 0)
        {
            foreach (var bindable in bindables)
            {
                ReadBinding(bindable, bindingIndex);
            }
        }

        public static void ClearBindings(params IBindableComponent[] bindables)
        {
            foreach (var item in bindables)
            {
                item.DataBindings.Clear();
            }
        }

        #region DataGridView specific
        public static void RemoveColumnsFromDataGrid(DataGridView view, params string[] columnsToRemove)
        {
            foreach (var columnName in columnsToRemove)
            {
                view.Columns.Remove(columnName);
            }
        }
        public static void SetDataGridColumnsVisible(DataGridView view, bool visible, params string[] columnNames)
        {
            for (int i = 0; i < view.ColumnCount; i++)
            {
                if (columnNames.Contains(view.Columns[i].Name))
                {
                    view.Columns[i].Visible = visible;
                }
            }
        }
        /// <summary>
        /// Sets column which <see cref="DataGridViewColumn.Name"/> equals <paramref name="columnName"/> 
        /// <see cref="DataGridViewColumn.DisplayIndex"/> to <paramref name="index"/>. <br></br>
        /// Negative indexes are treated as indexing from the last column.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if found and indexed <see cref="DataGridViewColumn.Name"/> 
        /// equal to <paramref name="columnName"/>, <see langword="false"/> otherwise.
        /// </returns>
        public static bool SetDataGridColumnIndex(DataGridView view, int index, string columnName)
        {
            if (index < 0)  
            { 
                index = view.ColumnCount + index; 
            }

            for (int i = 0; i < view.ColumnCount; i++)
            {
                if (view.Columns[i].Name == columnName)
                {
                    view.Columns[i].DisplayIndex = index;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Sets <see cref="DataGridViewColumn.DisplayIndex"/> base on provided in <paramref name="columnNamesInOrder"/>
        /// (<see cref="DataGridViewColumn.Name"/> in <see cref="DataGridView.Columns"/>). 
        /// Throws <see cref="ArgumentException"/> when no column with matching header found 
        /// and <paramref name="throwOnMissing"/> parameter is set to <see langword="true"/>.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static void OrderColumnsInDataGrid(DataGridView view, bool throwOnMissing = true, params string[] columnNamesInOrder)
        {
            if (view == null || columnNamesInOrder == null)
            {
                throw new ArgumentNullException();
            }

            for (int i = 0; i < columnNamesInOrder.Length; i++)
            {
                DataGridViewColumn column = view.Columns[columnNamesInOrder[i]];
                if (column is null)
                {
                    for (int j = 0; j < view.Columns.Count; j++)
                    {
                        if (view.Columns[j].Name == columnNamesInOrder[i])
                            column = view.Columns[i];
                    }
                }

                if (column != null)
                {
                    column.DisplayIndex = i;
                } else if (throwOnMissing)
                {
                    throw new ArgumentException($"Column.Name == '{columnNamesInOrder[i]}' not found in passed DataGridView.");
                }
            }
        }
        /// <summary>
        /// <see cref="DataGridViewCellFormattingEventHandler"/> that uses <see cref="Register32.HexString"/> formatting
        /// on each formatted cell which <see cref="DataGridViewCell.ValueType"/> is <see cref="Register32"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void FormatRegister32OnDataGridViewCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (sender is DataGridView view)
            {
                var cell = view.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.ValueType == typeof(Register32) && e.Value != null)
                {
                    e.Value = ((Register32)e.Value).HexString;
                    e.FormattingApplied = true;
                }
            }
        }
        #endregion
    }
}
