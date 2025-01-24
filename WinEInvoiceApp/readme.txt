update 24-Jan-2025
- Download your submitted document into your portal and view the raw JSON or XML submitted by youself or your supplier.
- To be able to download, you must supply your own ClientID and Secret ID generated from your portal.
- Auto prompt the Settings Form to fill up the required settings.


- Unzip WinEInvoiceWinApp.zip to any folder. (windows base app)

- run EInvoiceWinApp.exe. 
  1. Load Json or Xml document (without signature) 
  2. Generate Signature
       - all the Signature related data will show up.
  3. Submit Document (only to sandbox api only)
  4. Get Document/Document Detail status.
  5. Save the output to file for checking and comparing purpose.
  6. View document in Form or raw format 

Note:
This app is primarily intended for testing and debugging purposes and is not recommended for production use.


- You can now view the document (JSON/XML) for hashing (before signing).
- You can view the final document.
- You can view the XML sign's QualifyingProperties for the signProp (the most challenging part).
- You can view all the certificate information.
- You can save the file or information.
- Can check the JWT Token payload (check if intermediary is setup properly)
- Can verify the TIN No
