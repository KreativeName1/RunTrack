using System.Windows.Controls;

namespace Klimalauf
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

         //btnRemove.Click += (s, e) =>
         //{
         //   if (Parent is Panel panel)
         //   {
         //      panel.Children.Remove(this);
         //   }
         //};
      }
   }
}
