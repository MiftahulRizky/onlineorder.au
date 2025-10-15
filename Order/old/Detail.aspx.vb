Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Web.UI
Imports System.Web
Partial Class Order_Detail
    Inherits Page

    Dim publicCfg As New PublicConfig
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("headerId") = "" Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        lblHeaderId.Text = Session("headerId")
        If Not IsPostBack Then
            Call MessageError(False, String.Empty)
            Call MessageSuccess(False, String.Empty)

            Call BindDesignType()

            Call BindDataOrder(lblHeaderId.Text)
            Call CekOrder()
            Call PermissionAction()
        End If
    End Sub

    Protected Sub btnPreviewPrint_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If gvList.Rows.Count < 1 Then
                Call MessageError(True, "PLEASE ADD MINIMAL 1 ITEM !")
                Exit Sub
            End If

            Call CreatePDFOrder(lblHeaderId.Text, "Preview")
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnPreviewPrint_Click", ex.ToString())
            End If
        End Try
    End Sub


    Protected Function btnReprintJobSheet_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Call CreateJobSheet(lblHeaderId.Text, "Reprint")
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnReprintJobSheet_Click", ex.ToString())
            End If
        End Try
    End Function


    Protected Function btnConvertToJob_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            '#CreateJobSheet
            publicCfg.CreateJobSheet(lblHeaderId.Text)
            '#message success
            Dim myScript As String = "window.onload = function() { Swal.fire({ title: 'Success!', text: 'Job successfully created!', icon: 'success', showConfirmButton: false,timer: 1800 }).then(()=> { window.location = '/order/detail'   }); };"
            ClientScript.RegisterStartupScript(Me.GetType(), "showAlerts", myScript, True)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnConvertToJob_Click", ex.ToString())
            End If
        End Try
    End Function

    Protected Function btnReloadPricing_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim headerId As String = lblHeaderId.Text
            Dim detailQuery As String = "SELECT * FROM view_details WHERE HeaderId = '" + headerId + "' AND Active='1' ORDER BY Id, BlindNo, DesignName ASC"
            ' HITUNG HARGA
            If spanStatusOrder.InnerText <> "Canceled" Then
                Dim hargaData As DataSet = publicCfg.GetListData(detailQuery)
                If hargaData.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To hargaData.Tables(0).Rows.Count - 1
                        Dim itemId As String = hargaData.Tables(0).Rows(i).Item("Id").ToString()
                        Dim BlindName As String = hargaData.Tables(0).Rows(i).Item("BlindName").ToString()
                        Dim TubeType As String = hargaData.Tables(0).Rows(i).Item("TubeType").ToString()
                        Dim FabricId As String = hargaData.Tables(0).Rows(i).Item("FabricId").ToString()
                        Dim DesignId As String = hargaData.Tables(0).Rows(i).Item("DesignId").ToString() 
                        Dim DesignName As String = hargaData.Tables(0).Rows(i).Item("DesignName").ToString() 
                        Dim BottomHoldDown As String = hargaData.Tables(0).Rows(i).Item("BottomHoldDown").ToString() 
                        Dim FabricGroups As String = hargaData.Tables(0).Rows(i).Item("FabricGroups").ToString() 

                        Dim fabricGroup As String = publicCfg.GetFabricGroup(FabricId)

                        '#Validate
                        Dim priceGroupName As String = ""
                        ' If TubeType = "Track Only" Then
                        '     priceGroupName = BlindName + " - " + TubeType
                        ' End If
                        '#----------------------|| Vertical Blinds ||----------------------#
                        If DesignName = "Vertical Blinds" Then
                            priceGroupName = BlindName + " - " + fabricGroup
                            If BlindName = "Track Only" Then
                                priceGroupName = BlindName & " - " & TubeType
                            End If
                            If BlindName = "Slat Only" And BottomHoldDown = "Top Hanger Only" Then
                                priceGroupName = BlindName & " With Hanger - " & fabricGroup
                            End If
                        End If
                        '#----------------------|| Veri Shades ||----------------------#
                        If DesignName = "Veri Shades" Then
                            If BlindName = "Single" Then
                                priceGroupName = "Veri Shades" & " - " & fabricGroup
                            End If
                            If BlindName = "Slat Only" Then
                                priceGroupName = BlindName & " - " & fabricGroup
                            End If
                            priceGroupName = BlindName
                        End If
                        '#----------------------|| Venetian Blinds ||----------------------#
                        If DesignName = "Venetian Blinds" Then
                            priceGroupName = BlindName
                        End IF
                        '#----------------------|| Roller Blinds ||----------------------#
                        If DesignName = "Roller Blinds" Then
                            priceGroupName = "Roller Blind" & " - " & fabricGroup
                            If BlindName = "Skin Only" Then
                                priceGroupName = "Roller Skin Only" & " - " & fabricGroup
                            End If
                        End IF
                        '#----------------------|| Panorama Shutters ||----------------------#
                        If DesignName = "Panorama Shutters" Then
                           priceGroupName = "Panorama - " & BlindName
                        End IF
                        '#----------------------|| Aluminium Blinds ||----------------------#
                        If DesignName = "Aluminium Blinds" Then
                            priceGroupName = BlindName
                        End IF
                        '#----------------------|| Panel Glides ||----------------------#
                        If DesignName = "Panel Glides" Then
                            priceGroupName = "Panel Glide - " & FabricGroups
                        End IF

                        '#Get PriceGroupId
                        Dim priceGroupId As String = publicCfg.GetPriceGroupId(DesignId, priceGroupName)
                        '#validate & update price group on order detail
                        If Not priceGroupId = "" Then
                            Call publicCfg.UpdatePriceGroup(itemId, UCase(priceGroupId).ToString())
                        End If
                        Call publicCfg.ResetPriceDetail(itemId)
                        Call publicCfg.HitungHarga(headerId, itemId)
                        Call publicCfg.HitungSurcharge(headerId, itemId)
                    Next
                End If
            End If
            '#message success
            Dim myScript As String = "window.onload = function() { Swal.fire({ title: 'Success!', text: 'Pricing successfully reloaded!', icon: 'success', showConfirmButton: false,timer: 1800 }).then(()=> { window.location = '/order/detail'   }); };"
            ClientScript.RegisterStartupScript(Me.GetType(), "showAlerts", myScript, True)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnConvertToJob_Click", ex.ToString())
            End If
        End Try
    End Function

    Protected Sub btnDownloadJobSheet_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Call CreateJobSheet(lblHeaderId.Text, "Download")
        Catch ex As Exception
         Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnConvertToJob_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnPreviewPDF_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If gvList.Rows.Count < 1 Then
                Call MessageError(True, "PLEASE ADD MINIMAL 1 ITEM !")
                Exit Sub
            End If

            Dim userId As String = UCase(Session("UserId")).ToString()
            publicCfg.InsertActivity(userId, Page.Title, "DOWNLOAD PDF ORDER : " & lblHeaderId.Text)

            Call CreatePDFOrder(lblHeaderId.Text, "Download")
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnPreviewPDF_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If Not gvList.Rows.Count > 0 Then
                Call MessageError(True, "PLEASE ADD MINIMAL 1 ITEM !")
                Exit Sub
            End If

            sdsSubmit.Update()

            Dim fileDirectory As String = Server.MapPath("~/file/order/mail/")
            Call CreatePDFOrder(lblHeaderId.Text, "Mail")
            Call publicCfg.MailOrder(lblHeaderId.Text, fileDirectory)

            Call BindDataOrder(lblHeaderId.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnEditHeader_Click(sender As Object, e As EventArgs)
        Session("headerAction") = "EditHeader"
        Session("headerId") = lblHeaderId.Text
        Response.Redirect("~/order/header", False)
    End Sub

    Protected Sub btnQuoteDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            ' Call MessageError(True, "UNDER MAINTENANCE !")
            ' Exit Sub
            ' Session("headerId") = lblHeaderId.Text
            ' Response.Redirect("~/order/quote", False)
            Call CreatePDFQuote(lblHeaderId.Text, "Reprint")
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnQuoteDetail_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnDownloadQuote_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Call CreatePDFQuote(lblHeaderId.Text, "Download")
            Call BindDataOrder(lblHeaderId.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnDownloadQuote_Click", ex.ToString())
            End If
        End Try
    End Sub


    '#function create job sheet
    Private Sub CreatePDFQuote(HeaderId As String, Action As String)
        If Not gvList.Rows.Count > 1 Then
            Call MessageError(True, "PLEASE ADD MINIMAL 1 ITEM !")
            Exit Sub
        End If

        Dim printCfg As New PrintConfig

        Dim fileName As String = Trim("Quote_" & lblOrderNo.Text.Replace(" ", "") & "-" & lblStoreId.Text & ".pdf")

        Dim fileDirectory As String = Server.MapPath("~/file/order/quote")
        Call printCfg.CreatePDFQuote(HeaderId, Session("UserName"), fileDirectory, fileName)

        If Action = "Reprint" Then
            Response.Clear()
            Dim url As String = "/order/printquote"
            Session("Reprint") = fileName & "#toolbar=0"

            Dim sb As New StringBuilder()
            sb.Append("<scrip type = 'text/javascript'>")
            sb.Append("window.open('")
            sb.Append(url)
            sb.Append("');")
            sb.Append("</script>")
            ClientScript.RegisterStartupScript(Me.GetType(), "script", sb.ToString())
        End If

        If Action = "Download" Then
             Response.Clear()

            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(fileDirectory + "/" + fileName)))
            Response.WriteFile(fileDirectory + "/" + fileName)
            Response.Flush()
            Response.SuppressContent = True
            ApplicationInstance.CompleteRequest()
        End If

    End Sub

    Protected Sub btnChangeStatus_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order/change", False)
    End Sub


    Protected Sub btnSendOrderMail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If spanStatusOrder.InnerText = "Draft" Or spanStatusOrder.InnerText = "Canceled" Then
                Call MessageError(True, "ORDER STATUS IS NOT ALLOWED TO SEND AUTOMATED EMAILS !")
                Exit Sub
            End If

            Dim fileDirectory As String = Server.MapPath("~/file/order/mail/")
            Call CreatePDFOrder(lblHeaderId.Text, "Mail")
            Call publicCfg.MailOrder(lblHeaderId.Text, fileDirectory)

            Call BindDataOrder(lblHeaderId.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSendOrderMail_Click", ex.ToString())
            End If
        End Try
    End Sub


    Protected Sub btnSubmitAdd_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If ddlDesign.SelectedValue = "" Then
                Response.Redirect("~/order/detail", False)
                Exit Sub
            End If

            Session("headerId") = lblHeaderId.Text
            Session("itemAction") = "AddItem"
            Session("designId") = UCase(ddlDesign.SelectedValue).ToString()
            Dim page As String = publicCfg.GetDesignPage(ddlDesign.SelectedValue)
            Response.Redirect(page, False)
            Exit Sub
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmitAdd_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnFinish_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order", False)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        Call MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            Call BindDataOrder(lblHeaderId.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "gvList_PageIndexChanging", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub linkNext_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim itemId As String = row.Cells(1).Text
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id='" + itemId + "' AND Active=1")
            Dim designId As String = myData.Tables(0).Rows(0).Item("DesignId").ToString()
            Dim page As String = publicCfg.GetDesignPage(designId)

            Session("headerId") = lblHeaderId.Text
            Session("designId") = designId
            Session("itemId") = itemId
            Session("itemAction") = "NextItem"

            Response.Redirect(page, False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "linkNext_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub linkDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim itemId As String = row.Cells(1).Text

            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id='" + itemId + "' AND Active=1")
            Dim designId As String = myData.Tables(0).Rows(0).Item("DesignId").ToString()
            Dim page As String = publicCfg.GetDesignPage(designId)

            Session("headerId") = lblHeaderId.Text
            Session("designId") = designId
            Session("itemAction") = "ViewItem"
            Session("itemId") = itemId

            Response.Redirect(page, False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "linkDetail_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub linkEdit_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim itemId As String = row.Cells(1).Text

            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id='" + itemId + "' AND Active=1")
            Dim designId As String = myData.Tables(0).Rows(0).Item("DesignId").ToString()
            Dim page As String = publicCfg.GetDesignPage(designId)

            Session("headerId") = lblHeaderId.Text
            Session("designId") = designId
            Session("itemAction") = "EditItem"
            Session("itemId") = itemId

            Response.Redirect(page, False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "linkEdit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCopyDetail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            lblItemId.Text = txtIdCopy.Text
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id='" + lblItemId.Text + "' AND Active=1")
            Dim designId As String = myData.Tables(0).Rows(0).Item("DesignId").ToString()
            Dim bracketType As String = myData.Tables(0).Rows(0).Item("BracketType").ToString()

            lblItemIdNew.Text = publicCfg.CreateOrderItemId()
            lblBlindNo.Text = "Blind 1"
            If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                lblUniqueId.Text = GenerateUniqueId()
            End If
            sdsCopy.Insert()

            Response.Redirect("~/order/detail", False)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnCopyDetail_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub linkPricing_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            Dim rowIndex As Integer = Convert.ToInt32(TryCast(TryCast(sender, LinkButton).NamingContainer, GridViewRow).RowIndex)
            Dim row As GridViewRow = gvList.Rows(rowIndex)

            Dim itemId As String = row.Cells(1).Text

            Dim queryDetailsPrice As String = "SELECT *, FORMAT( CASE WHEN Description LIKE '%Discount%' THEN -Cost ELSE Cost END, 'C', 'en-US') AS FormatCost, FORMAT(CASE WHEN Description LIKE '%Discount%' THEN -FinalCost ELSE FinalCost END, 'C', 'en-US') AS FormatFinalCost FROM OrderDetailsPrice WHERE ItemId = '" + itemId + "' ORDER BY Description ASC"

            gvPricing.DataSource = publicCfg.GetListData(queryDetailsPrice)
            gvPricing.DataBind()

            Dim myScript As String = "window.onload = function() { showDetailPrice(); };"
            ClientScript.RegisterStartupScript(Me.GetType(), "showDetailPrice", myScript, True)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "linkPricing_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If txtActionDelete.Text = "Header" Then
                sdsHeader.Delete()
                Response.Redirect("~/order", False)
            End If
            If txtActionDelete.Text = "Detail" Then
                lblItemId.Text = txtIdDelete.Text
                Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id='" + lblItemId.Text + "' AND Active=1")

                Dim bracketName As String = myData.Tables(0).Rows(0).Item("BracketType").ToString()
                Dim blindNo As String = myData.Tables(0).Rows(0).Item("BlindNo").ToString()
                lblUniqueId.Text = myData.Tables(0).Rows(0).Item("UniqueId").ToString()

                sdsDetail.Update()
                sdsDetail.Delete()

                If bracketName = "Double" Or bracketName = "Linked 2 Blinds (Dep)" Or bracketName = "Linked 2 Blinds (Ind)" Then
                    If blindNo = "Blind 1" Then
                        lblBlindNo.Text = "Blind 2"
                        lblBlindNoNew.Text = "Blind 1"

                        sdsDetailBlindNo.Update()
                    End If
                End If

                If bracketName = "Linked 3 Blinds (Dep)" Or bracketName = "Linked 3 Blinds (Ind)" Then
                    If blindNo = "Blind 1" Then
                        lblBlindNo.Text = "Blind 2"
                        lblBlindNoNew.Text = "Blind 1"

                        sdsDetailBlindNo.Update()

                        lblBlindNo.Text = "Blind 3"
                        lblBlindNoNew.Text = "Blind 2"

                        sdsDetailBlindNo.Update()
                    End If

                    If blindNo = "Blind 2" Then
                        lblBlindNo.Text = "Blind 3"
                        lblBlindNoNew.Text = "Blind 2"

                        sdsDetailBlindNo.Update()
                    End If
                End If
                
                Response.Redirect("~/order/detail", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnDelete_Click", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataOrder(headerId As String)
        Session("headerAction") = "" : Session("designId") = ""
        Session("itemId") = "" : Session("itemAction") = ""
        Session("printPreview") = ""
        Try
            Dim headerData As DataSet = publicCfg.GetListData("SELECT * FROM view_headers WHERE Id = '" + headerId + "'")

            If headerData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/order/", False)
                Exit Sub
            End If

            Dim JoNo As String = headerData.Tables(0).Rows(0).Item("JoNumber").ToString()
            Dim spanJobNo As String = "-"
            If JoNo <> "" Then
                spanJobNo = "<span class='badge badge-outline text-red' style='font-size:larger;'>" + JoNo + "</span>"
            End If
            spanJoNumber.InnerHtml = spanJobNo

            spanOrderNo.InnerText = headerData.Tables(0).Rows(0).Item("OrderNo").ToString()
            spanOrderCust.InnerText = headerData.Tables(0).Rows(0).Item("OrderCust").ToString()
            spanCreatedDate.InnerText = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CreatedDate")).ToString("MMM, dd yyyy")
            spanCreatedBy.InnerText = headerData.Tables(0).Rows(0).Item("UserName").ToString()

            spanStatusOrder.InnerText = headerData.Tables(0).Rows(0).Item("Status").ToString()
            spanDelivery.InnerText = headerData.Tables(0).Rows(0).Item("Delivery").ToString()
            spanStatusNote.InnerHtml = headerData.Tables(0).Rows(0).Item("StatusDescription").ToString()
            spanNote.InnerHtml = headerData.Tables(0).Rows(0).Item("Note").ToString()

            spanSubmittedDate.InnerText = "-"
            spanCompletedDate.InnerText = "-"
            spanCanceledDate.InnerText = "-"
            If Not headerData.Tables(0).Rows(0).Item("SubmittedDate").ToString() = "" Then
                spanSubmittedDate.InnerText = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("SubmittedDate")).ToString("MMM, dd yyyy")
            End If
            If Not headerData.Tables(0).Rows(0).Item("CompletedDate").ToString() = "" Then
                spanCompletedDate.InnerText = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CompletedDate")).ToString("MMM, dd yyyy")
            End If
            If Not headerData.Tables(0).Rows(0).Item("CanceledDate").ToString() = "" Then
                spanCanceledDate.InnerText = Convert.ToDateTime(headerData.Tables(0).Rows(0).Item("CanceledDate")).ToString("MMM, dd yyyy")
            End If

            lblUserId.Text = UCase(headerData.Tables(0).Rows(0).Item("UserId").ToString())
            lblOrderNo.Text = headerData.Tables(0).Rows(0).Item("OrderNo").ToString()
            lblOrderCust.Text = headerData.Tables(0).Rows(0).Item("OrderCust").ToString()

            lblStoreId.Text = headerData.Tables(0).Rows(0).Item("StoreId").ToString()
            lblStoreType.Text = headerData.Tables(0).Rows(0).Item("StoreType").ToString()
            lblCompany.Text = headerData.Tables(0).Rows(0).Item("StoreCompany").ToString()

            aSubmit.Visible = False
            btnEditHeader.Visible = False
            aDeleteHeader.Visible = False
            btnQuote.Visible = False : btnQuoteDetail.Visible = False : btnDownloadQuote.Visible = False
            btnAdministrator.Visible = False : btnChangeStatus.Visible = False : btnSendOrderMail.Visible = False
            aAddItem.Visible = False
            divPrice.Visible = False

            '#JOB SHEET BUTTON
            btnJobSheet.Visible = False
            btnReloadPricing.Visible = False
            Dim statusOrder As String = headerData.Tables(0).Rows(0).Item("Status").ToString()
            If statusOrder <> "Draft" And statusOrder <> "Canceled" Then
                    btnJobSheet.Visible = True
                If Session("RoleName") <> "Administrator" Then
                    btnJobSheet.Visible = False
                End If
            End If
            If statusOrder <> "Canceled" Then
                 btnReloadPricing.Visible = True
                 If Session("RoleName") <> "Administrator" Then
                    btnReloadPricing.Visible = False
                End If
            End If

            pSubmit.InnerHtml = ""

            If Session("RoleName") = "Administrator" Or Session("RoleName") = "Customer" Then
                If Session("PriceAccess") = True Then
                    btnQuote.Visible = True : btnQuoteDetail.Visible = True : btnDownloadQuote.Visible = True
                End If
            End If

            If Session("PriceAccess") = True Then
                divPrice.Visible = True
            End If

            If spanStatusOrder.InnerText = "Draft" Then
                If Session("RoleName") = "Customer" Then
                    btnEditHeader.Visible = True
                    aSubmit.Visible = True
                    aDeleteHeader.Visible = True
                    aAddItem.Visible = True
                End If
                If Session("RoleName") = "PPIC & DE" Then
                    If lblUserId.Text = UCase(Session("UserId")).ToString() Then
                        btnEditHeader.Visible = True
                        aDeleteHeader.Visible = True
                        aAddItem.Visible = True
                    End If
                End If

                If Session("RoleName") = "Administrator" Then
                    btnEditHeader.Visible = True
                    aSubmit.Visible = True
                    aDeleteHeader.Visible = True
                    aAddItem.Visible = True
                End If
            End If

            If spanStatusOrder.InnerText = "New Order" Or spanStatusOrder.InnerText = "In Production" Or spanStatusOrder.InnerText = "Completed" Or spanStatusOrder.InnerText = "On Hold" Then
                pSubmit.InnerHtml = "Thank you for submitting your order. Your order will be processed within 1 business day.<br /> Once your order has been processed, you can check the status from web order.<br /> Please do not fax us any paper work in addition to this online order as it may result in duplication."

                If Session("RoleName") = "Administrator" Or Session("RoleName") = "PPIC & DE" Then
                    btnAdministrator.Visible = True
                    btnChangeStatus.Visible = True
                    If Session("RoleName") = "Administrator" Then
                        btnSendOrderMail.Visible = True
                    End If

                    If Session("RoleName") = "Administrator" Then
                        aAddItem.Visible = True
                    End If
                End If
                If spanStatusOrder.InnerText = "Canceled" Then
                    pSubmit.InnerHtml = ""
                End If
            End If

            Dim detailQuery As String = "SELECT * FROM view_details WHERE HeaderId = '" + headerId + "' AND Active='1' ORDER BY Id, BlindNo, DesignName ASC"
            ' HITUNG HARGA
            If spanStatusOrder.InnerText = "Draft" Then
                Dim hargaData As DataSet = publicCfg.GetListData(detailQuery)
                If hargaData.Tables(0).Rows.Count > 0 Then
                    For i As Integer = 0 To hargaData.Tables(0).Rows.Count - 1
                        Dim itemId As String = hargaData.Tables(0).Rows(i).Item("Id").ToString()

                        Call publicCfg.ResetPriceDetail(itemId)
                        Call publicCfg.HitungHarga(headerId, itemId)
                        Call publicCfg.HitungSurcharge(headerId, itemId)
                    Next
                End If
            End If


            gvList.DataSource = publicCfg.GetListData(detailQuery)
            gvList.DataBind()
            gvList.Columns(5).Visible = False ' COST
            gvList.Columns(6).Visible = False ' MARK UP
            If Session("PriceAccess") = True Then
                gvList.Columns(5).Visible = True ' COST
            End If
            If Session("MarkUpAccess") = True Then
                gvList.Columns(6).Visible = True ' MARK UP
            End If

            spanTotal.InnerHtml = "-"
            spanGST.InnerHtml = "-"
            spanFinalTotal.InnerHtml = "-"
            Dim spanStart = "<span class='badge badge-outline text-green' style='font-size:larger;'>$"
            Dim spanEnd = "</span>"
            If gvList.Rows.Count > 0 And Session("PriceAccess") = True Then
                Dim sumPrice As Decimal = publicCfg.GetItemData_Decimal("SELECT SUM(TotalMatrix + TotalCharge) AS SumPrice FROM OrderDetails WHERE HeaderId = '" + headerId + "' AND Active=1")
                If sumPrice > 0 Then
                    Dim gst As Decimal = sumPrice * 10 / 100
                    Dim finaltotal As Decimal = sumPrice + gst

                    spanTotal.InnerHtml = spanStart  & sumPrice.ToString("N2", enUS) & spanEnd
                    spanGST.InnerHtml = spanStart & gst.ToString("N2", enUS) & spanEnd
                    spanFinalTotal.InnerHtml = spanStart & finaltotal.ToString("N2", enUS) & spanEnd
                End If
            End If



            '#hide Reprint JobSheet if no JO
            Dim thisData As DataSet = publicCfg.GetListData("SELECT Id, JoNumber FROM Orderheaders WHERE Id='" + lblHeaderId.Text + "' AND Active=1")
            If thisData.Tables(0).Rows.Count > 0 Then
                Dim joNumber As String = thisData.Tables(0).Rows(0).Item("JoNumber").ToString()
                lblJoNumber.Text = joNumber
                If joNumber = "" Then
                    btnReprintJobSheet.Visible = False
                    btnDownloadJobSheet.Visible = False
                End If
            End If

        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataOrder", ex.ToString())
            End If
        End Try
    End Sub

    Protected Function BindItemDescription(ItemId As String) As String
        Dim result As String = ""

        Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id='" + ItemId + "'")

        Dim designName As String = thisData.Tables(0).Rows(0).Item("DesignName").ToString()
        Dim blindName As String = thisData.Tables(0).Rows(0).Item("BlindName").ToString()
        Dim kitName As String = thisData.Tables(0).Rows(0).Item("KitName").ToString()
        Dim bracketType As String = thisData.Tables(0).Rows(0).Item("BracketType").ToString()

        Dim fabricType As String = thisData.Tables(0).Rows(0).Item("FabricType").ToString()

        Dim blindNo As String = thisData.Tables(0).Rows(0).Item("BlindNo").ToString()
        Dim uniqueId As String = thisData.Tables(0).Rows(0).Item("UniqueId").ToString()

        Dim width As String = thisData.Tables(0).Rows(0).Item("Width").ToString()
        Dim drop As String = thisData.Tables(0).Rows(0).Item("Drop").ToString()

        result =  kitName & " (" & width & " x " & drop & ")"

        If designName = "Aluminium Blinds" Or designName = "Venetian Blinds" Then
            result = kitName & " (" & width & " x " & drop & ")"
        End If

        If designName = "Roller Blinds" Then
            Dim product As String = String.Empty
            result = kitName & " #" & fabricType & " (" & width & " x " & drop & ")"
            '#Linked 3 Blinds (Dep) & Linked 3 Blinds (Ind)
            If bracketType = "Linked 3 Blinds (Dep)" Or bracketType = "Linked 3 Blinds (Ind)" Then
                '#blinds1
                If blindNo = "Blind 1" Then
                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    If Not getConnectedId2 = "" Then
                        getConnectedId2 = " & ITEM ID " & getConnectedId2
                    End If
                    If Not getConnectedId = "" Then
                        result += "<br />"
                        result += "<small style='color:red;'>* LINKED ITEM ID " & getConnectedId & getConnectedId2 & "</small>"
                    End If
                End If

                If blindNo = "Blind 2" Then
                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    If Not getConnectedId2 = "" Then
                        getConnectedId2 = " & ITEM ID " & getConnectedId2
                    End If
                    If Not getConnectedId = "" Then
                        result += "<br />"
                        result += "<small style='color:red;'>* LINKED ITEM ID " & getConnectedId & getConnectedId2 & "</small>"
                    End If
                End If

                If blindNo = "Blind 3" Then
                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim getConnectedId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    If Not getConnectedId2 = "" Then
                        getConnectedId2 = " & ITEM ID " & getConnectedId2
                    End If
                    If Not getConnectedId = "" Then
                        result += "<br />"
                        result += "<small style='color:red;'>* LINKED ITEM ID " & getConnectedId & getConnectedId2 & "</small>"
                    End If
                End If
            End If


            '#Double and Link System Dep & Double and Link System Ind
            If bracketType = "Double and Link System Dep" Or bracketType = "Double and Link System Ind" Then
                '#blinds 1
                If blindNo = "Blind 1" Then
                    Dim blind2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim blind3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim blind4 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                    Dim spare As String = ""
                     If Not blind3 = "" Then
                        blind3 = "ITEM ID " & blind3
                        spare = " & "
                    End If
                    If Not blind4 = "" Then
                        blind4 = " & ITEM ID " & blind4
                        spare = ", "
                    End If
                    If Not blind2 = "" Then
                        result += "<br />"
                        result += "<small style='color:red;'>* LINKED ITEM ID " & blind2 & spare & blind3 & blind4 & "</small>"
                    End If
                End If

                '#blinds 2
                If blindNo = "Blind 2" Then
                    Dim blind1 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim blind3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim blind4 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                    Dim spare As String = ""
                    If Not blind3 = "" Then
                        blind3 = "ITEM ID " & blind3
                        spare = " & "
                    End If
                    If Not blind4 = "" Then
                        blind4 = " & ITEM ID " & blind4
                        spare = ", "
                    End If
                    If Not blind1 = "" Then
                        result += "<br />"
                        result += "<small style='color:red;'>* LINKED ITEM ID " & blind1 & spare & blind3 & blind4 & "</small>"
                    End If
                End If

                '#blinds 3
                If blindNo = "Blind 3" Then
                    Dim blind1 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim blind2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim blind4 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + uniqueId + "' AND Active = 1")

                    Dim spare As String = ""
                    If blind4 = "" Then
                        spare = " & "
                    End If
                    If Not blind4 = "" Then
                        blind4 = " & ITEM ID " & blind4
                        spare = ", "
                    End If
                    If Not blind2 = "" Then
                        blind2 = "ITEM ID " & blind2
                    End If
                    If Not blind1 = "" Then
                        result += "<br />"
                        result += "<small style='color:red;'>* LINKED ITEM ID " & blind1 & spare & blind2 & blind4 & "</small>"
                    End If
                End If

                '#blinds 4
                If blindNo = "Blind 4" Then
                    Dim blind1 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim blind2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    Dim blind3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    result += "<br />"
                    result += "<small style='color:red;'>* LINKED ITEM ID " & blind1 & ", ITEM ID " & blind2 & " & " & "ITEM ID" & blind3 & "</small>"
                End If
            End If


            '#Double, Linked 2 Blinds (Dep), Linked 2 Blinds (Ind)
            If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                If blindNo = "Blind 1" Then
                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    If Not getConnectedId = "" Then
                        result += "<br />"
                        result += "<small style='color:red;'>* Complete set with ITEM ID " & getConnectedId & "</small>"
                    End If
                End If

                If blindNo = "Blind 2" Then
                    Dim getConnectedId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + uniqueId + "' AND Active = 1")
                    If Not getConnectedId = "" Then
                        result += "<br />"
                        result += "<small style='color:red;'>* Complete set with ITEM ID " & getConnectedId & "</small>"
                    End If
                End If
            End If

            If bracketType = "With Tube & Bottom Included" then
                result = "Roller Skin Only (+Tube & Bottom Inc) #" & fabricType & " (" & width & " x " & drop & ")"
            End If


            If bracketType = "With Bottom Included" then
                result = "Roller Skin Only (+Bottom Inc) #" & fabricType & " (" & width & " x " & drop & ")"
            End If
        End If

        If designName = "Veri Shades" Or designName = "Vertical Blinds" Then
            Dim product As String = kitName & " #" & fabricType & " (" & width & " x " & drop & ")"
            If blindName = "Slat Only" Then
                product = kitName & " #" & fabricType & " (Drop : " & drop & "mm)"
            End If
            If blindName = "Track Only" Then
                product = kitName & " (Width : " & width & "mm)"
            End If
            result = product
        End If

        If designName = "Panorama Shutters" Then
            result =  "Panorama " & kitName & " (" & width & "mm x " & drop & "mm)"
        End If

        If designName = "Panel Glides" Then
            result =  kitName & " #" & fabricType & " (" & width & " x " & drop & ")"
        End If

        If designName = "Roman Blinds" Then
            result =  kitName & " #" & fabricType & " (" & width & " x " & drop & ")"
        End If

        Return result
    End Function

    Private Sub CekOrder()
        Dim idResult As String = String.Empty
        Dim action As String = String.Empty

        Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE HeaderId='" + lblHeaderId.Text + "' AND Active=1 ORDER BY Id ASC")
        If thisData.Tables(0).Rows.Count > 0 Then
            For i As Integer = 0 To thisData.Tables(0).Rows.Count - 1
                Dim id As String = thisData.Tables(0).Rows(i).Item("Id").ToString()
                Dim uniqueId As String = thisData.Tables(0).Rows(i).Item("UniqueId").ToString()
                Dim designName As String = thisData.Tables(0).Rows(i).Item("DesignName").ToString()
                Dim bracketType As String = thisData.Tables(0).Rows(i).Item("BracketType").ToString()

                Dim totalBlind As Integer = publicCfg.GetItemData("SELECT COUNT(*) FROM view_details WHERE UniqueId = '" + uniqueId + "' AND Active = 1")
                If designName = "Roller Blinds" Then
                    If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                        If totalBlind < 2 Then
                            action = "Yes"
                            idResult += "<b> " & id & "</b>,"
                        End If
                    End If

                    If bracketType = "Linked 3 Blinds (Dep)" Or bracketType = "Linked 3 Blinds (Ind)" Then
                        If totalBlind < 3 Then
                            action = "Yes"
                            idResult += "<b> " & id & "</b>,"
                        End If
                    End If

                    If bracketType = "Double and Link System Dep" Or bracketType = "Double and Link System Ind" Then
                        If totalBlind < 4 Then
                            action = "Yes"
                            idResult += "<b> " & id & "</b>,"
                        End If
                    End If

                End If
            Next

            If Not spanStatusOrder.InnerText = "Draft" Then
                action = "No"
            End If

            If Session("RoleName") = "PPIC & DE" And Not lblUserId.Text = UCase(Session("UserId")) Then
                action = "No"
            End If


        End If
        If action = "Yes" Then
            idResult.Remove(idResult.Length - 1).ToString()
            Dim myScript As String = "window.onload = function() { showConfirm('" + idResult.Remove(idResult.Length - 1).ToString() + "'); };"
            ClientScript.RegisterStartupScript(Me.GetType(), "showConfirm", myScript, True)
        End If
    End Sub

    Private Sub BindDesignType()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = publicCfg.GetListData("SELECT Id, UPPER(Name) AS NameText FROM Designs WHERE Active=1 ORDER BY Name ASC")
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 1 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
                publicCfg.MailError(Page.Title, "BindDesignType", Session("UserId"), ex.ToString())
            End If
        End Try
    End Sub



    Protected Function BindDetailCost(Matrix As Decimal, Charge As Decimal) As String
        Dim result As String = String.Empty
        Dim enUS As CultureInfo = New CultureInfo("en-US")
        Dim totalCost As Decimal = 0.00
        If Matrix > 0 Then
            totalCost = Matrix + Charge
            result = "$" & totalCost.ToString("N2", enUS)
        End If
        Return result
    End Function

    Protected Function BindDetailMarkUp(MarkUp As Decimal) As String
        Dim result As String = String.Empty
        If MarkUp > 0 Then : result = MarkUp & "%" : End If
        Return result
    End Function

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : divErrorB.Visible = False
        ' msgError.InnerText = Msg : msgErrorB.InnerText = Msg
        ' If Show = True Then : divError.Visible = True : divErrorB.Visible = True : End If
        If Show = True Then
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub

    Private Sub MessageSuccess(Show As Boolean, Msg As String)
        divSuccess.Visible = False
        msgSuccess.InnerText = Msg 
        If Show = True Then : divSuccess.Visible = True : End If
    End Sub

    Protected Function VisiblePricing() As Boolean
        Dim result As Boolean = False
        If Session("PriceAccess") = True Then : result = True : End If
        Return result
    End Function

    Protected Function VisibleDetail() As Boolean
        Dim result As Boolean = True
        If spanStatusOrder.InnerText = "Draft" Then
            result = False
            If Session("RoleName") = "PPIC & DE" And Not lblUserId.Text = UCase(Session("UserId")).ToString() Then
                result = True
            End If
        End If
        Return result
    End Function

    Protected Function VisibleEdit() As Boolean
        Dim result As Boolean = False
        If spanStatusOrder.InnerText = "Draft" Then
            result = True
            If Session("RoleName") = "PPIC & DE" And Not lblUserId.Text = UCase(Session("UserId")).ToString() Then
                result = False
            End If
        End If
        Return result
    End Function

    Protected Function TextNext(ItemId As String) As String
        Dim result As String = "Add blind that is doubled to this blind"
        Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id = '" + ItemId + "' AND Active=1")

        Dim bracketType As String = thisData.Tables(0).Rows(0).Item("BracketType").ToString()
        Dim blindNo As String = thisData.Tables(0).Rows(0).Item("BlindNo").ToString()

        If bracketType = "Linked 2 Blinds (Ind)" Or bracketType = "Linked 2 Blinds (Dep)" Then
            result = "Add 2nd blind that is linked to this blind"
        End If

        If bracketType = "Linked 3 Blinds (Ind)" Or bracketType = "Linked 3 Blinds (Dep)" Then
            result = "Add 2nd blind that is linked to this blind"
            If blindNo = "Blind 2" Then
                result = "Add to complete blind"
            End If
        End If

        If bracketType = "Double and Link System Dep" Or bracketType = "Double and Link System Ind" Then 'added 240925
            result = "Add a 2rd blind connected to this blind"
            If blindNo = "Blind 2" Then
                result = "Add a 3rd blind connected to this blind"
            End If
            If blindNo = "Blind 3" Then
                result = "Add to complete blind"
            End If

        End If
        Return result
    End Function

    Protected Function VisibleNext(ItemId As String) As Boolean
        Dim result As Boolean = False

        If spanStatusOrder.InnerText = "Draft" Then
            Dim thisData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id = '" + ItemId + "' AND Active=1")
            Dim designName As String = thisData.Tables(0).Rows(0).Item("DesignName").ToString()
            Dim bracketType As String = thisData.Tables(0).Rows(0).Item("BracketType").ToString()
            Dim uniqueId As String = thisData.Tables(0).Rows(0).Item("UniqueId").ToString()
            Dim blindNo As String = thisData.Tables(0).Rows(0).Item("BlindNo").ToString()

            If designName = "Roller Blinds" Then
                Dim totalBlind As Integer = publicCfg.GetItemData_Integer("SELECT COUNT(*) FROM OrderDetails WHERE UniqueId = '" + uniqueId + "' AND Active=1")
                If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Ind)" Or bracketType = "Linked 2 Blinds (Dep)" Then
                    result = True
                    If totalBlind >= 2 Then : result = False : End If
                End If

                If bracketType = "Linked 3 Blinds (Ind)" Or bracketType = "Linked 3 Blinds (Dep)" Then
                    result = False
                    If blindNo = "Blind 1" And totalBlind < 2 Then
                        result = True
                    End If
                    If blindNo = "Blind 2" And totalBlind < 3 Then
                        result = True
                    End If
                End If

                If bracketType ="Double and Link System Dep" Or bracketType = "Double and Link System Ind" Then 'added 240925
                    result = False
                    If blindNo = "Blind 1" And totalBlind < 2 Then
                        result = True
                    End If
                    If blindNo = "Blind 2" And totalBlind < 3 Then
                        result = True
                    End If
                     If blindNo = "Blind 3" And totalBlind < 4 Then
                        result = True
                    End If
                End If
            End If
            If Session("RoleName") = "PPIC & DE" And Not lblUserId.Text = UCase(Session("UserId")).ToString() Then
                result = False
            End If
        End If
        Return result
    End Function

    Protected Function VisibleCopy() As Boolean
        Dim result As Boolean = False
        If spanStatusOrder.InnerText = "Draft" Then
            result = True
            If Session("RoleName") = "PPIC & DE" And Not lblUserId.Text = UCase(Session("UserId")).ToString() Then
                result = False
            End If
            If Session("RoleName") = "Manager" Or Session("RoleName") = "Account" Then
                result = False
            End If
        End If
        Return result
    End Function

    Protected Function VisibleDelete() As Boolean
        Dim result As Boolean = False
        If spanStatusOrder.InnerText = "Draft" Then
            result = True
            If Session("RoleName") = "PPIC & DE" And Not lblUserId.Text = UCase(Session("UserId")).ToString() Then
                result = False
            End If
            If Session("RoleName") = "Manager" Or Session("RoleName") = "Account" Then
                result = False
            End If
        End If
        Return result
    End Function

    ' PRINT FUNCTION
    Private Sub CreatePDFOrder(HeaderId As String, Action As String)
        Dim result As String = String.Empty

        Dim fileDirectory As String = Server.MapPath("~/file/order/preview")
        If Action = "Mail" Then
            fileDirectory = Server.MapPath("~/file/order/mail")
        End If

        Dim orderNo As String = lblOrderNo.Text.Replace(" ", "")
        Dim fileName As String = Trim("Order" & "_" & orderNo & "-" & lblStoreId.Text & ".pdf")

        Dim printCfg As New PrintConfig

        printCfg.CreatePDFOrder(HeaderId, fileDirectory, fileName)

        If Action = "Preview" Then
            Response.Clear()
            Dim url As String = "/order/preview"
            Session("printPreview") = fileName & "#toolbar=0"

            Dim sb As New StringBuilder()
            sb.Append("<script type = 'text/javascript'>")
            sb.Append("window.open('")
            sb.Append(url)
            sb.Append("');")
            sb.Append("</script>")
            ClientScript.RegisterStartupScript(Me.GetType(), "script", sb.ToString())
        End If

        If Action = "Download" Then
            Response.Clear()

            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(fileDirectory + "/" + fileName)))
            Response.WriteFile(fileDirectory + "/" + fileName)
            Response.Flush()
            Response.SuppressContent = True
            ApplicationInstance.CompleteRequest()
        End If
    End Sub

    '#function create job sheet
    Private Sub CreateJobSheet(HeaderId As String, Action As String)
        Dim result As String = String.Empty
        Dim fileDirectory As String = Server.MapPath("~/file/order/job")

        Dim orderNo As String = lblOrderNo.Text.Replace(" ", "")
        Dim fileName As String = Trim("JobOrder" & "_" & orderNo & "-" & lblStoreId.Text & ".pdf")
        Dim jobConf As New JobConfig

        jobConf.CreateJob(HeaderId, fileDirectory, fileName)

        If Action = "Reprint" Then
            Response.Clear()
            Dim url As String = "/order/jobsheets"
            Session("Reprint") = fileName & "#toolbar=0"

            Dim sb As New StringBuilder()
            sb.Append("<script type = 'text/javascript'>")
            sb.Append("window.open('")
            sb.Append(url)
            sb.Append("');")
            sb.Append("</script>")
            ClientScript.RegisterStartupScript(Me.GetType(), "script", sb.ToString())
        End If

        If Action = "Download" Then
            Response.Clear()

            Response.ContentType = ContentType
            Response.AppendHeader("Content-Disposition", ("attachment; filename=" + Path.GetFileName(fileDirectory + "/" + fileName)))
            Response.WriteFile(fileDirectory + "/" + fileName)
            Response.Flush()
            Response.SuppressContent = True
            ApplicationInstance.CompleteRequest()
        End If

    End Sub



    Protected Function GenerateUniqueId() As String
        Dim result As String = String.Empty

        Dim alphabets As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        Dim small_alphabets As String = "abcdefghijklmnopqrstuvwxyz"
        Dim numbers As String = "1234567890"

        Dim characters As String = Convert.ToString(alphabets & small_alphabets) & numbers
        Dim length As Integer = Integer.Parse(20)
        Dim uniqueId As String = String.Empty
        For i As Integer = 0 To length - 1
            Dim character As String = String.Empty
            Do
                Dim index As Integer = New Random().Next(0, characters.Length)
                character = characters.ToCharArray()(index).ToString()
            Loop While uniqueId.IndexOf(character) <> -1
            uniqueId += character
        Next
        result = uniqueId

        Return result
    End Function



    Protected Function PermissionAction()
        ' If Session("RoleName") = "Manager" Or Session("RoleName") = "Account" Then
        '     btnAdd.Visible = False
        ' End If
    End Function
End Class




