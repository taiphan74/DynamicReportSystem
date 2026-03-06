using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using DynamicReportSystem.Configuration;
using DynamicReportSystem.Repositories;
using DynamicReportSystem.Services;

namespace DynamicReportSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDatabaseExplorerService? _databaseExplorerService;
        private readonly HashSet<string> _loadedTableNames = new(StringComparer.OrdinalIgnoreCase);

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var connectionString = AppConfiguration.GetConnectionString();
                var schemaRepository = new SchemaRepository(connectionString);
                var tableDataRepository = new TableDataRepository(connectionString);
                _databaseExplorerService = new DatabaseExplorerService(schemaRepository, tableDataRepository);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Cannot initialize application configuration.\nDetails: {ex.Message}",
                    "Configuration Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTablesOnStartup();
        }

        private void LoadTablesOnStartup()
        {
            if (_databaseExplorerService is null)
            {
                return;
            }

            try
            {
                var tableNames = _databaseExplorerService.LoadTableNames();

                _loadedTableNames.Clear();
                foreach (var tableName in tableNames)
                {
                    _loadedTableNames.Add(tableName);
                }

                lstTables.ItemsSource = tableNames;
                lstTables.SelectedIndex = tableNames.Count > 0 ? 0 : -1;

                if (tableNames.Count == 0)
                {
                    MessageBox.Show("No tables found in the selected database.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot load table list.\nDetails: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lstTables_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_databaseExplorerService is null)
            {
                MessageBox.Show("Data service is not initialized.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (lstTables.SelectedItem is not string selectedTable || string.IsNullOrWhiteSpace(selectedTable))
            {
                return;
            }

            if (!_loadedTableNames.Contains(selectedTable))
            {
                MessageBox.Show("Invalid table selected. Please restart and load table list again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadTablePreviewAndColumns(selectedTable);
        }

        private void LoadTablePreviewAndColumns(string tableName)
        {
            if (_databaseExplorerService is null)
            {
                return;
            }

            try
            {
                var previewData = _databaseExplorerService.GetPreviewData(tableName);
                var columns = _databaseExplorerService.LoadColumns(tableName);

                txtSelectedTable.Text = $"Preview dữ liệu: {tableName} (TOP 100)";
                dgData.ItemsSource = previewData.DefaultView;
                lstColumns.ItemsSource = columns;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot load table preview for '{tableName}'.\nDetails: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
