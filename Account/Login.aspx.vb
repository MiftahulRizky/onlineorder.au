Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Web.UI
Imports System.Web
Imports System.Web.UI.WebControls
Partial Class Account_Login
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Session.Clear()
        If Not IsPostBack Then
            Call BackColor()
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtUserLogin.Text = "" Then
                Call MessageError(True,"txtUserLogin", "USERNAME IS REQUIRED !")
                ' txtUserLogin.Focus()
                txtUserLogin.CssClass = "form-control  is-invalid"
                Exit Sub
            End If

            If txtPassword.Text = "" Then
                Call MessageError(True,"txtPassword", "PASSWORD IS REQUIRED !")
                ' txtPassword.Focus()
                txtPassword.CssClass = "form-control  is-invalid"
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim memberData As DataSet = publicCfg.GetListData("SELECT * FROM view_memberships WHERE UserName = '" + txtUserLogin.Text + "'")
                Dim appId As String = ""
                Dim userId As String = ""
                Dim userName As String = memberData.Tables(0).Rows(0).Item("UserName").ToString()
                Dim password As String = ""
                Dim memberActive As Boolean = False
                Dim storeActive As Boolean = False

                If memberData.Tables(0).Rows.Count > 0 Then
                    appId = memberData.Tables(0).Rows(0).Item("ApplicationId").ToString()
                    userId = memberData.Tables(0).Rows(0).Item("UserId").ToString()
                    password = memberData.Tables(0).Rows(0).Item("Password").ToString()
                    memberActive = memberData.Tables(0).Rows(0).Item("LockoutEnabled")
                    storeActive = memberData.Tables(0).Rows(0).Item("StoreActive")
                End If

                If userId = "" Then
                    Call MessageError(True, "txtUserLogin", "USERNAME NOT FOUND !")
                    Exit Sub
                End If

                If publicCfg.Encrypt(txtPassword.Text) <> password Then
                    Call MessageError(True, "txtPassword", "YOUR PASSWORD IS WRONG !")
                    Exit Sub
                End If

                If memberActive = True Then
                    Call MessageError(True, String.Empty, "YOUR ACCOUNT (LOGIN) IS BEING BLOCKED !")
                    Exit Sub
                End If

                If storeActive = False Then
                    Call MessageError(True, String.Empty, "YOUR ACCOUNT (STORE) IS BEING BLOCKED !")
                    Exit Sub
                End If

                If msgError.InnerText = "" Then
                    Session.Add("IsLoggedIn", True)
                    Session.Add("ApplicationId", UCase(appId).ToString())
                    Session.Add("UserId", UCase(userId).ToString())
                    Session.Add("UserName", userName)
                    Response.Redirect("~/", False)
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, String.Empty, "Please contact our IT team at support@onlineorder.au")
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, input As String, Msg As String)
        divError.Visible = False 
        msgError.InnerText = Msg
        If Show = True Then 
            ' divError.Visible = True 
            Dim escapedMsg As String = HttpUtility.JavaScriptStringEncode(Msg)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "Script", "showMessageError('"& escapedMsg &"', '"& input &"')", True)
        End If
    End Sub

    Private Sub BackColor()
        Call MessageError(False,String.Empty, String.Empty)
        txtUserLogin.CssClass = "form-control "
        txtPassword.CssClass = "form-control "
    End Sub
End Class
