<%@ WebHandler Language="C#" Class="BitcoinQRCode" %>

using System;
using System.Web;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Gma.QrCodeNet.Encoding;
using System.Drawing;
using System.IO;

public class BitcoinQRCode : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        var BTCAddress = context.Request["address"];

        if (BTCAddress != null)
        {
            QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.M);
            QrCode qrCode;
            encoder.TryEncode("bitcoin:" + BTCAddress, out qrCode);

            GraphicsRenderer gRenderer = new GraphicsRenderer(
            new FixedModuleSize(6, QuietZoneModules.Four),
            Brushes.Black, Brushes.White);

            using (MemoryStream ms = new MemoryStream())
            {
                gRenderer.WriteToStream(qrCode.Matrix, System.Drawing.Imaging.ImageFormat.Png, ms);
                context.Response.ContentType = "image/PNG";
                ms.WriteTo(context.Response.OutputStream);
            }
        }
    }

    public bool IsReusable { get { return false; } }
}