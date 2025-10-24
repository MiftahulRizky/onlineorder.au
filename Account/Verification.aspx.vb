
Partial Class Account_Verification
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Call MessageError(False, String.Empty)
            Call MessageSuccess(False, String.Empty)
        End If
        lblUserId.Text = UCase(Session("UserIdVerify")).ToString()
        thisEmail.InnerText = Session("EmailVerify")
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/", False)
    End Sub

    Protected Sub btnVerify_Click(sender As Object, e As EventArgs)
        Try

            Dim i1 As String = Request.Form("i1").ToString()
            Dim i2 As String = Request.Form("i2").ToString()
            Dim i3 As String = Request.Form("i3").ToString()
            Dim i4 As String = Request.Form("i4").ToString()
            Dim i5 As String = Request.Form("i5").ToString()
            Dim i6 As String = Request.Form("i6").ToString()

            Dim textVerify As String = i1 & i2 & i3 & i4 & i5 & i6

            Dim forgotCode As String = publicCfg.GetItemData("SELECT ForgotCode FROM Memberships WHERE UserId = '" + lblUserId.Text + "'")

            If forgotCode <> textVerify Then
                Call MessageError(True, "GAK SAMA !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Session.Clear()

                Dim appId As String = publicCfg.GetItemData("SELECT ApplicationId FROM Memberships WHERE UserId = '" + lblUserId.Text + "'")
                Dim userName As String = publicCfg.GetItemData("SELECT UserName FROM Memberships WHERE UserId = '" + lblUserId.Text + "'")

                Session.Add("IsLoggedIn", True)
                Session.Add("ApplicationId", UCase(appId).ToString())
                Session.Add("UserId", lblUserId.Text)
                Session.Add("UserName", userName)

                lblPasswordHash.Text = publicCfg.Encrypt("123456")
                sdsPage.Update()

                Dim msgSuccess As String = "Your password has been changed to <b>123456</b>"
                msgSuccess += "<br /><br />"
                msgSuccess += "You will be redirected to the change password page in 10 seconds."

                Call MessageSuccess(True, msgSuccess)

                btnVerify.Visible = False : btnCancel.Visible = False

                Dim meta As New HtmlMeta()
                meta.HttpEquiv = "Refresh"
                meta.Content = "10;url=password"
                Page.Controls.Add(meta)
            End If
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
            publicCfg.MailError(lblUserId.Text, Page.Title, "btnVerify_Click", ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub

    Private Sub MessageSuccess(Show As Boolean, Msg As String)
        divSuccess.Visible = False : msgSuccess.InnerHtml = Msg
        If Show = True Then : divSuccess.Visible = True : End If
    End Sub
End Class
