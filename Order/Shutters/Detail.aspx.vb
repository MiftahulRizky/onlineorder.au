Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.IO

Partial Class Order_Detail
    Inherits Page

    Dim orderCfg As New OrderConfig
    Dim mailCfg As New MailConfig

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim headerid As String = Request.QueryString("param")
        Dim ordertype As String = Request.QueryString("ordertype")
        lblOrderType.Text = StrConv(ordertype, VbStrConv.ProperCase)
        If headerid = "" Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        If ordertype = "" Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        lblHeaderId.Text = headerid
        If Not IsPostBack Then
            AllMessageError(False, String.Empty)
            BindDataOrder(lblHeaderId.Text)
            BindDesignType()
        End If
    End Sub

    Protected Sub btnPreview_Click(sender As Object, e As EventArgs)
        MessageError_Preview(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showPreview(); };"
        Try
            If gvList.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM !")
                Exit Sub
            End If

            If lblOrderType.Text = "RBR" Or lblOrderType.Text = "Blinds" Or lblOrderType.Text = "Curtain" Or lblOrderType.Text = "Veri Shades" Or lblOrderType.Text = "Zebra Blinds" Then

                Dim previewCfg As New PreviewConfig
                Dim filePath As String = "~/File/Order/"
                Dim fileName As String = "Preview" & "-" & spanOrderId.InnerText & ".pdf"

                Dim finalFilePath As String = Server.MapPath(filePath & fileName)
                previewCfg.BindContent(lblHeaderId.Text, finalFilePath)

                Dim documentFile As String = "~/File/Order/" & fileName
                framePreview.Attributes("src") = "../Handler/PDF.ashx?document=" & documentFile

                ClientScript.RegisterStartupScript(Me.GetType(), "showPreview", thisScript, True)
                Exit Sub
            End If

            If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                Dim previewCfg As New ShuttersPreviewConfig
                Dim filePath As String = "~/File/Order/"
                Dim fileName As String = "Preview" & "-" & spanOrderId.InnerText & ".pdf"

                Dim finalFilePath As String = Server.MapPath(filePath & fileName)
                previewCfg.BindContent(lblHeaderId.Text, finalFilePath)

                Response.Clear()
                Dim url As String = "/order/shutters/preview"
                Session("printPreview") = finalFilePath
                Dim sb As New StringBuilder()
                sb.Append("<script type = 'text/javascript'>")
                sb.Append("window.open('")
                sb.Append(url)
                sb.Append("', '_blank');")
                sb.Append("</script>")
                ClientScript.RegisterStartupScript(Me.GetType(), "script", sb.ToString())

                Exit Sub
            End If
        Catch ex As Exception
            MessageError_Preview(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Preview(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError_Preview(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                        MessageError_Preview(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "btnPreview_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showPreview", thisScript, True)
        End Try
    End Sub

    Protected Sub btnEditHeader_Click(sender As Object, e As EventArgs)
        ' Session("headerAction") = "EditHeader"
        ' Session("headerId") = lblHeaderId.Text
        Dim ordertype As String = Request.QueryString("ordertype")
        Response.Redirect("~/order/header?action=edit&param="+lblHeaderId.Text+"&ordertype=" + ordertype, False)
    End Sub

    Protected Sub btnSubmitOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If gvList.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM !")
                Exit Sub
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Status='New Order', SubmittedDate=GETDATE(), SubmittedBy=@SubmittedBy WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@SubmittedBy", UCase(Session("LoginId")).ToString())

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            orderCfg.UpdateProductType(lblHeaderId.Text)

            Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), "Submit Order"}
            orderCfg.Log_Orders(dataLog)

            Dim deposit As String = "1"
            If lblCashSale.Text = True Then deposit = "0"

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Deposit=@Deposit WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@Deposit", deposit)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim checkData As Integer = orderCfg.GetItemData_Integer("SELECT COUNT(*) FROM OrderAuthorizations WHERE HeaderId='" + lblHeaderId.Text + "' AND (Status IS NULL OR Status = '' OR Status = 'Declined') AND Active=1")
            Dim approve As String = "1"

            If checkData > 0 Then approve = "2"
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Approved=@Approved WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@Approved", approve)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            orderCfg.UpdateFinalCostIvoices(lblHeaderId.Text)

            If approve = "2" Then
                ddlStatusAdditional.Items.Add(New ListItem("On Hold - Order Detail Query", "On Hold - Order Detail Query"))
                ddlStatusAdditional.SelectedValue = "On Hold - Order Detail Query"

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If

            If lblCashSale.Text = True Then
                ddlStatusAdditional.Items.Add(New ListItem("Waiting for Deposit", "Waiting for Deposit"))
                ddlStatusAdditional.SelectedValue = "Waiting for Deposit"
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If

            Dim filePath As String = Server.MapPath("~/File/Order/")
            Dim fileName As String = "Order" & "-" & spanOrderId.InnerText & ".pdf"
            Dim finalFilePath As String = filePath & fileName

            If lblOrderType.Text = "RBR" Or lblOrderType.Text = "Blinds" Or lblOrderType.Text = "Curtain" Or lblOrderType.Text = "Zebra Blinds" Or lblOrderType.Text = "Veri Shades" Then
                Dim previewCfg As New PreviewConfig
                previewCfg.BindContent(lblHeaderId.Text, finalFilePath)
            End If

            If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                Dim previewCfg As New ShuttersPreviewConfig
                previewCfg.BindContent(lblHeaderId.Text, finalFilePath)
            End If

            mailCfg.MailSubmit(lblHeaderId.Text, filePath, fileName)

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                End If
                mailCfg.MailError(Page.Title, "btnSubmitOrder_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitUnsubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            ' UPDATE
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET JobId = NULL, ShipmentId = NULL, Status = 'Unsubmitted', StatusAdditional = NULL, SubmittedBy = NULL, SubmittedDate = NULL, Deposit = 0, Approved = 0 WHERE Id = @Id; UPDATE OrderDetails_Shutters SET Paid = 0 WHERE HeaderId = @Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            ' DELETE
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("DELETE FROM OrderInvoices WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), "Unsubmit Order"}
            orderCfg.Log_Orders(dataLog)

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitUnsubmit_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Active=0 WHERE Id=@Id; UPDATE OrderDetails_Shutters SET Active = 0 WHERE HeaderId=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), "Delete Order"}
            orderCfg.Log_Orders(dataLog)

            Response.Redirect("~/order", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                        MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "btnSubmitDelete_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitAuthorizeOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim checkData As Integer = orderCfg.GetItemData_Integer("SELECT COUNT(*) FROM OrderAuthorizations WHERE HeaderId='" + lblHeaderId.Text + "' AND (Status IS NULL OR Status = '' OR Status = 'Declined') AND Active=1")
            If checkData > 0 Then
                MessageError(True, "PLEASE CHECK THE AUTHORIZATION LIST & COMPLETE IT FIRST !")
                Exit Sub
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Approved=@Approved WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@Approved", "1")

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            ddlStatusAdditional.SelectedValue = ""
            If lblDeposit.Text = 0 Then
                ddlStatusAdditional.SelectedValue = "Waiting for Deposit"
            End If
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {lblHeaderId.Text, lblItemId.Text, Session("LoginId").ToString(), "Authorize Order"}
            orderCfg.Log_Orders(dataLog)
            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitAuthorizeOrder_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitDeclineOrder_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim checkData As Integer = orderCfg.GetItemData_Integer("SELECT COUNT(*) FROM OrderAuthorizations WHERE HeaderId='" + lblHeaderId.Text + "' AND (Status IS NULL OR Status = '' OR Status = 'Authorized')")
            If checkData > 0 Then
                MessageError(True, "PLEASE CHECK THE AUTHORIZATION LIST & COMPLETE IT FIRST !")
                Exit Sub
            End If

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Approved=@Approved WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@Approved", "2")

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {lblHeaderId.Text, lblItemId.Text, Session("LoginId").ToString(), "Decline Order"}
            orderCfg.Log_Orders(dataLog)

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitDeclineOrder_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitGenerateJob_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim customerMinimum As Boolean = orderCfg.GetItemData_Boolean("SELECT MinimumOrderSurcharge FROM Customers WHERE Id = '" + lblCustomerId.Text + "'")

            If customerMinimum = True Then
                Dim checkService As Integer = orderCfg.GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails_Shutters WHERE HeaderId = '" + lblHeaderId.Text + "' AND ProductId = '901C1FF3-F3B5-4ACE-A73E-79027ECAEFFF' AND Active = 1")
                Dim sumPrice As Decimal = orderCfg.GetItemData_Decimal("SELECT SUM(FinalCost) AS SumPrice FROM OrderDetails_Shutters WHERE HeaderId = '" + lblHeaderId.Text + "' AND Active=1")

                If checkService = 0 And sumPrice < 200 Then
                    MessageError(True, "PLEASE ADD THE MINIMUM ORDER SURCHARGE, THEN REGENERATE THE JOB. !")
                    Exit Sub
                End If
            End If

            Dim jobId As String = spanOrderId.InnerText & "/01"
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Status='In Production', JobId=@JobId, JobDate=GETDATE() WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@JobId", jobId)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), "Generate Job Order"}
            orderCfg.Log_Orders(dataLog)

            If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                Dim fileName As String = String.Format("Job Order {0}.pdf", spanOrderId.InnerText)
                Dim filePath As String = Server.MapPath("~/file/order/")

                Dim finalPath As String = Path.Combine(filePath, fileName)

                Dim previewCfg As New ShuttersPreviewConfig
                previewCfg.BindContent(lblHeaderId.Text, finalPath)

                mailCfg.MailSuplierShutters(lblHeaderId.Text, finalPath)

                Threading.Thread.Sleep(3000)

                Response.Redirect("~/order/detail", False)
                Exit Sub
            End If

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmitHuake_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim fileName As String = "Job Order " & spanOrderId.InnerText & ".pdf"
            Dim filePath As String = Server.MapPath("~/file/order/")

            Dim finalPath As String = Path.Combine(filePath, fileName)

            Dim previewCfg As New ShuttersPreviewConfig
            previewCfg.BindContent(lblHeaderId.Text, finalPath)

            mailCfg.MailSuplierShutters(lblHeaderId.Text, finalPath)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitHuake_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitCancel_Click(sender As Object, e As EventArgs)
        MessageError_CancelOrder(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showCancelOrder(); };"
        Try
            If ddlReasonCategory.SelectedValue = "" Then
                MessageError_CancelOrder(True, "REASON CATEGORY IS REQURIED !")
                ddlReasonCategory.Focus()
                ClientScript.RegisterStartupScript(Me.GetType(), "showCancelOrder", thisScript, True)
                Exit Sub
            End If

            If txtCancelReasonDesc.Text = "" Then
                MessageError_CancelOrder(True, "REASON DESCRIPTION IS REQURIED !")
                txtCancelReasonDesc.Focus()
                ClientScript.RegisterStartupScript(Me.GetType(), "showCancelOrder", thisScript, True)
                Exit Sub
            End If

            If msgErrorCancelOrder.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Status='Canceled', StatusAdditional=NULL, CanceledDate=GETDATE(), CanceledCategory=@CanceledCategory, CanceledDescription=@CanceledDescription WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@CanceledCategory", ddlReasonCategory.SelectedValue)
                        myCmd.Parameters.AddWithValue("@CanceledDescription", txtCancelReasonDesc.Text.Trim())

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), "Cancel Order"}
                orderCfg.Log_Orders(dataLog)

                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            MessageError_CancelOrder(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_CancelOrder(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitCancel_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showCancelOrder", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitComplete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Status='Completed', CompletedDate=GETDATE() WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), "Complete Order"}
            orderCfg.Log_Orders(dataLog)
            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitComplete_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitExact_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim exactCfg As New ExactConfig

            Dim fileName As String = String.Format("Order-{0}.xml", spanOrderId.InnerText)
            Dim filePath As String = Server.MapPath("~/file/inv/")

            Dim finalPath As String = Path.Combine(filePath, fileName)

            exactCfg.CreateXML(lblHeaderId.Text, fileName, filePath)
            exactCfg.Connect(finalPath)

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitExact_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitSlip_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim fileName As String = spanOrderId.InnerText & ".inv"
            Dim folderPath As String = Server.MapPath("~/file/inv/")

            Dim fullPath As String = Path.Combine(folderPath, fileName)

            Dim q As String = Chr(34)

            Dim address1 As String = orderCfg.GetItemData("SELECT Street FROM CustomerAddress WHERE CustomerId = '" + lblCustomerId.Text + "' AND [Primary] = 1")
            If String.IsNullOrEmpty(address1) Then
                address1 = "-"
            End If

            Dim address3 As String = orderCfg.GetItemData("SELECT Suburb + ' ' + States FROM CustomerAddress WHERE CustomerId = '" + lblCustomerId.Text + "' AND [Primary] = 1")
            If String.IsNullOrEmpty(address3) Then
                address3 = "-"
            End If
            Dim postCode As String = orderCfg.GetItemData("SELECT PostCode FROM CustomerAddress WHERE CustomerId = '" + lblCustomerId.Text + "' AND [Primary] = 1")
            If String.IsNullOrEmpty(postCode) Then
                postCode = "-"
            End If

            Dim content As String = q & "TYPE 2" & q & ","
            content += vbCrLf
            content += q & "" & q & "," &
                        q & "O" & q & "," &
                        q & "" & q & "," &
                        q & lblMnetId.Text & q & "," &
                        q & spanOrderNumber.InnerText & q & "," &
                        q & address1 & q & "," &
                        q & "" & q & "," &
                        q & address3 & q & "," &
                        q & postCode & q & "," &
                        q & spanOrderName.InnerText & q & "," &
                        q & spanJobId.InnerText & q & "," &
                        q & "" & q & "," &
                        q & "" & q & "," &
                        q & "" & q & ","

            Dim detailData As DataSet = orderCfg.GetListData("SELECT OrderDetails_Shutters.*, Products.Name AS ProductName, Blinds.Name AS BlindName FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id INNER JOIN Designs ON Products.DesignId = Designs.Id INNER JOIN dbo.Blinds ON Products.BlindId = Blinds.Id WHERE OrderDetails_Shutters.HeaderId='" + lblHeaderId.Text + "' AND OrderDetails_Shutters.Active=1 ORDER BY OrderDetails_Shutters.Number ASC")
            If detailData.Tables(0).Rows.Count > 0 Then
                For i As Integer = 0 To detailData.Tables(0).Rows.Count - 1
                    Dim finalCost As Decimal = detailData.Tables(0).Rows(i)("FinalCost")
                    Dim finalCostString As String = finalCost.ToString(CultureInfo.InvariantCulture)

                    Dim itemId As String = detailData.Tables(0).Rows(i)("Id").ToString()
                    Dim productId As String = detailData.Tables(0).Rows(i)("ProductId").ToString()
                    Dim production As String = detailData.Tables(0).Rows(i)("Production").ToString()
                    Dim micronetId As String = detailData.Tables(0).Rows(i)("MicronetId").ToString()

                    If String.IsNullOrEmpty(micronetId) Then
                        micronetId = orderCfg.ManualItemIdMnet(lblHeaderId.Text, productId, production)
                    End If

                    Dim totalBlinds As Integer = 1
                    If Not IsDBNull(detailData.Tables(0).Rows(i)("TotalBlinds")) Then
                        totalBlinds = Convert.ToInt32(detailData.Tables(0).Rows(i)("TotalBlinds"))
                    End If

                    Dim productName As String = BindProductSlip(itemId, 1)

                    content += vbCrLf
                    content += q & "I" & q & "," &
                        q & "9" & q & "," &
                        q & finalCostString & q & "," &
                        q & micronetId & q & "," &
                        q & "1" & q & "," &
                        q & "" & q & "," &
                        q & productName & q & "," &
                        q & 0 & q & ","

                    If totalBlinds > 1 Then
                        For j As Integer = 2 To totalBlinds
                            productName = BindProductSlip(itemId, j)

                            content += vbCrLf
                            content += q & "T" & q & "," & q & productName & q & ","
                        Next
                    End If
                Next
            End If

            File.WriteAllText(fullPath, content)

            Dim micronetCfg As New MicronetConfig
            Dim key As String = Server.MapPath("~/setting/ssh_key.ppk")
            Dim inv As String = Path.Combine(folderPath, fileName)

            Dim message As String = micronetCfg.Upload(key, inv)

            If message.Contains("SUCCEED") Then
                BindDataOrder(lblHeaderId.Text)
                BindDesignType()
                MessageSuccess(True, message)
                Exit Sub
            End If

            If message.Contains("FAILED") Then
                BindDataOrder(lblHeaderId.Text)
                BindDesignType()
                MessageError(True, message)
                Exit Sub
            End If

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitSlip_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitQuoteDetail_Click(sender As Object, e As EventArgs)
        MessageError_QuoteDetail(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showQuoteDetail(); };"
        Try
            Dim discount As Decimal
            Dim installation As Decimal
            Dim checkMeasure As Decimal
            Dim freight As Decimal

            If Not txtQuoteDiscount.Text = "" Then
                If InStr(txtQuoteDiscount.Text, ",") > 0 Then
                    MessageError_QuoteDetail(True, "PLEASE DON'T USE COMMA (,) FOR SEPARATOR ON DISCOUNT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteDetail", thisScript, True)
                    Exit Sub
                End If
                If Not Decimal.TryParse(txtQuoteDiscount.Text, discount) OrElse discount < 0 Then
                    MessageError_QuoteDetail(True, "PLEASE CHECK YOUR DISCOUNT FORMAT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteDetail", thisScript, True)
                    Exit Sub
                End If
            End If

            If Not txtQuoteInstallation.Text = "" Then
                If InStr(txtQuoteInstallation.Text, ",") > 0 Then
                    MessageError_QuoteDetail(True, "PLEASE DON'T USE COMMA (,) FOR SEPARATOR ON INSTALLATION !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteDetail", thisScript, True)
                    Exit Sub
                End If

                If Not Decimal.TryParse(txtQuoteInstallation.Text, installation) OrElse installation < 0 Then
                    MessageError_QuoteDetail(True, "PLEASE CHECK YOUR INSTALLATION FORMAT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteDetail", thisScript, True)
                    Exit Sub
                End If
            End If

            If Not txtQuoteCheckMeasure.Text = "" Then
                If InStr(txtQuoteCheckMeasure.Text, ",") > 0 Then
                    MessageError_QuoteDetail(True, "PLEASE DON'T USE COMMA (,) FOR SEPARATOR ON CHECK MEASURE !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteDetail", thisScript, True)
                    Exit Sub
                End If

                If Not Decimal.TryParse(txtQuoteCheckMeasure.Text, checkMeasure) OrElse checkMeasure < 0 Then
                    MessageError_QuoteDetail(True, "PLEASE CHECK YOUR CHECK MEASURE FORMAT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteDetail", thisScript, True)
                    Exit Sub
                End If
            End If

            If Not txtQuoteFreight.Text = "" Then
                If InStr(txtQuoteFreight.Text, ",") > 0 Then
                    MessageError_QuoteDetail(True, "PLEASE DON'T USE COMMA (,) FOR SEPARATOR ON FREIGHT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteDetail", thisScript, True)
                    Exit Sub
                End If

                If Not Decimal.TryParse(txtQuoteFreight.Text, freight) OrElse freight < 0 Then
                    MessageError_QuoteDetail(True, "PLEASE CHECK YOUR CHECK FREIGHT FORMAT !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteDetail", thisScript, True)
                    Exit Sub
                End If
            End If

            If msgErrorQuoteDetail.InnerText = "" Then
                If txtQuoteDiscount.Text = "" Then : txtQuoteDiscount.Text = 0 : End If
                If txtQuoteInstallation.Text = "" Then : txtQuoteInstallation.Text = 0 : End If
                If txtQuoteCheckMeasure.Text = "" Then : txtQuoteCheckMeasure.Text = 0 : End If
                If txtQuoteFreight.Text = "" Then : txtQuoteFreight.Text = 0 : End If

                txtQuoteDiscount.Text.Replace(".", ",")
                txtQuoteInstallation.Text.Replace(".", ",")
                txtQuoteCheckMeasure.Text.Replace(".", ",")
                txtQuoteFreight.Text.Replace(".", ",")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderQuotes SET Email=@Email, Phone=@Phone, Address=@Address, Suburb=@Suburb, States=@States, PostCode=@PostCode, Discount=@Discount, Installation=@Installation, CheckMeasure=@CheckMeasure, Freight=@Freight WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@Email", txtQuoteEmail.Text)
                        myCmd.Parameters.AddWithValue("@Phone", txtQuotePhone.Text)
                        myCmd.Parameters.AddWithValue("@Address", txtQuoteAddress.Text)
                        myCmd.Parameters.AddWithValue("@Suburb", txtQuoteSuburb.Text)
                        myCmd.Parameters.AddWithValue("@States", ddlQuoteStates.SelectedValue)
                        myCmd.Parameters.AddWithValue("@PostCode", txtQuotePostCode.Text)
                        myCmd.Parameters.AddWithValue("@Discount", txtQuoteDiscount.Text)
                        myCmd.Parameters.AddWithValue("@Installation", txtQuoteInstallation.Text)
                        myCmd.Parameters.AddWithValue("@CheckMeasure", txtQuoteCheckMeasure.Text)
                        myCmd.Parameters.AddWithValue("@Freight", txtQuoteFreight.Text)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            MessageError_QuoteDetail(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_QuoteDetail(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError_QuoteDetail(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                        MessageError_QuoteDetail(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "btnSubmitQuoteDetail_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteDetail", thisScript, True)
        End Try
    End Sub

    Protected Sub btnQuoteDownload_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If gvList.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM !")
                Exit Sub
            End If

            Dim quoteCfg As New QuoteConfig
            Dim fileName As String = "Quote_" & spanOrderId.InnerText & ".pdf"
            Dim pdfFilePath As String = Server.MapPath("~/File/Order/" & fileName)

            quoteCfg.BindContentQuoteCustomer(lblHeaderId.Text, pdfFilePath)

            Response.ContentType = "application/pdf"
            Response.AddHeader("Content-Disposition", "attachment; filename=""" & fileName & """")
            Response.TransmitFile(pdfFilePath)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                        MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "btnQuoteDownload_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitStatusAdditional_Click(sender As Object, e As EventArgs)
        MessageError_StatusAdditional(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showStatusAdditional(); };"
        Try
            If spanStatusOrder.InnerText = "New Order" Then
                If ddlStatusAdditional.SelectedValue = "" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Approved=@Approved WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@Approved", "1")

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    If spanStatusAdditional.InnerText = "Waiting for Deposit" Then
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Deposit=@Deposit WHERE Id=@Id")
                                myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                                myCmd.Parameters.AddWithValue("@Deposit", "1")

                                myCmd.Connection = thisConn
                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                                thisConn.Close()
                            End Using
                        End Using
                    End If

                    Dim changeStatus As String = spanStatusAdditional.InnerText & " to Clear Status"
                    Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), changeStatus}
                    orderCfg.Log_Orders(dataLog)

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    Response.Redirect("~/order/detail", False)
                    Exit Sub
                End If

                If ddlStatusAdditional.SelectedValue = "Waiting for Deposit" Then
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Deposit=@Deposit WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@Deposit", "0")

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using

                    Dim changeStatus As String = spanStatusAdditional.InnerText & " to " & ddlStatusAdditional.SelectedValue
                    If spanStatusAdditional.InnerText = "" Then
                        changeStatus = "Change to " & ddlStatusAdditional.SelectedValue
                    End If

                    Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), changeStatus}
                    orderCfg.Log_Orders(dataLog)

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    Response.Redirect("~/order/detail", False)
                    Exit Sub
                End If

                If ddlStatusAdditional.SelectedValue = "On Hold - Order Detail Query" Or ddlStatusAdditional.SelectedValue = "On Hold - Customer Request" Or ddlStatusAdditional.SelectedValue = "On Hold - Waiting for Template" Or ddlStatusAdditional.SelectedValue = "On Hold - Waiting for Colour Swatch" Or ddlStatusAdditional.SelectedValue = "On Hold - Waiting for Drawing Approval" Then
                    Dim changeStatus As String = spanStatusAdditional.InnerText & " to " & ddlStatusAdditional.SelectedValue
                    Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), changeStatus}
                    orderCfg.Log_Orders(dataLog)

                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    Response.Redirect("~/order/detail", False)
                    Exit Sub
                End If
            End If

            If spanStatusOrder.InnerText = "In Production" Then
                Dim changeStatus As String = spanStatusAdditional.InnerText & " to " & ddlStatusAdditional.SelectedValue
                Dim dataLog As Object() = {lblHeaderId.Text, "", Session("LoginId").ToString(), changeStatus}
                orderCfg.Log_Orders(dataLog)

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
                Response.Redirect("~/order/detail", False)
                Exit Sub
            End If
        Catch ex As Exception
            MessageError_StatusAdditional(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_StatusAdditional(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitStatusAdditional_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showStatusAdditional", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitIntenalNote_Click(sender As Object, e As EventArgs)
        MessageError_InternalNote(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showInternalNote(); };"
        Try
            If InStr(txtInternalNote.Text, "&") > 0 Then
                MessageError_InternalNote(True, "PLEASE DON'T USE CHARACTER [&] !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showInternalNote", thisScript, True)
                Exit Sub
            End If

            If msgErrorInternalNote.InnerText = "" Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET InternalNote=@InternalNote WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@InternalNote", txtInternalNote.Text.Trim())

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            MessageError_InternalNote(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_InternalNote(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitIntenalNote_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showInternalNote", thisScript, True)
        End Try
    End Sub

    Protected Sub btnQuotePrint_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If gvList.Rows.Count = 0 Then
                MessageError(True, "PLEASE ADD MINIMAL 1 ITEM !")
                Exit Sub
            End If

            Dim quoteCfg As New QuoteConfig

            Dim fileDirectory As String = Server.MapPath("~/File/Order/")
            Dim fileName As String = Trim("Order" & "_" & spanOrderId.InnerText & ".pdf")

            Dim pdfFilePath As String = fileDirectory & fileName

            quoteCfg.BindContentQuote(lblHeaderId.Text, pdfFilePath)

            Response.Clear()
            Dim url As String = "/order/preview"
            Session("printPreview") = fileDirectory + fileName

            Dim sb As New StringBuilder()
            sb.Append("<script type = 'text/javascript'>")
            sb.Append("window.open('")
            sb.Append(url)
            sb.Append("');")
            sb.Append("</script>")
            ClientScript.RegisterStartupScript(Me.GetType(), "script", sb.ToString())

        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                        MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "btnQuotePrint_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitQuoteEmail_Click(sender As Object, e As EventArgs)
        MessageError_QuoteEmail(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showQuoteEmail(); };"
        Try
            If txtEmailQuoteTo.Text = "" Then
                MessageError_QuoteEmail(True, "DESTINATION EMAIL ADDRESS REQUIRED !")
                txtEmailQuoteFrom.Focus()
                ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteEmail", thisScript, True)
                Exit Sub
            End If

            If Not mailCfg.IsValidEmail(txtEmailQuoteTo.Text) Then
                MessageError_QuoteEmail(True, "PLEASE CHECK YOUR DESTINATION EMAIL ADDRESS !")
                txtEmailQuoteFrom.Focus()
                ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteEmail", thisScript, True)
                Exit Sub
            End If

            If Not txtEmailQuoteCC.Text = "" Then
                If Not mailCfg.IsValidEmail(txtEmailQuoteCC.Text) Then
                    MessageError_QuoteEmail(True, "PLEASE CHECK YOUR CC EMAIL ADDRESS !")
                    txtEmailQuoteCC.Focus()
                    ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteEmail", thisScript, True)
                    Exit Sub
                End If
            End If

            If msgErrorQuoteEmail.InnerText = "" Then
                Dim fileDirectory As String = Server.MapPath("~/File/Order/")
                Dim quoteFileName As String = Trim("Quote" & " - " & spanOrderId.InnerText & ".pdf")
                Dim previewFileName As String = Trim("Order" & " - " & spanOrderId.InnerText & ".pdf")

                Dim quoteFilePath As String = fileDirectory & quoteFileName
                Dim previewFilePath As String = fileDirectory & previewFileName

                Dim quoteCfg As New QuoteConfig
                quoteCfg.BindContentQuote(lblHeaderId.Text, quoteFilePath)

                If lblOrderType.Text = "RBR" Or lblOrderType.Text = "Blinds" Or lblOrderType.Text = "Zebra Blinds" Or lblOrderType.Text = "Veri Shades" Or lblOrderType.Text = "Curtain" Then
                    Dim previewCfg As New PreviewConfig
                    previewCfg.BindContent(lblHeaderId.Text, previewFilePath)
                End If

                If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                    Dim previewCfg As New ShuttersPreviewConfig
                    previewCfg.BindContent(lblHeaderId.Text, previewFilePath)
                End If

                mailCfg.MailQuote(lblHeaderId.Text, quoteFilePath, previewFilePath, txtEmailQuoteTo.Text.Trim(), txtEmailQuoteCC.Text.Trim())
            End If
        Catch ex As Exception
            MessageError_QuoteEmail(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_QuoteEmail(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitQuoteEmail_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showQuoteEmail", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDeposit_Click(sender As Object, e As EventArgs)
        MessageError_Deposit(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showDeposit(); };"
        Try
            If txtDepositTo.Text = "" Then
                MessageError_Deposit(True, "DESTINATION EMAIL ADDRESS REQUIRED !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showDeposit", thisScript, True)
                Exit Sub
            End If

            If Not mailCfg.IsValidEmail(txtDepositTo.Text) Then
                MessageError_Deposit(True, "PLEASE CHECK YOUR DESTINATION EMAIL ADDRESS !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showDeposit", thisScript, True)
                Exit Sub
            End If
            If Not txtDepositCc.Text = "" Then
                If Not mailCfg.IsValidEmail(txtDepositCc.Text) Then
                    MessageError_Deposit(True, "PLEASE CHECK YOUR CC EMAIL ADDRESS !")
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDeposit", thisScript, True)
                    Exit Sub
                End If
            End If

            If msgErrorDeposit.InnerText = "" Then
                Dim fileName As String = Trim("Deposit" & "-" & spanOrderId.InnerText & ".pdf")
                Dim pdfFilePath As String = Server.MapPath("~/File/Order/" & fileName)

                Dim depositRequest As New DepositRequestConfig
                depositRequest.BindContentDeposit(lblHeaderId.Text, pdfFilePath)
                mailCfg.MailDeposit(lblHeaderId.Text, pdfFilePath, txtDepositTo.Text.Trim(), txtDepositCc.Text.Trim())

                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            MessageError_Deposit(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_Deposit(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitDeposit_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showDeposit", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitAddItem_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesign.SelectedValue = "" Then
                Response.Redirect("~/order/detail", False)
                Exit Sub
            End If

            Session("headerId") = lblHeaderId.Text
            Session("itemAction") = "AddItem"
            Session("designId") = UCase(ddlDesign.SelectedValue).ToString()

            Dim page As String = orderCfg.GetItemData("SELECT Page FROM Designs WHERE Id='" + UCase(ddlDesign.SelectedValue).ToString() + "'")
            Dim name As String = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id='" + UCase(ddlDesign.SelectedValue).ToString() + "'")

            Session("itemProduction") = ""
            If gvList.Rows.Count > 0 Then
                Dim production As String = orderCfg.GetItemData("SELECT TOP 1 Production FROM OrderDetails_Shutters WHERE HeaderId = '" + lblHeaderId.Text + "' AND Active = 1 ORDER BY Id ASC")
                Session("itemProduction") = production
            End If
            If lblCustomerId.Text = "LS-A388" Or lblCustomerId.Text = "LS-A346" Or lblCustomerId.Text = "LS-A194" Then
                Session("itemProduction") = "Orion"
            End If
            If name = "Vertical" And (lblCustomerId.Text = "LS-A195" Or lblCustomerId.Text = "LS-A307") Then
                Session("itemProduction") = "Orion"
            End If
            Response.Redirect(page, False)
            Exit Sub
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "" Or lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                        MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "btnSubmitAddItem_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnService_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Session("headerId") = lblHeaderId.Text
            Session("itemAction") = "AddItem"
            Session("designId") = "6C0B3347-9730-45CA-905C-5EF682CD06EA"
            Response.Redirect("~/order/additional", False)
            Exit Sub
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnService_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitEditPricing_Click(sender As Object, e As EventArgs)
        MessageError_EditPricing(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showEditPricing(); };"
        Try
            If InStr(txtOverridePrice.Text, ",") > 0 Then
                MessageError_EditPricing(True, "PLEASE DON'T USE COMMA (,) FOR OVERRIDE PRICE !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showEditPricing", thisScript, True)
                Exit Sub
            End If

            If InStr(txtDiscount.Text, ",") > 0 Then
                MessageError_EditPricing(True, "PLEASE DON'T USE COMMA (,) FOR DISCOUNT !")
                ClientScript.RegisterStartupScript(Me.GetType(), "showEditPricing", thisScript, True)
                Exit Sub
            End If

            Dim overridePrice As Decimal
            Dim overridePriceInput As String = txtOverridePrice.Text

            If Not Decimal.TryParse(txtOverridePrice.Text, overridePrice) OrElse overridePrice <= 0 Then
                Dim thisPrice As Decimal = orderCfg.GetItemData_Decimal("SELECT Cost FROM OrderDetails_Shutters WHERE Id = '" + lblItemId.Text + "'")
                Dim costOverride As Decimal = thisPrice

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails_Shutters SET CostOverride=@CostOverride WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblItemId.Text)
                        myCmd.Parameters.AddWithValue("@CostOverride", costOverride)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                orderCfg.UpdateFinalCost(lblItemId.Text)
                orderCfg.ResetPriceDetail_Edited(lblHeaderId.Text, lblItemId.Text, "Override")
            Else
                Dim costOverride As Decimal = txtOverridePrice.Text

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails_Shutters SET CostOverride=@CostOverride WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblItemId.Text)
                        myCmd.Parameters.AddWithValue("@CostOverride", costOverride)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                orderCfg.UpdateFinalCost(lblItemId.Text)
                orderCfg.ResetPriceDetail_Edited(lblHeaderId.Text, lblItemId.Text, "Override")

                Dim priceDetail As Object() = {lblHeaderId.Text, lblItemId.Text, 0, "Override", "Overridden", costOverride}
                orderCfg.InsertPriceDetail(priceDetail)
            End If

            Dim discount As Decimal
            If Not Decimal.TryParse(txtDiscount.Text, discount) OrElse discount <= 0 Then
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails_Shutters SET Discount=@Discount WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblItemId.Text)
                        myCmd.Parameters.AddWithValue("@Discount", 0)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                orderCfg.ResetPriceDetail_Edited(lblHeaderId.Text, lblItemId.Text, "Discount")
                orderCfg.UpdateFinalCost(lblItemId.Text)
            Else
                Dim costDiscount As Decimal = txtDiscount.Text
                Dim discountDesc As String = "Discount"
                If ddlDiscountType.SelectedValue = "%" Then
                    Dim costOverride As Decimal = orderCfg.GetItemData_Decimal("SELECT CostOverride FROM OrderDetails_Shutters WHERE Id = '" + lblItemId.Text + "'")
                    costDiscount = costOverride * txtDiscount.Text / 100
                    discountDesc = "Discount " & txtDiscount.Text & "%"
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails_Shutters SET Discount=@Discount WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblItemId.Text)
                        myCmd.Parameters.AddWithValue("@Discount", costDiscount)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                orderCfg.ResetPriceDetail_Edited(lblHeaderId.Text, lblItemId.Text, "Discount")
                Dim priceDetail As Object() = {lblHeaderId.Text, lblItemId.Text, 0, "Discount", discountDesc, costDiscount}
                orderCfg.InsertPriceDetail(priceDetail)
                orderCfg.UpdateFinalCost(lblItemId.Text)
            End If

            If msgErrorEditPricing.InnerText = "" Then
                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            MessageError_EditPricing(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError_EditPricing(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitEditPricing_Click", Session("LoginId"), ex.ToString())
            End If
            ClientScript.RegisterStartupScript(Me.GetType(), "showEditPricing", thisScript, True)
        End Try
    End Sub

    Protected Sub btnSubmitDeleteItem_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim itemId As String = txtDeleteId.Text
            Dim itemNumber As String = txtDeleteNumber.Text

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails_Shutters SET Number = 0, Active=0 WHERE Id=@Id UPDATE OrderAuthorizations SET Active=0 WHERE ItemId=@Id")
                    myCmd.Parameters.AddWithValue("@Id", itemId)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails_Shutters SET Number = Number - 1 WHERE HeaderId=@HeaderId AND Number>@Number AND Active = 1")
                    myCmd.Parameters.AddWithValue("@HeaderId", lblHeaderId.Text)
                    myCmd.Parameters.AddWithValue("@Number", itemNumber)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            orderCfg.UpdateProductType(lblHeaderId.Text)

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                        MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "btnSubmitDeleteItem_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmitOrion_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim itemId As String = txtProductionId.Text
            Dim production As String = ddlProduction.SelectedValue

            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderDetails_Shutters SET Production=@Production WHERE Id=@Id")
                    myCmd.Parameters.AddWithValue("@Id", itemId)
                    myCmd.Parameters.AddWithValue("@Production", production)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "btnSubmitOrion_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindDataOrder(lblHeaderId.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                        MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "gvList_PageIndexChanging", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim ItemId As String = e.CommandArgument.ToString()

            If e.CommandName = "Detail" Then
                MessageError(False, String.Empty)
                Try
                    Dim thisData As DataSet = orderCfg.GetListData("SELECT Products.DesignId FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderDetails_Shutters.Id='" + ItemId + "' AND OrderDetails_Shutters.Active=1")
                    Dim designId As String = thisData.Tables(0).Rows(0).Item("DesignId").ToString()
                    Dim page As String = orderCfg.GetItemData("SELECT Page FROM Designs WHERE Id='" + UCase(designId).ToString() + "'")

                    Session("headerId") = lblHeaderId.Text
                    Session("itemId") = ItemId
                    Session("designId") = designId
                    Session("itemAction") = "ViewItem"
                    Session("orderType") = Request.QueryString("ordertype")

                    Dim production As String = orderCfg.GetItemData("SELECT TOP 1 Production FROM OrderDetails_Shutters WHERE HeaderId = '" + lblHeaderId.Text + "' AND Active=1 ORDER BY Id ASC")
                    Session("itemProduction") = production

                    If gvList.Rows.Count = 1 And spanStatusOrder.InnerText = "Unsubmitted" Then
                        Session("itemProduction") = ""
                    End If
                    If lblCustomerId.Text = "LS-A388" Or lblCustomerId.Text = "LS-A346" Or lblCustomerId.Text = "LS-A194" Then
                        Session("itemProduction") = "Orion"
                    End If

                    If spanStatusOrder.InnerText = "Unsubmitted" Then
                        Session("itemAction") = "EditItem"
                        If Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Then
                            Session("itemAction") = "ViewItem"
                            If lblCreatedBy.Text = Session("LoginId") Then
                                Session("itemAction") = "EditItem"
                            End If
                        End If
                    End If
                    If spanStatusOrder.InnerText = "New Order" Then
                        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Or Session("RoleName") = "Customer Service" Then
                            Session("itemAction") = "EditItem"
                        End If
                        If Session("RoleName") = "Customer" And spanStatusAdditional.InnerText = "On Hold - Customer Request" Then
                            Session("itemAction") = "EditItem"
                        End If
                    End If
                    If spanStatusOrder.InnerText = "In Production" Then
                        If Session("RoleName") = "Administrator" And (Session("LevelName") = "Leader" Or Session("LevelName") = "Super Admin")Then
                            Session("itemAction") = "EditItem"
                        End If
                    End If
                    Response.Redirect(page, False)
                    Exit Sub
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                            If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                                MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                            End If
                        End If
                        mailCfg.MailError(Page.Title, "linkDetail_Click", Session("LoginId"), ex.ToString())
                    End If
                End Try
            ElseIf e.CommandName = "Copy" Then
                MessageError(False, String.Empty)
                Try
                    Dim thisData As DataSet = orderCfg.GetListData("SELECT Products.DesignId FROM OrderDetails_Shutters LEFT JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id WHERE OrderDetails_Shutters.Id='" + ItemId + "' AND OrderDetails_Shutters.Active=1")
                    Dim designId As String = thisData.Tables(0).Rows(0).Item("DesignId").ToString()
                    Dim page As String = orderCfg.GetItemData("SELECT Page FROM Designs WHERE Id='" + UCase(designId).ToString() + "'")

                    Session("headerId") = lblHeaderId.Text
                    Session("designId") = designId
                    Session("itemId") = ItemId
                    Session("itemAction") = "CopyItem"

                    Response.Redirect(page, False)
                    Exit Sub
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError(True, "Please contact IT at reza@bigblinds.co.id")
                        If Session("RoleName") = "Customer" Then
                            MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                            If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                                MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                            End If
                        End If
                        mailCfg.MailError(Page.Title, "linkCopy_Click", Session("LoginId"), ex.ToString())
                    End If
                End Try
            ElseIf e.CommandName = "InfoPrice" Then
                MessageError_Pricing(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showDetailPrice(); };"
                Try
                    Dim queryDetailsPrice As String = "SELECT *, FORMAT(Price, 'C', 'en-US') AS FinalPrice FROM OrderDetailPrices WHERE ItemId = '" + ItemId + "' AND Active = 1 ORDER BY CASE WHEN ItemNumber = 0 THEN 10 ELSE ItemNumber END, CASE WHEN Type = 'Base' THEN 1 WHEN Type = 'DiscountFabric' THEN 2 WHEN Type = 'Surcharge' THEN 3 WHEN Type = 'DiscountProduct' THEN 4 WHEN Type = 'DiscountFabric' THEN 5 WHEN Type = 'Override' THEN 6 WHEN Type = 'Discount' THEN 7 ELSE 8 END ASC"

                    gvListDetailPrice.DataSource = orderCfg.GetListData(queryDetailsPrice)
                    gvListDetailPrice.DataBind()

                    gvListDetailPrice.Columns(1).Visible = False ' TYPE
                    If Session("RoleName") = "Administrator" Then
                        gvListDetailPrice.Columns(1).Visible = True ' TYPE
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailPrice", thisScript, True)
                Catch ex As Exception
                    MessageError_Pricing(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError_Pricing(True, "Please contact IT Support at reza@bigblinds.co.id")
                        If Session("RoleName") = "Customer" Then
                            MessageError_Pricing(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                            If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                                MessageError_Pricing(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                            End If
                        End If
                        mailCfg.MailError(Page.Title, "linkPriceInfo_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showDetailPrice", thisScript, True)
                End Try
            ElseIf e.CommandName = "EditPrice" Then
                MessageError_EditPricing(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showEditPricing(); };"
                Try
                    lblItemId.Text = ItemId

                    Dim detailData As DataSet = orderCfg.GetListData("SELECT * FROM OrderDetails_Shutters WHERE Id='" + ItemId + "'")
                    Dim productId As String = detailData.Tables(0).Rows(0).Item("ProductId").ToString()
                    Dim originalPrice As Decimal = detailData.Tables(0).Rows(0).Item("Cost")
                    lblBasePrice.Text = "$" & originalPrice.ToString().Replace(",", ".")

                    Dim overrridePrice As Decimal = orderCfg.GetItemData_Decimal("SELECT Price FROM OrderDetailPrices WHERE HeaderId = '" + lblHeaderId.Text + "' AND ItemId = '" + ItemId + "' AND Type = 'Override'")
                    If overrridePrice > 0 Then
                        txtOverridePrice.Text = overrridePrice.ToString().Replace(",", ".")
                    End If

                    ddlDiscountType.SelectedValue = "$"
                    txtDiscount.Text = ""
                    Dim discountValue As Decimal = orderCfg.GetItemData_Decimal("SELECT Price FROM OrderDetailPrices WHERE HeaderId = '" + lblHeaderId.Text + "' AND ItemId = '" + ItemId + "' AND Type = 'Discount'")
                    If discountValue > 0 Then
                        Dim discountDesc As String = orderCfg.GetItemData("SELECT Description FROM OrderDetailPrices WHERE HeaderId = '" + lblHeaderId.Text + "' AND ItemId = '" + ItemId + "' AND Type = 'Discount'")
                        txtDiscount.Text = discountValue.ToString().Replace(",", ".")

                        If InStr(discountDesc, "%") > 0 Then
                            ddlDiscountType.SelectedValue = "%"
                            txtDiscount.Text = discountDesc.Replace("Discount ", "").Replace("%", "").Trim()
                        End If
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showEditPricing", thisScript, True)
                Catch ex As Exception
                    MessageError_EditPricing(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError_EditPricing(True, "Please contact IT Support at reza@bigblinds.co.id")
                        mailCfg.MailError(Page.Title, "linkEditPricing_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showEditPricing", thisScript, True)
                End Try
            ElseIf e.CommandName = "Log" Then
                MessageError_Log(False, String.Empty)
                Dim thisScript As String = "window.onload = function() { showLog(); };"
                Try
                    gvListLogs.DataSource = orderCfg.GetListData("SELECT * FROM Log_Orders WHERE HeaderId='" + lblHeaderId.Text + "' AND ItemId = '" + ItemId + "' ORDER BY ActionDate ASC")
                    gvListLogs.DataBind()
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                Catch ex As Exception
                    MessageError_Log(True, ex.ToString())
                    If Not Session("RoleName") = "Administrator" Then
                        MessageError_Log(True, "Please contact IT Support at reza@bigblinds.co.id")
                        If Session("RoleName") = "Customer" Then
                            MessageError_Log(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                            If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                                MessageError_Log(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                            End If
                        End If
                        mailCfg.MailError(Page.Title, "linkLog_Click", Session("LoginId"), ex.ToString())
                    End If
                    ClientScript.RegisterStartupScript(Me.GetType(), "showLog", thisScript, True)
                End Try
            End If
        End If
    End Sub

    Protected Sub lvOtorisasi_ItemCommand(sender As Object, e As ListViewCommandEventArgs)
        MessageError(False, String.Empty)
        Try
            If e.CommandName = "Authorize" Then
                Dim thisId As String = e.CommandArgument
                Dim status As String = "Authorized"
                Dim loginId As String = UCase(Session("LoginId")).ToString()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderAuthorizations SET Status=@Status, LoginId=@LoginId WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Status", status)
                        myCmd.Parameters.AddWithValue("@LoginId", loginId)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                BindDataOrder(lblHeaderId.Text)

                Dim myScript As String = "window.onload = function() { showCanvasOtorisasi(); };"
                ClientScript.RegisterStartupScript(Me.GetType(), "showCanvasOtorisasi", myScript, True)
            End If

            If e.CommandName = "Decline" Then
                Dim thisId As String = e.CommandArgument
                Dim status As String = "Declined"
                Dim loginId As String = UCase(Session("LoginId")).ToString()

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderAuthorizations SET Status=@Status, LoginId=@LoginId WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Status", status)
                        myCmd.Parameters.AddWithValue("@LoginId", loginId)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                BindDataOrder(lblHeaderId.Text)

                Dim myScript As String = "window.onload = function() { showCanvasOtorisasi(); };"
                ClientScript.RegisterStartupScript(Me.GetType(), "showCanvasOtorisasi", myScript, True)
            End If

            If e.CommandName = "Reset" Then
                Dim thisId As String = e.CommandArgument
                Dim status As String = ""
                Dim loginId As String = ""

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderAuthorizations SET Status=@Status, LoginId=@LoginId WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", thisId)
                        myCmd.Parameters.AddWithValue("@Status", status)
                        myCmd.Parameters.AddWithValue("@LoginId", loginId)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Approved=@Approved WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@Approved", "2")

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                ddlStatusAdditional.SelectedValue = "On Hold - Order Detail Query"
                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using

                BindDataOrder(lblHeaderId.Text)

                Dim myScript As String = "window.onload = function() { showCanvasOtorisasi(); };"
                ClientScript.RegisterStartupScript(Me.GetType(), "showCanvasOtorisasi", myScript, True)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "lvOtorisasi_ItemCommand", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDesignType()
        ddlDesign.Items.Clear()
        Try
            If Not lblCustomerId.Text = "" Then
                Dim ordertype As String = Request.QueryString("ordertype")
                Dim types As String = StrConv(ordertype, VbStrConv.ProperCase)

                Dim thisQuery As String = "SELECT Designs.Id, UPPER(Designs.Name) AS NameText FROM CustomerProductAccess CROSS APPLY STRING_SPLIT(CustomerProductAccess.DesignId, ',') AS designArray INNER JOIN Designs ON designArray.VALUE = Designs.Id WHERE CustomerProductAccess.Id = '" + Session("CustomerId").ToString() + "' AND Designs.Type <> 'Additional' AND Designs.Type = '" +  types + "' AND Designs.Active = 1 ORDER BY Designs.Name ASC"



                If gvList.Rows.Count > 0 Then
                    Dim type As String = orderCfg.GetItemData("SELECT TOP 1 Designs.Type FROM OrderDetails_Shutters INNER JOIN Products ON OrderDetails_Shutters.ProductId = Products.Id INNER JOIN Designs ON Products.DesignId = Designs.Id WHERE OrderDetails_Shutters.HeaderId = '" + lblHeaderId.Text + "' AND OrderDetails_Shutters.Active = 1 ORDER BY OrderDetails_Shutters.Id ASC")

                    thisQuery = "SELECT Designs.Id, UPPER(Designs.Name) AS NameText FROM CustomerProductAccess CROSS APPLY STRING_SPLIT(CustomerProductAccess.DesignId, ',') AS designArray INNER JOIN Designs ON designArray.VALUE = Designs.Id WHERE CustomerProductAccess.Id = '" + Session("CustomerId").ToString() + "' AND Designs.Type <> 'Additional' AND Type = '" + type + "' AND Designs.Active = 1 ORDER BY Designs.Name ASC"

                    If lblCustomerId.Text = "LS-A195" Or lblCustomerId.Text = "LS-A307" Then
                        Dim production As String = orderCfg.GetItemData("SELECT TOP 1 Production FROM OrderDetails_Shutters WHERE HeaderId = '" + lblHeaderId.Text + "' AND Active=1 ORDER BY Id ASC")
                        If type = "Blinds" And production = "JKT" Then
                            thisQuery = "SELECT Designs.Id, UPPER(Designs.Name) AS NameText FROM CustomerProductAccess CROSS APPLY STRING_SPLIT(CustomerProductAccess.DesignId, ',') AS designArray INNER JOIN Designs ON designArray.VALUE = Designs.Id WHERE CustomerProductAccess.Id = '" + Session("CustomerId").ToString() + "' AND Designs.Type <> 'Additional' AND Designs.Name <> 'Vertical' AND Type = '" + type + "' AND Designs.Active = 1 ORDER BY Designs.Name ASC"
                        End If
                    End If
                End If

                ddlDesign.DataSource = orderCfg.GetListData(thisQuery)
                ddlDesign.DataTextField = "NameText"
                ddlDesign.DataValueField = "Id"
                ddlDesign.DataBind()

                If ddlDesign.Items.Count > 0 Then
                    ddlDesign.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "" Or lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                        MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "BindDesignType", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataOrder(headerId As String)
        AllMessageError(False, String.Empty)
        Try
            Dim headerData As DataSet = orderCfg.GetListData("SELECT * FROM view_order_headers WHERE Id = '" + headerId + "' AND OrderType = '" + lblOrderType.Text + "'")

            If headerData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            spanCustomerName.InnerText = orderCfg.GetItemData("SELECT Name FROM Customers WHERE Id = '" + headerData.Tables(0).Rows(0).Item("CustomerId").ToString() + "'")
            spanOrderId.InnerText = headerData.Tables(0).Rows(0).Item("OrderId").ToString()
            spanOrderNumber.InnerText = headerData.Tables(0).Rows(0).Item("OrderNumber").ToString()
            spanOrderName.InnerText = headerData.Tables(0).Rows(0).Item("OrderName").ToString()
            spanOrderNote.InnerText = headerData.Tables(0).Rows(0).Item("OrderNote").ToString()
            lblOrderType.Text = headerData.Tables(0).Rows(0).Item("OrderType").ToString()
            spanStatusOrder.InnerText = headerData.Tables(0).Rows(0).Item("Status").ToString()
            spanStatusAdditional.InnerText = headerData.Tables(0).Rows(0).Item("StatusAdditional").ToString()
            lblStatusAdditional.Text = headerData.Tables(0).Rows(0).Item("StatusAdditional").ToString()
            spanCanceledNote.InnerText = headerData.Tables(0).Rows(0).Item("CanceledCategory").ToString()
            spanInternalNote.InnerText = headerData.Tables(0).Rows(0).Item("InternalNote").ToString()
            txtInternalNote.Text = headerData.Tables(0).Rows(0).Item("InternalNote").ToString()
            lblCreatedBy.Text = UCase(headerData.Tables(0).Rows(0).Item("CreatedBy").ToString())
            spanCreatedBy.InnerText = orderCfg.GetItemData("SELECT FullName FROM CustomerLogins WHERE Id = '" + UCase(lblCreatedBy.Text).ToString() + "'")
            spanCreatedDate.InnerText = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CreatedDate")).ToString("dd MMM yyyy")
            spanSubmittedDate.InnerText = "-"
            If Not headerData.Tables(0).Rows(0).Item("SubmittedDate").ToString() = "" Then
                spanSubmittedDate.InnerText = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("SubmittedDate")).ToString("dd MMM yyyy")
            End If
            spanCompletedDate.InnerText = "-"
            If Not headerData.Tables(0).Rows(0).Item("CompletedDate").ToString() = "" Then
                spanCompletedDate.InnerText = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CompletedDate")).ToString("dd MMM yyyy")
            End If

            If headerData.Tables(0).Rows(0).Item("OrderNote").ToString() = "" Then
                spanOrderNote.InnerText = "-"
            End If
            If headerData.Tables(0).Rows(0).Item("CanceledCategory").ToString() = "" Then
                spanCanceledNote.InnerText = "-"
            End If
            If headerData.Tables(0).Rows(0).Item("InternalNote").ToString() = "" Then
                spanInternalNote.InnerText = "-"
            End If
            spanCanceledDate.InnerText = "-"
            If Not headerData.Tables(0).Rows(0).Item("CanceledDate").ToString() = "" Then
                spanCanceledDate.InnerText = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CanceledDate")).ToString("dd MMM yyyy")
            End If
            spanJobId.InnerText = headerData.Tables(0).Rows(0).Item("JoNumberId").ToString()
            If headerData.Tables(0).Rows(0).Item("JoNumberId").ToString() = "" Then
                spanJobId.InnerText = "-"
            End If
            spanJobDate.InnerText = "-"
            If Not headerData.Tables(0).Rows(0).Item("JobDate").ToString() = "" Then
                spanJobDate.InnerText = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("JobDate")).ToString("dd MMM yyyy")
            End If

            lblShipmentId.Text = headerData.Tables(0).Rows(0).Item("ShipmentId").ToString()
            lblCustomerId.Text = headerData.Tables(0).Rows(0).Item("CustomerId").ToString()
            lblMnetId.Text = orderCfg.GetItemData("SELECT MicronetId FROM Customers WHERE Id = '" + lblCustomerId.Text + "'")
            lblDeposit.Text = Convert.ToInt32(headerData.Tables(0).Rows(0).Item("Deposit"))
            lblApproved.Text = headerData.Tables(0).Rows(0).Item("Approved").ToString()
            lblCashSale.Text = orderCfg.GetItemData_Boolean("SELECT CashSale FROM Customers WHERE Id = '" + lblCustomerId.Text + "'")

            spanShipmentNo.InnerText = "-"
            spanShipmentPort.InnerText = "-"
            spanShipmentCustomer.InnerText = "-"
            spanTotal.InnerText = "-"
            spanGST.InnerText = "-"
            spanFinalTotal.InnerText = "-"

            BindAdditionalStatus(spanStatusOrder.InnerText)
            ddlStatusAdditional.SelectedValue = headerData.Tables(0).Rows(0).Item("StatusAdditional").ToString()

            gvList.DataSource = orderCfg.GetListData("SELECT *, CASE WHEN Paid = 1 THEN 'Yes' WHEN Paid = 0 THEN 'No' ELSE 'Error' END AS PaidText FROM OrderDetails_Shutters WHERE HeaderId = '" + headerId + "' AND Active = 1 ORDER BY Number ASC")
            gvList.DataBind()

            BindDataQuote(headerId)
            BindDataShipment(lblShipmentId.Text)
            BindDataCosting(gvList.Rows.Count, headerId)

            Dim mailServerDeposit As String = orderCfg.GetItemData("SELECT Mailings.Server FROM Mailings LEFT JOIN CustomerLogins ON Mailings.ApplicationId = CustomerLogins.ApplicationId WHERE CustomerLogins.Id = '" + lblCreatedBy.Text + "' AND Mailings.Name = 'Deposit Request' AND Mailings.Active = 1")
            If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                mailServerDeposit = orderCfg.GetItemData("SELECT Mailings.Server FROM Mailings LEFT JOIN CustomerLogins ON Mailings.ApplicationId = CustomerLogins.ApplicationId WHERE CustomerLogins.Id = '" + lblCreatedBy.Text + "' AND Mailings.Name = 'Deposit Request Shutters' AND Mailings.Active = 1")
            End If

            Dim mailServerQuote As String = orderCfg.GetItemData("SELECT Mailings.Server FROM Mailings LEFT JOIN CustomerLogins ON Mailings.ApplicationId = CustomerLogins.ApplicationId WHERE CustomerLogins.Id = '" + lblCreatedBy.Text + "' AND Mailings.Name = 'Quote Order' AND Mailings.Active = 1")
            If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                mailServerQuote = orderCfg.GetItemData("SELECT Mailings.Server FROM Mailings LEFT JOIN CustomerLogins ON Mailings.ApplicationId = CustomerLogins.ApplicationId WHERE CustomerLogins.Id = '" + lblCreatedBy.Text + "' AND Mailings.Name = 'Quote Order Shutters' AND Mailings.Active = 1")
            End If

            Dim mailServerInvoice As String = orderCfg.GetItemData("SELECT Mailings.Server FROM Mailings LEFT JOIN CustomerLogins ON Mailings.ApplicationId = CustomerLogins.ApplicationId WHERE CustomerLogins.Id = '" + lblCreatedBy.Text + "' AND Mailings.Name = 'Send Invoice' AND Mailings.Active = 1")
            Dim mailCustomer As String = orderCfg.GetItemData("SELECT Email FROM CustomerContacts WHERE CustomerId = '" + lblCustomerId.Text + "' AND [Primary] = 1")

            Dim production As String = orderCfg.GetItemData("SELECT TOP 1 Production FROM OrderDetails_Shutters WHERE HeaderId = '" + headerId + "' AND Active = 1 ORDER BY Id ASC")

            txtDepositFrom.Text = mailServerDeposit
            txtDepositTo.Text = mailCustomer

            txtEmailQuoteFrom.Text = mailServerQuote
            txtEmailQuoteTo.Text = mailCustomer

            Dim otorisasiQuery As String = "SELECT OrderAuthorizations.*, CONVERT(VARCHAR, OrderHeaders_Shutters.OrderId) + ' (Item# ' + CONVERT(VARCHAR, OrderDetails_Shutters.Number) + ')' AS JobOtorisasi FROM OrderAuthorizations INNER JOIN OrderHeaders_Shutters ON OrderAuthorizations.HeaderId = OrderHeaders_Shutters.Id LEFT JOIN OrderDetails_Shutters ON OrderAuthorizations.ItemId = OrderDetails_Shutters.Id WHERE OrderAuthorizations.HeaderId='" + headerId + "' AND OrderAuthorizations.Active = 1 ORDER BY OrderAuthorizations.ItemId ASC"

            lvOtorisasi.DataSource = orderCfg.GetListData(otorisasiQuery)
            lvOtorisasi.DataBind()

            Dim totalAuthorization As Integer = orderCfg.GetItemData_Integer("SELECT COUNT(*) FROM OrderAuthorizations WHERE OrderAuthorizations.HeaderId='" + headerId + "' AND Active = 1 AND (Status IS NULL OR Status = '')")
            spanOtorisasi.InnerText = totalAuthorization

            Dim listOtorisasi As Integer = lvOtorisasi.Items.Count
            If spanStatusOrder.InnerText = "New Order" Then
                If listOtorisasi > 0 And totalAuthorization > 0 Then
                    If spanStatusAdditional.InnerText = "" Or spanStatusAdditional.InnerText = "Waiting for Deposit" Then
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET Approved=@Approved WHERE Id=@Id")
                                myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                                myCmd.Parameters.AddWithValue("@Approved", "2")

                                myCmd.Connection = thisConn
                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                                thisConn.Close()
                            End Using
                        End Using
                        ddlStatusAdditional.SelectedValue = "On Hold - Order Detail Query"
                        Using thisConn As New SqlConnection(myConn)
                            Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                                myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                                myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                                myCmd.Connection = thisConn
                                thisConn.Open()
                                myCmd.ExecuteNonQuery()
                                thisConn.Close()
                            End Using
                        End Using
                        Response.Redirect("~/order/detail", False)
                        Exit Sub
                    End If
                End If

                If lblDeposit.Text = 0 And spanStatusAdditional.InnerText = "" Then
                    ddlStatusAdditional.SelectedValue = "Waiting for Deposit"
                    Using thisConn As New SqlConnection(myConn)
                        Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderHeaders_Shutters SET StatusAdditional=@StatusAdditional WHERE Id=@Id")
                            myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                            myCmd.Parameters.AddWithValue("@StatusAdditional", ddlStatusAdditional.SelectedValue)

                            myCmd.Connection = thisConn
                            thisConn.Open()
                            myCmd.ExecuteNonQuery()
                            thisConn.Close()
                        End Using
                    End Using
                    Response.Redirect("~/order/detail", False)
                    Exit Sub
                End If
            End If

            Dim customerOnStop As Boolean = orderCfg.GetItemData_Boolean("SELECT OnStop FROM Customers WHERE Id='" + lblCustomerId.Text + "'")
            Dim customerMinimum As Boolean = orderCfg.GetItemData_Boolean("SELECT MinimumOrderSurcharge FROM Customers WHERE Id = '" + lblCustomerId.Text + "'")

            divRetailerName.Visible = False
            btnEditHeader.Visible = False
            aDeleteOrder.Visible = False
            btnApproval.Visible = False
            aGenerateJob.Visible = False : divGenerateJob.Visible = False : divMinimumSurcharge.Visible = False
            aSubmitOrder.Visible = False
            aCancelOrder.Visible = False
            aCompleteOrder.Visible = False
            aExact.Visible = False
            aSlip.Visible = False
            aUnsubmitOrder.Visible = False

            btnQuoteAction.Visible = False
            aQuoteDetail.Visible = False
            btnQuoteDownload.Visible = False

            btnMoreAction.Visible = False
            aStatusAdditional.Visible = False
            aInternalNote.Visible = False
            divDividerQuote.Visible = False
            btnQuotePrint.Visible = False
            aQuoteEmail.Visible = False
            divDividerDeposit.Visible = False
            aDeposit.Visible = False

            divInternalNote.Visible = False

            aAddItem.Visible = False
            btnService.Visible = False

            aOtorisasi.Visible = False

            aSubmitOrder.InnerText = "Submit Order"
            titleSubmit.InnerText = "Submit Order"

            If Session("RoleName") = "Administrator" Then
                divRetailerName.Visible = True
                divInternalNote.Visible = True
                If spanStatusOrder.InnerText = "Unsubmitted" Then
                    If customerOnStop = False Then
                        If gvList.Rows.Count > 0 Then aSubmitOrder.Visible = True
                        aAddItem.Visible = True
                    End If
                    btnEditHeader.Visible = True
                    aDeleteOrder.Visible = True
                    btnQuoteAction.Visible = True
                    aQuoteDetail.Visible = True
                    btnQuoteDownload.Visible = True

                    btnMoreAction.Visible = True
                    btnQuotePrint.Visible = True
                    aQuoteEmail.Visible = True
                    btnService.Visible = True
                End If

                If spanStatusOrder.InnerText = "New Order" Then
                    btnEditHeader.Visible = True
                    btnMoreAction.Visible = True
                    aCancelOrder.Visible = True
                    btnQuoteAction.Visible = True
                    aQuoteDetail.Visible = True
                    btnQuoteDownload.Visible = True
                    aInternalNote.Visible = True
                    divDividerQuote.Visible = True
                    btnQuotePrint.Visible = True
                    aQuoteEmail.Visible = True
                    aAddItem.Visible = True
                    btnService.Visible = True

                    If lvOtorisasi.Items.Count > 0 Then aOtorisasi.Visible = True
                    If lblApproved.Text = "1" Then aStatusAdditional.Visible = True
                    If lblApproved.Text = "2" Or lblApproved.Text = "0" Then
                        btnApproval.Visible = True
                    End If

                    If spanStatusAdditional.InnerText = "" Then
                        aGenerateJob.Visible = True
                        If lblCashSale.Text = True Then
                            divDividerDeposit.Visible = True
                            aDeposit.Visible = True
                        End If
                    End If
                    If spanStatusAdditional.InnerText = "Waiting for Deposit" Then
                        divDividerDeposit.Visible = True
                        aDeposit.Visible = True
                    End If
                    If lblCashSale.Text = True Then divGenerateJob.Visible = True
                    If customerMinimum = True Then
                        Dim checkService As Integer = orderCfg.GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails_Shutters WHERE HeaderId = '" + headerId + "' AND ProductId = '901C1FF3-F3B5-4ACE-A73E-79027ECAEFFF' AND Active = 1")
                        Dim sumPrice As Decimal = orderCfg.GetItemData_Decimal("SELECT SUM(FinalCost) AS SumPrice FROM OrderDetails_Shutters WHERE HeaderId = '" + headerId + "' AND Active=1")
                        If checkService = 0 And sumPrice < 200 Then divMinimumSurcharge.Visible = True
                    End If

                    If spanStatusAdditional.InnerText = "On Hold - Customer Request" Then
                        aAddItem.Visible = True
                        aSubmitOrder.Visible = True
                        aSubmitOrder.InnerText = "Re-Submit Order"
                        titleSubmit.InnerText = "Re-Submit Order"
                    End If
                End If

                If spanStatusOrder.InnerText = "In Production" Then
                    btnEditHeader.Visible = True
                    aCompleteOrder.Visible = True
                    aCancelOrder.Visible = True
                    aUnsubmitOrder.Visible = True
                    btnService.Visible = True

                    btnMoreAction.Visible = True
                    aStatusAdditional.Visible = True
                    btnQuoteAction.Visible = True
                    aQuoteDetail.Visible = True
                    btnQuoteDownload.Visible = True
                    aInternalNote.Visible = True

                    If Session("LevelName") = "Leader" Or Session("LevelName") = "Super Admin" Then
                        aExact.Visible = True
                    End If

                    If lblOrderType.Text = "RBR" Or (lblOrderType.Text = "Blinds" And production = "JKT") Then
                        aCancelOrder.Visible = False
                        aUnsubmitOrder.Visible = False
                    End If
                End If

                If spanStatusOrder.InnerText = "Completed" Then
                    btnEditHeader.Visible = True
                    btnMoreAction.Visible = True
                    aStatusAdditional.Visible = True
                    aInternalNote.Visible = True
                    aSlip.Visible = True
                    btnEditHeader.Visible = True
                    If Session("LevelName") = "Leader" Or Session("LevelName") = "Super Admin" Then
                        aExact.Visible = True
                    End If
                End If
            End If

            If Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Then
                divRetailerName.Visible = True
                divInternalNote.Visible = True
                If spanStatusOrder.InnerText = "Unsubmitted" Then
                    btnEditHeader.Visible = True
                    aSubmitOrder.Visible = True
                    If lblOrderType.Text = "" Or lblOrderType.Text = "RBR" Or lblOrderType.Text = "Blinds" Or lblOrderType.Text = "Curtain" Or lblOrderType.Text = "Veri Shades" Or lblOrderType.Text = "Zebra Blinds" Then
                        aSubmitOrder.Visible = False
                    End If
                    If lblCreatedBy.Text = Session("LoginId") Then
                        aDeleteOrder.Visible = True
                        If customerOnStop = False Then
                            aAddItem.Visible = True
                        End If
                    End If
                    btnMoreAction.Visible = True
                    btnQuotePrint.Visible = True
                    aQuoteEmail.Visible = True
                    If gvList.Rows.Count > 0 Then
                        btnService.Visible = True
                    End If
                End If

                If spanStatusOrder.InnerText = "New Order" Then
                    btnEditHeader.Visible = True
                    aCancelOrder.Visible = True
                    btnMoreAction.Visible = True
                    aStatusAdditional.Visible = True
                    aInternalNote.Visible = True
                    divDividerQuote.Visible = True
                    btnQuotePrint.Visible = True
                    aQuoteEmail.Visible = True
                    aAddItem.Visible = True
                    btnService.Visible = True

                    If lvOtorisasi.Items.Count > 0 Then aOtorisasi.Visible = True
                    If lblApproved.Text = "1" Then aStatusAdditional.Visible = True
                    If lblApproved.Text = "2" Or lblApproved.Text = "0" Then
                        btnApproval.Visible = True
                    End If

                    If spanStatusAdditional.InnerText = "" Then
                        aGenerateJob.Visible = True
                        If lblCashSale.Text = True Then
                            divDividerDeposit.Visible = True
                            aDeposit.Visible = True
                        End If
                    End If
                    If spanStatusAdditional.InnerText = "Waiting for Deposit" Then
                        divDividerDeposit.Visible = True
                        aDeposit.Visible = True
                    End If
                    If lblCashSale.Text = True Then divGenerateJob.Visible = True
                    If customerMinimum = True Then
                        Dim checkService As Integer = orderCfg.GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails_Shutters WHERE HeaderId = '" + headerId + "' AND ProductId = '901C1FF3-F3B5-4ACE-A73E-79027ECAEFFF' AND Active = 1")
                        Dim sumPrice As Decimal = orderCfg.GetItemData_Decimal("SELECT SUM(FinalCost) AS SumPrice FROM OrderDetails_Shutters WHERE HeaderId = '" + headerId + "' AND Active=1")
                        If checkService = 0 And sumPrice < 200 Then divMinimumSurcharge.Visible = True
                    End If
                    If spanStatusAdditional.InnerText = "On Hold - Customer Request" Then
                        aAddItem.Visible = True
                        aSubmitOrder.Visible = True
                        aSubmitOrder.InnerText = "Re-Submit Order"
                        titleSubmit.InnerText = "Re-Submit Order"
                    End If
                End If

                If spanStatusOrder.InnerText = "In Production" Then
                    aCompleteOrder.Visible = True
                    aCancelOrder.Visible = True
                    aUnsubmitOrder.Visible = True
                    btnMoreAction.Visible = True
                    aStatusAdditional.Visible = True
                    btnService.Visible = True
                    aInternalNote.Visible = True
                    aSlip.Visible = True
                    aExact.Visible = True

                    If lblOrderType.Text = "RBR" Or (lblOrderType.Text = "Blinds" And production = "JKT") Then
                        aCancelOrder.Visible = False
                        aUnsubmitOrder.Visible = False
                    End If
                End If

                If spanStatusOrder.InnerText = "Completed" Then
                    aSlip.Visible = True
                    aExact.Visible = True
                End If
            End If

            If Session("RoleName") = "Account" Then
                divInternalNote.Visible = True
                If spanStatusOrder.InnerText = "In Production" Then
                    aCompleteOrder.Visible = True
                    btnMoreAction.Visible = True
                    aStatusAdditional.Visible = True
                    aInternalNote.Visible = True
                    aSlip.Visible = True
                End If

                If spanStatusOrder.InnerText = "Completed" Then
                    aCompleteOrder.Visible = True
                    btnMoreAction.Visible = True
                    aStatusAdditional.Visible = True
                    aInternalNote.Visible = True
                    aSlip.Visible = True
                End If
            End If

            If Session("RoleName") = "Representative" Then
                If spanStatusOrder.InnerText = "Unsubmitted" Then
                    If customerOnStop = False Then
                        btnEditHeader.Visible = True
                        aSubmitOrder.Visible = True

                        If lblOrderType.Text = "" Or lblOrderType.Text = "RBR" Or lblOrderType.Text = "Blinds" Or lblOrderType.Text = "Curtain" Or lblOrderType.Text = "Veri Shades" Or lblOrderType.Text = "Zebra Blinds" Then
                            aSubmitOrder.Visible = False
                        End If

                        btnQuoteAction.Visible = True
                        aQuoteDetail.Visible = True
                        btnQuoteDownload.Visible = True
                        aAddItem.Visible = True
                        aDeleteOrder.Visible = True
                    End If
                End If

                If spanStatusOrder.InnerText = "New Order" Then
                    btnEditHeader.Visible = True
                    btnQuoteAction.Visible = True
                    aQuoteDetail.Visible = True
                    btnQuoteDownload.Visible = True
                    If spanStatusAdditional.InnerText = "On Hold - Customer Request" Then
                        aAddItem.Visible = True
                    End If

                    If spanStatusAdditional.InnerText = "On Hold - Customer Request" Then
                        aAddItem.Visible = True
                        aSubmitOrder.Visible = True
                        aSubmitOrder.InnerText = "Re-Submit Order"
                        titleSubmit.InnerText = "Re-Submit Order"
                    End If
                End If
                If spanStatusOrder.InnerText = "In Production" Or spanStatusOrder.InnerText = "Completed" Then
                    btnQuoteAction.Visible = True
                    aQuoteDetail.Visible = True
                    btnQuoteDownload.Visible = True
                End If
            End If

            If Session("RoleName") = "Customer" Then
                If spanStatusOrder.InnerText = "Unsubmitted" Then
                    If customerOnStop = False Then
                        btnEditHeader.Visible = True
                        aAddItem.Visible = True
                        aDeleteOrder.Visible = True

                        If lblOrderType.Text = "Panorama" Or lblOrderType.Text = "Evolve" Then
                            btnQuoteAction.Visible = True
                            aQuoteDetail.Visible = True
                            btnQuoteDownload.Visible = True
                        End If

                        If gvList.Rows.Count > 0 Then
                            aSubmitOrder.Visible = True

                            If lblOrderType.Text = "" Or lblOrderType.Text = "RBR" Or lblOrderType.Text = "Blinds" Or lblOrderType.Text = "Curtain" Or lblOrderType.Text = "Veri Shades" Or lblOrderType.Text = "Zebra Blinds" Then
                                aSubmitOrder.Visible = False
                            End If
                        End If
                    End If
                End If

                If spanStatusOrder.InnerText = "New Order" Then
                    btnEditHeader.Visible = True
                    btnQuoteAction.Visible = True
                    aQuoteDetail.Visible = True
                    btnQuoteDownload.Visible = True
                    If spanStatusAdditional.InnerText = "On Hold - Customer Request" Then
                        aAddItem.Visible = True
                        aSubmitOrder.Visible = True
                        aSubmitOrder.InnerText = "Re-Submit Order"
                        titleSubmit.InnerText = "Re-Submit Order"
                    End If
                End If

                If spanStatusOrder.InnerText = "In Production" Or spanStatusOrder.InnerText = "Completed" Then
                    btnQuoteAction.Visible = True
                    aQuoteDetail.Visible = True
                    btnQuoteDownload.Visible = True
                End If
            End If

            gvList.Columns(1).Visible = False ' ID
            gvList.Columns(2).Visible = False ' NUMBER
            gvList.Columns(3).Visible = False ' QTY
            gvList.Columns(4).Visible = False ' DESCRIPTION
            gvList.Columns(5).Visible = False ' COST
            gvList.Columns(6).Visible = False ' MARK UP
            gvList.Columns(7).Visible = False ' FACTORY
            gvList.Columns(8).Visible = False ' ACTION

            If Session("RoleName") = "Administrator" Then
                If Session("LevelName") = "Leader" Or Session("LevelName") = "Super Admin" Then
                    gvList.Columns(1).Visible = True ' ID
                End If
                gvList.Columns(2).Visible = True ' NUMBER
                gvList.Columns(3).Visible = True ' QTY
                gvList.Columns(4).Visible = True ' DESCRIPTION
                gvList.Columns(5).Visible = True ' COST
                gvList.Columns(6).Visible = True ' MARK UP
                gvList.Columns(8).Visible = True ' ACTION
            End If

            If Session("RoleName") = "Customer Service" Then
                gvList.Columns(2).Visible = True ' NUMBER
                gvList.Columns(3).Visible = True ' QTY
                gvList.Columns(4).Visible = True ' DESCRIPTION
                gvList.Columns(5).Visible = True ' COST
                gvList.Columns(8).Visible = True ' ACTION
            End If

            If Session("RoleName") = "Account" Then
                gvList.Columns(2).Visible = True ' NUMBER
                gvList.Columns(3).Visible = True ' QTY
                gvList.Columns(4).Visible = True ' DESCRIPTION
                gvList.Columns(5).Visible = True ' COST
                gvList.Columns(8).Visible = True ' PAID
            End If

            If Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Then
                gvList.Columns(2).Visible = True ' NUMBER
                gvList.Columns(3).Visible = True ' QTY
                gvList.Columns(4).Visible = True ' DESCRIPTION
                gvList.Columns(5).Visible = True ' COST
                gvList.Columns(8).Visible = True ' ACTION
            End If

            If Session("RoleName") = "Customer" Then
                gvList.Columns(2).Visible = True ' NUMBER
                gvList.Columns(3).Visible = True ' QTY
                gvList.Columns(4).Visible = True ' DESCRIPTION
                gvList.Columns(5).Visible = True ' COST
                gvList.Columns(6).Visible = True ' MARK UP
                gvList.Columns(8).Visible = True ' ACTION
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                    If lblOrderType.Text = "" Or lblOrderType.Text = "Evolve" Or lblOrderType.Text = "Panorama" Then
                        MessageError(True, "Please contact Customer Service at customerservice@sunlight.com.au")
                    End If
                End If
                mailCfg.MailError(Page.Title, "BindDataOrder", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataQuote(HeaderId As String)
        Dim quoteData As DataSet = orderCfg.GetListData("SELECT * FROM OrderQuotes WHERE Id = '" + HeaderId + "'")
        If quoteData.Tables(0).Rows.Count = 0 Then
            Using thisConn As New SqlConnection(myConn)
                Using myCmd As SqlCommand = New SqlCommand("INSERT INTO OrderQuotes VALUES (@Id, '', '', '', '', '', '', 0.00, 0.00, 0.00, 0.00)")
                    myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)

                    myCmd.Connection = thisConn
                    thisConn.Open()
                    myCmd.ExecuteNonQuery()
                    thisConn.Close()
                End Using
            End Using
            Exit Sub
        End If

        txtQuoteEmail.Text = quoteData.Tables(0).Rows(0).Item("Email").ToString()
        txtQuotePhone.Text = quoteData.Tables(0).Rows(0).Item("Phone").ToString()
        txtQuoteAddress.Text = quoteData.Tables(0).Rows(0).Item("Address").ToString()
        txtQuoteSuburb.Text = quoteData.Tables(0).Rows(0).Item("Suburb").ToString()
        ddlQuoteStates.SelectedValue = quoteData.Tables(0).Rows(0).Item("States").ToString()
        txtQuotePostCode.Text = quoteData.Tables(0).Rows(0).Item("PostCode").ToString()

        txtQuoteDiscount.Text = quoteData.Tables(0).Rows(0).Item("Discount").ToString().Replace(",", ".")
        txtQuoteInstallation.Text = quoteData.Tables(0).Rows(0).Item("Installation").ToString().Replace(",", ".")
        txtQuoteCheckMeasure.Text = quoteData.Tables(0).Rows(0).Item("CheckMeasure").ToString().Replace(",", ".")
        txtQuoteFreight.Text = quoteData.Tables(0).Rows(0).Item("Freight").ToString().Replace(",", ".")
    End Sub

    Private Sub BindDataShipment(ShipmentId As String)
        Dim shipmentData As DataSet = orderCfg.GetListData("SELECT * FROM OrderShipments WHERE Id = '" + ShipmentId + "'")
        If shipmentData.Tables(0).Rows.Count = 1 Then
            If Not shipmentData.Tables(0).Rows(0).Item("ShipmentNumber").ToString() = "" Then
                spanShipmentNo.InnerText = shipmentData.Tables(0).Rows(0).Item("ShipmentNumber").ToString()
            End If
            If Not shipmentData.Tables(0).Rows(0).Item("ETAPort").ToString() = "" Then
                spanShipmentPort.InnerText = Convert.ToDateTime(shipmentData.Tables(0).Rows(0).Item("ETAPort")).ToString("dd MMM yyyy")
            End If
            If Not shipmentData.Tables(0).Rows(0).Item("ETACustomer").ToString() = "" Then
                spanShipmentCustomer.InnerText = Convert.ToDateTime(shipmentData.Tables(0).Rows(0).Item("ETACustomer")).ToString("dd MMM yyyy")
            End If
        End If
    End Sub

    Private Sub BindDataCosting(Data As Integer, HeaderId As String)
        If Data > 0 Then
            Dim sumPrice As Decimal = orderCfg.GetItemData_Decimal("SELECT SUM(FinalCost) AS SumPrice FROM OrderDetails_Shutters WHERE HeaderId = '" + HeaderId + "' AND Active=1")
            Dim gst As Decimal = sumPrice * 10 / 100
            Dim finaltotal As Decimal = sumPrice + gst

            spanTotal.InnerText = "$ " & sumPrice.ToString("N2", enUS)
            spanGST.InnerText = "$ " & gst.ToString("N2", enUS)
            spanFinalTotal.InnerText = "$ " & finaltotal.ToString("N2", enUS)

            If spanStatusOrder.InnerText = "New Order" Then
                Dim orderCost As Decimal = sumPrice
                Dim orderGst As Decimal = sumPrice * 10 / 100

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE OrderInvoices SET OrderCost=@OrderCost, GST=@GST WHERE Id=@Id")
                        myCmd.Parameters.AddWithValue("@Id", lblHeaderId.Text)
                        myCmd.Parameters.AddWithValue("@OrderCost", orderCost)
                        myCmd.Parameters.AddWithValue("@GST", orderGst)

                        myCmd.Connection = thisConn
                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                        thisConn.Close()
                    End Using
                End Using
            End If
        End If
    End Sub

    Protected Function BindProductDescription(ItemId As String) As String
        Dim result As String = String.Empty

        Dim thisData As DataSet = orderCfg.GetListData("SELECT * FROM OrderDetails_Shutters WHERE Id='" + ItemId + "'")

        Dim room As String = thisData.Tables(0).Rows(0).Item("Room").ToString()
        Dim productId As String = thisData.Tables(0).Rows(0).Item("ProductId").ToString()
        Dim designId As String = orderCfg.GetItemData("SELECT DesignId FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")
        Dim blindId As String = orderCfg.GetItemData("SELECT BlindId FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")

        Dim designName As String = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id='" + UCase(designId).ToString() + "'")
        Dim blindName As String = orderCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + UCase(blindId).ToString() + "'")
        Dim productName As String = orderCfg.GetItemData("SELECT Name FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")

        Dim itemDescription As String = "<b>" & room & "</b>, " & productName
        If room = "" Then itemDescription = productName

        Dim width As String = thisData.Tables(0).Rows(0).Item("Width").ToString()
        Dim widthB As String = thisData.Tables(0).Rows(0).Item("WidthB").ToString()
        Dim widthC As String = thisData.Tables(0).Rows(0).Item("WidthC").ToString()
        Dim widthD As String = thisData.Tables(0).Rows(0).Item("WidthD").ToString()
        Dim widthE As String = thisData.Tables(0).Rows(0).Item("WidthE").ToString()
        Dim widthF As String = thisData.Tables(0).Rows(0).Item("WidthF").ToString()

        Dim drop As String = thisData.Tables(0).Rows(0).Item("Drop").ToString()
        Dim dropB As String = thisData.Tables(0).Rows(0).Item("DropB").ToString()
        Dim dropC As String = thisData.Tables(0).Rows(0).Item("DropC").ToString()
        Dim dropD As String = thisData.Tables(0).Rows(0).Item("DropD").ToString()
        Dim dropE As String = thisData.Tables(0).Rows(0).Item("DropF").ToString()
        Dim dropF As String = thisData.Tables(0).Rows(0).Item("DropG").ToString()

        Dim size As String = "(" & width & " x " & drop & ")"
        Dim sizeB As String = "(" & widthB & " x " & dropB & ")"
        Dim sizeC As String = "(" & widthC & " x " & dropC & ")"
        Dim sizeD As String = "(" & widthD & " x " & dropD & ")"
        Dim sizeE As String = "(" & widthE & " x " & dropE & ")"
        Dim sizeF As String = "(" & widthF & " x " & dropF & ")"

        Dim fabricId As String = thisData.Tables(0).Rows(0).Item("FabricId").ToString()
        Dim fabricIdB As String = thisData.Tables(0).Rows(0).Item("FabricIdB").ToString()
        Dim fabricIdC As String = thisData.Tables(0).Rows(0).Item("FabricIdC").ToString()
        Dim fabricIdD As String = thisData.Tables(0).Rows(0).Item("FabricIdD").ToString()
        Dim fabricIdE As String = thisData.Tables(0).Rows(0).Item("FabricIdE").ToString()
        Dim fabricIdF As String = thisData.Tables(0).Rows(0).Item("FabricIdF").ToString()

        Dim fabricColourId As String = thisData.Tables(0).Rows(0).Item("FabricColourId").ToString()

        Dim fabricName As String = orderCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricId + "'")
        Dim fabricNameB As String = orderCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricIdB + "'")
        Dim fabricNameC As String = orderCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricIdC + "'")
        Dim fabricNameD As String = orderCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricIdD + "'")
        Dim fabricNameE As String = orderCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricIdE + "'")
        Dim fabricNameF As String = orderCfg.GetItemData("SELECT Name FROM Fabrics WHERE Id = '" + fabricIdF + "'")

        Dim fabricColourName As String = orderCfg.GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourId + "'")

        Dim partCategory As String = thisData.Tables(0).Rows(0).Item("PartCategory").ToString()
        Dim partComponent As String = thisData.Tables(0).Rows(0).Item("PartComponent").ToString()
        Dim partColour As String = thisData.Tables(0).Rows(0).Item("PartColour").ToString()
        Dim partLength As String = thisData.Tables(0).Rows(0).Item("PartLength").ToString()

        Dim controlPosition As String = thisData.Tables(0).Rows(0).Item("ControlPosition").ToString()

        If designName = "Additional" Then
            Dim addName As String = thisData.Tables(0).Rows(0).Item("AddName").ToString()
            Dim addNumber As String = thisData.Tables(0).Rows(0).Item("AddNumber").ToString()
            result = blindName & " | " & productName
            If blindName = "Check Measure" And productName = "Template Fee" Then
                result = blindName & " | " & productName & " - " & "Item #" & addNumber
            End If
            If blindName = "Installation" And (productName = "Blind Products" Or productName = "Concrete/Brick" Or productName = "Program Hub") Then
                result = blindName & " | " & productName & " - " & "Item #" & addNumber
            End If
            If blindName = "Takedown" Then
                result = blindName & " - " & productName & " " & "Item #" & addNumber
            End If
            If blindName = "Parts" Then
                result = blindName & " - " & addName
            End If
            If blindName = "Minimum Order Surcharge" Then
                result = blindName
            End If
        End If

        If designName = "Aluminium Blind" Then
            result = itemDescription & " " & size
        End If

        If designName = "Cellular Shades" Then
            result = itemDescription & " #" & fabricColourName & " " & size
        End If

        If designName = "Curtain" Then
            result = itemDescription & " #" & fabricName & " " & size
        End If

        If designName = "LS Venetian" Then
            result = itemDescription & " " & size
        End If

        If designName = "Panel Glide" Then
            Dim tubeType As String = orderCfg.GetItemData("SELECT TubeType FROM Products WHERE Id = '" + productId + "'")

            result = itemDescription & " #" & fabricName & " - " & size & "mm"
            If tubeType = "Track Only" Then
                result = itemDescription & " - " & width & "mm"
            End If
        End If



        If designName = "Evolve Parts" Then
            result = String.Format("{0}", itemDescription)

            If partCategory = "Louvres" Then
                result = String.Format("{0} - {1} {2}", itemDescription, partComponent, partColour)
                If partComponent = "63mm Louvre" Or partComponent = "89mm Louvre" Then
                    result = String.Format("{0} - {1} {2} ({3}mm)", itemDescription, partComponent, partColour, partLength)
                End If
            End If

            If partCategory = "Framing | Hinged" OrElse partCategory = "Framing | Fixed" OrElse partCategory = "Extrusion" Then
                result = String.Format("{0} - {1} {2} ({3}mm)", itemDescription, partComponent, partColour, partLength)
            End If

            If partCategory = "Framing | Bi-fold or Sliding" Then
                result = String.Format("{0} - {1}", itemDescription, partComponent)
                If Not (partComponent = "Fascia H Clip" OrElse partComponent = "Fascia Return Connector") Then
                    result = String.Format("{0} - {1} {2} ({3}mm)", itemDescription, partComponent, partColour, partLength)
                End If
            End If

            If partCategory = "Posts" Then
                result = String.Format("{0} - {1} {2} ({3}mm)", itemDescription, partComponent, partColour, partLength)
                If Not (partComponent = "T Post" OrElse partComponent = "90° Corner Post" OrElse partComponent = "135° Bay Post") Then
                    result = String.Format("{0} - {1} {2}", itemDescription, partComponent, partColour)
                End If
            End If

            If partCategory = "Hinges" Then
                result = String.Format("{0} - {1} {2}", itemDescription, partComponent, partColour)
                If partComponent = "Hinge Spacer" Then
                    result = String.Format("{0} - {1}", itemDescription, partComponent)
                End If
            End If

            If partCategory = "Catches" OrElse partCategory = "Misc" Then
                result = String.Format("{0} - {1}", itemDescription, partComponent)
            End If

            If partCategory = "Track Hardware" Then
                result = String.Format("{0} - {1} {2} ({3}mm)", itemDescription, partComponent, partColour, partLength)
                If Not (partComponent = "Top Track" OrElse partComponent = "Bottom M Track") Then
                    result = String.Format("{0} - {1}", itemDescription, partComponent)
                End If
            End If

        End If

        If designName = "Panorama PVC Parts" Then
            result = itemDescription & " | " & partComponent
            If partCategory = "Louvres" Then
                result = itemDescription & " | " & partComponent & " - " & partColour & " (" & partLength & "mm)"
                If partComponent = "Louvre Repair Pin" Or partComponent = "Standard Louvre Pin" Then
                    result = itemDescription & " | " & partComponent & " - " & partColour
                End If
            End If

            If partCategory = "Framing | Hinged" Or partCategory = "Framing | Bi-fold or Sliding" Or partCategory = "Framing | Fixed" Or partCategory = "Extrusion" Then
                result = itemDescription & " | " & partComponent & " - " & partColour & " (" & partLength & "mm)"
            End If

            If partCategory = "Post" Then
                result = itemDescription & " | " & partComponent
                If partComponent = "T-Post" Or partComponent = "90° Corner Post" Or partComponent = "135° Bay Post" Then
                    result = itemDescription & " | " & partComponent & " - " & partColour & " (" & partLength & "mm)"
                End If
            End If

            If partCategory = "Hinges" Then
                result = itemDescription & " | " & partComponent
                If partComponent = "76mm Non-Mortise Hinge" Or partComponent = "76mm Rabbet Hinge" Then
                    result = itemDescription & " | " & partComponent & " - " & partColour
                End If
            End If

            If partCategory = "Magnets and Strikers" Then
                result = itemDescription & " | " & partComponent & " - " & partColour
                If partComponent = "Magnet" Then
                    result = itemDescription & " | " & partComponent
                End If
            End If

            If partCategory = "Track Hardware" Then
                result = itemDescription & " | " & partComponent
                If partComponent = "Top Track" Or partComponent = "Bottom M Track" Or partComponent = "Bottom U Track" Then
                    result = itemDescription & " | " & partComponent & " (" & partLength & "mm)"
                End If
            End If

            If partCategory = "Mist" Then
                result = itemDescription & " | " & partComponent & " - " & partColour
            End If
        End If

        If designName = "Panorama PVC Shutters" Then
            result = itemDescription & " - " & width & " x " & drop
        End If

        If designName = "Evolve Shutters" Then
            result = itemDescription & " - " & width & " x " & drop
        End If

        If designName = "Pelmet" Then
            result = itemDescription & " #" & fabricName & " - " & width & "mm"
            If fabricName = "" Then
                result = itemDescription & " - " & width & "mm"
            End If
        End If

        If designName = "Roller Blind" Then
            If blindName = "Regular" Then
                result = itemDescription & " #" & fabricName & " " & size
            End If

            If blindName = "Double Bracket" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "<u>First Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "<u>Second Blind</u> #" & fabricNameB & " " & sizeB

                result = itemDescription
            End If

            If blindName = "Link 2 Blinds Dep" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "<u>Control Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "<u>End Blind</u> #" & fabricNameB & " " & sizeB

                result = itemDescription
            End If

            If blindName = "Link 3 Blinds Dep" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "<u>Control Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "<u>Middle Blind</u> #" & fabricNameB & " " & sizeB
                itemDescription += "<br />"
                itemDescription += "<u>End Blind</u> #" & fabricNameC & " " & sizeC

                result = itemDescription
            End If

            If blindName = "Link 2 Blinds Ind" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "<u>First Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "<u>Second Blind</u> #" & fabricNameB & " " & sizeB

                result = itemDescription
            End If

            If blindName = "Link 3 Blinds Ind with Dep" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "<u>Control Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "<u>Middle Blind</u> #" & fabricNameB & " " & sizeB
                itemDescription += "<br />"
                itemDescription += "<u>End Blind</u> #" & fabricNameC & " " & sizeC

                result = itemDescription
            End If

            If blindName = "Link 4 Blinds Ind with Dep" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "<u>Control Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "<u>Middle Blind</u> #" & fabricNameB & " " & sizeB
                itemDescription += "<br />"
                itemDescription += "<u>Middle Blind</u> #" & fabricNameC & " " & sizeC
                itemDescription += "<br />"
                itemDescription += "<u>End Blind</u> #" & fabricNameD & " " & sizeD

                result = itemDescription
            End If

            If blindName = "Link 2 Blinds Head to Tail" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "<u>First Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "<u>Second Blind</u> #" & fabricNameB & " " & sizeB

                result = itemDescription
            End If

            If blindName = "Link 3 Blinds Head to Tail with Ind" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "<u>First Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "<u>Second Blind</u> #" & fabricNameB & " " & sizeB
                itemDescription += "<br />"
                itemDescription += "<u>Third Blind</u> #" & fabricNameC & " " & sizeC

                result = itemDescription
            End If

            If blindName = "DB Link 2 Blinds Dep" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "(1) <u>Control Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "(1) <u>End Blind</u> #" & fabricNameB & " " & sizeB
                itemDescription += "<br />"
                itemDescription += "(2) <u>Control Blind</u> #" & fabricNameC & " " & sizeC
                itemDescription += "<br />"
                itemDescription += "(2) <u>End Blind</u> #" & fabricNameD & " " & sizeD

                result = itemDescription
            End If

            If blindName = "DB Link 3 Blinds Dep" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "(1) <u>Control Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "(1) <u>Middle Blind</u> #" & fabricNameB & " " & sizeB
                itemDescription += "<br />"
                itemDescription += "(1) <u>End Blind</u> #" & fabricNameC & " " & sizeC
                itemDescription += "<br />"
                itemDescription += "(2) <u>Control Blind</u> #" & fabricNameD & " " & sizeD
                itemDescription += "<br />"
                itemDescription += "(2) <u>Middle Blind</u> #" & fabricNameE & " " & sizeE
                itemDescription += "<br />"
                itemDescription += "(2) <u>End Blind</u> #" & fabricNameF & " " & sizeF

                result = itemDescription
            End If

            If blindName = "DB Link 2 Blinds Ind" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "(1) <u>First Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "(1) <u>Second Blind</u> #" & fabricNameB & " " & sizeB
                itemDescription += "<br />"
                itemDescription += "(2) <u>Third Blind</u> #" & fabricNameC & " " & sizeC
                itemDescription += "<br />"
                itemDescription += "(2) <u>Fourth Blind</u> #" & fabricNameD & " " & sizeD

                result = itemDescription
            End If

            If blindName = "DB Link 3 Blinds Ind with Dep" Then
                itemDescription = "<b>" & itemDescription & "</b>"
                itemDescription += "<br />"
                itemDescription += "(1) <u>First Blind</u> #" & fabricName & " " & size
                itemDescription += "<br />"
                itemDescription += "(1) <u>Second Blind</u> #" & fabricNameB & " " & sizeB
                itemDescription += "<br />"
                itemDescription += "(1) <u>Third Blind</u> #" & fabricNameC & " " & sizeC
                itemDescription += "<br />"
                itemDescription += "(2) <u>Fourth Blind</u> #" & fabricNameD & " " & sizeD
                itemDescription += "<br />"
                itemDescription += "(2) <u>Fifth Blind</u> #" & fabricNameE & " " & sizeE
                itemDescription += "<br />"
                itemDescription += "(2) <u>Sixth Blind</u> #" & fabricNameF & " " & sizeF

                result = itemDescription
            End If
        End If

        If designName = "Roman Blind" Then
            result = itemDescription & " #" & fabricName & " " & size
        End If

        If designName = "Skin Only" Then
            result = itemDescription & " #" & fabricName & " " & size
        End If

        If designName = "Venetian Blind" Then
            result = itemDescription & " " & size
        End If

        If designName = "Veri Shades" Then
            result = itemDescription & " #" & fabricName & " " & size
        End If

        If designName = "Vertical" Then
            result = itemDescription & " #" & fabricName & "  " & size
            If blindName = "Track Only" Then
                result = itemDescription & "  " & size
            End If
            If blindName = "Blades Only" Then
                result = itemDescription & " #" & fabricName & " (" & drop & ")"
            End If
        End If

        If designName = "Zebra Blind" Then
            result = itemDescription & " #" & fabricName & " " & size
        End If
        Return result
    End Function

    Protected Function BindProductSlip(ItemId As String, blinds As Integer) As String
        Dim result As String = String.Empty

        Dim thisData As DataSet = orderCfg.GetListData("SELECT * FROM OrderDetails_Shutters WHERE Id='" + ItemId + "'")

        Dim room As String = thisData.Tables(0).Rows(0).Item("Room").ToString()
        Dim productId As String = thisData.Tables(0).Rows(0).Item("ProductId").ToString()
        Dim designId As String = orderCfg.GetItemData("SELECT DesignId FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")
        Dim blindId As String = orderCfg.GetItemData("SELECT BlindId FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")

        Dim designName As String = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id='" + UCase(designId).ToString() + "'")
        Dim blindName As String = orderCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + UCase(blindId).ToString() + "'")
        Dim productName As String = orderCfg.GetItemData("SELECT Name FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")
        Dim tubeName As String = orderCfg.GetItemData("SELECT TubeType FROM Products WHERE Id = '" + UCase(productId).ToString() + "'")

        Dim itemDescription As String = productName

        Dim width As String = thisData.Tables(0).Rows(0).Item("Width").ToString()
        Dim widthB As String = thisData.Tables(0).Rows(0).Item("WidthB").ToString()
        Dim widthC As String = thisData.Tables(0).Rows(0).Item("WidthC").ToString()
        Dim widthD As String = thisData.Tables(0).Rows(0).Item("WidthD").ToString()
        Dim widthE As String = thisData.Tables(0).Rows(0).Item("Width").ToString()
        Dim widthF As String = thisData.Tables(0).Rows(0).Item("Width").ToString()

        Dim drop As String = thisData.Tables(0).Rows(0).Item("Drop").ToString()
        Dim dropB As String = thisData.Tables(0).Rows(0).Item("DropB").ToString()
        Dim dropC As String = thisData.Tables(0).Rows(0).Item("DropC").ToString()
        Dim dropD As String = thisData.Tables(0).Rows(0).Item("DropD").ToString()
        Dim dropE As String = thisData.Tables(0).Rows(0).Item("Drop").ToString()
        Dim dropF As String = thisData.Tables(0).Rows(0).Item("Drop").ToString()

        Dim size As String = "(" & width & "x" & drop & ")"
        Dim sizeB As String = "(" & widthB & "x" & dropB & ")"
        Dim sizeC As String = "(" & widthC & "x" & dropC & ")"
        Dim sizeD As String = "(" & widthD & "x" & dropD & ")"
        Dim sizeE As String = "(" & widthE & "x" & dropE & ")"
        Dim sizeF As String = "(" & widthF & "x" & dropF & ")"

        Dim fabricColourId As String = thisData.Tables(0).Rows(0).Item("FabricColourId").ToString()
        Dim fabricColourIdB As String = thisData.Tables(0).Rows(0).Item("FabricColourIdB").ToString()
        Dim fabricColourIdC As String = thisData.Tables(0).Rows(0).Item("FabricColourIdC").ToString()
        Dim fabricColourIdD As String = thisData.Tables(0).Rows(0).Item("FabricColourIdD").ToString()

        Dim fabricColourName As String = orderCfg.GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourId + "'")
        Dim fabricColourNameB As String = orderCfg.GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdB + "'")
        Dim fabricColourNameC As String = orderCfg.GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdC + "'")
        Dim fabricColourNameD As String = orderCfg.GetItemData("SELECT Name FROM FabricColours WHERE Id = '" + fabricColourIdD + "'")

        Dim partCategory As String = thisData.Tables(0).Rows(0).Item("PartCategory").ToString()
        Dim partComponent As String = thisData.Tables(0).Rows(0).Item("PartComponent").ToString()
        Dim partColour As String = thisData.Tables(0).Rows(0).Item("PartColour").ToString()
        Dim partLength As String = thisData.Tables(0).Rows(0).Item("PartLength").ToString()

        Dim controlPosition As String = thisData.Tables(0).Rows(0).Item("ControlPosition").ToString()

        If designName = "Additional" Then
            Dim addNumber As String = thisData.Tables(0).Rows(0).Item("AddNumber").ToString()
            result = blindName & " - " & productName
            If blindName = "Check Measure" And productName = "Template Fee" Then
                result = blindName & " | " & productName & " - " & "Item #" & addNumber
            End If
            If blindName = "Installation" And (productName = "Blind Products" Or productName = "Concrete/Brick" Or productName = "Program Hub") Then
                result = blindName & " | " & productName & " - " & "Item #" & addNumber
            End If
            If blindName = "Takedown" Then
                result = blindName & " | " & productName & " - " & "Item #" & addNumber
            End If
        End If

        If designName = "Aluminium Blind" Then
            result = itemDescription & " " & size
        End If

        If designName = "Cellular Shades" Then
            result = itemDescription & " #" & fabricColourName & " " & size
        End If

        If designName = "Curtain" Then
            result = itemDescription & " #" & fabricColourName & " " & size
        End If

        If designName = "LS Venetian" Then
            result = itemDescription & " " & size
        End If

        If designName = "Panel Glide" Then
            Dim tubeType As String = orderCfg.GetItemData("SELECT TubeType FROM Products WHERE Id = '" + productId + "'")

            result = itemDescription & " #" & fabricColourName & " " & size
            If tubeType = "Track Only" Then
                result = itemDescription & " (" & width & "mm)"
            End If
        End If

        If designName = "Panorama PVC Parts" Then
            result = itemDescription & " | " & partComponent
            If partCategory = "Louvres" Then
                result = itemDescription & " | " & partComponent & " - " & partColour & " (" & partLength & "mm)"
                If partComponent = "Louvre Repair Pin" Or partComponent = "Standard Louvre Pin" Then
                    result = itemDescription & " | " & partComponent & " - " & partColour
                End If
            End If

            If partCategory = "Framing | Hinged" Or partCategory = "Framing | Bi-fold or Sliding" Or partCategory = "Framing | Fixed" Or partCategory = "Extrusion" Then
                result = itemDescription & " | " & partComponent & " - " & partColour & " (" & partLength & "mm)"
            End If

            If partCategory = "Post" Then
                result = itemDescription & " | " & partComponent
                If partComponent = "T-Post" Or partComponent = "90° Corner Post" Or partComponent = "135° Bay Post" Then
                    result = itemDescription & " | " & partComponent & " - " & partColour & " (" & partLength & "mm)"
                End If
            End If

            If partCategory = "Hinges" Then
                result = itemDescription & " | " & partComponent
                If partComponent = "76mm Non-Mortise Hinge" Or partComponent = "76mm Rabbet Hinge" Then
                    result = itemDescription & " | " & partComponent & " - " & partColour
                End If
            End If

            If partCategory = "Magnets and Strikers" Then
                result = itemDescription & " | " & partComponent & " - " & partColour
                If partComponent = "Magnet" Then
                    result = itemDescription & " | " & partComponent
                End If
            End If

            If partCategory = "Track Hardware" Then
                result = itemDescription & " | " & partComponent
                If partComponent = "Top Track" Or partComponent = "Bottom M Track" Or partComponent = "Bottom U Track" Then
                    result = itemDescription & " | " & partComponent & " (" & partLength & "mm)"
                End If
            End If

            If partCategory = "Mist" Then
                result = itemDescription & " | " & partComponent & " - " & partColour
            End If
        End If

        If designName = "Panorama PVC Shutters" Then
            result = itemDescription & " - " & width & " x " & drop
        End If

        If designName = "Evolve Shutters" Then
            result = itemDescription & " - " & width & " x " & drop
        End If

        If designName = "Pelmet" Then
            result = itemDescription & " #" & fabricColourName & " " & width
            If String.IsNullOrEmpty(fabricColourName) Then
                result = itemDescription & " " & " (" & width & ")"
            End If
        End If

        If designName = "Roller Blind" Then
            result = itemDescription & " #" & fabricColourName & " " & size
            If blinds = 2 Then
                If blindName = "Double Bracket" Or blindName = "Link 2 Blinds Ind" Then
                    result = itemDescription & " #" & fabricColourNameB & " " & sizeB
                End If
                If blindName = "Link 2 Blinds Ind" Then
                    result = itemDescription & " " & sizeB
                End If
                If blindName = "Link 2 Blinds Dep" Or blindName = "Link 3 Blinds Dep" Then
                    result = "Roller" & " " & tubeName & " " & sizeB
                End If
            End If

            If blinds = 3 Then
                If blindName = "Link 3 Blinds Dep" Then
                    result = "Roller" & " " & tubeName & " " & sizeC
                End If
            End If
        End If

        If designName = "Roman Blind" Then
            result = itemDescription & " #" & fabricColourName & " " & size
        End If

        If designName = "Skin Only" Then
            result = itemDescription & " #" & fabricColourName & " " & size
        End If

        If designName = "Venetian Blind" Then
            result = itemDescription & " " & size
        End If

        If designName = "Veri Shades" Then
            result = itemDescription & " #" & fabricColourName & " " & size
        End If

        If designName = "Vertical" Then
            result = itemDescription & " #" & fabricColourName & "  " & size
            If blindName = "Track Only" Then
                result = itemDescription & "  " & size
            End If
            If blindName = "Blades Only" Then
                result = itemDescription & " #" & fabricColourName & " (" & drop & ")"
            End If
        End If

        If designName = "Zebra Blind" Then
            result = itemDescription & " #" & fabricColourName & " " & size
        End If
        Return result
    End Function

    Private Sub BindAdditionalStatus(Status As String)
        ddlStatusAdditional.Items.Clear()
        Try
            If Status = "New Order" Then
                ddlStatusAdditional.Items.Add(New ListItem("WAITING FOR DEPOSIT", "Waiting for Deposit"))
                ddlStatusAdditional.Items.Add(New ListItem("ON HOLD - ORDER DETAIL QUERY", "On Hold - Order Detail Query"))
                ddlStatusAdditional.Items.Add(New ListItem("ON HOLD - CUSTOMER REQUEST", "On Hold - Customer Request"))

                ddlStatusAdditional.Items.Add(New ListItem("ON HOLD - WAITING FOR TEMPLATE", "On Hold - Waiting for Template"))
                ddlStatusAdditional.Items.Add(New ListItem("ON HOLD - WAITING FOR COLOUR SWATCH", "On Hold - Waiting for Colour Swatch"))
                ddlStatusAdditional.Items.Add(New ListItem("ON HOLD - WAITING FOR DRAWING APPROVAL", "On Hold - Waiting for Drawing Approval"))
            End If

            If Status = "In Production" Then
                ddlStatusAdditional.Items.Add(New ListItem("WAITING FOR FINAL PAYMENT", "Waiting for Final Payment"))
                ddlStatusAdditional.Items.Add(New ListItem("ON HOLD - WAITING FOR TEMPLATE", "On Hold - Waiting for Template"))
                ddlStatusAdditional.Items.Add(New ListItem("ON HOLD - WAITING FOR COLOUR SWATCH", "On Hold - Waiting for Colour Swatch"))
                ddlStatusAdditional.Items.Add(New ListItem("ON HOLD - WAITING FOR DRAWING APPROVAL", "On Hold - Waiting for Drawing Approval"))
                ddlStatusAdditional.Items.Add(New ListItem("ORDER ASSIGNED TO SHIPMENT", "Order Assigned to Shipment"))
            End If

            If Status = "Completed" Then
                ddlStatusAdditional.Items.Add(New ListItem("WAITING FOR FINAL PAYMENT", "Waiting for Final Payment"))
            End If

            If ddlStatusAdditional.Items.Count > 0 Then
                ddlStatusAdditional.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                MessageError(True, "Please contact IT Support at reza@bigblinds.co.id")
                mailCfg.MailError(Page.Title, "BindAdditionalStatus", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Private Sub AllMessageError(Show As Boolean, Msg As String)
        MessageSuccess(Show, Msg)
        MessageError(Show, Msg)
        MessageError_Pricing(Show, Msg)
        MessageError_EditPricing(Show, Msg)
        MessageError_QuoteDetail(Show, Msg)
        MessageError_StatusAdditional(Show, Msg)
        MessageError_QuoteEmail(Show, Msg)
        MessageError_Deposit(Show, Msg)
        MessageError_CancelOrder(Show, Msg)
        MessageError_InternalNote(Show, Msg)
        MessageError_Preview(Show, Msg)
        MessageError_Log(Show, Msg)
    End Sub

    Private Sub MessageSuccess(Show As Boolean, Msg As String)
        divSuccess.Visible = Show : msgSuccess.InnerText = Msg
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : divErrorB.Visible = Show
        msgError.InnerText = Msg : msgErrorB.InnerText = Msg
    End Sub

    Private Sub MessageError_Pricing(Show As Boolean, Msg As String)
        divErrorPricing.Visible = Show : msgErrorPricing.InnerText = Msg
    End Sub

    Private Sub MessageError_EditPricing(Show As Boolean, Msg As String)
        divErrorEditPricing.Visible = Show : msgErrorEditPricing.InnerText = Msg
    End Sub

    Private Sub MessageError_QuoteDetail(Show As Boolean, Msg As String)
        divErrorQuoteDetail.Visible = Show : msgErrorQuoteDetail.InnerText = Msg
    End Sub

    Private Sub MessageError_StatusAdditional(Show As Boolean, Msg As String)
        divErrorStatusAdditional.Visible = Show : msgErrorStatusAdditional.InnerText = Msg
    End Sub

    Private Sub MessageError_QuoteEmail(Show As Boolean, Msg As String)
        divErrorQuoteEmail.Visible = Show : msgErrorQuoteEmail.InnerText = Msg
    End Sub

    Private Sub MessageError_Deposit(Show As Boolean, Msg As String)
        divErrorDeposit.Visible = Show : msgErrorDeposit.InnerText = Msg
    End Sub

    Private Sub MessageError_CancelOrder(Show As Boolean, Msg As String)
        divErrorCancelOrder.Visible = Show : msgErrorCancelOrder.InnerText = Msg
    End Sub

    Private Sub MessageError_InternalNote(Show As Boolean, Msg As String)
        divErrorInternalNote.Visible = Show : msgErrorInternalNote.InnerText = Msg
    End Sub

    Private Sub MessageError_Preview(Show As Boolean, Msg As String)
        divErrorPreview.Visible = Show : msgErrorPreview.InnerText = Msg
    End Sub

    Private Sub MessageError_Log(Show As Boolean, Msg As String)
        divErrorLog.Visible = Show : msgErrorLog.InnerText = Msg
    End Sub

    Protected Function VisibleDetail(ProductId As String) As Boolean
        Dim result As Boolean = False
        If Not ProductId = "" Then
            Dim designId As String = orderCfg.GetItemData("SELECT DesignId FROM Products WHERE Id = '" + ProductId + "'")

            Dim designName As String = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
            Dim designType As String = orderCfg.GetItemData("SELECT Type FROM Designs WHERE Id = '" + designId + "'")

            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Then
                result = True
            End If

            If Session("RoleName") = "Customer" Or Session("RoleName") = "Representative" Then
                result = True
                If spanStatusOrder.InnerText = "Unsubmitted" Then
                    result = False
                End If
                If designType = "Additional" Then
                    result = False
                End If
                If designType = "Evolve" Or designType = "Panorama" Then
                    result = True
                End If
            End If
        End If
        Return result
    End Function

    Protected Function VisibleCopy(ProductId As String) As Boolean
        Dim result As Boolean = False
        If Not ProductId = "" Then
            Dim designId As String = orderCfg.GetItemData("SELECT DesignId FROM Products WHERE Id = '" + ProductId + "'")

            Dim designName As String = orderCfg.GetItemData("SELECT Name FROM Designs WHERE Id = '" + designId + "'")
            Dim designType As String = orderCfg.GetItemData("SELECT Type FROM Designs WHERE Id = '" + designId + "'")

            If spanStatusOrder.InnerText = "Unsubmitted" Then
                If Session("RoleName") = "Customer" Or Session("RoleName") = "Representative" Then
                    If designType = "Evolve" Or designType = "Panorama" Then
                        result = True
                    End If
                End If

                If Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Then
                    If designType = "RBR" Or designType = "Curtain" Or designType = "Blinds" Or designType = "Veri Shades" Or designType = "Zebra Blinds" Then
                        result = False
                    End If
                    If designType = "Additional" Then : result = False : End If
                End If

                If Session("RoleName") = "Administrator" Then
                    result = True
                    If designName = "Additional" Then : result = False : End If
                End If
            End If

            If spanStatusOrder.InnerText = "New Order" Then
                If Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Then
                    result = True
                End If
                If Session("RoleName") = "Administrator" Then
                    result = True
                End If

                If (Session("RoleName") = "Customer" Or Session("RoleName") = "Representative") And spanStatusAdditional.InnerText = "On Hold - Customer Request" Then
                    result = True
                End If
            End If

            If designName = "Additional" Then
                result = False
            End If
        End If
        Return result
    End Function

    Protected Function VisibleDelete(ProductId As String) As Boolean
        Dim result As Boolean = False
        If Not ProductId = "" Then
            Dim designName As String = orderCfg.GetItemData("SELECT Designs.Name FROM Designs LEFT JOIN Products ON Designs.Id = Products.DesignId WHERE Products.Id = '" + ProductId + "'")
            If spanStatusOrder.InnerText = "Unsubmitted" Then
                If Session("RoleName") = "Customer" Or Session("RoleName") = "Representative" Then
                    result = True
                End If
                If Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Then
                    If lblCreatedBy.Text = Session("LoginId") Then
                        result = True
                    End If
                End If
                If Session("RoleName") = "Administrator" Then
                    result = True
                End If
            End If

            If spanStatusOrder.InnerText = "New Order" Then
                If Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Then
                    result = True
                End If
                If Session("RoleName") = "Administrator" Then
                    result = True
                End If

                If (Session("RoleName") = "Customer" Or Session("RoleName") = "Representative") And spanStatusAdditional.InnerText = "On Hold - Customer Request" Then
                    result = True
                End If
            End If
            If spanStatusOrder.InnerText = "In Production" Then
                If Session("RoleName") = "Administrator" Then
                    result = True
                End If
                If designName = "Additional" And (Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE") Then
                    result = True
                End If
            End If
        End If
        Return result
    End Function

    Protected Function VisibleProduction(ProductId As String, Production As String) As Boolean
        Dim result As Boolean = False

        If Session("RoleName") = "Administrator" And (Session("LevelName") = "Leader" OR Session("LevelName") = "Super Admin") Then
            If spanStatusOrder.InnerText = "New Order" And lblApproved.Text = 1 Then
                If Not ProductId = "" Then
                    result = True
                    Dim designName As String = orderCfg.GetItemData("SELECT Designs.Name FROM Designs LEFT JOIN Products ON Designs.Id = Products.DesignId WHERE Products.Id = '" + ProductId + "'")
                    If designName = "Additional" Then : result = False : End If
                End If
            End If

            If spanStatusOrder.InnerText = "In Production" Then
                If Not ProductId = "" Then
                    result = True
                    Dim designName As String = orderCfg.GetItemData("SELECT Designs.Name FROM Designs LEFT JOIN Products ON Designs.Id = Products.DesignId WHERE Products.Id = '" + ProductId + "'")
                    If designName = "Additional" Then : result = False : End If
                End If
            End If
        End If

        If Session("RoleName") = "Customer Service" Or Session("RoleName") = "Data Entry" OR Session("RoleName") = "PPIC & DE" Then
            If spanStatusOrder.InnerText = "New Order" And lblApproved.Text = 1 Then
                If Not ProductId = "" Then
                    result = True
                    Dim designName As String = orderCfg.GetItemData("SELECT Designs.Name FROM Designs LEFT JOIN Products ON Designs.Id = Products.DesignId WHERE Products.Id = '" + ProductId + "'")

                    If designName = "Additional" Then : result = False : End If
                End If
            End If
        End If
        Return result
    End Function

    Protected Function VisibleEditPricing() As Boolean
        If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer Service" Then
            If spanStatusOrder.InnerText = "Completed" Or spanStatusOrder.InnerText = "Canceled" Then
                Return False
            End If
            Return True
        End If
        Return False
    End Function

    Protected Function BindStylePrice(Price As Decimal) As String
        Dim result As String = String.Empty
        If Price > 0 Then : result = "btn btn-outline-secondary" : End If
        Return result
    End Function

    Protected Function BindStyleAuthorization(Status As String) As String
        Dim result As String = String.Empty
        If Status = "Authorized" Then
            result = "mt-3 badge bg-green text-green-fg"
        End If
        If Status = "Declined" Then
            result = "mt-3 badge bg-red text-red-fg"
        End If
        Return result
    End Function

    Protected Function BindTextAuthorization(Status As String, LoginId As String) As String
        Dim result As String = String.Empty

        If Not LoginId = "" Then
            Dim fullName As String = orderCfg.GetItemData("SELECT FullName FROM CustomerLogins WHERE Id = '" + UCase(LoginId).ToString() + "'")
            If Not Status = "" Then
                result = Status & " by " & fullName
            End If
        End If

        Return result
    End Function

    Protected Function VisibleAuthorizeDeclineListView(Status As String) As Boolean
        Dim result As Boolean = False
        If Status = "" Then : result = True : End If
        Return result
    End Function

    Protected Function VisibleResetListView(Status As String) As Boolean
        Dim result As Boolean = False
        If Status = "Authorized" Or Status = "Declined" Then : result = True : End If
        Return result
    End Function

    Protected Function ShowingPrice(Data As Decimal) As String
        Dim result As String = String.Empty
        If Data > 0 Then
            result = "$" & Data.ToString("N2", enUS)
        End If
        Return result
    End Function

    Protected Function BindMarkUp(MarkUp As Decimal) As String
        Dim result As String = String.Empty
        If MarkUp > 0 Then : result = MarkUp & "%" : End If
        Return result
    End Function

    Protected Function BindTextLog(Id As String) As String
        Dim result As String = String.Empty
        Try
            Dim thisData As DataSet = orderCfg.GetListData("SELECT * FROM Log_Orders WHERE Id = '" + Id + "'")
            Dim actionBy As String = thisData.Tables(0).Rows(0).Item("ActionBy").ToString()
            Dim actionDate As String = Convert.ToDateTime(thisData.Tables(0).Rows(0).Item("ActionDate")).ToString("dd MMM yyyy hh:mm")
            Dim description As String = thisData.Tables(0).Rows(0).Item("Description").ToString()

            Dim fullName As String = orderCfg.GetItemData("SELECT FullName FROM CustomerLogins WHERE Id = '" + UCase(actionBy).ToString() + "'")

            result = "<b>" & fullName & "</b> on " & actionDate & ". Action: " & description
        Catch ex As Exception
            result = ""
        End Try
        Return result
    End Function
End Class
