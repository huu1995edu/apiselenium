using AForge.Imaging.Filters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tesseract;

namespace DockerApi
{
    public static class OCR_Recaptcha
    {
        public static Bitmap FormatImageRecaptcha(Bitmap imagem)
        {
            imagem = imagem.Clone(new Rectangle(0, 0, imagem.Width, imagem.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Erosion erosion = new Erosion();
            Dilatation dilatation = new Dilatation();
            Closing close = new Closing();
            Invert inverter = new Invert();
            ColorFiltering cor = new ColorFiltering();
            cor.Blue = new AForge.IntRange(200, 255);
            cor.Red = new AForge.IntRange(200, 255);
            cor.Green = new AForge.IntRange(200, 255);
            Opening open = new Opening();
            BlobsFiltering bc = new BlobsFiltering();
            bc.MinHeight = 30; // chỉ bắt các đối tượng có chiều cao >=30
            GaussianSharpen gs = new GaussianSharpen(1, 2);// tăng độ đậm của chi tiết lên gấp đôi
            ContrastCorrection cc = new ContrastCorrection();
            FiltersSequence seq = new FiltersSequence(gs, inverter, open, inverter, bc, open, cc, bc, cor);
            return seq.Apply(imagem);
        }
        public static string OCR(Pix b)
        {
            try
            {
                string res = "";
                var pathtessdata = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
                using (var engine = new TesseractEngine(pathtessdata, "eng", EngineMode.Default))
                {
                    engine.SetVariable("tessedit_char_whitelist", "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                    engine.SetVariable("tessedit_unrej_any_wd", true);
                    using (var page = engine.Process(b, PageSegMode.SingleLine))
                    {
                        res = page.GetText();
                    }
                }
                return res;
            }
            catch (Exception ex)
            {
                LogWriter.LogWrite($"Không thể đọc được mã: {ex.Message}");
                throw new Exception("Không thể đọc được mã: " + ex.Message);

            }

        }

    }
}
