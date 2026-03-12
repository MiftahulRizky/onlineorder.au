Imports System.Data

Partial Class Order_Vertical
    Inherits Page

    Dim publicCfg As New PublicConfig
    Dim orderCfg As New OrderConfig

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

        If Session("orderType") = "" Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If

        If Session("itemAction") = "" Then
            Response.Redirect("~/order/detail?param=" & Session("headerId") & "&ordertype=" & Session("orderType"), False)
            Exit Sub
        End If

        designId = Session("designId")
        If publicCfg.GetDesignActive(designId) = False Then
            Response.Redirect("~/order/maintenance", False)
            Exit Sub
        End If

        lblHeaderId.Text = Session("headerId") : lblItemId.Text = ""
        lblOrderType.Text = Session("orderType")
        lblOrderNo.Text = publicCfg.GetOrderNo(lblHeaderId.Text)
        lblOrderCust.Text = publicCfg.GetOrderCust(lblHeaderId.Text)

        pageAction.InnerHtml = Session("itemAction")
        pageTitle.InnerHtml = publicCfg.GetDesignName(designId)

        If Session("itemAction") = "AddItem" Then
            lblHeaderId.Text = Session("HeaderId")
            lblBlindNo.Text = "Blind 1"
            btnSubmit.Visible = True : btnSubmit.Text = "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Submit"
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
                Call BindBracketColour(ddlControlType.SelectedValue)
                Call BindControlPosition(ddlControlType.SelectedValue)

                Call BindHanger(ddlBlindType.SelectedValue)

                Call BindComponenForm(ddlControlType.SelectedValue)
            End If
        End If

        If Session("itemAction") = "ViewItem" Then
            lblHeaderId.Text = Session("HeaderId")
            lblItemId.Text = Session("itemId")
            btnSubmit.Visible = False : btnSubmit.Text = "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Submit"
            cardTitle.InnerHtml = "VIEW ITEM ID : " & lblItemId.Text
            If Session("RoleName") = "Administrator" Then : btnSubmit.Visible = True : End If
            If Not IsPostBack Then
                Call BindItemOrder(lblItemId.Text)
            End If
        End If

        If Session("itemAction") = "EditItem" Then
            lblHeaderId.Text = Session("HeaderId")
            lblItemId.Text = Session("itemId")
            btnSubmit.Visible = True : btnSubmit.Text = "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Submit"
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
        Call BindBracketColour(ddlControlType.SelectedValue)
        Call BindControlPosition(ddlControlType.SelectedValue)

        Call BindHanger(ddlBlindType.SelectedValue)

        Call BindComponenForm(ddlControlType.SelectedValue)
    End Sub

    Protected Sub ddlTubeType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindControlType(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)

        Call BindTrackColour(ddlControlType.SelectedValue)
        Call BindBracketColour(ddlControlType.SelectedValue)
        Call BindControlPosition(ddlControlType.SelectedValue)

        Call BindHanger(ddlBlindType.SelectedValue)

        Call BindComponenForm(ddlControlType.SelectedValue)
    End Sub

    Protected Sub ddlControlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindTrackColour(ddlControlType.SelectedValue)
        Call BindBracketColour(ddlControlType.SelectedValue)

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

            If Not txtLocation.Text = "" Then
                If InStr(txtLocation.Text, "&") > 0 Then
                    Call MessageError(True, "CHARACTER [&] IS NO RECOMMENDED !")
                    txtLocation.CssClass = "form-control  is-invalid"
                    txtLocation.Focus()
                    Exit Sub
                End If
            End If

            If Not blindName = "Slat Only" And ddlMounting.SelectedValue = "" Then
                Call MessageError(True, "MOUNTING IS REQUIRED !")
                ddlMounting.CssClass = "form-select  is-invalid"
                ddlMounting.Focus()
                Exit Sub
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
                If ddlWandLength.SelectedValue = "" Then
                    Call MessageError(True, "WAND LENGTH IS REQUIRED !")
                    ddlWandLength.CssClass = "form-select  is-invalid"
                    ddlWandLength.Focus()
                    Exit Sub
                End If

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

                If ddlWandColour.SelectedValue = "" Then
                    Call MessageError(True, "WAND COLOUR IS REQUIRED !")
                    ddlWandColour.CssClass = "form-select  is-invalid"
                    ddlWandColour.Focus()
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

                lblKitId.Text = UCase(ddlControlType.SelectedValue).ToString()
                lblSoeKitId.Text = publicCfg.GetSoeKitId(ddlControlType.SelectedValue)

                Dim designName As String = publicCfg.GetDesignName(designId)
                Dim exactName As String = designName & " - " & blindName
                lblExactId.Text = orderCfg.GetItemData("SELECT ExactId FROM Exacts WHERE Name = '" + exactName + "'")

                Dim fabricGroup As String = publicCfg.GetFabricGroup(ddlFabricColour.SelectedValue)

                Dim priceGroupName As String = blindName & " - " & fabricGroup
                If blindName = "Track Only" Then
                    priceGroupName = blindName & " - " & ddlTubeType.SelectedValue
                End If
                If blindName = "Slat Only" And ddlBottom.SelectedValue = "Top Hanger Only" Then
                    priceGroupName = blindName & " With Hanger - " & fabricGroup
                End If

                Dim priceGroupId As String = publicCfg.GetPriceGroupId(designId, priceGroupName)
                If String.IsNullOrEmpty(priceGroupId) Then
                    throw New Exception("SOMETHING WENT WRONG !")
                End If
                lblPriceGroupId.Text = UCase(priceGroupId).ToString()

                If controlType = "Chain" Then
                    Dim chainColour As String = "(" & ddlChainColour.SelectedValue & ")"

                    Dim chainLength As String = txtChainLength.Text
                    Dim dropValue As Double

                    ' Pastikan drop berupa angka valid
                    If Not Double.TryParse(txtDrop.Text, dropValue) Then
                        dropValue = 0
                    End If

                    If String.IsNullOrWhiteSpace(txtChainLength.Text) Then
                        If dropValue >= 3000 Then
                            chainLength = "2200"
                        ElseIf dropValue >= 2700 Then
                            chainLength = "2000"
                        ElseIf dropValue >= 2400 Then
                            chainLength = "1800"
                        ElseIf dropValue >= 2000 Then
                            chainLength = "1500"
                        ElseIf dropValue >= 1600 Then
                            chainLength = "1250"
                        ElseIf dropValue >= 1300 Then
                            chainLength = "1000"
                        ElseIf dropValue >= 1100 Then
                            chainLength = "800"
                        ElseIf dropValue >= 800 Then
                            chainLength = "600"
                        Else
                            chainLength = "500"
                        End If
                    End If


                    txtChainLength.Text = chainLength '#return new chain length

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

                    Dim ListParamCarriers As New List(Of Object) From {
                        ddlSlatSize.SelectedValue,
                        ddlTubeType.SelectedValue,
                        designName,
                        blindName,
                        txtWidth.Text,
                        "CarrierQty"
                    }
                    If txtSlatQty.Text = "" Then 
                         txtSlatQty.Text = "1" 'GetCarrierSpacer(ListParamCarriers)
                    End If
                End If

                If blindName = "Track Only" Then
                    ddlFabricColour.Items.Clear()
                    ddlFabricColour.Items.Add(new ListItem("", ""))
                    ddlFabricColour.SelectedValue = ""
                    txtDrop.Text = "0"

                    Dim ListParamCarriers As New List(Of Object) From {
                        ddlSlatSize.SelectedValue,
                        ddlTubeType.SelectedValue,
                        designName,
                        blindName,
                        txtWidth.Text,
                        "CarrierQty"
                    }
                    If txtSlatQty.Text = "" Then 
                         txtSlatQty.Text = GetCarrierSpacer(ListParamCarriers)
                    End If
                End If

                If ddlTubeType.SelectedValue = "Louvolite" Then
                    ddlInsertInTrack.SelectedValue = ""
                End If

                ' Call MessageError(True, txtSlatQty.Text)
                ' Exit Sub

                If Session("itemAction") = "AddItem" Then
                    lblItemId.Text = publicCfg.CreateOrderItemId()
                    IF blindName <> "Complete" Then 
                        sdsNoComplate.Insert()
                    End if
                    IF blindName = "Complete" Then
                        sdsComplete.Insert()
                    End if
                    Dim dataLog As Object() = {lblHeaderId.Text, lblItemId.Text, lblOrderType.Text, Session("LoginId"), "Add Item Order"}
                    orderCfg.Log_Orders(dataLog)
                End If

                If Session("itemAction") = "EditItem" Or Session("itemAction") = "ViewItem" Then
                    IF blindName <> "Complete" Then 
                        sdsNoComplate.Update()
                    End if
                    IF blindName = "Complete" Then
                        sdsComplete.Update()
                    End if
                    Dim dataLog As Object() = {lblHeaderId.Text, lblItemId.Text, lblOrderType.Text, Session("LoginId"), "Update Item Order"}
                    orderCfg.Log_Orders(dataLog)
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
            Call BindBracketColour(kitId)
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

                    If Session("itemAction") = "AddItem" AND Not ddlTubeType.SelectedValue = "Louvolite" Then
                        ddlBracketColour.SelectedValue = "Silver"
                    End If

                    divTrackColour.Visible = True
                    divBrackets.Visible = True
                    divBracketColour.Visible = True
                    divHangerType.Visible = True
                    divBottom.Visible = True
                    divInsertInTrack.Visible = True
                    divSloper.Visible = True

                    if tubeType = "Louvolite" Then
                        divInsertInTrack.Visible = False
                    End If

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

                    ' If Session("itemAction") = "AddItem"   Then
                    '     ddlBracketColour.SelectedValue = "Silver"
                    ' End If

                    if tubeType = "Louvolite" Then
                        divInsertInTrack.Visible = False
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
            ddlFabricType.DataSource = publicCfg.GetListData(String.Format("SELECT UPPER(Type) AS TypeText, Type AS TypeValue FROM Fabrics WHERE DesignId='{0}' AND Active ='1' GROUP BY Type ORDER BY Type ASC", designId))
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
            Dim Width As String = ""
            If ddlTubeType.SelectedValue = "Louvolite" Then
                Width = "AND Width IN ('89', '127')"
            End If
            If Not Type = "" Then
                ddlFabricLength.DataSource = publicCfg.GetListData(String.Format("SELECT Width FROM Fabrics WHERE DesignId='{0}'  AND Type='{1}' {2} AND Active='1' GROUP BY Width ORDER BY Width ASC",designId, Type, Width))
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
                    ddlTrackColour.Items.Add(New ListItem("PRIMROSE", "Primrose"))
                End If

                If tubeType = "Fairline" Or tubeType = "Javaline" Then
                    ddlTrackColour.Items.Add(New ListItem("BEIGE", "Beige"))
                    ddlTrackColour.Items.Add(New ListItem("BIRCH WHITE", "Birch White"))
                    ddlTrackColour.Items.Add(New ListItem("BLACK", "Black"))
                    ddlTrackColour.Items.Add(New ListItem("SILVER", "Silver"))
                End If

                If tubeType = "Louvolite" Then
                    ddlTrackColour.Items.Add(New ListItem("BLACK", "Black"))
                    ddlTrackColour.Items.Add(New ListItem("WHITE", "White"))
                    ddlTrackColour.Items.Add(New ListItem("GREY", "Grey"))
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

    Private Sub BindBracketColour(Data As String)
        ddlBracketColour.Items.Clear()
        Try
            If Not Data = "" Then
                Dim tubeType As String = publicCfg.GetTubeType(Data)

                If tubeType = "Louvolite" Then
                    ddlBracketColour.Items.Add(New ListItem("BLACK", "Black"))
                    ddlBracketColour.Items.Add(New ListItem("WHITE", "White"))
                    ddlBracketColour.Items.Add(New ListItem("GREY", "Grey"))
                Else
                    ddlBracketColour.Items.Add(New ListItem("BLACK", "Black"))
                    ddlBracketColour.Items.Add(New ListItem("WHITE", "White"))
                    ddlBracketColour.Items.Add(New ListItem("SILVER", "Silver"))
                End If

            End If
            If ddlBracketColour.Items.Count > 1 Then
                ddlBracketColour.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindBracketColour", ex.ToString())
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

    Private Function GetCarrierSpacer(ListParam As List(Of Object)) As String
        Dim result As String = "0"
        Dim SlatSize As String = CStr(ListParam(0))
        Dim TubeType As String = CStr(ListParam(1))
        Dim DesignName As String = CStr(ListParam(2))
        Dim BlindName As String = CStr(ListParam(3))
        Dim Width As Integer = CStr(ListParam(4))
        Dim param As String = CStr(ListParam(5))

        If DesignName = "Vertical Blinds" Then
            If BlindName = "Complete" OR BlindName = "Track Only" Then
                Select Case SlatSize
                    Case "127", "127mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer127Tiltrack
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        ElseIf TubeType.Contains("Louvolite") Then
                            For Each item In Spacer127Louvolite
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer127Metal
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        End If
                    Case "100", "100mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer100Tiltrack
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer100Metal
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        End If
                    Case "89", "89mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer89Tiltrack
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        ElseIf TubeType.Contains("Louvolite") Then
                            For Each item In Spacer89Louvolite
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer89Metal
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        End If
                    Case "63", "63mm"
                        If TubeType.Contains("Tiltrack") Then
                            For Each item In Spacer63Tiltrack
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        Else
                            For Each item In Spacer63Metal
                                If Width <= item.MaxWidth Then
                                    If param = "Spacer1Type" Then
                                        result = item.Spacer1Type
                                    End If
                                    If param = "CarrierQty" Then
                                        result = item.CarriersQty
                                    End If
                                    Exit For
                                End If
                            Next
                        End If
                End Select
            End If
        End If
        Return result
    End Function

    Private Class SpacerInfo
        Public MaxWidth As Integer
        Public Spacer1Type As String
        Public CarriersQty As Integer
    End Class

    Private ReadOnly Spacer127Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 249, .Spacer1Type = "110", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 250, .Spacer1Type = "111", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 251, .Spacer1Type = "112", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 252, .Spacer1Type = "113", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 253, .Spacer1Type = "114", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 254, .Spacer1Type = "115", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 255, .Spacer1Type = "116", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 358, .Spacer1Type = "110", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 358, .Spacer1Type = "110", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 360, .Spacer1Type = "111", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 362, .Spacer1Type = "112", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 364, .Spacer1Type = "113", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 366, .Spacer1Type = "114", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 368, .Spacer1Type = "115", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 370, .Spacer1Type = "116", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 466, .Spacer1Type = "110", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 469, .Spacer1Type = "111", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 472, .Spacer1Type = "112", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 475, .Spacer1Type = "113", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 478, .Spacer1Type = "114", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 481, .Spacer1Type = "115", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 484, .Spacer1Type = "116", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 574, .Spacer1Type = "110", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 579, .Spacer1Type = "111", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 583, .Spacer1Type = "112", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 587, .Spacer1Type = "113", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 591, .Spacer1Type = "114", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 595, .Spacer1Type = "115", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 599, .Spacer1Type = "116", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 683, .Spacer1Type = "110", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 688, .Spacer1Type = "111", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 693, .Spacer1Type = "112", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 698, .Spacer1Type = "113", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 703, .Spacer1Type = "114", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 708, .Spacer1Type = "115", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 713, .Spacer1Type = "116", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 791, .Spacer1Type = "110", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 797, .Spacer1Type = "111", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 803, .Spacer1Type = "112", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 809, .Spacer1Type = "113", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 815, .Spacer1Type = "114", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 821, .Spacer1Type = "115", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 827, .Spacer1Type = "116", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 899, .Spacer1Type = "110", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 907, .Spacer1Type = "111", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 914, .Spacer1Type = "112", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 921, .Spacer1Type = "113", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 928, .Spacer1Type = "114", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 935, .Spacer1Type = "115", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 942, .Spacer1Type = "116", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 1007, .Spacer1Type = "110", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1016, .Spacer1Type = "111", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1024, .Spacer1Type = "112", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1032, .Spacer1Type = "113", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1040, .Spacer1Type = "114", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1048, .Spacer1Type = "115", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1056, .Spacer1Type = "116", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1116, .Spacer1Type = "110", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1126, .Spacer1Type = "111", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1135, .Spacer1Type = "112", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1144, .Spacer1Type = "113", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1153, .Spacer1Type = "114", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1162, .Spacer1Type = "115", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1171, .Spacer1Type = "116", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1224, .Spacer1Type = "110", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1235, .Spacer1Type = "111", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1245, .Spacer1Type = "112", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1255, .Spacer1Type = "113", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1265, .Spacer1Type = "114", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1275, .Spacer1Type = "115", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1285, .Spacer1Type = "116", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1332, .Spacer1Type = "110", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1344, .Spacer1Type = "111", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1355, .Spacer1Type = "112", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1366, .Spacer1Type = "113", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1377, .Spacer1Type = "114", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1388, .Spacer1Type = "115", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1399, .Spacer1Type = "116", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1441, .Spacer1Type = "110", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1454, .Spacer1Type = "111", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1466, .Spacer1Type = "112", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1478, .Spacer1Type = "113", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1490, .Spacer1Type = "114", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1502, .Spacer1Type = "115", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1514, .Spacer1Type = "116", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1549, .Spacer1Type = "110", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1563, .Spacer1Type = "111", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1576, .Spacer1Type = "112", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1589, .Spacer1Type = "113", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1602, .Spacer1Type = "114", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1615, .Spacer1Type = "115", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1628, .Spacer1Type = "116", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1641, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1657, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1673, .Spacer1Type = "111", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1687, .Spacer1Type = "112", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1701, .Spacer1Type = "113", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1715, .Spacer1Type = "114", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1729, .Spacer1Type = "115", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1743, .Spacer1Type = "116", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1766, .Spacer1Type = "110", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1782, .Spacer1Type = "111", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1797, .Spacer1Type = "112", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1812, .Spacer1Type = "113", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1827, .Spacer1Type = "114", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1842, .Spacer1Type = "115", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1857, .Spacer1Type = "116", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1872, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1874, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1891, .Spacer1Type = "111", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1907, .Spacer1Type = "112", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1923, .Spacer1Type = "113", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1939, .Spacer1Type = "114", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1955, .Spacer1Type = "115", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1971, .Spacer1Type = "116", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1982, .Spacer1Type = "110", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2001, .Spacer1Type = "111", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2018, .Spacer1Type = "112", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2035, .Spacer1Type = "113", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2052, .Spacer1Type = "114", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2069, .Spacer1Type = "115", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2086, .Spacer1Type = "116", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2090, .Spacer1Type = "110", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2110, .Spacer1Type = "111", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2128, .Spacer1Type = "112", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2146, .Spacer1Type = "113", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2164, .Spacer1Type = "114", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2182, .Spacer1Type = "115", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2199, .Spacer1Type = "110", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "116", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2218, .Spacer1Type = "111", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2220, .Spacer1Type = "111", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2239, .Spacer1Type = "112", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2258, .Spacer1Type = "113", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2277, .Spacer1Type = "114", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2296, .Spacer1Type = "115", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2307, .Spacer1Type = "110", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2315, .Spacer1Type = "116", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2329, .Spacer1Type = "111", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2349, .Spacer1Type = "112", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2369, .Spacer1Type = "113", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2389, .Spacer1Type = "114", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2409, .Spacer1Type = "115", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2415, .Spacer1Type = "110", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2429, .Spacer1Type = "116", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2438, .Spacer1Type = "111", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2449, .Spacer1Type = "112", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2459, .Spacer1Type = "112", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2480, .Spacer1Type = "113", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2501, .Spacer1Type = "114", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2522, .Spacer1Type = "115", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2524, .Spacer1Type = "110", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2543, .Spacer1Type = "116", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2548, .Spacer1Type = "111", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2564, .Spacer1Type = "112", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2570, .Spacer1Type = "112", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2592, .Spacer1Type = "113", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2614, .Spacer1Type = "114", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2632, .Spacer1Type = "110", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2636, .Spacer1Type = "115", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2657, .Spacer1Type = "111", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2658, .Spacer1Type = "116", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2680, .Spacer1Type = "112", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2703, .Spacer1Type = "113", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2726, .Spacer1Type = "114", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2740, .Spacer1Type = "110", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2749, .Spacer1Type = "115", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2767, .Spacer1Type = "111", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2772, .Spacer1Type = "116", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2791, .Spacer1Type = "112", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2815, .Spacer1Type = "113", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2839, .Spacer1Type = "114", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2849, .Spacer1Type = "110", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2863, .Spacer1Type = "115", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2876, .Spacer1Type = "111", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2887, .Spacer1Type = "116", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2901, .Spacer1Type = "112", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2926, .Spacer1Type = "113", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2951, .Spacer1Type = "114", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2957, .Spacer1Type = "110", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2976, .Spacer1Type = "115", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2985, .Spacer1Type = "111", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3001, .Spacer1Type = "116", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3011, .Spacer1Type = "112", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3037, .Spacer1Type = "113", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3063, .Spacer1Type = "114", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3065, .Spacer1Type = "110", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3089, .Spacer1Type = "115", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3095, .Spacer1Type = "111", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3115, .Spacer1Type = "116", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3122, .Spacer1Type = "112", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3149, .Spacer1Type = "113", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3173, .Spacer1Type = "110", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3176, .Spacer1Type = "114", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3203, .Spacer1Type = "115", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3204, .Spacer1Type = "111", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3230, .Spacer1Type = "116", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3232, .Spacer1Type = "112", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3260, .Spacer1Type = "113", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3282, .Spacer1Type = "110", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3288, .Spacer1Type = "114", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3314, .Spacer1Type = "111", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3316, .Spacer1Type = "115", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3343, .Spacer1Type = "112", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3344, .Spacer1Type = "116", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3372, .Spacer1Type = "113", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3390, .Spacer1Type = "110", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3401, .Spacer1Type = "114", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3423, .Spacer1Type = "111", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3430, .Spacer1Type = "115", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3453, .Spacer1Type = "112", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3459, .Spacer1Type = "116", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3483, .Spacer1Type = "113", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3498, .Spacer1Type = "110", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3513, .Spacer1Type = "114", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3532, .Spacer1Type = "111", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3543, .Spacer1Type = "115", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3563, .Spacer1Type = "112", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3573, .Spacer1Type = "116", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3594, .Spacer1Type = "113", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3607, .Spacer1Type = "110", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3625, .Spacer1Type = "114", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3642, .Spacer1Type = "111", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3656, .Spacer1Type = "115", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3674, .Spacer1Type = "112", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3687, .Spacer1Type = "116", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3706, .Spacer1Type = "113", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3715, .Spacer1Type = "110", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3738, .Spacer1Type = "114", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3751, .Spacer1Type = "111", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3770, .Spacer1Type = "115", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3784, .Spacer1Type = "112", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3802, .Spacer1Type = "116", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3817, .Spacer1Type = "113", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3823, .Spacer1Type = "110", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3850, .Spacer1Type = "114", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3861, .Spacer1Type = "111", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3883, .Spacer1Type = "115", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3895, .Spacer1Type = "112", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3916, .Spacer1Type = "116", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3929, .Spacer1Type = "113", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3932, .Spacer1Type = "110", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3963, .Spacer1Type = "114", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3970, .Spacer1Type = "111", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3997, .Spacer1Type = "115", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4005, .Spacer1Type = "112", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4031, .Spacer1Type = "116", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4040, .Spacer1Type = "110", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4040, .Spacer1Type = "113", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4075, .Spacer1Type = "114", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4079, .Spacer1Type = "111", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4110, .Spacer1Type = "115", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4115, .Spacer1Type = "112", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4145, .Spacer1Type = "116", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4148, .Spacer1Type = "110", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4151, .Spacer1Type = "113", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4187, .Spacer1Type = "114", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4189, .Spacer1Type = "111", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4223, .Spacer1Type = "115", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4226, .Spacer1Type = "112", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4256, .Spacer1Type = "110", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4259, .Spacer1Type = "116", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4263, .Spacer1Type = "113", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4298, .Spacer1Type = "111", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4300, .Spacer1Type = "114", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4336, .Spacer1Type = "112", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4337, .Spacer1Type = "115", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4365, .Spacer1Type = "110", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4374, .Spacer1Type = "113", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4374, .Spacer1Type = "116", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4408, .Spacer1Type = "111", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4412, .Spacer1Type = "114", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4447, .Spacer1Type = "112", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4450, .Spacer1Type = "115", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4473, .Spacer1Type = "110", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4486, .Spacer1Type = "113", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4488, .Spacer1Type = "116", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4517, .Spacer1Type = "111", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4525, .Spacer1Type = "114", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4557, .Spacer1Type = "112", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4564, .Spacer1Type = "115", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4581, .Spacer1Type = "110", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4597, .Spacer1Type = "113", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4603, .Spacer1Type = "116", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4626, .Spacer1Type = "111", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4637, .Spacer1Type = "114", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4667, .Spacer1Type = "112", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4677, .Spacer1Type = "115", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4690, .Spacer1Type = "110", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4708, .Spacer1Type = "113", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4717, .Spacer1Type = "116", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4736, .Spacer1Type = "111", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4749, .Spacer1Type = "114", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4778, .Spacer1Type = "112", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4790, .Spacer1Type = "115", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4798, .Spacer1Type = "110", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4820, .Spacer1Type = "113", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4831, .Spacer1Type = "116", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4845, .Spacer1Type = "111", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4862, .Spacer1Type = "114", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4888, .Spacer1Type = "112", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4904, .Spacer1Type = "115", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4906, .Spacer1Type = "110", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 4931, .Spacer1Type = "113", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4946, .Spacer1Type = "116", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4955, .Spacer1Type = "111", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 4974, .Spacer1Type = "114", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4999, .Spacer1Type = "112", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5015, .Spacer1Type = "110", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5017, .Spacer1Type = "115", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5043, .Spacer1Type = "113", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5060, .Spacer1Type = "116", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5064, .Spacer1Type = "111", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5087, .Spacer1Type = "114", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5109, .Spacer1Type = "112", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5123, .Spacer1Type = "110", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5131, .Spacer1Type = "115", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5154, .Spacer1Type = "113", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5173, .Spacer1Type = "111", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5175, .Spacer1Type = "116", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5199, .Spacer1Type = "114", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5219, .Spacer1Type = "112", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5231, .Spacer1Type = "110", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5244, .Spacer1Type = "115", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5265, .Spacer1Type = "113", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5283, .Spacer1Type = "111", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5289, .Spacer1Type = "116", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5311, .Spacer1Type = "114", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5330, .Spacer1Type = "112", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5339, .Spacer1Type = "110", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5357, .Spacer1Type = "115", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5377, .Spacer1Type = "113", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5392, .Spacer1Type = "111", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5403, .Spacer1Type = "116", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5424, .Spacer1Type = "114", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5440, .Spacer1Type = "112", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5448, .Spacer1Type = "110", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5471, .Spacer1Type = "115", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5488, .Spacer1Type = "113", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5502, .Spacer1Type = "111", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5518, .Spacer1Type = "116", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5536, .Spacer1Type = "114", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5551, .Spacer1Type = "112", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5556, .Spacer1Type = "110", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5584, .Spacer1Type = "115", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5600, .Spacer1Type = "113", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5611, .Spacer1Type = "111", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5632, .Spacer1Type = "116", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5649, .Spacer1Type = "114", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5661, .Spacer1Type = "112", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5664, .Spacer1Type = "110", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5698, .Spacer1Type = "115", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5711, .Spacer1Type = "113", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5720, .Spacer1Type = "111", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5747, .Spacer1Type = "116", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5761, .Spacer1Type = "114", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5771, .Spacer1Type = "112", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5773, .Spacer1Type = "110", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5811, .Spacer1Type = "115", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5822, .Spacer1Type = "113", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5830, .Spacer1Type = "111", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5861, .Spacer1Type = "116", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5873, .Spacer1Type = "114", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5881, .Spacer1Type = "110", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 5882, .Spacer1Type = "112", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5924, .Spacer1Type = "115", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5934, .Spacer1Type = "113", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5939, .Spacer1Type = "111", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 5975, .Spacer1Type = "116", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 5986, .Spacer1Type = "114", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 5989, .Spacer1Type = "110", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 5992, .Spacer1Type = "112", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6038, .Spacer1Type = "115", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 6045, .Spacer1Type = "113", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6049, .Spacer1Type = "111", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6090, .Spacer1Type = "116", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 6097, .Spacer1Type = "110", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6098, .Spacer1Type = "114", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6103, .Spacer1Type = "112", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6151, .Spacer1Type = "115", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6157, .Spacer1Type = "113", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6158, .Spacer1Type = "111", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6204, .Spacer1Type = "116", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 6206, .Spacer1Type = "110", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6211, .Spacer1Type = "114", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6213, .Spacer1Type = "112", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6265, .Spacer1Type = "115", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6267, .Spacer1Type = "111", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6268, .Spacer1Type = "113", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6314, .Spacer1Type = "110", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6319, .Spacer1Type = "116", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 6323, .Spacer1Type = "112", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6323, .Spacer1Type = "114", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6377, .Spacer1Type = "111", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6378, .Spacer1Type = "115", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6379, .Spacer1Type = "113", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6422, .Spacer1Type = "110", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 6433, .Spacer1Type = "116", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 6434, .Spacer1Type = "112", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6435, .Spacer1Type = "114", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 6486, .Spacer1Type = "111", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 6491, .Spacer1Type = "113", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 6491, .Spacer1Type = "115", .CarriersQty = 57}
    }

    Private ReadOnly Spacer100Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 196, .Spacer1Type = "84", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 197, .Spacer1Type = "85", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 198, .Spacer1Type = "86", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 199, .Spacer1Type = "87", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 200, .Spacer1Type = "88", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 279, .Spacer1Type = "84", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 281, .Spacer1Type = "85", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 283, .Spacer1Type = "86", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 285, .Spacer1Type = "87", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 287, .Spacer1Type = "88", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 361, .Spacer1Type = "84", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 364, .Spacer1Type = "85", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 367, .Spacer1Type = "86", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 370, .Spacer1Type = "87", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 373, .Spacer1Type = "88", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 444, .Spacer1Type = "84", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 448, .Spacer1Type = "85", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 452, .Spacer1Type = "86", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 456, .Spacer1Type = "87", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 460, .Spacer1Type = "88", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 526, .Spacer1Type = "84", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 531, .Spacer1Type = "85", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 536, .Spacer1Type = "86", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 541, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 546, .Spacer1Type = "88", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 608, .Spacer1Type = "84", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 614, .Spacer1Type = "85", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 620, .Spacer1Type = "86", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 626, .Spacer1Type = "87", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 632, .Spacer1Type = "88", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 691, .Spacer1Type = "84", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 698, .Spacer1Type = "85", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 705, .Spacer1Type = "86", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 712, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 719, .Spacer1Type = "88", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 773, .Spacer1Type = "84", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 781, .Spacer1Type = "85", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 789, .Spacer1Type = "86", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 797, .Spacer1Type = "87", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 805, .Spacer1Type = "88", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 856, .Spacer1Type = "84", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 865, .Spacer1Type = "85", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 874, .Spacer1Type = "86", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 883, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 892, .Spacer1Type = "88", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 938, .Spacer1Type = "84", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 948, .Spacer1Type = "85", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 958, .Spacer1Type = "86", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 968, .Spacer1Type = "87", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 978, .Spacer1Type = "88", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1020, .Spacer1Type = "84", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1031, .Spacer1Type = "85", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1042, .Spacer1Type = "86", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1053, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1064, .Spacer1Type = "88", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1103, .Spacer1Type = "84", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1115, .Spacer1Type = "85", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1127, .Spacer1Type = "86", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1139, .Spacer1Type = "87", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1151, .Spacer1Type = "88", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1185, .Spacer1Type = "84", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1198, .Spacer1Type = "85", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1211, .Spacer1Type = "86", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1224, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1237, .Spacer1Type = "88", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1268, .Spacer1Type = "84", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1282, .Spacer1Type = "85", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1296, .Spacer1Type = "86", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1310, .Spacer1Type = "87", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1324, .Spacer1Type = "88", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1350, .Spacer1Type = "84", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1365, .Spacer1Type = "85", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1380, .Spacer1Type = "86", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1395, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1410, .Spacer1Type = "88", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1432, .Spacer1Type = "84", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1448, .Spacer1Type = "85", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1464, .Spacer1Type = "86", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1480, .Spacer1Type = "87", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1496, .Spacer1Type = "88", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1515, .Spacer1Type = "84", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1532, .Spacer1Type = "85", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1549, .Spacer1Type = "86", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1566, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1583, .Spacer1Type = "88", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1597, .Spacer1Type = "84", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1615, .Spacer1Type = "85", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1633, .Spacer1Type = "86", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1651, .Spacer1Type = "87", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1669, .Spacer1Type = "88", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1680, .Spacer1Type = "84", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1699, .Spacer1Type = "85", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1718, .Spacer1Type = "86", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1737, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1756, .Spacer1Type = "88", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1762, .Spacer1Type = "84", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1782, .Spacer1Type = "85", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1802, .Spacer1Type = "86", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1822, .Spacer1Type = "87", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1842, .Spacer1Type = "88", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1844, .Spacer1Type = "84", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "85", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1886, .Spacer1Type = "86", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1907, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1927, .Spacer1Type = "84", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1928, .Spacer1Type = "88", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1949, .Spacer1Type = "85", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1971, .Spacer1Type = "86", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1993, .Spacer1Type = "87", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2009, .Spacer1Type = "84", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2015, .Spacer1Type = "88", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2032, .Spacer1Type = "85", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2055, .Spacer1Type = "86", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2078, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2092, .Spacer1Type = "84", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2101, .Spacer1Type = "88", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2116, .Spacer1Type = "85", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2140, .Spacer1Type = "86", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2164, .Spacer1Type = "87", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2174, .Spacer1Type = "84", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2188, .Spacer1Type = "88", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2199, .Spacer1Type = "85", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2224, .Spacer1Type = "86", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2249, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2256, .Spacer1Type = "84", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2274, .Spacer1Type = "88", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2282, .Spacer1Type = "85", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2308, .Spacer1Type = "86", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2334, .Spacer1Type = "87", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2339, .Spacer1Type = "84", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2360, .Spacer1Type = "88", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2366, .Spacer1Type = "85", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2393, .Spacer1Type = "86", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2420, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2421, .Spacer1Type = "84", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2447, .Spacer1Type = "88", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2449, .Spacer1Type = "85", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2477, .Spacer1Type = "86", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2504, .Spacer1Type = "84", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2505, .Spacer1Type = "87", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "85", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "88", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2562, .Spacer1Type = "86", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2586, .Spacer1Type = "84", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2591, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2616, .Spacer1Type = "85", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2620, .Spacer1Type = "88", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2646, .Spacer1Type = "86", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2668, .Spacer1Type = "84", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2676, .Spacer1Type = "87", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2699, .Spacer1Type = "85", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2706, .Spacer1Type = "88", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2730, .Spacer1Type = "86", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2751, .Spacer1Type = "84", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2761, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2783, .Spacer1Type = "85", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2792, .Spacer1Type = "88", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2815, .Spacer1Type = "86", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2833, .Spacer1Type = "84", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2847, .Spacer1Type = "87", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2866, .Spacer1Type = "85", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2879, .Spacer1Type = "88", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2899, .Spacer1Type = "86", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2916, .Spacer1Type = "84", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2932, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2950, .Spacer1Type = "85", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2965, .Spacer1Type = "88", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2984, .Spacer1Type = "86", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2998, .Spacer1Type = "84", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3018, .Spacer1Type = "87", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3033, .Spacer1Type = "85", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3052, .Spacer1Type = "88", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3068, .Spacer1Type = "86", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3080, .Spacer1Type = "84", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3103, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3116, .Spacer1Type = "85", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3138, .Spacer1Type = "88", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3152, .Spacer1Type = "86", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3163, .Spacer1Type = "84", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3188, .Spacer1Type = "87", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3200, .Spacer1Type = "85", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3224, .Spacer1Type = "88", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3237, .Spacer1Type = "86", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3245, .Spacer1Type = "84", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3274, .Spacer1Type = "87", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3283, .Spacer1Type = "85", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3311, .Spacer1Type = "88", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3321, .Spacer1Type = "86", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3328, .Spacer1Type = "84", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3359, .Spacer1Type = "87", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3367, .Spacer1Type = "85", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3397, .Spacer1Type = "88", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3406, .Spacer1Type = "86", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3410, .Spacer1Type = "84", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3445, .Spacer1Type = "87", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3450, .Spacer1Type = "85", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3484, .Spacer1Type = "88", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3490, .Spacer1Type = "86", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3492, .Spacer1Type = "84", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3530, .Spacer1Type = "87", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3533, .Spacer1Type = "85", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3570, .Spacer1Type = "88", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3574, .Spacer1Type = "86", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3575, .Spacer1Type = "84", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3615, .Spacer1Type = "87", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3617, .Spacer1Type = "85", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3656, .Spacer1Type = "88", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3657, .Spacer1Type = "84", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3659, .Spacer1Type = "86", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3700, .Spacer1Type = "85", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3701, .Spacer1Type = "87", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3740, .Spacer1Type = "84", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3743, .Spacer1Type = "86", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3743, .Spacer1Type = "88", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3784, .Spacer1Type = "85", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3786, .Spacer1Type = "87", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3822, .Spacer1Type = "84", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3828, .Spacer1Type = "86", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3829, .Spacer1Type = "88", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3867, .Spacer1Type = "85", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3872, .Spacer1Type = "87", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3904, .Spacer1Type = "84", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3912, .Spacer1Type = "86", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3916, .Spacer1Type = "88", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3950, .Spacer1Type = "85", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3957, .Spacer1Type = "87", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3987, .Spacer1Type = "84", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3996, .Spacer1Type = "86", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4002, .Spacer1Type = "88", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 4034, .Spacer1Type = "85", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4042, .Spacer1Type = "87", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4069, .Spacer1Type = "84", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4081, .Spacer1Type = "86", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4088, .Spacer1Type = "88", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4117, .Spacer1Type = "85", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4128, .Spacer1Type = "87", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4152, .Spacer1Type = "84", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4165, .Spacer1Type = "86", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4175, .Spacer1Type = "88", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4201, .Spacer1Type = "85", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4213, .Spacer1Type = "87", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4234, .Spacer1Type = "84", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4250, .Spacer1Type = "86", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4261, .Spacer1Type = "88", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4284, .Spacer1Type = "85", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4299, .Spacer1Type = "87", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4316, .Spacer1Type = "84", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4334, .Spacer1Type = "86", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4348, .Spacer1Type = "88", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4367, .Spacer1Type = "85", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4384, .Spacer1Type = "87", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4399, .Spacer1Type = "84", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4418, .Spacer1Type = "86", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4434, .Spacer1Type = "88", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4451, .Spacer1Type = "85", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4469, .Spacer1Type = "87", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4481, .Spacer1Type = "84", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4503, .Spacer1Type = "86", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4520, .Spacer1Type = "88", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4534, .Spacer1Type = "85", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4555, .Spacer1Type = "87", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4564, .Spacer1Type = "84", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4587, .Spacer1Type = "86", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4607, .Spacer1Type = "88", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4618, .Spacer1Type = "85", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4640, .Spacer1Type = "87", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4646, .Spacer1Type = "84", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4672, .Spacer1Type = "86", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4693, .Spacer1Type = "88", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4701, .Spacer1Type = "85", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4726, .Spacer1Type = "87", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4728, .Spacer1Type = "84", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4756, .Spacer1Type = "86", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4780, .Spacer1Type = "88", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4784, .Spacer1Type = "85", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4811, .Spacer1Type = "84", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4811, .Spacer1Type = "87", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4840, .Spacer1Type = "86", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4866, .Spacer1Type = "88", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4868, .Spacer1Type = "85", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4893, .Spacer1Type = "84", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4896, .Spacer1Type = "87", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4925, .Spacer1Type = "86", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4951, .Spacer1Type = "85", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4952, .Spacer1Type = "88", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4976, .Spacer1Type = "84", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4982, .Spacer1Type = "87", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 5009, .Spacer1Type = "86", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5035, .Spacer1Type = "85", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5039, .Spacer1Type = "88", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 5058, .Spacer1Type = "84", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5067, .Spacer1Type = "87", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5094, .Spacer1Type = "86", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5118, .Spacer1Type = "85", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5125, .Spacer1Type = "88", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5140, .Spacer1Type = "84", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5153, .Spacer1Type = "87", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5178, .Spacer1Type = "86", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5201, .Spacer1Type = "85", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5212, .Spacer1Type = "88", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5223, .Spacer1Type = "84", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5238, .Spacer1Type = "87", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5262, .Spacer1Type = "86", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5285, .Spacer1Type = "85", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5298, .Spacer1Type = "88", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5305, .Spacer1Type = "84", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5323, .Spacer1Type = "87", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5347, .Spacer1Type = "86", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5368, .Spacer1Type = "85", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5384, .Spacer1Type = "88", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5388, .Spacer1Type = "84", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5409, .Spacer1Type = "87", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5431, .Spacer1Type = "86", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5452, .Spacer1Type = "85", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5470, .Spacer1Type = "84", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5471, .Spacer1Type = "88", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5494, .Spacer1Type = "87", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5516, .Spacer1Type = "86", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5535, .Spacer1Type = "85", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5552, .Spacer1Type = "84", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5557, .Spacer1Type = "88", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5580, .Spacer1Type = "87", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5600, .Spacer1Type = "86", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5618, .Spacer1Type = "85", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5635, .Spacer1Type = "84", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5644, .Spacer1Type = "88", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5665, .Spacer1Type = "87", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5684, .Spacer1Type = "86", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5702, .Spacer1Type = "85", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5717, .Spacer1Type = "84", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5730, .Spacer1Type = "88", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5750, .Spacer1Type = "87", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5769, .Spacer1Type = "86", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5785, .Spacer1Type = "85", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5800, .Spacer1Type = "84", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5816, .Spacer1Type = "88", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5836, .Spacer1Type = "87", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5853, .Spacer1Type = "86", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5869, .Spacer1Type = "85", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5882, .Spacer1Type = "84", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5903, .Spacer1Type = "88", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5921, .Spacer1Type = "87", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5938, .Spacer1Type = "86", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5952, .Spacer1Type = "85", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5964, .Spacer1Type = "84", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5989, .Spacer1Type = "88", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 6007, .Spacer1Type = "87", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 6022, .Spacer1Type = "86", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 6035, .Spacer1Type = "85", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6047, .Spacer1Type = "84", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6076, .Spacer1Type = "88", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 6092, .Spacer1Type = "87", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 6106, .Spacer1Type = "86", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6119, .Spacer1Type = "85", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6129, .Spacer1Type = "84", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6162, .Spacer1Type = "88", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 6177, .Spacer1Type = "87", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6191, .Spacer1Type = "86", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6202, .Spacer1Type = "85", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6212, .Spacer1Type = "84", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6248, .Spacer1Type = "88", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 6263, .Spacer1Type = "87", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6275, .Spacer1Type = "86", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6286, .Spacer1Type = "85", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6294, .Spacer1Type = "84", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 6335, .Spacer1Type = "88", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 6348, .Spacer1Type = "87", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6360, .Spacer1Type = "86", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6369, .Spacer1Type = "85", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 6376, .Spacer1Type = "84", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 6421, .Spacer1Type = "88", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 6434, .Spacer1Type = "87", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 6444, .Spacer1Type = "86", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 6452, .Spacer1Type = "85", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 6459, .Spacer1Type = "84", .CarriersQty = 78}
    }

    Private ReadOnly Spacer89Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 103, .Spacer1Type = "76", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 175, .Spacer1Type = "74", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 176, .Spacer1Type = "75", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 177, .Spacer1Type = "76", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 178, .Spacer1Type = "77", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 180, .Spacer1Type = "78", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 248, .Spacer1Type = "74", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 250, .Spacer1Type = "75", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 252, .Spacer1Type = "76", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 254, .Spacer1Type = "77", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 256, .Spacer1Type = "78", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 320, .Spacer1Type = "74", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 323, .Spacer1Type = "75", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 326, .Spacer1Type = "76", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 329, .Spacer1Type = "77", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 333, .Spacer1Type = "78", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 393, .Spacer1Type = "74", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 397, .Spacer1Type = "75", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 401, .Spacer1Type = "76", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 405, .Spacer1Type = "77", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 409, .Spacer1Type = "78", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 465, .Spacer1Type = "74", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 470, .Spacer1Type = "75", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 475, .Spacer1Type = "76", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 480, .Spacer1Type = "77", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 486, .Spacer1Type = "78", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 537, .Spacer1Type = "74", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 543, .Spacer1Type = "75", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 549, .Spacer1Type = "76", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 555, .Spacer1Type = "77", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 562, .Spacer1Type = "78", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 610, .Spacer1Type = "74", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 617, .Spacer1Type = "75", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 624, .Spacer1Type = "76", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 631, .Spacer1Type = "77", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 639, .Spacer1Type = "78", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 682, .Spacer1Type = "74", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 690, .Spacer1Type = "75", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 698, .Spacer1Type = "76", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 706, .Spacer1Type = "77", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 715, .Spacer1Type = "78", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 755, .Spacer1Type = "74", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 764, .Spacer1Type = "75", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 773, .Spacer1Type = "76", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 782, .Spacer1Type = "77", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 792, .Spacer1Type = "78", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 827, .Spacer1Type = "74", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 837, .Spacer1Type = "75", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 847, .Spacer1Type = "76", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 857, .Spacer1Type = "77", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 868, .Spacer1Type = "78", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 899, .Spacer1Type = "74", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 910, .Spacer1Type = "75", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 921, .Spacer1Type = "76", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 932, .Spacer1Type = "77", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 945, .Spacer1Type = "78", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 972, .Spacer1Type = "74", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 984, .Spacer1Type = "75", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 996, .Spacer1Type = "76", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1008, .Spacer1Type = "77", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1021, .Spacer1Type = "78", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1044, .Spacer1Type = "74", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1057, .Spacer1Type = "75", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1070, .Spacer1Type = "76", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1083, .Spacer1Type = "77", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1098, .Spacer1Type = "78", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1117, .Spacer1Type = "74", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1131, .Spacer1Type = "75", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "76", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1159, .Spacer1Type = "77", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1174, .Spacer1Type = "78", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1189, .Spacer1Type = "74", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1204, .Spacer1Type = "75", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1219, .Spacer1Type = "76", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1234, .Spacer1Type = "77", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1251, .Spacer1Type = "78", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1261, .Spacer1Type = "74", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1277, .Spacer1Type = "75", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1293, .Spacer1Type = "76", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1309, .Spacer1Type = "77", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1327, .Spacer1Type = "78", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1334, .Spacer1Type = "74", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1351, .Spacer1Type = "75", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1368, .Spacer1Type = "76", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1385, .Spacer1Type = "77", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1404, .Spacer1Type = "78", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1406, .Spacer1Type = "74", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1424, .Spacer1Type = "75", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1442, .Spacer1Type = "76", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1460, .Spacer1Type = "77", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1479, .Spacer1Type = "74", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1480, .Spacer1Type = "78", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1498, .Spacer1Type = "75", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1517, .Spacer1Type = "76", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1536, .Spacer1Type = "77", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1551, .Spacer1Type = "74", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1557, .Spacer1Type = "78", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1571, .Spacer1Type = "75", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1591, .Spacer1Type = "76", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1611, .Spacer1Type = "77", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1623, .Spacer1Type = "74", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1633, .Spacer1Type = "78", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1644, .Spacer1Type = "75", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1665, .Spacer1Type = "76", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1686, .Spacer1Type = "77", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1696, .Spacer1Type = "74", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1710, .Spacer1Type = "78", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1718, .Spacer1Type = "75", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1740, .Spacer1Type = "76", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1762, .Spacer1Type = "77", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1768, .Spacer1Type = "74", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1786, .Spacer1Type = "78", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1791, .Spacer1Type = "75", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1814, .Spacer1Type = "76", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1837, .Spacer1Type = "77", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1841, .Spacer1Type = "74", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1863, .Spacer1Type = "78", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "75", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1889, .Spacer1Type = "76", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1913, .Spacer1Type = "74", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1913, .Spacer1Type = "77", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1938, .Spacer1Type = "75", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1939, .Spacer1Type = "78", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1963, .Spacer1Type = "76", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1985, .Spacer1Type = "74", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1988, .Spacer1Type = "77", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2011, .Spacer1Type = "75", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2016, .Spacer1Type = "78", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2037, .Spacer1Type = "76", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2058, .Spacer1Type = "74", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2063, .Spacer1Type = "77", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2085, .Spacer1Type = "75", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2092, .Spacer1Type = "78", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2112, .Spacer1Type = "76", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2130, .Spacer1Type = "74", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2139, .Spacer1Type = "77", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2158, .Spacer1Type = "75", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2169, .Spacer1Type = "78", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2186, .Spacer1Type = "76", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2203, .Spacer1Type = "74", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2214, .Spacer1Type = "77", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2232, .Spacer1Type = "75", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2245, .Spacer1Type = "78", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2261, .Spacer1Type = "76", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2275, .Spacer1Type = "74", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2290, .Spacer1Type = "77", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2305, .Spacer1Type = "75", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2322, .Spacer1Type = "78", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2335, .Spacer1Type = "76", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2347, .Spacer1Type = "74", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2365, .Spacer1Type = "77", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2378, .Spacer1Type = "75", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2398, .Spacer1Type = "78", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2409, .Spacer1Type = "76", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2420, .Spacer1Type = "74", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2440, .Spacer1Type = "77", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2452, .Spacer1Type = "75", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2475, .Spacer1Type = "78", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2484, .Spacer1Type = "76", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2492, .Spacer1Type = "74", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2516, .Spacer1Type = "77", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2525, .Spacer1Type = "75", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2551, .Spacer1Type = "78", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2558, .Spacer1Type = "76", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2565, .Spacer1Type = "74", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2591, .Spacer1Type = "77", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2599, .Spacer1Type = "75", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2628, .Spacer1Type = "78", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2633, .Spacer1Type = "76", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2637, .Spacer1Type = "74", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2667, .Spacer1Type = "77", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2672, .Spacer1Type = "75", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2704, .Spacer1Type = "78", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2707, .Spacer1Type = "76", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2709, .Spacer1Type = "74", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2742, .Spacer1Type = "77", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2745, .Spacer1Type = "75", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2781, .Spacer1Type = "76", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2781, .Spacer1Type = "78", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2782, .Spacer1Type = "74", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2817, .Spacer1Type = "77", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2819, .Spacer1Type = "75", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2854, .Spacer1Type = "74", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2856, .Spacer1Type = "76", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2857, .Spacer1Type = "78", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2892, .Spacer1Type = "75", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2893, .Spacer1Type = "77", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2927, .Spacer1Type = "74", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2930, .Spacer1Type = "76", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2934, .Spacer1Type = "78", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2966, .Spacer1Type = "75", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2968, .Spacer1Type = "77", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2999, .Spacer1Type = "74", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3005, .Spacer1Type = "76", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3010, .Spacer1Type = "78", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3039, .Spacer1Type = "75", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3044, .Spacer1Type = "77", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3071, .Spacer1Type = "74", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3079, .Spacer1Type = "76", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3087, .Spacer1Type = "78", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3112, .Spacer1Type = "75", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3119, .Spacer1Type = "77", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3144, .Spacer1Type = "74", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3153, .Spacer1Type = "76", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3163, .Spacer1Type = "78", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3186, .Spacer1Type = "75", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3194, .Spacer1Type = "77", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3216, .Spacer1Type = "74", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3228, .Spacer1Type = "76", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3240, .Spacer1Type = "78", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3259, .Spacer1Type = "75", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3270, .Spacer1Type = "77", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3289, .Spacer1Type = "74", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3302, .Spacer1Type = "76", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3316, .Spacer1Type = "78", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3333, .Spacer1Type = "75", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3345, .Spacer1Type = "77", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3361, .Spacer1Type = "74", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3377, .Spacer1Type = "76", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3393, .Spacer1Type = "78", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3406, .Spacer1Type = "75", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3421, .Spacer1Type = "77", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3433, .Spacer1Type = "74", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3451, .Spacer1Type = "76", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3469, .Spacer1Type = "78", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3479, .Spacer1Type = "75", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3496, .Spacer1Type = "77", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3506, .Spacer1Type = "74", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3525, .Spacer1Type = "76", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3546, .Spacer1Type = "78", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3553, .Spacer1Type = "75", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3571, .Spacer1Type = "77", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3578, .Spacer1Type = "74", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3600, .Spacer1Type = "76", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3622, .Spacer1Type = "78", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3626, .Spacer1Type = "75", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3647, .Spacer1Type = "77", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3651, .Spacer1Type = "74", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3674, .Spacer1Type = "76", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3699, .Spacer1Type = "78", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3700, .Spacer1Type = "75", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3722, .Spacer1Type = "77", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3723, .Spacer1Type = "74", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3749, .Spacer1Type = "76", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3773, .Spacer1Type = "75", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3775, .Spacer1Type = "78", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3795, .Spacer1Type = "74", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3798, .Spacer1Type = "77", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3823, .Spacer1Type = "76", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3846, .Spacer1Type = "75", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3852, .Spacer1Type = "78", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3868, .Spacer1Type = "74", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3873, .Spacer1Type = "77", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3897, .Spacer1Type = "76", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3920, .Spacer1Type = "75", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3928, .Spacer1Type = "78", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3940, .Spacer1Type = "74", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3948, .Spacer1Type = "77", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3972, .Spacer1Type = "76", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 3993, .Spacer1Type = "75", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4005, .Spacer1Type = "78", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4013, .Spacer1Type = "74", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4024, .Spacer1Type = "77", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4046, .Spacer1Type = "76", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4067, .Spacer1Type = "75", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4081, .Spacer1Type = "78", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4085, .Spacer1Type = "74", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4099, .Spacer1Type = "77", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4121, .Spacer1Type = "76", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4140, .Spacer1Type = "75", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4157, .Spacer1Type = "74", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4158, .Spacer1Type = "78", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4175, .Spacer1Type = "77", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4195, .Spacer1Type = "76", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4213, .Spacer1Type = "75", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4230, .Spacer1Type = "74", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4234, .Spacer1Type = "78", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4250, .Spacer1Type = "77", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4269, .Spacer1Type = "76", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4287, .Spacer1Type = "75", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4302, .Spacer1Type = "74", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4311, .Spacer1Type = "78", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4325, .Spacer1Type = "77", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4344, .Spacer1Type = "76", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4360, .Spacer1Type = "75", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "74", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4387, .Spacer1Type = "78", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4401, .Spacer1Type = "77", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4418, .Spacer1Type = "76", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4434, .Spacer1Type = "75", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4447, .Spacer1Type = "74", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4464, .Spacer1Type = "78", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4476, .Spacer1Type = "77", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4493, .Spacer1Type = "76", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4507, .Spacer1Type = "75", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4519, .Spacer1Type = "74", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4540, .Spacer1Type = "78", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4552, .Spacer1Type = "77", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4567, .Spacer1Type = "76", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4580, .Spacer1Type = "75", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4592, .Spacer1Type = "74", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4617, .Spacer1Type = "78", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4627, .Spacer1Type = "77", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4641, .Spacer1Type = "76", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4654, .Spacer1Type = "75", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4664, .Spacer1Type = "74", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4693, .Spacer1Type = "78", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4702, .Spacer1Type = "77", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4716, .Spacer1Type = "76", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4727, .Spacer1Type = "75", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4737, .Spacer1Type = "74", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4770, .Spacer1Type = "78", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4778, .Spacer1Type = "77", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4790, .Spacer1Type = "76", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4801, .Spacer1Type = "75", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4809, .Spacer1Type = "74", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4846, .Spacer1Type = "78", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4853, .Spacer1Type = "77", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4865, .Spacer1Type = "76", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4874, .Spacer1Type = "75", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 4881, .Spacer1Type = "74", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4923, .Spacer1Type = "78", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4929, .Spacer1Type = "77", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 4939, .Spacer1Type = "76", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4947, .Spacer1Type = "75", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 4954, .Spacer1Type = "74", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 4999, .Spacer1Type = "78", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 5004, .Spacer1Type = "77", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5013, .Spacer1Type = "76", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5021, .Spacer1Type = "75", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5026, .Spacer1Type = "74", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5076, .Spacer1Type = "78", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 5079, .Spacer1Type = "77", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5088, .Spacer1Type = "76", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5094, .Spacer1Type = "75", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5099, .Spacer1Type = "74", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5152, .Spacer1Type = "78", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 5155, .Spacer1Type = "77", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5162, .Spacer1Type = "76", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5168, .Spacer1Type = "75", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5171, .Spacer1Type = "74", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5229, .Spacer1Type = "78", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 5230, .Spacer1Type = "77", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5237, .Spacer1Type = "76", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5241, .Spacer1Type = "75", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5243, .Spacer1Type = "74", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5305, .Spacer1Type = "78", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 5306, .Spacer1Type = "77", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5311, .Spacer1Type = "76", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5314, .Spacer1Type = "75", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5316, .Spacer1Type = "74", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5381, .Spacer1Type = "77", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5382, .Spacer1Type = "78", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 5385, .Spacer1Type = "76", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5388, .Spacer1Type = "74", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5388, .Spacer1Type = "75", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5456, .Spacer1Type = "77", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5458, .Spacer1Type = "78", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 5460, .Spacer1Type = "76", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5461, .Spacer1Type = "74", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5461, .Spacer1Type = "75", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5532, .Spacer1Type = "77", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5533, .Spacer1Type = "74", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5534, .Spacer1Type = "76", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5535, .Spacer1Type = "75", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5535, .Spacer1Type = "78", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 5605, .Spacer1Type = "74", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5607, .Spacer1Type = "77", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5608, .Spacer1Type = "75", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5609, .Spacer1Type = "76", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 5611, .Spacer1Type = "78", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 5678, .Spacer1Type = "74", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5681, .Spacer1Type = "75", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5683, .Spacer1Type = "76", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 5683, .Spacer1Type = "77", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5688, .Spacer1Type = "78", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 5750, .Spacer1Type = "74", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5755, .Spacer1Type = "75", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5757, .Spacer1Type = "76", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 5764, .Spacer1Type = "78", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 5823, .Spacer1Type = "74", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5828, .Spacer1Type = "75", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5832, .Spacer1Type = "76", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 5895, .Spacer1Type = "74", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 5902, .Spacer1Type = "75", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5906, .Spacer1Type = "76", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 5975, .Spacer1Type = "75", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 5981, .Spacer1Type = "76", .CarriersQty = 81}
    }

    Private ReadOnly Spacer63Metal As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 77, .Spacer1Type = "51", .CarriersQty = 1},
        New SpacerInfo With {.MaxWidth = 126, .Spacer1Type = "51", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 176, .Spacer1Type = "51", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 225, .Spacer1Type = "51", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 275, .Spacer1Type = "51", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 324, .Spacer1Type = "51", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 373, .Spacer1Type = "51", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 423, .Spacer1Type = "51", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 472, .Spacer1Type = "51", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 522, .Spacer1Type = "51", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 571, .Spacer1Type = "51", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 620, .Spacer1Type = "51", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 670, .Spacer1Type = "51", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 719, .Spacer1Type = "51", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 769, .Spacer1Type = "51", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 818, .Spacer1Type = "51", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 867, .Spacer1Type = "51", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 917, .Spacer1Type = "51", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 966, .Spacer1Type = "51", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1016, .Spacer1Type = "51", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1065, .Spacer1Type = "51", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1114, .Spacer1Type = "51", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1164, .Spacer1Type = "51", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1213, .Spacer1Type = "51", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1263, .Spacer1Type = "51", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1312, .Spacer1Type = "51", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1361, .Spacer1Type = "51", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1411, .Spacer1Type = "51", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 1460, .Spacer1Type = "51", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 1510, .Spacer1Type = "51", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 1559, .Spacer1Type = "51", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 1608, .Spacer1Type = "51", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 1658, .Spacer1Type = "51", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 1707, .Spacer1Type = "51", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 1757, .Spacer1Type = "51", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 1806, .Spacer1Type = "51", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 1855, .Spacer1Type = "51", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 1905, .Spacer1Type = "51", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 1954, .Spacer1Type = "51", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2004, .Spacer1Type = "51", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2053, .Spacer1Type = "51", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 2102, .Spacer1Type = "51", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 2152, .Spacer1Type = "51", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 2201, .Spacer1Type = "51", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 2251, .Spacer1Type = "51", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 2300, .Spacer1Type = "51", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 2349, .Spacer1Type = "51", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 2399, .Spacer1Type = "51", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 2448, .Spacer1Type = "51", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 2498, .Spacer1Type = "51", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 2547, .Spacer1Type = "51", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 2596, .Spacer1Type = "51", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 2646, .Spacer1Type = "51", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 2695, .Spacer1Type = "51", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 2745, .Spacer1Type = "51", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 2794, .Spacer1Type = "51", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 2843, .Spacer1Type = "51", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 2893, .Spacer1Type = "51", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 2942, .Spacer1Type = "51", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 2992, .Spacer1Type = "51", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 3041, .Spacer1Type = "51", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 3090, .Spacer1Type = "51", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 3140, .Spacer1Type = "51", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 3189, .Spacer1Type = "51", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 3239, .Spacer1Type = "51", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 3288, .Spacer1Type = "51", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 3337, .Spacer1Type = "51", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 3387, .Spacer1Type = "51", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 3436, .Spacer1Type = "51", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 3486, .Spacer1Type = "51", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 3535, .Spacer1Type = "51", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 3584, .Spacer1Type = "51", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 3634, .Spacer1Type = "51", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 3683, .Spacer1Type = "51", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 3733, .Spacer1Type = "51", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 3782, .Spacer1Type = "51", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 3831, .Spacer1Type = "51", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 3881, .Spacer1Type = "51", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 3930, .Spacer1Type = "51", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 3980, .Spacer1Type = "51", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 4029, .Spacer1Type = "51", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 4078, .Spacer1Type = "51", .CarriersQty = 82},
        New SpacerInfo With {.MaxWidth = 4128, .Spacer1Type = "51", .CarriersQty = 83},
        New SpacerInfo With {.MaxWidth = 4177, .Spacer1Type = "51", .CarriersQty = 84},
        New SpacerInfo With {.MaxWidth = 4227, .Spacer1Type = "51", .CarriersQty = 85},
        New SpacerInfo With {.MaxWidth = 4276, .Spacer1Type = "51", .CarriersQty = 86},
        New SpacerInfo With {.MaxWidth = 4325, .Spacer1Type = "51", .CarriersQty = 87},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "51", .CarriersQty = 88}
    }

    Private ReadOnly Spacer127Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 555, .Spacer1Type = "106", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 575, .Spacer1Type = "110", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 585, .Spacer1Type = "112", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 595, .Spacer1Type = "114", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 610, .Spacer1Type = "117", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 661, .Spacer1Type = "106", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 685, .Spacer1Type = "110", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 697, .Spacer1Type = "112", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 709, .Spacer1Type = "114", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 727, .Spacer1Type = "117", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 767, .Spacer1Type = "106", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 795, .Spacer1Type = "110", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 809, .Spacer1Type = "112", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 823, .Spacer1Type = "114", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 844, .Spacer1Type = "117", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 873, .Spacer1Type = "106", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 905, .Spacer1Type = "110", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 921, .Spacer1Type = "112", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 937, .Spacer1Type = "114", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 961, .Spacer1Type = "117", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 979, .Spacer1Type = "106", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1015, .Spacer1Type = "110", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1033, .Spacer1Type = "112", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1051, .Spacer1Type = "114", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1078, .Spacer1Type = "117", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1085, .Spacer1Type = "106", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1125, .Spacer1Type = "110", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "112", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1165, .Spacer1Type = "114", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1191, .Spacer1Type = "106", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1195, .Spacer1Type = "117", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1235, .Spacer1Type = "110", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1257, .Spacer1Type = "112", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1279, .Spacer1Type = "114", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1297, .Spacer1Type = "106", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1312, .Spacer1Type = "117", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1345, .Spacer1Type = "110", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1369, .Spacer1Type = "112", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1393, .Spacer1Type = "114", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1403, .Spacer1Type = "106", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1429, .Spacer1Type = "117", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1455, .Spacer1Type = "110", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1481, .Spacer1Type = "112", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1507, .Spacer1Type = "114", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1509, .Spacer1Type = "106", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1546, .Spacer1Type = "117", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1565, .Spacer1Type = "110", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1593, .Spacer1Type = "112", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1615, .Spacer1Type = "106", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1621, .Spacer1Type = "114", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1663, .Spacer1Type = "117", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1675, .Spacer1Type = "110", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1705, .Spacer1Type = "112", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1721, .Spacer1Type = "106", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1735, .Spacer1Type = "114", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1780, .Spacer1Type = "117", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1785, .Spacer1Type = "110", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1817, .Spacer1Type = "112", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1827, .Spacer1Type = "106", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1849, .Spacer1Type = "114", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1895, .Spacer1Type = "110", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1897, .Spacer1Type = "117", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1929, .Spacer1Type = "112", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1933, .Spacer1Type = "106", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1963, .Spacer1Type = "114", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2005, .Spacer1Type = "110", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2014, .Spacer1Type = "117", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2039, .Spacer1Type = "106", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2041, .Spacer1Type = "112", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2077, .Spacer1Type = "114", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2115, .Spacer1Type = "110", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2131, .Spacer1Type = "117", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2145, .Spacer1Type = "106", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2153, .Spacer1Type = "112", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2191, .Spacer1Type = "114", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2225, .Spacer1Type = "110", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2248, .Spacer1Type = "117", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2251, .Spacer1Type = "106", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2265, .Spacer1Type = "112", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2305, .Spacer1Type = "114", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2335, .Spacer1Type = "110", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2357, .Spacer1Type = "106", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2365, .Spacer1Type = "117", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2377, .Spacer1Type = "112", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2419, .Spacer1Type = "114", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2445, .Spacer1Type = "110", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2463, .Spacer1Type = "106", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2482, .Spacer1Type = "117", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2489, .Spacer1Type = "112", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2533, .Spacer1Type = "114", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2555, .Spacer1Type = "110", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2569, .Spacer1Type = "106", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2599, .Spacer1Type = "117", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2601, .Spacer1Type = "112", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2647, .Spacer1Type = "114", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2665, .Spacer1Type = "110", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2675, .Spacer1Type = "106", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2713, .Spacer1Type = "112", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2716, .Spacer1Type = "117", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2761, .Spacer1Type = "114", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2775, .Spacer1Type = "110", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2781, .Spacer1Type = "106", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2825, .Spacer1Type = "112", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2833, .Spacer1Type = "117", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2875, .Spacer1Type = "114", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2885, .Spacer1Type = "110", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2887, .Spacer1Type = "106", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2937, .Spacer1Type = "112", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2950, .Spacer1Type = "117", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2989, .Spacer1Type = "114", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2993, .Spacer1Type = "106", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2995, .Spacer1Type = "110", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3049, .Spacer1Type = "112", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3067, .Spacer1Type = "117", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3099, .Spacer1Type = "106", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3103, .Spacer1Type = "114", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3105, .Spacer1Type = "110", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3161, .Spacer1Type = "112", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3184, .Spacer1Type = "117", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3205, .Spacer1Type = "106", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3215, .Spacer1Type = "110", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3217, .Spacer1Type = "114", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3273, .Spacer1Type = "112", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3301, .Spacer1Type = "117", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3311, .Spacer1Type = "106", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3325, .Spacer1Type = "110", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3331, .Spacer1Type = "114", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3385, .Spacer1Type = "112", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3417, .Spacer1Type = "106", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3418, .Spacer1Type = "117", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3435, .Spacer1Type = "110", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3445, .Spacer1Type = "114", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3497, .Spacer1Type = "112", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3523, .Spacer1Type = "106", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3535, .Spacer1Type = "117", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3545, .Spacer1Type = "110", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3559, .Spacer1Type = "114", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3609, .Spacer1Type = "112", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3629, .Spacer1Type = "106", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3652, .Spacer1Type = "117", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3655, .Spacer1Type = "110", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3673, .Spacer1Type = "114", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3721, .Spacer1Type = "112", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3735, .Spacer1Type = "106", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3765, .Spacer1Type = "110", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3769, .Spacer1Type = "117", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3787, .Spacer1Type = "114", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3833, .Spacer1Type = "112", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3841, .Spacer1Type = "106", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3875, .Spacer1Type = "110", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3886, .Spacer1Type = "117", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3901, .Spacer1Type = "114", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3945, .Spacer1Type = "112", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3947, .Spacer1Type = "106", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3985, .Spacer1Type = "110", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4003, .Spacer1Type = "117", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 4015, .Spacer1Type = "114", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4053, .Spacer1Type = "106", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4057, .Spacer1Type = "112", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4095, .Spacer1Type = "110", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4120, .Spacer1Type = "117", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4129, .Spacer1Type = "114", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4159, .Spacer1Type = "106", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4169, .Spacer1Type = "112", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4205, .Spacer1Type = "110", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4237, .Spacer1Type = "117", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4243, .Spacer1Type = "114", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4265, .Spacer1Type = "106", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4281, .Spacer1Type = "112", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4315, .Spacer1Type = "110", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4354, .Spacer1Type = "117", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 4357, .Spacer1Type = "114", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4371, .Spacer1Type = "106", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4393, .Spacer1Type = "112", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4425, .Spacer1Type = "110", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4471, .Spacer1Type = "114", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4471, .Spacer1Type = "117", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 4477, .Spacer1Type = "106", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4505, .Spacer1Type = "112", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4535, .Spacer1Type = "110", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4583, .Spacer1Type = "106", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4585, .Spacer1Type = "114", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4588, .Spacer1Type = "117", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 4617, .Spacer1Type = "112", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4645, .Spacer1Type = "110", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4689, .Spacer1Type = "106", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4699, .Spacer1Type = "114", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4705, .Spacer1Type = "117", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 4729, .Spacer1Type = "112", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4755, .Spacer1Type = "110", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4795, .Spacer1Type = "106", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 4813, .Spacer1Type = "114", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4822, .Spacer1Type = "117", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 4841, .Spacer1Type = "112", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4865, .Spacer1Type = "110", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4901, .Spacer1Type = "106", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 4927, .Spacer1Type = "114", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 4939, .Spacer1Type = "117", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 4953, .Spacer1Type = "112", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 4975, .Spacer1Type = "110", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5007, .Spacer1Type = "106", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5041, .Spacer1Type = "114", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5056, .Spacer1Type = "117", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 5065, .Spacer1Type = "112", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5085, .Spacer1Type = "110", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5113, .Spacer1Type = "106", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5155, .Spacer1Type = "114", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5173, .Spacer1Type = "117", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 5177, .Spacer1Type = "112", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5195, .Spacer1Type = "110", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5219, .Spacer1Type = "106", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5269, .Spacer1Type = "114", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5289, .Spacer1Type = "112", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5290, .Spacer1Type = "117", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 5305, .Spacer1Type = "110", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5325, .Spacer1Type = "106", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5383, .Spacer1Type = "114", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5401, .Spacer1Type = "112", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5407, .Spacer1Type = "117", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 5415, .Spacer1Type = "110", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5431, .Spacer1Type = "106", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5497, .Spacer1Type = "114", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5513, .Spacer1Type = "112", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5524, .Spacer1Type = "117", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 5525, .Spacer1Type = "110", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5611, .Spacer1Type = "114", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5625, .Spacer1Type = "112", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5635, .Spacer1Type = "110", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5641, .Spacer1Type = "117", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 5725, .Spacer1Type = "114", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5737, .Spacer1Type = "112", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5758, .Spacer1Type = "117", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 5839, .Spacer1Type = "114", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 5875, .Spacer1Type = "117", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 5992, .Spacer1Type = "117", .CarriersQty = 51}
    }

    Private ReadOnly Spacer100Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 425, .Spacer1Type = "80", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 435, .Spacer1Type = "82", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 460, .Spacer1Type = "87", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 505, .Spacer1Type = "80", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 517, .Spacer1Type = "82", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 547, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 585, .Spacer1Type = "80", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 599, .Spacer1Type = "82", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 634, .Spacer1Type = "87", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 665, .Spacer1Type = "80", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 681, .Spacer1Type = "82", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 721, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 745, .Spacer1Type = "80", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 763, .Spacer1Type = "82", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 808, .Spacer1Type = "87", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 825, .Spacer1Type = "80", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 845, .Spacer1Type = "82", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 895, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 905, .Spacer1Type = "80", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 927, .Spacer1Type = "82", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 982, .Spacer1Type = "87", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 985, .Spacer1Type = "80", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1009, .Spacer1Type = "82", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1065, .Spacer1Type = "80", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1069, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1091, .Spacer1Type = "82", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "80", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1156, .Spacer1Type = "87", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1173, .Spacer1Type = "82", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1225, .Spacer1Type = "80", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1243, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1255, .Spacer1Type = "82", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1305, .Spacer1Type = "80", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1330, .Spacer1Type = "87", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1337, .Spacer1Type = "82", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1385, .Spacer1Type = "80", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1417, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1419, .Spacer1Type = "82", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1465, .Spacer1Type = "80", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1501, .Spacer1Type = "82", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1504, .Spacer1Type = "87", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1545, .Spacer1Type = "80", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1583, .Spacer1Type = "82", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1591, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1625, .Spacer1Type = "80", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1665, .Spacer1Type = "82", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1678, .Spacer1Type = "87", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1705, .Spacer1Type = "80", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1747, .Spacer1Type = "82", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1765, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1785, .Spacer1Type = "80", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1829, .Spacer1Type = "82", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1852, .Spacer1Type = "87", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1865, .Spacer1Type = "80", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1911, .Spacer1Type = "82", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1939, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1945, .Spacer1Type = "80", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1993, .Spacer1Type = "82", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2025, .Spacer1Type = "80", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2026, .Spacer1Type = "87", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2075, .Spacer1Type = "82", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2105, .Spacer1Type = "80", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2113, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2157, .Spacer1Type = "82", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2185, .Spacer1Type = "80", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "87", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2239, .Spacer1Type = "82", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2265, .Spacer1Type = "80", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2287, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2321, .Spacer1Type = "82", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2345, .Spacer1Type = "80", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2374, .Spacer1Type = "87", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2403, .Spacer1Type = "82", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2425, .Spacer1Type = "80", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2461, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2485, .Spacer1Type = "82", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2505, .Spacer1Type = "80", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2548, .Spacer1Type = "87", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2567, .Spacer1Type = "82", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2585, .Spacer1Type = "80", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2635, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2649, .Spacer1Type = "82", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2665, .Spacer1Type = "80", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2722, .Spacer1Type = "87", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2731, .Spacer1Type = "82", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2745, .Spacer1Type = "80", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2809, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2813, .Spacer1Type = "82", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2825, .Spacer1Type = "80", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2895, .Spacer1Type = "82", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2896, .Spacer1Type = "87", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2905, .Spacer1Type = "80", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2977, .Spacer1Type = "82", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2983, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2985, .Spacer1Type = "80", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3059, .Spacer1Type = "82", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3065, .Spacer1Type = "80", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3070, .Spacer1Type = "87", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3141, .Spacer1Type = "82", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3145, .Spacer1Type = "80", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3157, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 3223, .Spacer1Type = "82", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3225, .Spacer1Type = "80", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3244, .Spacer1Type = "87", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 3305, .Spacer1Type = "80", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3305, .Spacer1Type = "82", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3331, .Spacer1Type = "87", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 3385, .Spacer1Type = "80", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3387, .Spacer1Type = "82", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3418, .Spacer1Type = "87", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3465, .Spacer1Type = "80", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3469, .Spacer1Type = "82", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3505, .Spacer1Type = "87", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3545, .Spacer1Type = "80", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3551, .Spacer1Type = "82", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3592, .Spacer1Type = "87", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3625, .Spacer1Type = "80", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3633, .Spacer1Type = "82", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3679, .Spacer1Type = "87", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3705, .Spacer1Type = "80", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3715, .Spacer1Type = "82", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3766, .Spacer1Type = "87", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3785, .Spacer1Type = "80", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3797, .Spacer1Type = "82", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3853, .Spacer1Type = "87", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3865, .Spacer1Type = "80", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3879, .Spacer1Type = "82", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3940, .Spacer1Type = "87", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3945, .Spacer1Type = "80", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3961, .Spacer1Type = "82", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4025, .Spacer1Type = "80", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4025, .Spacer1Type = "80", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4027, .Spacer1Type = "87", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 4043, .Spacer1Type = "82", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4105, .Spacer1Type = "80", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4114, .Spacer1Type = "87", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 4125, .Spacer1Type = "82", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4125, .Spacer1Type = "82", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4201, .Spacer1Type = "87", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 4207, .Spacer1Type = "82", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4265, .Spacer1Type = "80", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4288, .Spacer1Type = "87", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 4345, .Spacer1Type = "80", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4371, .Spacer1Type = "82", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "87", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "87", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 4425, .Spacer1Type = "80", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4453, .Spacer1Type = "82", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4462, .Spacer1Type = "87", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 4505, .Spacer1Type = "80", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4535, .Spacer1Type = "82", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4585, .Spacer1Type = "80", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4617, .Spacer1Type = "82", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4636, .Spacer1Type = "87", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4665, .Spacer1Type = "80", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4699, .Spacer1Type = "82", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4723, .Spacer1Type = "87", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4745, .Spacer1Type = "80", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4781, .Spacer1Type = "82", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4810, .Spacer1Type = "87", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4825, .Spacer1Type = "80", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4863, .Spacer1Type = "82", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4897, .Spacer1Type = "87", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4905, .Spacer1Type = "80", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4945, .Spacer1Type = "82", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4984, .Spacer1Type = "87", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4985, .Spacer1Type = "80", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5027, .Spacer1Type = "82", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5065, .Spacer1Type = "80", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5071, .Spacer1Type = "87", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 5109, .Spacer1Type = "82", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5145, .Spacer1Type = "80", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5158, .Spacer1Type = "87", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 5191, .Spacer1Type = "82", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5245, .Spacer1Type = "87", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 5273, .Spacer1Type = "82", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 5332, .Spacer1Type = "87", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 5419, .Spacer1Type = "87", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 5506, .Spacer1Type = "87", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 5593, .Spacer1Type = "87", .CarriersQty = 64}
    }

    Private ReadOnly Spacer89Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 390, .Spacer1Type = "73", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 400, .Spacer1Type = "75", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 463, .Spacer1Type = "73", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 475, .Spacer1Type = "75", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 536, .Spacer1Type = "73", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 550, .Spacer1Type = "75", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 609, .Spacer1Type = "73", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 625, .Spacer1Type = "75", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 682, .Spacer1Type = "73", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 700, .Spacer1Type = "75", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 755, .Spacer1Type = "73", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 775, .Spacer1Type = "75", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 828, .Spacer1Type = "73", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 850, .Spacer1Type = "75", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 901, .Spacer1Type = "73", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 925, .Spacer1Type = "75", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 974, .Spacer1Type = "73", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1000, .Spacer1Type = "75", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1047, .Spacer1Type = "73", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1075, .Spacer1Type = "75", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1120, .Spacer1Type = "73", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1150, .Spacer1Type = "75", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1193, .Spacer1Type = "73", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1225, .Spacer1Type = "75", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1266, .Spacer1Type = "73", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1300, .Spacer1Type = "75", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1339, .Spacer1Type = "73", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1375, .Spacer1Type = "75", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1412, .Spacer1Type = "73", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1450, .Spacer1Type = "75", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1485, .Spacer1Type = "73", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1525, .Spacer1Type = "75", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1558, .Spacer1Type = "73", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1600, .Spacer1Type = "75", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1631, .Spacer1Type = "73", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1675, .Spacer1Type = "75", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1704, .Spacer1Type = "73", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1750, .Spacer1Type = "75", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1777, .Spacer1Type = "73", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1825, .Spacer1Type = "75", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1850, .Spacer1Type = "73", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1900, .Spacer1Type = "75", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1923, .Spacer1Type = "73", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1975, .Spacer1Type = "75", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1996, .Spacer1Type = "73", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2050, .Spacer1Type = "75", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2069, .Spacer1Type = "73", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2125, .Spacer1Type = "75", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2142, .Spacer1Type = "73", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "75", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2215, .Spacer1Type = "73", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2275, .Spacer1Type = "75", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2288, .Spacer1Type = "73", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2350, .Spacer1Type = "75", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2361, .Spacer1Type = "73", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2425, .Spacer1Type = "75", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2434, .Spacer1Type = "73", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2500, .Spacer1Type = "75", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2507, .Spacer1Type = "73", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2575, .Spacer1Type = "75", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2580, .Spacer1Type = "73", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2650, .Spacer1Type = "75", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2653, .Spacer1Type = "73", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2725, .Spacer1Type = "75", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2726, .Spacer1Type = "73", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2799, .Spacer1Type = "73", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2800, .Spacer1Type = "75", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 2872, .Spacer1Type = "73", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 2875, .Spacer1Type = "75", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 2945, .Spacer1Type = "73", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 2950, .Spacer1Type = "75", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 3018, .Spacer1Type = "73", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3025, .Spacer1Type = "75", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 3091, .Spacer1Type = "73", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3100, .Spacer1Type = "75", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 3164, .Spacer1Type = "73", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3175, .Spacer1Type = "75", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 3237, .Spacer1Type = "73", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3250, .Spacer1Type = "75", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 3310, .Spacer1Type = "73", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3325, .Spacer1Type = "75", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 3383, .Spacer1Type = "73", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3400, .Spacer1Type = "75", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 3456, .Spacer1Type = "73", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3475, .Spacer1Type = "75", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 3529, .Spacer1Type = "73", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3550, .Spacer1Type = "75", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 3602, .Spacer1Type = "73", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3625, .Spacer1Type = "75", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 3675, .Spacer1Type = "73", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3675, .Spacer1Type = "73", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3700, .Spacer1Type = "75", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 3748, .Spacer1Type = "73", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3775, .Spacer1Type = "75", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 3775, .Spacer1Type = "75", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 3850, .Spacer1Type = "75", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 3894, .Spacer1Type = "73", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 3967, .Spacer1Type = "73", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4000, .Spacer1Type = "75", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 4040, .Spacer1Type = "73", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4075, .Spacer1Type = "75", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 4113, .Spacer1Type = "73", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4150, .Spacer1Type = "75", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 4186, .Spacer1Type = "73", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4225, .Spacer1Type = "75", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 4259, .Spacer1Type = "73", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4300, .Spacer1Type = "75", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 4332, .Spacer1Type = "73", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4375, .Spacer1Type = "75", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 4405, .Spacer1Type = "73", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4450, .Spacer1Type = "75", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 4478, .Spacer1Type = "73", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4525, .Spacer1Type = "75", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 4551, .Spacer1Type = "73", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4600, .Spacer1Type = "75", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 4624, .Spacer1Type = "73", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4675, .Spacer1Type = "75", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 4697, .Spacer1Type = "73", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 4750, .Spacer1Type = "75", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 4825, .Spacer1Type = "75", .CarriersQty = 64}
    }

    Private ReadOnly Spacer63Tiltrack As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 260, .Spacer1Type = "47", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 307, .Spacer1Type = "47", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 354, .Spacer1Type = "47", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 401, .Spacer1Type = "47", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 448, .Spacer1Type = "47", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 495, .Spacer1Type = "47", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 542, .Spacer1Type = "47", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 589, .Spacer1Type = "47", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 636, .Spacer1Type = "47", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 683, .Spacer1Type = "47", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 730, .Spacer1Type = "47", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 777, .Spacer1Type = "47", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 824, .Spacer1Type = "47", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 871, .Spacer1Type = "47", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 918, .Spacer1Type = "47", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 965, .Spacer1Type = "47", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1012, .Spacer1Type = "47", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1059, .Spacer1Type = "47", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1106, .Spacer1Type = "47", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1153, .Spacer1Type = "47", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1200, .Spacer1Type = "47", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1247, .Spacer1Type = "47", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1294, .Spacer1Type = "47", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1341, .Spacer1Type = "47", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 1388, .Spacer1Type = "47", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 1435, .Spacer1Type = "47", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 1482, .Spacer1Type = "47", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 1529, .Spacer1Type = "47", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 1576, .Spacer1Type = "47", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 1623, .Spacer1Type = "47", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 1670, .Spacer1Type = "47", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 1717, .Spacer1Type = "47", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 1764, .Spacer1Type = "47", .CarriersQty = 37},
        New SpacerInfo With {.MaxWidth = 1811, .Spacer1Type = "47", .CarriersQty = 38},
        New SpacerInfo With {.MaxWidth = 1858, .Spacer1Type = "47", .CarriersQty = 39},
        New SpacerInfo With {.MaxWidth = 1905, .Spacer1Type = "47", .CarriersQty = 40},
        New SpacerInfo With {.MaxWidth = 1952, .Spacer1Type = "47", .CarriersQty = 41},
        New SpacerInfo With {.MaxWidth = 1999, .Spacer1Type = "47", .CarriersQty = 42},
        New SpacerInfo With {.MaxWidth = 2046, .Spacer1Type = "47", .CarriersQty = 43},
        New SpacerInfo With {.MaxWidth = 2093, .Spacer1Type = "47", .CarriersQty = 44},
        New SpacerInfo With {.MaxWidth = 2140, .Spacer1Type = "47", .CarriersQty = 45},
        New SpacerInfo With {.MaxWidth = 2187, .Spacer1Type = "47", .CarriersQty = 46},
        New SpacerInfo With {.MaxWidth = 2234, .Spacer1Type = "47", .CarriersQty = 47},
        New SpacerInfo With {.MaxWidth = 2281, .Spacer1Type = "47", .CarriersQty = 48},
        New SpacerInfo With {.MaxWidth = 2328, .Spacer1Type = "47", .CarriersQty = 49},
        New SpacerInfo With {.MaxWidth = 2375, .Spacer1Type = "47", .CarriersQty = 50},
        New SpacerInfo With {.MaxWidth = 2422, .Spacer1Type = "47", .CarriersQty = 51},
        New SpacerInfo With {.MaxWidth = 2469, .Spacer1Type = "47", .CarriersQty = 52},
        New SpacerInfo With {.MaxWidth = 2516, .Spacer1Type = "47", .CarriersQty = 53},
        New SpacerInfo With {.MaxWidth = 2563, .Spacer1Type = "47", .CarriersQty = 54},
        New SpacerInfo With {.MaxWidth = 2610, .Spacer1Type = "47", .CarriersQty = 55},
        New SpacerInfo With {.MaxWidth = 2657, .Spacer1Type = "47", .CarriersQty = 56},
        New SpacerInfo With {.MaxWidth = 2704, .Spacer1Type = "47", .CarriersQty = 57},
        New SpacerInfo With {.MaxWidth = 2751, .Spacer1Type = "47", .CarriersQty = 58},
        New SpacerInfo With {.MaxWidth = 2798, .Spacer1Type = "47", .CarriersQty = 59},
        New SpacerInfo With {.MaxWidth = 2845, .Spacer1Type = "47", .CarriersQty = 60},
        New SpacerInfo With {.MaxWidth = 2892, .Spacer1Type = "47", .CarriersQty = 61},
        New SpacerInfo With {.MaxWidth = 2939, .Spacer1Type = "47", .CarriersQty = 62},
        New SpacerInfo With {.MaxWidth = 2986, .Spacer1Type = "47", .CarriersQty = 63},
        New SpacerInfo With {.MaxWidth = 3033, .Spacer1Type = "47", .CarriersQty = 64},
        New SpacerInfo With {.MaxWidth = 3080, .Spacer1Type = "47", .CarriersQty = 65},
        New SpacerInfo With {.MaxWidth = 3127, .Spacer1Type = "47", .CarriersQty = 66},
        New SpacerInfo With {.MaxWidth = 3174, .Spacer1Type = "47", .CarriersQty = 67},
        New SpacerInfo With {.MaxWidth = 3221, .Spacer1Type = "47", .CarriersQty = 68},
        New SpacerInfo With {.MaxWidth = 3268, .Spacer1Type = "47", .CarriersQty = 69},
        New SpacerInfo With {.MaxWidth = 3315, .Spacer1Type = "47", .CarriersQty = 70},
        New SpacerInfo With {.MaxWidth = 3362, .Spacer1Type = "47", .CarriersQty = 71},
        New SpacerInfo With {.MaxWidth = 3409, .Spacer1Type = "47", .CarriersQty = 72},
        New SpacerInfo With {.MaxWidth = 3456, .Spacer1Type = "47", .CarriersQty = 73},
        New SpacerInfo With {.MaxWidth = 3503, .Spacer1Type = "47", .CarriersQty = 74},
        New SpacerInfo With {.MaxWidth = 3550, .Spacer1Type = "47", .CarriersQty = 75},
        New SpacerInfo With {.MaxWidth = 3597, .Spacer1Type = "47", .CarriersQty = 76},
        New SpacerInfo With {.MaxWidth = 3644, .Spacer1Type = "47", .CarriersQty = 77},
        New SpacerInfo With {.MaxWidth = 3691, .Spacer1Type = "47", .CarriersQty = 78},
        New SpacerInfo With {.MaxWidth = 3738, .Spacer1Type = "47", .CarriersQty = 79},
        New SpacerInfo With {.MaxWidth = 3785, .Spacer1Type = "47", .CarriersQty = 80},
        New SpacerInfo With {.MaxWidth = 3832, .Spacer1Type = "47", .CarriersQty = 81},
        New SpacerInfo With {.MaxWidth = 3879, .Spacer1Type = "47", .CarriersQty = 82},
        New SpacerInfo With {.MaxWidth = 3926, .Spacer1Type = "47", .CarriersQty = 83},
        New SpacerInfo With {.MaxWidth = 3973, .Spacer1Type = "47", .CarriersQty = 84},
        New SpacerInfo With {.MaxWidth = 4020, .Spacer1Type = "47", .CarriersQty = 85},
        New SpacerInfo With {.MaxWidth = 4067, .Spacer1Type = "47", .CarriersQty = 86},
        New SpacerInfo With {.MaxWidth = 4114, .Spacer1Type = "47", .CarriersQty = 87}
    }

    Private ReadOnly Spacer127Louvolite As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 253,  .Spacer1Type = "123", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 255,  .Spacer1Type = "125", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 257,  .Spacer1Type = "127", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 264,  .Spacer1Type = "129", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 363,  .Spacer1Type = "123", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 367,  .Spacer1Type = "125", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 371,  .Spacer1Type = "127", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 380,  .Spacer1Type = "129", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 473,  .Spacer1Type = "123", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 480,  .Spacer1Type = "125", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 486,  .Spacer1Type = "127", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 497,  .Spacer1Type = "129", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 583,  .Spacer1Type = "123", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 592,  .Spacer1Type = "125", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 600,  .Spacer1Type = "127", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 613,  .Spacer1Type = "129", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 694,  .Spacer1Type = "123", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 705,  .Spacer1Type = "125", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 714,  .Spacer1Type = "127", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 729,  .Spacer1Type = "129", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 804,  .Spacer1Type = "123", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 817,  .Spacer1Type = "125", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 829,  .Spacer1Type = "127", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 845,  .Spacer1Type = "129", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 914,  .Spacer1Type = "123", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 930,  .Spacer1Type = "125", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 943,  .Spacer1Type = "127", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 962,  .Spacer1Type = "129", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 1024, .Spacer1Type = "123", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1042, .Spacer1Type = "125", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1057, .Spacer1Type = "127", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1078, .Spacer1Type = "129", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 1134, .Spacer1Type = "123", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1155, .Spacer1Type = "125", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1171, .Spacer1Type = "127", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1194, .Spacer1Type = "129", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 1244, .Spacer1Type = "123", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1267, .Spacer1Type = "125", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1286, .Spacer1Type = "127", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1310, .Spacer1Type = "129", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 1355, .Spacer1Type = "123", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1380, .Spacer1Type = "125", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1400, .Spacer1Type = "127", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1427, .Spacer1Type = "129", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 1465, .Spacer1Type = "123", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1492, .Spacer1Type = "125", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1514, .Spacer1Type = "127", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1543, .Spacer1Type = "129", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1575, .Spacer1Type = "123", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1605, .Spacer1Type = "125", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1629, .Spacer1Type = "127", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1659, .Spacer1Type = "129", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1685, .Spacer1Type = "123", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1717, .Spacer1Type = "125", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1743, .Spacer1Type = "127", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1775, .Spacer1Type = "129", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1759, .Spacer1Type = "123", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1830, .Spacer1Type = "125", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1857, .Spacer1Type = "127", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1892, .Spacer1Type = "129", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1905, .Spacer1Type = "123", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1942, .Spacer1Type = "125", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1972, .Spacer1Type = "127", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2008, .Spacer1Type = "129", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 2015, .Spacer1Type = "123", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2055, .Spacer1Type = "125", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2086, .Spacer1Type = "127", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2124, .Spacer1Type = "129", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 2126, .Spacer1Type = "123", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2167, .Spacer1Type = "125", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2200, .Spacer1Type = "127", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2240, .Spacer1Type = "129", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 2236, .Spacer1Type = "123", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2280, .Spacer1Type = "125", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2314, .Spacer1Type = "127", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2357, .Spacer1Type = "129", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 2346, .Spacer1Type = "123", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2392, .Spacer1Type = "125", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2429, .Spacer1Type = "127", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2473, .Spacer1Type = "129", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 2456, .Spacer1Type = "123", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2505, .Spacer1Type = "125", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2543, .Spacer1Type = "127", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2589, .Spacer1Type = "129", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 2566, .Spacer1Type = "123", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2617, .Spacer1Type = "125", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2657, .Spacer1Type = "127", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2705, .Spacer1Type = "129", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 2676, .Spacer1Type = "123", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2730, .Spacer1Type = "125", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2772, .Spacer1Type = "127", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2822, .Spacer1Type = "129", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 2786, .Spacer1Type = "123", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2842, .Spacer1Type = "125", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2886, .Spacer1Type = "127", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2938, .Spacer1Type = "129", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 2897, .Spacer1Type = "123", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2955, .Spacer1Type = "125", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3000, .Spacer1Type = "127", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3054, .Spacer1Type = "129", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 3007, .Spacer1Type = "123", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3067, .Spacer1Type = "125", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3115, .Spacer1Type = "127", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3170, .Spacer1Type = "129", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 3117, .Spacer1Type = "123", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3180, .Spacer1Type = "125", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3229, .Spacer1Type = "127", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3287, .Spacer1Type = "129", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 3227, .Spacer1Type = "123", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3292, .Spacer1Type = "125", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3343, .Spacer1Type = "127", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3403, .Spacer1Type = "129", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 3337, .Spacer1Type = "123", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3405, .Spacer1Type = "125", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3457, .Spacer1Type = "127", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3519, .Spacer1Type = "129", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 3447, .Spacer1Type = "123", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3517, .Spacer1Type = "125", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3572, .Spacer1Type = "127", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3635, .Spacer1Type = "129", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 3558, .Spacer1Type = "123", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3630, .Spacer1Type = "125", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3686, .Spacer1Type = "127", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3752, .Spacer1Type = "129", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 3668, .Spacer1Type = "123", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3742, .Spacer1Type = "125", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3800, .Spacer1Type = "127", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3868, .Spacer1Type = "129", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 3778, .Spacer1Type = "123", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3855, .Spacer1Type = "125", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3915, .Spacer1Type = "127", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3984, .Spacer1Type = "129", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 3888, .Spacer1Type = "123", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3967, .Spacer1Type = "125", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4029, .Spacer1Type = "127", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 4100, .Spacer1Type = "129", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 3998, .Spacer1Type = "123", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4080, .Spacer1Type = "125", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4143, .Spacer1Type = "127", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 4217, .Spacer1Type = "129", .CarriersQty = 36}
    }

    Private ReadOnly Spacer89Louvolite As New List(Of SpacerInfo) From {
        New SpacerInfo With {.MaxWidth = 178, .Spacer1Type = "83", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 180, .Spacer1Type = "85", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 182, .Spacer1Type = "87", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 184, .Spacer1Type = "89", .CarriersQty = 2},
        New SpacerInfo With {.MaxWidth = 248, .Spacer1Type = "83", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 252, .Spacer1Type = "85", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 256, .Spacer1Type = "87", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 260, .Spacer1Type = "89", .CarriersQty = 3},
        New SpacerInfo With {.MaxWidth = 317, .Spacer1Type = "83", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 323, .Spacer1Type = "85", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 330, .Spacer1Type = "87", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 336, .Spacer1Type = "89", .CarriersQty = 4},
        New SpacerInfo With {.MaxWidth = 387, .Spacer1Type = "83", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 395, .Spacer1Type = "85", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 404, .Spacer1Type = "87", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 412, .Spacer1Type = "89", .CarriersQty = 5},
        New SpacerInfo With {.MaxWidth = 456, .Spacer1Type = "83", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 467, .Spacer1Type = "85", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 478, .Spacer1Type = "87", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 488, .Spacer1Type = "89", .CarriersQty = 6},
        New SpacerInfo With {.MaxWidth = 526, .Spacer1Type = "83", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 539, .Spacer1Type = "85", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 553, .Spacer1Type = "87", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 564, .Spacer1Type = "89", .CarriersQty = 7},
        New SpacerInfo With {.MaxWidth = 595, .Spacer1Type = "83", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 610, .Spacer1Type = "85", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 627, .Spacer1Type = "87", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 640, .Spacer1Type = "89", .CarriersQty = 8},
        New SpacerInfo With {.MaxWidth = 665, .Spacer1Type = "83", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 682, .Spacer1Type = "85", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 701, .Spacer1Type = "87", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 716, .Spacer1Type = "89", .CarriersQty = 9},
        New SpacerInfo With {.MaxWidth = 734, .Spacer1Type = "83", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 754, .Spacer1Type = "85", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 775, .Spacer1Type = "87", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 792, .Spacer1Type = "89", .CarriersQty = 10},
        New SpacerInfo With {.MaxWidth = 804, .Spacer1Type = "83", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 825, .Spacer1Type = "85", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 849, .Spacer1Type = "87", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 868, .Spacer1Type = "89", .CarriersQty = 11},
        New SpacerInfo With {.MaxWidth = 873, .Spacer1Type = "83", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 897, .Spacer1Type = "85", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 923, .Spacer1Type = "87", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 944, .Spacer1Type = "89", .CarriersQty = 12},
        New SpacerInfo With {.MaxWidth = 943, .Spacer1Type = "83", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 969, .Spacer1Type = "85", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 997, .Spacer1Type = "87", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1020, .Spacer1Type = "89", .CarriersQty = 13},
        New SpacerInfo With {.MaxWidth = 1012, .Spacer1Type = "83", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1040, .Spacer1Type = "85", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1071, .Spacer1Type = "87", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1096, .Spacer1Type = "89", .CarriersQty = 14},
        New SpacerInfo With {.MaxWidth = 1082, .Spacer1Type = "83", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1112, .Spacer1Type = "85", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1145, .Spacer1Type = "87", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1172, .Spacer1Type = "89", .CarriersQty = 15},
        New SpacerInfo With {.MaxWidth = 1151, .Spacer1Type = "83", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1184, .Spacer1Type = "85", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1219, .Spacer1Type = "87", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1248, .Spacer1Type = "89", .CarriersQty = 16},
        New SpacerInfo With {.MaxWidth = 1221, .Spacer1Type = "83", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1256, .Spacer1Type = "85", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1294, .Spacer1Type = "87", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1324, .Spacer1Type = "89", .CarriersQty = 17},
        New SpacerInfo With {.MaxWidth = 1290, .Spacer1Type = "83", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1327, .Spacer1Type = "85", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1368, .Spacer1Type = "87", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1400, .Spacer1Type = "89", .CarriersQty = 18},
        New SpacerInfo With {.MaxWidth = 1360, .Spacer1Type = "83", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1399, .Spacer1Type = "85", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1442, .Spacer1Type = "87", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1476, .Spacer1Type = "89", .CarriersQty = 19},
        New SpacerInfo With {.MaxWidth = 1429, .Spacer1Type = "83", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1471, .Spacer1Type = "85", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1516, .Spacer1Type = "87", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1552, .Spacer1Type = "89", .CarriersQty = 20},
        New SpacerInfo With {.MaxWidth = 1499, .Spacer1Type = "83", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1542, .Spacer1Type = "85", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1590, .Spacer1Type = "87", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1628, .Spacer1Type = "89", .CarriersQty = 21},
        New SpacerInfo With {.MaxWidth = 1568, .Spacer1Type = "83", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1614, .Spacer1Type = "85", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1664, .Spacer1Type = "87", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1704, .Spacer1Type = "89", .CarriersQty = 22},
        New SpacerInfo With {.MaxWidth = 1638, .Spacer1Type = "83", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1686, .Spacer1Type = "85", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1738, .Spacer1Type = "87", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1780, .Spacer1Type = "89", .CarriersQty = 23},
        New SpacerInfo With {.MaxWidth = 1707, .Spacer1Type = "83", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1757, .Spacer1Type = "85", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1812, .Spacer1Type = "87", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1856, .Spacer1Type = "89", .CarriersQty = 24},
        New SpacerInfo With {.MaxWidth = 1777, .Spacer1Type = "83", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1829, .Spacer1Type = "85", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1886, .Spacer1Type = "87", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1932, .Spacer1Type = "89", .CarriersQty = 25},
        New SpacerInfo With {.MaxWidth = 1846, .Spacer1Type = "83", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1901, .Spacer1Type = "85", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1960, .Spacer1Type = "87", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 2008, .Spacer1Type = "89", .CarriersQty = 26},
        New SpacerInfo With {.MaxWidth = 1916, .Spacer1Type = "83", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1973, .Spacer1Type = "85", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2035, .Spacer1Type = "87", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 2084, .Spacer1Type = "89", .CarriersQty = 27},
        New SpacerInfo With {.MaxWidth = 1985, .Spacer1Type = "83", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2044, .Spacer1Type = "85", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2109, .Spacer1Type = "87", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2160, .Spacer1Type = "89", .CarriersQty = 28},
        New SpacerInfo With {.MaxWidth = 2055, .Spacer1Type = "83", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2116, .Spacer1Type = "85", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2183, .Spacer1Type = "87", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2236, .Spacer1Type = "89", .CarriersQty = 29},
        New SpacerInfo With {.MaxWidth = 2124, .Spacer1Type = "83", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2188, .Spacer1Type = "85", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2257, .Spacer1Type = "87", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2312, .Spacer1Type = "89", .CarriersQty = 30},
        New SpacerInfo With {.MaxWidth = 2194, .Spacer1Type = "83", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2259, .Spacer1Type = "85", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2331, .Spacer1Type = "87", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2388, .Spacer1Type = "89", .CarriersQty = 31},
        New SpacerInfo With {.MaxWidth = 2263, .Spacer1Type = "83", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2331, .Spacer1Type = "85", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2405, .Spacer1Type = "87", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2464, .Spacer1Type = "89", .CarriersQty = 32},
        New SpacerInfo With {.MaxWidth = 2333, .Spacer1Type = "83", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2403, .Spacer1Type = "85", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2479, .Spacer1Type = "87", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2540, .Spacer1Type = "89", .CarriersQty = 33},
        New SpacerInfo With {.MaxWidth = 2402, .Spacer1Type = "83", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2474, .Spacer1Type = "85", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2553, .Spacer1Type = "87", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2616, .Spacer1Type = "89", .CarriersQty = 34},
        New SpacerInfo With {.MaxWidth = 2472, .Spacer1Type = "83", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2546, .Spacer1Type = "85", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2627, .Spacer1Type = "87", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2692, .Spacer1Type = "89", .CarriersQty = 35},
        New SpacerInfo With {.MaxWidth = 2541, .Spacer1Type = "83", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2618, .Spacer1Type = "85", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2701, .Spacer1Type = "87", .CarriersQty = 36},
        New SpacerInfo With {.MaxWidth = 2768, .Spacer1Type = "89", .CarriersQty = 36}
    }

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
        Dim headerid As String = lblHeaderId.Text
        Dim ordertype As String = lblOrderType.Text
        Response.Redirect("~/order/detail?param=" & headerid & "&ordertype=" & ordertype, False)
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
