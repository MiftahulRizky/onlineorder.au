Partial Class Setting_Chain_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/chain", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If txtId.Text = "" Then
                Call MessageError(True, "CHAIN ID IS REQUIRED !")
                txtId.BackColor = Drawing.Color.Red
                txtId.Focus()
                Exit Sub
            End If

            If txtId.Text = publicCfg.GetItemData("SELECT Id FROM Chains WHERE Id='" + txtId.Text + "'") Then
                Call MessageError(True, "CHAIN ID ALREADY EXISTS !")
                txtId.BackColor = Drawing.Color.Red
                txtId.Focus()
                Exit Sub
            End If

            If txtName.Text = "" Then
                Call MessageError(True, "CHAIN NAME IS REQUIRED !")
                txtName.BackColor = Drawing.Color.Red
                txtName.Focus()
                Exit Sub
            End If

            If txtColour.Text = "" Then
                Call MessageError(True, "CHAIN COLOUR IS REQUIRED !")
                txtColour.BackColor = Drawing.Color.Red
                txtColour.Focus()
                Exit Sub
            End If

            If ddlLength.Text = "" Then
                Call MessageError(True, "CHAIN LENGTH IS REQUIRED !")
                ddlLength.BackColor = Drawing.Color.Red
                ddlLength.Focus()
                Exit Sub
            End If

            If ddlActive.SelectedValue = "" Then
                Call MessageError(True, "CHAIN ACTIVE IS REQUIRED !")
                ddlActive.BackColor = Drawing.Color.Red
                ddlActive.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT NEW CHAIN. NAME : " & lblName.Text)

                Response.Redirect("~/setting/chain", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/chain", False)
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        txtId.BackColor = Drawing.Color.Empty
        txtName.BackColor = Drawing.Color.Empty
        txtColour.BackColor = Drawing.Color.Empty
        ddlLength.BackColor = Drawing.Color.Empty
        txtDescription.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
