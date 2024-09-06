1. install .Net 8 runtime
2. unzip the eInvoiceApp_Net8.zip in any folder.
3. go to the unzip folder, edit eInvoiceApp.dll.config
    <add key="EInv_CertPath" value="cert fullpath name"/>   eg /home/Projects/postcert.p12"
		<add key="EInv_CertPass" value="cert pass" />		
		<add key="EInv_RespPath" value="output path" />  eg /home/Projects/output"

4. at terminal (at the same folder) run
    dotnet eInvoiceApp.dll <input filename>
  eg  dotnet eInvoiceApp.dll /home/files/myxml.xml   
      the output xml with signature will saved at the output path.
5. Any error, please refer to the log folder.

MOk

