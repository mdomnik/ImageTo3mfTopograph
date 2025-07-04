using System.IO;
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
using ImageTo3mfTopograph_Application.Sources;

namespace ImageTo3mfTopograph_Application.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //Window events
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //DropZone events
        private void DropZone_Drop(object sender, DragEventArgs e)
        {
            // Check if the data being dropped is a file from the file system
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // creates a string array of the dropped files paths
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // if there are files in the array, and the first file exists, you can process it
                if (files.Length > 0 && File.Exists(files[0]))
                {
                    TryLoadImage(files[0]);
                }
            }
        }

        private void DropZone_DragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;

            // Check if the data being dragged is a file from the file system
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Set the effects to Copy to indicate that the file can be copied
                e.Effects = DragDropEffects.Copy;
                // Change the appearance of the DropZone to indicate a valid drop target
                DropZone.BorderBrush = new SolidColorBrush(Color.FromRgb(32, 214, 161));
                DropZone.Background = new SolidColorBrush(Color.FromRgb(245, 255, 250));
            }
            else
            {
                // If the data is not a file, set the effects to None
                e.Effects = DragDropEffects.None;
            }
        }

        private void DropZone_DragLeave(object sender, DragEventArgs e)
        {
            // Reset to original style
            DropZone.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00B37E"));
            DropZone.Background = Brushes.White;
        }

        private void DropZone_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select an Image",
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.tiff"
            };

            if (dialog.ShowDialog() == true)
            {
                // If a file is selected, try to load the image
                TryLoadImage(dialog.FileName);
            }
        }

        //member functions
        private void TryLoadImage(string filePath)
        {
            // Attempt to load the image from the specified file path
            try
            {
                var bitmap = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                ImageLoader.inputImage = bitmap;

                PreviewImage.Source = bitmap;
                PreviewImage.Visibility = Visibility.Visible;
                DropHint.Visibility = Visibility.Collapsed;

                ErrorMessage.Text = $"Image loaded successfully!";
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Error loading image: Unsupported file type";
            }
        }

        private void HeightIn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(HeightIn.Text, out double height))
            {
                if (height <= 0.1)
                {
                    HeightIn.Background = new SolidColorBrush(Color.FromRgb(255, 229, 229));
                    return;
                }
                ImageLoader.inputHeight = height;
                HeightIn.ClearValue(BackgroundProperty);
            }
            else
            {
                HeightIn.Background = new SolidColorBrush(Color.FromRgb(255, 229, 229));
            }
        }

        private void ScaleIn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(ScaleIn.Text, out double scale))
            {
                if (scale <= 0)
                {
                    ScaleIn.Background = new SolidColorBrush(Color.FromRgb(255, 229, 229));
                    return;
                }
                ImageLoader.inputScale = scale;
                ScaleIn.ClearValue(BackgroundProperty);
            }
            else
            {
                ScaleIn.Background = new SolidColorBrush(Color.FromRgb(255, 229, 229));
            }
        }

        private void MaxLayerCountIn_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(MaxLayerCountIn.Text, out int max))
            {
                if (max <= 0)
                {
                    ScaleIn.Background = new SolidColorBrush(Color.FromRgb(255, 229, 229));
                    return;
                }
                ImageLoader.maxLayerCount = max;
                ScaleIn.ClearValue(BackgroundProperty);
            }
            else
            {
                ScaleIn.Background = new SolidColorBrush(Color.FromRgb(255, 229, 229));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ImageLoader.inputImage == null)
            {
                // Open file dialog to load image (like DropZone_Click)
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Select an image file",
                    Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp"
                };

                if (dialog.ShowDialog() == true)
                {
                    TryLoadImage(dialog.FileName);
                }
            }
            else
            {
                ImageLoader.colorMasks = ImageAnalyzer.GetColorMasks();
                ImageLoader.SaveMasksToDirectory(ImageLoader.colorMasks, "C:\\Users\\mdomn\\Downloads\\GeneratedMasks");
            }
        }
    }
}