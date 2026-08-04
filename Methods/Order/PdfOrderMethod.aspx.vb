Imports System.IO
Imports Microsoft.VisualBasic.FileIO
Imports System.Web.Services
Imports OfficeOpenXml
Imports System.Web.Script.Services
Imports System.Web.Script.Serialization
Imports System.Data
Imports System.Data.SqlClient
Imports System.Guid
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Globalization
Imports System.Linq
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.tool.xml
Imports Microsoft.VisualBasic
Imports Newtonsoft.Json
Imports System.Net
Imports System.Net.Mail
Imports System.Net.Http
Imports System.Text
Partial Class Methods_Order_PdfOrderMethod
    Inherits System.Web.UI.Page
    Shared orderCfg As New OrderConfig()
    Shared publicCfg As New PublicConfig()
    Shared printCfg As New PrintConfig()
    Shared jobsheet As New HalperJobSheetRenderer()
    Shared enUS As CultureInfo = New CultureInfo("en-US")
    Public Shared myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Public Class ErrorDetail
        Public Property message As String
        Public Property field As String
    End Class

    Public Class ErrorResponse
        Public Property [error] As ErrorDetail
    End Class

    Public Class SuccessDetail
        Public Property message As String
        Public Property url As String
    End Class

    Public Class SuccessResponse
        Public Property success As SuccessDetail
    End Class

    Private Shared tableStart As String = "<table style='width:100%;border:1px solid black;border-collapse:collapse;margin-bottom:15px;'>"
    Private Shared tableEnd As String = "</table>"

    Private Shared trStart As String = "<tr>"
    Private Shared trEnd As String = "</tr>"

    Private Shared thStart As String = "<th style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;word-wrap:break-word;'>"
    Private Shared thStartColSpan2 As String = "<th colspan='2' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Private Shared thStartColSpan3 As String = "<th colspan='3' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Private Shared thStartColSpan4 As String = "<th colspan='4' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Private Shared thStartColSpan8 As String = "<th colspan='8' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>"
    Private Shared thStartRowSpan2 As String = "<th rowspan='2' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;word-wrap:break-word;'>"
    Private Shared thStartRowSpan3 As String = "<th rowspan='3' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;word-wrap:break-word;'>"
    Private Shared thEnd As String = "</th>"

    Private Shared tdStart As String = "<td style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdStartRowSpan2 As String = "<td rowspan='2' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdStartColSpan2 As String = "<td colspan='2' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdStartColSpan3 As String = "<td colspan='3' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdStartColSpan4 As String = "<td colspan='4' style='text-align:center;height:auto;font-size:8px;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
    Private Shared tdEnd As String = "</td>"

    Private Shared bNotesStart As String = "<b style='margin-left:100px;color:red;'>Notes: "
    Private Shared bNotesEnd As String = "</b>"

    Private Shared spanStart As String = "<span style='font-size:12px;font-weight:bold;'>"
    Private Shared spanEnd As String = "</span>"

    Private Shared queryQtyBlind As String = "SELECT COUNT(*) FROM view_details WHERE Active = 1 {0} {1}"

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CreatePDFOrder(ByVal headerid As String, ByVal action As String) As Object
        Try
            Dim msg As String = ""
            Dim url As String = ""
            Dim fileDirectory As String = ""
            Dim detailData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_details WHERE HeaderId='{0}' AND Active='1'", headerid))

            If detailData.Tables(0).Rows.Count < 1 Then
                Return New With { .warning = true, .message = "Please add item first !"}
            End If

            Dim headerData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_headers WHERE Id='{0}'", headerid))
            Dim status As String = headerData.Tables(0).Rows(0).Item("Status").ToString()
            if headerData.Tables(0).Rows.Count < 1 Then
                Throw New Exception("Order Header not found.")
            End If


            If action = "mail" Then
                If status = "Draft" Or Status = "Cenceled" Then
                    Return New With { .warning = true, .message = "You can't send an email for a draft or canceled order."}
                End If
            End If

            '# --------------------------|| Prepare Generate PDF ||-------------------------------
            Dim orderNo As String = headerData.Tables(0).Rows(0).Item("OrderNo").ToString()
            Dim storeId As String = headerData.Tables(0).Rows(0).Item("StoreId").ToString()
            Dim fileName As String = (String.Format("-ORDER-{0}-{1}.pdf", orderNo, storeId)).Replace(" ", "")

            If action = "preview" Or action = "download" Then
                fileDirectory = HttpContext.Current.Server.MapPath("~/file/order/preview")

                If action = "preview" Then
                    HttpContext.Current.Session("printPreview") = fileName
                    msg = "Print page is successfully prepared. <br> Click <b>OK</b> to open it."
                    url = "/order/preview"
                End If

            
                If action = "download" Then
                    msg = "Your download is ready. Click <b>OK</b> if download does not start automatically."
                    url = "/Methods/Order/Handler/DowloadPDFOrder.ashx?file=" & fileName & "&keyDownload=invoice"
                End If

                Dim currentDomain As String = HttpContext.Current.Request.Url.Host.ToLower()
                ' If currentDomain.Contains("onlineorder.au") Then
                '     printCfg.CreatePDFOrder(headerid, fileDirectory, fileName)
                ' Else
                    Dim ResPDF As String = ThisCreatePDFOrder(headerid, fileDirectory, fileName)
                    If Not ResPDF = "200" Then
                        Throw New Exception(ResPDF)
                    End If
                ' End If

            End If


            If action = "mail" Or action = "submit" Then
                fileDirectory = HttpContext.Current.Server.MapPath("~/file/order/mail")
                
                If action = "submit" Then
                    msg = "This order was submitted successfully"
                End If

                If action = "mail" Then
                    msg = "This order was sent successfully"
                End If

                Dim currentDomain As String = HttpContext.Current.Request.Url.Host.ToLower()

                Dim ResPDF As String = ThisCreatePDFOrder(headerid, fileDirectory, fileName)
                If Not ResPDF = "200" Then
                    Throw New Exception(ResPDF)
                End If

                ' Hanya kirim email jika domain sesuai
                ' printCfg.CreatePDFOrder(headerid, fileDirectory, fileName)
                ' If currentDomain.Contains("onlineorder.au") Then
                    ' publicCfg.MailOrder(headerid, fileDirectory)
                ' Else
                    Dim Res As String = MailSubmitOrder(headerid, fileDirectory)
                    IF Not Res = "200" Then
                        Throw New Exception(Res)
                    End If
                ' End If
            End If


            Return New With {.success = true, .message = msg, .url = url}

        Catch ex As Exception
            Return New With {.error = true, .message = ex.Message}
        End Try
    End Function

    Private Shared Function MailSubmitOrder(headerid As String, directory As String) As String
        Try
            Dim OrderData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM view_order_headers WHERE Id = '{0}' AND OrderType IN ('Blinds', 'Door and Window') ", headerid))
            If OrderData.Tables(0).Rows.Count = 0 Then Return "invalid orders"

            Dim CustomerId As String = OrderData.Tables(0).Rows(0).Item("CustomerId").ToString()
            Dim OrderNumber As String = OrderData.Tables(0).Rows(0).Item("OrderNumber").ToString()
            Dim OrderName As String = OrderData.Tables(0).Rows(0).Item("OrderName").ToString()
            Dim Delivery As String = OrderData.Tables(0).Rows(0).Item("Delivery").ToString()

            Dim AppId As String = publicCfg.GetItemData(String.Format("SELECT ApplicationId FROM CustomerLogins WHERE CustomerId = '{0}'", CustomerId))
            Dim mailData As DataSet = publicCfg.GetListData(String.Format("SELECT * FROM Mailings WHERE ApplicationId = '{0}' AND Name = 'Submit Order Blinds' AND Active = '1' ", AppId))
            Dim mailDevelopment As DataSet = publicCfg.GetListData("SELECT * From MailConfiguration WHERE Id='FADBA62C-2072-4501-8901-5E071BBF5E67'")

            If mailData.Tables(0).Rows.Count = 0 Then Return "invalid mailings"
            Dim CustomerName As String = publicCfg.GetItemData(String.Format("SELECT Name FROM Customers WHERE Id = '{0}'", CustomerId))
            Dim Mail As String = publicCfg.GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId = '" + CustomerId + "' AND [Primary] = 1")
            If Mail = "" Then Return String.Format("please set primary contact on customer : {0}", CustomerName)

            Dim mailServer As String = mailData.Tables(0).Rows(0)("Server").ToString()
            Dim mailHost As String = mailData.Tables(0).Rows(0)("Host").ToString()
            Dim mailPort As Integer = CInt(mailData.Tables(0).Rows(0)("Port"))
            Dim mailAccount As String = mailData.Tables(0).Rows(0)("Account").ToString()
            Dim mailPassword As String = mailData.Tables(0).Rows(0)("Password").ToString()
            Dim mailAlias As String = mailData.Tables(0).Rows(0)("Alias").ToString()
            Dim mailTo As String = mailData.Tables(0).Rows(0)("To").ToString()
            Dim mailCc As String = mailData.Tables(0).Rows(0)("Cc").ToString()
            Dim mailBcc As String = mailData.Tables(0).Rows(0)("Bcc").ToString()
            Dim mailNetworkCredentials As Boolean = CBool(mailData.Tables(0).Rows(0)("NetworkCredentials"))
            Dim mailDefaultCredentials As Boolean = CBool(mailData.Tables(0).Rows(0)("DefaultCredentials"))
            Dim mailEnableSSL As Boolean = CBool(mailData.Tables(0).Rows(0)("EnableSSL"))
   
            Dim mailBody As String ="Thank you for your order."
            mailBody += "<br />"
            mailBody += "This is an automated message confirming the receipt of your order. Your order has been successfully registered and has been forwarded directly to our production system for processing. Please note that due to this streamlined process, we regret to inform you that we are unable to accept cancellations or modifications for this order. For any inquiries or assistance, kindly contact our office.<br /><b>Please do not reply to this email as it is unattended. We appreciate your understanding and trust in our products & services</b>."
            mailBody += "<br /><br />"
            mailBody += "Customer Name : " & CustomerName
            mailBody += "<br />"
            mailBody += "Order Number : " & OrderNumber
            mailBody += "<br />"
            mailBody += "Order Name : " & OrderName
            mailBody += "<br /><br />"
            mailBody += "Detail order as attached PDF."

            mailBody += "<br /><br />"
            mailBody += "Kind regards,"
            mailBody += "<br /><br />"

            mailBody += "<br /><br />"
            mailBody += "<b>Sunlight Products Pty Ltd</b>"


            Using myMail As New MailMessage()
                Dim fileName As String = Trim("-ORDER-" & OrderNumber.Replace(" ", "") & "-" & CustomerId & ".pdf")
                myMail.Subject = "Order No " & OrderNumber & " | " & OrderName & " Confirmed"
                myMail.From = New MailAddress(mailServer, mailAlias)
                myMail.Body = mailBody
                myMail.IsBodyHtml = True

                If mailDevelopment.Tables.Count > 0 Then
                    Dim mDev As String = mailDevelopment.Tables(0).Rows(0).Item("To").ToString()
                    Dim activeDev As String = mailDevelopment.Tables(0).Rows(0).Item("Active").ToString()

                    If activeDev = "True" Or activeDev = "1" Then
                        myMail.To.Add(mdev)
                    Else
                        myMail.To.Add(Mail)
                        If Not String.IsNullOrEmpty(mailTo) Then myMail.To.Add(mailTo)
                        If Not String.IsNullOrEmpty(mailCc) Then myMail.CC.Add(mailCc)
                        If Not String.IsNullOrEmpty(mailBcc) Then
                            Dim BccList() As String = mailBcc.Split(";")
                            Dim ThisMail As String = ""
                            For Each ThisMail In BccList
                                myMail.Bcc.Add(ThisMail)
                            Next
                        End If
                    End If
                Else
                    myMail.To.Add(Mail)
                    If Not String.IsNullOrEmpty(mailTo) Then myMail.To.Add(mailTo)
                    If Not String.IsNullOrEmpty(mailCc) Then myMail.CC.Add(mailCc)
                    If Not String.IsNullOrEmpty(mailBcc) Then
                        Dim BccList() As String = mailBcc.Split(";")
                        Dim ThisMail As String = ""
                        For Each ThisMail In BccList
                            myMail.Bcc.Add(ThisMail)
                        Next
                    End If
                End If

                Dim fullPath = Path.Combine(directory, fileName)
                Using fs As New FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read)
                Dim attach As New Attachment(fs, fileName)
                myMail.Attachments.Add(attach)
                    Using smtpClient As New SmtpClient(mailHost, mailPort)
                        smtpClient.EnableSsl = mailEnableSSL
                        smtpClient.UseDefaultCredentials = mailDefaultCredentials
                        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network

                        If mailNetworkCredentials Then
                            smtpClient.Credentials = New NetworkCredential(mailAccount, mailPassword)
                        ElseIf mailDefaultCredentials Then
                            smtpClient.UseDefaultCredentials = True
                        Else
                            smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials
                        End If

                        smtpClient.Send(myMail)
                    End Using
                End Using
            End Using

            Return "200"
        Catch ex As Exception
            Dim errorMessage As String = "Failure sending mail. " & ex.Message
            If ex.InnerException IsNot Nothing Then
                errorMessage &= " Inner Exception: " & ex.InnerException.Message
            End If
            Return errorMessage
        End Try
    End Function
    
    Private Shared Function ThisCreatePDFOrder(headerid As String, directory As String, filename As String) As String
        Try
            Dim result As String = String.Empty
            result = Print_HeaderTemplate(headerid)

            'ALUMINIUM
            result += Print_AluminiumBlinds(headerid)

            'CELLULAR
            result += Print_CellularCellora(headerid)
            result += Print_CellularGalaxy(headerid)
            result += Print_CellularPotrait(headerid)

            'LUMEN
            result += Print_LumenChain(headerid)
            result += Print_LumenMotorised(headerid)

            'PANEL GLIDES
            result += Print_PanelGlides(headerid)

            'PANEL GLIDES
            result += Print_GlobalPanelGlides(headerid)

            'ROMAN BLINDS
            result += Print_RomanBlinds(headerid)

            'GLOBAL ROMAN BLINDS
            result += Print_GlobalRomanBlinds(headerid)

            'VENETIAN
            result += Print_VenetianBlinds(headerid)

            'PELMETS BLINDS
            result += Print_Pelmets(headerid)

            'VERI SHADES
            result += Print_Verishades(headerid)
            result += Print_Verishades_Track(headerid)
            result += Print_Verishades_Slat(headerid)

            'VERTICAL
            result += Print_Vertical_Complete(headerid)
            result += Print_Vertical_Track(headerid)
            result += Print_Vertical_Slat(headerid)

            'VERTICAL
            result += Print_GlobalVertical_Complete(headerid)
            result += Print_GlobalVertical_Track(headerid)
            result += Print_GlobalVertical_Slat(headerid)

            'ROLLER
            result += Print_Roller_SkinOnly(headerid)
            result += PrintPDFRollerStandard(headerid)
            result += Print_Roller_Motorised(headerid)
            result += Print_Cassette(headerid)
            result += Print_CassetteMotorised(headerid)

            '#Door
            result += Print_Door(headerid)
            result += Print_Window(headerid)

            'ROLLER
            result += PrintPDFGlobalGearReduction(headerid)
            result += Print_Global_Roller_Motorised(headerid)

            Using stream As FileStream = New FileStream(directory + "/" + filename, FileMode.Create)
                Dim pdfDoc As Document = New Document(PageSize.A4.Rotate)
                Dim writer As PdfWriter = PdfWriter.GetInstance(pdfDoc, stream)
                pdfDoc.Open()
                Dim sr As StringReader = New StringReader(result)
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr)
                pdfDoc.NewPage()
                pdfDoc.Close()
                stream.Close()
            End Using
            Return "200"
        Catch ex As Exception
            Return "ERROR ThisCreatePDFOrder: " & ex.Message ' biar kelihatan errornya
        End Try
    End Function

    Private Shared Function Print_HeaderTemplate(HeaderId As String) As String
        Dim result As String = String.Empty

        Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id = '" + HeaderId + "'")

        Dim orderNo As String = headerData.Tables(0).Rows(0).Item("OrderNo").ToString()
        Dim orderCust As String = headerData.Tables(0).Rows(0).Item("OrderCust").ToString()
        Dim delivery As String = headerData.Tables(0).Rows(0).Item("Delivery").ToString()
        Dim createdBy As String = headerData.Tables(0).Rows(0).Item("UserName").ToString()
        Dim note As String = headerData.Tables(0).Rows(0).Item("Note").ToString()
        Dim createdDate As String = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CreatedDate")).ToString("dd MMM yyyy")
        Dim status As String = headerData.Tables(0).Rows(0).Item("Status").ToString()
        Dim storeId As String = headerData.Tables(0).Rows(0).Item("StoreId").ToString()
        Dim storeName As String = headerData.Tables(0).Rows(0).Item("StoreName").ToString()

        Dim request As HttpRequest = HttpContext.Current.Request
        Dim baseUrl As String = request.Url.Scheme & "://" & request.Url.Authority & request.ApplicationPath.TrimEnd("/"c)

        Dim Path As String = System.Web.HttpContext.Current.Server.MapPath("~/Content/static/new-icon.png")

        result += "<table style='width:100%;margin-bottom:10px;margin-top:25px;font-size:smaller;'>"
        result += trStart
        result += "<td style='vertical-align:top;width:40%;font-size:small;'>"
        result += "<img width='100%' src='"& Path &"' alt='Your Logo'/>"
        ' result += String.Format("<img width='100%' src='{0}' alt='Your Logo'/>", Path)
        result += "<br />"
        result += "<p style='font-size:small;'>"
        result += "<b>Sunlight Products Pty Ltd</b>"
        result += "<br />"
        result += "ABN 72 953 837 890"
        result += "<br /><br />"
        result += "Phone: 02 9688 1555"
        result += "<br />"
        result += "Fax: 02 9631 7555"
        result += "</p>"

        result += tdEnd

        result += "<td style='vertical-align:top;width:60%;font-size:small;'>"
        result += "<table style='width:100%;font-size:smaller;'>"
        result += trStart
        result += "<td style='vertical-align:top;font-size:small;'>"
        result += "<table style='width:100%;font-size:small;'>"

        result += trStart
        result += "<td style='width:170px;font-size:small;'>Store Name</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & storeName & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;font-size:large;'>Order Number</td>"
        result += "<td style='width:10px;font-size:large;'>:</td>"
        result += "<td style='font-size:large;'><b>" & orderNo & "</b>" & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;font-size:large;'>Reference</td>"
        result += "<td style='width:10px;font-size:large;'>:</td>"
        result += "<td style='font-size:large;'><b>" & orderCust & "</b>" & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;font-size:small;'>Delivery / Pick Up</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & delivery & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;font-size:small;'>Created</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & createdBy & " on " & createdDate & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;'>Status</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & status & tdEnd
        result += trEnd

        result += trStart
        result += "<td style='width:170px;'>Total Quantity Order</td>"
        result += "<td style='width:10px;font-size:small;'>:</td>"
        result += "<td style='font-size:small;'>" & publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND Active=1") & " Piece" & tdEnd
        result += trEnd
        result += tableEnd
        result += tdEnd
        result += trEnd

        'START NOTE
        If Not note = "" Then
            result += trStart
            result += "<td style='vertical-align:top;font-size:smaller;'>"
            result += "<table style='width:100%;font-size:smaller;'>"
            result += trStart
            result += "<td>Note :</td>"
            result += trEnd

            result += trStart
            result += tdStart & note & tdEnd
            result += trEnd

            result += tableEnd
            result += tdEnd
            result += trEnd
        End If
        'END NOTE

        'START DESCRIPTION
        result += trStart
        result += "<td style='vertical-align:top;font-size:smaller;'>"
        result += "<table style='width:100%;font-size:smaller;'>"
        result += trStart
        result += "<td>Description Quantity :</td>"
        result += trEnd

        result += trStart
        result += tdStart & BindDescOrderItem(HeaderId) & tdEnd
        result += trEnd

        result += tableEnd
        result += tdEnd
        result += trEnd
        'END DESCRIPTION

        result += tableEnd
        result += tdEnd
        result += trEnd
        result += tableEnd

        Return result
    End Function

    Private Shared Function Print_AluminiumBlinds(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Aluminium Blinds' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "ALUMINIUM BLINDS" & spanEnd
                result += tableStart
                result += trStart
                result += thStartRowSpan3 & "No" & thEnd
                result += thStartRowSpan3 & "ID" & thEnd
                result += thStartRowSpan3 & "Qty" & thEnd
                result += thStartRowSpan3 & "Product" & thEnd
                result += thStartRowSpan3 & "Location" & thEnd
                result += thStartRowSpan3 & "Mounting" & thEnd
                result += thStartRowSpan3 & "Width" & thEnd
                result += thStartRowSpan3 & "Drop" & thEnd
                result += thStartRowSpan3 & "Bracket" & thEnd
                result += thStartRowSpan3 & "Bottomrail Hold Down Clip (Clear Plastic)" & thEnd
                result += "<th colspan='2' rowspan='2' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>" & "Control" & thEnd
                result += "<th colspan='8' style='text-align:center;height:auto;font-size:8px;color:white;background-color:#007ACC;word-wrap:break-word;border:1px solid black;border-collapse:collapse;padding-top:5px;padding-bottom:5px;'>" & "Cut Out" & thEnd               
                result += trEnd

                result += trStart
                    result += thStartColSpan2 & "Top LHS" & thEnd
                    result += thStartColSpan2 & "Top RHS" & thEnd
                    result += thStartColSpan2 & "Bottom LHS" & thEnd
                    result += thStartColSpan2 & "Bottom RHS" & thEnd
                result += trEnd 

                result += trStart
                result += thStart & "Position" & thEnd
                result += thStart & "Length" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim LHSWidth_Top As String = thisData.Tables(0).Rows(i).Item("LHSWidth_Top").ToString()
                    Dim LHSHeight_Top As String = thisData.Tables(0).Rows(i).Item("LHSHeight_Top").ToString()
                    Dim RHSWidth_Top As String = thisData.Tables(0).Rows(i).Item("RHSWidth_Top").ToString()
                    Dim RHSHeight_Top As String = thisData.Tables(0).Rows(i).Item("RHSHeight_Top").ToString()
                    Dim LHSWidth_Bottom As String = thisData.Tables(0).Rows(i).Item("LHSWidth_Bottom").ToString()
                    Dim LHSHeight_Bottom As String = thisData.Tables(0).Rows(i).Item("LHSHeight_Bottom").ToString()
                    Dim RHSWidth_Bottom As String = thisData.Tables(0).Rows(i).Item("RHSWidth_Bottom").ToString()
                    Dim RHSHeight_Bottom As String = thisData.Tables(0).Rows(i).Item("RHSHeight_Bottom").ToString()

                    If LHSWidth_Top = "0" Then : LHSWidth_Top = "" : End If
                    If LHSHeight_Top = "0" Then : LHSHeight_Top = "" : End If
                    If RHSWidth_Top = "0" Then : RHSWidth_Top = "" : End If
                    If RHSHeight_Top = "0" Then : RHSHeight_Top = "" : End If
                    If LHSWidth_Bottom = "0" Then : LHSWidth_Bottom = "" : End If
                    If LHSHeight_Bottom = "0" Then : LHSHeight_Bottom = "" : End If
                    If RHSWidth_Bottom = "0" Then : RHSWidth_Bottom = "" : End If
                    If RHSHeight_Bottom = "0" Then : RHSHeight_Bottom = "" : End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketOption").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & tdEnd
                    result += tdStart & LHSWidth_Top & tdEnd
                    result += tdStart & LHSHeight_Top & tdEnd
                    result += tdStart & RHSWidth_Top & tdEnd
                    result += tdStart & RHSHeight_Top & tdEnd
                    result += tdStart & LHSWidth_Bottom & tdEnd
                    result += tdStart & LHSHeight_Bottom & tdEnd
                    result += tdStart & RHSWidth_Bottom & tdEnd
                    result += tdStart & RHSHeight_Bottom & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ALUMINIUM BLINDS ERROR CREATE PDF"
        End Try
        Return result
    End Function

    Private Shared Function Print_CellularCellora(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Cellular Blinds' AND BlindName='Cellora' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='11' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "CELLULAR CELLORA" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Side" & thEnd
                result += thStart & "Chain Length" & thEnd
                result += thStart & "Hold Down Bracket" & thEnd
                result += thStart & "Cut Out" & thEnd
                result += trEnd


                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("DoorCutOut").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF CELLULAR CELLORA"
        End Try
        Return result
    End Function

    Private Shared Function Print_CellularGalaxy(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Cellular Blinds' AND BlindName='Galaxy' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='11' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "CELLULAR GALAXY" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Fabric B" & thEnd
                result += thStart & "Cord Type" & thEnd
                result += thStart & "Side" & thEnd
                result += thStart & "Chain Length" & thEnd
                result += thStart & "Hold Down Bracket" & thEnd
                result += thStart & "Cut Out" & thEnd
                result += trEnd


                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricNameB").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MaterialCord").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("DoorCutOut").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF CELLORA GALAXY"
        End Try
        Return result
    End Function

    Private Shared Function Print_CellularPotrait(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Cellular Blinds' AND BlindName='Potrait' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='11' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "CELLULAR POTAIT" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Side" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Motor type" & thEnd
                result += thStart & "Motor Extra" & thEnd
                result += thStart & "Chain Length" & thEnd
                result += thStart & "Hold Down Bracket" & thEnd
                result += thStart & "Additional" & thEnd
                result += thStart & "Cut Out" & thEnd
                result += trEnd


                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorStyle").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("AdditionalMotor").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("DoorCutOut").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF CELLORA POTAIT"
        End Try
        Return result
    End Function

    Private Shared Function Print_LumenChain(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Lumen' AND ControlType='Chain' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='12' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "LUMEN BLIND CHAINED" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Side" & thEnd
                result += thStart & "Chained" & thEnd
                result += thStart & "Headbox" & thEnd
                result += thStart & "Insert" & thEnd
                result += thStart & "Bottom Rail Colour" & thEnd
                result += thStart & "Butting Blind" & thEnd
                result += trEnd

                
                
                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1

                    Dim ChainLength As String = thisData.Tables(0).Rows(i).Item("ChainLength").ToString()
                    Dim ChainColour As String = thisData.Tables(0).Rows(i).Item("ChainColour").ToString()
                    Dim Chainded As String = String.Format("{0} - {1}", ChainLength, ChainColour)
    
                    Dim TrackType As String = thisData.Tables(0).Rows(i).Item("TrackType").ToString()
                    Dim TrackColour As String = thisData.Tables(0).Rows(i).Item("TrackColour").ToString()
                    Dim Headbox As String = String.Format("{0} - {1}", TrackType, TrackColour)


                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & Chainded & tdEnd
                    result += tdStart & Headbox & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Fitting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SwipelColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SideBySide").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF LUMEN CHAIN"
        End Try
        Return result
    End Function

    Private Shared Function Print_LumenMotorised(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Lumen' AND ControlType='Motorised' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='12' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "LUMEN BLIND MOTORISED" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Side" & thEnd
                result += thStart & "Motor Option" & thEnd
                result += thStart & "Remote Option" & thEnd
                result += thStart & "Charger Option" & thEnd
                result += thStart & "Headbox" & thEnd
                result += thStart & "Insert" & thEnd
                result += thStart & "Bottom Rail Colour" & thEnd
                result += thStart & "Butting Blind" & thEnd
                result += trEnd

                
                
                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1

                    Dim ChainLength As String = thisData.Tables(0).Rows(i).Item("ChainLength").ToString()
                    Dim ChainColour As String = thisData.Tables(0).Rows(i).Item("ChainColour").ToString()
                    Dim Chainded As String = String.Format("{0} - {1}", ChainLength, ChainColour)
    
                    Dim TrackType As String = thisData.Tables(0).Rows(i).Item("TrackType").ToString()
                    Dim TrackColour As String = thisData.Tables(0).Rows(i).Item("TrackColour").ToString()
                    Dim Headbox As String = String.Format("{0} - {1}", TrackType, TrackColour)


                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorStyle").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorRemote").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorCharger").ToString() & tdEnd
                    result += tdStart & Headbox & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Fitting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SwipelColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SideBySide").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF LUMEN CHAIN"
        End Try
        Return result
    End Function

    Private Shared Function Print_PanelGlides(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Panel Glides' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='19' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "PANEL GLIDES" & spanEnd
                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Fabric" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Layout" & thEnd
                result += thStartRowSpan2 & "No Panel" & thEnd
                result += thStartColSpan2 & "Track" & thEnd
                result += thStartColSpan3 & "Wand" & thEnd
                result += thStartRowSpan2 & "Bottom Rail" & thEnd
                ' result += thStartRowSpan2 & "Batten" & thEnd
                ' result += thStartRowSpan2 & "Batten Colour" & thEnd
                ' result += thStartRowSpan2 & "Fitting" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Type" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Position" & thEnd
                result += thStart & "Length" & thEnd
                result += thStart & "Colour" & thEnd
                result += trEnd


                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Layout").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("NumOfPanel").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("Batten").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("BattenColour").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("Fitting").ToString() & tdEnd
                    result += trEnd


                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF PANEL GLIDES"
        End Try
        Return result
    End Function

    Private Shared Function Print_RomanBlinds(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roman Blinds' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='18' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "ROMAN BLINDS" & spanEnd
                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Fabric" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Control Position" & thEnd
                result += thStartColSpan3 & "Chain" & thEnd
                result += thStartColSpan2 & "Cord" & thEnd
                result += thStartRowSpan2 & "Batten Colour" & thEnd
                result += thStartRowSpan2 & "Plastic Colour" & thEnd
                result += thStartRowSpan2 & "Cleat" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Material" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Length" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Length" & thEnd
                result += trEnd


                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MaterialChain").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("CordColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("CordLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BattenColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("AcornPlasticColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Cleat").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF ROMAN BLINDS"
        End Try
        Return result
    End Function

    Private Shared Function Print_Pelmets(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Pelmet' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='18' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "PELMETS" & spanEnd
                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Fabric" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                ' result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Pelmet Over" & thEnd
                ' result += thStartColSpan3 & "Chain" & thEnd
                result += thStartColSpan2 & "Hand Returns" & thEnd
                ' result += thStartRowSpan2 & "Batten Colour" & thEnd
                ' result += thStartRowSpan2 & "Plastic Colour" & thEnd
                ' result += thStartRowSpan2 & "Cleat" & thEnd
                result += trEnd

                result += trStart
                ' result += thStart & "Material" & thEnd
                ' result += thStart & "Colour" & thEnd
                ' result += thStart & "Length" & thEnd
                result += thStart & "Left" & thEnd
                result += thStart & "Right" & thEnd
                result += trEnd


                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PelmetType").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("MaterialChain").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("ChainColour").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PelmetReturnSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PelmetReturnSize2").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("BattenColour").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("AcornPlasticColour").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("Cleat").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF ROMAN BLINDS"
        End Try
        Return result
    End Function

    Private Shared Function Print_VenetianBlinds(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName ='Venetian Blinds' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='25' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "VENETIAN BLINDS" & spanEnd
                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Bracket" & thEnd
                result += thStartRowSpan2 & "Hold Down" & thEnd
                result += thStartColSpan2 & "Control" & thEnd
                result += thStartColSpan3 & "Pelmet" & thEnd
                result += thStartColSpan2 & "Pelmet Return" & thEnd
                result += thStartColSpan2 & "Top LHS" & thEnd
                result += thStartColSpan2 & "Top RHS" & thEnd
                result += thStartColSpan2 & "Bottom LHS" & thEnd
                result += thStartColSpan2 & "Bottom RHS" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Position Lift, Tilt" & thEnd
                result += thStart & "Length" & thEnd
                result += thStart & "Type" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Size" & thEnd
                result += thStart & "Left" & thEnd
                result += thStart & "Right" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Heigth" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim BlindName As String = thisData.Tables(0).Rows(i).Item("BlindName").ToString()

                    Dim PelmetType As String = thisData.Tables(0).Rows(i).Item("PelmetType").ToString()
                    Dim PelmetWidth As String = thisData.Tables(0).Rows(i).Item("PelmetWidth").ToString()
                    Dim PelmetSize As String = thisData.Tables(0).Rows(i).Item("PelmetSize").ToString()
                    Dim PelmetReturnSize As String = thisData.Tables(0).Rows(i).Item("PelmetReturnSize").ToString()
                    Dim PelmetReturnSize2 As String = thisData.Tables(0).Rows(i).Item("PelmetReturnSize2").ToString()

                    If InStr(BlindName, "Alum") > 0 Then
                        PelmetType = ""
                        PelmetWidth = ""
                        PelmetSize = ""
                        PelmetReturnSize = ""
                        PelmetReturnSize2 = ""
                    End IF

                    
                    Dim LHSWidth_Top As String = thisData.Tables(0).Rows(i).Item("LHSWidth_Top").ToString()
                    Dim LHSHeight_Top As String = thisData.Tables(0).Rows(i).Item("LHSHeight_Top").ToString()
                    Dim RHSWidth_Top As String = thisData.Tables(0).Rows(i).Item("RHSWidth_Top").ToString()
                    Dim RHSHeight_Top As String = thisData.Tables(0).Rows(i).Item("RHSHeight_Top").ToString()
                    Dim LHSWidth_Bottom As String = thisData.Tables(0).Rows(i).Item("LHSWidth_Bottom").ToString()
                    Dim LHSHeight_Bottom As String = thisData.Tables(0).Rows(i).Item("LHSHeight_Bottom").ToString()
                    Dim RHSWidth_Bottom As String = thisData.Tables(0).Rows(i).Item("RHSWidth_Bottom").ToString()
                    Dim RHSHeight_Bottom As String = thisData.Tables(0).Rows(i).Item("RHSHeight_Bottom").ToString()

                    If LHSWidth_Top = "0" Then : LHSWidth_Top = "" : End If
                    If LHSHeight_Top = "0" Then : LHSHeight_Top = "" : End If
                    If RHSWidth_Top = "0" Then : RHSWidth_Top = "" : End If
                    If RHSHeight_Top = "0" Then : RHSHeight_Top = "" : End If
                    If LHSWidth_Bottom = "0" Then : LHSWidth_Bottom = "" : End If
                    If LHSHeight_Bottom = "0" Then : LHSHeight_Bottom = "" : End If
                    If RHSWidth_Bottom = "0" Then : RHSWidth_Bottom = "" : End If
                    If RHSHeight_Bottom = "0" Then : RHSHeight_Bottom = "" : End If

                    Dim CPosition As String = thisData.Tables(0).Rows(i).Item("ControlPosition").ToString()
                    If CPosition.Contains("|") Then
                        CPosition = CPosition.Replace("|", ", ")
                    End If

                    Dim CLength As String = thisData.Tables(0).Rows(i).Item("ControlLength").ToString()
                    If String.IsNullOrEmpty(CLength) Or CLength = "0" Then CLength = "Standard"

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketOption").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & CPosition & tdEnd
                    result += tdStart & CLength & tdEnd
                    result += tdStart & PelmetType & tdEnd
                    result += tdStart & PelmetWidth & tdEnd
                    result += tdStart & PelmetSize & tdEnd
                    result += tdStart & PelmetReturnSize & tdEnd
                    result += tdStart & PelmetReturnSize2 & tdEnd
                    result += tdStart & LHSWidth_Top & tdEnd
                    result += tdStart & LHSHeight_Top & tdEnd
                    result += tdStart & RHSWidth_Top & tdEnd
                    result += tdStart & RHSHeight_Top & tdEnd
                    result += tdStart & LHSWidth_Bottom & tdEnd
                    result += tdStart & LHSHeight_Bottom & tdEnd
                    result += tdStart & RHSWidth_Bottom & tdEnd
                    result += tdStart & RHSHeight_Bottom & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF VENETIAN BLINDS"
        End Try
        Return result
    End Function

    Private Shared Function Print_Verishades(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Veri Shades' AND BlindName='Single' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='14' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "SINGLE VERI SHADES" & spanEnd
                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Fabric" & thEnd
                result += thStartRowSpan2 & "Stack" & thEnd
                result += thStartColSpan2 & "Track" & thEnd
                result += thStartColSpan2 & "Wand" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Type" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Size" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & "mm" & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF SINGLE VERI SHADES"
        End Try
        Return result
    End Function

    Private Shared Function Print_Verishades_Track(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Veri Shades' AND BlindName='Track Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='11' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "VERI SHADES TRACK ONLY" & spanEnd

                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Stack" & thEnd
                result += thStartColSpan2 & "Track" & thEnd
                result += thStartColSpan2 & "Wand" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Type" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Size" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ID").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF TRACK ONLY VERI SHADES"
        End Try
        Return result
    End Function

    Private Shared Function Print_Verishades_Slat(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Veri Shades' AND BlindName='Slat Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='8' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;'>"
                result += spanStart & "VERI SHADES SLAT ONLY" & spanEnd

                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Blind Size" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim blindSize As String = "No"
                    If thisData.Tables(0).Rows(i).Item("BlindSize").ToString() = "1" Then
                        blindSize = "Yes"
                    End If
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ID").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & blindSize & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF SKIN ONLY VERI SHADES"
        End Try
        Return result
    End Function

    Private Shared Function Print_Vertical_Complete(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Vertical Blinds' AND BlindName='Complete' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='21' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "COMPLETE VERTICAL BLIND" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Fabric/Slat Size" & thEnd
                result += thStart & "Stack" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Chain/Wand Colour" & thEnd
                result += thStart & "Control Length" & thEnd
                result += thStart & "Track" & thEnd
                result += thStart & "Brackets" & thEnd
                result += thStart & "Bracket Colour" & thEnd
                result += thStart & "Hanger Type" & thEnd
                result += thStart & "Bottom" & thEnd
                result += thStart & "Insert In Track" & thEnd
                result += thStart & "Sloper" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim controlType As String = thisData.Tables(0).Rows(i).Item("ControlType").ToString()

                    Dim chainwandColour As String = thisData.Tables(0).Rows(i).Item("WandColour").ToString()
                    Dim chainwandLength As String = thisData.Tables(0).Rows(i).Item("WandLength").ToString()
                    If controlType = "Chain" Then
                        chainwandColour = thisData.Tables(0).Rows(i).Item("ChainColour").ToString()
                        chainwandLength = thisData.Tables(0).Rows(i).Item("ChainLength").ToString()
                    End If

                    Dim insertInTrack As String = "No"
                    Dim sloper As String = "No"
                    If thisData.Tables(0).Rows(i).Item("InsertInTrack").ToString() = "1" Then
                        insertInTrack = "Yes"
                    End If
                    If thisData.Tables(0).Rows(i).Item("Sloper").ToString() = "1" Then
                        sloper = "Yes"
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SlatSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & chainwandColour & tdEnd
                    result += tdStart & chainwandLength & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketOption").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & insertInTrack & tdEnd
                    result += tdStart & sloper & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF VERTICAL COMPLETE"
        End Try
        Return result
    End Function

    Private Shared Function Print_Vertical_Track(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Vertical Blinds' AND BlindName='Track Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='19' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "VERTICAL TRACK ONLY" & spanEnd

                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Slat" & thEnd
                result += thStart & "Stack" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Chain/Wand" & thEnd
                result += thStart & "Length/Size" & thEnd
                result += thStart & "Track" & thEnd
                result += thStart & "Brackets" & thEnd
                result += thStart & "Bracket Colour" & thEnd
                result += thStart & "Hanger Type" & thEnd
                result += thStart & "Bottom" & thEnd
                result += thStart & "Insert In Track" & thEnd
                result += thStart & "Sloper" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim controlType As String = thisData.Tables(0).Rows(i).Item("ControlType").ToString()

                    Dim chainwandColour As String = thisData.Tables(0).Rows(i).Item("WandColour").ToString()
                    Dim chainwandLength As String = thisData.Tables(0).Rows(i).Item("WandLength").ToString()
                    If controlType = "Chain" Then
                        chainwandColour = thisData.Tables(0).Rows(i).Item("ChainColour").ToString()
                        chainwandLength = thisData.Tables(0).Rows(i).Item("ChainLength").ToString()
                    End If

                    Dim insertInTrack As String = "No"
                    Dim sloper As String = "No"
                    If Not thisData.Tables(0).Rows(i).Item("InsertInTrack").ToString() = "False" OR thisData.Tables(0).Rows(i).Item("InsertInTrack").ToString() = "0" Then
                        insertInTrack = "Yes"
                    End If
                    If Not thisData.Tables(0).Rows(i).Item("Sloper").ToString() = "False" OR thisData.Tables(0).Rows(i).Item("Sloper").ToString() = "0" Then
                        sloper = "Yes"
                    End If

                    Dim slat As String = thisData.Tables(0).Rows(i).Item("SlatSize").ToString()
                    ' slat += " - "
                    ' slat += thisData.Tables(0).Rows(i).Item("SlatQty").ToString()

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & slat & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & chainwandColour & tdEnd
                    result += tdStart & chainwandLength & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketOption").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & insertInTrack & tdEnd
                    result += tdStart & sloper & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF TRACK ONLY VERTICAL"
        End Try
        Return result
    End Function

    Private Shared Function Print_Vertical_Slat(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Vertical Blinds' AND BlindName='Slat Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='10' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "VERTICAL SLAT ONLY" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Slat Size" & thEnd
                result += thStart & "Slat Qty" & thEnd
                result += thStart & "Hanger Type" & thEnd
                result += thStart & "Bottom" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricWidth").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SlatQty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF SKIN ONLY VERTICAL"
        End Try
        Return result
    End Function

    Private Shared Function Print_Roller_SkinOnly(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Skin Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='9' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "ROLLER SKIN ONLY" & spanEnd

                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Trim" & thEnd
                result += thStart & "Bottom Rail" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF ROLLER SKIN ONLY"
        End Try
        Return result
    End Function

    Private Shared Function PrintPDFRollerStandard(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Standard' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "ROLLER STANDARD" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Roll" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Chain Colour" & thEnd
                result += thStart & "Chain Length" & thEnd
                result += thStart & "Trim" & thEnd
                result += thStart & "Bottom Rail" & thEnd
                result += thStart & "Tube" & thEnd
                result += thStart & "Childsafe" & thEnd
                result += thStart & "Accessory" & thEnd
                result += thStart & "Bracket Covers" & thEnd
                result += thStart & "Bracket Ext" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim bracketType As String = thisData.Tables(0).Rows(i).Item("BracketType").ToString()
                    Dim kitName As String = thisData.Tables(0).Rows(i).Item("KitName").ToString()

                    If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If
                    End If


                    If bracketType = "Linked 3 Blinds (Dep)" Or bracketType = "Linked 3 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 3" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & kitName & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("RollDirection").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TubeSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChildSafe").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketExtension").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result += "THERE IS AN ERROR IN THE ROLLER STANDARD. PLEASE CONTACT <b>support@onlineorder.au</b>"
        End Try
        Return result
    End Function

    Private Shared Function Print_Roller_Motorised(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Motorised' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse: collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "ROLLER MOTORISED" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Roll" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Motor" & thEnd
                result += thStart & "Remote" & thEnd
                result += thStart & "Charger" & thEnd
                result += thStart & "Flush Connect" & thEnd
                result += thStart & "Trim" & thEnd
                result += thStart & "Bottom" & thEnd
                result += thStart & "Tube" & thEnd
                result += thStart & "Accessory" & thEnd
                result += thStart & "Extras" & thEnd
                result += thStart & "Bracket Covers" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim BlindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                    Dim bracketType As String = thisData.Tables(0).Rows(i).Item("BracketType").ToString()
                    Dim kitName As String = thisData.Tables(0).Rows(i).Item("KitName").ToString()
                    Dim MotorStyle As String = thisData.Tables(0).Rows(i).Item("MotorStyle").ToString()
                    Dim MotorRemote As String = thisData.Tables(0).Rows(i).Item("MotorRemote").ToString()
                    Dim MotorCharger As String = thisData.Tables(0).Rows(i).Item("MotorCharger").ToString()

                    If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                        ' Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If BlindNo = "Blind 1" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If

                        If BlindNo = "Blind 2" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If
                    End If


                    If bracketType = "Linked 3 Blinds (Dep)" Or bracketType = "Linked 3 Blinds (Ind)" Then
                        ' Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If BlindNo = "Blind 1" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If BlindNo = "Blind 2" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If BlindNo = "Blind 3" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If
                    End If

                    If Not BlindNo = "Blind 1" AND InStr(bracketType, "Dep") > 0 Then
                        MotorStyle = ""
                        MotorRemote = ""
                        MotorCharger = ""
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & kitName & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("RollDirection").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & MotorStyle & tdEnd
                    result += tdStart & MotorRemote & tdEnd
                    result += tdStart & MotorCharger & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Connector").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TubeSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("AdditionalMotor").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                    result += trEnd
                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF ROLLER MOTORIZED"
        End Try
        Return result
    End Function

    Private Shared Function Print_Cassette(HeaderId As String) As String
        Dim result As String = String.Empty
        Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Cassette' AND TubeType='JAI Geared' AND Active=1 ORDER BY Id, BlindNo ASC")
        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim tdNotes As String = "<td colspan='18' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
            result += spanStart & "ROLLER CASSETTE - JAI SYSTEM" & spanEnd
            result += tableStart

            result += trStart
            result += thStart & "No" & thEnd
            result += thStart & "ID" & thEnd
            result += thStart & "Qty" & thEnd
            result += thStart & "Product" & thEnd
            result += thStart & "Location" & thEnd
            result += thStart & "Mounting" & thEnd
            result += thStart & "Width" & thEnd
            result += thStart & "Drop" & thEnd
            result += thStart & "Roll" & thEnd
            result += thStart & "Fabric" & thEnd
            result += thStart & "Control" & thEnd
            result += thStart & "Chain Colour" & thEnd
            result += thStart & "Chain Length" & thEnd
            result += thStart & "Trim" & thEnd
            result += thStart & "Bottom" & thEnd
            result += thStart & "ChildSafe" & thEnd
            result += thStart & "Accessory" & thEnd
            result += thStart & "Bracket Covers" & thEnd
            result += trEnd

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += trStart
                result += tdStart & i + 1 & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("RollDirection").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ChainColour").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ChildSafe").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                result += trEnd

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += trStart
                    result += tdNotes
                    result += bNotesStart
                    result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                    result += bNotesEnd
                    result += tdEnd
                    result += trEnd
                End If
            Next
            result += tableEnd
        End If
        Return result
    End Function

    Private Shared Function Print_CassetteMotorised(HeaderId As String) As String
        Dim result As String = String.Empty
        Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Roller Blinds' AND BlindName='Cassette' AND TubeType='Motorised' AND Active=1 ORDER BY Id, BlindNo ASC")
        If Not thisData.Tables(0).Rows.Count = 0 Then
            Dim tdNotes As String = "<td colspan='21' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
            result += spanStart & "ROLLER CASSETTE - MOTORISED" & spanEnd
            result += tableStart

            result += trStart
            result += thStart & "No" & thEnd
            result += thStart & "ID" & thEnd
            result += thStart & "Qty" & thEnd
            result += thStart & "Product" & thEnd
            result += thStart & "Location" & thEnd
            result += thStart & "Mounting" & thEnd
            result += thStart & "Control" & thEnd
            result += thStart & "Fabric" & thEnd
            result += thStart & "Width" & thEnd
            result += thStart & "Drop" & thEnd
            result += thStart & "Roll Direction" & thEnd
            result += thStart & "Motor" & thEnd
            result += thStart & "Remote" & thEnd
            result += thStart & "Charger" & thEnd
            result += thStart & "Flush Connect" & thEnd
            result += thStart & "Cable Exit" & thEnd
            result += thStart & "Trim" & thEnd
            result += thStart & "Bottom" & thEnd
            result += thStart & "Accessory" & thEnd
            result += thStart & "Extras" & thEnd
            result += thStart & "Bracket Covers" & thEnd
            result += trEnd

            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                result += trStart
                result += tdStart & i + 1 & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ID").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("RollDirection").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("MotorStyle").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("MotorRemote").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("MotorCharger").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Connector").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("CableExitPoint").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("AdditionalMotor").ToString() & tdEnd
                result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                result += trEnd

                If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                    result += trStart
                    result += tdNotes
                    result += bNotesStart
                    result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                    result += bNotesEnd
                    result += tdEnd
                    result += trEnd
                End If
            Next
            result += tableEnd
        End If
        Return result
    End Function

    Private Shared Function Print_Door(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Door' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "DOOR" & spanEnd
                result += tableStart
                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartColSpan3 & "Width" & thEnd
                result += thStartRowSpan2 & "Height" & thEnd       
                result += thStartColSpan2 & "Grille/Frame" & thEnd       
                result += thStartRowSpan2 & "Mesh" & thEnd       
                result += thStartColSpan2 & "Handle" & thEnd       
                result += thStartRowSpan2 & "Inswing Hinges" & thEnd       
                result += thStartRowSpan2 & "Lock Colour" & thEnd       
                result += thStartRowSpan2 & "Keyed Alike" & thEnd       
                result += thStartRowSpan2 & "Bug Seals" & thEnd       
                result += thStartRowSpan2 & "Closer" & thEnd     
                result += thStartColSpan2 & "Pet Door" & thEnd    
                result += thStartRowSpan2 & "Extras" & thEnd    
                result += trEnd

                result += trStart
                    '#Width
                    result += thStart & "Width" & thEnd
                    result += thStart & "Middle" & thEnd
                    result += thStart & "Bottom" & thEnd

                    '#Grille
                    result += thStart & "Type" & thEnd
                    result += thStart & "Colour" & thEnd

                    '#Handle
                    result += thStart & "Side" & thEnd
                    result += thStart & "Height" & thEnd

                    '#Pet Door
                    result += thStart & "Type" & thEnd
                    result += thStart & "Position" & thEnd
                result += trEnd 

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim FrameColour As String = thisData.Tables(0).Rows(i).Item("FrameColour").ToString()
                    Dim FrameLeft As String = thisData.Tables(0).Rows(i).Item("FrameLeft").ToString()
                    Dim FrameRight As String = thisData.Tables(0).Rows(i).Item("FrameRight").ToString()
                    Dim MeshType As String = thisData.Tables(0).Rows(i).Item("MeshType").ToString()
                    Dim Brace As String = thisData.Tables(0).Rows(i).Item("Brace").ToString()
                    Dim SlatSize As String = thisData.Tables(0).Rows(i).Item("SlatSize").ToString()
                    Dim SlatQty As String = thisData.Tables(0).Rows(i).Item("SlatQty").ToString()
                    Dim TrackColour As String = thisData.Tables(0).Rows(i).Item("TrackColour").ToString()
                    Dim WandPosition As String = thisData.Tables(0).Rows(i).Item("WandPosition").ToString()
                    Dim AdditionalMotorRaw As String = thisData.Tables(0).Rows(i).Item("AdditionalMotor").ToString()
                    
                    If InStr(FrameLeft, "Dulux Standard") > 0 Then
                        FrameLeft = "Dulux Standard"
                    Else If InStr(FrameLeft, "Duralloy Colours") > 0 Then
                        FrameLeft = "Duralloy Colours"
                    Else If InStr(FrameLeft, "Dulux Precious") > 0 Then
                        FrameLeft = "Dulux Precious"
                    Else If InStr(FrameLeft, "Dulux Alphatec") > 0 Then
                        FrameLeft = "Dulux Alphatec"
                    Else If InStr(FrameLeft, "Dulux Duratec Eternity") > 0 Then
                        FrameLeft = "Dulux Duratec Eternity"
                    Else If InStr(FrameLeft, "Dulux Duratec Elements") > 0 Then
                        FrameLeft = "Dulux Duratec Elements"
                    Else If InStr(FrameLeft, "Dulux Duratex Intensity") > 0 Then
                        FrameLeft = "Dulux Duratex Intensity"
                    End If

                    If FrameColour = "Powder Coating" Then 
                        FrameColour = String.Format("{0} - {1}", FrameLeft, FrameRight)
                    End IF

                    If Not (SlatQty = "" Or SlatQty = "0") Then
                        SlatSize += String.Format(" - ({0})", SlatQty)
                    End If

                    If Not (WandPosition = "" Or WandPosition = "0") Then
                        TrackColour += String.Format(" - ({0})", WandPosition)
                    End If

                    Dim AdditionalMotor As String = ""
                    If Not String.IsNullOrEmpty(AdditionalMotorRaw) Then
                        Try
                            Dim serializer As New JavaScriptSerializer()
                            
                            Dim rows As List(Of Dictionary(Of String, Object)) = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(AdditionalMotorRaw)
                            
                            Dim lines As New List(Of String)()
                            
                            For Each item As Dictionary(Of String, Object) In rows
                                Dim name As String = item("name").ToString()
                                Dim unit As String = item("unit").ToString()
                                Dim value As String = item("value").ToString()
                                
                                Dim formattedLine As String = ""
                                
                                If unit.Equals("Qty", StringComparison.OrdinalIgnoreCase) Then
                                    formattedLine = name & " - " & value & "Pcs/Qty"
                                Else
                                    formattedLine = name & " - " & value & unit
                                End If
                                
                                lines.Add(formattedLine)
                            Next
                            
                            AdditionalMotor = String.Join("<br />", lines)
                            
                        Catch ex As Exception
                            AdditionalMotor = "Error Parsing Data"
                        End Try
                    Else
                        AdditionalMotor = "-"
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WidthMiddle").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WidthBottom").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FrameType").ToString() & tdEnd
                    result += tdStart & FrameColour & tdEnd
                    result += tdStart & MeshType & tdEnd
                    result += tdStart & Brace & tdEnd
                    result += tdStart & SlatSize & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PortHole").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("PlungerPin").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Batten").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FlatType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChildSafe").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackType").ToString() & tdEnd
                    result += tdStart & TrackColour & tdEnd
                    result += tdStart & AdditionalMotor & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "DOOR ERROR CREATE PDF"
        End Try
        Return result
    End Function

    Private Shared Function Print_Window(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Window' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "WINDOW" & spanEnd
                result += tableStart
                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Height" & thEnd       
                result += thStartRowSpan2 & "Mesh" & thEnd       
                result += thStartRowSpan2 & "Sliding" & thEnd 
                result += thStartRowSpan2 & "Stacking" & thEnd 
                result += thStartRowSpan2 & "Trackless" & thEnd 
                result += thStartColSpan2 & "Frame" & thEnd       
                result += thStartRowSpan2 & "Brace" & thEnd       
                result += thStartRowSpan2 & "Fitting Otions" & thEnd 
                result += thStartRowSpan2 & "Extras" & thEnd    
                result += trEnd

                result += trStart

                    '#Grille
                    result += thStart & "Type" & thEnd
                    result += thStart & "Colour" & thEnd

                result += trEnd 

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim FrameColour As String = thisData.Tables(0).Rows(i).Item("FrameColour").ToString()
                    Dim FrameLeft As String = thisData.Tables(0).Rows(i).Item("FrameLeft").ToString()
                    Dim FrameRight As String = thisData.Tables(0).Rows(i).Item("FrameRight").ToString()
                    Dim MeshType As String = thisData.Tables(0).Rows(i).Item("MeshType").ToString()
                    Dim Brace As String = thisData.Tables(0).Rows(i).Item("Brace").ToString()
                    Dim SlatSize As String = thisData.Tables(0).Rows(i).Item("SlatSize").ToString()
                    Dim SlatQty As String = thisData.Tables(0).Rows(i).Item("SlatQty").ToString()
                    Dim TrackColour As String = thisData.Tables(0).Rows(i).Item("TrackColour").ToString()
                    Dim WandPosition As String = thisData.Tables(0).Rows(i).Item("WandPosition").ToString()
                    Dim AdditionalMotorRaw As String = thisData.Tables(0).Rows(i).Item("AdditionalMotor").ToString()
                    
                    If InStr(FrameLeft, "Dulux Standard") > 0 Then
                        FrameLeft = "Dulux Standard"
                    Else If InStr(FrameLeft, "Duralloy Colours") > 0 Then
                        FrameLeft = "Duralloy Colours"
                    Else If InStr(FrameLeft, "Dulux Precious") > 0 Then
                        FrameLeft = "Dulux Precious"
                    Else If InStr(FrameLeft, "Dulux Alphatec") > 0 Then
                        FrameLeft = "Dulux Alphatec"
                    Else If InStr(FrameLeft, "Dulux Duratec Eternity") > 0 Then
                        FrameLeft = "Dulux Duratec Eternity"
                    Else If InStr(FrameLeft, "Dulux Duratec Elements") > 0 Then
                        FrameLeft = "Dulux Duratec Elements"
                    Else If InStr(FrameLeft, "Dulux Duratex Intensity") > 0 Then
                        FrameLeft = "Dulux Duratex Intensity"
                    End If

                    If FrameColour = "Powder Coating" Then 
                        FrameColour = String.Format("{0} - {1}", FrameLeft, FrameRight)
                    End IF

                    If Not (SlatQty = "" Or SlatQty = "0") Then
                        SlatSize += String.Format(" - ({0})", SlatQty)
                    End If

                    If Not (WandPosition = "" Or WandPosition = "0") Then
                        TrackColour += String.Format(" - ({0})", WandPosition)
                    End If

                    Dim AdditionalMotor As String = ""
                    If Not String.IsNullOrEmpty(AdditionalMotorRaw) Then
                        Try
                            Dim serializer As New JavaScriptSerializer()
                            
                            Dim rows As List(Of Dictionary(Of String, Object)) = serializer.Deserialize(Of List(Of Dictionary(Of String, Object)))(AdditionalMotorRaw)
                            
                            Dim lines As New List(Of String)()
                            
                            For Each item As Dictionary(Of String, Object) In rows
                                Dim name As String = item("name").ToString()
                                Dim unit As String = item("unit").ToString()
                                Dim value As String = item("value").ToString()
                                
                                Dim formattedLine As String = ""
                                
                                If unit.Equals("Qty", StringComparison.OrdinalIgnoreCase) Then
                                    formattedLine = name & " - " & value & "Pcs/Qty"
                                Else
                                    formattedLine = name & " - " & value & unit
                                End If
                                
                                lines.Add(formattedLine)
                            Next
                            
                            AdditionalMotor = String.Join("<br />", lines)
                            
                        Catch ex As Exception
                            AdditionalMotor = "Error Parsing Data"
                        End Try
                    Else
                        AdditionalMotor = "-"
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & MeshType & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomTrackType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TilterPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FrameType").ToString() & tdEnd
                    result += tdStart & FrameColour & tdEnd
                    result += tdStart & Brace & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Fitting").ToString() & tdEnd
                    result += tdStart & AdditionalMotor & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "WINDOW ERROR CREATE PDF"
        End Try
        Return result
    End Function



    Private Shared Function PrintPDFGlobalGearReduction(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Global Roller Blinds' AND BlindName='Gear Reduction' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "GLOBAL GEAR REDUCTION" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Roll" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Chain Colour" & thEnd
                result += thStart & "Chain Length" & thEnd
                result += thStart & "Trim" & thEnd
                result += thStart & "Bottom Rail / Decorative" & thEnd
                result += thStart & "Tube" & thEnd
                result += thStart & "Childsafe" & thEnd
                ' result += thStart & "Accessory" & thEnd
                result += thStart & "Bracket Covers" & thEnd
                result += thStart & "Bracket Ext" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim bracketType As String = thisData.Tables(0).Rows(i).Item("BracketType").ToString()
                    Dim kitName As String = thisData.Tables(0).Rows(i).Item("KitName").ToString()

                    If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If
                    End If


                    If bracketType = "Linked 3 Blinds (Dep)" Or bracketType = "Linked 3 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 3" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & kitName & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("RollDirection").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TubeSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChildSafe").ToString() & tdEnd
                    ' result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketExtension").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result += "THERE IS AN ERROR IN THE GLOBAL GEAR REDUCTION. PLEASE CONTACT <b>support@onlineorder.au</b>"
        End Try
        Return result
    End Function

    Private Shared Function Print_Global_Roller_Motorised(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Global Roller Blinds' AND BlindName='Motorised' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='20' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse: collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "GLOBAL ROLLER MOTORISED" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Roll" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Motor" & thEnd
                result += thStart & "Remote" & thEnd
                result += thStart & "Charger" & thEnd
                result += thStart & "Flush Connect" & thEnd
                result += thStart & "Trim" & thEnd
                result += thStart & "Bottom" & thEnd
                result += thStart & "Tube" & thEnd
                result += thStart & "Accessory" & thEnd
                result += thStart & "Extras" & thEnd
                result += thStart & "Bracket Covers" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim bracketType As String = thisData.Tables(0).Rows(i).Item("BracketType").ToString()
                    Dim kitName As String = thisData.Tables(0).Rows(i).Item("KitName").ToString()

                    If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            If Not getConnectedId = "" Then
                                kitName += "<br />"
                                kitName += "<span style='font-size:6px;color:red;'>" & "* COMPLETE SET WITH ITEM ID : " & getConnectedId & "</span>"
                            End If
                        End If
                    End If


                    If bracketType = "Linked 3 Blinds (Dep)" Or bracketType = "Linked 3 Blinds (Ind)" Then
                        Dim blindNo As String = thisData.Tables(0).Rows(i).Item("BlindNo").ToString()
                        Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()

                        If blindNo = "Blind 1" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 2" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If

                        If blindNo = "Blind 3" Then
                            Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                            Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                            Dim id2 As String = String.Empty

                            If Not getConnectedId2 = "" Then
                                id2 = " item ID " & getConnectedId2
                            End If
                            kitName += "<br />"
                            kitName += "<span style='font-size:6px;color:red;'>" & "* LINKED WITH ITEM ID : " & getConnectedId & id2 & "</span>"
                        End If
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & kitName & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("RollDirection").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorStyle").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorRemote").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MotorCharger").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Connector").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Trim").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TubeSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Accessory").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("AdditionalMotor").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketCover").ToString() & tdEnd
                    result += trEnd
                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF ROLLER MOTORIZED"
        End Try
        Return result
    End Function

    Private Shared Function Print_GlobalPanelGlides(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Global Panel Glides' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='19' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "GLOBAL PANEL GLIDES" & spanEnd
                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Fabric" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Layout" & thEnd
                result += thStartRowSpan2 & "No Panel" & thEnd
                result += thStartColSpan2 & "Track" & thEnd
                result += thStartColSpan3 & "Wand" & thEnd
                result += thStartRowSpan2 & "Bottom Rail" & thEnd
                result += thStartRowSpan2 & "Batten" & thEnd
                result += thStartRowSpan2 & "Batten Colour" & thEnd
                result += thStartRowSpan2 & "Fitting" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Type" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Position" & thEnd
                result += thStart & "Length" & thEnd
                result += thStart & "Colour" & thEnd
                result += trEnd


                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Layout").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("NumOfPanel").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("WandColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Batten").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BattenColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Fitting").ToString() & tdEnd
                    result += trEnd


                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF GLOBAL PANEL GLIDES"
        End Try
        Return result
    End Function

    Private Shared Function Print_GlobalRomanBlinds(HeaderId As String) As String
        Dim result As String = String.Empty

        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Global Roman Blinds' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='18' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "GLOBAL ROMAN BLINDS" & spanEnd
                result += tableStart

                result += trStart
                result += thStartRowSpan2 & "No" & thEnd
                result += thStartRowSpan2 & "ID" & thEnd
                result += thStartRowSpan2 & "Qty" & thEnd
                result += thStartRowSpan2 & "Product" & thEnd
                result += thStartRowSpan2 & "Location" & thEnd
                result += thStartRowSpan2 & "Mounting" & thEnd
                result += thStartRowSpan2 & "Fabric" & thEnd
                result += thStartRowSpan2 & "Width" & thEnd
                result += thStartRowSpan2 & "Drop" & thEnd
                result += thStartRowSpan2 & "Control Position" & thEnd
                result += thStartColSpan3 & "Chain" & thEnd
                result += thStartColSpan2 & "Cord" & thEnd
                result += thStartRowSpan2 & "Batten Colour" & thEnd
                result += thStartRowSpan2 & "Plastic Colour" & thEnd
                result += thStartRowSpan2 & "Cleat" & thEnd
                result += trEnd

                result += trStart
                result += thStart & "Material" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Length" & thEnd
                result += thStart & "Colour" & thEnd
                result += thStart & "Length" & thEnd
                result += trEnd


                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("MaterialChain").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ChainLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("CordColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("CordLength").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BattenColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("AcornPlasticColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Cleat").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF ROMAN BLINDS"
        End Try
        Return result
    End Function

    Private Shared Function Print_GlobalVertical_Complete(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Global Vertical Blinds' AND BlindName='Complete' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='21' style='margin-left:50px;word-wrap:break-word;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;'>"
                result += spanStart & "GLOBAL VERTICAL COMPLETE" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Fabric/Slat Size" & thEnd
                result += thStart & "Stack" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Chain/Wand Colour" & thEnd
                result += thStart & "Control Length" & thEnd
                result += thStart & "Track" & thEnd
                result += thStart & "Brackets" & thEnd
                result += thStart & "Bracket Colour" & thEnd
                result += thStart & "Hanger Type" & thEnd
                result += thStart & "Bottom" & thEnd
                result += thStart & "Insert In Track" & thEnd
                result += thStart & "Sloper" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim controlType As String = thisData.Tables(0).Rows(i).Item("ControlType").ToString()

                    Dim chainwandColour As String = thisData.Tables(0).Rows(i).Item("WandColour").ToString()
                    Dim chainwandLength As String = thisData.Tables(0).Rows(i).Item("WandLength").ToString()
                    If controlType = "Chain" Then
                        chainwandColour = thisData.Tables(0).Rows(i).Item("ChainColour").ToString()
                        chainwandLength = thisData.Tables(0).Rows(i).Item("ChainLength").ToString()
                    End If

                    Dim insertInTrack As String = "No"
                    Dim sloper As String = "No"
                    If thisData.Tables(0).Rows(i).Item("InsertInTrack").ToString() = "1" Then
                        insertInTrack = "Yes"
                    End If
                    If thisData.Tables(0).Rows(i).Item("Sloper").ToString() = "1" Then
                        sloper = "Yes"
                    End If

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SlatSize").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & chainwandColour & tdEnd
                    result += tdStart & chainwandLength & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketOption").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & insertInTrack & tdEnd
                    result += tdStart & sloper & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF VERTICAL COMPLETE"
        End Try
        Return result
    End Function

    Private Shared Function Print_GlobalVertical_Track(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Global Vertical Blinds' AND BlindName='Track Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='19' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "GLOBAL VERTICAL TRACK ONLY" & spanEnd

                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Mounting" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Width" & thEnd
                result += thStart & "Slat" & thEnd
                result += thStart & "Stack" & thEnd
                result += thStart & "Control" & thEnd
                result += thStart & "Chain/Wand" & thEnd
                result += thStart & "Length/Size" & thEnd
                result += thStart & "Track" & thEnd
                result += thStart & "Brackets" & thEnd
                result += thStart & "Bracket Colour" & thEnd
                result += thStart & "Hanger Type" & thEnd
                result += thStart & "Bottom" & thEnd
                result += thStart & "Insert In Track" & thEnd
                result += thStart & "Sloper" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    Dim controlType As String = thisData.Tables(0).Rows(i).Item("ControlType").ToString()

                    Dim chainwandColour As String = thisData.Tables(0).Rows(i).Item("WandColour").ToString()
                    Dim chainwandLength As String = thisData.Tables(0).Rows(i).Item("WandLength").ToString()
                    If controlType = "Chain" Then
                        chainwandColour = thisData.Tables(0).Rows(i).Item("ChainColour").ToString()
                        chainwandLength = thisData.Tables(0).Rows(i).Item("ChainLength").ToString()
                    End If

                    Dim insertInTrack As String = "No"
                    Dim sloper As String = "No"
                    If Not thisData.Tables(0).Rows(i).Item("InsertInTrack").ToString() = "False" OR thisData.Tables(0).Rows(i).Item("InsertInTrack").ToString() = "0" Then
                        insertInTrack = "Yes"
                    End If
                    If Not thisData.Tables(0).Rows(i).Item("Sloper").ToString() = "False" OR thisData.Tables(0).Rows(i).Item("Sloper").ToString() = "0" Then
                        sloper = "Yes"
                    End If

                    Dim slat As String = thisData.Tables(0).Rows(i).Item("SlatSize").ToString()
                    ' slat += " - "
                    ' slat += thisData.Tables(0).Rows(i).Item("SlatQty").ToString()

                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Mounting").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Width").ToString() & tdEnd
                    result += tdStart & slat & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("StackPosition").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("ControlPosition").ToString() & tdEnd
                    result += tdStart & chainwandColour & tdEnd
                    result += tdStart & chainwandLength & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("TrackColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketOption").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BracketColour").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += tdStart & insertInTrack & tdEnd
                    result += tdStart & sloper & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF TRACK ONLY VERTICAL"
        End Try
        Return result
    End Function

    Private Shared Function Print_GlobalVertical_Slat(HeaderId As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName='Global Vertical Blinds' AND BlindName='Slat Only' AND Active=1 ORDER BY Id, BlindNo ASC")
            If Not thisData.Tables(0).Rows.Count = 0 Then
                Dim tdNotes As String = "<td colspan='10' style='margin-left:50px;height:auto;font-size:8px;border:1px solid black;border-collapse:collapse;padding-top:10px;padding-bottom:10px;word-wrap:break-word;'>"
                result += spanStart & "GLOBAL VERTICAL SLAT ONLY" & spanEnd
                result += tableStart

                result += trStart
                result += thStart & "No" & thEnd
                result += thStart & "ID" & thEnd
                result += thStart & "Qty" & thEnd
                result += thStart & "Location" & thEnd
                result += thStart & "Product" & thEnd
                result += thStart & "Drop" & thEnd
                result += thStart & "Fabric" & thEnd
                result += thStart & "Slat Size" & thEnd
                result += thStart & "Slat Qty" & thEnd
                result += thStart & "Hanger Type" & thEnd
                result += thStart & "Bottom" & thEnd
                result += trEnd

                For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                    result += trStart
                    result += tdStart & i + 1 & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Id").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Qty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("KitName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Location").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("Drop").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricName").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("FabricWidth").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("SlatQty").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("HangerType").ToString() & tdEnd
                    result += tdStart & thisData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() & tdEnd
                    result += trEnd

                    If Not thisData.Tables(0).Rows(i).Item("Notes").ToString() = "" Then
                        result += trStart
                        result += tdNotes
                        result += bNotesStart
                        result += thisData.Tables(0).Rows(i).Item("Notes").ToString()
                        result += bNotesEnd
                        result += tdEnd
                        result += trEnd
                    End If
                Next
                result += tableEnd
            End If
        Catch ex As Exception
            result = "ERROR CREATE PDF SKIN ONLY VERTICAL"
        End Try
        Return result
    End Function

    Private Shared Function BindDescOrderItem(HeaderId As String) As String
        Dim result As String = ""

        Dim separted As String = " | "
        Dim totalAluminium As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Aluminium Blinds' AND Active=1")
        Dim totalCellular As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Cellular Blinds' AND Active=1")
        Dim totalLumen As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Lumen' AND Active=1")
        Dim totalPG As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Panel Glides' AND Active=1")
        Dim totalVenetian As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Venetian Blinds' AND Active=1")
        Dim totalRoller As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Roller Blinds' AND Active=1")
        Dim totalGlobalRoller As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Global Roller Blinds' AND Active=1")
        Dim totalRoman As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Roman Blinds' AND Active=1")
        Dim totalGlobalRoman As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Global Roman Blinds' AND Active=1")
        Dim totalVerishades As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Veri Shades' AND Active=1")
        Dim totalVertical As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Vertical Blinds' AND Active=1")
        Dim totalDoor As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Door' AND Active=1")
        Dim totalWindow As String = publicCfg.GetItemData("SELECT SUM(Qty) FROM view_details WHERE HeaderId='" + HeaderId + "' AND DesignName = 'Window' AND Active=1")

        If totalAluminium = "" Then : totalAluminium = "-" : End If
        If totalCellular = "" Then : totalCellular = "-" : End If
        If totalLumen = "" Then : totalLumen = "-" : End If
        If totalPG = "" Then : totalPG = "-" : End If
        If totalVenetian = "" Then : totalVenetian = "-" : End If
        If totalRoller = "" Then : totalRoller = "-" : End If
        If totalGlobalRoller = "" Then : totalGlobalRoller = "-" : End If
        If totalRoman = "" Then : totalRoman = "-" : End If
        If totalGlobalRoman = "" Then : totalGlobalRoman = "-" : End If
        If totalVerishades = "" Then : totalVerishades = "-" : End If
        If totalVertical = "" Then : totalVertical = "-" : End If
        If totalDoor = "" Then : totalDoor = "-" : End If
        If totalWindow = "" Then : totalWindow = "-" : End If

        Dim aluminiumblinds As String = "<b>Aluminium Blinds: " & totalAluminium & "</b>"
        Dim celloraBlinds As String = "<b>Cellular Blinds: " & totalCellular & "</b>"
        Dim lumen As String = "<b>Lumen : " & totalLumen & "</b>"
        Dim panelGlides As String = "<b>Panel Glides: " & totalPG & "</b>"
        Dim venetianblinds As String = "<b>Venetian Blinds:  " & totalVenetian & "</b>"
        Dim rollerblinds As String = "<b>Roller Blinds: " & totalRoller & "</b>"
        Dim rollerglobalblinds As String = "<b>Global Roller Blinds: " & totalGlobalRoller & "</b>"
        Dim romanBlinds As String = "<b>Roman Blinds: " & totalRoman & "</b>"
        Dim romanGlobalBlinds As String = "<b>Global Roman Blinds: " & totalGlobalRoman & "</b>"
        Dim verishades As String = "<b>Veri Shades: " & totalVerishades & "</b>"
        Dim verticalblinds As String = "<b>Vertical Blinds: " & totalVertical & "</b>"
        Dim door As String = "<b>Door: " & totalDoor & "</b>"
        Dim window As String = "<b>Window: " & totalWindow & "</b>"

        ' result = celloraBlinds & separted & lumen & separted & panelGlides & separted & venetianblinds & separted & rollerblinds & separted & rollerglobalblinds & separted & romanBlinds & separted & verishades & separted & verticalblinds
        result = String.Format("{0} | {1} | {2} | {3} | {4} | {5} | {6} | {7} | {8} | {9} | {10} | {11}",celloraBlinds, lumen, panelGlides, venetianblinds, rollerblinds, rollerglobalblinds, romanBlinds, romanGlobalBlinds, verishades, verticalblinds, door, window)
        Return result
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CreatePDFCustomerQuote(ByVal headerid As String, ByVal username As String, ByVal action As String) As Object
        Try
            Dim msg As String = ""
            Dim url As String = ""
            Dim fileDirectory As String = ""
            Dim detailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + headerid + "' AND Active='1'")
            If detailData.Tables(0).Rows.Count < 1 Then
                Return New With { .warning = true, .message = "Please add item first."}
            End If

            Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM OrderHeaders WHERE Id='" + headerid + "'")
            if headerData.Tables(0).Rows.Count < 1 Then
                Throw New Exception("Order Header not found.")
            End If

            Dim orderNo As String = headerData.Tables(0).Rows(0).Item("OrderNo").ToString()
            Dim storeId As String = headerData.Tables(0).Rows(0).Item("StoreId").ToString()
            Dim fileName As String = (String.Format("-QUOTE-ORDER-{0}-{1}.pdf", orderNo, storeId)).Replace(" ", "")

            fileDirectory = HttpContext.Current.Server.MapPath("~/file/order/quote")


            Dim Key As String = "Customer"
            If action = "reprint" or action = "preview" Then
                HttpContext.Current.Session("Reprint") = fileName
                HttpContext.Current.Session("KeyReprint") = Key
                msg = "Print page is successfully prepared. <br> Click <b>OK</b> to open it."
                url = "/order/printquote"
            End If

            If action = "download" Then
                msg = "Your download is ready. Click <b>OK</b> if download does not start automatically."
                url = "/Methods/Order/Handler/DowloadPDFOrder.ashx?file=" & fileName & "&keyDownload=quote"
            End If
            
            printCfg.CreatePDFQuote(headerid, username, fileDirectory, fileName, Key)

            Return New With {.success = true, .message = msg, .url = url}

        Catch ex As Exception
            Return New With { .error = true, .message = ex.Message}
        End Try
    End Function

    <WebMethod()>
    <ScriptMethod(ResponseFormat:=ResponseFormat.Json)>
    Public Shared Function CreatePDFQuote(ByVal headerid As String, ByVal action As String, ByVal username As String) As Object
        Try
            Dim msg As String = "Barcode has been downloaded."
            Dim url As String = ""
            Dim rolename As String = HttpContext.Current.Session("rolename").ToString()
            

            Dim HeaderData As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id='" & headerid & "'")
            If HeaderData.Tables(0).Rows.Count < 1 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Order Header not found."}}
            End If

            Dim DetailData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" & headerid & "'")
            If DetailData.Tables(0).Rows.Count < 1 Then
                Return New ErrorResponse With {.error = New ErrorDetail With {.message = "please add item first."}}
            End If

            ' Dim status As String = HeaderData.Tables(0).Rows(0)("Status").ToString()
            ' If rolename <> "Administrator" AndAlso status <> "Draft" Then
            '     Return New ErrorResponse With {.error = New ErrorDetail With {.message = "Permission denied : not administrator."}}
            ' End If

            Dim OrderCust As String = HeaderData.Tables(0).Rows(0).Item("OrderCust").ToString()
            Dim OrderNo As String = HeaderData.Tables(0).Rows(0).Item("OrderNo").ToString()
            Dim StoreId As String = HeaderData.Tables(0).Rows(0).Item("StoreId").ToString()
            Dim StoreName As String = HeaderData.Tables(0).Rows(0).Item("StoreName").ToString()
            Dim Delivery As String = HeaderData.Tables(0).Rows(0).Item("Delivery").ToString()
            Dim FileName As String = ("-QUOTE-ORDER-" & OrderNo & "-" & StoreId & ".pdf").Replace(" ", "")

            Dim dirPath As String = HttpContext.Current.Server.MapPath("~/File/Order/Quote/Origin/")
            If Not Directory.Exists(dirPath) Then
                Directory.CreateDirectory(dirPath)
            End If
            Dim fullPath As String = Path.Combine(dirPath, FileName)

            Dim Key As String = "Origin"
            If action = "preview" Then
                HttpContext.Current.Session("Reprint") = FileName
                HttpContext.Current.Session("KeyReprint") = Key
                msg = "Print page is successfully prepared. <br> Click <b>OK</b> to open it."
                url = "/order/printquote"
            End IF

            If action = "mail" Then
                msg = "Email has been sent successfully. <br> Click <b>OK</b> to continue."
            End If

            printCfg.CreatePDFQuote(headerid, username, dirPath, FileName, Key)

            Return New SuccessResponse With {
                .Success = New SuccessDetail With {.message = msg, .url = url}
            }
        Catch ex As Exception
            Return New ErrorResponse With {.error = New ErrorDetail With {.message = ex.Message}}
        End Try
    End Function

End Class
