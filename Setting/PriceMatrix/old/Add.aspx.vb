
Partial Class Setting_PriceMatrix_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Call BackColor()

            Call BindDataDesign()
            Call BindDataPriceGroup(ddlDesign.SelectedValue)
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BackColor()

        Call BindDataPriceGroup(ddlDesign.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlPriceGroup.SelectedValue = "" Then
                Call MessageError(True, "PRICE GROUP IS REQUIRED !")
                ddlPriceGroup.BackColor = Drawing.Color.Red
                ddlPriceGroup.Focus()
                Exit Sub
            End If

            If ddlType.SelectedValue = "" Then
                Call MessageError(True, "TYPE IS REQUIRED !")
                ddlType.BackColor = Drawing.Color.Red
                ddlType.Focus()
                Exit Sub
            End If

            If Not txtWidth.Text = "" Then
                If Not IsNumeric(txtWidth.Text) Then
                    Call MessageError(True, "WIDTH SHOULD BE NUMERIC !")
                    txtWidth.BackColor = Drawing.Color.Red
                    txtWidth.Focus()
                    Exit Sub
                End If
            End If

            If Not txtDrop.Text = "" Then
                If Not IsNumeric(txtDrop.Text) Then
                    Call MessageError(True, "DROP / HEIGHT SHOULD BE NUMERIC !")
                    txtDrop.BackColor = Drawing.Color.Red
                    txtDrop.Focus()
                    Exit Sub
                End If
            End If

            If txtCost.Text = "" Then
                Call MessageError(True, "COST IS REQUIRED !")
                txtCost.BackColor = Drawing.Color.Red
                txtCost.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                lblPriceGroup.Text = UCase(ddlPriceGroup.SelectedValue).ToString()

                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                Dim description As String = ddlPriceGroup.SelectedItem.Text & " " & ddlType.SelectedValue & " " & txtCost.Text
                publicCfg.InsertActivity(userId, Page.Title, "INSERT NEW PRICE. PRICE DESC : " & description)

                Response.Redirect("~/setting/pricematrix", False)
            End If
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at reza@bigblinds.co.id")
            publicCfg.MailError(Session("UserId"), Page.Title, "gvList_PageIndexChanging", ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/pricematrix/", False)
    End Sub

    Private Sub BindDataDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) + ' | ' + Company AS NameText FROM Designs WHERE Active=1 ORDER BY Name ASC")
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at support@onlineorder.au")
            publicCfg.MailError(Session("UserId"), Page.Title, "BindDataDesign", ex.ToString())
        End Try
    End Sub

    Private Sub BindDataPriceGroup(DesignId As String)
        ddlPriceGroup.Items.Clear()
        Try
            If Not DesignId = "" Then
                ddlPriceGroup.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM PricesGroup WHERE DesignId='" + DesignId + "' AND Active=1 ORDER BY Name ASC")
                ddlPriceGroup.DataTextField = "NameText"
                ddlPriceGroup.DataValueField = "Id"
                ddlPriceGroup.DataBind()
            End If

            If ddlPriceGroup.Items.Count > 0 Then
                ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, "Please contact our IT team at reza@bigblinds.co.id")
            publicCfg.MailError(Session("UserId"), Page.Title, "gvList_PageIndexChanging", ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)
        ddlPriceGroup.BackColor = Drawing.Color.Empty
        ddlType.BackColor = Drawing.Color.Empty
        txtWidth.BackColor = Drawing.Color.Empty
        txtDrop.BackColor = Drawing.Color.Empty
        txtCost.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False
        msgError.InnerText = Msg
        If Show = True Then
            divError.Visible = True
        End If
    End Sub

End Class
