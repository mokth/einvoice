  How to use
  --------------
  xmlString is the is full document (invoice,CN,DN...) xml string without UBLExtensions and Signature 
  UBLSignatureXML.StartGenerate(xmlString);
  string content = UBLSignatureXML.xmlInv.OuterXml;
  content = UBLSignatureXML.XmlDsigC14N(content);
  now can submit the content (full xml with signature).

  
