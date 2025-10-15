Partial Class Account_Password
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        lblUserId.Text = UCase(Session("UserId")).ToString()

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
                Call MessageError(True, "NEW PASSWORD IS REQUIRED !")
                Exit Sub
            End If

            If txtCNewPass.Text <> txtNewPass.Text Then
                txtCNewPass.BackColor = Drawing.Color.Red
                txtCNewPass.Focus()
                Call MessageError(True, "PASSWORD ARE NOT THE SAME !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                lblPassword.Text = publicCfg.Encrypt(txtNewPass.Text)
                sdsPage.Update()

                publicCfg.InsertActivity(lblUserId.Text, Page.Title, "CHANGE PASSWORD")

                Session.Clear()
                Response.Redirect("~/", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Administrator" Then
                Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
                publicCfg.MailError(lblUserId.Text, Page.Title, "btnSubmit_Click", ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/account/", False)
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtNewPass.BackColor = Drawing.Color.Empty
        txtCNewPass.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
