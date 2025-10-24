Partial Class Setting_Member_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" And Not Session("RoleName") = "Account" Then
            Response.Redirect("~/setting/member", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()

            Call BindApp()
            Call BindRole()
            Call BindLevel()
            Call BindStore()
        End If
    End Sub

    Protected Sub btnFindStore_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If Not txtStore.Text = "" Then
                ddlStore.SelectedValue = txtStore.Text
                txtStore.Text = ""
            End If
        Catch ex As Exception
            txtStore.Text = ""
            ddlStore.SelectedValue = ""
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlAppId.SelectedValue = "" Then
                Call MessageError(True, "APPLICATION ID IS REQUIRED !")
                ddlAppId.CssClass = "form-control  is-invalid"
                ddlAppId.Focus()
                Exit Sub
            End If

            If ddlRole.SelectedValue = "" Then
                Call MessageError(True, "ROLE IS REQUIRED !")
                ddlRole.CssClass = "form-select  is-invalid"
                ddlRole.Focus()
                Exit Sub
            End If

            If ddlLevel.SelectedValue = "" Then
                Call MessageError(True, "LEVEL IS REQUIRED !")
                ddlLevel.CssClass = "form-select  is-invalid"
                ddlLevel.Focus()
                Exit Sub
            End If

            If txtUserName.Text = "" Then
                Call MessageError(True, "USERNAME IS REQUIRED !")
                txtUserName.CssClass = "form-control  is-invalid"
                txtUserName.Focus()
                Exit Sub
            End If

            If txtUserName.Text = publicCfg.GetItemData("SELECT UserName FROM Memberships WHERE UserName = '" + txtUserName.Text + "'") Then
                Call MessageError(True, "USERNAME ALREADY EXISTS !")
                txtUserName.CssClass = "form-control  is-invalid"
                txtUserName.Focus()
                Exit Sub
            End If

            If ddlStore.SelectedValue = "" Then
                Call MessageError(True, "STORE IS REQUIRED !")
                ddlStore.CssClass = "form-select  is-invalid"
                ddlStore.Focus()
                Exit Sub
            End If

            If txtEmail.Text = publicCfg.GetItemData("SELECT Email FROM Users WHERE Email = '" + txtEmail.Text + "'") Then
                Call MessageError(True, "THIS EMAIL ADDRESS ALREADY USED !")
                txtEmail.CssClass = "form-control  is-invalid"
                txtEmail.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If txtPassword.Text = "" Then
                    txtPassword.Text = "123456"
                End If
                If txtFullName.Text = "" Then
                    txtFullName.Text = txtUserName.Text
                End If
                lblPasswordHash.Text = publicCfg.Encrypt(txtPassword.Text)
                sdsPageMember.Insert()

                lblUserId.Text = publicCfg.GetItemData("SELECT UserId FROM Memberships WHERE UserName='" + txtUserName.Text + "'")

                sdsPageUsers.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT MEMBERSHIPS. USERNAME : " & txtUserName.Text)

                Response.Redirect("~/setting/member", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/member/", False)
    End Sub

    Private Sub BindApp()
        ddlAppId.Items.Clear()
        Try
            ddlAppId.DataSource = publicCfg.GetListData("SELECT Id, UPPER(Name) AS Name FROM Applications ORDER BY Name ASC")
            ddlAppId.DataTextField = "Name"
            ddlAppId.DataValueField = "Id"
            ddlAppId.DataBind()

            If ddlAppId.Items.Count > 1 Then
                ddlAppId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindRole()
        ddlRole.Items.Clear()
        Try
            Dim Query As String = "SELECT Id, UPPER(Name) AS NameText FROM Roles ORDER BY Name ASC"
            If Not Session("RoleName") = "Administrator" Then
                Query = "SELECT Id, UPPER(Name) AS NameText FROM Roles WHERE Name <> 'Administrator' ORDER BY Name ASC"
            End If
            ddlRole.DataSource = publicCfg.GetListData(Query)
            ddlRole.DataTextField = "NameText"
            ddlRole.DataValueField = "Id"
            ddlRole.DataBind()

            If ddlRole.Items.Count > 1 Then
                ddlRole.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindLevel()
        ddlLevel.Items.Clear()
        Try
            Dim Query As String = "SELECT Id, UPPER(Name) AS NameText FROM MembersLevel ORDER BY Name ASC"
            If Not Session("RoleName") = "Administrator" Then
                Query = "SELECT Id, UPPER(Name) AS NameText FROM MembersLevel WHERE Name <> 'Super Admin' ORDER BY Name ASC"
            End If
            ddlLevel.DataSource = publicCfg.GetListData(Query)
            ddlLevel.DataTextField = "NameText"
            ddlLevel.DataValueField = "Id"
            ddlLevel.DataBind()

            If ddlLevel.Items.Count > 1 Then
                ddlLevel.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindStore()
        ddlStore.Items.Clear()
        Try
            Dim Query As String = "SELECT *, UPPER(Name) AS NameText FROM Stores WHERE Active=1 ORDER BY Name ASC"
            If Not Session("RoleName") = "Administrator" Then
                Query = "SELECT *, UPPER(Name) AS NameText FROM Stores WHERE Active=1 AND Name <> 'Administrator' ORDER BY Name ASC"
            End If
            ddlStore.DataSource = publicCfg.GetListData(Query)
            ddlStore.DataTextField = "NameText"
            ddlStore.DataValueField = "Id"
            ddlStore.DataBind()

            If ddlStore.Items.Count > 1 Then
                ddlStore.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlAppId.CssClass = "form-control "
        txtUserName.CssClass = "form-control "
        txtPassword.CssClass = "form-control "
        ddlRole.CssClass = "form-select "
        ddlLevel.CssClass = "form-select "
        ddlStore.CssClass = "form-select "
        txtFullName.CssClass = "form-control "
        txtEmail.CssClass = "form-control "
        txtPassword.CssClass = "form-control "
        txtPhone.CssClass = "form-control "
        ddlMarkUp.CssClass = "form-select "
        ddlPrice.CssClass = "form-select "
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
