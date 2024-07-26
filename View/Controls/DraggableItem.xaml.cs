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
        public string TextContent { get; set; } = "Draggable Item";
        public DraggableItem()
        {
            InitializeComponent();
            this.DataContext = this;
            AllowDrop = true;
            PreviewMouseLeftButtonDown += DraggableItem_PreviewMouseLeftButtonDown;
            DragEnter += DraggableItem_DragEnter;
            Drop += DraggableItem_Drop;
        }


        private void DraggableItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
            }
        }

        private void DraggableItem_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        private void DraggableItem_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(DraggableItem)))
            {
                var source = (DraggableItem)e.Data.GetData(typeof(DraggableItem));
                var target = (DraggableItem)sender;

                // Swap the positions of the two controls in the parent StackPanel
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
