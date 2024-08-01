using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Security.Cryptography;

/// <summary>
/// Author      : TH MOK
/// Date        : 16/07/2024
/// Description : To manually generate the UBLExtensions scratch. Cause LHDN keep changing. Easy to maintain
/// </summary>
namespace AngelMay.EInvoiceLib.Utility
{
    public class SignatureData
    {
        public string SignatureValue { get; set; }
        public string PropsDigest { get; set; }
        public string DocDigest { get; set; }
        public string CertDigest { get; set; }
        public string SigningTime { get; set; }
        public string X509Certificate { get; set; }
        public string X509IssuerName { get; set; }
        public string X509SerialNumber { get; set; }
        public string X509SubjectName { get; set; }
        public string RSAKey_Modulus { get; set; }
        public string RSAKey_Exponent { get; set; }
    }

    public static class UBLSignatureXML
    {
        public static XmlDocument doc;
        public static XmlDocument xmlInv;

        static XNamespace xmlns = "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2";
        static XNamespace cac = "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2";
        static XNamespace cbc = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2";
        static XNamespace ext = "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2";
        static XNamespace sac = "urn:oasis:names:specification:ubl:schema:xsd:SignatureAggregateComponents-2";
        static XNamespace sbc = "urn:oasis:names:specification:ubl:schema:xsd:SignatureBasicComponents-2";
        static XNamespace sig = "urn:oasis:names:specification:ubl:schema:xsd:CommonSignatureComponents-2";
        static XNamespace ds = "http://www.w3.org/2000/09/xmldsig#";
        static XNamespace xades = "http://uri.etsi.org/01903/v1.3.2#";

        static SignatureData _signData;
        static string _certPath;
        static X509Certificate2 cert;
        static string invXmlString;

        /// <summary>
        /// docXml is full document (invoice,CN,DN...) xml string without UBLExtensions and Signature
        /// </summary>
        /// <param name="docXml"></param>
        public static void StartGenerate(string docXml)
        {
            invXmlString = docXml;
            doc = new XmlDocument();
            ReadCertFile();
            PrepareDigestData();

            //create a temp Invoice xml, cause need to include all the header xmlns            
            XElement root = InvoiceRoot();
            var mainXMl = ToXmlElement(root, doc);
            //now doc contains the whole UBLExtensions struture and data
            doc.AppendChild(mainXMl);

            //get the SignedProperties via xpath
            var propSignXml = getSignPropertiesXML();
            propSignXml = RemoveXmlDeclaration(propSignXml);

            byte[] propCert = Sha256Hash(propSignXml);
            //populate the propCert into the Reference
            setSignReferenceXML(Convert.ToBase64String(propCert));

            //now extra the UBLExtensions from the temp doc and merge into the main xml (docXm)
            InsertUBLExtIntoMainDoc();

        }

        private static void InsertUBLExtIntoMainDoc()
        {
            var docXML = doc.OuterXml;

            int start = docXML.IndexOf("<ext:UBLExtensions");
            int end = docXML.IndexOf("</ext:UBLExtensions>");

            int start2 = docXML.IndexOf("<cac:Signature");
            int end2 = docXML.IndexOf("</cac:Signature>");

            var UBLExtensionsXML = docXML.Substring(start, end - start + "</ext:UBLExtensions>".Length);
            var signatureXML = docXML.Substring(start2, end2 - start2 + "</cac:Signature>".Length);

            start = invXmlString.IndexOf("<cbc:ID>");
            var content = invXmlString.Insert(start, UBLExtensionsXML);
            start = content.IndexOf("<cac:AccountingSupplierParty>");
            content = content.Insert(start, signatureXML);
            content = XmlDsigC14N(content);
            xmlInv = new XmlDocument();
            xmlInv.LoadXml(content);
        }

        private static byte[] GenerateDocHash()
        {

            string canonicalXml = XmlDsigC14N(invXmlString);
            xmlInv = new XmlDocument();
            xmlInv.LoadXml(canonicalXml);
            var processedXml = PreprocessXml(xmlInv.OuterXml);
            return Sha256Hash(RunC14N(processedXml));
        }

