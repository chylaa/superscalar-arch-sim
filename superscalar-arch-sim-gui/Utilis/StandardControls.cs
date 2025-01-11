using System.Drawing;
using System.Windows.Forms;

namespace superscalar_arch_sim_gui.Utilis
{
    public static partial class StandardControls
    {
        public static class StdDataGridView
        {

            public static void InitStdDataGridView( DataGridView dataGrid, 
                                                    DataGridViewBindingCompleteEventHandler onDataBindingComplete, 
                                                    bool allowToOrderColumns=true,
                                                    DataGridViewDataErrorEventHandler stdDataErrorHandler = null
                )
            {
                dataGrid.ReadOnly = true;
                dataGrid.AllowUserToOrderColumns = allowToOrderColumns;

                dataGrid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.ColumnHeadersVisible = true;
                dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGrid.AllowUserToResizeColumns = false;

                //dataGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
                dataGrid.RowHeadersVisible = false; // true;
                dataGrid.AllowUserToResizeRows = false;
                dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGrid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font(dataGrid.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);

                dataGrid.DefaultCellStyle.Font = new Font(dataGrid.DefaultCellStyle.Font.FontFamily, 8.0f, FontStyle.Regular);
                dataGrid.RowTemplate.ReadOnly = true;

                dataGrid.DataError += stdDataErrorHandler ?? StdDataGrid_IgnoreDataError;
                if (onDataBindingComplete != null)
                    dataGrid.DataBindingComplete += onDataBindingComplete;
            }

            public static void StdDataGrid_IgnoreDataError(object sender, DataGridViewDataErrorEventArgs e)
            {
                e.ThrowException = false;
                e.Cancel = true;
            }
            public static void StdDataGrid_ConsoleErrorWriteDataError(object sender, DataGridViewDataErrorEventArgs e)
            {
                e.ThrowException = false;
                e.Cancel = true;
                if (sender is DataGridView view)
                    System.Console.Error.WriteLine($"DataError: {view.Name}[{e.RowIndex},{e.ColumnIndex}] - {e.Exception.Message}");
            }

            public static void BindToDataGrid(DataGridView dataGrid, object dataSource, string propertyName, DataSourceUpdateMode updateMode)
            {
                dataGrid.DataBindings.DefaultDataSourceUpdateMode = updateMode;
                BindingSource source = new BindingSource(dataSource, propertyName);
                dataGrid.DataSource = source;
                
                UpdateBinding(dataGrid);
            }

            public static void UpdateBinding(DataGridView dataGrid)
            {
                dataGrid.Invalidate();
            }
        }
    }
}
