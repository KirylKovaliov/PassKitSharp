using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Org.BouncyCastle.Asn1.X9;
using QRCoder;

namespace PassKitSharp
{
    public static class PK2Html
    {
        public static string Convert(PassKit passKit)
        {
            var templateSb = new StringBuilder(GetBaseHtmlTemplate());
            templateSb.Replace("{{ backgroundColor }}", passKit.BackgroundColor.ToString());
            if (passKit.Logo != null)
            {
                templateSb.Replace("{{ logo }}", passKit.Logo.ToBase64());
            }
            
            if (passKit.Background != null)
            {
                templateSb.Replace("{{ backgroundimage }}", passKit.Logo.ToBase64());
            }
            
            templateSb.Replace("{{ headerFieldsHtml }}", BuildFields(passKit, passKit.HeaderFields));
            templateSb.Replace("{{ primaryFieldsHtml }}", BuildFields(passKit, passKit.PrimaryFields));
            templateSb.Replace("{{ secondaryFieldsHtml }}", BuildFields(passKit, passKit.SecondaryFields));
            templateSb.Replace("{{ auxiliaryFieldsHtml }}", BuildFields(passKit, passKit.AuxiliaryFields));
            
            templateSb.Replace("{{ qrCodeHtml }}", BuildQrCodeHtml(passKit));
            return templateSb.ToString();
        }

        private static string BuildQrCodeHtml(PassKit kit)
        {
            if (kit.Barcode == null || kit.Barcode.Message == null)
                return string.Empty;
            
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode("kit.Barcode.Message", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            MemoryStream stream = new MemoryStream();
            qrCode.GetGraphic(20).Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            var base64Image = System.Convert.ToBase64String(stream.ToArray());
            return $"<img id=\"qrcode\" width=\"120\" height=\"120\" src=\"data:image/png;base64,{base64Image}\">";
        }

        private static string BuildFields(PassKit kit, PKPassFieldSet fields)
        {
            if (fields == null)
                return string.Empty;
            
            var sb = new StringBuilder();
            sb.Append("<dl>");

            foreach (PKPassField field in fields.Where(x => x != null))
            {
                if (string.IsNullOrEmpty(field.Label) && string.IsNullOrEmpty(field.Value))
                    continue;
                
                sb.Append($"<dt ");
                if (kit.LabelColor != null)
                {
                    sb.Append($"style=\"color: {kit.LabelColor} ;\"");    
                }
                sb.Append($">");
                sb.Append($"{field.Label}</dt>");
                
                sb.Append($"<dd ");
                if (kit.ForegroundColor != null)
                {
                    sb.Append($"style=\"color: {kit.ForegroundColor} ;\"");    
                }
                sb.Append($">");
                sb.Append($"{field.Value}</dd>");
            }
            
            sb.Append("</dl>");

            return sb.ToString();
        }

        private static string GetBaseHtmlTemplate()
        {
            using (var stream = typeof(PK2Html).Assembly.GetManifestResourceStream("PassKitSharp.Templates.baseTemplate.html"))
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}