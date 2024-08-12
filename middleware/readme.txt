
- Unzip middleappConsole.zip to any folder. (windows base app)
- Go to the unzip folder, configure the eInvoiceConsole.exe.config file. fill in the valid value. 
- run eInvoiceConsole.exe at cmd prompt. list out all the available commands.

Here is a summary of each command along with its usage examples and descriptions:

1.Verify TIN
  
   eInvoiceConsole.exe verify tin idtype idvalue
   
   -idtype: Type of ID (BRN, NRIC, PASSPORT, ARMY)
   -idvalue: The actual ID value
   -Example: eInvoiceConsole.exe verify C122023333 BRN C233333
   -Response: Written in the output folder

2.Submit Document
   
   eInvoiceConsole.exe submitxml docno docpath
   eInvoiceConsole.exe submitjson docno docpath
   
   -docno: Document number
   -docpath: Full path to the XML or JSON file (without signature)
   -Example XML: eInvoiceConsole.exe submitxml INV00001 c:\data\INV00001.xml
   -Example JSON: eInvoiceConsole.exe submitjson INV00001 c:\data\INV00001.json
   -Response: Written in the output folder

3.Submission
   
   eInvoiceConsole.exe submission submissionid
   
   -submissionid: ID of the submission
   -Example: eInvoiceConsole.exe submission 777SFG9992SSS424
   -Response: Written in the output folder

4.Document
   
   eInvoiceConsole.exe document docUUID
   
   -docUUID: Unique identifier of the document
   -Example: eInvoiceConsole.exe document 34444SFG9992SSS424
   -Response: Written in the output folder

5.Document Detail
   
   eInvoiceConsole.exe documentdetail docUUID
   
   -docUUID: Unique identifier of the document
   -Example: eInvoiceConsole.exe documentdetail 34444SFG9992SSS424
   -Response: Written in the output folder

6.Cancel Document
   
   eInvoiceConsole.exe cancel docUUID reasonString
   
   -docUUID: Unique identifier of the document
   -reasonString: Reason for cancellation
   -Example: eInvoiceConsole.exe cancel 34444SFG9992SSS424 "wrong invoice amount"
   -Response: Written in the output folder

7.Reject Document
   
   eInvoiceConsole.exe reject docUUID reasonString
   
   -docUUID: Unique identifier of the document
   -reasonString: Reason for rejection
   -Example: eInvoiceConsole.exe reject 34444SFG9992SSS424 "wrong invoice amount"
   -Response: Written in the output folder

8.Recent Documents
   
   eInvoiceConsole.exe recent querystringPath
   
   -querystringPath: refer to https://sdk.myinvois.hasil.gov.my/einvoicingapi/#get-recent-documents
   -Example: eInvoiceConsole.exe recent issueDateFrom=2024-08-01T01:59:10Z&issueDateTo=2024-08-10T01:59:10Z&status=valid
   -Response: Written in the output folder

9.Search Documents
   
   eInvoiceConsole.exe search querystringPath
   
   -querystringPath: refer to https://sdk.myinvois.hasil.gov.my/einvoicingapi/#search-documents
   -Example: eInvoiceConsole.exe search issueDateFrom=2024-08-01T01:59:10Z&issueDateTo=2024-08-10T01:59:10Z&status=valid
   -Response: Written in the output folder

10.Login
    
    eInvoiceConsole.exe login
    
    -Example: eInvoiceConsole.exe login
    -Response: Written in the output folder

11.Notification
    
    eInvoiceConsole.exe notification querystringPath
    
    -querystringPath: refer to https://sdk.myinvois.hasil.gov.my/api/06-get-notifications/
    -Example: eInvoiceConsole.exe notification dateFrom=2024-08-01T01:59:10Z&dateTo=2024-08-10T01:59:10Z&status=delivered
    -Response: Written in the output folder

any request please email to mokth@hotmail.com.
