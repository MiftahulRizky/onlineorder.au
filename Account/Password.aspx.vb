Partial Class Account_Password
    Inherits Page

    Dim settingCfg As New SettingConfig
    Dim mailCfg As New MailConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'cardTitle.InnerText = "CHANGE YOUR PASSWORD"
        'If Session("resetLogin") = True Then
        '    cardTitle.InnerText = "YOU MUST CHANGE YOUR TEMPORARY PASSWORD !"
        'End If
        If Not IsPostBack Then
            Call BackColor()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtNewPass.Text = "" Then
                txtNewPass.BackColor = Drawing.Color.Red
                txtNewPass.Focus()
                Call MessageError(True, "PASSWORD IS REQUIRED !")
                Exit Sub
            End If

            If Not txtCNewPass.Text = txtNewPass.Text Then
                txtCNewPass.BackColor = Drawing.Color.Red
                txtCNewPass.Focus()
                Call MessageError(True, "KATA SANDI TIDKA SAMA !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                lblLoginId.Text = UCase(Session("LoginId")).ToString()
                lblPasswordHash.Text = settingCfg.Encrypt(txtNewPass.Text)
                sdsPage.Update()

                Dim thisScript As String = "window.onload = function() { showSuccess(); };"
                ClientScript.RegisterStartupScript(Me.GetType(), "showSuccess", thisScript, True)
                Session.Clear()
                Exit Sub
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact IT at reza@bigblinds.co.id")
                If Session("RoleName") = "Customer" Then
                    Call MessageError(True, "Please contact Customer Service at customerservice@lifestyleshutters.com.au")
                End If
                mailCfg.MailError(Page.Title, "btnSubmit_Click", Session("LoginId"), ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        txtNewPass.BackColor = Drawing.Color.Empty
        txtCNewPass.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = Show : msgError.InnerText = Msg
    End Sub
End Class
