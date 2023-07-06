
using System.Windows.Media.Imaging;

namespace wms.Client.Common
{
    /// <summary>
    /// 图标操作类
    /// </summary>
    public class ImageHelper
    {
        public static BitmapImage ConvertToImage(string fileName)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new System.Uri(fileName);
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
    }
}
