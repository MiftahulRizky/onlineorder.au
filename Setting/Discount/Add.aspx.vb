Partial Class Setting_Discount_Add
    Inherits Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/blind", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            Call MessageError(False, String.Empty)

            Call BindStore()
            Call BindPriceGroup()
        End If
    End Sub

    Protected Sub btnCheckStore_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If Not txtStoreId.Text = "" Then
                If txtStoreId.Text = publicCfg.GetItemData("SELECT Id FROM Stores WHERE Id = '" + txtStoreId.Text + "'") Then
                    ddlStoreId.SelectedValue = txtStoreId.Text
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If ddlStoreId.SelectedValue = "" And txtStoreId.Text = "" Then
                Call MessageError(True, "STORE ACCOUNT IS REQUIRED !")
                ddlStoreId.BackColor = Drawing.Color.Red
                ddlStoreId.Focus()
                Exit Sub
            End If

            If Not ddlStoreId.SelectedValue = "" And Not txtStoreId.Text = "" Then
                Call MessageError(True, "PILIH SATU AJA !")
                ddlStoreId.BackColor = Drawing.Color.Red
                ddlStoreId.Focus()
                txtStoreId.BackColor = Drawing.Color.Red
                txtStoreId.Focus()
                Exit Sub
            End If

            If ddlStoreId.SelectedValue = "" And Not txtStoreId.Text = "" Then
                If Not txtStoreId.Text = publicCfg.GetItemData("SELECT Id FROM Stores WHERE Id = '" + txtStoreId.Text + "'") Then
                    Call MessageError(True, "STORE ACCOUNT NOT FOUND !")
                    txtStoreId.BackColor = Drawing.Color.Red
                    txtStoreId.Focus()
                End If
            End If

            If ddlPriceGroupId.SelectedValue = "" Then
                Call MessageError(True, "PRICE GROUP IS REQUIRED !")
                ddlPriceGroupId.BackColor = Drawing.Color.Red
                ddlPriceGroupId.Focus()
                Exit Sub
            End If

            If txtDiscount.Text = "" Then
                Call MessageError(True, "")
                txtDiscount.BackColor = Drawing.Color.Red
                txtDiscount.Focus()
                Exit Sub
            End If

            If Not IsNumeric(txtDiscount.Text) Then
                Call MessageError(True, "")
                txtDiscount.BackColor = Drawing.Color.Red
                txtDiscount.Focus()
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                sdsPage.Insert()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "INSERT NEW DISCOUNT. STORE ID : " & ddlStoreId.SelectedValue)

                Response.Redirect("~/setting/discount", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/discount", False)
    End Sub

    Private Sub BindStore()
        ddlStoreId.Items.Clear()
        Try
            ddlStoreId.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Stores WHERE Active=1 ORDER BY Name ASC")
            ddlStoreId.DataTextField = "NameText"
            ddlStoreId.DataValueField = "Id"
            ddlStoreId.DataBind()

            If ddlStoreId.Items.Count > 0 Then
                ddlStoreId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindPriceGroup()
        ddlPriceGroupId.Items.Clear()
        Try
            ddlPriceGroupId.DataSource = publicCfg.GetListData("SELECT *, UPPER(Designs.Name) + ', ' + UPPER(PricesGroup.Name) AS NameText FROM PricesGroup INNER JOIN Designs ON PricesGroup.DesignId = Designs.Id ORDER BY Designs.Name, PricesGroup.Name ASC")
            ddlPriceGroupId.DataTextField = "NameText"
            ddlPriceGroupId.DataValueField = "Id"
            ddlPriceGroupId.DataBind()

            If ddlPriceGroupId.Items.Count > 0 Then
                ddlPriceGroupId.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlStoreId.BackColor = Drawing.Color.Empty
        txtStoreId.BackColor = Drawing.Color.Empty
        ddlPriceGroupId.BackColor = Drawing.Color.Empty
        txtDiscount.BackColor = Drawing.Color.Empty
        txtDescription.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then : divError.Visible = True : End If
    End Sub
End Class
