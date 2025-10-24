Imports System.Data

Partial Class Account_Default
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Session("UserId") = "" Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        lblUserId.Text = UCase(Session("UserId")).ToString()
        If Not IsPostBack Then
            Call BindData(lblUserId.Text)
        End If
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Protected Sub btnEmail_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If txtEmail.Text = "" Then
                Call MessageError(True, "EMAIL ADDRESS IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsEmail.Update()

                Response.Redirect("~/account/", False)
                Exit Sub
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(lblUserId.Text, Page.Title, "btnEmail_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                sdsUsers.Update()

                Session("UserName") = txtUserName.Text

                Response.Redirect("~/account/", False)
                Exit Sub
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(lblUserId.Text, Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindData(UserId As String)
        Call MessageError(False, String.Empty)
        Try
            Dim myData As DataSet = publicCfg.GetListData("SELECT * FROM view_memberships WHERE UserId = '" + UCase(UserId).ToString() + "'")

            txtUserName.Text = myData.Tables(0).Rows(0).Item("UserName").ToString()
            txtFullName.Text = myData.Tables(0).Rows(0).Item("FullName").ToString()
            txtEmail.Text = myData.Tables(0).Rows(0).Item("UserEmail").ToString()
            lblEmailOld.Text = myData.Tables(0).Rows(0).Item("UserEmail").ToString()

            Dim roleId As String = myData.Tables(0).Rows(0).Item("RoleId").ToString()
            Dim levelId As String = myData.Tables(0).Rows(0).Item("LevelId").ToString()

            Call BindRole(roleId)
            Call BindLevel(levelId)
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(UserId, Page.Title, "BindData", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindRole(RoleId As String)
        ddlRole.Items.Clear()
        Try
            ddlRole.DataSource = publicCfg.GetListData("SELECT * FROM Roles WHERE Id='" + UCase(RoleId).ToString() + "' ORDER BY Name ASC")
            ddlRole.DataTextField = "Name"
            ddlRole.DataValueField = "Id"
            ddlRole.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(lblUserId.Text, Page.Title, "BindRole", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub BindLevel(LevelId As String)
        ddlLevel.Items.Clear()
        Try
            ddlLevel.DataSource = publicCfg.GetListData("SELECT * FROM LevelMembers WHERE Id='" + UCase(LevelId).ToString() + "' ORDER BY Name ASC")
            ddlLevel.DataTextField = "Name"
            ddlLevel.DataValueField = "Id"
            ddlLevel.DataBind()
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(lblUserId.Text, Page.Title, "BindLevel", ex.ToString())
            End If
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
