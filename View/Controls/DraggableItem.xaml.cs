using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RunTrack
{
    /// <summary>
    /// Interaktionslogik für DraggableItem.xaml
    /// </summary>
    public partial class DraggableItem : ContentControl
    {
        // Eigenschaft für den Textinhalt des DraggableItem
        public string TextContent { get; set; } = "Draggable Item";

        public DraggableItem()
        {
            InitializeComponent();
            this.DataContext = this;
            AllowDrop = true; // Erlaubt das Ablegen von Elementen auf diesem Control
            PreviewMouseLeftButtonDown += DraggableItem_PreviewMouseLeftButtonDown; // Event-Handler für das Drücken der linken Maustaste
            DragEnter += DraggableItem_DragEnter; // Event-Handler für das Ziehen eines Elements über dieses Control
            Drop += DraggableItem_Drop; // Event-Handler für das Ablegen eines Elements auf diesem Control
        }

        // Event-Handler für das Drücken der linken Maustaste
        private void DraggableItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, this, DragDropEffects.Move); // Startet den Drag & Drop-Vorgang
            }
        }

        // Event-Handler für das Ziehen eines Elements über dieses Control
        private void DraggableItem_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move; // Setzt den Effekt auf "Verschieben"
        }

        // Event-Handler für das Ablegen eines Elements auf diesem Control
        private void DraggableItem_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DraggableItem)))
            {
                var source = (DraggableItem)e.Data.GetData(typeof(DraggableItem)); // Quelle des Drag & Drop-Vorgangs
                var target = (DraggableItem)sender; // Ziel des Drag & Drop-Vorgangs

                // Tauscht die Positionen der beiden Controls im übergeordneten StackPanel
                var parent = (StackPanel)source.Parent;
                int sourceIndex = parent.Children.IndexOf(source);
                int targetIndex = parent.Children.IndexOf(target);
                parent.Children.Remove(source);
                parent.Children.Insert(targetIndex, source);
                parent.Children.Remove(target);
                parent.Children.Insert(sourceIndex, target);
            }
        }
    }
}
