Imports System.Data
Partial Class Setting_CustomDiscount_Detail
    Inherits System.Web.UI.Page

    Dim publicCfg As New PublicConfig

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Session("RoleName") = "Administrator" Then
            Response.Redirect("~/setting/customdiscount", False)
            Exit Sub
        End If

        If Session("customDiscountDetail") = "" Then
            Response.Redirect("~/setting/customdiscount", False)
            Exit Sub
        End If

        lblId.Text = UCase(Session("customDiscountDetail")).ToString()
        If Not IsPostBack Then
            Call BackColor()

            Call BindData(lblId.Text)
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        Call BindBlindType(ddlDesign.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        Call BackColor()
        Try
            If msgError.InnerText = "" Then
                lblDesignId.Text = UCase(ddlDesign.SelectedValue).ToString()
                lblBlindId.Text = UCase(ddlBlindId.SelectedValue).ToString()
                txtFormula.Text = ddlFieldName.SelectedValue & txtFormula.Text

                sdsPage.Update()

                Dim userId As String = UCase(Session("UserId")).ToString()
                publicCfg.InsertActivity(userId, Page.Title, "UPDATE SURCHARGE")

                Response.Redirect("~/setting/customdiscount", False)
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/customdiscount", False)
    End Sub

    Private Sub BindData(Id As String)
        Try
            Dim myData As  DataSet = publicCfg.GetListData("SELECT * FROM CustomDiscount WHERE Id = '" + Id + "'")
            If myData.Tables(0).Rows.Count = 0 Then
                Response.Redirect("~/setting/customdiscount", False)
                Exit Sub
            End If

            Dim designId As String = myData.Tables(0).Rows(0).Item("DesignId").ToString()

            Call BindDesignType()
            Call BindBlindType(designId)
            Call BindFieldName()

            ddlDesign.SelectedValue = designId
            ddlBlindId.SelectedValue = myData.Tables(0).Rows(0).Item("BlindId").ToString()
            ddlBlindNo.SelectedValue = myData.Tables(0).Rows(0).Item("BlindNo").ToString()
            txtName.Text = myData.Tables(0).Rows(0).Item("Name").ToString()
            ddlFieldName.SelectedValue = myData.Tables(0).Rows(0).Item("FieldName").ToString()
            txtFormula.Text = myData.Tables(0).Rows(0).Item("Formula").ToString().Replace(ddlFieldName.SelectedValue, "")
            txtCharge.Text = myData.Tables(0).Rows(0).Item("Charge").ToString()
            ' Pastikan txtFromDate dan txtToDate adalah TextBox dengan TextMode="Date"
            If myData IsNot Nothing AndAlso myData.Tables.Count > 0 AndAlso myData.Tables(0).Rows.Count > 0 Then
                Dim row As DataRow = myData.Tables(0).Rows(0)

                If row("FromDate") IsNot DBNull.Value AndAlso TypeOf row("FromDate") Is DateTime Then
                    txtFromDate.Text = DirectCast(row("FromDate"), DateTime).ToString("yyyy-MM-dd")
                End If

                If row("ToDate") IsNot DBNull.Value AndAlso TypeOf row("ToDate") Is DateTime Then
                    txtToDate.Text = DirectCast(row("ToDate"), DateTime).ToString("yyyy-MM-dd")
                End If
            End If
            txtDescription.Text = myData.Tables(0).Rows(0).Item("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData.Tables(0).Rows(0).Item("Active"))
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindDesignType()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "NameText"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindBlindType(DesignId As String)
        ddlBlindId.Items.Clear()
        Try
            If Not DesignId = "" Then
                ddlBlindId.DataSource = publicCfg.GetListData("SELECT *, UPPER(Name) AS NameText FROM Blinds WHERE DesignId = '" + DesignId + "' AND Active=1 ORDER BY Name ASC")
                ddlBlindId.DataTextField = "NameText"
                ddlBlindId.DataValueField = "Id"
                ddlBlindId.DataBind()

                If ddlBlindId.Items.Count > 0 Then
                    ddlBlindId.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BindFieldName()
        ddlFieldName.Items.Clear()
        Try
            ddlFieldName.DataSource = publicCfg.GetListData("SELECT COLUMN_NAME AS FieldName FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'view_details'")
            ddlFieldName.DataTextField = "FieldName"
            ddlFieldName.DataValueField = "FieldName"
            ddlFieldName.DataBind()

            If ddlFieldName.Items.Count > 0 Then
                ddlFieldName.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            Call MessageError(True, ex.ToString())
        End Try
    End Sub

    Private Sub BackColor()
        Call MessageError(False, String.Empty)

        ddlDesign.BackColor = Drawing.Color.Empty
        ddlBlindId.BackColor = Drawing.Color.Empty
        ddlBlindNo.BackColor = Drawing.Color.Empty
        ddlFieldName.BackColor = Drawing.Color.Empty
        txtFormula.BackColor = Drawing.Color.Empty
        txtCharge.BackColor = Drawing.Color.Empty
        txtDescription.BackColor = Drawing.Color.Empty
        ddlActive.BackColor = Drawing.Color.Empty
    End Sub

    Private Sub MessageError(Show As Boolean, Msg As String)
        divError.Visible = False : msgError.InnerText = Msg
        If Show = True Then
            divError.Visible = True
        End If
    End Sub
End Class
