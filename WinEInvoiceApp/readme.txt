updated 17-10-2024
- You can now generate the e-invoice in PDF format.
- First, submit a JSON document (currently, the PDF only supports JSON documents).
- Get the valid document (with a valid UUID).
- Click "Print Invoice" to generate and display the PDF invoice.
- Alternatively, you can enter the valid UUID and click "Print Invoice."

Note: The procedure for setup is the same as mentioned below.
download at : https://drive.google.com/file/d/112ITWTIRqQbahA93-gR9PMePjO57UKgt/view?usp=sharing


- Unzip WinEInvoiceWinApp.zip to any folder. (windows base app)
- Go to the unzip folder, configure the EInvoiceWinApp.exe.config file. fill in the valid value. 
- run EInvoiceWinApp.exe. 
  1. Load Json or Xml document (without signature) 
  2. Generate Signature
       - all the Signature related data will show up.
  3. Submit Document (only to sandbox api only)
  4. Get Document/Document Detail status.
  5. Save the output to file for checking and comparing purpose.

Note:
This app is primarily intended for testing and debugging purposes and is not recommended for production use.


- You can now view the document (JSON/XML) for hashing (before signing).
- You can view the final document.
- You can view the XML sign's QualifyingProperties for the signProp (the most challenging part).
- You can view all the certificate information.
- You can save the file or information.
- Can check the JWT Token payload (check if intermediary is setup properly)
- Can verify the TIN No
