Imports System.Data

Partial Class Order_Vertical
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
            lblBlindNo.Text = "Blind 1"
            btnSubmit.Visible = True : btnSubmit.Text = "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Add Item)"
            cardTitle.InnerHtml = "Add Item"
            If Not IsPostBack Then
                txtQty.Text = "1"
                Call BackColor()

                Call BindBlind()
                Call BindVerticalStyle(ddlBlindType.SelectedValue)
                Call BindControlType(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)

                Call BindFabricType()
                Call BindFabricLength(ddlFabricType.SelectedValue)
                Call BindFabricColour(ddlFabricType.SelectedValue, ddlFabricLength.SelectedValue)

                Call BindTrackColour(ddlControlType.SelectedValue)
                Call BindControlPosition(ddlControlType.SelectedValue)

                Call BindHanger(ddlBlindType.SelectedValue)

                Call BindComponenForm(ddlControlType.SelectedValue)
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

        Dim tubeType As String = ddlTubeType.SelectedValue
        Call BindVerticalStyle(ddlBlindType.SelectedValue)
        Try
            If Not tubeType = "" Then
                ddlTubeType.SelectedValue = tubeType
            End If
        Catch ex As Exception
        End Try

        Call BindControlType(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)

        Call BindTrackColour(ddlControlType.SelectedValue)
        Call BindControlPosition(ddlControlType.SelectedValue)

        Call BindHanger(ddlBlindType.SelectedValue)

        Call BindComponenForm(ddlControlType.SelectedValue)
    End Sub

    Protected Sub ddlTubeType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindControlType(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)

        Call BindTrackColour(ddlControlType.SelectedValue)
        Call BindControlPosition(ddlControlType.SelectedValue)

        Call BindHanger(ddlBlindType.SelectedValue)

        Call BindComponenForm(ddlControlType.SelectedValue)
    End Sub

    Protected Sub ddlControlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindTrackColour(ddlControlType.SelectedValue)

        Call BindControlPosition(ddlControlType.SelectedValue)
        Call BindControlPosition(ddlControlType.SelectedValue)
        Call BindWandLength(String.Empty)
        Call BindHanger(ddlBlindType.SelectedValue)

        Call BindComponenForm(ddlControlType.SelectedValue)
    End Sub

    Protected Sub ddlFabricType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindFabricLength(ddlFabricType.SelectedValue)
        Call BindFabricColour(ddlFabricType.SelectedValue, ddlFabricLength.SelectedValue)
    End Sub

    Protected Sub ddlFabricLength_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        Call BindFabricColour(ddlFabricType.SelectedValue, ddlFabricLength.SelectedValue)
    End Sub

    Protected Sub ddlWandColour_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        If ddlWandLength.SelectedValue = "custom" Then
           divWandCustomLength.Visible = True
           divBtnInfoCustom.Visible = True
        Else
           divWandCustomLength.Visible = False
           divBtnInfoCustom.Visible = False
        End If
    End Sub

    Protected Sub ddlWandLength_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        Call BindWandColour(ddlWandLength.SelectedValue)
        If ddlWandLength.SelectedValue = "custom" Then
           divWandCustomLength.Visible = True
           divBtnInfoCustom.Visible = True
        Else
           divWandCustomLength.Visible = False
           divBtnInfoCustom.Visible = False
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlBlindType.SelectedValue = "" Then
                Call MessageError(True, "VERTICAL TYPE IS REQUIRED !")
                ddlBlindType.CssClass = "form-select  is-invalid"
                ddlBlindType.Focus()
                Exit Sub
            End If

            If ddlTubeType.SelectedValue = "" Then
                Call MessageError(True, "VERTICAL STYLE IS REQUIRED !")
                ddlTubeType.CssClass = "form-select  is-invalid"
                ddlTubeType.Focus()
                Exit Sub
            End If

            If ddlControlType.SelectedValue = "" Then
                Call MessageError(True, "CONTROL TYPE IS REQUIRED !")
                ddlControlType.CssClass = "form-select  is-invalid"
                ddlControlType.Focus()
                Exit Sub
            End If
           

            Dim blindName As String = publicCfg.GetBlindName(ddlBlindType.SelectedValue)
            Dim tubeType As String = ddlTubeType.SelectedValue
            Dim controlType As String = publicCfg.GetControlType(ddlControlType.SelectedValue)

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

            If Not blindName = "Slat Only" And ddlMounting.SelectedValue = "" Then
                Call MessageError(True, "MOUNTING IS REQUIRED !")
                ddlMounting.CssClass = "form-select  is-invalid"
                ddlMounting.Focus()
                Exit Sub
            End If

            If Not txtLocation.Text = "" Then
                If InStr(txtLocation.Text, "&") > 0 Then
                    Call MessageError(True, "CHARACTER [&] IS NO RECOMMENDED !")
                    txtLocation.CssClass = "form-control  is-invalid"
                    txtLocation.Focus()
                    Exit Sub
                End If
            End If

            If blindName = "Slat Only" Then
                ' If txtSlatQty.Text = "" Or txtSlatQty.Text = "0" Then
                '     Call MessageError(True, "SLAT QTY IS REQUIRED !")
                '     txtSlatQty.CssClass = "form-control  is-invalid"
                '     txtSlatQty.Focus()
                '     Exit Sub
                ' End If

                If Not txtSlatQty.Text = "" Then
                    If Not IsNumeric(txtSlatQty.Text) Then
                        Call MessageError(True, "SLAT QTY SHOULD BE NUMERIC !")
                        txtSlatQty.CssClass = "form-control  is-invalid"
                        txtSlatQty.Focus()
                        Exit Sub
                    End If

                    ' If txtSlatQty.Text < 1 Then
                    '     Call MessageError(True, "PLEASE CHECK YOUR SLAT QTY ORDER !")
                    '     txtSlatQty.CssClass = "form-control  is-invalid"
                    '     txtSlatQty.Focus()
                    '     Exit Sub
                    ' End If
                End If
            End If

            If Not blindName = "Track Only" Then
                If ddlFabricType.SelectedValue = "" Then
                    Call MessageError(True, "FABRIC TYPE IS REQUIRED !")
                    ddlFabricType.CssClass = "form-select  is-invalid"
                    ddlFabricType.Focus()
                    Exit Sub
                End If

                If ddlFabricLength.SelectedValue = "" Then
                    Call MessageError(True, "FABRIC/SLAT SIZE IS REQUIRED !")
                    ddlFabricLength.CssClass = "form-select  is-invalid"
                    ddlFabricLength.Focus()
                    Exit Sub
                End If

                If ddlFabricColour.SelectedValue = "" Then
                    Call MessageError(True, "FABRIC COLOUR IS REQUIRED !")
                    ddlFabricColour.CssClass = "form-select  is-invalid"
                    ddlFabricColour.Focus()
                    Exit Sub
                End If
            End If

            If blindName = "Complete" Or blindName = "Track Only" Then
                If txtWidth.Text = "" Or txtWidth.Text = "0" Then
                    Call MessageError(True, "WIDTH IS REQUIRED !")
                    txtWidth.CssClass = "form-control  is-invalid"
                    txtWidth.Focus()
                    Exit Sub
                End If

                If Not txtWidth.Text = "" Then
                    If Not IsNumeric(txtWidth.Text) Then
                        Call MessageError(True, "WIDTH IS SHOULD BE NUMERIC !")
                        txtWidth.CssClass = "form-control  is-invalid"
                        txtWidth.Focus()
                        Exit Sub
                    End If

                    If txtWidth.Text < 150 Then
                        Call MessageError(True, "MINIMUM WIDTH IS 150mm !")
                        txtWidth.CssClass = "form-control  is-invalid"
                        txtWidth.Focus()
                        Exit Sub
                    End If

                    If txtWidth.Text > 6000 Then
                        Call MessageError(True, "MAXIMUM WIDTH IS 6000mm !")
                        txtWidth.CssClass = "form-control  is-invalid"
                        txtWidth.Focus()
                        Exit Sub
                    End If
                End If
            End If

            If blindName = "Complete" Or blindName = "Slat Only" Then
                If txtDrop.Text = "" Or txtDrop.Text = "" Then
                    Call MessageError(True, "DROP IS REQUIRED !")
                    txtDrop.CssClass = "form-control  is-invalid"
                    txtDrop.Focus()
                    Exit Sub
                End If

                If Not txtDrop.Text = "" Then
                    If Not IsNumeric(txtDrop.Text) Then
                        Call MessageError(True, "DROP IS SHOULD BE NUMERIC !")
                        txtDrop.CssClass = "form-control  is-invalid"
                        txtDrop.Focus()
                        Exit Sub
                    End If

                    If txtDrop.Text < 150 Then
                        Call MessageError(True, "MINIMUM DROP IS 150mm !")
                        txtDrop.CssClass = "form-control  is-invalid"
                        txtDrop.Focus()
                        Exit Sub
                    End If

                    If txtDrop.Text > 3200 Then
                        Call MessageError(True, "MAXIMUM DROP IS 3200mm !")
                        txtDrop.CssClass = "form-control  is-invalid"
                        txtDrop.Focus()
                        Exit Sub
                    End If
                End If
            End If

            If blindName = "Complete" Or blindName = "Track Only" Then
                If ddlTrackColour.SelectedValue = "" Then
                    Call MessageError(True, "TRACK COLOUR IS REQUIRED !")
                    ddlTrackColour.CssClass = "form-select  is-invalid"
                    ddlTrackColour.Focus()
                    Exit Sub
                End If
            End If

            If blindName = "Complete" Or blindName = "Track Only" Then
                If ddlStackPosition.SelectedValue = "" Then
                    Call MessageError(True, "STACK POSITION IS REQUIRED !")
                    ddlStackPosition.CssClass = "form-select  is-invalid"
                    ddlStackPosition.Focus()
                    Exit Sub
                End If

                If ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If
            End If

            If controlType = "Chain" Then
                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                ' If txtChainLength.Text = "" Or txtChainLength.Text = "0" Then
                '     Call MessageError(True, "CUSTOM CHAIN LENGTH IS REQUIRED !")
                '     txtChainLength.CssClass = "form-control  is-invalid"
                '     txtChainLength.Focus()
                '     Exit Sub
                ' End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CHAIN LENGTH SHOULD BE NUMERIC !")
                        txtChainLength.CssClass = "form-control  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If

                    If txtChainLength.Text < 1 Then
                        Call MessageError(True, "PLEASE CHECK YOUR CHAIN LENGTH ORDER !")
                        txtChainLength.CssClass = "form-control  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If
                End If
            End If

            If controlType = "Wand" Then
                If ddlWandColour.SelectedValue = "" Then
                    Call MessageError(True, "WAND COLOUR IS REQUIRED !")
                    ddlWandColour.CssClass = "form-select  is-invalid"
                    ddlWandColour.Focus()
                    Exit Sub
                End If

                ' If ddlWandLength.SelectedValue = "" Then
                '     Call MessageError(True, "WAND LENGTH IS REQUIRED !")
                '     ddlWandLength.CssClass = "form-select  is-invalid"
                '     ddlWandLength.Focus()
                '     Exit Sub
                ' End If

                If ddlWandLength.SelectedValue = "custom" AndAlso txtWandCustomLength.Text = "" Then
                    Call MessageError(True, "CUSTOM WAND LENGTH IS REQUIRED !")
                    txtWandCustomLength.CssClass = "form-control  is-invalid"
                    txtWandCustomLength.Focus()
                    Exit Sub
                End If

                If ddlWandLength.SelectedValue = "custom" AndAlso txtWandCustomLength.Text > 3000 Then
                    Call MessageError(True, "MAXIMUM WAND LENGTH IS 3000mm !")
                    txtWandCustomLength.CssClass = "form-control  is-invalid"
                    txtWandCustomLength.Focus()
                    Exit Sub
                End If
            End If

            If blindName = "Complete" Or blindName = "Track Only" Then
                If ddlBrackets.SelectedValue = "" Then
                    Call MessageError(True, "BRACKETS IS REQUIRED !")
                    ddlBrackets.CssClass = "form-select  is-invalid"
                    ddlBrackets.Focus()
                    Exit Sub
                End If

                If ddlBracketColour.SelectedValue = "" Then
                    Call MessageError(True, "BRACKET COLOUR IS REQUIRED !")
                    ddlBracketColour.CssClass = "form-select  is-invalid"
                    ddlBracketColour.Focus()
                    Exit Sub
                End If
            End If

            If Not blindName = "Track Only" Then
                If ddlBottom.SelectedValue = "" Then
                    Call MessageError(True, "BOTTOM IS REQUIRED !")
                    ddlBottom.CssClass = "form-select  is-invalid"
                    ddlBottom.Focus()
                    Exit Sub
                End If
            End If

            If ddlHangerType.SelectedValue = "" Then
                Call MessageError(True, "HANGER TYPE IS REQUIRED !")
                ddlHangerType.CssClass = "form-select  is-invalid"
                ddlHangerType.Focus()
                Exit Sub
            End If


            If Not txtNotes.Text = "" Then
                If InStr(txtNotes.Text, "&") > 0 Then
                    Call MessageError(True, "CHARACTER [&] IS NO RECOMMENDED !")
                    txtNotes.CssClass = "form-control  is-invalid"
                    txtNotes.Focus()
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

                If txtMarkUp.Text < 0 Then
                    Call MessageError(True, "PLEASE CHECK YOUR MARK UP ORDER !")
                    txtMarkUp.CssClass = "form-control  is-invalid"
                    txtMarkUp.Focus()
                    Exit Sub
                End If
            End If

            If msgError.InnerText = "" Then
                If txtMarkUp.Text = "" Then : txtMarkUp.Text = "0" : End If

                Dim userId As String = UCase(Session("UserId")).ToString()
                lblKitId.Text = UCase(ddlControlType.SelectedValue).ToString()
                lblSoeKitId.Text = publicCfg.GetSoeKitId(ddlControlType.SelectedValue)

                Dim fabricGroup As String = publicCfg.GetFabricGroup(ddlFabricColour.SelectedValue)

                Dim priceGroupName As String = blindName & " - " & fabricGroup
                If blindName = "Track Only" Then
                    priceGroupName = blindName & " - " & ddlTubeType.SelectedValue
                End If
                If blindName = "Slat Only" And ddlBottom.SelectedValue = "Top Hanger Only" Then
                    priceGroupName = blindName & " With Hanger - " & fabricGroup
                End If
                Dim priceGroupId As String = publicCfg.GetPriceGroupId(designId, priceGroupName)
                lblPriceGroupId.Text = UCase(priceGroupId).ToString()

                If controlType = "Chain" Then
                    Dim chainColour As String = "(" & ddlChainColour.SelectedValue & ")"

                    Dim chainLength As String = txtChainLength.Text
                    If txtChainLength.Text = "" Then
                        chainLength = "500"
                        If txtDrop.Text > "700" Then : chainLength = "600" : End If
                        If txtDrop.Text > 800 Then : chainLength = "800" : End If
                        If txtDrop.Text > 1100 Then : chainLength = "1000" : End If
                        If txtDrop.Text > 1300 Then : chainLength = "1200" : End If
                        If txtDrop.Text > 1600 Then : chainLength = "1500" : End If
                        If txtDrop.Text > 2000 Then : chainLength = "1800" : End If
                        If txtDrop.Text > 2400 Then : chainLength = "2000" : End If
                        If txtDrop.Text > 2700 Then : chainLength = "2200" : End If
                    End If

                    Dim chainName As String = chainLength & " " & "Chain + Joiner" & " " & chainColour
                    Dim FormulaChain As String = publicCfg.GetItemData("SELECT Id FROM Chains WHERE Name = '" + chainName + "'")
                    IF Not FormulaChain = "" Then
                        lblChainId.Text = FormulaChain
                    End If
                    If FormulaChain = "" Then
                        chainName = "Custom Chain + Joiner " & chainColour
                        lblChainId.Text = publicCfg.GetItemData("SELECT Id FROM Chains WHERE Name = '" + chainName + "'")
                    End If
                    ddlWandColour.SelectedValue = "" : ddlWandLength.SelectedValue = ""
                End If

                If controlType = "Wand" Then
                    lblChainId.Text = ""
                    ddlChainColour.SelectedValue = "" : txtChainLength.Text = ""
                    If ddlWandLength.SelectedValue = "custom" Then
                        lblWandLength.Text = txtWandCustomLength.Text
                    Else
                        lblWandLength.Text = ddlWandLength.SelectedValue
                    End If
                End If

                If blindName = "Complete" Then
                    ddlSlatSize.SelectedValue = "" : txtSlatQty.Text = ""
                End If

                If blindName = "Slat Only" Then
                    txtWidth.Text = "0"
                    ddlSlatSize.SelectedValue = ""
                    ddlMounting.SelectedValue = ""
                    lblChainId.Text = "" : txtChainLength.Text = ""
                    ddlTrackColour.SelectedValue = ""
                    ddlControlPosition.Items.Clear()
                    ddlControlPosition.Items.Add(new ListItem("", ""))
                    ddlWandColour.SelectedValue = ""
                    ddlStackPosition.SelectedValue = ""
                    ddlBrackets.SelectedValue = ""
                    ddlInsertInTrack.SelectedValue = "" : ddlSloper.SelectedValue = ""
                    If txtSlatQty.Text = "" Then : txtSlatQty.Text = 1 : End If
                End If

                If blindName = "Track Only" Then
                    ddlFabricColour.Items.Clear()
                    ddlFabricColour.Items.Add(new ListItem("", ""))
                    ddlFabricColour.SelectedValue = ""
                    txtDrop.Text = "0"
                End If

                If Session("itemAction") = "AddItem" Then
                    lblItemId.Text = publicCfg.CreateOrderItemId()
                    IF blindName <> "Complete" Then 
                        sdsNoComplate.Insert()
                    End if
                    IF blindName = "Complete" Then
                        sdsComplete.Insert()
                    End if
                    publicCfg.InsertActivity(userId, Page.Title, "INSERT ORDER DETAIL. ITEM ID : " & lblItemId.Text)
                End If

                If Session("itemAction") = "EditItem" Or Session("itemAction") = "ViewItem" Then
                    IF blindName <> "Complete" Then 
                        sdsNoComplate.Update()
                    End if
                    IF blindName = "Complete" Then
                        sdsComplete.Update()
                    End if
                    publicCfg.InsertActivity(userId, Page.Title, "UPDATE ORDER DETAIL. ITEM ID : " & lblItemId.Text)
                End If

                Call publicCfg.ResetPriceDetail(lblItemId.Text)
                Call publicCfg.HitungHarga(lblHeaderId.Text, lblItemId.Text)
                Call publicCfg.HitungSurcharge(lblHeaderId.Text, lblItemId.Text)

                Call myCancel()
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
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
            Dim tubeType As String = myData.Tables(0).Rows(0).Item("TubeType").ToString()
            Dim controlType As String = myData.Tables(0).Rows(0).Item("ControlType").ToString()

            Dim fabricId As String = myData.Tables(0).Rows(0).Item("FabricId").ToString()
            Dim fabricType As String = myData.Tables(0).Rows(0).Item("FabricType").ToString()
            Dim fabricLength As String = myData.Tables(0).Rows(0).Item("FabricWidth").ToString()

            Dim chainId As String = myData.Tables(0).Rows(0).Item("ChainId").ToString()
            Dim chainColour As String = myData.Tables(0).Rows(0).Item("ChainColour").ToString()
            Dim chainLength As String = myData.Tables(0).Rows(0).Item("CLength").ToString()

            Call BindBlind()
            Call BindVerticalStyle(blindId)
            Call BindControlType(blindId, tubeType)

            Call BindFabricType()
            Call BindFabricLength(fabricType)
            Call BindFabricColour(fabricType, fabricLength)

            Call BindTrackColour(kitId)
            Call BindControlPosition(kitId)
            Call BindWandLength(String.Empty)
            
            

            Call BindHanger(blindId)

            ddlBlindType.SelectedValue = blindId
            ddlTubeType.SelectedValue = tubeType
            ddlControlType.SelectedValue = kitId
            lblBlindNo.Text = myData.Tables(0).Rows(0).Item("BlindNo").ToString()

            txtQty.Text = myData.Tables(0).Rows(0).Item("Qty").ToString()
            ddlMounting.SelectedValue = myData.Tables(0).Rows(0).Item("Mounting").ToString()
            txtLocation.Text = myData.Tables(0).Rows(0).Item("Location").ToString()

            ddlFabricType.SelectedValue = fabricType
            ddlFabricLength.SelectedValue = fabricLength
            ddlFabricColour.SelectedValue = fabricId

            ddlChainColour.SelectedValue = chainColour
            txtChainLength.Text = myData.Tables(0).Rows(0).Item("ChainLength").ToString()

            Dim WandLengthKey As String() = {"","500","750","1100", "1250", "1500", "2000"}
            Dim WandLengthVal As String = myData.Tables(0).Rows(0).Item("WandLength").ToString()
            If Not WandLengthKey.Contains(WandLengthVal) Then
                ddlWandLength.SelectedValue = "custom"
                txtWandCustomLength.Text = WandLengthVal
            Else
                ddlWandLength.SelectedValue = WandLengthVal
            End If
            Call BindWandColour(ddlWandLength.SelectedValue)
            ddlWandColour.SelectedValue = myData.Tables(0).Rows(0).Item("WandColour").ToString()
            ddlTrackColour.SelectedValue = myData.Tables(0).Rows(0).Item("TrackColour").ToString()

            txtWidth.Text = myData.Tables(0).Rows(0).Item("Width").ToString()
            txtDrop.Text = myData.Tables(0).Rows(0).Item("Drop").ToString()

            txtSlatQty.Text = myData.Tables(0).Rows(0).Item("SlatQty").ToString()
            ddlSlatSize.SelectedValue = myData.Tables(0).Rows(0).Item("SlatSize").ToString()

            ddlControlPosition.SelectedValue = myData.Tables(0).Rows(0).Item("ControlPosition").ToString()
            ddlStackPosition.SelectedValue = myData.Tables(0).Rows(0).Item("StackPosition").ToString()

            ddlBrackets.SelectedValue = myData.Tables(0).Rows(0).Item("BracketOption").ToString()
            ddlBracketColour.SelectedValue = myData.Tables(0).Rows(0).Item("BracketColour").ToString()
            ddlHangerType.SelectedValue = myData.Tables(0).Rows(0).Item("HangerType").ToString()
            ddlBottom.SelectedValue = myData.Tables(0).Rows(0).Item("BottomHoldDown").ToString()

            Dim insertInTrack As String = myData.Tables(0).Rows(0).Item("InsertInTrack").ToString()
            If insertInTrack = "True" Then : ddlInsertInTrack.SelectedValue = "1" : End If
            If insertInTrack = "False" Then : ddlInsertInTrack.SelectedValue = "0" : End If

            Dim sloper As String = myData.Tables(0).Rows(0).Item("Sloper").ToString()
            If sloper = "True" Then : ddlSloper.SelectedValue = "1" : End If
            If sloper = "False" Then : ddlSloper.SelectedValue = "0" : End If

            txtNotes.Text = myData.Tables(0).Rows(0).Item("Notes").ToString()
            txtMarkUp.Text = myData.Tables(0).Rows(0).Item("MarkUp").ToString()
            If txtMarkUp.Text = "0" Then : txtMarkUp.Text = "" : End If

            Call BindComponenForm(kitId)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindItemOrder", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindComponenForm(Data As String)
        Try
            divDetail.Visible = False
            divVerticalStyle.Visible = False
            divControlType.Visible = False

            If Not ddlBlindType.SelectedValue = "" Then
                divVerticalStyle.Visible = True
                divControlType.Visible = True

                Dim blindName As String = publicCfg.GetBlindName(ddlBlindType.SelectedValue)
                If blindName = "Slat Only" Then
                    divControlType.Visible = False
                End If
            End If

            If Not Data = "" Then
                divDetail.Visible = True

                divSlatSize.Visible = False : divSlatQty.Visible = False
                divMounting.Visible = False
                divFabricType.Visible = False : divFabricLength.Visible = False : divFabricColour.Visible = False
                divStackPosition.Visible = False : divControlPosition.Visible = False
                divWand.Visible = False
                divWandCustomLength.Visible = False
                divBtnInfoCustom.Visible = False
                divChainColour.Visible = False : divChainLength.Visible = False
                ddlChainColour.Enabled = True
                divTrackColour.Visible = False
                divBrackets.Visible = False
                divBracketColour.Visible = False
                divHangerType.Visible = False
                divBottom.Visible = False
                divInsertInTrack.Visible = False : noteInsertInTrack.InnerText = ""
                divSloper.Visible = False : noteSloper.InnerText = ""
                divWidth.Visible = True : divDrop.Visible = True
                lblSize.Text = "WIDTH x DROP"

                divMarkUp.Visible = False
                If Session("MarkUpAccess") = True Then : divMarkUp.Visible = True : End If

                Dim blindName As String = publicCfg.GetBlindName(ddlBlindType.SelectedValue)
                Dim tubeType As String = publicCfg.GetTubeType(Data)
                Dim controlType As String = publicCfg.GetControlType(Data)

                If blindName = "Complete" Then
                    divMounting.Visible = True
                    divFabricType.Visible = True : divFabricLength.Visible = True : divFabricColour.Visible = True
                    divStackPosition.Visible = True : divControlPosition.Visible = True

                    If Session("itemAction") = "AddItem" Then
                        ddlBracketColour.SelectedValue = "Silver"
                    End If

                    divTrackColour.Visible = True
                    divBrackets.Visible = True
                    divBracketColour.Visible = True
                    divHangerType.Visible = True
                    divBottom.Visible = True
                    divInsertInTrack.Visible = True
                    divSloper.Visible = True

                    If controlType = "Chain" Then
                        divChainColour.Visible = True
                        divChainLength.Visible = True
                        If tubeType = "28mm Tiltrack" Then
                            ddlChainColour.SelectedValue = "White" : ddlChainColour.Enabled = False
                        End If
                    End If
                    If controlType = "Wand" Or controlType = "Wand" Then
                        divSloper.Visible = True : divWand.Visible = True
                        If ddlWandLength.SelectedValue = "custom" Then 
                            divWandCustomLength.Visible = True
                            divBtnInfoCustom.Visible = True
                        End If
                    End If

                    noteSloper.InnerText = "* Tracks will be supplied first as final measurement will need to be supplied after install"
                    If tubeType = "Fairline" Then
                        noteSloper.InnerText = "* Blades will be Tilt Only - Track supplied First"
                    End If
                End If

                If blindName = "Track Only" Then
                    divSlatQty.Visible = True : divSlatSize.Visible = True
                    divMounting.Visible = True
                    divStackPosition.Visible = True : divControlPosition.Visible = True
                    divTrackColour.Visible = True
                    divBrackets.Visible = True
                    divBracketColour.Visible = True
                    divHangerType.Visible = True
                    divSloper.Visible = True
                    divBottom.Visible = false
                    divInsertInTrack.Visible = True
                    divDrop.Visible = False

                    If Session("itemAction") = "AddItem" Then
                        ddlBracketColour.SelectedValue = "Silver"
                    End If

                    lblSize.Text = "WIDTH"

                    If controlType = "Chain" Then
                        divChainColour.Visible = True
                        divChainLength.Visible = True
                    End If
                    If controlType = "Wand" Or controlType = "Wand" Then
                        divSloper.Visible = False : divWand.Visible = True
                        If ddlWandLength.SelectedValue = "custom" Then 
                            divWandCustomLength.Visible = True 
                            divBtnInfoCustom.Visible = True
                        End If
                    End If

                    noteInsertInTrack.InnerText = "* Karma not available"
                    noteSloper.InnerText = ""
                    If controlType = "Fairline Chain" Or controlType = "Fairline Wand" Then
                        noteInsertInTrack.InnerText = "* Karma not available"
                        noteSloper.InnerText = "* Blades will be Tilt Only"
                    End If
                End If

                If blindName = "Slat Only" Then
                    divWidth.Visible = False
                    divSlatQty.Visible = True
                    divFabricType.Visible = True : divFabricLength.Visible = True : divFabricColour.Visible = True
                    divHangerType.Visible = True
                    divBottom.Visible = True

                    lblSize.Text = "DROP EXACT"
                    If controlType = "Blind Size" Then : lblSize.Text = "DROP BLIND" : End If
                End If

                If tubeType = "28mm Tiltrack" Then
                    divInsertInTrack.Visible = False
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindComponenForm", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindBlind()
        ddlBlindType.Items.Clear()
        Try
            ddlBlindType.DataSource = publicCfg.GetListData("SELECT Id, UPPER(Name) AS NameText FROM Blinds WHERE DesignId='" + designId + "' AND Active=1 ORDER BY Name ASC")
            ddlBlindType.DataTextField = "NameText"
            ddlBlindType.DataValueField = "Id"
            ddlBlindType.DataBind()
            If ddlBlindType.Items.Count > 1 Then
                ddlBlindType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindBlind", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindVerticalStyle(BlindId As String)
        ddlTubeType.Items.Clear()
        Try
            If Not BlindId = "" Then
                ddlTubeType.DataSource = publicCfg.GetListData("SELECT TubeType AS TubeValue, UPPER(TubeType) AS TubeText FROM HardwareKits WHERE BlindId='" + BlindId + "' AND Active=1 GROUP BY TubeType ORDER BY TubeType ASC")
                ddlTubeType.DataTextField = "TubeText"
                ddlTubeType.DataValueField = "TubeValue"
                ddlTubeType.DataBind()
                If ddlTubeType.Items.Count > 1 Then
                    ddlTubeType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindVerticalStyle", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindControlType(BlindId As String, TubeType As String)
        ddlControlType.Items.Clear()
        Try
            If Not BlindId = "" And Not TubeType = "" Then
                ddlControlType.DataSource = publicCfg.GetListData("SELECT *, UPPER(ControlType) AS ControlText FROM HardwareKits WHERE DesignId='" + designId + "' AND BlindId = '" + BlindId + "' AND TubeType='" + TubeType + "' ORDER BY Name ASC")
                ddlControlType.DataTextField = "ControlText"
                ddlControlType.DataValueField = "Id"
                ddlControlType.DataBind()
                If ddlControlType.Items.Count > 1 Then
                    ddlControlType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindControlType", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindFabricType()
        ddlFabricType.Items.Clear()
        Try
            ddlFabricType.Items.Add(New ListItem("", ""))
            ddlFabricType.DataSource = publicCfg.GetListData("SELECT UPPER(Type) AS TypeText, Type AS TypeValue FROM Fabrics WHERE DesignId='" + designId + "' AND Active ='1' GROUP BY Type ORDER BY Type ASC")
            ddlFabricType.DataTextField = "TypeText"
            ddlFabricType.DataValueField = "TypeValue"
            ddlFabricType.DataBind()
            If ddlFabricType.Items.Count > 1 Then
                ddlFabricType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindFabricType", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindFabricLength(Type As String)
        ddlFabricLength.Items.Clear()
        Try
            If Not Type = "" Then
                ddlFabricLength.DataSource = publicCfg.GetListData("SELECT Width FROM Fabrics WHERE DesignId='" + designId + "' AND Type='" + Type + "' AND Active='1' GROUP BY Width ORDER BY Width ASC")
                ddlFabricLength.DataTextField = "Width"
                ddlFabricLength.DataValueField = "Width"
                ddlFabricLength.DataBind()
                If ddlFabricLength.Items.Count > 1 Then
                    ddlFabricLength.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindFabricLength", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindFabricColour(Type As String, Length As String)
        ddlFabricColour.Items.Clear()
        Try
            If Not Length = "" And Not Type = "" Then
                ddlFabricColour.DataSource = publicCfg.GetListData("SELECT Id, UPPER(Colour) AS Colour FROM Fabrics WHERE DesignId='" + designId + "' AND Type='" + Type + "' AND Width='" + Length + "' AND Active='1'  ORDER BY Name ASC")
                ddlFabricColour.DataTextField = "Colour"
                ddlFabricColour.DataValueField = "Id"
                ddlFabricColour.DataBind()
                If ddlFabricColour.Items.Count > 1 Then
                    ddlFabricColour.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindFabricColour", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindTrackColour(Data As String)
        ddlTrackColour.Items.Clear()
        Try
            If Not Data = "" Then
                Dim tubeType As String = publicCfg.GetTubeType(Data)

                If tubeType = "28mm Tiltrack" Then
                    ' ddlTrackColour.Items.Add(New ListItem("BIRCH WHITE", "Birch White"))
                    ddlTrackColour.Items.Add(New ListItem("PRIMROSE", "Primrose"))
                    ' ddlTrackColour.Items.Add(New ListItem("WHITE", "White"))
                End If

                If tubeType = "Fairline" Then
                    ddlTrackColour.Items.Add(New ListItem("BEIGE", "Beige"))
                    ddlTrackColour.Items.Add(New ListItem("BIRCH WHITE", "Birch White"))
                    ddlTrackColour.Items.Add(New ListItem("BLACK", "Black"))
                    ddlTrackColour.Items.Add(New ListItem("WHITE", "White"))
                    ddlTrackColour.Items.Add(New ListItem("SILVER", "Silver"))
                End If

            End If
            If ddlTrackColour.Items.Count > 1 Then
                ddlTrackColour.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindTrackColour", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindControlPosition(Data As String)
        ddlControlPosition.Items.Clear()
        Try
            If Not Data = "" Then
                Dim controlType As String = publicCfg.GetControlType(Data)

                ddlControlPosition.Items.Add(New ListItem("RHC", "RHC")) 
                ddlControlPosition.Items.Add(New ListItem("LHC", "LHC"))
                If controlType = "Wand" Then
                    ddlControlPosition.Items.Add(New ListItem("TWIN WAND", "Twin Wand"))
                End If
            End If

            If ddlTrackColour.Items.Count > 1 Then
                ddlControlPosition.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindControlPosition", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindWandColour(Data As String)
        ddlWandColour.Items.Clear()
        Try
        If Not Data = "" Then
            If Not Data = "custom"  Then
                ddlWandColour.Items.Clear()
                ddlWandColour.Items.Add(New ListItem("BIRCH", "Birch"))
                ddlWandColour.Items.Add(New ListItem("BLACK", "Black"))
                ddlWandColour.Items.Add(New ListItem("BEIGE", "Beige"))
                ddlWandColour.Items.Add(New ListItem("WHITE", "White"))
            End If

            If Data = "custom" Then
                ddlWandColour.Items.Clear()
                ddlWandColour.Items.Add(New ListItem("WHITE", "White"))
            End If

            If ddlWandColour.Items.Count > 1 Then
                ddlWandColour.Items.Insert(0, New ListItem("", ""))
            End If
        End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindControlPosition", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindWandLength(Data As String)
        ddlWandLength.Items.Clear()
        Try
            ddlWandLength.Items.Clear()
            ddlWandLength.Items.Add(New ListItem("Custom (White Only)", "custom"))
            ddlWandLength.Items.Add(New ListItem("500mm", "500"))
            ddlWandLength.Items.Add(New ListItem("750mm", "750"))
            ddlWandLength.Items.Add(New ListItem("800mm", "800"))
            ddlWandLength.Items.Add(New ListItem("1100mm", "1100"))
            ddlWandLength.Items.Add(New ListItem("1250mm", "1250"))
            ddlWandLength.Items.Add(New ListItem("1500mm", "1500"))
            ddlWandLength.Items.Add(New ListItem("2000mm", "2000"))

            If ddlWandLength.Items.Count > 1 Then
                ddlWandLength.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindControlPosition", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindHanger(BlindId As String)
        ddlHangerType.Items.Clear()
        Try
            If Not BlindId = "" Then
                ddlHangerType.Items.Add(New ListItem("STANDARD", "Standard"))
                ddlHangerType.Items.Add(New ListItem("PEGHOOK", "Peghook"))

                Dim blindName As String = publicCfg.GetBlindName(BlindId)

                If blindName = "Vertical Slat Only" Then
                    ddlHangerType.Items.Clear()
                    ddlHangerType.Items.Add(New ListItem("STANDARD", "Standard"))
                    ddlHangerType.Items.Add(New ListItem("PEGHOOK", "Peghook"))
                    ddlHangerType.Items.Add(New ListItem("TILTRACK 28mm", "Tiltrack 28mm"))
                End If
            End If
            If ddlHangerType.Items.Count > 1 Then
                ddlHangerType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindHanger", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlBlindType.CssClass="form-select "
        ddlTubeType.CssClass="form-select "
        ddlControlType.CssClass="form-select "
        txtQty.CssClass="form-control "
        ddlMounting.CssClass="form-select "
        txtLocation.CssClass="form-control "
        ddlFabricType.CssClass="form-select "
        ddlFabricLength.CssClass="form-select "
        ddlFabricColour.CssClass="form-select "
        ddlChainColour.CssClass="form-select "
        txtChainLength.CssClass="form-control "
        ddlWandColour.CssClass="form-select "
        ddlWandLength.CssClass="form-select "
        txtWandCustomLength.CssClass="form-control "
        ddlTrackColour.CssClass="form-select "
        txtWidth.CssClass="form-control "
        txtDrop.CssClass="form-control "
        ddlSlatSize.CssClass="form-select "
        txtSlatQty.CssClass="form-control "
        ddlControlPosition.CssClass="form-select "
        ddlStackPosition.CssClass="form-select "
        ddlBrackets.CssClass="form-select "
        ddlBracketColour.CssClass="form-select "
        ddlHangerType.CssClass="form-select "
        ddlBottom.CssClass="form-select "
        ddlInsertInTrack.CssClass="form-select "
        ddlSloper.CssClass="form-select "
        txtNotes.CssClass="form-control"
        txtMarkUp.CssClass="form-control "
    End Sub

    Private Sub myCancel()
        Session("headerId") = lblHeaderId.Text
        Response.Redirect("~/order/detail", False)
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        ' If Show = True Then : divError.Visible = True : End If
        If Show = True Then
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
