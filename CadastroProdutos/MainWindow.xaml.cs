using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.ObjectModel;

namespace CadastroProdutos;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
namespace CadastroProdutos{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Product> _products = new ObservableCollection<Product>();
        private ObservableCollection<Fornecedor> _fornecedores = new ObservableCollection<Fornecedor>();
        private readonly HttpClient _httpClient;
        private const string API_BASE_URL = "http://localhost:5099";

        public MainWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };
            LoadFornecedoresFromApi();
            LoadProductsFromApi();
            dgProducts.ItemsSource = _products;
            dgFornecedores.ItemsSource = _fornecedores;
        }

        #region Fornecedores

        private async void LoadFornecedoresFromApi(){
            try
            {
                var fornecedores = await _httpClient.GetFromJsonAsync<List<Fornecedor>>("/fornecedores");
                if (fornecedores != null)
                {
                    _fornecedores.Clear();
                    foreach (var fornecedor in fornecedores)
                    {
                        _fornecedores.Add(fornecedor);
                    }
                    cboFornecedor.ItemsSource = _fornecedores;
                    if (_fornecedores.Any())
                    {
                        cboFornecedor.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar fornecedores: {ex.Message}", "Erro", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AdicionarFornecedor_Click(object sender, RoutedEventArgs e){
            if(string.IsNullOrWhiteSpace(txtFornecedorName.Text) || 
               string.IsNullOrWhiteSpace(txtFornecedorCNPJ.Text) ||
               string.IsNullOrWhiteSpace(txtFornecedorTelefone.Text)){
                MessageBox.Show("Preencha todos os campos!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newFornecedor = new Fornecedor {
                Nome = txtFornecedorName.Text,
                CNPJ = txtFornecedorCNPJ.Text,
                Telefone = txtFornecedorTelefone.Text
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/fornecedores", newFornecedor);
                
                if (response.IsSuccessStatusCode)
                {
                    var createdFornecedor = await response.Content.ReadFromJsonAsync<Fornecedor>();
                    if (createdFornecedor != null)
                    {
                        _fornecedores.Add(createdFornecedor);
                    }
                    
                    txtFornecedorName.Clear();
                    txtFornecedorCNPJ.Clear();
                    txtFornecedorTelefone.Clear();
                    
                    MessageBox.Show("Fornecedor adicionado com sucesso!", "Sucesso", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao adicionar fornecedor: {errorContent}", "Erro", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar com a API: {ex.Message}", "Erro", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RemoverFornecedor_Click(object sender, RoutedEventArgs e){
            if(dgFornecedores.SelectedItem is Fornecedor fornecedorSelected){
                var result = MessageBox.Show($"Deseja realmente excluir o fornecedor: {fornecedorSelected.Nome}?",
                "Confirmar exclusão", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

                if(result == MessageBoxResult.Yes){
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"/fornecedores/{fornecedorSelected.Id}");
                        
                        if (response.IsSuccessStatusCode)
                        {
                            _fornecedores.Remove(fornecedorSelected);
                            MessageBox.Show("Fornecedor removido com sucesso!", "Sucesso", 
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Erro ao remover fornecedor da API.", "Erro", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao conectar com a API: {ex.Message}", "Erro", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            } else {
                MessageBox.Show("Selecione um fornecedor!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void AtualizarFornecedor_Click(object sender, RoutedEventArgs e){
            if(dgFornecedores.SelectedItem is Fornecedor fornecedorSelected){
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"/fornecedores/{fornecedorSelected.Id}", fornecedorSelected);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Fornecedor atualizado com sucesso!", "Sucesso",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erro ao atualizar fornecedor: {errorContent}", "Erro", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao conectar com a API: {ex.Message}", "Erro", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione um fornecedor para atualizar!", "Aviso", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Produtos

        private async void LoadProductsFromApi(){
            try
            {
                var products = await _httpClient.GetFromJsonAsync<List<Product>>("");
                if (products != null)
                {
                    _products.Clear();
                    foreach (var product in products)
                    {
                        _products.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar produtos da API: {ex.Message}\n\nCertifique-se de que a API está rodando em {API_BASE_URL}", 
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Adicionar_Click(object sender, RoutedEventArgs e){
            int qty = int.TryParse(txtProductQuantity.Text, out var q) ? q : 0;

            if(string.IsNullOrWhiteSpace(txtProductName.Text) || string.IsNullOrWhiteSpace(txtProductCategory.Text)){
                MessageBox.Show("Preencha todos os campos!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if(cboFornecedor.SelectedValue == null){
                MessageBox.Show("Selecione um fornecedor!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newProduct = new Product {
                Nome = txtProductName.Text,
                Categoria = txtProductCategory.Text,
                Quantidade = qty,
                FornecedorId = (int)cboFornecedor.SelectedValue
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("", newProduct);
                
                if (response.IsSuccessStatusCode)
                {
                    var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
                    if (createdProduct != null)
                    {
                        _products.Add(createdProduct);
                    }
                    
                    txtProductName.Clear();
                    txtProductCategory.Clear();
                    txtProductQuantity.Clear();
                    cboFornecedor.SelectedIndex = _fornecedores.Any() ? 0 : -1;
                    
                    MessageBox.Show("Produto adicionado com sucesso!", "Sucesso", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Erro ao adicionar produto: {errorContent}", "Erro", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao conectar com a API: {ex.Message}", "Erro", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Remover_Click(object sender, RoutedEventArgs e){
            if(dgProducts.SelectedItem is Product productSelected){
                var result = MessageBox.Show($"Deseja realmente excluir o produto: {productSelected.Nome}?",
                "Confirmar exclusão", 
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

                if(result == MessageBoxResult.Yes){
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"/{productSelected.Id}");
                        
                        if (response.IsSuccessStatusCode)
                        {
                            _products.Remove(productSelected);
                            MessageBox.Show("Produto removido com sucesso!", "Sucesso", 
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            MessageBox.Show("Erro ao remover produto da API.", "Erro", 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao conectar com a API: {ex.Message}", "Erro", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            } else {
                MessageBox.Show("Selecione um produto!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void SalvarBanco_Click(object sender, RoutedEventArgs e){
            // Como agora estamos usando a API, cada operação já salva automaticamente
            // Este botão pode ser usado para atualizar produtos existentes
            if(dgProducts.SelectedItem is Product productSelected){
                try
                {
                    var response = await _httpClient.PutAsJsonAsync($"/{productSelected.Id}", productSelected);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Produto atualizado com sucesso!", "Sucesso",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Erro ao atualizar produto: {errorContent}", "Erro", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao conectar com a API: {ex.Message}", "Erro", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Selecione um produto para atualizar!", "Aviso", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void AdicionarProduto_Click(object sender, RoutedEventArgs e) => await Adicionar_Click(sender, e);
        private async void RemoverProduto_Click(object sender, RoutedEventArgs e) => await Remover_Click(sender, e);
        private async void AtualizarProduto_Click(object sender, RoutedEventArgs e) => await SalvarBanco_Click(sender, e);

        #endregion
    }

    // Model class matching the API
    public class Product
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string Categoria { get; set; } = "";
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
        public int Quantidade { get; set; } = 0;
        public int FornecedorId { get; set; }
    }

    public class Fornecedor
    {
        public int Id { get; set; }
        public string Nome { get; set; } = "";
        public string CNPJ { get; set; } = "";
        public string Telefone { get; set; } = "";
    }
}