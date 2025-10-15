Partial Class Setting_Email_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()
            Call BindApp()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtName.Text = "" Then
                Call MessageError(True, "APP NAME IS REQUIRED !")
                txtName.CssClass = "form-control  is-invalid"
                txtName.Focus()
                Exit Sub
            End If

            If txtServer.Text = "" Then
                Call MessageError(True, "MAIL SERVER IS REQUIRED !")
                txtServer.CssClass = "form-control  is-invalid"
                txtServer.Focus()
                Exit Sub
            End If

            If txtHost.Text = "" Then
                Call MessageError(True, "MAIL HOST IS REQUIRED !")
                txtHost.CssClass = "form-control  is-invalid"
                txtHost.Focus()
                Exit Sub
            End If

            If txtPort.Text = "" Then
                Call MessageError(True, "MAIL PORT IS REQUIRED !")
                txtPort.CssClass = "form-control  is-invalid"
                txtPort.Focus()
                Exit Sub
            End If

            If txtAccount.Text = "" Then
                Call MessageError(True, "MAIL ACCOUNT IS REQUIRED !")
                txtAccount.CssClass = "form-control  is-invalid"
                txtAccount.Focus()
                Exit Sub
            End If

            If txtPassword.Text = "" Then
                Call MessageError(True, "MAIL PASSWORD IS REQUIRED !")
                txtPassword.CssClass = "form-control  is-invalid"
                txtPassword.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If txtAlias.Text = "" Then
                    txtAlias.Text = txtAccount.Text
                End If
                If txtSubject.Text = "" Then
                    txtSubject.Text = txtName.Text
                End If
                lblAppId.Text = UCase(ddlAppId.SelectedValue).ToString()
                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT MAIL CONFIGURATION. NAME : " & txtName.Text)

                Response.Redirect("~/setting/email", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/email/", False)
    End Sub

    Private Sub BindApp()
        ddlAppId.Items.Clear()
        Try
            ddlAppId.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Applications ORDER BY Name ASC")
            ddlAppId.DataTextField = "NameText"
            ddlAppId.DataValueField = "Id"
            ddlAppId.DataBind()

            If ddlAppId.Items.Count > 1 Then
                ddlAppId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlAppId.CssClass = "form-select "
        txtName.CssClass = "form-control "
        txtServer.CssClass = "form-control "
        txtHost.CssClass = "form-control "
        txtPort.CssClass = "form-control "
        txtAccount.CssClass = "form-control "
        txtPassword.CssClass = "form-control "
        txtAlias.CssClass = "form-control "
        txtSubject.CssClass = "form-control "
        txtTo.CssClass = "form-control"
        txtCc.CssClass = "form-control"
        ddlSSL.CssClass = "form-select "
        ddlCredentials.CssClass = "form-select "
        txtDescription.CssClass = "form-control"
        ddlActive.CssClass = "form-select "
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
