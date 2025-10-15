Imports System.Data

Partial Class Order_Header
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("headerAction") = "" Then
            Response.Redirect("~/order", False)
            Exit Sub
        End If

        If Session("headerAction") = "AddHeader" Then
            cardTitle.InnerText = "Create New Order"
            Page.Title = "Create Header"

            divFindStore.Visible = False
            If Session("RoleName") = "Administrator" Then : divFindStore.Visible = True : End If
            If Not IsPostBack Then
                lblUserId.Text = UCase(Session("UserId")).ToString()
                Call BackColor()
                Call BindDataStore()
            End If
        End If

        If Session("headerAction") = "EditHeader" Then
            cardTitle.InnerText = "Edit Data Order"
            Page.Title = "Edit Header"
            lblHeaderId.Text = Session("HeaderId")

            divFindStore.Visible = True
            If Session("RoleName") = "Customer" Then : divFindStore.Visible = False : End If
            If Not IsPostBack Then
                Call BindDataHeader(lblHeaderId.Text)
            End If
        End If
    End Sub

    Protected Sub btnFindStore_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If Not txtStore.Text = "" Then
                ddlStore.SelectedValue = txtStore.Text
                txtStore.Text = ""
            End If
        Catch ex As Exception
            txtStore.Text = "" : ddlStore.SelectedValue = ""
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlStore.SelectedValue = "" Then
                Call MessageError(True, "STORE ACCOUNT IS REQUIRED !")
                ddlStore.CssClass = "form-select  is-invalid"
                ddlStore.Focus()
                Exit Sub
            End If

            If txtOrderNo.Text = "" Then
                Call MessageError(True, "ORDER NUMBER IS REQUIRED !")
                txtOrderNo.CssClass = "form-control  is-invalid"
                txtOrderNo.Focus()
                Exit Sub
            End If

            If InStr(txtOrderNo.Text, "\") > 0 Or InStr(txtOrderNo.Text, "/") > 0 Or InStr(txtOrderNo.Text, "|") > 0 Or InStr(txtOrderNo.Text, ",") > 0 Or InStr(txtOrderNo.Text, "&") > 0 Or InStr(txtOrderNo.Text, ",") > 0 Or InStr(txtOrderNo.Text, "#") > 0 Or InStr(txtOrderNo.Text, "'") > 0 Or InStr(txtOrderNo.Text, ".") > 0 Or InStr(txtOrderNo.Text, "`") > 0 Then
                Call MessageError(True, "PLEASE DON'T USE [ / ], [ | ], [ \ ], [ & ], [ # ], [ ' ], [ ` ], [ . ] AND [ , ]")
                 txtOrderNo.CssClass = "form-control  is-invalid"
                txtOrderNo.Focus()
                Exit Sub
            End If

            If Trim(txtOrderNo.Text).Length > 50 Then
                Call MessageError(True, "MAXIMUM 10 CHARACTERS FOR ORDER NUMBER !")
                 txtOrderNo.CssClass = "form-control  is-invalid"
                txtOrderNo.Focus()
                Exit Sub
            End If

            ' If Session("headerAction") = "AddHeader" Then
            '     Dim OrderNo As String = txtOrderNo.Text.Trim()

            '     '#check by store
            '     If OrderNo = publicCfg.GetItemData("SELECT OrderNo FROM OrderHeaders WHERE OrderNo = '" + OrderNo + "' AND StoreId = '" + ddlStore.SelectedValue + "' AND Active=1") Then
            '         Call MessageError(True, "YOU HAVE USED THIS ORDER NUMBER !")
            '          txtOrderNo.CssClass = "form-control  is-invalid"
            '         txtOrderNo.Focus()
            '         Exit Sub
            '     End If

            '     '#check all
            '     If OrderNo = publicCfg.GetItemData("SELECT OrderNo FROM OrderHeaders WHERE OrderNo = '" + OrderNo + "' AND Active=1") Then
            '         Call MessageError(True, "SORRY. YOU CAN NOT USE THIS ORDER NUMBER !")
            '          txtOrderNo.CssClass = "form-control  is-invalid"
            '         txtOrderNo.Focus()
            '         Exit Sub
            '     End If
            ' End If

            If Session("headerAction") = "AddHeader" Then
                Dim orderNo As String = txtOrderNo.Text.Trim()
                Dim storeId As String = ddlStore.SelectedValue

                '--- Check by store
                Dim sqlByStore As String = "SELECT LTRIM(RTRIM(OrderNo)) FROM OrderHeaders WHERE LTRIM(RTRIM(OrderNo)) = '" & orderNo & "' AND StoreId = '" & storeId & "' AND Active = 1"
                Dim existingByStore As String = publicCfg.GetItemData(sqlByStore)

                If orderNo = existingByStore Then
                    Call MessageError(True, "YOU HAVE USED THIS ORDER NUMBER !")
                    txtOrderNo.CssClass = "form-control is-invalid"
                    txtOrderNo.Focus()
                    Exit Sub
                End If

                '--- Check all store
                Dim sqlAll As String = "SELECT LTRIM(RTRIM(OrderNo)) FROM OrderHeaders WHERE LTRIM(RTRIM(OrderNo)) = '" & orderNo & "' AND Active = 1"
                Dim existingAll As String = publicCfg.GetItemData(sqlAll)

                If orderNo = existingAll Then
                    Call MessageError(True, "SORRY. YOU CAN NOT USE THIS ORDER NUMBER !")
                    txtOrderNo.CssClass = "form-control is-invalid"
                    txtOrderNo.Focus()
                    Exit Sub
                End If
            End If


            ' If Session("headerAction") = "EditHeader" And Not txtOrderNo.Text = lblOrderNo.Text Then
            '     Dim OrderNo As String = txtOrderNo.Text.Trim()

            '     If OrderNo = publicCfg.GetItemData("SELECT OrderNo FROM OrderHeaders WHERE OrderNo = '" + OrderNo + "' AND StoreId = '" + ddlStore.SelectedValue + "' AND Active=1") Then
            '         Call MessageError(True, "YOU HAVE USED THIS ORDER NUMBER !")
            '          txtOrderNo.CssClass = "form-control  is-invalid"
            '         txtOrderNo.Focus()
            '         Exit Sub
            '     End If

            '     If OrderNo = publicCfg.GetItemData("SELECT OrderNo FROM OrderHeaders WHERE OrderNo = '" + OrderNo + "' AND Active=1") Then
            '         Call MessageError(True, "SORRY. YOU CAN NOT USE THIS ORDER NUMBER !")
            '          txtOrderNo.CssClass = "form-control  is-invalid"
            '         txtOrderNo.Focus()
            '         Exit Sub
            '     End If
            ' End If

            If Session("headerAction") = "EditHeader" AndAlso txtOrderNo.Text.Trim() <> lblOrderNo.Text.Trim() Then
                Dim orderNo As String = txtOrderNo.Text.Trim()
                Dim storeId As String = ddlStore.SelectedValue
                Dim headerId As String = lblHeaderId.Text

                '#check by store
                Dim existingByStore As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNo)) FROM OrderHeaders WHERE LTRIM(RTRIM(OrderNo)) = '" & orderNo & "' AND StoreId = '" & storeId & "' AND Active = 1 AND Id != '" & headerId & "'")

                If orderNo = existingByStore Then
                    Call MessageError(True, "YOU HAVE USED THIS ORDER NUMBER !")
                    txtOrderNo.CssClass = "form-control is-invalid"
                    txtOrderNo.Focus()
                    Exit Sub
                End If

                '#check all
                Dim existingByAll As String = publicCfg.GetItemData("SELECT LTRIM(RTRIM(OrderNo)) FROM OrderHeaders WHERE LTRIM(RTRIM(OrderNo)) = '" & orderNo & "' AND Active = 1 AND Id != '" & headerId & "'")

                If orderNo = existingByAll Then
                    Call MessageError(True, "SORRY. YOU CAN NOT USE THIS ORDER NUMBER !")
                    txtOrderNo.CssClass = "form-control is-invalid"
                    txtOrderNo.Focus()
                    Exit Sub
                End If
            End If


            If txtReference.Text = "" Then
                Call MessageError(True, "REFERENCE IS REQUIRED !")
                txtReference.CssClass = "form-control  is-invalid"
                txtReference.Focus()
                Exit Sub
            End If

            If InStr(txtReference.Text, "\") > 0 Or InStr(txtReference.Text, "/") > 0 Or InStr(txtOrderNo.Text, "|") > 0 Or InStr(txtReference.Text, ",") > 0 Or InStr(txtReference.Text, "&") > 0 Or InStr(txtReference.Text, ",") > 0 Or InStr(txtReference.Text, "#") > 0 Or InStr(txtReference.Text, "'") > 0 Or InStr(txtReference.Text, ".") > 0 Or InStr(txtReference.Text, "`") > 0 Then
                Call MessageError(True, "PLEASE DON'T USE [ / ], [ | ], [ \ ], [ & ], [ # ], [ ' ], [ ` ], [ . ] AND [ , ]")
                txtReference.CssClass = "form-control  is-invalid"
                txtReference.Focus()
                Exit Sub
            End If

            If ddlDelivery.SelectedValue = "" Then
                Call MessageError(True, "DELIVERY / PICK UP IS REQUIRED !")
                ddlDelivery.CssClass = "form-select  is-invalid"
                ddlDelivery.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If Session("headerAction") = "AddHeader" Then
                    lblHeaderId.Text = publicCfg.CreateOrderHeaderId()

                    sdsPage.Insert()
                    publicCfg.InsertActivity(lblUserId.Text, Page.Title, "ADD ORDER. ID : " & lblHeaderId.Text & " | " & txtOrderNo.Text & " - " & txtReference.Text)

                    Session("headerId") = lblHeaderId.Text
                    Response.Redirect("~/order/detail", False)
                    Exit Sub
                End If

                If Session("headerAction") = "EditHeader" Then
                    sdsPage.Update()
                    publicCfg.InsertActivity(lblUserId.Text, Page.Title, "EDIT DATA ORDER HEADER " & lblHeaderId.Text)
                    Call myCancel()
                    Exit Sub
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

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Call myCancel()
    End Sub

    Private Sub BindDataHeader(HeaderId As String)
        Call BackColor()
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM OrderHeaders WHERE Id = '" + HeaderId + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/order", False)
                Exit Sub
            End If

            Call BindDataStore()

            lblUserId.Text = UCase(myData.Tables(0).Rows(0).Item("UserId").ToString())
            ddlStore.SelectedValue = myData.Tables(0).Rows(0).Item("StoreId").ToString()
            txtOrderNo.Text = myData.Tables(0).Rows(0).Item("OrderNo").ToString()
            lblOrderNo.Text = myData.Tables(0).Rows(0).Item("OrderNo").ToString()
            txtReference.Text = myData.Tables(0).Rows(0).Item("OrderCust").ToString()
            ddlDelivery.SelectedValue = myData.Tables(0).Rows(0).Item("Delivery").ToString()
            txtNote.Text = myData.Tables(0).Rows(0).Item("Note").ToString()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataHeader", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindDataStore()
        ddlStore.Items.Clear()
        Try
            ddlStore.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Stores WHERE Company = 'SP' AND Active=1 ORDER BY Name ASC")
            ddlStore.DataTextField = "NameText"
            ddlStore.DataValueField = "Id"
            ddlStore.DataBind()

            If ddlStore.Items.Count > 1 Then
                ddlStore.Items.Insert(0, New ListItem("", ""))
            End If

            ddlStore.Enabled = False
            ddlStore.SelectedValue = Session("StoreId")
            If Session("RoleName") = "Administrator" Then
                ddlStore.Enabled = True
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(Session("UserId"), Page.Title, "BindDataStore", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlStore.CssClass = "form-select "
        txtOrderNo.CssClass = "form-control "
        txtReference.CssClass = "form-control "
        ddlDelivery.CssClass = "form-select "
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
        If Session("headerAction") = "AddHeader" Then
            Response.Redirect("~/order/", False)
            Exit Sub
        End If
        If Session("headerAction") = "EditHeader" Then
            Session("HeaderId") = lblHeaderId.Text
            Response.Redirect("~/order/detail", False)
            Exit Sub
        End If
    End Sub
End Class