        private static void ReadCertFile()
        {
            string certPath = "your cert full path";
            string certPass = "your cert passsowrd";
            cert = new X509Certificate2();
            cert.Import(File.ReadAllBytes(certPath), certPass, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
        }

        static void PrepareDigestData()
        {
            _signData = new SignatureData();
            byte[] docHash = GenerateDocHash();
            byte[] certbytes = Sha256HashBytes(cert.RawData);
            byte[] sign = SignData(docHash, cert);

            _signData.CertDigest = Convert.ToBase64String(certbytes);
            _signData.DocDigest = Convert.ToBase64String(docHash);
            _signData.SignatureValue = Convert.ToBase64String(sign);
            _signData.X509Certificate = Convert.ToBase64String(cert.RawData);
            _signData.X509IssuerName = cert.Issuer;
            _signData.X509SerialNumber = BigInteger.Parse(cert.SerialNumber, NumberStyles.HexNumber).ToString();
            _signData.X509SubjectName = cert.Subject;
            _signData.SigningTime = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
        }

        static string getSignPropertiesXML()
        {

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            addNameSpace(nsmgr);

            string xpath = "/a:Invoice/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent/sig:UBLDocumentSignatures/sac:SignatureInformation/ds:Signature/ds:Object/xades:QualifyingProperties/xades:SignedProperties";
            var element = doc.SelectSingleNode(xpath, nsmgr);

            return LinearizeXml(element.OuterXml);

        }

        static void setSignReferenceXML(string digest)
        {
            var nav = doc.CreateNavigator();

            XmlNamespaceManager nsmgr = XMLNs(nav);
            string xpath = "/a:Invoice/ext:UBLExtensions/ext:UBLExtension/ext:ExtensionContent/sig:UBLDocumentSignatures/sac:SignatureInformation/ds:Signature/ds:SignedInfo/ds:Reference[2]/ds:DigestValue";
            XPathNavigator element = nav.SelectSingleNode(xpath, nsmgr);
            element.SetValue(digest);

        }

        private static XmlNamespaceManager XMLNs(XPathNavigator nav)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(nav.NameTable);
            addNameSpace(nsmgr);
            return nsmgr;
        }

        private static void addNameSpace(XmlNamespaceManager nsmgr)
        {
            nsmgr.AddNamespace("a", xmlns.NamespaceName);
            nsmgr.AddNamespace("cac", cac.NamespaceName);
            nsmgr.AddNamespace("cbc", cbc.NamespaceName);
            nsmgr.AddNamespace("ext", ext.NamespaceName);
            nsmgr.AddNamespace("sac", sac.NamespaceName);
            nsmgr.AddNamespace("sig", sig.NamespaceName);
            nsmgr.AddNamespace("ds", ds.NamespaceName);
            nsmgr.AddNamespace("xades", xades.NamespaceName);
        }

        static XmlElement ToXmlElement(this XElement xelement, XmlDocument xmldoc)
        {
            return xmldoc.ReadNode(xelement.CreateReader()) as XmlElement;
        }
        static XElement InvoiceRoot()
        {
            return new XElement(xmlns + "Invoice",
                new XAttribute(XNamespace.Xmlns + "cac", cac),
                new XAttribute(XNamespace.Xmlns + "cbc", cbc),
                new XAttribute(XNamespace.Xmlns + "ext", ext),
                 new XElement(UBLExtensions()),
                 new XElement(InvoiceSignature()));
        }

        static XElement InvoiceSignature()
        {
            return new XElement(cac + "Signature",
                    new XElement(cbc + "ID", "urn:oasis:names:specification:ubl:signature:Invoice"),
                    new XElement(cbc + "SignatureMethod", "urn:oasis:names:specification:ubl:dsig:enveloped:xades"));

        }

        static XElement UBLExtensions()
        {
            return new XElement(ext + "UBLExtensions",
                    new XElement(ext + "UBLExtension",
                       new XElement(ExtensionURI()),
                       new XElement(ExtensionContent())));
        }

        static XElement ExtensionURI()
        {
            return new XElement(ext + "ExtensionURI", "urn:oasis:names:specification:ubl:dsig:enveloped:xades");
        }

        static XElement ExtensionContent()
        {
            return new XElement(ext + "ExtensionContent",
                     new XElement(UBLDocumentSignatures()));
        }

