Imports System.Data
Imports System.Globalization
Imports System.IO
Imports System.Web.UI
Imports System.Web
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Security.Cryptography

Partial Class Account_Login
    Inherits Page

    Dim publicCfg As New PublicConfig
    Dim settingCfg As New SettingConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Session.Clear()
        If Not IsPostBack Then
            BackColor()
            CheckSessionStates()

            ' Dim pswd As String = "0+CHL1SJ4KLKQlDj28II5GJ8d0rixamuhRksskf1JYI="
            ' Dim pswdDecrypt As String = publicCfg.Decrypt(pswd)
            ' call MessageError(True, String.Empty, pswdDecrypt)
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
                Dim memberData As DataSet = publicCfg.GetListData("SELECT * FROM view_auth WHERE UserName = '" + txtUserLogin.Text + "'")
                
                If memberData.Tables(0).Rows.Count = 0 Then
                    Call MessageError(True, "txtUserLogin", "USERNAME NOT FOUND !")
                    Exit Sub
                End If

                '#check duplikasi username
                If memberData.Tables(0).Rows.Count > 1 Then
                    Call MessageError(True, "txtUserLogin", "USERNAME NOT FOUND !")
                    Exit Sub
                End If


                Dim loginId As String = memberData.Tables(0).Rows(0).Item("Id").ToString()
                Dim userName As String = memberData.Tables(0).Rows(0).Item("UserName").ToString()
                Dim password As String = memberData.Tables(0).Rows(0).Item("Password").ToString()
                Dim memberActive As Boolean = memberData.Tables(0).Rows(0).Item("Active")
                Dim customerId As String = memberData.Tables(0).Rows(0).Item("CustomerId").ToString()

                Dim appId As String = memberData.Tables(0).Rows(0).Item("ApplicationId").ToString()
                Dim appActive As Boolean = memberData.Tables(0).Rows(0).Item("AppActive")

                Dim roleActive As Boolean = memberData.Tables(0).Rows(0).Item("RoleActive")
                Dim levelActive As Boolean = memberData.Tables(0).Rows(0).Item("LevelActive")


                If publicCfg.Encrypt(txtPassword.Text) <> password Then
                    Call MessageError(True, "txtPassword", "YOUR PASSWORD IS WRONG !")
                    Exit Sub
                End If

                If appActive = False Then
                    Response.Redirect("~/error/maintenance", False)
                    Exit Sub
                End If


                If memberActive = False Then
                    Call MessageError(True, String.Empty, "YOUR ACCOUNT (LOGIN) IS BEING BLOCKED !")
                    Exit Sub
                End If

                If msgError.InnerText = "" Then
                    Call MessageError(True, String.Empty, "masuk")
                    settingCfg.UpdateSession(lblDeviceId.Text, loginId)
                    Session.Add("IsLoggedIn", True)
                    Session.Add("LoginId", UCase(loginId).ToString())
                    Session.Add("ApplicationId", UCase(appId).ToString())
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

    Private Sub CheckSessionStates()
        If Request.Cookies("deviceId") IsNot Nothing Then
            lblDeviceId.Text = Request.Cookies("deviceId").Value
            Dim checkSession As Integer = publicCfg.GetItemData_Integer("SELECT COUNT(*) FROM Sessions WHERE Id = '" + UCase(lblDeviceId.Text).ToString() + "'")
            If checkSession = 1 Then
                Dim loginId As String = publicCfg.GetItemData("SELECT LoginId FROM Sessions WHERE Id = '" + UCase(lblDeviceId.Text).ToString() + "'")
                If Not loginId = "" Then
                    Dim appId As String = publicCfg.GetItemData("SELECT ApplicationId FROM CustomerLogins WHERE Id = '" + UCase(loginId).ToString() + "'")
                    Dim userName As String = publicCfg.GetItemData("SELECT UserName FROM CustomerLogins WHERE Id = '" + UCase(loginId).ToString() + "'")

                    Session.Add("IsLoggedIn", True)
                    Session.Add("LoginId", UCase(loginId).ToString())
                    Session.Add("ApplicationId", UCase(appId).ToString())
                    Session.Add("UserName", userName)

                    Response.Redirect("~/", False)
                    Exit Sub
                Else
                    lblDeviceId.Text = settingCfg.InsertSession()
                    Dim deviceCookie As New HttpCookie("deviceId", UCase(lblDeviceId.Text).ToString())
                    deviceCookie.Expires = DateTime.Now.AddMonths(1)
                    Response.Cookies.Add(deviceCookie)
                    Exit Sub
                End If
            Else
                lblDeviceId.Text = settingCfg.InsertSession()
                Dim deviceCookie As New HttpCookie("deviceId", UCase(lblDeviceId.Text).ToString())
                deviceCookie.Expires = DateTime.Now.AddMonths(1)
                Response.Cookies.Add(deviceCookie)
                Exit Sub
            End If
        Else
            lblDeviceId.Text = settingCfg.InsertSession()
            Dim deviceCookie As New HttpCookie("deviceId", UCase(lblDeviceId.Text).ToString())
            deviceCookie.Expires = DateTime.Now.AddMonths(1)
            Response.Cookies.Add(deviceCookie)
            Exit Sub
        End If
    End Sub
End Class
