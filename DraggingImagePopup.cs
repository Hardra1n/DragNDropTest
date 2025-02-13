using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Xml.Linq;

namespace DragNDropTask
{
    internal class DraggingImagePopup : Popup
    {
        private Size ImageSize;
        private int XOffset = 15;
        private int YOffset = 15;

        public DraggingImagePopup(UIElement imageElement)
        {
            ImageSize.Width = imageElement.RenderSize.Width / 2;
            ImageSize.Height = imageElement.RenderSize.Height / 2;
            Width = ImageSize.Width;
            Height = ImageSize.Height;
            Placement = PlacementMode.Absolute;
            AllowsTransparency = true;

            var dragginImage = new Image()
            {
                Source = CreateImageSource(imageElement),
                Width = ImageSize.Width,
                Height = ImageSize.Height
            };
            dragginImage.CacheMode = new BitmapCache();

            Child = dragginImage;
        }

        public void UpdatePosition()
        {
            Extensions.GetCursorPos(out POINT mousePoint);
            HorizontalOffset = mousePoint.X + XOffset;
            VerticalOffset = mousePoint.Y + YOffset;
        }


        private ImageSource CreateImageSource(UIElement element)
        {
            double dpi = 124;
            int pixelWidth = (int)(ImageSize.Width / 96 * dpi);
            int pixelHeight = (int)(ImageSize.Height / 96 * dpi);
            var renderTargetBitmap = new RenderTargetBitmap(pixelWidth, pixelHeight, dpi, dpi, PixelFormats.Pbgra32);

            var visual = new DrawingVisual();

            using (var context = visual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(element), null, new Rect(ImageSize));
            }

            renderTargetBitmap.Render(visual);
            return renderTargetBitmap;
        }
    }
}