        static XElement UBLDocumentSignatures()
        {
            return new XElement(sig + "UBLDocumentSignatures",
                        new XAttribute(XNamespace.Xmlns + "sig", sig),
                         new XAttribute(XNamespace.Xmlns + "sac", sac),
                         new XAttribute(XNamespace.Xmlns + "sbc", sbc),
                        new XElement(SignatureInformation()));
        }

        static XElement SignatureInformation()
        {
            return new XElement(sac + "SignatureInformation",
                         new XElement(cbcID()),
                         new XElement(ReferencedSignatureID()),
                          new XElement(Signature())
                 );
        }

        static XElement cbcID()
        {
            return new XElement(cbc + "ID", "urn:oasis:names:specification:ubl:signature:1");
        }

        static XElement ReferencedSignatureID()
        {
            return new XElement(sbc + "ReferencedSignatureID", "urn:oasis:names:specification:ubl:signature:Invoice");
        }

        static XElement Signature()
        {
            return new XElement(ds + "Signature",
                         new XAttribute(XNamespace.Xmlns + "ds", ds),
                         new XAttribute("Id", "signature"),
                         new XElement(SignedInfo()),
                         new XElement(SignatureValue()),
                         new XElement(KeyInfo()),
                         new XElement(Object())
                         );
        }

        static XElement SignedInfo()
        {
            return new XElement(ds + "SignedInfo",
                     new XElement(CanonicalizationMethod()),
                     new XElement(SignatureMethod()),
                     new XElement(ReferenceDoc()),
                     new XElement(ReferenceSign())
                     );
        }

        static XElement CanonicalizationMethod()
        {
            return new XElement(ds + "CanonicalizationMethod",
                     new XAttribute("Algorithm", "http://www.w3.org/2006/12/xml-c14n11"));
        }
        static XElement SignatureMethod()
        {
            return new XElement(ds + "SignatureMethod",
                     new XAttribute("Algorithm", "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256"));
        }

        static XElement ReferenceDoc()
        {
            return new XElement(ds + "Reference",
                      new XAttribute("Id", "id-doc-signed-data"),
                      new XAttribute("URI", ""),
                      new XElement(ds + "Transforms",
                         new XElement(ds + "Transform",
                             new XAttribute("Algorithm", "http://www.w3.org/TR/1999/REC-xpath-19991116"),
                             new XElement(ds + "XPath", "not(//ancestor-or-self::ext:UBLExtensions)")),
                        new XElement(ds + "Transform",
                             new XAttribute("Algorithm", "http://www.w3.org/TR/1999/REC-xpath-19991116"),
                             new XElement(ds + "XPath", "not(//ancestor-or-self::cac:Signature)")),
                        new XElement(ds + "Transform",
                             new XAttribute("Algorithm", "http://www.w3.org/2006/12/xml-c14n11"))),

                      new XElement(ds + "DigestMethod",
                              new XAttribute("Algorithm", "http://www.w3.org/2001/04/xmlenc#sha256")),
                      new XElement(ds + "DigestValue", _signData.DocDigest)

                     );
        }

        static XElement ReferenceSign()
        {
            return new XElement(ds + "Reference",
                      new XAttribute("Type", "http://www.w3.org/2000/09/xmldsig#SignatureProperties"),
                      new XAttribute("URI", "#id-xades-signed-props"),
                      new XElement(ds + "DigestMethod",
                              new XAttribute("Algorithm", "http://www.w3.org/2001/04/xmlenc#sha256")),
                      new XElement(ds + "DigestValue", _signData.PropsDigest)

                     );
        }

        static XElement SignatureValue()
        {
            return new XElement(ds + "SignatureValue", _signData.SignatureValue);
        }

        static XElement KeyInfo()
        {
            return new XElement(ds + "KeyInfo",
                     new XElement(ds + "X509Data",
                       new XElement(ds + "X509Certificate", _signData.X509Certificate)));
        }

        static XElement Object()
        {
            return new XElement(ds + "Object",
                    new XElement(QualifyingProperties()));
        }

        static XElement QualifyingProperties()
        {
            return new XElement(xades + "QualifyingProperties",
                     new XAttribute(XNamespace.Xmlns + "xades", xades),
                     new XAttribute("Target", "signature"),
                     new XElement(SignedPropertiess()));

        }

