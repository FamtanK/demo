using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Utils
{
    public static class ListModifications
    {
        private const string AscendingOrder = "От А до Я";
        private const string DescendingOrder = "От Я до А";

        public static List<MainWindow.ProductListModel> Order(IEnumerable<MainWindow.ProductListModel> source, string order)
        {
            switch (order)
            {
                case AscendingOrder:
                    return source.OrderBy(x => x.ProductName).ToList();
                case DescendingOrder:
                    return source.OrderByDescending(x => x.ProductName).ToList();
            }

            return new List<MainWindow.ProductListModel>();
        }

        public static List<MainWindow.ProductListModel> FilterByType(IEnumerable<MainWindow.ProductListModel> source,
            string type)
        {
            return source
                .Where(x => x.ProductType == type)
                .ToList();
        }

        public static List<MainWindow.ProductListModel> Search(IEnumerable<MainWindow.ProductListModel> source, string phrase)
        {
            return source.Where(x => x.ProductName.ToLower().Contains(phrase.ToLower())).ToList();
        }
    }
}
