Imports System.Data

Partial Class Setting_Member_Detail
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("memberDetail") = "" Then
            Response.Redirect("~/setting/member", False)
            Exit Sub
        End If

        lblUserId.Text = UCase(Session("memberDetail")).ToString()
        If Not IsPostBack Then
            Call BackColor()
            Call BindData(lblUserId.Text)
        End If
    End Sub

    Protected Sub btnFindStore_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtStore.Text <> "" Then
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
                ddlAppId.CssClass = "form-select  is-invalid"
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

            If Session("RoleName") = "CUSTOMER" And ddlLevel.SelectedItem.Text = "LEADER" Then
                Call MessageError(True, "YOU CAN'T REGISTER A NEW ACCOUNT AS A LEADER. PLEASE CHANGE TO MEMBER !")
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

            If ddlStore.SelectedValue = "" Then
                Call MessageError(True, "STORE IS REQUIRED !")
                ddlStore.CssClass = "form-select  is-invalid"
                ddlStore.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                If txtFullName.Text = "" Then
                    txtFullName.Text = txtUserName.Text
                End If

                sdsPageMember.Update()
                sdsPageUsers.Update()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE MEMBERSHIPS. USER ID : " & lblUserId.Text)

                Response.Redirect("~/setting/member", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/member/", False)
    End Sub

    Protected Sub btnSubmitPassword_Click(sender As Object, e As EventArgs)
        Try
            If txtPassword.Text = "" Then
                Call MessageErrorPassword(True, "NEW PASSWORD IS REQUIRED !")
                txtPassword.CssClass = "form-control  is-invalid"
                txtPassword.Focus()
                Exit Sub
            End If

            If txtCPassword.Text <> txtPassword.Text Then
                Call MessageErrorPassword(True, "PASSWORDS ARE NOT THE SAME !")
                txtPassword.CssClass = "form-control  is-invalid"
                txtPassword.Focus()
                Exit Sub
            End If

            If msgErrorPassword.InnerText = "" Then
                lblPasswordHash.Text = publicCfg.Encrypt(txtPassword.Text)
                sdsPassword.Update()
            End If

            Call BindData(lblUserId.Text)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindData(UserId As String)
        Try
            Dim myDataMember As DataSet = publicCfg.GetListData("SELECT * FROM Memberships WHERE UserId = '" + UCase(UserId).ToString() + "'")

            If myDataMember.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/member", False)
                Exit Sub
            End If

            Call BindApp()
            Call BindRole()
            Call BindLevel()
            Call BindStore()

            ddlAppId.SelectedValue = myDataMember.Tables(0).Rows(0).Item("ApplicationId").ToString()
            ddlRole.SelectedValue = myDataMember.Tables(0).Rows(0).Item("RoleId").ToString()
            ddlLevel.SelectedValue = myDataMember.Tables(0).Rows(0).Item("LevelId").ToString()
            txtUserName.Text = myDataMember.Tables(0).Rows(0).Item("UserName").ToString()

            ' USERS

            Dim myDataUser As DataSet = publicCfg.GetListData("SELECT * FROM Users WHERE UserId = '" + UCase(UserId).ToString() + "'")
            If myDataUser.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/member", False)
                Exit Sub
            End If

            ddlStore.SelectedValue = myDataUser.Tables(0).Rows(0).Item("StoreId").ToString()
            txtFullName.Text = myDataUser.Tables(0).Rows(0).Item("FullName").ToString()
            txtEmail.Text = myDataUser.Tables(0).Rows(0).Item("Email").ToString()
            txtPhone.Text = myDataUser.Tables(0).Rows(0).Item("Phone").ToString()
            ddlMarkUp.SelectedValue = Convert.ToInt32(myDataUser.Tables(0).Rows(0).Item("MarkUp"))
            ddlPrice.SelectedValue = Convert.ToInt32(myDataUser.Tables(0).Rows(0).Item("Price"))
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindApp()
        ddlAppId.Items.Clear()
        Try
            ddlAppId.DataSource = publicCfg.GetListData("SELECT Id, UPPER(Name) AS Name FROM Applications ORDER BY Name ASC")
            ddlAppId.DataTextField = "Name"
            ddlAppId.DataValueField = "Id"
            ddlAppId.DataBind()

            If ddlAppId.Items.Count > 0 Then
                ddlAppId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindRole()
        ddlRole.Items.Clear()
        Try
            ddlRole.DataSource = publicCfg.GetListData("SELECT Id, UPPER(Name) AS NameText FROM Roles ORDER BY Name ASC")
            ddlRole.DataTextField = "NameText"
            ddlRole.DataValueField = "Id"
            ddlRole.DataBind()

            If ddlRole.Items.Count > 0 Then
                ddlRole.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindLevel()
        ddlLevel.Items.Clear()
        Try
            ddlLevel.DataSource = publicCfg.GetListData("SELECT Id, UPPER(Name) AS NameText FROM MembersLevel ORDER BY Name ASC")
            ddlLevel.DataTextField = "NameText"
            ddlLevel.DataValueField = "Id"
            ddlLevel.DataBind()

            ddlLevel.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindStore()
        ddlStore.Items.Clear()
        Try
            ddlStore.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Stores WHERE Active=1 ORDER BY Name ASC")
            ddlStore.DataTextField = "NameText"
            ddlStore.DataValueField = "Id"
            ddlStore.DataBind()

            If ddlStore.Items.Count > 0 Then
                ddlStore.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        Call MessageErrorPassword(False, String.Empty)

        ddlAppId.CssClass = "form-select "
        txtUserName.CssClass = "form-control "
        txtPassword.CssClass = "form-control "
        txtCPassword.CssClass = "form-control "
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

    Private Sub MessageErrorPassword(Show As Boolean, Msg As String)
        divErrorPassword.Visible = False : msgErrorPassword.InnerText = Msg
        ' If Show = True Then : divErrorPassword.Visible = True : End If
        If Show = True Then
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"')", True)
        End If
    End Sub
End Class