        static XElement SignedPropertiess()
        {
            return new XElement(xades + "SignedProperties",
                    new XAttribute("Id", "id-xades-signed-props"),
                    new XElement(SignedSignatureProperties()));
        }

        static XElement SignedSignatureProperties()
        {
            return new XElement(xades + "SignedSignatureProperties",
                    new XElement(xades + "SigningTime", _signData.SigningTime),
                    new XElement(SigningCertificate()));
        }

        static XElement SigningCertificate()
        {
            return new XElement(xades + "SigningCertificate",
                    new XElement(xades + "Cert",
                       new XElement(xades + "CertDigest",
                           new XElement(ds + "DigestMethod",
                                new XAttribute("Algorithm", "http://www.w3.org/2001/04/xmlenc#sha256"), ""),
                           new XElement(ds + "DigestValue", _signData.CertDigest)
                           ),
                        new XElement(xades + "IssuerSerial",
                             new XElement(ds + "X509IssuerName", _signData.X509IssuerName),
                               new XElement(ds + "X509SerialNumber", _signData.X509SerialNumber))));
        }

        private static string PreprocessXml(string xml)
        {
            xml = RemoveXmlDeclaration(xml);
            xml = RemoveSignatureElement(xml);
            xml = LinearizeXml(xml);
            return xml;
        }

        private static string RemoveXmlDeclaration(string xml)
        {
            return Regex.Replace(xml, @"<\?xml.*?\?>", string.Empty).Trim();
        }

        private static string RemoveSignatureElement(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlNamespaceManager nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");

            XmlNode signatureNode = doc.SelectSingleNode("//cac:Signature", nsManager);
            if (signatureNode != null)
            {
                signatureNode.ParentNode.RemoveChild(signatureNode);
            }

            return doc.OuterXml;
        }

        private static string RunC14N(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            XmlDsigC14NTransform transform = new XmlDsigC14NTransform();
            transform.LoadInput(doc);

            using (var ms = new System.IO.MemoryStream())
            {
                var stream = (Stream)transform.GetOutput(typeof(Stream));
                stream.CopyTo(ms);
                ms.Position = 0;

                using (var sr = new System.IO.StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        //Mok added on 24-07-2024 proper method to LinearizeXml 
        private static string LinearizeXml(string xml)
        {
            XDocument xdoc = XDocument.Parse(xml);

            // Convert to string with single line
            string linearizedXml = xdoc.ToString(SaveOptions.DisableFormatting);
            return linearizedXml;
            //return Regex.Replace(xml, @"\s+", " ").Trim();
        }
       //old methold may have issues take out on 24-07-2024
       // private static string LinearizeXml(string xml)
       // {
       //     return Regex.Replace(xml, @"\s+", " ").Trim();
       // }

        public static string SerializeObjectToXml(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            namespaces.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            namespaces.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");

            XmlWriterSettings settings = new XmlWriterSettings
            {
                NewLineHandling = NewLineHandling.None,
                NewLineChars = "",
                OmitXmlDeclaration = true
            };

            StringBuilder output = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(output, settings))
            {
                serializer.Serialize(xmlWriter, obj, namespaces);
                xmlWriter.Flush();
                return output.ToString();
            }


        }

        public static string XmlDsigC14N(string xmlString)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            // Create a XmlDsigC14NTransform object
            XmlDsigC14NTransform transform = new XmlDsigC14NTransform();
            // Load XML data into the transform
            transform.LoadInput(xmlDoc);
            // Perform canonicalization
            Stream outputStream = (Stream)transform.GetOutput(typeof(Stream));

            // Read canonical XML data
            StreamReader reader = new StreamReader(outputStream);
            string canonicalXml = reader.ReadToEnd();
            return canonicalXml;
        }

        static byte[] Sha256Hash(string text)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] byteData = Encoding.UTF8.GetBytes(text);
                var hashBytes = sha256.ComputeHash(byteData);
                return hashBytes;
            }
        }

        static byte[] Sha256HashBytes(byte[] byteData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(byteData);
                return hashBytes;
            }
        }

        static byte[] SignData(byte[] hashdata, X509Certificate2 cert)
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
                catch (CryptographicException)
                {
                   throw;
                }
            }

            return signedData;
        }
    }
}
