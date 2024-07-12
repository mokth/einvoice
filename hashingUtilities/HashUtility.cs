using Newtonsoft.Json;
using System;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Author      : TH MOK
/// Description : Utility use to hash,serialize and sign data
/// Date        : 2027-05-02
/// </summary>
namespace EInvoiceAPI.Utility
{
    public class HashUtility
    {
        public static string HashString(string text, string salt = "")
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // Convert the string to a byte array first, to be processed
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }

        public static string StringToBase64(string Base64String)
        {

            // Convert string to Base64
            byte[] bytes = Encoding.UTF8.GetBytes(Base64String);

            return System.Convert.ToBase64String(bytes, Base64FormattingOptions.None);
        }

        public static string SerializeJson(object doc)
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTH:mmZ",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,

            };
            var jsonString = JsonConvert.SerializeObject(doc, settings);

            return jsonString;
        }

        public static string SerializeJsonIndented(object doc)
        {
            var settings = new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-ddTH:mmZ",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            var jsonString = JsonConvert.SerializeObject(doc, settings);

            return jsonString;
        }

        public static byte[] Sha256Hash(string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] byteData = Encoding.UTF8.GetBytes(text);
                var hashBytes = sha256.ComputeHash(byteData);

                return hashBytes;
            }
        }
        public static byte[] Sha256HashBytes(byte[] byteData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(byteData);

                return hashBytes;
            }
        }

         //this will get the certDigest
        public static string GetCertHash(X509Certificate2 cert)
        {
            byte[] rawcertbytes = cert.RawData;
            byte[] certbytes = Sha256HashBytes(rawcertbytes);

            return Convert.ToBase64String(certbytes);
        }

        // this will get the cert raw data and put into Json's X509Certificate
        public static string GetX509Certificate(X509Certificate2 cert)
        {
            byte[] rawcertbytes = cert.RawData;
            return Convert.ToBase64String(rawcertbytes);

        }

        //Get Certificate number, need to convert to Int64
        public static Int64 GetCertSerialNumber(X509Certificate2 cert)
        {
            return Int64.Parse(cert.SerialNumber, NumberStyles.HexNumber);

        }

        //to sign the data become sign digest
        public static byte[] SignData(byte[] hashdata, X509Certificate2 cert)
        {
            byte[] signedData = null;
            using (RSA rsa = cert.GetRSAPrivateKey())
            {
                try
                {
                    var sharedParameters = rsa.ExportParameters(false);
                    RSAPKCS1SignatureFormatter rsaFormatter = new RSAPKCS1SignatureFormatter(rsa);
                    rsaFormatter.SetHashAlgorithm(nameof(SHA256));
                    signedData = rsaFormatter.CreateSignature(hashdata);
                }
                catch (CryptographicException ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }

            return signedData;
        }
    }
}
