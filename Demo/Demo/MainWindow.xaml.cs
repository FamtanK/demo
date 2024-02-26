using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class ProductListModel
        {
            public int Id { get; set; }
            public string ProductType { get; set; }
            public string ProductName { get; set; }
            public decimal Price { get; set; }
            public string Articul { get; set; }
            public string Materials { get; set; }
            public byte[] Image { get; set; }
        }

        private const string DefaultOrder = "По умолчанию";
        private const string AscendingOrder = "От А до Я";
        private const string DescendingOrder = "От Я до А";
        private const string UniversalFilter = "Все";

        private const string FilterKey = "filter";
        private const string SearchKey = "search";
        private const string SortKey = "sort";

        private const string SearchPlaceholder = "Введите для поиска";

        private List<ProductListModel> _products = new List<ProductListModel>();
        private List<ProductListModel> _paginatedProducts = new List<ProductListModel>();

        private Dictionary<string, Action> _listModifications = new Dictionary<string, Action>();

        public MainWindow()
        {
            InitializeComponent();

            Init();

            // CbFilter.ItemsSource = GlobalContext.MaterialTypeRepo.GetAll().Select(x => x.Name).ToList();
            SortComboBox.ItemsSource = new[]
            {
                DefaultOrder,
                AscendingOrder,
                DescendingOrder
            };

            SortComboBox.SelectedItem = DefaultOrder;

            var types = App.connection.ProductType.ToList();
            List<string> filters = new List<string>();
            filters.Add(UniversalFilter);
            foreach (var type in types)
            {
                filters.Add(type.Name);
            }
            FilterComboBox.ItemsSource = filters;

            FilterComboBox.SelectedItem = UniversalFilter;

            LvProducts.ItemsSource = _paginatedProducts;
        }

        private void Init()
        {
            var products = App.connection.Product.ToList();
            foreach (var product in products)
            {
                ProductListModel model = new ProductListModel();
                model.Id = product.Id;
                model.ProductType = product.ProductType.Name;
                model.ProductName = product.Name;
                model.Price = product.MinimalCost;
                model.Articul = product.Articul;
                model.Image = product.Image;
                model.Materials = "";
                
                foreach (var material in product.ProductMaterial)
                {
                    model.Materials += App.connection.Material.Where(x => x.Id == material.MaterialId).FirstOrDefault().Name;
                    model.Materials += ", ";
                }
                model.Materials.TrimEnd();
                model.Materials.TrimEnd();

                _paginatedProducts.Add(model);
            }
        }

        private void Tb_Search_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox currentTb = sender as TextBox;
            string text = currentTb.Text;

            _listModifications.Remove(SearchKey);
            if (!String.IsNullOrEmpty(text) && text != SearchPlaceholder)
            {
                _listModifications.Add(SearchKey, () => LvProducts.ItemsSource = Utils.ListModifications.Search(LvProducts.ItemsSource.Cast<ProductListModel>(), text));
            }

            ApplyModifications();
        }

        private void Tb_Search_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _listModifications.Remove(SearchKey);
            ApplyModifications();

            TextBox currentTb = sender as TextBox;
            string text = currentTb.Text;

            if (String.IsNullOrEmpty(text))
            {
                SearchTextBox.Text = SearchPlaceholder;
            }
        }

        private void OnSortingChanged(object sender, SelectionChangedEventArgs e)
        {
            string order = (sender as ComboBox).SelectedItem as string;

            _listModifications.Remove(SortKey);

            if (order != DefaultOrder)
            {
                _listModifications.Add(SortKey, () => LvProducts.ItemsSource = Utils.ListModifications.Order(LvProducts.ItemsSource.Cast<ProductListModel>(), order));
            }

            ApplyModifications();
        }

        private void OnFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            var filter = (sender as ComboBox).SelectedItem as string;

            _listModifications.Remove(FilterKey);

            if (filter != UniversalFilter)
            {
                _listModifications.Add(FilterKey, () => LvProducts.ItemsSource = Utils.ListModifications.FilterByType(LvProducts.ItemsSource.Cast<ProductListModel>(), filter));
            }

            ApplyModifications();
        }

        private void ApplyModifications()
        {
            if (LvProducts is null)
            {
                return;
            }

            LvProducts.ItemsSource = _paginatedProducts;

            foreach (var key in _listModifications.Keys)
            {
                _listModifications[key]();
            }

            LvProducts.Items.Refresh();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
        }
    }
}
