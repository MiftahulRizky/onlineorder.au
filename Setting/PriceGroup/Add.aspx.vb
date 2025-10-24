Partial Class Setting_PriceGroup_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call BackColor()

            Call BindDataDesign()
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlDesign.SelectedValue = "" Then
                Call MessageError(True, "DESIGN TYPE IS REQUIRED !")
                ddlDesign.BackColor = Drawing.Color.Red
                ddlDesign.Focus()
                Exit Sub
            End If

            If txtName.Text = "" Then
                Call MessageError(True, "NAME IS REQUIRED !")
                txtName.BackColor = Drawing.Color.Red
                txtName.Focus()
                Exit Sub
            End If

            If ddlActive.SelectedValue = "" Then
                Call MessageError(True, "ACTIVE IS REQUIRED !")
                ddlActive.BackColor = Drawing.Color.Red
                ddlActive.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT PRICE GROUP. NAME : " & txtName.Text)

                Response.Redirect("~/setting/pricegroup", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/pricegroup", False)
    End Sub

    Private Sub BindDataDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 1 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlDesign.BackColor = Drawing.Color.Empty
        txtName.BackColor = Drawing.Color.Empty
        txtDescription.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
