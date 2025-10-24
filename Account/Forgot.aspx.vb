Partial Class Account_Forgot
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Session.Clear()
        If Not IsPostBack Then
            Call MessageError(False, String.Empty)
            Call MessageSuccess(False, String.Empty)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call MessageError(False, String.Empty)
        Call MessageSuccess(False, String.Empty)
        txtEmail.BackColor = Drawing.Color.Empty
        Dim userId As String = ""

        Try
            If txtEmail.Text = "" Then
                Call MessageError(True, "EMAIL ADDRESS IS REQUIRED !")
                txtEmail.BackColor = Drawing.Color.Red
                txtEmail.Focus()
                Exit Sub
            End If

            userId = publicCfg.GetItemData("SELECT UserId FROM Users WHERE Email = '" + txtEmail.Text + "'")
            If userId = "" Then
                Call MessageError(True, "EMAIL NOT REGISTERED !")
                txtEmail.BackColor = Drawing.Color.Red
                txtEmail.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                lblCode.Text = GetRandomCode()
                lblUserId.Text = UCase(userId).ToString()

                Session("UserIdVerify") = lblUserId.Text
                Session("EmailVerify") = txtEmail.Text

                Call publicCfg.MailVerify(lblUserId.Text, lblCode.Text)

                sdsPage.Update()
                Response.Redirect("~/account/verification", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub

    Private Sub MessageSuccess(Show As Boolean, Msg As String)
        divSuccess.Visible = False : msgSuccess.InnerText = Msg
        If Show = True Then : divSuccess.Visible = True : End If
    End Sub

    Protected Function GetRandomCode() As String
        Dim result As String = ""

        Dim numbers As String = "1234567890"
        Dim characters As String = numbers

        Dim length As Integer = 6
        Dim otp As String = String.Empty
        For i As Integer = 0 To length - 1
            Dim character As String = String.Empty
            Do
                Dim index As Integer = New Random().Next(0, characters.Length)
                character = characters.ToCharArray()(index).ToString()
            Loop While otp.IndexOf(character) <> -1
            otp += character
        Next
        result = otp

        Return result
    End Function
End Class
