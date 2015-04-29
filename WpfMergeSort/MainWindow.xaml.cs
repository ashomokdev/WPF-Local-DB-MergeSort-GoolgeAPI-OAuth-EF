using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data;
using System.ComponentModel;
using System.Diagnostics;
using FillDBFromGmail;

namespace WpfMergeSort
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FillDataGrid();
            //(grdEmployee.ItemsSource as DataView).Sort = "Name";
        }

        private void FillDataGrid()
        {
            try
            {
                var context = new MessageContext();
                var query = from c in context.Messages
                            select new { c.MessageId, c.Size, c.Snippet };
                var messages = query.ToList();
                DataTable dt = ConvertToDatatable(messages);
                grdEmployee.ItemsSource = dt.DefaultView;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
        }

        private void SortDataGrid(int threadsCount, int columnIndex)
        {
            MergeSort.StartWork(threadsCount, columnIndex, grdEmployee);
        }

        [STAThread]
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                int threadsCount;
                int columnIndex;
                bool parseThreadsCount = Int32.TryParse(tbThreadsCount.Text, out threadsCount);
                bool parseColumnIndex = Int32.TryParse(tbColumnIndex.Text, out columnIndex);

                if (parseThreadsCount && parseColumnIndex)
                {
                    var watch = Stopwatch.StartNew();
                    SortDataGrid(threadsCount, columnIndex);
                    watch.Stop();
                    var elapsedMs = watch.ElapsedMilliseconds;
                    Logs.Text += "Table size: " + grdEmployee.Items.Count;
                    Logs.Text += "Execution time: " + elapsedMs;
                    Logs.Text += MergeSort.Logs;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Check inputed values.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }

        private static DataTable ConvertToDatatable<T>(List<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    table.Columns.Add(prop.Name, prop.PropertyType.GetGenericArguments()[0]);
                else
                    table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}
