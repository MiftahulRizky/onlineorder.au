Imports System.Data

Partial Class Order_Verishades
    Inherits Page

    Dim publicCfg As New PublicConfig

    Public designId As String = String.Empty

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

                Call BindBlind()
                Call BindKit(ddlBlindType.SelectedValue)
                Call BindFabricType()
                Call BindFabricColour(ddlFabricType.SelectedValue)
                Call BindWandSize(String.Empty)

                Call BindComponentForm(ddlKitId.SelectedValue)
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
    End Sub

    Protected Sub ddlBlindType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindKit(ddlBlindType.SelectedValue)
        Call BindWandSize(String.Empty)

        Call BindComponentForm(ddlKitId.SelectedValue)
    End Sub

    Protected Sub ddlFabricType_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindFabricColour(ddlFabricType.SelectedValue)
    End Sub

    Protected Sub ddlWandSize_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindWandColour(ddlWandSize.SelectedValue)
        If ddlWandSize.SelectedValue = "custom" Then
           divWandCustomLength.Visible = True
           divBtnInfoCustom.Visible = True
        Else
           divWandCustomLength.Visible = False
           divBtnInfoCustom.Visible = False
        End If
    End Sub
    
    Protected Sub ddlWandColour_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        If ddlWandSize.SelectedValue = "custom" Then
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
                Call MessageError(True, "VERI SHADES TYPE IS REQUIRED !")
                ddlBlindType.CssClass = "form-select  is-invalid"
                ddlBlindType.Focus()
                Exit Sub
            End If

            If ddlKitId.SelectedValue = "" Then
                Call MessageError(True, "PLEASE CONTACT OUR IT TEAM AT SUPPORT@ONLINEORDER.AU")
                ddlKitId.CssClass = "form-select  is-invalid"
                ddlKitId.Focus()
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

            Dim blindName As String = publicCfg.GetBlindName(ddlBlindType.SelectedValue)

            If ddlMounting.SelectedValue = "" And (blindName = "Single" Or blindName = "Track Only") Then
                Call MessageError(True, "MOUNTING IS REQUIRED !")
                ddlMounting.CssClass = "form-select  is-invalid"
                ddlMounting.Focus()
                Exit Sub
            End If

            If blindName = "Single" Or blindName = "Slat Only" Then
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
            End If

            If blindName = "Single" Or blindName = "Track Only" Then
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
                        Call MessageError(True, "MINIMUM WIDTH IS 150mm !")
                        txtWidth.CssClass = "form-control  is-invalid"
                        txtWidth.Focus()
                        Exit Sub
                    End If

                    If CInt(txtWidth.Text) > 6000 Then
                        Call MessageError(True, "MAXIMUM WIDTH IS 3000mm !")
                        txtWidth.CssClass = "form-control  is-invalid"
                        txtWidth.Focus()
                        Exit Sub
                    End If
                End If
            End If

            If blindName = "Single" Or blindName = "Slat Only" Then
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

                    If CInt(txtDrop.Text) < 150 Then
                        Call MessageError(True, "MINIMUM DROP IS 150mm !")
                        txtDrop.CssClass = "form-control  is-invalid"
                        txtDrop.Focus()
                        Exit Sub
                    End If

                    If CInt(txtDrop.Text) > 3200 Then
                        Call MessageError(True, "MAXIMUM DROP IS 3200mm !")
                        txtDrop.CssClass = "form-control  is-invalid"
                        txtDrop.Focus()
                        Exit Sub
                    End If
                End If
            End If

            If blindName = "Single" Or blindName = "Track Only" Then
                If ddlStackPosition.SelectedValue = "" Then
                    Call MessageError(True, "STACK CONFIGURATION IS REQUIRED !")
                    ddlStackPosition.CssClass = "form-select  is-invalid"
                    ddlStackPosition.Focus()
                    Exit Sub
                End If

                If ddlTrackType.SelectedValue = "" Then
                    Call MessageError(True, "TRACK TYPE IS REQUIRED !")
                    ddlTrackType.CssClass = "form-select  is-invalid"
                    ddlTrackType.Focus()
                    Exit Sub
                End If

                If ddlTrackColour.SelectedValue = "" Then
                    Call MessageError(True, "TRACK COLOUR IS REQUIRED !")
                    ddlTrackColour.CssClass = "form-select  is-invalid"
                    ddlTrackColour.Focus()
                    Exit Sub
                End If

                If ddlWandColour.SelectedValue = "" Then
                    Call MessageError(True, "WAND COLOUR IS REQUIRED !")
                    ddlWandColour.CssClass = "form-select  is-invalid"
                    ddlWandColour.Focus()
                    Exit Sub
                End If

                If ddlWandSize.SelectedValue = "" Then
                    Call MessageError(True, "WAND SIZE IS REQUIRED !")
                    ddlWandSize.CssClass = "form-select  is-invalid"
                    ddlWandSize.Focus()
                    Exit Sub
                End If

                If ddlWandSize.SelectedValue = "custom" AndAlso txtWandCustomLength.Text = "" Then
                    Call MessageError(True, "CUSTOM WAND SIZE IS REQUIRED !")
                    txtWandCustomLength.CssClass = "form-control  is-invalid"
                    txtWandCustomLength.Focus()
                    Exit Sub
                End If
                If ddlWandSize.SelectedValue = "custom" AndAlso txtWandCustomLength.Text > 3000 Then
                    Call MessageError(True, "MAXIMUM WAND SIZE IS 3000mm !")
                    txtWandCustomLength.CssClass = "form-control  is-invalid"
                    txtWandCustomLength.Focus()
                    Exit Sub
                End If
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

                If txtMarkUp.Text < 1 Then
                    Call MessageError(True, "PLEASE CHECK YOUR MARK UP ORDER !")
                    txtMarkUp.CssClass = "form-control  is-invalid"
                    txtMarkUp.Focus()
                    Exit Sub
                End If
            End If

            If msgError.InnerText = "" Then
                lblKitId.Text = UCase(ddlKitId.SelectedValue).ToString()
                lblSoeKitId.Text = publicCfg.GetSoeKitId(ddlKitId.SelectedValue)

                Dim priceGroupName As String = blindName
                Dim fabricGroup As String = publicCfg.GetFabricGroup(ddlFabricColour.SelectedValue)
                If blindName = "Single" Then
                    priceGroupName = "Veri Shades" & " - " & fabricGroup

                    If ddlWandSize.SelectedValue = "custom" Then
                        lblWandSize.Text = txtWandCustomLength.Text
                    Else
                        lblWandSize.Text = ddlWandSize.SelectedValue
                    End If
                End If
                If blindName = "Slat Only" Then
                    priceGroupName = blindName & " - " & fabricGroup
                End If
                Dim priceGroupId As String = publicCfg.GetPriceGroupId(designId, priceGroupName)
                lblPriceGroupId.Text = UCase(priceGroupId).ToString()

                If txtMarkUp.Text = "" Then : txtMarkUp.Text = "0" : End If

                If blindName = "Track Only" Then
                    ddlFabricColour.SelectedValue = ""
                    txtDrop.Text = "0"
                    If ddlWandSize.SelectedValue = "custom" Then
                        lblWandSize.Text = txtWandCustomLength.Text
                    Else
                        lblWandSize.Text = ddlWandSize.SelectedValue
                    End If
                End If

                If blindName = "Slat Only" Then
                    ddlMounting.SelectedValue = ""
                    ddlStackPosition.SelectedValue = ""
                    ddlTrackType.SelectedValue = "" : ddlTrackColour.SelectedValue = ""
                    ddlWandColour.SelectedValue = "" : ddlWandSize.SelectedValue = ""
                    txtWidth.Text = "0"
                End If

                Dim userId As String = UCase(Session("UserId")).ToString()
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

            Dim fabricType As String = myData.Tables(0).Rows(0).Item("fabricType").ToString()

            Call BindBlind()
            Call BindKit(blindId)
            Call BindFabricType()
            Call BindFabricColour(fabricType)
            Call BindWandSize(String.Empty)

            ddlBlindType.SelectedValue = blindId : ddlBlindType.Enabled = False
            ddlKitId.SelectedValue = kitId

            txtQty.Text = myData.Tables(0).Rows(0).Item("Qty").ToString()
            ddlMounting.SelectedValue = myData.Tables(0).Rows(0).Item("Mounting").ToString()
            txtLocation.Text = myData.Tables(0).Rows(0).Item("Location").ToString()
            ddlFabricType.SelectedValue = fabricType
            ddlFabricColour.SelectedValue = myData.Tables(0).Rows(0).Item("FabricId").ToString()
            txtWidth.Text = myData.Tables(0).Rows(0).Item("Width").ToString()
            txtDrop.Text = myData.Tables(0).Rows(0).Item("Drop").ToString()
            ddlBlindSize.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("BlindSize"))
            ddlStackPosition.SelectedValue = myData.Tables(0).Rows(0).Item("StackPosition").ToString()

            ddlTrackType.SelectedValue = myData.Tables(0).Rows(0).Item("TrackType").ToString()
            ddlTrackColour.SelectedValue = myData.Tables(0).Rows(0).Item("TrackColour").ToString()
            Dim WandSizeKey As String() = {"","500","750","1100", "1500", "2000"}
            Dim WandSizeVal As String = myData.Tables(0).Rows(0).Item("WandLength").ToString()
             If Not WandSizeKey.Contains(WandSizeVal) Then
                ddlWandSize.SelectedValue = "custom"
                txtWandCustomLength.Text = WandSizeVal
            Else
                ddlWandSize.SelectedValue = WandSizeVal
            End If
            Call BindWandColour(ddlWandSize.SelectedValue)
            ddlWandColour.SelectedValue = myData.Tables(0).Rows(0).Item("WandColour").ToString()
            txtNotes.Text = myData.Tables(0).Rows(0).Item("Notes").ToString()
            txtMarkUp.Text = myData.Tables(0).Rows(0).Item("MarkUp").ToString()
            If txtMarkUp.Text = "0" Then : txtMarkUp.Text = String.Empty : End If

            Call BindComponentForm(kitId)
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

            If Not Data = "" Then
                divDetail.Visible = True

                divMounting.Visible = True
                divFabric.Visible = True
                divTrack.Visible = True
                divStack.Visible = True
                divBlindSize.Visible = False

                divWand.Visible = True
                divWandCustomLength.Visible = False
                divBtnInfoCustom.Visible = False
                divWidth.Visible = True : divDrop.Visible = True
                lblSize.Text = "WIDTH x DROP"

                Dim blindName As String = publicCfg.GetBlindName(ddlBlindType.SelectedValue)
                If blindName = "Track Only" Then
                    divDrop.Visible = False
                    lblSize.Text = "WIDTH"
                    divFabric.Visible = False
                End If

                If blindName = "Slat Only" Then
                    divBlindSize.Visible = True
                    divWidth.Visible = False
                    divMounting.Visible = False
                    divTrack.Visible = False
                    divWand.Visible = False
                    divStack.Visible = False
                    lblSize.Text = "DROP"
                End If

                 If ddlWandSize.SelectedValue = "custom" Then 
                    divWandCustomLength.Visible = True
                    divBtnInfoCustom.Visible = True
                End If
                divMarkUp.Visible = False
                If Session("MarkUpAccess") = True Then : divMarkUp.Visible = True : End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindComponentForm", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindBlind()
        ddlBlindType.Items.Clear()
        Try
            ddlBlindType.DataSource = publicCfg.GetListData("SELECT UPPER(Name) AS NameText, * FROM Blinds WHERE DesignId='28AF4887-5E18-4434-A6A0-08319672D7AA' AND Active='1' ORDER BY Name ASC")
            ddlBlindType.DataTextField = "NameText"
            ddlBlindType.DataValueField = "Id"
            ddlBlindType.DataBind()
            If ddlBlindType.Items.Count > 0 Then
                ddlBlindType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindKit(Blind As String)
        ddlKitId.Items.Clear()
        Try
            If Not Blind = "" Then
                Dim thisQuery As String = "SELECT *, UPPER(Name) AS KitName FROM HardwareKits WHERE DesignId='" + designId + "' AND BlindId = '" + Blind + "' AND Active=1 ORDER BY Name ASC"
                ddlKitId.DataSource = publicCfg.GetListData(thisQuery)
                ddlKitId.DataTextField = "KitName"
                ddlKitId.DataValueField = "Id"
                ddlKitId.DataBind()
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindKit", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindFabricType()
        ddlFabricType.Items.Clear()
        Try
            If Not designId = "" Then
                ddlFabricType.DataSource = publicCfg.GetListData("SELECT UPPER(Type) AS TypeText, Type AS TypeValue FROM Fabrics WHERE DesignId='" + designId + "' AND Active='1' GROUP BY Type ORDER BY Type ASC")
                ddlFabricType.DataTextField = "TypeText"
                ddlFabricType.DataValueField = "TypeValue"
                ddlFabricType.DataBind()
                If ddlFabricType.Items.Count > 1 Then
                    ddlFabricType.Items.Insert(0, New ListItem("", ""))
                End If
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
                ddlFabricColour.DataSource = publicCfg.GetListData("SELECT Id, UPPER(Colour) AS Colour FROM Fabrics WHERE DesignId='" + designId + "' AND Active='1' AND Type='" + Type + "' ORDER BY Name ASC")
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
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindFabricColour", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindWandSize(Data As String)
        ddlWandSize.Items.Clear()
        Try
            ddlWandSize.Items.Clear()
            ddlWandSize.Items.Add(New ListItem("Custom", "custom"))
            ddlWandSize.Items.Add(New ListItem("500mm", "500"))
            ddlWandSize.Items.Add(New ListItem("750mm", "750"))
            ddlWandSize.Items.Add(New ListItem("1100mm", "1100"))
            ddlWandSize.Items.Add(New ListItem("1500mm", "1500"))
            ddlWandSize.Items.Add(New ListItem("2000mm", "2000"))

            If ddlWandSize.Items.Count > 1 Then
                ddlWandSize.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                publicCfg.MailError(Session("UserId"), Page.Title, "BindWandSize", ex.ToString())
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
                publicCfg.MailError(Session("UserId"), Page.Title, "BindWandColour", ex.ToString())
            End If
        End Try
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

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlBlindType.CssClass = "form-select "
        txtLocation.CssClass = "form-control "
        ddlKitId.CssClass = "form-select "
        txtQty.CssClass = "form-control "
        ddlBlindSize.CssClass = "form-select "
        ddlMounting.CssClass = "form-select "
        ddlFabricType.CssClass = "form-select "
        ddlFabricColour.CssClass = "form-select "
        ddlTrackType.CssClass = "form-select "
        ddlTrackColour.CssClass = "form-select "
        ddlStackPosition.CssClass = "form-select "
        txtWidth.CssClass = "form-control "
        txtDrop.CssClass = "form-control "
        ddlWandSize.CssClass = "form-select "
        ddlWandColour.CssClass = "form-select "
        txtWandCustomLength.CssClass="form-control "
        txtNotes.CssClass = "form-control "
        txtMarkUp.CssClass = "form-control "
    End Sub
End Class