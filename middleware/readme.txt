updated 20-Sep-2024

I can customize my console application to read and process CSV or text files from legacy systems containing Invoice, Credit Note, and Debit Note data. The application will generate JSON or XML formats and submit them to the LHDN API server. Additionally, I can implement functionality to write the API response to a local text file for the legacy system to access. Alternatively, I can assist in updating the API response directly to the legacy system's database, such as DBF or Access.

============================================================
============================================================
Updated 15/Aug/2024 at 5pm local Malaysia Time
please download the zip file again (have missing files).
============================================================
============================================================

- Unzip middleappConsole.zip to any folder. (windows base app)
- Go to the unzip folder, configure the eInvoiceConsole.exe.config file. fill in the valid value. 
- run eInvoiceConsole.exe at cmd prompt. list out all the available commands.
============================================================
- Note: the app only connected to SANDBOX API only!!!!.
============================================================

Here is a summary of each command along with its usage examples and descriptions:

1.Verify TIN
  
   eInvoiceConsole.exe verify tin idtype idvalue
   
   -idtype: Type of ID (BRN, NRIC, PASSPORT, ARMY)
   -idvalue: The actual ID value
   -Example: eInvoiceConsole.exe verify C122023333 BRN C233333
   -Response: Written in the output folder

2.Submit Document
   Submit json or xml document to LHDN
   Pass in the json or xml without signature, the app will generate the signature and submit to LHDN

   eInvoiceConsole.exe submitxml docno docpath
   eInvoiceConsole.exe submitjson docno docpath
   
   -docno: Document number
   -docpath: Full path to the XML or JSON file (without signature)
   -Example XML: eInvoiceConsole.exe submitxml INV00001 c:\data\INV00001.xml
   -Example JSON: eInvoiceConsole.exe submitjson INV00001 c:\data\INV00001.json
   -Response: Written in the output folder

3.Submit Document only
   Submit json or xml document to LHDN (either ver 1.0 or 1.1)
   Pass in the json or xml with or without signature, the app only package and submit to LHDN

   eInvoiceConsole.exe submitonlyxml docno docpath
   eInvoiceConsole.exe submitonlyjson docno docpath
   
   -docno: Document number
   -docpath: Full path to the XML or JSON file (without signature)
   -Example XML: eInvoiceConsole.exe submitxml INV00001 c:\data\INV00001.xml
   -Example JSON: eInvoiceConsole.exe submitjson INV00001 c:\data\INV00001.json
   -Response: Written in the output folder

4.Submission
   Get submission status
   eInvoiceConsole.exe submission submissionid
   
   -submissionid: ID of the submission
   -Example: eInvoiceConsole.exe submission 777SFG9992SSS424
   -Response: Written in the output folder

5.Document
   Get document info
   eInvoiceConsole.exe document docUUID
   
   -docUUID: Unique identifier of the document
   -Example: eInvoiceConsole.exe document 34444SFG9992SSS424
   -Response: Written in the output folder

6.Document Detail
   Get document detail info
   eInvoiceConsole.exe documentdetail docUUID
   
   -docUUID: Unique identifier of the document
   -Example: eInvoiceConsole.exe documentdetail 34444SFG9992SSS424
   -Response: Written in the output folder

7.Cancel Document
   To cancel a document
   eInvoiceConsole.exe cancel docUUID reasonString
   
   -docUUID: Unique identifier of the document
   -reasonString: Reason for cancellation
   -Example: eInvoiceConsole.exe cancel 34444SFG9992SSS424 "wrong invoice amount"
   -Response: Written in the output folder

8.Reject Document
   To request reject
   eInvoiceConsole.exe reject docUUID reasonString
   
   -docUUID: Unique identifier of the document
   -reasonString: Reason for rejection
   -Example: eInvoiceConsole.exe reject 34444SFG9992SSS424 "wrong invoice amount"
   -Response: Written in the output folder

9.Recent Documents
   Get Recent documents 
   eInvoiceConsole.exe recent querystringPath
   
   -querystringPath: refer to https://sdk.myinvois.hasil.gov.my/einvoicingapi/#get-recent-documents
   -Example: eInvoiceConsole.exe recent issueDateFrom=2024-08-01T01:59:10Z&issueDateTo=2024-08-10T01:59:10Z&status=valid
   -Response: Written in the output folder

10.Search Documents
   To search documents
   eInvoiceConsole.exe search querystringPath
   
   -querystringPath: refer to https://sdk.myinvois.hasil.gov.my/einvoicingapi/#search-documents
   -Example: eInvoiceConsole.exe search issueDateFrom=2024-08-01T01:59:10Z&issueDateTo=2024-08-10T01:59:10Z&status=valid
   -Response: Written in the output folder

11.Login
    Peform login, the decode token is save at the respone folder.
    eInvoiceConsole.exe login
    
    -Example: eInvoiceConsole.exe login
    -Response: Written in the output folder

12.Notification
    To get notification
    eInvoiceConsole.exe notification querystringPath
    
    -querystringPath: refer to https://sdk.myinvois.hasil.gov.my/api/06-get-notifications/
    -Example: eInvoiceConsole.exe notification dateFrom=2024-08-01T01:59:10Z&dateTo=2024-08-10T01:59:10Z&status=delivered
    -Response: Written in the output folder

13.  Sign XML document
     generate XML signature and return the XML document with digital signature 

     eInvoiceConsole.exe xmlsign docpath 
     -Example: eInvoiceConsole.exe xmlsign c:\data\invouce.xml
     -Response: Written in the output folder

14.  Sign JSON document
     generate JSON signature and return the JSON document with digital signature 

     eInvoiceConsole.exe jsonsign docpath 
     -Example: eInvoiceConsole.exe jsonsign c:\data\invouce.json
     -Response: Written in the output folder

any request please email to mokth@hotmail.com.
