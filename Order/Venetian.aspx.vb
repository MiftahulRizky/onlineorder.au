Imports System.Data

Partial Class Order_Venetian
    Inherits Page

    Dim publicCfg As New PublicConfig

    Dim designId As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("designId") = "" Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        If Session("headerId") = "" Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        If Session("itemAction") = "" Then
            Response.Redirect("~/order/detail", False)
            Exit Sub
        End If

        designId = Session("designId")
        If publicCfg.GetDesignActive(designId) = False Then
            Response.Redirect("~/order/maintenance", False)
            Exit Sub
        End If

        lblHeaderId.Text = Session("headerId") : lblItemId.Text = ""
        lblOrderNo.Text = publicCfg.GetOrderNo(lblHeaderId.Text)
        lblOrderCust.Text = publicCfg.GetOrderCust(lblHeaderId.Text)

        pageAction.InnerHtml = Session("itemAction")
        pageTitle.InnerHtml = publicCfg.GetDesignName(designId)

        If Session("itemAction") = "AddItem" Then
            lblHeaderId.Text = Session("HeaderId")
            btnSubmit.Visible = True : btnSubmit.Text = "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Add Item)"
            cardTitle.InnerHtml = "Add Item"
            If Not IsPostBack Then
                txtQty.Text = "1"
                Call BackColor()

                Call BindDataBlind()
                Call BindDataStyle(ddlBlindType.SelectedValue)
                Call BindDataColour(ddlBlindType.SelectedValue, ddlStyle.SelectedValue)

                Call BindComponentForm(ddlColour.SelectedValue)
            End If
        End If

        If Session("itemAction") = "ViewItem" Then
            lblHeaderId.Text = Session("HeaderId")
            lblItemId.Text = Session("itemId")
            btnSubmit.Visible = False : btnSubmit.Text = "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Update Item)"
            cardTitle.InnerHtml = "VIEW ITEM ID : " & lblItemId.Text
            If Session("RoleName") = "Administrator" Then : btnSubmit.Visible = True : End If
            If Not IsPostBack Then
                Call BindItemOrder(lblItemId.Text)
            End If
        End If

        If Session("itemAction") = "EditItem" Then
            lblHeaderId.Text = Session("HeaderId")
            lblItemId.Text = Session("itemId")
            btnSubmit.Visible = True : btnSubmit.Text = "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Update Item)"
            If Session("RoleName") = "Manager" Or Session("RoleName") = "Account" Then
                btnSubmit.Visible = False 
            End If
            cardTitle.InnerHtml = "EDIT ITEM ID : " & lblItemId.Text
            If Not IsPostBack Then
                Call BindItemOrder(lblItemId.Text)
            End If
        End If
    End Sub

    Protected Sub ddlBlindType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Dim style As String = ddlStyle.SelectedValue
        Call BindDataStyle(ddlBlindType.SelectedValue)
        Try
            ddlStyle.SelectedValue = style
        Catch ex As Exception

        End Try
        Call BindDataColour(ddlBlindType.SelectedValue, ddlStyle.SelectedValue)

        Call BindComponentForm(ddlColour.SelectedValue)
    End Sub

    Protected Sub ddlStyle_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindDataColour(ddlBlindType.SelectedValue, ddlStyle.SelectedValue)

        Call BindComponentForm(ddlColour.SelectedValue)
    End Sub

    Protected Sub ddlColour_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindComponentForm(ddlColour.SelectedValue)
    End Sub

    Protected Sub ddlPelmetType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindComponentForm(ddlColour.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlBlindType.SelectedValue = "" Then
                Call MessageError(True, "VENETIAN TYPE IS REQUIRED !")
                ddlBlindType.CssClass = "form-select  is-invalid"
                ddlBlindType.Focus()
                Exit Sub
            End If

            If ddlStyle.SelectedValue = "" Then
                Call MessageError(True, "STYLE TYPE IS REQUIRED !")
                ddlStyle.CssClass = "form-select  is-invalid"
                ddlStyle.Focus()
                Exit Sub
            End If

            If ddlColour.SelectedValue = "" Then
                Call MessageError(True, "VENETIAN COLOUR IS REQUIRED !")
                ddlColour.CssClass = "form-select  is-invalid"
                ddlColour.Focus()
                Exit Sub
            End If

            If txtQty.Text = "" Or txtQty.Text = "0" Then
                Call MessageError(True, "QUANTITY IS REQUIRED !")
                txtQty.CssClass = "form-control  is-invalid"
                txtQty.Focus()
                Exit Sub
            End If

            If Not txtQty.Text = "" Then
                If Not IsNumeric(txtQty.Text) Then
                    Call MessageError(True, "QUANTITY SHOULD BE NUMERIC !")
                    txtQty.CssClass = "form-control  is-invalid"
                    txtQty.Focus()
                    Exit Sub
                End If

                If txtQty.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR QUANTITY ORDER !")
                    txtQty.CssClass = "form-control  is-invalid"
                    txtQty.Focus()
                    Exit Sub
                End If
            End If

            If Not txtLocation.Text = "" Then
                If InStr(txtLocation.Text, "&") > 0 Then
                    Call MessageError(True, "CHARACTER [&] IS NO RECOMMENDED !")
                    txtLocation.CssClass = "form-control  is-invalid"
                    txtLocation.Focus()
                    Exit Sub
                End If
            End If

            If ddlMounting.SelectedValue = "" Then
                Call MessageError(True, "MOUNTING IS REQUIRED !")
                ddlMounting.CssClass = "form-control  is-invalid"
                ddlMounting.Focus()
                Exit Sub
            End If

            If txtWidth.Text = "" Or txtWidth.Text = "0" Then
                Call MessageError(True, "WIDTH IS REQUIRED !")
                txtWidth.CssClass = "form-control  is-invalid"
                txtWidth.Focus()
                Exit Sub
            End If

            If Not txtWidth.Text = "" Then
                If Not IsNumeric(txtWidth.Text) Then
                    Call MessageError(True, "WIDTH SHOULD BE NUMERIC !")
                    txtWidth.CssClass = "form-control  is-invalid"
                    txtWidth.Focus()
                    Exit Sub
                End If

                If CInt(txtWidth.Text) < 150 Then
                    Call MessageError(True, "MINIUMUM WIDTH IS 150mm !")
                    txtWidth.CssClass = "form-control  is-invalid"
                    txtWidth.Focus()
                    Exit Sub
                End If

                If CInt(txtWidth.Text) > 3000 Then
                    Call MessageError(True, "MAXIMUM WIDTH IS 3000mm !")
                    txtWidth.CssClass = "form-control  is-invalid"
                    txtWidth.Focus()
                    Exit Sub
                End If
            End If

            If txtDrop.Text = "" Or txtDrop.Text = "0" Then
                Call MessageError(True, "BLIND DROP IS REQUIRED !")
                txtDrop.CssClass = "form-control  is-invalid"
                txtDrop.Focus()
                Exit Sub
            End If

            If Not txtDrop.Text = "" Then
                If Not IsNumeric(txtDrop.Text) Then
                    Call MessageError(True, "BLIND DROP SHOULD BE NUMERIC !")
                    txtDrop.CssClass = "form-control  is-invalid"
                    txtDrop.Focus()
                    Exit Sub
                End If

                If CInt(txtDrop.Text) < 150 Then
                    Call MessageError(True, "MINIMUM BLIND DROP IS 150mm !")
                    txtDrop.CssClass = "form-control  is-invalid"
                    txtDrop.Focus()
                    Exit Sub
                End If

                If CInt(txtDrop.Text) > 3200 Then
                    Call MessageError(True, "MAXIMUM BLIND DROP IS 3200mm !")
                    txtDrop.CssClass = "form-control  is-invalid"
                    txtDrop.Focus()
                    Exit Sub
                End If
            End If

            If txtWidth.Text * txtDrop.Text / 1000000 > 4 Then
                Call MessageError(True, "MAXIMUM SIZE FOR A TIMMBER STYLE VENETIAN IS 4 SQM !")
                txtDrop.CssClass = "form-control  is-invalid"
                txtWidth.CssClass = "form-control  is-invalid"
                txtWidth.Focus() : txtDrop.Focus()
                Exit Sub
            End If

            If ddlControl.SelectedValue = "" Then
                Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                ddlControl.CssClass = "form-select  is-invalid"
                ddlControl.Focus()
                Exit Sub
            End If

            If Not txtControlLength.Text = "" Then
                If Not IsNumeric(txtControlLength.Text) Then
                    Call MessageError(True, "CONTROL LENGTH SHOULD BE NUMERIC !")
                    txtControlLength.CssClass = "form-control  is-invalid"
                    txtControlLength.Focus()
                    Exit Sub
                End If

                If txtControlLength.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR CONTROL LENGTH ORDER !")
                    txtControlLength.CssClass = "form-control  is-invalid"
                    txtControlLength.Focus()
                    Exit Sub
                End If
            End If

            ' If ddlPelmetSize.SelectedValue = "" Then
            '     Call MessageError(True, "PELMET SIZE IS REQUIRED !")
            '     ddlPelmetSize.CssClass = "form-select  is-invalid"
            '     ddlPelmetSize.Focus()
            '     Exit Sub
            ' End If

            If Not txtPelmetWidth.Text = "" Then
                If Not IsNumeric(txtPelmetWidth.Text) Then
                    Call MessageError(True, "PELMET WIDTH SHOULD BE NUMERIC !")
                    txtPelmetWidth.CssClass = "form-control  is-invalid"
                    txtPelmetWidth.Focus()
                    Exit Sub
                End If

                If txtPelmetWidth.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR PELMET WIDTH ORDER !")
                    txtPelmetWidth.CssClass = "form-control  is-invalid"
                    txtPelmetWidth.Focus()
                    Exit Sub
                End If
            End If

            If Not txtReturnLeft.Text = "" Then
                If Not IsNumeric(txtReturnLeft.Text) Then
                    Call MessageError(True, "LEFT HAND RETURN SHOULD BE NUMERIC !")
                    txtReturnLeft.CssClass = "form-control  is-invalid"
                    txtReturnLeft.Focus()
                    Exit Sub
                End If

                If txtReturnLeft.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR LEFT HAND RETURN ORDER !")
                    txtReturnLeft.CssClass = "form-control  is-invalid"
                    txtReturnLeft.Focus()
                    Exit Sub
                End If
            End If

            If Not txtReturnRight.Text = "" Then
                If Not IsNumeric(txtReturnRight.Text) Then
                    Call MessageError(True, "RIGHT HAND RETURN SHOULD BE NUMERIC !")
                    txtReturnRight.CssClass = "form-control  is-invalid"
                    txtReturnRight.Focus()
                    Exit Sub
                End If

                If txtReturnRight.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR RIGHT HAND RETURN ORDER !")
                    txtReturnRight.CssClass = "form-control  is-invalid"
                    txtReturnRight.Focus()
                    Exit Sub
                End If
            End If

            If Not txtTopLHSWidth.Text = "" Then
                If Not IsNumeric(txtTopLHSWidth.Text) Then
                    Call MessageError(True, "TOP LHS WIDTH SHOULD BE NUMERIC !")
                    txtTopLHSWidth.CssClass = "form-control  is-invalid"
                    txtTopLHSWidth.Focus()
                    Exit Sub
                End If

                If txtTopLHSWidth.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR TOP LHS WIDTH ORDER !")
                    txtTopLHSWidth.CssClass = "form-control  is-invalid"
                    txtTopLHSWidth.Focus()
                    Exit Sub
                End If
            End If
            If Not txtTopLHSHeigth.Text = "" Then
                If Not IsNumeric(txtTopLHSHeigth.Text) Then
                    Call MessageError(True, "TOP LHS HEIGHT SHOULD BE NUMERIC !")
                    txtTopLHSHeigth.CssClass = "form-control  is-invalid"
                    txtTopLHSHeigth.Focus()
                    Exit Sub
                End If

                If txtTopLHSHeigth.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR TOP LHS HEIGHT ORDER !")
                    txtTopLHSHeigth.CssClass = "form-control  is-invalid"
                    txtTopLHSHeigth.Focus()
                    Exit Sub
                End If
            End If

            If Not txtTopRHSWidth.Text = "" Then
                If Not IsNumeric(txtTopRHSWidth.Text) Then
                    Call MessageError(True, "TOP RHS WIDTH SHOULD BE NUMERIC !")
                    txtTopRHSWidth.CssClass = "form-control  is-invalid"
                    txtTopRHSWidth.Focus()
                    Exit Sub
                End If

                If txtTopRHSWidth.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR TOP RHS WIDTH ORDER !")
                    txtTopRHSWidth.CssClass = "form-control  is-invalid"
                    txtTopRHSWidth.Focus()
                    Exit Sub
                End If
            End If
            If Not txtTopRHSHeigth.Text = "" Then
                If Not IsNumeric(txtTopRHSHeigth.Text) Then  
                    Call MessageError(True, "TOP RHS HEIGHT SHOULD BE NUMERIC !")
                    txtTopRHSHeigth.CssClass = "form-control  is-invalid"
                    txtTopRHSHeigth.Focus()
                    Exit Sub
                End If

                If txtTopRHSHeigth.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR TOP RHS HEIGHT ORDER !")
                    txtTopRHSHeigth.CssClass = "form-control  is-invalid"
                    txtTopRHSHeigth.Focus()
                    Exit Sub
                End If
            End If

            If Not txtBottomLHSWidth.Text = "" Then
                If Not IsNumeric(txtBottomLHSWidth.Text) Then
                    Call MessageError(True, "BOTTOM LHS WIDTH SHOULD BE NUMERIC !")
                    txtBottomLHSWidth.CssClass = "form-control  is-invalid"
                    txtBottomLHSWidth.Focus()
                    Exit Sub
                End If
                If txtBottomLHSWidth.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR BOTTOM LHS WIDTH ORDER !")
                    txtBottomLHSWidth.CssClass = "form-control  is-invalid"
                    txtBottomLHSWidth.Focus()
                    Exit Sub
                End If
            End If
            If Not txtBottomLHSHeigth.Text = "" Then
                If Not IsNumeric(txtBottomLHSHeigth.Text) Then
                    Call MessageError(True, "BOTTOM LHS HEIGHT SHOULD BE NUMERIC !")
                    txtBottomLHSHeigth.CssClass = "form-control  is-invalid"
                    txtBottomLHSHeigth.Focus()
                    Exit Sub
                End If

                If txtBottomLHSHeigth.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR BOTTOM LHS HEIGHT ORDER !")
                    txtBottomLHSHeigth.CssClass = "form-control  is-invalid"
                    txtBottomLHSHeigth.Focus()
                    Exit Sub
                End If
            End If

            If Not txtBottomRHSWidth.Text = "" Then
                If Not IsNumeric(txtBottomRHSWidth.Text) Then
                    Call MessageError(True, "BOTTOM RHS WIDTH SHOULD BE NUMERIC !")
                    txtBottomRHSWidth.CssClass = "form-control  is-invalid"
                    txtBottomRHSWidth.Focus()
                    Exit Sub
                End If

                If txtBottomRHSWidth.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR BOTTOM RHS WIDTH ORDER !")
                    txtBottomRHSWidth.CssClass = "form-control  is-invalid"
                    txtBottomRHSWidth.Focus()
                    Exit Sub
                End If
            End If
            If Not txtBottomRHSHeigth.Text = "" Then
                If Not IsNumeric(txtBottomRHSHeigth.Text) Then
                    Call MessageError(True, "BOTTOM RHS HEIGHT SHOULD BE NUMERIC !")
                    txtBottomRHSHeigth.CssClass = "form-control  is-invalid"
                    txtBottomRHSHeigth.Focus()
                    Exit Sub
                End If

                If txtBottomRHSHeigth.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR BOTTOM RHS HEIGHT ORDER !")
                    txtBottomRHSHeigth.CssClass = "form-control  is-invalid"
                    txtBottomRHSHeigth.Focus()
                    Exit Sub
                End If
            End If

            If Not txtMarkUp.Text = "" Then
                If Not IsNumeric(txtMarkUp.Text) Then
                    Call MessageError(True, "MARK UP SHOULD BE NUMERIC !")
                    txtMarkUp.CssClass = "form-control  is-invalid"
                    txtMarkUp.Focus()
                    Exit Sub
                End If

                If txtMarkUp.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR MARK UP ORDER !")
                    txtMarkUp.CssClass = "form-control  is-invalid"
                    txtMarkUp.Focus()
                    Exit Sub
                End If
            End If

            If msgError.InnerText = "" Then
                Dim userId As String = UCase(Session("UserId")).ToString()

                If txtMarkUp.Text = "" Then : txtMarkUp.Text = "0" : End If
                If txtControlLength.Text = "" Or txtControlLength.Text = "0" Then
                    txtControlLength.Text = txtWidth.Text - 50
                End If

                lblKitId.Text = UCase(ddlColour.SelectedValue).ToString()
                lblSoeKitId.Text = publicCfg.GetSoeKitId(ddlColour.SelectedValue)

                Dim priceGroupName As String = publicCfg.GetBlindName(ddlBlindType.SelectedValue)
                Dim priceGroupId As String = publicCfg.GetPriceGroupId(designId, priceGroupName)
                lblPriceGroupId.Text = UCase(priceGroupId).ToString()

                If txtTopLHSWidth.Text = "" Then : txtTopLHSWidth.Text = 0 : End If
                If txtTopLHSHeigth.Text = "" Then : txtTopLHSHeigth.Text = 0 : End If
                If txtTopRHSWidth.Text = "" Then : txtTopRHSWidth.Text = 0 : End If
                If txtTopRHSHeigth.Text = "" Then : txtTopRHSHeigth.Text = 0 : End If

                If txtBottomLHSWidth.Text = "" Then : txtBottomLHSWidth.Text = 0 : End If
                If txtBottomLHSHeigth.Text = "" Then : txtBottomLHSHeigth.Text = 0 : End If
                If txtBottomRHSWidth.Text = "" Then : txtBottomRHSWidth.Text = 0 : End If
                If txtBottomRHSHeigth.Text = "" Then : txtBottomRHSHeigth.Text = 0 : End If

                lblCutOut_LeftTop.Text = 0
                lblCutOut_RightTop.Text = 0

                lblCutOut_LeftBottom.Text = 0
                lblCutOut_RightBottom.Text = 0
                If txtTopLHSWidth.Text > 0 Or txtTopLHSHeigth.Text > 0 Then
                    lblCutOut_LeftTop.Text = 1
                End If
                If txtTopRHSWidth.Text > 0 Or txtTopRHSHeigth.Text > 0 Then
                    lblCutOut_RightTop.Text = 1
                End If

                If txtBottomLHSWidth.Text > 0 Or txtBottomLHSHeigth.Text > 0 Then
                    lblCutOut_LeftBottom.Text = 1
                End If
                If txtBottomRHSWidth.Text > 0 Or txtBottomRHSHeigth.Text > 0 Then
                    lblCutOut_RightBottom.Text = 1
                End If

                If txtPelmetWidth.Text = "" Or txtPelmetWidth.Text = "0" Then
                    If ddlMounting.SelectedValue = "Face Fit" Then
                        txtPelmetWidth.Text = txtWidth.Text + 20
                    End If
                    If ddlMounting.SelectedValue = "Reveal Fit" Then
                        txtPelmetWidth.Text = txtWidth.Text + 7
                    End If
                End If

                If ddlPelmetType.SelectedValue = "No Return" Then
                    txtReturnLeft.Text = "" : txtReturnRight.Text = ""
                End If
                If ddlPelmetType.SelectedValue = "With Return" Then
                    txtReturnLeft.Text = "67" : txtReturnRight.Text = "67"
                End If
                If ddlPelmetType.SelectedValue = "Single Left Return" Then
                    txtReturnLeft.Text = "67" : txtReturnRight.Text = ""
                End If
                If ddlPelmetType.SelectedValue = "Single Right Return" Then
                    txtReturnLeft.Text = "" : txtReturnRight.Text = "67"
                End If

                If Session("itemAction") = "AddItem" Or Session("itemAction") = "CopyItem" Then
                    lblItemId.Text = publicCfg.CreateOrderItemId()
                    sdsPage.Insert()
                    publicCfg.InsertActivity(userId, Page.Title, "INSERT ORDER DETAIL. ITEM ID : " & lblItemId.Text)
                    Call SetPricing(lblItemId.Text, lblHeaderId.Text)
                    Call myCancel()
                End If

                If Session("itemAction") = "EditItem" Or Session("itemAction") = "ViewItem" Then
                    sdsPage.Update()
                    publicCfg.InsertActivity(userId, Page.Title, "UPDATE ORDER DETAIL. ITEM ID : " & lblItemId.Text)
                    Call SetPricing(lblItemId.Text, lblHeaderId.Text)
                    Call myCancel()
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub SetPricing(itemId As String, headerId As String)
        publicCfg.ResetPriceDetail(itemId)
        publicCfg.HitungHarga(headerId, itemId)
        publicCfg.HitungSurcharge(headerId, itemId)
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Call myCancel()
    End Sub

    Private Sub BindItemOrder(ItemId As String)
        Call BackColor()
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id = '" + ItemId + "'")

            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/order/detail", False)
                Exit Sub
            End If

            Dim kitId As String = myData.Tables(0).Rows(0).Item("KitId").ToString()
            Dim blindId As String = myData.Tables(0).Rows(0).Item("BlindId").ToString()
            Dim style As String = myData.Tables(0).Rows(0).Item("ControlType").ToString()

            Call BindDataBlind()
            Call BindDataStyle(blindId)
            Call BindDataColour(blindId, style)

            ddlBlindType.SelectedValue = blindId : ddlBlindType.Enabled = False
            ddlStyle.SelectedValue = style
            ddlColour.SelectedValue = kitId

            txtQty.Text = myData.Tables(0).Rows(0).Item("Qty").ToString()
            txtLocation.Text = myData.Tables(0).Rows(0).Item("Location").ToString()
            ddlMounting.SelectedValue = myData.Tables(0).Rows(0).Item("Mounting").ToString()
            txtWidth.Text = myData.Tables(0).Rows(0).Item("Width").ToString()
            txtDrop.Text = myData.Tables(0).Rows(0).Item("Drop").ToString()
            ddlControl.SelectedValue = myData.Tables(0).Rows(0).Item("ControlPosition").ToString()
            txtControlLength.Text = myData.Tables(0).Rows(0).Item("ControlLength").ToString()
            ddlHoldDown.SelectedValue = myData.Tables(0).Rows(0).Item("BottomHoldDown").ToString()
            ddlPelmetType.SelectedValue = myData.Tables(0).Rows(0).Item("PelmetType").ToString()
            txtPelmetWidth.Text = myData.Tables(0).Rows(0).Item("PelmetWidth").ToString()
            ddlPelmetSize.SelectedValue = myData.Tables(0).Rows(0).Item("PelmetSize").ToString()
            txtReturnLeft.Text = myData.Tables(0).Rows(0).Item("PelmetReturnSize").ToString()
            txtReturnRight.Text = myData.Tables(0).Rows(0).Item("PelmetReturnSize2").ToString()
            txtTopLHSWidth.Text = myData.Tables(0).Rows(0).Item("LHSWidth_Top").ToString()
            txtTopLHSHeigth.Text = myData.Tables(0).Rows(0).Item("LHSHeight_Top").ToString()
            txtTopRHSWidth.Text = myData.Tables(0).Rows(0).Item("RHSWidth_Top").ToString()
            txtTopRHSHeigth.Text = myData.Tables(0).Rows(0).Item("RHSHeight_Top").ToString()
            txtBottomLHSWidth.Text = myData.Tables(0).Rows(0).Item("LHSWidth_Bottom").ToString()
            txtBottomLHSHeigth.Text = myData.Tables(0).Rows(0).Item("LHSHeight_Bottom").ToString()
            txtBottomRHSWidth.Text = myData.Tables(0).Rows(0).Item("RHSWidth_Bottom").ToString()
            txtBottomRHSHeigth.Text = myData.Tables(0).Rows(0).Item("RHSHeight_Bottom").ToString()
            txtNotes.Text = myData.Tables(0).Rows(0).Item("Notes").ToString()
            txtMarkUp.Text = myData.Tables(0).Rows(0).Item("MarkUp").ToString()

            If txtTopLHSWidth.Text = "0" Then : txtTopLHSWidth.Text = "" : End If
            If txtTopLHSHeigth.Text = "0" Then : txtTopLHSHeigth.Text = "" : End If
            If txtTopRHSWidth.Text = "0" Then : txtTopRHSWidth.Text = "" : End If
            If txtTopRHSHeigth.Text = "0" Then : txtTopRHSHeigth.Text = "" : End If
            If txtBottomLHSWidth.Text = "0" Then : txtBottomLHSWidth.Text = "" : End If
            If txtBottomLHSHeigth.Text = "0" Then : txtBottomLHSHeigth.Text = "" : End If
            If txtBottomRHSWidth.Text = "0" Then : txtBottomRHSWidth.Text = "" : End If
            If txtBottomRHSHeigth.Text = "0" Then : txtBottomRHSHeigth.Text = "" : End If
            If txtMarkUp.Text = "0" Then : txtMarkUp.Text = "" : End If

            Call BindComponentForm(ddlColour.SelectedValue)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindItemOrder", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindComponentForm(Data As String)
        Try
            divDetail.Visible = False
            divStyle.Visible = False

            Dim blindName As String = String.Empty
            If Not ddlBlindType.SelectedValue = "" Then
                blindName = publicCfg.GetBlindName(ddlBlindType.SelectedValue)
                If blindName = "50mm Timberstyle" Or blindName = "63mm Timberstyle" Then
                    divStyle.Visible = True
                End If
            End If
            If Not Data = "" Then
                divDetail.Visible = True
                divReturnLength.Visible = False

                divMarkUp.Visible = False
                If Session("MarkUpAccess") = True Then : divMarkUp.Visible = True : End If

                If ddlPelmetType.SelectedValue = "With Return" Then
                    divReturnLength.Visible = True
                    divReturnLeft.Visible = True : divReturnRight.Visible = True
                End If
                If ddlPelmetType.SelectedValue = "Single Left Return" Then
                    divReturnLength.Visible = True
                    divReturnLeft.Visible = True : divReturnRight.Visible = False
                End If
                If ddlPelmetType.SelectedValue = "Single Right Return" Then
                    divReturnLength.Visible = True
                    divReturnLeft.Visible = False : divReturnRight.Visible = True
                End If

                blindName = publicCfg.GetBlindName(ddlBlindType.SelectedValue)
                Dim result As String = "<h3>" & UCase(blindName).ToString() & "</h3>"
                result += "<br />"
                result += "- Minimum width available is 180mm"
                result += "<br />"
                result += "- Width between 180mm & 260mm -> Tilter only in the middle and surcharge of $25"
                result += "<br />"
                result += "<br />"
                result += "- Width between 261mm & 399mm -> Tilter and Cord lock are on opposite ends - Surcharge of $25 applies"

                pNotes.InnerHtml = result
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindComponentForm", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataBlind()
        ddlBlindType.Items.Clear()
        Try
            Dim myQuery As String = "SELECT UPPER(Name) AS NameText, * FROM Blinds WHERE DesignId='" + designId + "' AND Active=1 ORDER BY Name ASC"
            ddlBlindType.DataSource = publicCfg.GetListData(myQuery)
            ddlBlindType.DataTextField = "NameText"
            ddlBlindType.DataValueField = "Id"
            ddlBlindType.DataBind()
            If ddlBlindType.Items.Count > 1 Then
                ddlBlindType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataBlind", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataStyle(BlindId As String)
        ddlStyle.Items.Clear()
        Try
            If Not BlindId = "" Then
                Dim myQuery As String = "SELECT ControlType AS ControlValue, UPPER(ControlType) AS ControlText FROM HardwareKits WHERE DesignId='" + designId + "' AND BlindId='" + BlindId + "' AND Active=1 GROUP BY ControlType ORDER BY ControlType ASC"
                ddlStyle.DataSource = publicCfg.GetListData(myQuery)
                ddlStyle.DataTextField = "ControlText"
                ddlStyle.DataValueField = "ControlValue"
                ddlStyle.DataBind()
                If ddlStyle.Items.Count > 1 Then
                    ddlStyle.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataStyle", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataColour(BlindId As String, Style As String)
        ddlColour.Items.Clear()
        Try
            If Not BlindId = "" And Not Style = "" Then
                Dim myQuery As String = "SELECT Id, UPPER(ColourType) AS ColourText FROM HardwareKits WHERE DesignId='" + designId + "' AND BlindId='" + BlindId + "' AND ControlType = '" + Style + "' AND Active=1 ORDER BY ColourType ASC"
                ddlColour.DataSource = publicCfg.GetListData(myQuery)
                ddlColour.DataTextField = "ColourText"
                ddlColour.DataValueField = "Id"
                ddlColour.DataBind()
                If ddlColour.Items.Count > 1 Then
                    ddlColour.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataColour", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlBlindType.CssClass = "form-select "
        ddlStyle.CssClass = "form-select "
        ddlColour.CssClass = "form-select "
        txtQty.CssClass = "form-control "
        txtLocation.CssClass = "form-control "
        ddlMounting.CssClass = "form-select "
        txtWidth.CssClass = "form-control "
        txtDrop.CssClass = "form-control "
        ddlControl.CssClass = "form-select "
        txtControlLength.CssClass = "form-control "
        ddlHoldDown.CssClass = "form-select "
        ddlPelmetType.CssClass = "form-select "
        txtPelmetWidth.CssClass = "form-control "
        ddlPelmetSize.CssClass = "form-select "
        txtReturnLeft.CssClass = "form-control "
        txtReturnRight.CssClass = "form-control "
        txtTopLHSWidth.CssClass = "form-control "
        txtTopLHSHeigth.CssClass = "form-control "
        txtTopRHSWidth.CssClass = "form-control "
        txtTopRHSHeigth.CssClass = "form-control "
        txtBottomLHSWidth.CssClass = "form-control "
        txtBottomLHSHeigth.CssClass = "form-control "
        txtBottomRHSWidth.CssClass = "form-control "
        txtBottomRHSHeigth.CssClass = "form-control "
        txtNotes.CssClass = "form-control"
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        ' If Show = True Then : divError.Visible = True : End If
        If Show = True Then
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub

    Private Sub myCancel()
        Session("headerId") = lblHeaderId.Text
        Response.Redirect("~/order/detail", False)
    End Sub
End Class
