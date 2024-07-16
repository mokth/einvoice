  How to use
  --------------
  xmlString is the is full document (invoice,CN,DN...) xml string without UBLExtensions and Signature 
  UBLSignatureXML.StartGenerate(xmlString);
  string content = UBLSignatureXML.xmlInv.OuterXml;
  content = UBLSignatureXML.XmlDsigC14N(content);
  now can submit the content (full xml with signature).

  Note.
For those who still fail to digest (whatever digest), one thing you need to bear in mind is that the sequence of the string will affect the hash.
eg
    var a=  Convert.ToBase64String(Sha256Hash("ABCDE"));
    var b = Convert.ToBase64String(Sha256Hash("EDCBA"));
the a != b.

 line1 = "<xades:QualifyingProperties xmlns:xades="http://uri.etsi.org/01903/v1.3.2#" Target="signature">"
 line2 = "<xades:QualifyingProperties Target="signature xmlns:xades="http://uri.etsi.org/01903/v1.3.2#"">
line1 hash != line2 hash

My experience with debugging has shown that one must follow the sequence, especially the XML signature part, as shown on the e-Invoice SDK side. A very good example is the sequence of the XML namespaces (which supper killing), as it can produce a different hash. Only by following the SDK guidelines can one obtain a valid result. Therefore, I believe that LHDN also hashes based on the provided sequence. (As of now, I'm unsure when they gila2 update again. Hope this info can help... happy coding.
