Imports System.Data

Partial Class Order_Roller
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

        cardTitle.InnerHtml = "Add Item"
        pageAction.InnerHtml = Session("itemAction")
        pageTitle.InnerHtml = publicCfg.GetDesignName(designId)

        btnReset.Visible = False

        If Session("itemAction") = "AddItem" Then
            cardTitle.InnerHtml = "Add Item"
            lblHeaderId.Text = Session("HeaderId")
            lblBlindNo.Text = "Blind 1"
            btnSubmit.Visible = True : btnSubmit.Text = "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Add Item)"
            btnReset.Visible = True
            If Not IsPostBack Then
                txtQty.Text = "1"
                Call BackColor()

                Call BindBlindType()
                Call BindBracket(ddlBlindType.SelectedValue)
                Call BindTubeType(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue)
                Call BindControlType(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue, ddlTubeType.SelectedValue)
                Call BindColour(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue, ddlTubeType.SelectedValue, ddlControlType.SelectedValue)

                Call BindFabricType()

                Call BindTrim(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)
                Call BindRailType(ddlBracketType.SelectedValue)

                Call BindTubeSize(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)

                Call BindMotorStyle(ddlControlType.SelectedValue)
                Call BindRemoteMotor(ddlControlType.SelectedValue)
                Call BindChargerMotor(ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)
                Call BindExternalBattery()
                Call BindExtras(ddlBlindType.SelectedValue, ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)

                Call BindComponentForm(ddlColourType.SelectedValue)
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
                Call BindDataItem(lblItemId.Text)
            End If
        End If

        If Session("itemAction") = "NextItem" Then
            lblHeaderId.Text = Session("HeaderId")
            lblItemId.Text = Session("itemId")
            btnSubmit.Visible = True : btnSubmit.Text = "<i class='fa-solid fa-cloud-arrow-up me-2'></i>Process (Next Item)"
            cardTitle.InnerHtml = "Next Item"
            If Not IsPostBack Then
                Call BindDataItem(lblItemId.Text)
            End If
        End If

        If Session("itemAction") = "ViewItem" Then
            lblHeaderId.Text = Session("HeaderId")
            lblItemId.Text = Session("itemId")
            btnSubmit.Visible = False
            cardTitle.InnerHtml = "VIEW ITEM ID : " & lblItemId.Text
            If Session("RoleName") = "Administrator" Then : btnSubmit.Visible = True : End If
            If Not IsPostBack Then
                Call BindDataItem(lblItemId.Text)
            End If
        End If
    End Sub

    Protected Sub btnReset_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/order/roller", False)
    End Sub

    Protected Sub ddlBlindType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        divError.Visible = False '#reset msg error
        ddlBlindType.Enabled = False

        Call BindBracket(ddlBlindType.SelectedValue)
        Call BindTubeType(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue)
        Call BindControlType(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue, ddlTubeType.SelectedValue)
        Call BindColour(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue, ddlTubeType.SelectedValue, ddlControlType.SelectedValue)

        Call BindTrim(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)
        Call BindRailType(ddlBracketType.SelectedValue)
        Call BindTubeSize(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)

        Call BindMotorStyle(ddlControlType.SelectedValue)
        Call BindRemoteMotor(ddlControlType.SelectedValue)
        Call BindChargerMotor(ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)
        Call BindExternalBattery()
        Call BindExtras(ddlBlindType.SelectedValue, ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)

        Call BindComponentForm(ddlColourType.SelectedValue)
    End Sub

    Protected Sub ddlBracketType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        divError.Visible = False '#reset msg error
        ddlBracketType.Enabled = False

        Call BindTubeType(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue)
        Call BindControlType(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue, ddlTubeType.SelectedValue)
        Call BindColour(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue, ddlTubeType.SelectedValue, ddlControlType.SelectedValue)

        Call BindTrim(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)
        Call BindRailType(ddlBracketType.SelectedValue)
        Call BindTubeSize(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)

        Call BindMotorStyle(ddlControlType.SelectedValue)
        Call BindRemoteMotor(ddlControlType.SelectedValue)
        Call BindChargerMotor(ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)
        Call BindExternalBattery()
        Call BindExtras(ddlBlindType.SelectedValue, ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)

        Call BindComponentForm(ddlColourType.SelectedValue)
    End Sub

    Protected Sub ddlTubeType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        divError.Visible = False '#reset msg error
        Dim controlType = ddlControlType.SelectedValue
        Call BindControlType(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue, ddlTubeType.SelectedValue)
        Try
            ddlControlType.SelectedValue = controlType
        Catch ex As Exception
        End Try

        Call BindColour(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue, ddlTubeType.SelectedValue, ddlControlType.SelectedValue)

        Dim trim As String = ddlTrim.SelectedValue
        Call BindTrim(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)
        Try
            ddlTrim.SelectedValue = trim
        Catch ex As Exception
        End Try

        Dim tubeSize As String = ddlTubeSize.SelectedValue
        Call BindTubeSize(ddlBlindType.SelectedValue, ddlTubeType.SelectedValue)
        Try
            ddlTubeSize.SelectedValue = tubeSize
        Catch ex As Exception
        End Try
        Call BindComponentForm(ddlColourType.SelectedValue)
    End Sub

    Protected Sub ddlControlType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        divError.Visible = False '#reset msg error
        Call BindColour(ddlBlindType.SelectedValue, ddlBracketType.SelectedValue, ddlTubeType.SelectedValue, ddlControlType.SelectedValue)

        Dim motorStyle As String = ddlMotorStyle.SelectedValue
        Call BindMotorStyle(ddlControlType.SelectedValue)
        Try
            ddlMotorStyle.SelectedValue = motorStyle
        Catch ex As Exception

        End Try

        Dim motorRemote As String = ddlMotorRemote.SelectedValue
        Call BindRemoteMotor(ddlControlType.SelectedValue)
        Call BindChargerMotor(ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)
        Call BindExternalBattery()
        Call BindExtras(ddlBlindType.SelectedValue, ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)
        Try
            ddlMotorRemote.SelectedValue = motorRemote
        Catch ex As Exception

        End Try

        Call BindComponentForm(ddlColourType.SelectedValue)
    End Sub


    Protected Sub ddlMotorStyle_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        divError.Visible = False '#reset msg error
        Call BindChargerMotor(ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)
        Call BindExternalBattery()
        Call BindExtras(ddlBlindType.SelectedValue, ddlControlType.SelectedValue, ddlMotorStyle.SelectedValue)
        Call BindComponentForm(ddlColourType.SelectedValue)
    End Sub



    Protected Sub ddlColourType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        divError.Visible = False '#reset msg error
        Call BindComponentForm(ddlColourType.SelectedValue)
    End Sub

    Protected Sub ddlFabricType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        divError.Visible = False '#reset msg error
        Call BindFabricColour(ddlFabricType.SelectedValue)
    End Sub

    Protected Sub ddlTrim_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        divError.Visible = False '#reset msg error
        Call BindComponentForm(ddlColourType.SelectedValue)
    End Sub

    Protected Sub ddlTrimSkin_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()
        divError.Visible = False '#reset msg error
        Call BindComponentForm(ddlBracketType.SelectedValue)
    End Sub

    Protected Sub ddlRailType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindRailColour(ddlBracketType.SelectedValue, ddlRailType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            '#Dataset
            Dim bracketName As String = ddlBracketType.SelectedValue
            Dim tubeType As String = ddlTubeType.SelectedValue
            Dim controlType As String = ddlControlType.SelectedValue

            If ddlBlindType.SelectedValue = "" Then
                Call MessageError(True, "ROLLER TYPE IS REQUIRED !")
                ddlBlindType.CssClass = "form-select  is-invalid"
                ddlBlindType.Focus()
                Exit Sub
            End If
            
            Dim blindName As String = publicCfg.GetBlindName(ddlBlindType.SelectedValue)

            If ddlBracketType.SelectedValue = "" Then
                Dim xmsgx As String = "BRACKET TYPE IS REQUIRED !"
                If blindName = "Skin Only" Then
                    xmsgx = "SKIN TYPE IS REQUIRED !"
                End If
                Call MessageError(True, xmsgx)
                ddlBracketType.CssClass = "form-select  is-invalid"
                ddlBracketType.Focus()
                Exit Sub
            End If

            If ddlTubeType.SelectedValue = "" Then
                Dim xmsgx As String = "MECHANISM TYPE IS REQUIRED !"
                If blindName = "Skin Only" Then
                    xmsgx = "TUBE SKIN TYPE IS REQUIRED !"
                End If
                Call MessageError(True, xmsgx)
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

            If ddlColourType.SelectedValue = "" Then
                Call MessageError(True, "COLOUR TYPE IS REQUIRED !")
                ddlColourType.CssClass = "form-select  is-invalid"
                ddlColourType.Focus()
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
                    txtQty.BackColor = Drawing.Color.Red
                    txtQty.Focus()
                    Exit Sub
                End If

                If txtQty.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR QUANTITY ORDER !")
                    txtQty.BackColor = Drawing.Color.Red
                    txtQty.Focus()
                    Exit Sub
                End If
            End If

            If Not txtLocation.Text = "" Then
                If InStr(txtLocation.Text, "&") > 0 Then
                    Call MessageError(True, "CHARACTER [&] IS NO RECOMMENDED !")
                    txtLocation.BackColor = Drawing.Color.Red
                    txtLocation.Focus()
                    Exit Sub
                End If
            End If

            ' If Not blindName = "Skin Only" Then
            '     If ddlMounting.SelectedValue = "" Then
            '         Call MessageError(True, "MOUNTING IS REQUIRED !")
            '         ddlMounting.CssClass = "form-select  is-invalid"
            '         ddlMounting.Focus()
            '         Exit Sub
            '     End If
            ' End If

            If ddlFabricType.SelectedValue = "" Then
                Call MessageError(True, "FABRIC TYPE IS REQUIRED !")
                ddlFabricType.CssClass = "form-select  is-invalid"
                ddlFabricType.Focus()
                Exit Sub
            End If

            If ddlFabricColour.SelectedValue = "" Then
                Call MessageError(True, "FABRIC COLOUR IS REQUIRED !")
                ddlFabricColour.CssClass = "form-select  is-invalid"
                ddlFabricColour.Focus()
                Exit Sub
            End If

            If Not tubeType = "Spring Operated" And Not tubeType = "N/A" Then
                If Not blindName = "Skin Only" Then
                    If ddlRoll.SelectedValue = "" Then
                        Call MessageError(True, "ROLL DIRECTION IS REQUIRED !")
                        ddlRoll.CssClass = "form-select  is-invalid"
                        ddlRoll.Focus()
                        Exit Sub
                    End If
                End If

                If bracketName = "Single" Or bracketName = "Double" Or bracketName = "Linked 2 Blinds (Ind)" Or bracketName = "Headbox Only" Or bracketName = "Headbox & Side Channels" Then
                    If ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION / SIDE IS REQUIRED !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If
                End If
            End If
            '#===================================Control Event===================================#
            If controlType = "Chain" Then
                '#Single, Double, Linked 2 (Ind)
                If bracketName = "Single" Or bracketName = "Double" Or bracketName = "Linked 2 Blinds (Ind)" Then
                    Call chainSingleDoubleLinked2Ind()
                End If

                '#Linked 2 Blinds (Dep)
                If bracketName = "Linked 2 Blinds (Dep)" Then
                    Call chainLinked2BlindsDep()  
                End If

                '#Linked 3 Blinds (Dep)
                If bracketName = "Linked 3 Blinds (Dep)" Then 
                    Call chainLinked3BlindsDep()
                End If

                '#Linked 3 Blinds (Ind)
                If bracketName = "Linked 3 Blinds (Ind)" Then
                    Call chainLinked3BlindsInd()
                End If


                '#DB Linked 2 Blinds (Dep)
                If bracketName = "Double and Link System Dep" Then
                    Call chainDBLinked2BlindsDep()
                End If


                '#DB Linked 2 Blinds (Dep)
                If bracketName = "Double and Link System Ind" Then
                    Call chainDBLinked2BlindsInd()
                End If
            End If


            If bracketName = "With Tube & Bottom Included" Or bracketName = "With Bottom Included" Then
                ' If ddlTrimSkin.SelectedValue <> "1F" Then
                '     Call MessageError(True, "NOT SUITABLE FOR THIS TYPE OF TRIM")
                '     ddlTrimSkin.CssClass = "form-select  is-invalid"
                '     ddlTrimSkin.Focus()
                '     Exit Sub
                ' End If

                If ddlTrimSkin.SelectedValue = "" Then
                    Call MessageError(True, "TRIM IS REQUIRED !")
                    ddlTrimSkin.CssClass = "form-select  is-invalid"
                    ddlTrimSkin.Focus()
                    Exit Sub
                End If

                If ddlTrimSkin.SelectedValue = "1F" Then
                    If ddlRailType.SelectedValue = "" Then
                        Call MessageError(True, "BOTTOM RAIL TYPE IS REQUIRED !")
                        ddlRailType.CssClass = "form-select  is-invalid"
                        ddlRailType.Focus()
                        Exit Sub
                    End If

                    If ddlRailColour.SelectedValue = "" Then
                        Call MessageError(True, "BOTTOM RAIL COLOUR IS REQUIRED !")
                        ddlRailColour.CssClass = "form-select  is-invalid"
                        ddlRailColour.Focus()
                        Exit Sub
                    End If
                End If

                If ddlTrimSkin.SelectedValue <> "1F" And (ddlRailType.SelectedValue <> "" Or ddlRailColour.SelectedValue <> "") Then
                    Call MessageError(True, "BOTTOM RAIL NOT REQUIRED FOR THIS TRIM. PLEASE EMPTY BOTTOM RAIL !")
                    ddlRailType.CssClass = "form-select  is-invalid"
                    ddlRailColour.CssClass = "form-select  is-invalid"
                    ddlRailType.Focus()
                    ddlRailColour.Focus()
                    Exit Sub
                End If

                
                ' Call MessageError(True, ddlTrimSkin.SelectedValue)
            End If

            If Not tubeType = "N/A" Then
                If Not blindName = "Skin Only" Then
                    If ddlTrim.SelectedValue = "" Then
                        Call MessageError(True, "TRIM IS REQUIRED !")
                        ddlTrim.CssClass = "form-select  is-invalid"
                        ddlTrim.Focus()
                        Exit Sub
                    End If
                End If

                If ddlTrim.SelectedValue = "1F" And tubeType = "Spring Operated" Then
                    Call MessageError(True, "TRIM 1F IS NOT RECOMMENDED FOR ROLLER SPRING OPERATED !")
                    ddlTrim.CssClass = "form-select  is-invalid"
                    ddlTrim.Focus()
                    Exit Sub
                End If

                If ddlTrim.SelectedValue = "1F"Then
                    If ddlRailType.SelectedValue = "" Then
                        Call MessageError(True, "BOTTOM RAIL TYPE IS REQUIRED !")
                        ddlRailType.CssClass = "form-select  is-invalid"
                        ddlRailType.Focus()
                        Exit Sub
                    End If

                    If ddlRailColour.SelectedValue = "" Then
                        Call MessageError(True, "BOTTOM RAIL COLOUR IS REQUIRED !")
                        ddlRailColour.CssClass = "form-select  is-invalid"
                        ddlRailColour.Focus()
                        Exit Sub
                    End If
                End If

                If Not blindName = "Skin Only" Then
                    If Not ddlTrim.SelectedValue = "1F" And (Not ddlRailType.SelectedValue = "" Or Not ddlRailColour.SelectedValue = "") Then
                        Call MessageError(True, "BOTTOM RAIL NOT REQUIRED FOR THIS TRIM. PLEASE EMPTY BOTTOM RAIL !")
                        ddlRailType.CssClass = "form-select  is-invalid"
                        ddlRailColour.CssClass = "form-select  is-invalid"
                        ddlRailType.Focus()
                        ddlRailColour.Focus()
                        Exit Sub
                    End If
                End If
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

                If txtWidth.Text < 150 Then
                    Call MessageError(True, "MINIMUM WIDTH FOR ROLLER BLIND IS 150mm !")
                    txtWidth.CssClass = "form-control  is-invalid"
                    txtWidth.Focus()
                    Exit Sub
                End If

                If txtWidth.Text > 6000 Then
                    Call MessageError(True, "MAXIMUM WIDTH FOR ROLLER BLIND IS 3000mm !")
                    txtWidth.CssClass = "form-control  is-invalid"
                    txtWidth.Focus()
                    Exit Sub
                End If
            End If

            If txtDrop.Text = "" Or txtDrop.Text = "0" Then
                Call MessageError(True, "DROP IS REQUIRED !")
                txtDrop.CssClass = "form-control  is-invalid"
                txtDrop.Focus()
                Exit Sub
            End If

            If Not txtDrop.Text = "" Then
                If Not IsNumeric(txtDrop.Text) Then
                    Call MessageError(True, "DROP SHOULD BE NUMERIC !")
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

            ' If Not blindName = "Skin Only" Then
            If blindName = "Roller Blind" Then
                If ddlTubeSize.SelectedValue = "" Then
                    Call MessageError(True, "TUBE SIZE IS REQUIRED !")
                    ddlTubeSize.CssClass = "form-select  is-invalid"
                    ddlTubeSize.Focus()
                    Exit Sub
                End If
            End If

            If blindName = "Skin Only" Then
                If InStr(ddlBracketType.SelectedValue, "Tube") > 0 Then
                    If ddlTubeSize.SelectedValue = "" Then
                        Call MessageError(True, "TUBE SIZE IS REQUIRED !")
                        ddlTubeSize.CssClass = "form-select  is-invalid"
                        ddlTubeSize.Focus()
                        Exit Sub
                    End If
                End If
            End If

            ' If blindName = "Roller Blind" And ddlChildSafe.SelectedValue = "" Then
            '     Call MessageError(True, "CHILDSAFE IS REQUIRED !")
            '     ddlChildSafe.CssClass = "form-select  is-invalid"
            '     ddlChildSafe.Focus()
            '     Exit Sub
            ' End If

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

                lblKitId.Text = UCase(ddlColourType.SelectedValue).ToString()
                lblSoeKitId.Text = publicCfg.GetSoeKitId(lblKitId.Text)

                Dim fabricGroup As String = publicCfg.GetFabricGroup(ddlFabricColour.SelectedValue)

                Dim priceGroupName As String = "Roller Blind" & " - " & fabricGroup
                If blindName = "Skin Only" Then
                    priceGroupName = "Roller Skin Only" & " - " & fabricGroup
                End If
                Dim priceGroupId As String = publicCfg.GetPriceGroupId(designId, priceGroupName)
                lblPriceGroupId.Text = UCase(priceGroupId).ToString()

                lblCassetteExtraId.Text = ""
                If blindName = "Cassette" Then
                    Dim cassetteExtraName As String = bracketName & " - " & ddlMounting.SelectedValue
                    If bracketName = "Headbox Only" Then : cassetteExtraName = bracketName : End If
                    priceGroupId = publicCfg.GetPriceGroupId(designId, cassetteExtraName)
                    lblCassetteExtraId.Text = UCase(priceGroupId).ToString()
                End If

                lblChainId.Text = "" : lblChainLength.Text = ""
                If controlType = "Chain" And Not ddlChainColour.SelectedValue = "" Then
                    Dim chainColour As String = "(" & ddlChainColour.SelectedValue & ")"

                    Dim chainLength As String = txtChainLength.Text
                    If txtChainLength.Text = "" Or txtChainLength.Text = "0" Then
                        chainLength = "500"
                        If txtDrop.Text > 700 Then : chainLength = "600" : End If
                        If txtDrop.Text > 800 Then : chainLength = "800" : End If
                        If txtDrop.Text > 1100 Then : chainLength = "1000" : End If
                        If txtDrop.Text > 1300 Then : chainLength = "1250" : End If
                        If txtDrop.Text > 1600 Then : chainLength = "1500" : End If
                        If txtDrop.Text > 2000 Then : chainLength = "1800" : End If
                        If txtDrop.Text > 2400 Then : chainLength = "2000" : End If
                        If txtDrop.Text > 2700 Then : chainLength = "2200" : End If
                    End If

                    If Not txtChainLength.text = "" Then
                        chainLength  = "500"
                        If txtChainLength.Text > 500 Then : chainLength = "600" : End If
                        If txtChainLength.Text > 600 Then : chainLength = "800" : End If
                        If txtChainLength.Text > 800 Then : chainLength = "1000" : End If
                        If txtChainLength.Text > 1000 Then : chainLength = "1250" : End If
                        If txtChainLength.Text > 1250 Then : chainLength = "1500" : End If
                        If txtChainLength.Text > 1500 Then : chainLength = "1800" : End If
                        If txtChainLength.Text > 1800 Then : chainLength = "2000" : End If
                        If txtChainLength.Text > 2000 Then : chainLength = "2200" : End If
                        If txtChainLength.Text > 2200 Then : chainLength = "2500" : End If
                    End If
 
                    Dim chainName As String = chainLength & " " & "Chain + Joiner" & " " & chainColour
                    lblChainId.Text = publicCfg.GetItemData("SELECT Id FROM Chains WHERE Name = '" + chainName + "'")
                    '#empty chain length
                    If txtChainLength.Text = "" Or txtChainLength.Text = "0" Then : lblChainLength.Text = chainLength : End If
                    '#input chain length
                    If Not txtChainLength.Text = "" Then : lblChainLength.Text = txtChainLength.Text : End If

                    ddlMotorStyle.SelectedValue = ""
                    ddlMotorRemote.SelectedValue = ""
                    ddlExternalBattery.SelectedValue = ""
                    ddlMotorCharger.SelectedValue = ""
                    ddlCableExitPoint.SelectedValue = ""
                    ddlConnector.SelectedValue = ""
                End If

                If InStr(controlType, "Somfy") > 0 Or InStr(controlType, "Alpha") > 0 Then
                    ddlChainColour.SelectedValue = ""
                    lblChainLength.Text = ""
                    If bracketName = "Headbox & Side Channels" Then
                        ddlConnector.SelectedValue = ""
                    End If
                    If blindName = "Motorised" Or (controlType = "Alpha WF" Or controlType = "Sonfy WF") Then
                        ddlCableExitPoint.SelectedValue = ""
                    End If
                End If

                lblTrim.Text = ddlTrim.SelectedValue
                If blindName = "Skin Only" Then
                    lblTrim.Text = ddlTrimSkin.SelectedValue
                End If

                If ddlTrim.SelectedValue = "1F" Then
                    ddlAccessory.SelectedValue = ""
                End If

                ' Call MessageError(True, lblTrim.Text)
                

                '#create new
                If Session("itemAction") = "AddItem" Then
                    lblItemId.Text = publicCfg.CreateOrderItemId()
                    lblUniqueId.Text = ""
                    If bracketName = "Double" Or InStr(bracketName, "Linked") > 0 Or InStr(bracketName, "Link") > 0 Then
                        lblUniqueId.Text = GenerateUniqueId()
                    End If

                    sdsPage.Insert()
                    Call SetPricing(lblItemId.Text, lblHeaderId.Text)

                    publicCfg.InsertActivity(userId, Page.Title, "INSERT ORDER DETAIL. ITEM ID : " & lblItemId.Text)
                    If bracketName = "Double" Or InStr(bracketName, "Linked") > 0 Or InStr(bracketName, "Link") > 0 Then
                        Session("itemAction") = ""
                        Dim myScript As String = "window.onload = function() { showConfirm('" + lblItemId.Text + "', '" + lblBlindNo.Text + "'); };"
                        ClientScript.RegisterStartupScript(Me.GetType(), "showConfirm", myScript, True)
                        Exit Sub
                    End If

                    '#Show popup WF Motorised
                    publicCfg.InsertActivity(userId, Page.Title, "UPDATE ORDER DETAIL. ITEM ID : " & lblItemId.Text)
                    If  InStr(bracketName, "Linked") AND ddlControlType.SelectedValue = "Somfy WF" Then
                        Dim msg As String ="<b>Warning :</b>Check SP the availability for linking blind for WF motorised !"
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showPopUpWfMotorised('"& msg &"')", True)
                        Session("headerId") = lblHeaderId.Text
                        Exit Sub
                    End If
                    If  InStr(bracketName, "Linked") AND ddlControlType.SelectedValue = "Alpha WF" AndAlso ddlMotorStyle.SelectedValue = "Alpha 2NM Std" Then
                        Dim msg As String ="<b>Warning :</b> Check SP the availability for linking blind for WF motorised !"
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showPopUpWfMotorised('"& msg &"')", True)
                        Session("headerId") = lblHeaderId.Text
                        Exit Sub
                    End If

                    Call myCancel()
                End If

                '#next items
                If Session("itemAction") = "NextItem" Then
                    lblItemId.Text = publicCfg.CreateOrderItemId()
                    sdsPage.Insert()

                    publicCfg.InsertActivity(userId, Page.Title, "COPY ORDER DETAIL. ITEM ID : " & lblItemId.Text)

                    '#Double
                    If bracketName = "Double" Then
                        sdsNext.Update()
                        sdsSize.Update()
                    End If

                    '#Linked 2 Blinds (Dep)
                    If bracketName = "Linked 2 Blinds (Dep)" Then
                        sdsDrop.Update()
                        sdsRollDep.Update()
                        sdsFabric.Update()
                        sdsNext.Update()'#added 240924
                    End If

                    '#Linked 2 Blinds (Ind)
                    If bracketName = "Linked 2 Blinds (Ind)" Then
                        sdsNext.Update()
                        sdsFabric.Update()
                    End If

                    '#Linked 3 Blinds (Dep)
                    If bracketName = "Linked 3 Blinds (Dep)" Then 
                        sdsDrop.Update()
                        sdsRollDep.Update()
                        sdsFabric.Update()
                        sdsNext.Update()
                    End If

                    '#Linked 3 Blinds (Ind)
                    If bracketName = "Linked 3 Blinds (Ind)" Then 
                        sdsFabric.Update()
                        sdsNext.Update()
                    End If

                    '#DB and Link System Dep
                    If bracketName = "Double and Link System Dep" Then
                        sdsDrop.Update()
                         If lblBlindNo.Text = "Blind 2"  Then
                            sdsDB2Depfirst.Update()
                        End If

                        If lblBlindNo.Text = "Blind 4"  Then
                            sdsDB2Depsecond.Update()
                        End If
                        sdsNext.Update()
                    End If

                    '#DB and Link System Ind
                    If bracketName = "Double and Link System Ind" Then
                        If lblBlindNo.Text = "Blind 2"  Then
                            sdsDB2Indfirst.Update()
                        End If
                        If lblBlindNo.Text = "Blind 4"  Then
                            sdsDB2Indsecond.Update()
                        End If
                        sdsNext.Update()
                    End If

                    Call SetPricing(lblItemId.Text, lblHeaderId.Text)

                    If lblBlindNo.Text = "Blind 2" And (bracketName = "Linked 3 Blinds (Dep)" Or bracketName = "Linked 3 Blinds (Ind)" Or bracketName = "Double and Link System Dep" Or bracketName = "Double and Link System Ind") Then
                        Dim blindNo As String = lblBlindNo.Text
                        Dim myScript As String = "window.onload = function() { showConfirm('" + lblItemId.Text + "', '" + blindNo + "'); };"
                        ClientScript.RegisterStartupScript(Me.GetType(), "showConfirm", myScript, True)
                        Exit Sub
                    End If


                    Call myCancel()
                End If

                '#update Items
                If Session("itemAction") = "EditItem" Or Session("itemAction") = "ViewItem" Then
                    sdsPage.Update()

                    If bracketName = "Double" Then
                        sdsNext.Update()
                        sdsSize.Update()
                    End If

                    '#Linked 2 Blinds (Dep)
                    If bracketName = "Linked 2 Blinds (Dep)" Then
                        sdsDrop.Update()
                        sdsRollDep.Update()
                        sdsFabric.Update()
                        sdsNext.Update()
                    End If

                    '#Linked 2 Blinds (Ind)
                    If bracketName = "Linked 2 Blinds (Ind)" Then
                        sdsNext.Update()
                        sdsTubeSize.Update()
                        sdsFabric.Update()
                    End If

                    '#Linked 3 Blinds (Dep)
                    If bracketName = "Linked 3 Blinds (Dep)" Then 
                        sdsDrop.Update()
                        sdsRollDep.Update()
                        sdsFabric.Update()
                        sdsNext.Update()
                    End If

                    '#Linked 3 Blinds (Ind)
                     If bracketName = "Linked 3 Blinds (Ind)" Then 
                        sdsFabric.Update()
                        sdsTubeSize.Update()
                        sdsNext.Update()
                    End If

                    '#DB and Link System Dep
                    If bracketName = "Double and Link System Dep" Then
                        sdsDrop.Update()
                        If lblBlindNo.Text = "Blind 1" Or lblBlindNo.Text = "Blind 2"  Then
                            sdsDB2Depfirst.Update()
                        End If
                        If lblBlindNo.Text = "Blind 3" Or lblBlindNo.Text = "Blind 4"  Then
                            sdsDB2Depsecond.Update()
                        End If
                        sdsNext.Update()
                    End If

                    '#DB and Link System Ind
                    If bracketName = "Double and Link System Ind" Then 
                        If lblBlindNo.Text = "Blind 1" Or lblBlindNo.Text = "Blind 2"  Then
                            sdsDB2Indfirst.Update()
                        End If
                        If lblBlindNo.Text = "Blind 3" Or lblBlindNo.Text = "Blind 4"  Then
                            sdsDB2Indsecond.Update()
                        End If
                        sdsNext.Update()
                    End If

                    Call SetPricing(lblItemId.Text, lblHeaderId.Text)

                    '#Show popup WF Motorised
                    publicCfg.InsertActivity(userId, Page.Title, "UPDATE ORDER DETAIL. ITEM ID : " & lblItemId.Text)
                    If  InStr(bracketName, "Linked") AND ddlControlType.SelectedValue = "Somfy WF" Then
                        Dim msg As String ="<b>Warning :</b>Check SP the availability for linking blind for WF motorised !"
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showPopUpWfMotorised('"& msg &"')", True)
                        Session("headerId") = lblHeaderId.Text
                        Exit Sub
                    End If
                    If  InStr(bracketName, "Linked") AND ddlControlType.SelectedValue = "Alpha WF" AndAlso ddlMotorStyle.SelectedValue = "Alpha 2NM Std" Then
                        Dim msg As String ="<b>Warning :</b> Check SP the availability for linking blind for WF motorised !"
                        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showPopUpWfMotorised('"& msg &"')", True)
                        Session("headerId") = lblHeaderId.Text
                        Exit Sub
                    End If

                    
                   
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

    Private Sub BindDataItem(ItemId As String)
        Call BackColor()
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_details WHERE Id = '" + ItemId + "'")

            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/order/detail", False)
                Exit Sub
            End If

            Dim kitId As String = myData.Tables(0).Rows(0).Item("KitId").ToString()
            Dim blindId As String = publicCfg.GetBlindId(kitId)
            Dim blindName As String = publicCfg.GetBlindName(blindId)
            Dim bracketName As String = publicCfg.GetBracketType(kitId)
            Dim tubeType As String = publicCfg.GetTubeType(kitId)
            Dim controlType As String = publicCfg.GetControlType(kitId)

            Dim motorStyle As String = myData.Tables(0).Rows(0).Item("MotorStyle").ToString()
            Dim fabricType As String = myData.Tables(0).Rows(0).Item("FabricType").ToString()
            Dim bottomType As String = myData.Tables(0).Rows(0).Item("BottomType").ToString()
            Dim chainColour As String = myData.Tables(0).Rows(0).Item("ChainColour").ToString()
            Dim chainLength As String = myData.Tables(0).Rows(0).Item("CLength").ToString()

            ddlBlindType.SelectedValue = blindId : ddlBlindType.Enabled = False
            ddlBracketType.SelectedValue = bracketName : ddlBracketType.Enabled = False
            ddlTubeType.SelectedValue = tubeType : ddlTubeType.Enabled = True
            ddlControlType.SelectedValue = controlType : ddlControlType.Enabled = True
            ddlColourType.SelectedValue = kitId : ddlColourType.Enabled = True

            lblUniqueId.Text = myData.Tables(0).Rows(0).Item("UniqueId").ToString()

            Dim blindNo As String = myData.Tables(0).Rows(0).Item("BlindNo").ToString()
            lblBlindNo.Text = blindNo

            Call BindBlindType()
            Call BindBracket(blindId)
            Call BindTubeType(blindId, bracketName)
            Call BindControlType(blindId, bracketName, tubeType)
            Call BindColour(blindId, bracketName, tubeType, controlType)

            Call BindFabricType()
            Call BindFabricColour(fabricType)

            Call BindTrim(blindId, tubeType)
            Call BindTubeSize(blindId, tubeType)

            Call BindRailType(bracketName)
            Call BindRailColour(bracketName, bottomType)

            Call BindMotorStyle(controlType)
            Call BindRemoteMotor(controlType)
            Call BindChargerMotor(controlType, motorStyle)
            Call BindExternalBattery()
            Call BindExtras(blindId, controlType, motorStyle)

            If Session("itemAction") = "NextItem" Then
                '#Double, Linked 2B (Dep), Linked 2B (Ind)
                If blindNo = "Blind 1" And (bracketName = "Double" Or bracketName = "Linked 2 Blinds (Dep)" Or bracketName = "Linked 2 Blinds (Ind)") Then
                    lblBlindNo.Text = "Blind 2"
                End If

                '#Double, Linked 3B (Dep), Linked 3B (Ind)
                If bracketName = "Linked 3 Blinds (Dep)" Or bracketName = "Linked 3 Blinds (Ind)" Then
                    If blindNo = "Blind 1" Then
                        lblBlindNo.Text = "Blind 2"
                    End If
                    If blindNo = "Blind 2" Then
                        lblBlindNo.Text = "Blind 3"
                    End If
                End If

                '#DB Linked 2B (Dep), DB Linked 2B (Ind)
                If bracketName = "Double and Link System Dep" Or bracketName = "Double and Link System Ind" Then
                    If blindNo = "Blind 1" Then
                        lblBlindNo.Text = "Blind 2"
                    End If
                    If blindNo = "Blind 2" Then
                        lblBlindNo.Text = "Blind 3"
                    End If
                    If blindNo = "Blind 3" Then
                        lblBlindNo.Text = "Blind 4"
                    End If
                End If
            End If

            txtQty.Text = myData.Tables(0).Rows(0).Item("Qty").ToString()
            txtLocation.Text = myData.Tables(0).Rows(0).Item("Location").ToString()
            ddlMounting.SelectedValue = myData.Tables(0).Rows(0).Item("Mounting").ToString() : ddlMounting.Enabled = True

            If bracketName = "Double" Or InStr(bracketName, "Linked") > 0 Then
                ddlTubeType.Enabled = False
                ddlControlType.Enabled = False
                ddlColourType.Enabled = False
            End If

            ddlControlPosition.SelectedValue = myData.Tables(0).Rows(0).Item("ControlPosition").ToString()
            ddlChainColour.SelectedValue = chainColour
            txtChainLength.Text = ""
            txtChainLength.Text = myData.Tables(0).Rows(0).Item("ChainLength").ToString()

            ddlRoll.SelectedValue = myData.Tables(0).Rows(0).Item("RollDirection").ToString()
            ddlFabricType.SelectedValue = fabricType
            ddlFabricColour.SelectedValue = myData.Tables(0).Rows(0).Item("FabricId").ToString()

            txtWidth.Text = myData.Tables(0).Rows(0).Item("Width").ToString()
            txtDrop.Text = myData.Tables(0).Rows(0).Item("Drop").ToString()

            ddlMotorStyle.SelectedValue = myData.Tables(0).Rows(0).Item("MotorStyle").ToString()
            ddlMotorRemote.SelectedValue = myData.Tables(0).Rows(0).Item("MotorRemote").ToString()
            ddlExternalBattery.SelectedValue = myData.Tables(0).Rows(0).Item("MotorBattery").ToString()
            ddlMotorCharger.SelectedValue = myData.Tables(0).Rows(0).Item("MotorCharger").ToString()
            ddlConnector.SelectedValue = myData.Tables(0).Rows(0).Item("Connector").ToString()
            ddlCableExitPoint.SelectedValue = myData.Tables(0).Rows(0).Item("CableExitPoint").ToString()

            ' If tubeType = "N/A" Then
            '     ddlTrimSkin.SelectedValue = myData.Tables(0).Rows(0).Item("Trim").ToString()
            ' End If
            ddlTrimSkin.SelectedValue = myData.Tables(0).Rows(0).Item("Trim").ToString()
            If Not tubeType = "N/A" Then
                ddlTrim.SelectedValue = myData.Tables(0).Rows(0).Item("Trim").ToString()
            End If
            ddlRailType.SelectedValue = bottomType
            ddlRailColour.SelectedValue = myData.Tables(0).Rows(0).Item("BottomRailId").ToString()
            ddlTubeSize.SelectedValue = myData.Tables(0).Rows(0).Item("TubeSize").ToString()
            lblTubeSize.Text = myData.Tables(0).Rows(0).Item("TubeSize").ToString()
            ddlChildSafe.SelectedValue = myData.Tables(0).Rows(0).Item("ChildSafe").ToString()
            ddlExtras.SelectedValue = myData.Tables(0).Rows(0).Item("AdditionalMotor").ToString()
            ddlAccessory.SelectedValue = myData.Tables(0).Rows(0).Item("Accessory").ToString()
            ddlBracketCover.SelectedValue = myData.Tables(0).Rows(0).Item("BracketCover").ToString()
            ddlBracketExt.SelectedValue = myData.Tables(0).Rows(0).Item("BracketExtension").ToString()

            txtNotes.Text = myData.Tables(0).Rows(0).Item("Notes").ToString()
            txtMarkUp.Text = myData.Tables(0).Rows(0).Item("MarkUp").ToString()
            If txtMarkUp.Text = "0" Then : txtMarkUp.Text = "" : End If

            Call BindComponentForm(ddlColourType.SelectedValue)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataItem", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindComponentForm(Data As String)
        Try
            divDetail.Visible = False

            divBracketType.Visible = False
            divTubeType.Visible = False
            divControlType.Visible = False
            divColourType.Visible = False

            divMarkUp.Visible = False
            If Session("MarkUpAccess") = True Then : divMarkUp.Visible = True : End If

            lblBracketType.Text = "BRACKET TYPE"
            lblControlPosition.Text = "CONTROL POSITION"
            lblTubeType.Text = "MECHANISM TYPE"
            lblColourType.Text = "COLOUR TYPE"
            

            If Not ddlBlindType.SelectedValue = "" Then
                Dim blindName As String = publicCfg.GetBlindName(ddlBlindType.SelectedValue)
                If blindName = "Cassette" Then
                    lblBracketType.Text = "CASSETTE TYPE"
                End If

                ' If blindName = "Skin Only" Then
                '     lblBracketType.Text = "SKIN TYPE"
                '     If InStr(ddlBracketType.SelectedValue, "Tube") > 0 Then
                '         divTubeType.Visible = True
                '         lblTubeType.Text = "TUBE SKIN TYPE"
                '         lblColourType.Text = "TUBE SKIN COLOUR"
                '     End If
                ' End If

                

                If Not blindName = "Skin Only" Then
                    divBracketType.Visible = True
                    divTubeType.Visible = True
                End If
                divBracketType.Visible = True
                
                If Not ddlTubeType.SelectedValue = "" And Not ddlTubeType.SelectedValue = "N/A" And Not ddlTubeType.SelectedValue = "Spring Operated" Then
                    If Not blindName = "Skin Only" Then
                        divControlType.Visible = True
                    End If
                    divColourType.Visible = True
                End If
            End If

            

            If Not Data = "" Then
                divDetail.Visible = True

                divAttention.Visible = False
                divMounting.Visible = False
                divMounting.Visible = False
                divTubeSize.Visible = False
                divFabricType.Visible = False : divFabricColour.Visible = False
                divTrim.Visible = False : divTrimSkin.Visible = False : divBottomRail.Visible = False
                divControlPosition.Visible = False : divRollDirection.Visible = False
                divChainColour.Visible = False : divChainLength.Visible = False

                ' MOTOR BLIND
                divMotorStyle.Visible = False
                divMotorRemote.Visible = False
                divMotorBattery.Visible = False
                divMotorCharger.Visible = False
                divCableExitPoint.Visible = False
                divConnector.Visible = False

                divAdditional.Visible = False
                divChildSafe.Visible = False
                divAccessory.Visible = False
                divExtras.Visible = False
                divBracketCover.Visible = False
                divBracketExt.Visible = False

                Dim blindName As String = publicCfg.GetBlindName(ddlBlindType.SelectedValue)
                Dim bracketType As String = ddlBracketType.SelectedValue
                Dim tubeType As String = ddlTubeType.SelectedValue
                Dim controlType As String = ddlControlType.SelectedValue

                Dim blindNo As String = lblBlindNo.Text

                If blindName = "Skin Only" Then
                    divFabricType.Visible = True : divFabricColour.Visible = True
                    divTrimSkin.Visible = True
                    divBottomRail.Visible = False

                    If ddlTrimSkin.SelectedValue = "1F" Then
                        divBottomRail.Visible = True
                        divAccessory.Visible = False
                    End If

                    If ddlTrimSkin.SelectedValue <> "1F" Then
                        ddlRailType.SelectedValue = ""
                        ddlRailColour.SelectedValue = ""
                    End If

                    If InStr(ddlBracketType.SelectedValue, "Tube") > 0 Then
                        divTubeSize.Visible = True
                    End If

                End If

                If blindName = "Roller Blind" Or blindName = "Motorised" Or blindName = "Cassette" Then
                    divMounting.Visible = True
                    divControlPosition.Visible = True
                    divFabricType.Visible = True : divFabricColour.Visible = True
                    divTubeSize.Visible = False
                    divRollDirection.Visible = True
                    divTrim.Visible = True : divBottomRail.Visible = False

                    If blindName = "Roller Blind" Then
                        divTubeSize.Visible = True
                    End If

                    If blindName = "Motorised" Then
                        lblControlPosition.Text = "MOTOR SIDE"
                        divExtras.Visible = True
                    End If

                    If TubeType = "Motorised" Then
                        divExtras.Visible = True
                    End If

                    divChildSafe.Visible = True
                    divAccessory.Visible = True
                    divBracketCover.Visible = True
                    divAdditional.Visible = True
                    divBracketExt.Visible = True
                    If bracketType = "Double" Then
                        divBracketExt.Visible = False
                    End If

                    divChainColour.Visible = True : divChainLength.Visible = True
                    divMotorStyle.Visible = True : divMotorStyleInfo.Visible = False
                    divMotorRemote.Visible = True : divMotorRemoteInfo.Visible = False
                     If InStr(ddlMotorStyle.SelectedValue, "EXB") > 0 Then
                        divMotorBattery.Visible = True
                    End If
                    divMotorCharger.Visible = True
                    divConnector.Visible = True
                    divCableExitPoint.Visible = True

                   


                    If ddlTrim.SelectedValue = "1F" Then
                        divBottomRail.Visible = True
                        divAccessory.Visible = False
                    End If

                    If ddlTrim.SelectedValue <> "1F" Then
                        ddlRailType.SelectedValue = ""
                        ddlRailColour.SelectedValue = ""
                    End If

                    '#update items
                    If Session("itemAction") = "EditItem" Then
                        Dim blinds As String = "first blind"
                        If blindNo = "Blind 2" Then : blinds = "second blind" : End If
                        If blindNo = "Blind 3" Then : blinds = "third blind" : End If
                        If blindNo = "Blind 4" Then : blinds = "fourth blind" : End If

                        '# Double , Linked 2B (Dep) & Linked 2B (Ind)
                        If bracketType = "Double" Or bracketType = "Linked 2 Blinds (Dep)" Or bracketType = "Linked 2 Blinds (Ind)" Then
                            Dim totalBlind As Integer = publicCfg.GetItemData("SELECT COUNT(*) FROM OrderDetails WHERE UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")

                            If totalBlind > 1 Then
                                divAttention.Visible = True
                                Dim connectId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + lblUniqueId.Text + "'")
                                If blindNo = "Blind 2" Then
                                    connectId = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + lblUniqueId.Text + "'")
                                End If
                                lblNextDesc.Text = "This is the " & blinds & " for your order. If you change the location, mounting, blind size, tube size, childsafe, accessory, then the data on the <=b><u>ITEM ID " & connectId & "</u></=b>  blind will automatically be changed according to this data."
                            End If
                        End If

                        '# Linked 3B (Dep)& Linked 3B (Ind)
                        If bracketType = "Linked 3 Blinds (Dep)" Or bracketType = "Linked 3 Blinds (Ind)" Then
                            Dim totalBlind As Integer = publicCfg.GetItemData("SELECT COUNT(*) FROM OrderDetails WHERE UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")

                            If totalBlind > 1 Then
                                divAttention.Visible = True
                                Dim connectId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + lblUniqueId.Text + "'")
                                Dim connectId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + lblUniqueId.Text + "'")

                                Dim blindId As String = connectId
                                If Not connectId2 = "" Then
                                    blindId += " AND ITEM ID " & connectId2
                                End If

                                If blindNo = "Blind 2" Then
                                    connectId = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + lblUniqueId.Text + "'")
                                    connectId2 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + lblUniqueId.Text + "'")
                                    blindId = connectId
                                    If Not connectId2 = "" Then
                                        blindId += " AND ITEM ID " & connectId2
                                    End If
                                End If

                                If blindNo = "Blind 3" Then
                                    connectId = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + lblUniqueId.Text + "'")
                                    connectId2 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + lblUniqueId.Text + "'")
                                    blindId = connectId & " AND ITEM ID " & connectId2
                                End If

                                lblNextDesc.Text = "This is the " & blinds & " for your order. If you change the location, mounting, blind size, tube size, childsafe, accessory, bracket cover and bracket extension,  then the data on the <b><u>ITEM ID " & blindId & "</u></b>  blind will automatically be changed according to this data."
                            End If
                        End If

                        '#Double and Link System Dep & Double and Link System Ind
                        If bracketType = "Double and Link System Dep" Or bracketType = "Double and Link System Ind" Then
                            '#count blinds
                            Dim totalBlind As Integer = publicCfg.GetItemData("SELECT COUNT(*) FROM OrderDetails WHERE UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")

                            If totalBlind > 1 Then
                                divAttention.Visible = True
                                Dim connectId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                Dim connectId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                Dim connectId3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")

                                Dim blindId As String = connectId
                                If Not connectId2 = "" Then
                                    blindId += " AND ITEM ID " & connectId2
                                End If
                                If Not connectId3 = "" Then
                                    blindId += ", ITEM ID " & connectId2 & " AND ITEM ID " & connectId3
                                End If

                                '#blinds 2
                                If blindNo = "Blind 2" Then
                                    connectId = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                    connectId2 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                    connectId3 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                    blindId = connectId
                                    If Not connectId2 = "" Then
                                        blindId += " AND ITEM ID " & connectId2
                                    End If
                                    If Not connectId3 = "" Then
                                        blindId += ", ITEM ID " & connectId2 & " AND ITEM ID " & connectId3
                                    End If
                                End If

                                '#blinds 3
                                If blindNo = "Blind 3" Then
                                    connectId = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                    connectId2 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                    connectId3 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                    blindId = connectId
                                    If Not connectId2 = "" Then
                                        blindId += " AND ITEM ID " & connectId2
                                    End If
                                    If Not connectId3 = "" Then
                                        blindId += ", ITEM ID " & connectId2 & " AND ITEM ID " & connectId3
                                    End If
                                End If

                                '#blinds 4
                                If blindNo = "Blind 4" Then
                                    connectId = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                    connectId2 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                    connectId3 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + lblUniqueId.Text + "' AND Active = 1")
                                    blindId = connectId
                                    ' If Not connectId2 = "" Then
                                    '     blindId += " AND ITEM ID " & connectId2
                                    ' End If
                                    If Not connectId3 = "" Then
                                        blindId += ", ITEM ID " & connectId2 & " AND ITEM ID " & connectId3
                                    End If
                                End If

                                lblNextDesc.Text = "This is the <b>" & blinds & "</b> for your order. If you change the location, mounting, blind size, tube size, childsafe, accessory then the data on the <b><u>ITEM ID " & blindId & "</u></b>  blind will automatically be changed according to this data."

                                
                            End If
                        End If

                    End If


                    '#next items
                    If Session("itemAction") = "NextItem" Then
                        divAttention.Visible = True
                        Dim blinds As String = "second blind"
                        If blindNo = "Blind 3" Then : blinds = "third blind" : End If
                        If blindNo = "Blind 4" Then : blinds = "fourth blind" : End If

                        Dim connectId As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + lblUniqueId.Text + "'")
                        Dim connectId2 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + lblUniqueId.Text + "'")
                        Dim connectId3 As String = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + lblUniqueId.Text + "'")

                        Dim blindId As String = connectId
                        If Not connectId2 = "" Then
                            blindId += " AND ITEM ID " & connectId2
                        End If
                        If Not connectId3 = "" Then
                            blindId += ", ITEM ID " & connectId2 & " AND ITEM ID " & connectId3
                        End If

                        If blindNo = "Blind 3" Then
                            connectId = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + lblUniqueId.Text + "'")
                            connectId2 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + lblUniqueId.Text + "'")
                            connectId3 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId = '" + lblUniqueId.Text + "'")
                            blindId = connectId & " AND ITEMmm ID " & connectId2 & " AND ITEM ID " & connectId3
                        End If

                        If blindNo = "Blind 4" Then
                            connectId = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId = '" + lblUniqueId.Text + "'")
                            connectId2 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId = '" + lblUniqueId.Text + "'")
                            connectId3 = publicCfg.GetItemData("SELECT Id FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId = '" + lblUniqueId.Text + "'")
                            blindId = connectId & " AND ITEM ID " & connectId2 & " AND ITEM ID " & connectId3
                        End If
                        lblNextDesc.Text = "This is the <b><u>" & blinds & "</b></u> for your order. If you change the location, mounting, blind size, tube size, childsafe, accessory, then the data on the <b><u>ITEM ID " & connectId & "</u></b>  blind will automatically be changed according to this data."
                    End If

                    If controlType = "Chain" Then
                        divMotorStyle.Visible = False : divMotorStyleInfo.Visible = False
                        divMotorRemote.Visible = False
                        divMotorBattery.Visible = False
                        divMotorCharger.Visible = False
                        divConnector.Visible = False
                        divCableExitPoint.Visible = False
                    End If

                    If InStr(controlType, "Alpha") > 0 Or InStr(controlType, "Somfy") > 0 Then
                        divChainColour.Visible = False : divChainLength.Visible = False
                        divChildSafe.Visible = False
                        ' divBracketExt.Visible = False
                        divCableExitPoint.Visible = False

                        If controlType = "Alpha WF" Or controlType = "Somfy WF" Then
                            divConnector.Visible = False
                        End If
                        If controlType = "Alpha RTS" Or controlType = "Alpha WS" Or controlType = "Somfy RTS" Or controlType = "Somfy WS" Then
                            divMotorBattery.Visible = False
                            divMotorCharger.Visible = False
                        End If

                        If blindName = "Cassette" Then
                            If controlType = "Alpha RTS" Or controlType = "Alpha WS" Or controlType = "Somfy RTS" Or controlType = "Somfy WS" Then
                                divCableExitPoint.Visible = True
                            End If
                        End If
                    End If

                    If blindName = "Cassette" Then
                        lblControlPosition.Text = "CONTROL SIDE"
                    End If

                    If tubeType = "Spring Operated" Then
                        divControlPosition.Visible = False
                        divChainColour.Visible = False : divChainLength.Visible = False
                        divBracketCover.Visible = False
                        divBracketExt.Visible = False : divBottomRail.Visible = False

                        divMotorStyle.Visible = False : divMotorStyleInfo.Visible = False
                        divMotorRemote.Visible = False : divMotorRemoteInfo.Visible = False
                        divMotorBattery.Visible = False
                        divMotorCharger.Visible = False
                        divConnector.Visible = False
                        divCableExitPoint.Visible = False
                        divRollDirection.Visible = False
                    End If
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindComponentForm", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindBlindType()
        ddlBlindType.Items.Clear()
        Try
            ddlBlindType.DataSource = publicCfg.GetListData("SELECT UPPER(Name) AS NameText, * FROM Blinds WHERE DesignId = '" + designId + "' AND Company = 'SP' AND Active=1 ORDER BY Name ASC")
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
                publicCfg.MailError(Session("UserId"), Page.Title, "BindBlindType", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindBracket(BlindId As String)
        ddlBracketType.Items.Clear()
        Try
            If Not BlindId = "" Then
                ddlBracketType.DataSource = publicCfg.GetListData("SELECT UPPER(BracketType) AS BracketValue, BracketType AS BracketText FROM HardwareKits WHERE BlindId = '" + BlindId + "' AND Active=1 GROUP BY BracketType ORDER BY BracketType ASC")
                ddlBracketType.DataTextField = "BracketValue"
                ddlBracketType.DataValueField = "BracketText"
                ddlBracketType.DataBind()

                If ddlBracketType.Items.Count > 1 Then
                    ddlBracketType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindBracket", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindTubeType(BlindId As String, BracketType As String)
        ddlTubeType.Items.Clear()
        Try
            If Not BlindId = "" And Not BracketType = "" Then
                ddlTubeType.DataSource = publicCfg.GetListData("SELECT UPPER(TubeType) AS TubeValue, TubeType AS TubeText FROM HardwareKits WHERE BlindId = '" + BlindId + "' AND BracketType = '" + BracketType + "' AND Active=1 GROUP BY TubeType ORDER BY TubeType ASC")
                ddlTubeType.DataTextField = "TubeValue"
                ddlTubeType.DataValueField = "TubeText"
                ddlTubeType.DataBind()

                If ddlTubeType.Items.Count > 1 Then
                    ddlTubeType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindTubeType", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindControlType(BlindId As String, Bracket As String, TubeType As String)
        ddlControlType.Items.Clear()
        Try
            If Not BlindId = "" And Not Bracket = "" And Not TubeType = "" Then
                ddlControlType.DataSource = publicCfg.GetListData("SELECT UPPER(ControlType) AS ControlText, ControlType AS ControlValue FROM HardwareKits WHERE BlindId = '" + BlindId + "' AND BracketType = '" + Bracket + "' AND TubeType = '" + TubeType + "' AND Active=1 GROUP BY ControlType ORDER BY ControlType ASC")
                ddlControlType.DataTextField = "ControlText"
                ddlControlType.DataValueField = "ControlValue"
                ddlControlType.DataBind()

                If ddlControlType.Items.Count > 1 Then
                    ddlControlType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindControlType", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindColour(Blind As String, Bracket As String, Tube As String, Control As String)
        ddlColourType.Items.Clear()
        Try
            If Not Blind = "" And Not Bracket = "" Then
                ddlColourType.DataSource = publicCfg.GetListData("SELECT *, UPPER(ColourType) AS ColourText FROM HardwareKits WHERE BlindId = '" + UCase(Blind).ToString() + "' AND BracketType = '" + Bracket + "' AND TubeType = '" + Tube + "' AND ControlType='" + Control + "' AND Active=1 ORDER BY Name ASC")
                ddlColourType.DataTextField = "ColourText"
                ddlColourType.DataValueField = "Id"
                ddlColourType.DataBind()

                If ddlColourType.Items.Count > 1 Then
                    ddlColourType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindColour", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindMotorStyle(MotorType As String)
        ddlMotorStyle.Items.Clear()
        Try
            If Not MotorType = "" Then
                If MotorType = "Somfy RTS" Then
                    ddlMotorStyle.Items.Add(New ListItem("ALTUS 40 RTS", "Altus 40 RTS"))
                    ddlMotorStyle.Items.Add(New ListItem("ALTUS 50 RTS", "Altus 50 RTS"))
                    ddlMotorStyle.Items.Add(New ListItem("SONESSE 40 RTS", "Sonesse 40 RTS"))
                    ddlMotorStyle.Items.Add(New ListItem("SON 40 RTS ZB", "Son 40 RTS ZB"))
                End If

                If MotorType = "Somfy WF" Then
                    ddlMotorStyle.Items.Add(New ListItem("ALTUS 28 WF", "Altus 28 WF"))
                    ddlMotorStyle.Items.Add(New ListItem("ALTUS 28 EXB", "Altus 28 EXB"))
                    ddlMotorStyle.Items.Add(New ListItem("SON 28 WF ZB", "Son 28 WF ZB"))
                    ddlMotorStyle.Items.Add(New ListItem("SON 28 WF ZBEXB", "Son 28 WF ZBEXB"))
                    ddlMotorStyle.Items.Add(New ListItem("SONESSE 30 WF", "Sonesse 30 WF"))
                    ddlMotorStyle.Items.Add(New ListItem("SONESSE 40 WF", "Sonesse 40 WF"))
                    ddlMotorStyle.Items.Add(New ListItem("SON 40 WF ZB", "Son 40 WF ZB"))
                End If

                If MotorType = "Somfy WS" Then
                    ddlMotorStyle.Items.Add(New ListItem("MECURE LS 40", "Mecure LS 40"))
                    ddlMotorStyle.Items.Add(New ListItem("SONESSE 40 WT", "Sonesse 40 WT"))
                    ' ddlMotorStyle.Items.Add(New ListItem("SONESSE 50 WT", "Sonesse 50 WT"))
                End If

                If MotorType = "Alpha RTS" Then
                    ddlMotorStyle.Items.Add(New ListItem("WSER 40 UNIVERSAL", "WSER 40 Universal"))
                End If

                If MotorType = "Alpha WF" Then
                    ddlMotorStyle.Items.Add(New ListItem("ALPHA 1NM SML", "Alpha 1NM Sml"))
                    ddlMotorStyle.Items.Add(New ListItem("ALPHA 2NM STD", "Alpha 2NM Std"))
                    ddlMotorStyle.Items.Add(New ListItem("ALPHA 3NM HD", "Alpha 3NM HD"))
                End If

                If MotorType = "Alpha WS" Then
                    ddlMotorStyle.Items.Add(New ListItem("WSEC 40 UNIVERSAL", "WSEC 40 Universal"))
                    ddlMotorStyle.Items.Add(New ListItem("WSS40 ALLEN KEY", "WSS40 Allen Key"))
                End If
            End If

            If ddlMotorStyle.Items.Count > 0 Then
                ddlMotorStyle.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindMotorStyle", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindRemoteMotor(MotorType As String)
        ddlMotorRemote.Items.Clear()
        Try
            If Not MotorType = "" Then
                If MotorType = "Somfy RTS" Or MotorType = "Somfy WF" Then
                    ddlMotorRemote.Items.Clear()
                    ddlMotorRemote.Items.Add(New ListItem("1 SITUO (1 CH)", "1 Situo (1 ch)"))
                    ddlMotorRemote.Items.Add(New ListItem("4 SITUO (5 CH)", "4 Situo (5ch)"))
                    ddlMotorRemote.Items.Add(New ListItem("TELIS (16 CH)", "Telis (16 ch)"))
                    If MotorType = "Somfy RTS" Then
                        ddlMotorRemote.Items.Add(New ListItem("SM O (W+FRAME)", "Sm O (w+frame)"))
                        ddlMotorRemote.Items.Add(New ListItem("SM O 2CH (W+FRAME)", "Sm O 2ch (w+frame)"))
                        ddlMotorRemote.Items.Add(New ListItem("SM O 4CH (W+FRAME)", "Sm O 4ch (w+frame)"))
                    End If
                    ddlMotorRemote.Items.Add(New ListItem("YSIA ZB (1 CH)", "Ysia ZB (1 ch)"))
                    ddlMotorRemote.Items.Add(New ListItem("YSIA ZB (5 CH)", "Ysia ZB (5 ch)"))
                    ddlMotorRemote.Items.Add(New ListItem("CONNEXOON", "Connexoon"))
                    ddlMotorRemote.Items.Add(New ListItem("TAHOMA WIFI BOX", "Tahoma Wifi Box"))
                    ddlMotorRemote.Items.Add(New ListItem("E-ADAPTOR TAHOMA", "E-Adaptor Tahoma"))
                End If

                If MotorType = "Somfy WS" Then
                    ddlMotorRemote.Items.Clear()
                    ddlMotorRemote.Items.Add(New ListItem("SM UNO (+FRAME)", "Sm Uno (+frame)"))
                    ddlMotorRemote.Items.Add(New ListItem("SM DUO (+FRAME)", "Sm Duo (+frame)"))
                    ddlMotorRemote.Items.Add(New ListItem("TRIPLE TOGGLE SWITCH", "Triple Toggle Switch"))
                End If

                If MotorType = "Alpha RTS" Or MotorType = "Alpha WF" Then
                    ddlMotorRemote.Items.Clear()
                    ddlMotorRemote.Items.Add(New ListItem("PIONEER 1 CHANNEL", "Pioneer 1 Channel"))
                    ddlMotorRemote.Items.Add(New ListItem("PIONEER 4 CHANNELS", "Pioneer 4 Channels"))
                    ddlMotorRemote.Items.Add(New ListItem("PIONEER 16 CHANNELS", "Pioneer 16 Channels"))
                    ddlMotorRemote.Items.Add(New ListItem("NAVIGATOR 1 CHANNEL", "Navigator 1 Channel"))
                    ddlMotorRemote.Items.Add(New ListItem("NAVIGATOR 5 CHANNELS", "Navigator 5 Channels"))
                    ddlMotorRemote.Items.Add(New ListItem("NAVIGATOR 16 CHANNELS", "Navigator 16 Channels"))
                    ddlMotorRemote.Items.Add(New ListItem("1 CH WALL", "1 Ch Wall"))
                    ddlMotorRemote.Items.Add(New ListItem("8 CH WALL", "8 Ch Wall"))
                    ddlMotorRemote.Items.Add(New ListItem("NEO LINK BOX", "Neo Link Box"))
                End If

                If MotorType = "Alpha WS" Then
                    ddlMotorRemote.Items.Clear()
                    ddlMotorRemote.Items.Add(New ListItem("MT PADDLE (4C)", "Mt Paddle (4c)"))
                    ddlMotorRemote.Items.Add(New ListItem("NEO LINK BOX", "Neo Link Box"))
                End If
            End If

            If ddlMotorRemote.Items.Count > 0 Then
                ddlMotorRemote.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindRemoteMotor", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindExternalBattery()
        ddlExternalBattery.Items.Clear()
        Try
            ddlExternalBattery.Items.Add(New ListItem("YES", "Yes"))
            If ddlExternalBattery.Items.Count > 0 Then
                ddlExternalBattery.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindExternalBattery", ex.ToString())
            End If
        End Try
    End Sub


    Private Sub BindChargerMotor(MotorType As String, MotorStyle As String)
        ddlMotorCharger.Items.Clear()
        Try
            If Not MotorType = "" Then
                If MotorType = "Somfy WF" Then
                    If InStr(MotorStyle, "ZB") > 0 Then
                        ddlMotorCharger.Items.Clear()
                        ddlMotorCharger.Items.Add(New ListItem("USB-C", "USB-C"))
                    Else
                        ddlMotorCharger.Items.Clear()
                        ddlMotorCharger.Items.Add(New ListItem("YES", "Yes"))
                    End If
                End If

                If MotorType = "Alpha WF" Then
                    If MotorStyle = "Alpha 1NM Sml" Then
                        ddlMotorCharger.Items.Clear()
                        ddlMotorCharger.Items.Add(New ListItem("ALPHA", "Alpha"))
                        ddlMotorCharger.Items.Add(New ListItem("USB-C", "USB-C"))
                    End If
                    
                    If MotorStyle = "Alpha 2NM Std" Then
                        ddlMotorCharger.Items.Clear()
                        ddlMotorCharger.Items.Add(New ListItem("ALPHA 2NM (C)", "Alpha 2NM (C)"))
                        ddlMotorCharger.Items.Add(New ListItem("USB-C", "USB-C"))
                    End If

                    If MotorStyle = "Alpha 3NM HD" Then
                        ddlMotorCharger.Items.Clear()
                        ddlMotorCharger.Items.Add(New ListItem("ALPHA 3NM (old)", "Alpha 3NM (old)"))
                    End If
                End If
            End If

            If ddlMotorCharger.Items.Count > 0 Then
                ddlMotorCharger.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindChargerMotor", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindFabricType()
        ddlFabricType.Items.Clear()
        Try
            Dim thisQuery As String = "SELECT Type AS TypeValue, UPPER(Type) AS TypeText FROM Fabrics WHERE DesignId='50CE8EDF-E106-414C-BDE3-D7AA8F8046D2' AND Active='1' GROUP BY Type ORDER BY Type ASC"
            ddlFabricType.DataSource = publicCfg.GetListData(thisQuery)
            ddlFabricType.DataTextField = "TypeText"
            ddlFabricType.DataValueField = "TypeValue"
            ddlFabricType.DataBind()
            If ddlFabricType.Items.Count > 1 Then
                ddlFabricType.Items.Insert(0, New ListItem("", ""))
            End If

        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindFabricType", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindFabricColour(Type As String)
        ddlFabricColour.Items.Clear()
        Try
            If Not Type = "" Then
                ddlFabricColour.DataSource = publicCfg.GetListData("SELECT Id, UPPER(Colour) AS ColourText FROM Fabrics WHERE DesignId='50CE8EDF-E106-414C-BDE3-D7AA8F8046D2' AND Type='" + Type + "' AND Active='1' ORDER BY Colour ASC")
                ddlFabricColour.DataTextField = "ColourText"
                ddlFabricColour.DataValueField = "Id"
                ddlFabricColour.DataBind()
                If ddlFabricColour.Items.Count > 1 Then
                    ddlFabricColour.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindFabricColour", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindTrim(BlindId As String, TubeType As String)
        ddlTrim.Items.Clear() : ddlTrimSkin.Items.Clear()
        Try
            If Not BlindId = "" Or Not TubeType = "" Then
                
                Dim blindName As String = publicCfg.GetBlindName(BlindId)
                Dim skinType As String = ddlBracketType.SelectedValue

                If blindName = "Roller Blind" Or blindName = "Motorised" Or blindName = "Cassette" Then
                    ddlTrim.Items.Add(New ListItem("1P", "1P"))
                    ddlTrim.Items.Add(New ListItem("1F", "1F"))
                    ddlTrim.Items.Add(New ListItem("5F", "5F"))
                    ddlTrim.Items.Add(New ListItem("7F", "7F"))
                    ddlTrim.Items.Add(New ListItem("9F", "9F"))
                    ddlTrim.Items.Add(New ListItem("10F", "10F"))
                    ddlTrim.Items.Add(New ListItem("12F", "12F"))
                    ddlTrim.Items.Add(New ListItem("15F", "15F"))
                    ddlTrim.Items.Add(New ListItem("17F", "17F"))
                    ddlTrim.Items.Add(New ListItem("18F", "18F"))
                    ddlTrim.Items.Add(New ListItem("19F", "19F"))
                    ddlTrim.Items.Add(New ListItem("20F", "20F"))
                    ddlTrim.Items.Add(New ListItem("22F", "22F"))
                    ddlTrim.Items.Add(New ListItem("23F", "23F"))
                    ddlTrim.Items.Add(New ListItem("24F", "24F"))
                    ddlTrim.Items.Add(New ListItem("25F", "25F"))
                    ddlTrim.Items.Add(New ListItem("26F", "26F"))

                    If TubeType = "Spring Operated" Then
                        ddlTrim.Items.Clear()

                        ddlTrim.Items.Add(New ListItem("1P", "1P"))
                        ddlTrim.Items.Add(New ListItem("17F", "17F"))
                        ddlTrim.Items.Add(New ListItem("18F", "18F"))
                        ddlTrim.Items.Add(New ListItem("19F", "19F"))
                        ddlTrim.Items.Add(New ListItem("20F", "20F"))
                        ddlTrim.Items.Add(New ListItem("22F", "22F"))
                        ddlTrim.Items.Add(New ListItem("23F", "23F"))
                        ddlTrim.Items.Add(New ListItem("24F", "24F"))
                        ddlTrim.Items.Add(New ListItem("25F", "25F"))
                        ddlTrim.Items.Add(New ListItem("26F", "26F"))
                    End If

                    If ddlTrim.Items.Count > 1 Then
                        ddlTrim.Items.Insert(0, New ListItem("", ""))
                    End If
                End If

                If blindName = "Skin Only" Then
                    ddlTrimSkin.Items.Clear()

                    If skinType = "Excluded" Or skinType = "With Tube Included" Then
                        ddlTrimSkin.Items.Add(New ListItem("1P", "1P"))
                        ddlTrimSkin.Items.Add(New ListItem("SPLINE", "Spline"))
                    End If

                    If skinType = "Excluded" Then
                        ddlTrimSkin.Items.Add(New ListItem("POCKET", "Pocket"))
                        ddlTrimSkin.Items.Add(New ListItem("1RS", "1RS"))
                        ddlTrimSkin.Items.Add(New ListItem("1OS", "1OS"))
                        ddlTrimSkin.Items.Add(New ListItem("ADDED TRIM", "Added Trim"))
                    End If

                    If skinType = "With Tube & Bottom Included" Or skinType = "With Bottom Included" Then
                        ddlTrimSkin.Items.Add(New ListItem("1P", "1P"))
                        ddlTrimSkin.Items.Add(New ListItem("1F", "1F"))
                        ddlTrimSkin.Items.Add(New ListItem("5F", "5F"))
                        ddlTrimSkin.Items.Add(New ListItem("7F", "7F"))
                        ddlTrimSkin.Items.Add(New ListItem("9F", "9F"))
                        ddlTrimSkin.Items.Add(New ListItem("10F", "10F"))
                        ddlTrimSkin.Items.Add(New ListItem("12F", "12F"))
                        ddlTrimSkin.Items.Add(New ListItem("15F", "15F"))
                        ddlTrimSkin.Items.Add(New ListItem("17F", "17F"))
                        ddlTrimSkin.Items.Add(New ListItem("18F", "18F"))
                        ddlTrimSkin.Items.Add(New ListItem("19F", "19F"))
                        ddlTrimSkin.Items.Add(New ListItem("20F", "20F"))
                        ddlTrimSkin.Items.Add(New ListItem("22F", "22F"))
                        ddlTrimSkin.Items.Add(New ListItem("23F", "23F"))
                        ddlTrimSkin.Items.Add(New ListItem("24F", "24F"))
                        ddlTrimSkin.Items.Add(New ListItem("25F", "25F"))
                        ddlTrimSkin.Items.Add(New ListItem("26F", "26F"))
                    End if
                        
                    If ddlTrimSkin.Items.Count > 1 Then
                        ddlTrimSkin.Items.Insert(0, New ListItem("", ""))
                    End If
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindTrim", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindRailType(Bracket As String)
        ddlRailType.Items.Clear()
        Try
            If Not Bracket = "" Then
                Dim FindBracket As String = Bracket

                If Bracket = "Headbox & Side Channels" Then
                    FindBracket = "Headbox &amp; Side Channels"
                End If
                
                If Bracket = "With Tube & Bottom Included" Then
                    FindBracket = "With Tube &amp; Bottom Included"
                End If

                Dim thisQuery As String = "SELECT UPPER(Type) AS TypeText, Type AS TypeValue FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '" + FindBracket + "' AND Active ='1' GROUP BY Type ORDER BY Type ASC"

                ddlRailType.DataSource = publicCfg.GetListData(thisQuery)
                ddlRailType.DataTextField = "TypeText"
                ddlRailType.DataValueField = "TypeValue"
                ddlRailType.DataBind()
                If ddlRailType.Items.Count > 0 Then
                    ddlRailType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindRailType", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindRailColour(Bracket As String, RailType As String)
        ddlRailColour.Items.Clear()
        Try
            If Not RailType = "" Then
                Dim FindBracket As String = Bracket

                If Bracket = "Headbox & Side Channels" Then
                    FindBracket = "Headbox &amp; Side Channels"
                End If

                If Bracket = "With Tube & Bottom Included" Then
                    FindBracket = "With Tube &amp; Bottom Included"
                End If

                Dim thisQuery As String = "SELECT Id, UPPER(Colour) AS Colour, VALUE Product FROM Bottoms CROSS APPLY STRING_SPLIT(BracketType, ',') WHERE VALUE = '" + FindBracket + "' AND Type='" + RailType + "' AND Active ='1' ORDER BY Name ASC"
                ddlRailColour.DataSource = publicCfg.GetListData(thisQuery)
                ddlRailColour.DataTextField = "Colour"
                ddlRailColour.DataValueField = "Id"
                ddlRailColour.DataBind()
                If ddlRailColour.Items.Count > 1 Then
                    ddlRailColour.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindRailColour", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindTubeSize(Blind As String, Control As String)
        ddlTubeSize.Items.Clear()
        Try
            If Not Blind = "" And Not Control = "" Then
                Dim blindName As String = publicCfg.GetItemData("SELECT Name FROM Blinds WHERE Id = '" + UCase(Blind).ToString() + "'")

                If blindName = "Roller Blind" Then
                    If Control = "JAI Standard" Or Control = "JAI Geared" Or Control = "LOV Standard" Or Control = "LOV Geared" Or Control = "Spring Operated" Then
                        ddlTubeSize.Items.Clear()
                        ddlTubeSize.Items.Add(New ListItem("40", "40"))
                        ddlTubeSize.Items.Add(New ListItem("45", "45"))
                        ddlTubeSize.Items.Add(New ListItem("45H", "45H"))
                    End If
                End If

                If blindName = "Skin Only" Then
                    ddlTubeSize.Items.Clear()
                    ddlTubeSize.Items.Add(New ListItem("40", "40"))
                    ddlTubeSize.Items.Add(New ListItem("45", "45"))
                    ddlTubeSize.Items.Add(New ListItem("45H", "45H"))
                End If

                If blindName = "Cassette" Then
                    ddlTubeSize.Items.Add(New ListItem("45", "45"))
                End If

                If blindName = "Motorised" Then
                    If Control = "45 JAI" Then
                        ddlTubeSize.Items.Add(New ListItem("45", "45"))
                    End If
                    If Control = "45H JAI" Then
                        ddlTubeSize.Items.Add(New ListItem("45H", "45H"))
                    End If
                    If Control = "63 Acmeda" Then
                        ddlTubeSize.Items.Add(New ListItem("63", "63"))
                    End If
                End If
            End If
            If ddlTubeSize.Items.Count > 1 Then
                ddlTubeSize.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindTubeSize", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindExtras(blindId As String, controlType As String, motorStyle As String)
        ddlExtras.Items.Clear()
        Try
            If Not blindId = "" And Not controlType = "" And Not motorStyle = "" Then            
                Dim blindName As String = publicCfg.GetBlindName(blindId)
                
                if InStr(controlType, "Somfy") > 0 Then
                    If controlType = "Somfy WF" And InStr(motorStyle, "ZB") = 0  Then
                        ddlExtras.Items.Add(New ListItem("WF LI SOLAR PANEL KIT", "WF Li Solar Panel Kit"))
                        ddlExtras.Items.Add(New ListItem("ADAPTOR MG V2 LI", "Adaptor Mg V2 Li"))
                        
                        ' If motorStyle = "Sonesse 40 WF" Then
                        ' End If
                        ddlExtras.Items.Add(New ListItem("CABLE MG RIGID", "Cable Mg Rigid")) 
                    End If


                    If blindName = "Cassette" Then
                        ddlExtras.Items.Add(New ListItem("CABLE EX 20CM CASSETTE", "Cable Ex 20cm Cassette"))
                    End If

                    If InStr(motorStyle, "ZB") > 0 Then
                        If controlType = "Somfy WF" Then
                            ddlExtras.Items.Add(New ListItem("WF LI ZB SOLAR PANEL KIT", "WF Li ZB Solar Panel Kit"))
                        End If
                        ddlExtras.Items.Add(New ListItem("CABLE ZB EX 20CM USB-C", "Cable ZB Ex 20cm USB-C"))
                        ddlExtras.Items.Add(New ListItem("ADAPTOR MG ZB USB-C CHARGER", "Adaptor Mg ZB USB-C Charger"))
                        ddlExtras.Items.Add(New ListItem("CABLE MG RIGID ZB USB-C CHARGER", "Cable Mg Rg ZB USB-C Charger"))
                    End If
                End If

                If InStr(controlType, "Alpha") Then
                    ddlExtras.Items.Add(New ListItem("LEAD EX 3M ALDC CHARGER", "Lead Ex 3M ALDC Charger"))
                End If
            End If
            
            If ddlExtras.Items.Count > 0 Then
                ddlExtras.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindExtras", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlBlindType.CssClass = "form-select "
        ddlBracketType.CssClass = "form-select "
        ddlTubeType.CssClass = "form-select "
        ddlControlType.CssClass = "form-select "
        ddlColourType.CssClass = "form-select "
        txtQty.CssClass = "form-control "
        txtLocation.CssClass = "form-control "
        ddlMounting.CssClass = "form-select "
        ddlMotorStyle.CssClass = "form-select "
        ddlMotorRemote.CssClass = "form-select "
        ddlExternalBattery.CssClass = "form-select "
        ddlMotorCharger.CssClass = "form-select "
        ddlFabricType.CssClass = "form-select "
        ddlFabricColour.CssClass = "form-select "
        ddlControlPosition.CssClass = "form-select "
        ddlChainColour.CssClass = "form-select "
        ddlRoll.CssClass = "form-select "
        ddlTrim.CssClass = "form-select "
        ddlTrimSkin.CssClass = "form-select "
        ddlRailType.CssClass = "form-select "
        ddlRailColour.CssClass = "form-select "
        txtWidth.CssClass = "form-control "
        txtDrop.CssClass = "form-control "
        txtChainLength.CssClass = "form-control "
        ddlTubeSize.CssClass = "form-select "
        ddlChildSafe.CssClass = "form-select "
        ddlExtras.CssClass = "form-select "
        ddlAccessory.CssClass = "form-select "
        ddlBracketCover.CssClass = "form-select "
        txtNotes.CssClass = "form-control"
        ddlBracketExt.CssClass = "form-select "
        txtMarkUp.CssClass = "form-control "
        ddlCableExitPoint.CssClass = "form-select "
        ddlConnector.CssClass = "form-select "
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


    '#==================================Control Function==================================#
    '#chain controll | Single, Double, & Linked 2 (Ind)
    Private Sub chainSingleDoubleLinked2Ind()
        If ddlControlPosition.SelectedValue = "" Then
            Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
            ddlControlPosition.CssClass = "form-select  is-invalid"
            ddlControlPosition.Focus()
            Exit Sub
        End If

        If ddlChainColour.SelectedValue = "" Then
            Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
            ddlControlPosition.CssClass = "form-select  is-invalid"
            ddlControlPosition.Focus()
            Exit Sub
        End If

        If Not txtChainLength.Text = "" Then
            If Not IsNumeric(txtChainLength.Text) Then
                Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
                txtChainLength.CssClass = "form-select  is-invalid"
                txtChainLength.Focus()
                Exit Sub
            End If

            If txtChainLength.Text < 1 Then
                Call MessageError(True, "PLEASE CHECK YOUR CHAIN LENGTH ORDER !")
                txtChainLength.CssClass = "form-select  is-invalid"
                txtChainLength.Focus()
                Exit Sub
            End If
        End If
    End Sub

    '#chain controll | Linked 2 Blinds (Dep)
    Private Sub chainLinked2BlindsDep()
        '#......................Blind 1...........................#
        If lblBlindNo.Text = "Blind 1" Then
            If Session("itemAction") = "AddItem" Then
                If ddlControlPosition.SelectedValue = "" And Not ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not ddlControlPosition.SelectedValue = "" And ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If
            End If

            If Session("itemAction") = "EditItem" Or Session("itemAction") = "ViewItem" Then
                Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
                If Not controlPosition = "" Then
                    If Not ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION NOT REQUIRED !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If Not ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                        ddlChainColour.CssClass = "form-select  is-invalid"
                        ddlChainColour.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                        txtChainLength.CssClass = "form-controll  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If
                End If
                If controlPosition = "" Then
                    If ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                        ddlChainColour.CssClass = "form-select  is-invalid"
                        ddlChainColour.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        If Not IsNumeric(txtChainLength.Text) Then
                            Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
            End If
        End If

        '#......................Blind 2...........................#
        If lblBlindNo.Text = "Blind 2" Then
            Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
            If Not controlPosition = "" Then
                If Not ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION NOT REQUIRED !")
                    ddlControlPosition.BackColor = Drawing.Color.Red
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                    ddlChainColour.BackColor = Drawing.Color.Red
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                    txtChainLength.BackColor = Drawing.Color.Red
                    txtChainLength.Focus()
                    Exit Sub
                End If
            End If
            If controlPosition = "" Then
                If ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
                        txtChainLength.CssClass = "form-controll  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If

                    If txtChainLength.Text < 1 Then
                        Call MessageError(True, "PLEASE CHECK YOUR CHAIN LENGTH ORDER !")
                        txtChainLength.CssClass = "form-controll  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If
                End If
            End If
        End If
    End Sub

    '#chain controll | Linked 3 Blinds (Dep)
    Private Sub chainLinked3BlindsDep()
        '#......................Blind 1...........................#
        If lblBlindNo.Text = "Blind 1" Then
            If Session("ItemAction") = "AddItem" Then
                If ddlControlPosition.SelectedValue = "" And Not ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not ddlControlPosition.SelectedValue = "" And ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If
            End If
        End If

        '#......................Blind 2...........................#
        If lblBlindNo.Text = "Blind 2" Then
            If Session("ItemAction") = "NextItem" Or Session("ItemAction") = "EditItem" Then
                '#cek control position
                Dim cekControlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
                '# is not null
                If Not cekControlPosition = "" Then
                    If Not ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION NOT REQUIRED !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If Not ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                        ddlChainColour.CssClass = "form-select  is-invalid"
                        ddlChainColour.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                        txtChainLength.CssClass = "form-control  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If
                End If
                '# is null
                If cekControlPosition = "" Then
                    If ddlControlPosition.SelectedValue = "" And Not ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If Not ddlControlPosition.SelectedValue = "" And ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                        ddlChainColour.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If
                End If
            End If
        End If
        '#......................Blind 3...........................#
        If lblBlindNo.Text = "Blind 3" Then
            '#cek control position
            Dim cekControlB1 As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
            Dim cekControlB2 As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
            '# blind 1 not null & blind 2 null
            If Not cekControlB1 = "" And cekControlB2 = "" Then
                If Not ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION NOT REQUIRED !")
                    ddlControlPosition.BackColor = Drawing.Color.Red
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                    txtChainLength.CssClass = "form-controll  is-invalid"
                    txtChainLength.Focus()
                    Exit Sub
                End If
            End If
            '#blind 1 null & blind 2 not null
            If cekControlB1 = "" And Not cekControlB2 = "" Then
                If Not ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION NOT REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                    txtChainLength.CssClass = "form-control  is-invalid"
                    txtChainLength.Focus()
                    Exit Sub
                End If
            End If
            '#blind 1 null & blind 2 null
            If cekControlB1 = "" And cekControlB2 = "" Then
                If ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
        End If
    End Sub

    '#chain controll | Linked 3 Blinds (Ind)
    Private Sub chainLinked3BlindsInd()
        '#...................Blind 1...........................#
        If lblBlindNo.Text = "Blind 1" Then
            If Session("ItemAction") = "AddItem" Or Session("ItemAction") = "EditItem" Then
                If ddlControlPosition.SelectedValue = ""  Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
                        txtChainLength.CssClass = "form-control  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If

                    If txtChainLength.Text < 1 Then
                        Call MessageError(True, "PLEASE CHECK YOUR CHAIN LENGTH ORDER !")
                        txtChainLength.CssClass = "form-select  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If
                End If
            End If
        End If
        '#...................Blind 2...........................#
        If lblBlindNo.Text = "Blind 2" Then
            If Session("ItemAction") = "NextItem" Or Session("ItemAction") = "EditItem" Then
                If ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
                        txtChainLength.CssClass = "form-select  is-invalid"
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
        End If
        '#...................Blind 3...........................#
        If lblBlindNo.Text = "Blind 3" Then
            If Session("ItemAction") = "NextItem" Or Session("ItemAction") = "EditItem" Then
                '#cek control position & chainId B2
                Dim cekControlB2 As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
                Dim cekChainIdB2 As String = publicCfg.GetItemData("SELECT ChainId FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")

                '#control B2 not null & chainId B2 not null
                If Not cekControlB2 = "" AND Not cekChainIdB2 = "" Then
                    If ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                        ddlControlPosition.BackColor = Drawing.Color.Red
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If Not ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                        ddlChainColour.CssClass = "form-select  is-invalid"
                        ddlChainColour.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                        txtChainLength.CssClass = "form-control  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        If Not IsNumeric(txtChainLength.Text) Then
                            Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
                '#control B2 not null & chainId B2 null
                If Not cekControlB2 = "" AND cekChainIdB2 = "" Then
                    If ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                        ddlChainColour.CssClass = "form-select  is-invalid"
                        ddlChainColour.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        If Not IsNumeric(txtChainLength.Text) Then
                            Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
            End If
        End If
    End Sub

    '#chain control | DB Linked 2 Blinds (Dep)
    Private Sub chainDBLinked2BlindsDep()
        '#blind 1
        If lblBlindNo.Text = "Blind 1" Then
            '#create action
            If Session("itemAction") = "AddItem" Then
                If ddlControlPosition.SelectedValue = "" And Not ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not ddlControlPosition.SelectedValue = "" And ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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

            '#update action
            If Session("itemAction") = "EditItem" Then
                Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
                If Not controlPosition = "" Then
                    If Not ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION NOT REQUIRED !")
                        ddlControlPosition.CssClass = "form-control  is invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If Not ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                        ddlChainColour.CssClass = "form-control  is invalid"
                        ddlChainColour.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                        txtChainLength.CssClass = "form-control  is invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If
                End If

                If controlPosition = "" Then
                    If ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                        ddlChainColour.CssClass = "form-select  is-invalid"
                        ddlChainColour.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        If Not IsNumeric(txtChainLength.Text) Then
                            Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
            End If
        End If

        '#blind 2
        If lblBlindNo.Text = "Blind 2" Then
            Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
            If Not controlPosition = "" Then
                If Not ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION NOT REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                    ddlChainColour.CssClass = "form-control  is-invalid"
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                    txtChainLength.CssClass = "form-control  is-invalid"
                    txtChainLength.Focus()
                    Exit Sub
                End If
            End If

            If controlPosition = "" Then
                If ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
        End If

        '#blind 3
        If lblBlindNo.Text = "Blind 3" Then
            '#create action
            If Session("itemAction") = "AddItem" Then
                If ddlControlPosition.SelectedValue = "" And Not ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not ddlControlPosition.SelectedValue = "" And ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If
            End If

            '#update action
            If Session("itemAction") = "EditItem" Then
                Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
                If Not controlPosition = "" Then
                    If Not ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION NOT REQUIRED !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If Not ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                        ddlChainColour.CssClass = "form-select  is-invalid"
                        ddlChainColour.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                        txtChainLength.CssClass = "form-control  is-invalid"
                        txtChainLength.Focus()
                        Exit Sub
                    End If
                End If

                If controlPosition = "" Then
                    If ddlControlPosition.SelectedValue = "" Then
                        Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If

                    If ddlChainColour.SelectedValue = "" Then
                        Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                        ddlChainColour.CssClass = "form-select  is-invalid"
                        ddlChainColour.Focus()
                        Exit Sub
                    End If

                    If Not txtChainLength.Text = "" Then
                        If Not IsNumeric(txtChainLength.Text) Then
                            Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
            End If
        End If

        '#blind 4
        If lblBlindNo.Text = "Blind 4" Then
            Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
            If Not controlPosition = "" Then
                If Not ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION NOT REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR NOT REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    Call MessageError(True, "CHAIN LENGTH NOT REQUIRED !")
                    txtChainLength.CssClass = "form-control  is-invalid"
                    txtChainLength.Focus()
                    Exit Sub
                End If
            End If

            If controlPosition = "" Then
                If ddlControlPosition.SelectedValue = "" Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlChainColour.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
        End If
    End Sub

    '#chain control | DB Linked 2 Blinds (Ind)
    Private Sub chainDBLinked2BlindsInd()
        '#blinds 1
        If lblBlindNo.Text = "Blind 1" Then
            '#create action
             If Session("itemAction") = "AddItem"  Then
                If ddlControlPosition.SelectedValue = ""  Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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

             '#update action
             If Session("itemAction") = "EditItem" Then
                Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 2' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
                Dim controlPositionVar As String = ddlControlPosition.SelectedValue
                If ddlControlPosition.SelectedValue = ""  Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
                If Not controlPosition = "" Then
                    If controlPositionVar = controlPosition Then
                        Call MessageError(True, "PLEASE CHECK YOUR CONTROL POSITION ORDER !")
                        ddlControlPosition.CssClass = "form-select  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If
                End If
             End If
        End If

        '#blinds 2
        If lblBlindNo.Text = "Blind 2" Then
            '#create action
            If Session("itemAction") = "EditItem"  Or Session("itemAction") = "NextItem" Then
                 Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 1' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
                 Dim controlPositionVar As String = ddlControlPosition.SelectedValue
                If ddlControlPosition.SelectedValue = ""  Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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

                If controlPositionVar = controlPosition Then
                    Call MessageError(True, "PLEASE CHECK YOUR CONTROL POSITION ORDER !")
                    ddlControlPosition.CssClass = "form-control  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

            End If
        End If


        '#blinds 3
        If lblBlindNo.Text = "Blind 3" Then
            If Session("itemAction") = "EditItem" Or Session("itemAction") = "NextItem" Then
                Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 4' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
                Dim controlPositionVar As String = ddlControlPosition.SelectedValue
                If ddlControlPosition.SelectedValue = ""  Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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
                If Not controlPosition = "" Then
                    If controlPositionVar = controlPosition Then
                        Call MessageError(True, "PLEASE CHECK YOUR CONTROL POSITION ORDER !")
                        ddlControlPosition.CssClass = "form-control  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If
                End If
            End If
        End If

        '#blinds 4
        If lblBlindNo.Text = "Blind 4" Then
            If Session("itemAction") = "EditItem"  Or Session("itemAction") = "NextItem" Then
                Dim controlPosition As String = publicCfg.GetItemData("SELECT ControlPosition FROM OrderDetails WHERE BlindNo = 'Blind 3' AND UniqueId='" + lblUniqueId.Text + "' AND Active = 1")
                 Dim controlPositionVar As String = ddlControlPosition.SelectedValue
                If ddlControlPosition.SelectedValue = ""  Then
                    Call MessageError(True, "CONTROL POSITION IS REQUIRED !")
                    ddlControlPosition.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If ddlChainColour.SelectedValue = "" Then
                    Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                    ddlChainColour.CssClass = "form-select  is-invalid"
                    ddlControlPosition.Focus()
                    Exit Sub
                End If

                If Not txtChainLength.Text = "" Then
                    If Not IsNumeric(txtChainLength.Text) Then
                        Call MessageError(True, "CUSTOM CHAIN LENGTH SHOULD BE NUMERIC !")
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

                If ControlPosition <> "" Then
                    If controlPositionVar = controlPosition Then
                        Call MessageError(True, "PLEASE CHECK YOUR CONTROL POSITION ORDER !")
                        ddlControlPosition.CssClass = "form-control  is-invalid"
                        ddlControlPosition.Focus()
                        Exit Sub
                    End If
                End If

            End If
        End If
    End Sub

End Class
