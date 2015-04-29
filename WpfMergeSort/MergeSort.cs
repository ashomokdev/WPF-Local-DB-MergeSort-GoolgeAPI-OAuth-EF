using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WpfMergeSort
{
    static public class MergeSort
    {

        private static void ParticularSorting(object obj)
        {
            Params p = (Params)obj;
            List<object[]> result = SortDataGrid(p.indexOfFirst, p.indexOfLast);
            queue.Enqueue(result);
        }

        private static List<object[]> SortDataGrid(int indexOfFirst, int indexOfLast)
        {
            //list of rows_values
            List<object[]> rows = new List<object[]>();

            for (int i = indexOfFirst; i < indexOfLast + 1; i++)
            {
                try
                {
                    DataRowView dvr = datagrid.Items[i] as DataRowView;
                    if (dvr == null)
                    {
                        //new item pleceholder here
                    }
                    else
                    {
                        rows.Add(dvr.Row.ItemArray);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + ex.StackTrace);
                }
            }

            rows.Sort((val1, val2) => ((IComparable)val1.GetValue(_columnIndex)).CompareTo(val2.GetValue(_columnIndex)));
            return rows;
        }
        private static void Merging(object obj)
        {
            try
            {
                var list = obj as List<List<object[]>>;
                List<object[]> merged = Merge(list[0], list[1]);
                //string idsOfLeft = "";
                //for (int i = 0; i < list[0].Count; i++)
                //{
                //    idsOfLeft += ((list[0])[i])[0] + " ,";
                //}

                //string idsOfRight = "";
                //for (int i = 0; i < list[1].Count; i++)
                //{
                //    idsOfRight += ((list[1])[i])[0] + " ,";
                //}
                //LogsWriter.Log(Thread.CurrentThread.ManagedThreadId.ToString(), DateTime.Now.ToString("MM/dd/yyyy h:mm tt"));
                //LogsWriter.Log(Thread.CurrentThread.ManagedThreadId.ToString(), "merged lists: " + idsOfLeft + "with" + idsOfRight + "\n" +
                //    "result count = " + merged.Count);
                queue.Enqueue(merged);
                //LogsWriter.Log(Thread.CurrentThread.ManagedThreadId.ToString(), " ResultList Count = " + merged.Count + "added to queue");
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + e.StackTrace);
            }
        } 

        public static List<object[]> Merge(List<object[]> left, List<object[]> right)
        {
            # region Logic description
            //        function merge(left,right)
            //var list result
            //while length(left) > 0 and length(right) > 0
            //    if first(left) ≤ first(right)
            //        append first(left) to result
            //        left = rest(left)
            //    else
            //        append first(right) to result
            //        right = rest(right)
            //    end if
            //if length(left) > 0 
            //    append left to result
            //if length(right) > 0 
            //    append right to result
            //return result
            # endregion
            try
            {
                List<object[]> result = new List<object[]>();
                while (left.Count > 0 && right.Count > 0)
                {
                    if (((IComparable)left[0].GetValue(_columnIndex)).CompareTo(right[0].GetValue(_columnIndex)) <= 0)
                    {
                        result.Add(left[0]);
                        left = left.Skip(1).Take(left.Count - 1).ToList();
                    }
                    else
                    {
                        result.Add(right[0]);
                        right = right.Skip(1).Take(right.Count - 1).ToList();
                    }
                }
                if (left.Count > 0)
                {
                    result = result.Concat(left).ToList();
                }
                if (right.Count > 0)
                {
                    result = result.Concat(right).ToList();
                }
                return result;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + e.StackTrace);
                throw;
            }
        }

        //zero-baser index of column for processing
        private static int _columnIndex = 0;

        private static ConcurrentQueue<List<object[]>> queue = new ConcurrentQueue<List<object[]>>();
        private static System.Windows.Controls.DataGrid datagrid;
        public static string Logs;

        struct Params
        {
            //zero-baser index of first row
            public  int indexOfFirst;
            //zero-baser index of last row
            public  int indexOfLast;
        }
       
        internal static void StartWork(int threadsCount, int columnIndex, System.Windows.Controls.DataGrid grdEmployee)
        {
            datagrid = grdEmployee;
            _columnIndex = columnIndex;
            //Get count of rows for redirection to thread
            int RowsForThread = (int)(grdEmployee.Items.Count / threadsCount);
            
            List<Params> soartingData = new List<Params>();
            for (int i = 0; i < threadsCount; i++)
            {
                Params p = new Params();
                p.indexOfFirst = i * RowsForThread;
                if (i == threadsCount - 1) //last part of table
                {
                    p.indexOfLast = grdEmployee.Items.Count - 1;
                }
                else
                {
                    p.indexOfLast = ((i + 1) * RowsForThread) - 1;
                }
                soartingData.Add(p);
            }

            
            Parallel.ForEach(soartingData, dataItem =>
                {
                    ParticularSorting(dataItem);
                });
            while (true)
            {
                

                //TODO implement semafore?
                List<object[]> left;
                List<object[]> right;
                bool leftObtained = queue.TryDequeue(out left);
                if (leftObtained)
                {
                    if (left.Count == grdEmployee.Items.Count - 1)
                    {
                        queue.Enqueue(left); //put left back
                        break;
                    }

                    bool rightObtained = queue.TryDequeue(out right);
                    if (rightObtained)
                    {
                        var p = new List<List<object[]>>();
                        p.Add(left);
                        p.Add(right);
                        Thread t = new Thread(Merging);
                        t.Start(p);
                    }
                    else
                    {
                        queue.Enqueue(left); //put left back
                    }
                }
            }

            //get result
            List<object[]> result;
            bool resultObtained = queue.TryDequeue(out result);
            if (resultObtained)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    DataRowView dvr = (DataRowView)grdEmployee.Items[i];
                    dvr.Row.ItemArray = result[i];
                }
            }
        }

        public static int ColumnIndex
        {
            get { return _columnIndex; }
        }
    }
}
